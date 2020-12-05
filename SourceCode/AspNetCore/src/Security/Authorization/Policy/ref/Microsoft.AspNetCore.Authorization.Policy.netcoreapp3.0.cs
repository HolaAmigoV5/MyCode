// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.AspNetCore.Authorization
{
    public partial class AuthorizationMiddleware
    {
        public AuthorizationMiddleware(Microsoft.AspNetCore.Http.RequestDelegate next, Microsoft.AspNetCore.Authorization.IAuthorizationPolicyProvider policyProvider) { }
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public System.Threading.Tasks.Task Invoke(Microsoft.AspNetCore.Http.HttpContext context) { throw null; }
    }
}
namespace Microsoft.AspNetCore.Authorization.Policy
{
    public partial interface IPolicyEvaluator
    {
        System.Threading.Tasks.Task<Microsoft.AspNetCore.Authentication.AuthenticateResult> AuthenticateAsync(Microsoft.AspNetCore.Authorization.AuthorizationPolicy policy, Microsoft.AspNetCore.Http.HttpContext context);
        System.Threading.Tasks.Task<Microsoft.AspNetCore.Authorization.Policy.PolicyAuthorizationResult> AuthorizeAsync(Microsoft.AspNetCore.Authorization.AuthorizationPolicy policy, Microsoft.AspNetCore.Authentication.AuthenticateResult authenticationResult, Microsoft.AspNetCore.Http.HttpContext context, object resource);
    }
    public partial class PolicyAuthorizationResult
    {
        internal PolicyAuthorizationResult() { }
        public bool Challenged { [System.Runtime.CompilerServices.CompilerGeneratedAttribute]get { throw null; } }
        public bool Forbidden { [System.Runtime.CompilerServices.CompilerGeneratedAttribute]get { throw null; } }
        public bool Succeeded { [System.Runtime.CompilerServices.CompilerGeneratedAttribute]get { throw null; } }
        public static Microsoft.AspNetCore.Authorization.Policy.PolicyAuthorizationResult Challenge() { throw null; }
        public static Microsoft.AspNetCore.Authorization.Policy.PolicyAuthorizationResult Forbid() { throw null; }
        public static Microsoft.AspNetCore.Authorization.Policy.PolicyAuthorizationResult Success() { throw null; }
    }
    public partial class PolicyEvaluator : Microsoft.AspNetCore.Authorization.Policy.IPolicyEvaluator
    {
        public PolicyEvaluator(Microsoft.AspNetCore.Authorization.IAuthorizationService authorization) { }
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public virtual System.Threading.Tasks.Task<Microsoft.AspNetCore.Authentication.AuthenticateResult> AuthenticateAsync(Microsoft.AspNetCore.Authorization.AuthorizationPolicy policy, Microsoft.AspNetCore.Http.HttpContext context) { throw null; }
        [System.Diagnostics.DebuggerStepThroughAttribute]
        public virtual System.Threading.Tasks.Task<Microsoft.AspNetCore.Authorization.Policy.PolicyAuthorizationResult> AuthorizeAsync(Microsoft.AspNetCore.Authorization.AuthorizationPolicy policy, Microsoft.AspNetCore.Authentication.AuthenticateResult authenticationResult, Microsoft.AspNetCore.Http.HttpContext context, object resource) { throw null; }
    }
}
namespace Microsoft.AspNetCore.Builder
{
    public static partial class AuthorizationAppBuilderExtensions
    {
        public static Microsoft.AspNetCore.Builder.IApplicationBuilder UseAuthorization(this Microsoft.AspNetCore.Builder.IApplicationBuilder app) { throw null; }
    }
    public static partial class AuthorizationEndpointConventionBuilderExtensions
    {
        public static Microsoft.AspNetCore.Builder.IEndpointConventionBuilder RequireAuthorization(this Microsoft.AspNetCore.Builder.IEndpointConventionBuilder builder) { throw null; }
        public static Microsoft.AspNetCore.Builder.IEndpointConventionBuilder RequireAuthorization(this Microsoft.AspNetCore.Builder.IEndpointConventionBuilder builder, params Microsoft.AspNetCore.Authorization.IAuthorizeData[] authorizeData) { throw null; }
        public static Microsoft.AspNetCore.Builder.IEndpointConventionBuilder RequireAuthorization(this Microsoft.AspNetCore.Builder.IEndpointConventionBuilder builder, params string[] policyNames) { throw null; }
    }
}
namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class PolicyServiceCollectionExtensions
    {
        public static Microsoft.Extensions.DependencyInjection.IServiceCollection AddAuthorizationPolicyEvaluator(this Microsoft.Extensions.DependencyInjection.IServiceCollection services) { throw null; }
    }
}
