// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Server.IntegrationTesting;
using Microsoft.AspNetCore.Server.IntegrationTesting.IIS;
using Xunit;

namespace Microsoft.AspNetCore.Server.IISIntegration.FunctionalTests
{
    /// <summary>
    /// This type just maps collection names to available fixtures
    /// </summary>
    [CollectionDefinition(Name)]
    public class PublishedSitesCollection : ICollectionFixture<PublishedSitesFixture>, ICollectionFixture<ClientCertificateFixture>
    {
        public const string Name = nameof(PublishedSitesCollection);
    }

    public class PublishedSitesFixture : IDisposable
    {
        public PublishedApplicationPublisher InProcessTestSite { get; } = new PublishedApplicationPublisher(Helpers.GetInProcessTestSitesName());
        public PublishedApplicationPublisher OutOfProcessTestSite { get; } = new PublishedApplicationPublisher(Helpers.GetOutOfProcessTestSitesName());

        public void Dispose()
        {
        }

        public IISDeploymentParameters GetBaseDeploymentParameters(HostingModel hostingModel = HostingModel.InProcess)
        {
            var publisher = hostingModel == HostingModel.InProcess ? InProcessTestSite : OutOfProcessTestSite;
            return GetBaseDeploymentParameters(publisher, hostingModel);
        }

        public IISDeploymentParameters GetBaseDeploymentParameters(TestVariant variant)
        {
            var publisher = variant.HostingModel == HostingModel.InProcess ? InProcessTestSite : OutOfProcessTestSite;
            return GetBaseDeploymentParameters(publisher, new DeploymentParameters(variant));
        }

        public IISDeploymentParameters GetBaseDeploymentParameters(ApplicationPublisher publisher, HostingModel hostingModel = HostingModel.InProcess)
        {
            return GetBaseDeploymentParameters(
                publisher,
                new DeploymentParameters()
                {
                    ServerType = DeployerSelector.ServerType,
                    RuntimeFlavor = RuntimeFlavor.CoreClr,
                    RuntimeArchitecture = RuntimeArchitecture.x64,
                    HostingModel = hostingModel,
                    TargetFramework = Tfm.NetCoreApp30
                });
        }

        public IISDeploymentParameters GetBaseDeploymentParameters(ApplicationPublisher publisher, DeploymentParameters baseParameters)
        {
            return new IISDeploymentParameters(baseParameters)
            {
                ApplicationPublisher = publisher,
                PublishApplicationBeforeDeployment = true
            };
        }
    }
}
