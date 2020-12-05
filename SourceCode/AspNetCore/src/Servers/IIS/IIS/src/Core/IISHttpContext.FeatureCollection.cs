// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Server.IIS.Core.IO;
using Microsoft.AspNetCore.WebUtilities;

namespace Microsoft.AspNetCore.Server.IIS.Core
{
    internal partial class IISHttpContext : IFeatureCollection,
                                            IHttpRequestFeature,
                                            IHttpResponseFeature,
                                            IHttpUpgradeFeature,
                                            IHttpRequestLifetimeFeature,
                                            IHttpAuthenticationFeature,
                                            IServerVariablesFeature,
                                            IHttpBufferingFeature,
                                            ITlsConnectionFeature,
                                            IHttpBodyControlFeature
    {
        // NOTE: When feature interfaces are added to or removed from this HttpProtocol implementation,
        // then the list of `implementedFeatures` in the generated code project MUST also be updated.

        private int _featureRevision;
        private string _httpProtocolVersion = null;
        private X509Certificate2 _certificate;

        private List<KeyValuePair<Type, object>> MaybeExtra;

        public void ResetFeatureCollection()
        {
            Initialize();
            MaybeExtra?.Clear();
            _featureRevision++;
        }

        private object ExtraFeatureGet(Type key)
        {
            if (MaybeExtra == null)
            {
                return null;
            }
            for (var i = 0; i < MaybeExtra.Count; i++)
            {
                var kv = MaybeExtra[i];
                if (kv.Key == key)
                {
                    return kv.Value;
                }
            }
            return null;
        }

        private void ExtraFeatureSet(Type key, object value)
        {
            if (MaybeExtra == null)
            {
                MaybeExtra = new List<KeyValuePair<Type, object>>(2);
            }

            for (var i = 0; i < MaybeExtra.Count; i++)
            {
                if (MaybeExtra[i].Key == key)
                {
                    MaybeExtra[i] = new KeyValuePair<Type, object>(key, value);
                    return;
                }
            }
            MaybeExtra.Add(new KeyValuePair<Type, object>(key, value));
        }

        string IHttpRequestFeature.Protocol
        {
            get
            {
                if (_httpProtocolVersion == null)
                {
                    var protocol = HttpVersion;
                    if (protocol.Major == 1 && protocol.Minor == 1)
                    {
                        _httpProtocolVersion = "HTTP/1.1";
                    }
                    else if (protocol.Major == 1 && protocol.Minor == 0)
                    {
                        _httpProtocolVersion = "HTTP/1.0";
                    }
                    else
                    {
                        _httpProtocolVersion = "HTTP/" + protocol.ToString(2);
                    }
                }
                return _httpProtocolVersion;
            }
            set
            {
                _httpProtocolVersion = value;
            }
        }

        string IHttpRequestFeature.Scheme
        {
            get => Scheme;
            set => Scheme = value;
        }

        string IHttpRequestFeature.Method
        {
            get => Method;
            set => Method = value;
        }

        string IHttpRequestFeature.PathBase
        {
            get => PathBase;
            set => PathBase = value;
        }

        string IHttpRequestFeature.Path
        {
            get => Path;
            set => Path = value;
        }

        string IHttpRequestFeature.QueryString
        {
            get => QueryString;
            set => QueryString = value;
        }

        string IHttpRequestFeature.RawTarget
        {
            get => RawTarget;
            set => RawTarget = value;
        }

        IHeaderDictionary IHttpRequestFeature.Headers
        {
            get => RequestHeaders;
            set => RequestHeaders = value;
        }

        Stream IHttpRequestFeature.Body
        {
            get => RequestBody;
            set => RequestBody = value;
        }

        int IHttpResponseFeature.StatusCode
        {
            get => StatusCode;
            set => StatusCode = value;
        }

        string IHttpResponseFeature.ReasonPhrase
        {
            get => ReasonPhrase;
            set => ReasonPhrase = value;
        }

        IHeaderDictionary IHttpResponseFeature.Headers
        {
            get => ResponseHeaders;
            set => ResponseHeaders = value;
        }

        Stream IHttpResponseFeature.Body
        {
            get => ResponseBody;
            set => ResponseBody = value;
        }

        bool IHttpResponseFeature.HasStarted => HasResponseStarted;

        bool IHttpUpgradeFeature.IsUpgradableRequest => true;

        bool IFeatureCollection.IsReadOnly => false;

        int IFeatureCollection.Revision => _featureRevision;

        ClaimsPrincipal IHttpAuthenticationFeature.User
        {
            get => User;
            set => User = value;
        }

        string IServerVariablesFeature.this[string variableName]
        {
            get
            {
                if (string.IsNullOrEmpty(variableName))
                {
                    throw new ArgumentException($"{nameof(variableName)} should be non-empty string");
                }

                // Synchronize access to native methods that might run in parallel with IO loops
                lock (_contextLock)
                {
                    return NativeMethods.HttpTryGetServerVariable(_pInProcessHandler, variableName, out var value) ? value : null;
                }
            }
            set
            {
                if (string.IsNullOrEmpty(variableName))
                {
                    throw new ArgumentException($"{nameof(variableName)} should be non-empty string");
                }

                // Synchronize access to native methods that might run in parallel with IO loops
                lock (_contextLock)
                {
                    NativeMethods.HttpSetServerVariable(_pInProcessHandler, variableName, value);
                }
            }
        }

        object IFeatureCollection.this[Type key]
        {
            get => FastFeatureGet(key);
            set => FastFeatureSet(key, value);
        }

        TFeature IFeatureCollection.Get<TFeature>()
        {
            return (TFeature)FastFeatureGet(typeof(TFeature));
        }

        void IFeatureCollection.Set<TFeature>(TFeature instance)
        {
            FastFeatureSet(typeof(TFeature), instance);
        }

        void IHttpResponseFeature.OnStarting(Func<object, Task> callback, object state)
        {
            OnStarting(callback, state);
        }

        void IHttpResponseFeature.OnCompleted(Func<object, Task> callback, object state)
        {
            OnCompleted(callback, state);
        }

        async Task<Stream> IHttpUpgradeFeature.UpgradeAsync()
        {
            if (!((IHttpUpgradeFeature)this).IsUpgradableRequest)
            {
                throw new InvalidOperationException(CoreStrings.CannotUpgradeNonUpgradableRequest);
            }

            if (_wasUpgraded)
            {
                throw new InvalidOperationException(CoreStrings.UpgradeCannotBeCalledMultipleTimes);
            }
            if (HasResponseStarted)
            {
                throw new InvalidOperationException(CoreStrings.UpgradeCannotBeCalledMultipleTimes);
            }

            _wasUpgraded = true;

            StatusCode = StatusCodes.Status101SwitchingProtocols;
            ReasonPhrase = ReasonPhrases.GetReasonPhrase(StatusCodes.Status101SwitchingProtocols);

            // If we started reading before calling Upgrade Task should be completed at this point
            // because read would return 0 synchronously
            Debug.Assert(_readBodyTask == null || _readBodyTask.IsCompleted);

            // Reset reading status to allow restarting with new IO
            _hasRequestReadingStarted = false;

            // Upgrade async will cause the stream processing to go into duplex mode
            AsyncIO = new WebSocketsAsyncIOEngine(_contextLock, _pInProcessHandler);

            await InitializeResponse(flushHeaders: true);

            return _streams.Upgrade();
        }

        Task<X509Certificate2> ITlsConnectionFeature.GetClientCertificateAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(((ITlsConnectionFeature)this).ClientCertificate);
        }

