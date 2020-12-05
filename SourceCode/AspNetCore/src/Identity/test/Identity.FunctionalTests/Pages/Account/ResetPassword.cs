﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Dom.Html;

namespace Microsoft.AspNetCore.Identity.FunctionalTests.Account
{
    public class ResetPassword : DefaultUIPage
    {
        private readonly IHtmlFormElement _resetPasswordForm;

        public ResetPassword(HttpClient client, IHtmlDocument resetPassword, DefaultUIContext context) : base(client, resetPassword, context)
        {
            _resetPasswordForm = HtmlAssert.HasForm(resetPassword);
        }

        internal static async Task<ResetPassword> CreateAsync(IHtmlAnchorElement link, HttpClient client, DefaultUIContext context)
        {
            var resetPasswordResponse = await client.GetAsync(link.Href);
            var resetPassword = await ResponseAssert.IsHtmlDocumentAsync(resetPasswordResponse);

            return new ResetPassword(client, resetPassword, context);
        }

        public async Task<ResetPasswordConfirmation> SendNewPasswordAsync(string email, string newPassword)
        {
            var resetPasswordResponse = await Client.SendAsync(_resetPasswordForm, new Dictionary<string, string>
            {
                ["Input_Email"] = email,
                ["Input_Password"] = newPassword,
                ["Input_ConfirmPassword"] = newPassword
            });

            var goToResetPasswordConfirmation = ResponseAssert.IsRedirect(resetPasswordResponse);
            var resetPasswordConfirmationResponse = await Client.GetAsync(goToResetPasswordConfirmation);
            var resetPasswordConfirmation = await ResponseAssert.IsHtmlDocumentAsync(resetPasswordConfirmationResponse);

            return new ResetPasswordConfirmation(Client, resetPasswordConfirmation, Context);
        }
    }
}
