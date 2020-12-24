﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using System.IO.Compression;

namespace My.Util.Helper
{
    /// <summary>
    /// 描述：Http请求操作帮助类
    /// 作者：wby 2019/10/24 9:10:21
    /// </summary>
    public class HttpHelper
    {
        #region 静态构造
        static HttpHelper()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback
                ((sender, certificate, chain, sslPolicyErrors) => true);
        }
        #endregion

        #region 内部成员
        private static string BuildBody(Dictionary<string, object> parameters, ContentType contentType)
        {
            StringBuilder bodyBuilder = new StringBuilder();
            switch (contentType)
            {
                case ContentType.Form:
                    var paramList = parameters?.ToList() ?? new List<KeyValuePair<string, object>>();
                    for (int i = 0; i < paramList.Count; i++)
                    {
                        var theParameter = paramList[i];
                        string key = theParameter.Key;
                        string Value = theParameter.Value?.ToString();

                        string head = string.Empty;
                        if (i != 0)
                            head = "&";
                        bodyBuilder.Append($@"{head}{key}={Value}");
                    }
                    break;
                case ContentType.Json:
                    bodyBuilder.Append(JsonConvert.SerializeObject(parameters));
                    break;
                default:
                    break;
            }
            return bodyBuilder.ToString();
        }

        private static bool UrlHaveParam(string url)
        {
            return url.Contains("?");
        }

