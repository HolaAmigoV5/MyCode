﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Dom.Html;
using Microsoft.AspNetCore.Identity.FunctionalTests.Account;

namespace Microsoft.AspNetCore.Identity.FunctionalTests.Contoso
{
    public class Login : DefaultUIPage
    {
        private readonly IHtmlFormElement _loginForm;

        public Login(HttpClient client, IHtmlDocument login, DefaultUIContext context)
            : base(client, login, context)
        {
            _loginForm = HtmlAssert.HasForm(login);
        }

        public async Task<ExternalLogin> SendNewUserNameAsync(string userName)
        {
            var externalLogin = await SendLoginForm(userName);

            return new ExternalLogin(Client, externalLogin, Context.WithSocialLoginProvider());
        }

        public async Task<Index> SendExistingUserNameAsync(string userName)
        {
            var externalLogin = await SendLoginForm(userName);

            return new Index(Client, externalLogin, Context.WithAuthenticatedUser());
        }

        private async Task<IHtmlDocument> SendLoginForm(string userName)
        {
            var contosoResponse = await Client.SendAsync(_loginForm, new Dictionary<string, string>
            {
                ["Input_Login"] = userName
            });

            var goToExternalLogin = ResponseAssert.IsRedirect(contosoResponse);
            var externalLogInResponse = await Client.GetAsync(goToExternalLogin);
            if (Context.ExistingUser)
            {
                var goToIndex = ResponseAssert.IsRedirect(externalLogInResponse);
                var indexResponse = await Client.GetAsync(goToIndex);
                return await ResponseAssert.IsHtmlDocumentAsync(indexResponse);
            }
            else
            {
                return await ResponseAssert.IsHtmlDocumentAsync(externalLogInResponse);
            }
        }
    }
}
