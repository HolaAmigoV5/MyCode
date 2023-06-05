using SignalRDemo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSignalR();

// config Cors
string[] urls = new[] { "http://localhost:3000" };
builder.Services.AddCors(options => options.AddDefaultPolicy(builder => builder.WithOrigins(urls)
.AllowAnyMethod().AllowAnyHeader().AllowCredentials()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();

app.UseAuthorization();
app.MapHub<MyHub>("/MyHub");
app.MapControllers();

app.Run();