        private static string GetContentTypeStr(ContentType contentType)
        {
            string contentTypeStr = string.Empty;
            switch (contentType)
            {
                case ContentType.Form:
                    contentTypeStr = "application/x-www-form-urlencoded";
                    break;
                case ContentType.Json:
                    contentTypeStr = "application/json";
                    break;
                default:
                    break;
            }

            return contentTypeStr;
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 发起GET请求
        /// 注：若使用证书,推荐使用X509Certificate2的pkcs12证书
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="paramters">参数</param>
        /// <param name="headers">请求头</param>
        /// <param name="cerFile">证书</param>
        /// <returns></returns>
        public static string GetData(string url, Dictionary<string, object> paramters = null,
            Dictionary<string, string> headers = null, X509Certificate cerFile = null)
        {
            return RequestData(HttpMethod.Get, url, paramters, headers, ContentType.Form, cerFile);
        }

        /// <summary>
        /// 请求数据
        /// 注：若使用证书,推荐使用X509Certificate2的pkcs12证书
        /// </summary>
        /// <param name="method">请求方法</param>
        /// <param name="url">URL地址</param>
        /// <param name="paramters">参数</param>
        /// <param name="headers">请求头信息</param>
        /// <param name="contentType">请求数据类型</param>
        /// <param name="cerFile">证书</param>
        /// <returns></returns>
        public static string RequestData(HttpMethod method, string url, Dictionary<string, object> paramters = null,
            Dictionary<string, string> headers = null, ContentType contentType = ContentType.Form, X509Certificate cerFile = null)
        {
            if (url.IsNullOrEmpty())
                throw new ArgumentNullException("请求地址不能为NULL或空！");

            string newUrl = url;
            if (method == HttpMethod.Get)
            {
                //StringBuilder paramBuilder = new StringBuilder();
                //var paramList = new List<KeyValuePair<string, object>>();
                //paramList = paramters?.ToList() ?? new List<KeyValuePair<string, object>>();
                //for (int i = 0; i < paramList.Count; i++)
                //{
                //    var theParamter = paramList[i];
                //    string key = theParamter.Key;
                //    string value = theParamter.Value?.ToString();

                //    string head = string.Empty;
                //    if (i == 0 && !UrlHaveParam(url))
                //        head = "?";
                //    else
                //        head = "&";

                //    paramBuilder.Append($@"{head}{key}={value}");
                //}

                //newUrl = url + paramBuilder.ToString();
                newUrl = BuildGetFullUrl(url);
            }

            string body = BuildBody(paramters, contentType);

            return RequestData(method.ToString().ToUpper(), newUrl, body, GetContentTypeStr(contentType), headers, cerFile);
        }

        /// <summary>
        /// 请求数据
        /// 注：若使用证书,推荐使用X509Certificate2的pkcs12证书
        /// </summary>
        /// <param name="method">请求方法</param>
        /// <param name="url">请求地址</param>
        /// <param name="body">请求的body内容</param>
        /// <param name="contentType">请求数据类型</param>
        /// <param name="heads">请求头</param>
        /// <param name="cerFile">证书</param>
        /// <returns></returns>
        public static string RequestData(string method, string url, string body, string contentType,
            Dictionary<string, string> heads = null, X509Certificate cerFile = null)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("请求地址不能为NULL或空!");

            string newUrl = url;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(newUrl);
            request.Method = method.ToUpper();
            request.ContentType = contentType;
            heads?.ForEach(aHead =>
            {
                request.Headers.Add(aHead.Key, aHead.Value);
            });

            //HTTPS证书
            if (cerFile != null)
                request.ClientCertificates.Add(cerFile);

            if (method.ToUpper() != "GET")
            {
                byte[] data = Encoding.UTF8.GetBytes(body);
                request.ContentLength = data.Length;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(data, 0, data.Length);
                }
            }
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                int httpStatusCode = (int)response.StatusCode;
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    string resData = reader.ReadToEnd();
                    return resData;
                }
            }
        }

        /// <summary>
        /// 获取所有请求的参数（包括get参数和post参数）
        /// </summary>
        /// <param name="context">请求上下文</param>
        /// <returns></returns>
        public static Dictionary<string, object> GetAllRequestParams(HttpContext context)
        {
            Dictionary<string, object> allParams = new Dictionary<string, object>();

            var request = context.Request;
            List<string> paramKeys = new List<string>();
            var getParams = request.Query.Keys.ToList();
            var postParams = new List<string>();

            try
            {
                if (request.Method.ToLower() != "get")
                    postParams = request.Form.Keys.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            paramKeys.AddRange(getParams);
            paramKeys.AddRange(postParams);

            paramKeys.ForEach(aParam =>
            {
                object value = null;
                if (request.Query.ContainsKey(aParam))
                    value = request.Query[aParam].ToString();
                else if (request.Form.ContainsKey(aParam))
                    value = request.Form[aParam].ToString();

                allParams.Add(aParam, value);
            });

            string contentType = request.ContentType.ToLower() ?? "";

            //若为POST的application/json
            if (contentType.Contains("application/json"))
            {
                var stream = request.Body;
                string str = stream.ReadToString(Encoding.UTF8);
                if (!str.IsNullOrEmpty())
                {
                    var obj = str.ToJObject();
                    foreach (var aProperty in obj)
                    {
                        allParams[aProperty.Key] = aProperty.Value;
                    }
                }
            }

            return allParams;
        }

        /// <summary>
        /// 构建完全Url
        /// </summary>
        /// <param name="url">Url</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public static string BuildGetFullUrl(string url, Dictionary<string, object> parameters = null)
        {
            StringBuilder paramBuilder = new StringBuilder();
            var paramList = new List<KeyValuePair<string, object>>();
            paramList = parameters?.ToList();

            for (int i = 0; i < paramList.Count; i++)
            {
                var theParamter = paramList[i];
                string key = theParamter.Key;
                string value = theParamter.Value?.ToString();

                string head = string.Empty;
                if (i == 0 && !UrlHaveParam(url))
                    head = "?";
                else
                    head = "&";
                paramBuilder.Append($@"{head}{key}={value}");
            }
            return url + paramBuilder.ToString();
        }

        /// <summary>
        /// 从URL获取html文档
        /// </summary>
        /// <param name="url">URL</param>
        /// <returns></returns>
        public static string GetHtml(string url)
        {
            string htmlCode = string.Empty;
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                    webRequest.Timeout = 10000;
                    webRequest.Method = "GET";
                    webRequest.UserAgent = "Mozilla/4.0";
                    webRequest.Headers.Add("Accept-Encoding", "gzip, deflate");

                    HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();

                    //获取目标网站的编码格式
                    string contentype = webResponse.Headers["Content-Type"];
                    Regex regex = new Regex("charset\\s*=\\s*[\\W]?\\s*([\\w-]+)", RegexOptions.IgnoreCase);

                    //如果使用了GZip则先解压
                    if (webResponse.ContentEncoding.ToLower() == "gzip")
                    {
                        Stream streamReceive = webResponse.GetResponseStream();
                        MemoryStream ms = new MemoryStream();
                        streamReceive.CopyTo(ms);
                        ms.Seek(0, SeekOrigin.Begin);
                        var zipStream = new GZipStream(ms, CompressionMode.Decompress);
                        if (regex.IsMatch(contentype))
                        {
                            Encoding ending = Encoding.GetEncoding(regex.Match(contentype).Groups[1].Value.Trim());
                            using (StreamReader sr = new StreamReader(zipStream, ending))
                            {
                                htmlCode = sr.ReadToEnd();
                            }
                        }
                        else //匹配不到则自动转换成utf-8
                        {
                            StreamReader sr = new StreamReader(zipStream, Encoding.GetEncoding("utf-8"));
                            htmlCode = sr.ReadToEnd();
                            string subStr = htmlCode.Substring(0, 2000);
                            string pattern = "charset=(.*?)\"";
                            Encoding encoding;
                            foreach (Match match in Regex.Matches(subStr, pattern))
                            {
                                if (match.Groups[1].ToString().ToLower() == "utf-8")
                                    break;
                                else
                                {
                                    encoding = Encoding.GetEncoding(match.Groups[1].ToString().ToLower());
                                    ms.Seek(0, SeekOrigin.Begin); //设置流的初始位置
                                    var zipStream2 = new GZipStream(ms, CompressionMode.Decompress);
                                    StreamReader sr2 = new StreamReader(zipStream2, encoding);
                                    htmlCode = sr2.ReadToEnd();
                                }
                            }
                        }
                    }
                    else
                    {
                        using (Stream streamReceive = webResponse.GetResponseStream())
                        {
                            using (StreamReader sr = new StreamReader(streamReceive, Encoding.Default))
                            {
                                htmlCode = sr.ReadToEnd();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return htmlCode;
        }

        /// <summary>
        /// 发起安全签名请求
        /// 注：使用本框架签名算法,ContentType为application/json
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="body">请求body</param>
        /// <param name="appId">应用Id</param>
        /// <param name="appSecret">应用密钥</param>
        /// <returns></returns>
        public static string SafeSignRequest(string url, string body, string appId, string appSecret)
        {
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string guid = Guid.NewGuid().ToString();
            Dictionary<string, string> headers = new Dictionary<string, string>
            {
                {"appId",appId },
                {"time",time },
                {"guid",guid },
                {"sign",BuildApiSign(appId,appSecret,guid,time.ToDateTime(),body)}
            };

            return RequestData("post", url, body, "application/json", headers);
        }

        /// <summary>
        /// 生成接口签名sign
        /// 注：md5(appId+time+guid+body+appSecret)
        /// </summary>
        /// <param name="appId">应用Id</param>
        /// <param name="appSecret">应用密钥</param>
        /// <param name="guid">唯一GUID</param>
        /// <param name="time">时间</param>
        /// <param name="body">请求体</param>
        /// <returns></returns>
        public static string BuildApiSign(string appId, string appSecret, string guid, DateTime time, string body)
        {
            return $"{appId}{time.ToString("yyyy-MM-dd HH:mm:ss")}{guid}{body}{appSecret}".ToMD5String();
        } 
        #endregion
    }
    
    #region 类型定义
    public enum HttpMethod
    {
        Get, Post, Put, Delete, Head, Options, Trace, Connect
    }

    public enum ContentType
    {
        /// <summary>
        /// 传统Form表单,即application/x-www-form-urlencoded
        /// </summary>
        Form,

        /// <summary>
        /// 使用Json,即application/json
        /// </summary>
        Json
    } 
    #endregion
}
