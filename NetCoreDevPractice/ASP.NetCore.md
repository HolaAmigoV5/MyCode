# ASP.Net Core

## 总体框架

![DotNet Core_all](https://cdn.jsdelivr.net/gh/HolaAmigoV5/Images/DotNet/DotNetCore_all.png)

## 基础知识

​	一个ASP.Net Core应用本质上就是一个用来监听、接收和处理HTTP请求的后台服务。提供KestrelServer和Http.sys两种服务类型，前者支持跨平台，后者在window使用。

​	launchSetting.json：应用程序启动时自动加载的配置文件，该文件不需要手动编辑，右键项目，选择【属性】中的“调试”选项卡的所有设置最终都会体现在该文件上。

### 启动过程

​	Asp.Net Core Web应用程序启动顺序如下：

1. ConfigureWebHostDefaults：注册必要组件，比如配置组件，容器组件，
2. ConfigureHostConfiguration：启动时配置，比如监听的端口，URL地址等
3. ConfigureAppConfiguration：嵌入自己的配置文件，
4. ConfigureServices, ConfigureLogging, Startup, Startup.ConfigureServices：容器注入应用组件
5. Startup.Configure：注册中间件

![start_up](https://cdn.jsdelivr.net/gh/HolaAmigoV5/Images/DotNet/start_up.png)



### 依赖注入

​	说到依赖注入（Dependence Injection, DI），就得说到控制反转（Inverse of Control, IoC）。所谓的控制反转就是将对流程的控制转移到框架之中。而依赖注入就是一种IoC模式。

**3种依赖注入方式**

1. 构造器注入：在构造函数的入参中放入要依赖的对象。
2. 属性注入：依赖的对象作为类的某个属性，并且该属性不是只读的。
3. 方法注入：依赖的对象作为方法的入参传入。

​	依赖注入核心组件包DependencyInjection.Abstractions, DependencyInjection。这两个组件一个是抽象接口，一个是具体实现。其中核心类型如下：

1. IServiceCollection：存放服务注册信息的集合
2. ServiceDescriptor：服务注册项的描述
3. IServiceProvider：具体容器，由ServiceCollection构建出来，提供`GetService<T>();`方法获取注册对象
4. IServiceScope：子容器的生命周期
   - 单例 Singleton：都是单例，无论子容器还是根容器
   - 作用域 Scoped：当前容器释放掉，对象会释放
   - 瞬时（暂时） Transient：每次获取全信对象

#### 服务注册

在Startup.ConfigureServices中进行服务的注册，代码如下：

```c#
//注册不同的生命周期
services.AddSingleton<IMySingletonService, MySingletonService>();
services.AddScoped<IMyScopedService, MyScopedService>();
services.AddTransient<IMyTransientService, MyTransientService>();

//直接注入实例
services.AddSingleton<IOrderService>(new OrderService()); 
services.AddSingleton<IOrderService>(serviceProvider =>{
    //工厂模式注册，应对复杂逻辑。可组装对象返回
    return new OrderServiceEx();
});

//尝试注册，已经注册过就不能再注册了
services.TryAddSingleton<IOrderService, OrderServiceEx>();
//注册形同的接口，不同的实现对象
services.TryAddEnumerable(ServiceDescriptor.Singleton<IOrderService, OrderServiceEx>());

//替换原来的注册对象
services.Replace(ServiceDescriptor.Singleton<IOrderService, OrderServiceEx>());

//泛型对象的注册
services.AddSingleton(typeof(IGenericService<>), typeof(GenericService<>));
```

#### 服务获取

```c#
//方式一：构造函数获取，适用于这个对象，多个地方使用
IOrderService _orderService;
public WeatherForecastController(IOrderService orderService){
     _orderService = orderService; //获取到注册对象后，可以进行后续操作
}

//方式二：使用FromServices标签获取
public void GetService([FromServices]IMySingletonService singleton1){
    Console.WriteLine($"singleton1:{singleton1.GetHashCode()}");
}
```

**注意**：瞬时模式注册的对象，如果在根容器获取时，会导致根容器里面的瞬时对象不断积累，程序退出时才释放，不建议这样使用。。

#### AutoFac使用

1. 引用Autofac.Extensions.DependencyInjection和Autofac.Extras.DynamicProxy包；

2. 使用UseServiceProviderFactory(new AutofacServiceProviderFactory())注册Autofac容器

3. `builder.RegisterType<MyService>().As<IMyService>();`注册对象。可以命名注册，属性注册，AOP注册。使用AOP的时候，需要定义拦截器(继承自接口Interceptor)

   AutoFac提供基于名称的注入，属性注入，子容器，和基于动态代理的AOP。

### 配置框架

框架配置使用到的核心Nuget包有：

① Microsoft.Extensions.Configuration.Abstractions，② Microsoft.Extensions.Configuration.

​	IConfigurationBuilder对象利用注册在它上面的所有IConfigurationSource对象提供的IConfigurationProvider来读取原始配置数据并创建出相应的IConfiguration对象。总的来说，IConfigurationSource是配置原料，IConfigurationBuilder是工厂构建出IConfiguration供应用程序使用。读取配置方式如下：

1. 以键值对的形式读取配置
2. 命令行配置
3. 环境变量配置
4. 读取结构化配置或直接绑定为对象(Json, Xml等)

配置模型的最终目的在于提取原始的配置数据转化为一个IConfiguration(配置树，逻辑结构)对象，4个核心对象：

1. IConfigurationSource：配置源，为构建IConfiguration对象提供配置数据。扩展时继承
2. IConfigurationProvider：将原始结构转化为配置字典。扩展时继承
3. IConfigurationBuilder：IConfigurationSource注册于此，利用配置源构建IConfiguration
4. IConfiguration：逻辑上具有树形层次结构，称为配置树，供程序使用

#### 键值对配置

```c#
IConfigurationBuilder builder = new ConfigurationBuilder();
builder.AddInMemoryCollection(new Dictionary<string, string>()
{
   	{ "key1","value1" },
   	{ "key2","value2" },
	{ "section1:key4","value4" },
	{ "section2:key5","value5" },
	{ "section2:key6","value6" },
	{ "section2:section3:key7","value7" }
});
//配置对象构建出来，返回配置的根对象
IConfigurationRoot configurationRoot = builder.Build();
Console.WriteLine(configurationRoot["key1"]);

IConfigurationSection section = configurationRoot.GetSection("section1");
Console.WriteLine($"key4:{section["key4"]}");

IConfigurationSection section2 = configurationRoot.GetSection("section2");
Console.WriteLine($"key5_v2:{section2["key5"]}");
var section3 = section2.GetSection("section3");
Console.WriteLine($"key7:{section3["key7"]}");
```

#### 命令行配置

​	使用命令行配置提供程序接收命令行参数。命令替换模式时，必须单划线(-)或双划线(--)开头；映射字典不能包含重复Key。

```c#
var builder = new ConfigurationBuilder();
//builder.AddCommandLine(args);

//命令替换模式。这里常见的应用是：用短写的"-k1"命令代替，全称命令"CommandLineKey1"。
var mapper = new Dictionary<string, string> { {"-k1", "CommandLineKey1" } }; 
builder.AddCommandLine(args, mapper);

var configurationRoot = builder.Build();
Console.WriteLine($"CommandLineKey1：{configurationRoot["CommandLineKey1"]}");
```

#### 环境变量配置

​	环境变量的配置，适用场景有：① 在Docker或Kubernetes中运行时；② 需要设置ASP.NET Core的一些内置特殊配置时。对于配置的分层键，支持用双下划线"__"代替":"；支持前缀过滤。

```c#
var builder = new ConfigurationBuilder();
//builder.AddEnvironmentVariables();

//var configurationRoot = builder.Build();
//Console.WriteLine($"key1:{configurationRoot["key1"]}");

////分层键：获取Section下的键
//var section = configurationRoot.GetSection("SECTION1");
//Console.WriteLine($"key3:{section["KEY3"]}");

////多层键：多层级Section键获取
//var section2 = configurationRoot.GetSection("SECTION1:SECTION2");
//Console.WriteLine($"KEY4:{section2["KEY4"]}");


//前缀过滤：注入指定前缀的变量，如下只注入前缀为"XIAO_"的环境变量
builder.AddEnvironmentVariables("XIAO_");
var configurationRoot = builder.Build();
Console.WriteLine($"KEY1:{configurationRoot["KEY1"]}");
Console.WriteLine($"KEY2:{configurationRoot["KEY2"]}");
```

#### 文件配置

文件配置支持Json，Xml，Ini等格式文件的配置处理，可能需要使用到的包如下：

1. Microsoft.Extensions.Configuration.Ini
2. Microsoft.Extensions.Configuration.Json
3. Microsoft.Extensions.Configuration.NewtonsoftJson
4. Microsoft.Extensions.Configuration.Xml
5. Microsoft.Extensions.Configuration.UserSecrets

代码如下：

```c#
var builder = new ConfigurationBuilder();
//optional参数：true，配置不存在时不报错；false，配置不存在时报错。
//reloadOnChange参数：true，配置文件修改就重载，立马生效；false，修改不重载
//后添加进来的相同文件会覆盖前面的文件。
builder.AddJsonFile("appsettings.Json",true,true);
builder.AddIniFile("appsettings.ini",true,true);

var configurationRoot = builder.Build();
Console.WriteLine($"Key1:{configurationRoot["Key1"]}");
```

#### 配置更新

```c#
 //监视变更后重新输出，只能监视一次
//IChangeToken token = configurationRoot.GetReloadToken();
//token.RegisterChangeCallback(state =>
//{
	//    Console.WriteLine($"Key1:{configurationRoot["Key1"]}");
	//    Console.WriteLine($"Key2:{configurationRoot["Key2"]}");
//}, configurationRoot);
//Console.ReadKey();

//文件的监听，只要一有变更，立马捕获
ChangeToken.OnChange(() => configurationRoot.GetReloadToken(), () => {
    Console.WriteLine($"Key1:{configurationRoot["Key1"]}");
    Console.WriteLine($"Key2:{configurationRoot["Key2"]}");
});
Console.WriteLine("开始了");
```

#### 配置绑定

```c#
var config = new Config() { Key1 = "config key1", Key5 = false };
//将对象帮到到对象Config上，支持绑定到私有属性上
configurationRoot.Bind(config);

configurationRoot.GetSection("OrderService").Bind(config, 
    binderOptions => { binderOptions.BindNonPublicProperties = true; });
```

#### 自定义配置

```c#
//自定义配置数据源
//扩展ConfigurationBuilder实现，包装我的配置源对象，封装
builder.AddMyConfiguration();

//builder.Add(new MyConfigurationSource());

var configRoot = builder.Build();
ChangeToken.OnChange(() => configRoot.GetReloadToken(), () => {
    Console.WriteLine($"lastTime:{configRoot["lastTime"]}");
});
//Console.WriteLine($"lastTime:{configRoot["lastTime"]}");
Console.WriteLine("开始了。");
```

### 选项框架

​	选项框架支持单例模式读取配置，支持快照，支持配置变更通知，支持运行时动态修改选项值。服务设计时使用XXXOptions模式。用IOptions\<XXOptions>, IOptionsSnapshot\<XXOptions>，IOptionsMonitor\<XOptions>作为服务构造函数的参数。

```c#
//配置和选项绑定：将配置信息跟XXOptions绑定。在实现服务OrderService的构造函数中
//通过IOptions<XXOptins>获取到配置的值
services.Configure<OrderServiceOptions>(Configuration.GetSection("OrderService"));
services.AddSingleton<OrderServiceOptions>();

//选项数据热更新：让服务感知配置的变化：只需要在OrderService的构造函数中
//使用IOptionsSnapshot<XXOptions>获取配置信息，则可进行热更新
//这里注意使用AddScoped，而使用AddSingleton会出错，使用IOptionsSnapshot不支持单例模式
services.AddScoped<IOrderService, OrderService>();

//同上，如果必须使用单例模式，则构造函数中得使用IOptionsMonitor<XXOptions>

//使用PostConfigure进行动态配置：修改配置信息
services.PostConfigure<OrderServiceOptions>(option =>{
    option.MaxOrderCount += 100;
});
```

**最佳实践：** 服务配置较多时，考虑将所有服务注册放在services的扩展方法AddOrderService中，更简洁。

#### 选项数据验证

​	避免错误配置的应用接收用户流量。有三种验证方法：①直接注册验证函数，②实现IValidateOptions\<TOptions>；③ 使用Microsoft.Extensions.Options.DataAnnotations。

```c#
 //为数据选项添加验证
 services.AddOptions<OrderServiceOptions>().Configure(options =>{
     configuration.Bind(options);  //绑定配置
 }).Validate(options =>{
     return options.MaxOrderCount <= 100;
 }, "MaxOrderCount不能大于100.");

//属性验证：直接在OrderServiceOptions的属性上添加"[Range(30, 100)]"这样的属性验证即可


//接口注入验证方式：使用IValidateOptions<OrderServiceOptions>接口构建自己的验证实现
services.Configure<OrderServiceOptions>(configuration);
//注入验证接口
services.AddSingleton<IValidateOptions<OrderServiceOptions>, 
OrderServiceValidateOptions>();

```

### 日志框架

​	统一日志编程模型主要涉及由ILogger接口、ILoggerFactory接口、ILoggerProvider接口三个核心对象，这些接口定义在NuGet包“Microsoft.Extensions.Logging.Abstractions”和“Microsoft.Extensions.Logging”中。应用程序通过ILoggerFactory创建的ILogger对象来记录日志，而ILoggerProvider则完成针对相应的渠道的日志输出。日志级别(LogLevel)从低到高依次是：Trace，Debug，Information，Warning，Error，Critical，None。

```c#
//读取配置的json信息，然后将这个配置信息注入到容器 
IConfigurationBuilder configBuilder = new ConfigurationBuilder();
configBuilder.AddJsonFile("appsettings.json", false, true);
var config = configBuilder.Build();

IServiceCollection serviceCollection = new ServiceCollection();
//工厂模式将配置对象注册到容器管理
serviceCollection.AddSingleton<IConfiguration>(p => config);

serviceCollection.AddLogging(builder => {
	builder.AddConfiguration(config.GetSection("Logging"));
	builder.AddConsole();
});

//从容器中获取ILogger(日志记录器)，记录日志信息
IServiceProvider service = serviceCollection.BuildServiceProvider();
ILoggerFactory loggerFactory = service.GetService<ILoggerFactory>();
ILogger alogger = loggerFactory.CreateLogger("alogger");//创建日志记录器
alogger.LogDebug(2001, "aiya");
alogger.LogInformation("hello");
var ex = new Exception("出错了");
alogger.LogError(ex, "出错了");


//最佳实践：定义OrderService专门进行日志的记录，从容器拿到对象后调用。一般不用上述方法
IServiceProvider service = serviceCollection.BuildServiceProvider();
serviceCollection.AddTransient<OrderService>();
var order = service.GetService<OrderService>();
order.Show();
```

#### 日志作用域

​	所谓的日志范围是通过ILogger对象的BeginScope方法创建，调用时可以用GUID标识来描述并标识创建的上下文。**作用域场景**：① 一个事务包含多条操作时，需要把多条日志串联，② 复杂流程的日志关联时，③ 调用链追踪时需要把日志串联。

```c#
//读取配置信息。支持配置热跟新。
//注意：appsettings.json配置文件中需要设置【"IncludeScopes": true】，日志作用域才生效
var configBuilder = new ConfigurationBuilder();
configBuilder.AddCommandLine(args);
configBuilder.AddJsonFile("appsettings.json", false, true);
var config = configBuilder.Build();

var serviceCollection = new ServiceCollection();
//用工厂模式将配置对象注册到容器管理
serviceCollection.AddSingleton<IConfiguration>(p => config); 
serviceCollection.AddLogging(builder => {
	builder.AddConfiguration(config.GetSection("Logging"));
	builder.AddConsole();
	builder.AddDebug();
});

//开始根据配置记录日志，支持热更新
IServiceProvider service = serviceCollection.BuildServiceProvider();
var logger = service.GetService<ILogger<Program>>();
while (Console.ReadKey().Key!=ConsoleKey.Escape)
{
    //启用了作用域后，这里生成相同的日志ID
	using (logger.BeginScope("ScopeId:{scopeId}", Guid.NewGuid()))
	{
        logger.LogInformation("这是Info");
        logger.LogError("这是Error");
        logger.LogTrace("这是Trace");

		System.Threading.Thread.Sleep(100);
		Console.WriteLine("====================分割线==============");
	}
}
```

#### 结构化日志

​	对日志进行结构化易于检索和统计分析。**使用场景**：① 实现日志告警，② 实现上下文关联， ③ 实现追踪系统集成。使用Serilog进行日志结构化。使用方法如下图：

![Serilog](https://cdn.jsdelivr.net/gh/HolaAmigoV5/Images/DotNet/Serilog.png)

### 中间件

中间件工作原理图如下：

![middleware](https://cdn.jsdelivr.net/gh/HolaAmigoV5/Images/DotNet/middleware.png)

中间件核心对象

1. **IApplicationBuiler**：注册中间件。如调用`app.Use()`可以将两个委托(RequestDelegate)注册成链。调用Build()方法后，返回一个串联起来的委托(RequestDelegate)。Asp.net Core中在Configure(...)方法中注册中间件。
2. **RequestDelegate**：处理请求的委托。

中间件的执行顺序和注册顺序是有关的。最早注册的中间件权利最大，越早发挥作用。

```c#
//在Configure(IApplicationBuilder app, IWebHostEnvironment env)方法中注册中间件
app.Use(async (context, next) =>{
 	//await context.Response.WriteAsync("Hello");
 	await next();
 	await context.Response.WriteAsync("Hello222");
 });

//对特殊路径(如/abc)指定注册中间件
app.Map("/abc", abcBuilder =>{
    abcBuilder.Use(async (context, next) =>{
    	//一旦已经开始输出，则不能再修改响应头的内容
    	//await context.Response.WriteAsync("Hello");
    	await next();
    	await context.Response.WriteAsync("Hello222");
    });
});

//复杂条件判断，注册中间件
app.MapWhen(context =>{
	//请求参数查询包含"abc"时才执行后续操作
	return context.Request.Query.Keys.Contains("abc");
	}, builder =>{
		//这里Run()和User()类似，但有区别
		builder.Run(async context =>{
			await context.Response.WriteAsync("new abc");
	});
});


//注册自定义中间件
app.UseMyMiddleware();
```

#### 异常处理中间件

异常处理的方式有：①异常处理页，②异常处理匿名委托方法, ③IExceptionFilter, ④ExceptionFilterAttribute。

```c#
//定义错误页进行错误处理
app.UseExceptionHandler("/error");  //注册错误中间件处理

//使用匿名委托代理
app.UseExceptionHandler(errApp =>{
    errApp.Run(async context =>{
       //代码省略，查看Demo
    });
});

//MVC中使用异常处理过滤器。ConfigureServices中执行
services.AddMvc(mvcOptions =>{
    //mvcOptions.Filters.Add<MyExceptionFilter>(); //使用自定义异常处理过滤器

    //使用特性标注的方式过滤异常。这种方式可以指定Controller或者方法上执行，更灵活
    mvcOptions.Filters.Add<MyExceptionFilterAttribute>();
}).AddJsonOptions(jsonoptions =>{
   jsonoptions.JsonSerializerOptions.Encoder =JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
});
```

**异常处理技巧**：用特定的异常类或接口表示业务逻辑异常，为业务逻辑异常定义全局错误码。为未知异常定义特定的输出信息和错误码，对于已知业务逻辑异常响应HTTP 200（对监控系统友好），对未预见异常响应HTTP 500。为所有异常记录详细日志。

#### 静态文件中间件

静态文件中间件支持指定相对路径，支持目录浏览，支持设置默认文档，支持多目录映射。

```c#
services.AddDirectoryBrowser();  //注册目录浏览。在ConfigureServices中写
app.UseDirectoryBrowser();  //浏览器中显示静态文件目录。在Configure中写

//在Configure中注册静态文件中间件
//app.UseDefaultFiles();  //使用默认页面
app.UseStaticFiles();  //使用静态文件

//使用自定义目录访问静态文件
app.UseStaticFiles(new StaticFileOptions{
    RequestPath = "/File",  //映射到/file/page.html下
    FileProvider = new PhysicalFileProvider(Path.
  		Combine(Directory.GetCurrentDirectory(), "File"))
});


//对非以"/api"开头的请求进行重写，映射到静态文件上
app.MapWhen(context =>{
    //检索不是以"/api"开头的请求
    return !context.Request.Path.Value.StartsWith("/api");
}, appBuilder =>{
    //写法一：推荐使用
    var option = new RewriteOptions();
    option.AddRewrite(".*", "/index.html", true);
    appBuilder.UseRewriter(option);
    appBuilder.UseStaticFiles();
});
```



### 文件系统

​	ASP.Net Core应用具有很多读取文件的场景，如读取配置文件、静态web资源文件(如CSS、JavaScript和图片文件等)、MVC应用的View文件，以及直接编译到程序集的内嵌资源文件。这些文件的读取都需要使用一个IFileProvider对象。其核心类型有IFileProvider，IFileInfo，IDirectoryContents。内置的文件提供程序有PhysicalFileProvider，EmbeddedFileProvider(嵌入式文件)，CompositeFileProvider(组合文件提供程序，多种文件数据来源时，可以将这些源合并到一个目录)。

-  IFileProvider：提供GetFileInfo()方法输入相对路径返回一个IFileInfo；GetDirectoryContents()方法获取指定目录下的目录信息（IDirectoryContents对象）。
- IDirectoryContents：继承接口IEnumerable\<IFileInfo>，本质是IFileInfo的集合。提供一个Exists属性，表示当前目录是否存在。
- IFileInfo：具有属性Exists，Length，PhysicalPath，Name，LastModified，IsDirectory和CreateReadStream ()方法，用来读取文件流。

```c#
 //物理文件提供程序:读取物理文件
IFileProvider provider1 = new PhysicalFileProvider(AppDomain.CurrentDomain.BaseDirectory);
var contents = provider1.GetDirectoryContents("/"); //获取目录下所有内容

foreach (var item in contents){
	var stream=item.CreateReadStream();  //读取文件流
    Console.WriteLine(item.Name);
}

//嵌入式文件提供程序：读取嵌入式文件
IFileProvider provider2 = new EmbeddedFileProvider(typeof(Program).Assembly);
//var html = provider2.GetFileInfo("emb.html");

//组合文件提供程序
IFileProvider provider = new CompositeFileProvider(provider1, provider2);
var contents2 = provider.GetDirectoryContents("/");
foreach (var item in contents2){
	Console.WriteLine(item.Name);
}
```

### 路由和终结点

​	路由系统在ASP.Net MVC框架里面就已经存在了，在ASP.Net Core框架里面进行了改进。路由系统的核心是指我们的URL和应用程序的Controller的对应关系一种映射。这种映射有两种作用，一种是将Controller映射到action上面；另外一种是根据Controller和action名字生成URL。路由注册的两种方式：① 路由模板方式，② RouteAttribute方式（Web API中使用）。路由系统提供了两个关键类（LinkGenerator和IUrlHelper）实现路由生成URI地址。

**路由约束**：类型约束，范围约束，正则表达式约束，是否必选，自定义IRouteConstraint。

RouteAttribute的实现方式如下图代码：

![RouteAttribute](https://cdn.jsdelivr.net/gh/HolaAmigoV5/Images/DotNet/RouteAttribute.png)

#### 自定义路由约束

首先通过继承IRouteConstraint实现自定义的路由约束。然后使用如下代码注册自定义约束：

```c#
services.AddRouting(options => {
	//注册自定义约束
	options.ConstraintMap.Add("isLong", typeof(MyRouteConstraint));
});

//终结点映射
app.UseEndpoints(endpoints =>{
    //映射默认路由 {controller=Home}/{action=Index}/{id?}
    //endpoints.MapControllers();

    //endpoints.MapControllerRoute("api", "api/{controller}/{action}");

    //使用RouteAttribute
    endpoints.MapControllers();
});
```

## 微服务实战

![microserver](https://cdn.jsdelivr.net/gh/HolaAmigoV5/Images/DotNet/microserver.png)

### 分层结构

应用层：应用API和后台特殊Job实现。

领域模型层：定义领域模型。

基础设施层：领域模型和数据库的映射以及相关实现。仓储实现。

共享层：定义基础异常，Entity，DomainEvent，EFContext等基础实现。**最佳实践**，设计成共享包，使用私有NuGet仓库分发管理。

![DDD](https://cdn.jsdelivr.net/gh/HolaAmigoV5/Images/DotNet/DDD.png)

### 集成事件

​	实现多个微服务直接相互传递事件。实现方式有两种，一种是发布、订阅方式，另一种则是通过观察者模式将事件发送给关注事件的人。

![Integrated_event](https://cdn.jsdelivr.net/gh/HolaAmigoV5/Images/DotNet/Integrated_event.png)



​	通过CAP组件，对集成事件进行发布和订阅。RabbitMQ作为消息队列中间件，实现EventBus。MediatR组件，实现CQRS模式（轻松实现命令查询职责分离模式）。核心对象如下：

1. IMediator：用于命令查询业务操作的的调用 `mediator.Send(new MyCommand { ...});`
2. IRequest, IRequest\<T>：定义命令或查询等业务操作
3. IRequestHandler<in TRequest, TResponse>：命令或查询等业务操作的处理

MediatR组件处理领域事件的核心对象如下：

1. IMediator：` mediator.Publish(new MyEvent { EventName = "event01" });`
2. INotification：领域事件定义时需要继承
3. INotificationHandler\<in TNotification>：领域事件处理对象需要继承

领域事件的定义在领域模型中，领域事件的处理定义在应用程序中，实现分离。

**HttpClientFactory：管理向外请求的最佳实践**

![pipeLine_model](https://cdn.jsdelivr.net/gh/HolaAmigoV5/Images/DotNet/pipeLine_model.png)

HttpClientFactory在组件包Microsoft.Extensions.Http中，提供的核心能力有：

1. 管理内部HttpMessageHandler的生命周期，灵活应对资源问题和DNS刷新问题
2. 支持命名化、类型化配置、集中管理配置、避免冲突
3. 灵活的出站请求管道配置，轻松管理请求生命周期
4. 内置管道最外层和最内层日志记录器，有Information和Trace输出

核心对象：

1. HttpClient
2. HttpMessageHandler
3. SocketsHttpHandler
4. DelegateingHandler
5. IHttpClientFactory
6. IHttpClientBuilder

三种创建模式：①工厂模式， ②命名客户端模式，③类型化客户端模式

**RabbitMQ常用命令**

RabbitMQ在Ubuntu上安装参考官网给出的shell文件。安装完成后可以在浏览器中访问。默认端口15672。

```shell
#启动管理界面和外部监控系统
sudo rabbitmq-plugins enable rabbitmq_management

#启动RabbitMQ
sudo service rabbitmq-server start
sudo service rabbitmq-server restart

#默认情况下，guest用户存在，并且只能从localhost连接。您可以使用密码“ guest”在本地用该用户登录,
#要登录网络，请创建一个管理员用户。
rabbitmqctl add_user admin p.123456            //创建admin用户和密码p.123
rabbitmqctl set_user_tags admin administrator    //设置该用户为administrator角色
rabbitmqctl list_users    查看当前用户列表
rabbitmqctl change_password admin '123'    //修改admin用户密码

#添加v_host
rabbitmqctl add_vhost wby
rabbitmqctl list_vhosts
```

**gRPC：内部服务间通信利器**

​	gRPC(Google Remote Procedure Calls)是Google发起的一个高性能、开源和通用的RPC(Remote procedure call)框架，面向服务端和移动端，基于HTTP/2设计。利用它可以实现像调用本地类一样调用远程服务。特点如下：

1. 提供几乎所有主流语言的实现，打破语言隔阂
2. 基于HTTP/2，开放协议，受到广泛的支持，易于实现和集成。通过 proto3 工具生成指定语言的数据结构、服务端接口以及客户端 Stub；
3. 默认使用Protocal Buffers序列化，性能相较于RESTful Json好很多
4. 工具链成熟，代码生成便捷，开箱即用
5. 支持双向流式的请求和响应，对批处理、低延时场景友好

![gRPC](https://cdn.jsdelivr.net/gh/HolaAmigoV5/Images/DotNet/gRPC.png)

.NET生态对gRPC的支持情况

1. 提供基于HttpClient的原生框架实现
2. 提供原生的ASP.NET Core集成库
3. 提供完整的代码生成工具
4. Visual Studio和Visual Studio Code提供proto文件的智能提示

​    服务端核心包：Grpc.AspNetCore。客户端需要使用核心包：Google.Protobuf，Grpc.Net.Client，Grpc.Net.ClientFactory，Grpc.Tools。proto文件可以定义包、库名、服务"service"、输入输出模型“message”。gRPC异常处理，需要使用Grpc.Core.RpcException，Grpc.Core.Interceptors.Interceptor。

**gRPC命令行工具**

​	命令行工具dotnet grpc常用命令如下：dotnet grpc add-file， dotnet grpc add-url，dotnet grpc remove， dotnet grpc refresh。使用方法如下：

```powershell
//全局安装dotnet-grpc工具，然后切换到工程文件目录下
dotnet tool install dotnet-grpc -g

//将本地proto文件进行应用。当前工程文件目录下执行。
dotnet grpc add-file ..\GrpcServerDemo\Protos\order.proto

//将http上的proto文件引用到工程中.注意使用【raw】
dotnet grpc add-url 
https://raw.githubusercontent.com/grpc/grpc/master/examples/protos/helloworld.proto 
-o Protos\helloword.proto

//更新远程proto文件到本地
dotnet grpc refresh 
https://raw.githubusercontent.com/grpc/grpc/master/examples/protos/helloworld.proto

//删除远程引用。只是移除映射，工程里的文件需要手动删除
dotnet grpc remove
https://raw.githubusercontent.com/grpc/grpc/master/examples/protos/helloworld.proto
```

**注意：想要自动生成服务文件，需要【编辑项目文件】，在项目文件的\<ItemGroup>节点配置.proto文件路径，如：`<Protobuf Include="Grpc\ordering.proto" GrpcServices="Server"/>`。**

**最佳实践**：①使用单独的Git仓库管理proto文件，②使用submodule将proto文件集成到工程目录中，③使用dotnet-grpc命令行添加proto文件及相关依赖包引用。备注：由 proto 生成的代码文件会存放在 obj 目录中，不会被签入到 Git 仓库。

**Polly：用失败重试机制提升服务可用性**

​	使用Polly时涉及的组件包有Polly，Polly.Extensions.Http，Microsoft.Extensions.Http.Polly。Polly具有的能力有失败重试，服务熔断，超时处理，舱壁隔离，缓存策略，失败降级，组合策略。使用步骤①定义要处理的异常类型或返回值，②定义要处理动作（重试、熔断、降级响应等），③使用定义的策略来执行代码。

​	适合失败重试的场景：①服务“失败”是短暂的，可自愈的(网络闪断)，②服务是幂等的，重复调用不会有副作用。

**最佳实践：** ① 设置失败重试次数，② 设置带有步长策略的失败等待间隔， ③ 设置降级响应， ④设置断路器

### 网关和BFF

​	BFF(Backend For Frontend，服务于前端的后端)，负责认证授权，服务聚合，为前端提供服务。打造网关：①添加包Ocelot 14.0.3，②添加配置文件ocelot.json，③添加配置 读取代码，④注册Ocelot服务，⑤注册Ocelot中间件

**网关模式**

![gateway_ZL](https://cdn.jsdelivr.net/gh/HolaAmigoV5/Images/DotNet/gateway_ZL.png)



![gateway_share](https://cdn.jsdelivr.net/gh/HolaAmigoV5/Images/DotNet/gateway_share.png)



![gateway_share_Aggregatetor](https://cdn.jsdelivr.net/gh/HolaAmigoV5/Images/DotNet/gateway_share_Aggregatetor.png)

![gateway_dedicated](https://cdn.jsdelivr.net/gh/HolaAmigoV5/Images/DotNet/gateway_dedicated.png)





#### 身份认证JWT

​	JWT(Json Web Tokens)，一种支持签名的数据结构，针对移动客户端场景使用。无密钥时是可以查看到JWT内部存储信息。JWT中的Header和Payload本质是把Json信息进行了Base64数据转换，不是加密的动作。本质不是信息加密Token，只是一个信息签名Token。

JWT主要由三部分组成：

1. Header，存放令牌类型、加密类型等信息；
2. Payload，表示令牌内容，预定义了部分字段信息，支持自定义；
3. Signature，根据Header、Payload和私有秘钥计算出来的签名。

![jwt_datastructure](https://cdn.jsdelivr.net/gh/HolaAmigoV5/Images/DotNet/jwt_datastructure.png)

​	启用JwtBearer身份认证需要引用Microsoft.AspNetCore.Authentication.JwtBearer。配置身份认证，需要在Ocelot网关配置身份认证，微服务配置认证与授权。

**JWT注意事项：** ① Payload信息不宜过大，② 不宜存储敏感信息，③ 微服务和网关之间共享JWT密钥。只有密钥相同时，JWT才能在各个站点之间共享。

Asp.net Core提供两种身份认证方案，Cookie和JWT认证。

#### 安全防范

​	防御黑客攻击的建议：不使用cookie来存储和传输身份信息，使用AntiforgeryToken机制来防御，避免使用get作为业务操作的请求方法。

**防跨站脚本攻击**：坏站点通过脚本自动访问好站点网页

```c#
//携带有HeaderName = "X-CSRF-TOKEN"才可以正常访问
services.AddAntiforgery(options =>{
	options.HeaderName = "X-CSRF-TOKEN";
});

//开启全局AntiforgeryToken验证。也可以在方法上标记[ValidateAntiForgeryToken]特性进行局部验证
services.AddMvc(options => options.Filters
	.Add(new AutoValidateAntiforgeryTokenAttribute()));
```

**防开放重定向攻击**：黑客将好站点的访问通过重定向一个假的“好站点”，用户在未察觉的时候输入用户名和密码登录，从而向黑客泄露了登录信息。**防范措施**：使用LocalRedirect来处理重定向，验证重定向的目标域名是否合法。

**防跨站脚本攻击**：黑客在好站点中植入木马脚本，用户请求好站点后，客户端执行木马执行像坏站点提交用户身份信息等。**防范措施**：①对用户提交内容进行验证，拒绝恶意脚本，②对用户提交的内容进行编码UrlEncoder、JavaScriptEncoder，③慎用HtmlString和HtmlHelper.Raw，④身份信息Cookie设置为httpOnly，⑤避免使用Path传递带有不受信任的字符，使用Query进行传递

**跨域请求**

**同源和跨域**：如果两个域名方案相同(HTTP/HTTPS)，主机(域名)相同，端口相同，则这两个域名是同源的。如果这三个有任意一个不同，则表示跨域。

**跨域请求(CORS)**：CORS是浏览器运行跨域发起请求“君子协定”，是浏览器行为协议，不会让服务器拒绝其他途径发起的HTTP请求，开启时需要考虑是否存在被恶意网站攻击的情形。

### 缓存相关

​	缓存是计算结果的“临时”存储和重复使用，缓存本质是“空间”换“时间”。B/S架构中，缓存一般存储在浏览器，反向代理服务器（负载均衡），应用进程内存，分布式存储系统中。缓存使用可能出现的问题：①缓存失效，导致数据不一致；②缓存穿透，查询无效数据时，导致缓存不生效，查询都落到数据库（建议：数据为null时，缓存强制返回一个默认值，避免穿透）；③缓存击穿，缓存失效瞬间，大量请求访问到数据库（建议：使用二级缓存策略，当一级缓存失效，允许一个请求落到数据库上帮助更新缓存数据，重置缓存有效时间，其他请求仍然通过缓存响应）；④缓存雪崩，大量缓存同一时间失效，导致数据库压力（建议：缓存失效时间策略定义均匀，避免同时出现失效）。使用到的组件如下：

1. ResponseCache
2. Microsoft.Extensions.Caching.Memory.IMemoryCache  //微软的MemoryCache组件
3. Microsoft.Extensions.Caching.Distributed.IDistributedCache  //微软的DistributedCache组件
4. EasyCaching  //donet社区的一个开源组件

内存缓存和分布式缓存区别：①内存缓存可以存储任意对象；②分布式缓存的对象需要支持序列化；③分布式缓存远程请求可能失败，内存缓存不会。

## Kubernetes部署

Kubernetes是一个用于自动部署、扩展和管理容器化应用程序的开源系统。部署拓扑图如下：

![deploy](https://cdn.jsdelivr.net/gh/HolaAmigoV5/Images/DotNet/deploy.png)





### 环境搭建

① 安装 Docker Desktop，② 安装 Kubernetes，③ 安装Helm，④ 部署基础设施。

### 应用部署

① 准备Dockerfile，② 构建镜像，③ 准备配置， ④ 部署应用

### 配置管理

ConfigMap：实现基本配置方案。

Apollo：分布式配置中心版本化管理配置。

### 健康检查

Liveness、Readiness、Startup探测集成实现高可用。搭建全量健康检查探针和看板。

ForwardedHeaders：确保服务在负载均衡下正常工作。

### 安全

强制HTTPS的两种方式（① Ingress强制HTTPS和应用强制HTTPS）

### 日志

​	EFK日志三件套集成（① Elasticsearch，存储；② Fluentd，收集器；③ Kibana，数据看板）。Exceptionless日志系统记录异常信息。如下图

![EFK_log](https://cdn.jsdelivr.net/gh/HolaAmigoV5/Images/DotNet/EFK_log.png)





![Exceptionless_log](https://cdn.jsdelivr.net/gh/HolaAmigoV5/Images/DotNet/Exceptionless_log.png)

追踪：集成SkyWalking.Net实现追踪

![Skywalking](https://cdn.jsdelivr.net/gh/HolaAmigoV5/Images/DotNet/Skywalking.png)



监控与告警：Prometheus与AlertManager，Granfana实现监控看板，Prometheus-net可以自定义监控指标

![Prometheus_png](https://cdn.jsdelivr.net/gh/HolaAmigoV5/Images/DotNet/Prometheus_png.png)