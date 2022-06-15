using BlazorWASM;
using BlazorWASM.Model;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
//builder.RootComponents.Add<HeadOutlet>("head::after");

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5155") });
builder.Services.AddSingleton<Store>();

await builder.Build().RunAsync();
