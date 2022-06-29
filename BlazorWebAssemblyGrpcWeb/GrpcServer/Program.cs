using GrpcServer.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddGrpc();

var app = builder.Build();

app.Use(async (context, next) => {
    context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
    context.Response.Headers.Add("Access-Control-Allow-Headers", "*");
    context.Response.Headers.Add("Access-Control-Allow-Methods", "PUT,POST,GET,DELETE,OPTIONS,HEAD,PATCH");
    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
    context.Response.Headers.Add("Access-Control-Max-Age", "100000");
    context.Response.Headers.Add("Access-Control-Expose-Headers", "Grpc-Status,Grpc-Message,Grpc-Encoding,Grpc-Accept-Encoding");
    if (context.Request.Method.ToUpper() == "OPTIONS")
    {
        return;
    }

    await next.Invoke();
});

// Configure the HTTP request pipeline.
app.UseGrpcWeb();
app.MapGrpcService<GreeterService>().EnableGrpcWeb();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