        unsafe X509Certificate2 ITlsConnectionFeature.ClientCertificate
        {
            get
            {
                if (_certificate == null &&
                    NativeRequest->pSslInfo != null &&
                    NativeRequest->pSslInfo->pClientCertInfo != null &&
                    NativeRequest->pSslInfo->pClientCertInfo->pCertEncoded != null &&
                    NativeRequest->pSslInfo->pClientCertInfo->CertEncodedSize != 0)
                {
                    // Based off of from https://referencesource.microsoft.com/#system/net/System/Net/HttpListenerRequest.cs,1037c8ec82879ba0,references
                    var rawCertificateCopy = new byte[NativeRequest->pSslInfo->pClientCertInfo->CertEncodedSize];
                    Marshal.Copy((IntPtr)NativeRequest->pSslInfo->pClientCertInfo->pCertEncoded, rawCertificateCopy, 0, rawCertificateCopy.Length);
                    _certificate = new X509Certificate2(rawCertificateCopy);
                }

                return _certificate;
            }
            set
            {
                _certificate = value;
            }
        }

        IEnumerator<KeyValuePair<Type, object>> IEnumerable<KeyValuePair<Type, object>>.GetEnumerator() => FastEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => FastEnumerable().GetEnumerator();

        bool IHttpBodyControlFeature.AllowSynchronousIO { get; set; }

        void IHttpBufferingFeature.DisableRequestBuffering()
        {
        }

        void IHttpBufferingFeature.DisableResponseBuffering()
        {
            NativeMethods.HttpDisableBuffering(_pInProcessHandler);
            DisableCompression();
        }

        private void DisableCompression()
        {
            var serverVariableFeature = (IServerVariablesFeature)this;
            serverVariableFeature["IIS_EnableDynamicCompression"] = "0";
        }
    }
}
