#pragma checksum "D:\My Code\NetCoreDevPractice\src\microservices\Wby.BadSite\Views\Home\Cors.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "3c531c01357d346ae36a691466f816e1cfead5e5"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_Cors), @"mvc.1.0.view", @"/Views/Home/Cors.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "D:\My Code\NetCoreDevPractice\src\microservices\Wby.BadSite\Views\_ViewImports.cshtml"
using Wby.BadSite;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\My Code\NetCoreDevPractice\src\microservices\Wby.BadSite\Views\_ViewImports.cshtml"
using Wby.BadSite.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"3c531c01357d346ae36a691466f816e1cfead5e5", @"/Views/Home/Cors.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"b1a325f010559b44a5c14375065205ffca723f8b", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_Cors : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\n");
#nullable restore
#line 2 "D:\My Code\NetCoreDevPractice\src\microservices\Wby.BadSite\Views\Home\Cors.cshtml"
   ViewData["Title"] = "Cors"; 

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
<h1>Cors</h1>
<h2 id=""h2""></h2>
<script>

    fetch('https://localhost:5003/Home/PostCors?name=abc', {
        method: ""POST"",
        credentials: 'include',
        headers: new Headers({
            'Content-Type': 'application/json'
            //'Content-Type': 'text/html'
        }),
        body: {}
    })
        .then(response => response.json())
        .then(function (response) {
            document.getElementById('h2').innerText = response.name;
        });
</script>");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
