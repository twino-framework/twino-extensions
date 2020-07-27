# Twino Extensions

Twino Extensions project has useful extensions for twino projects.

## NuGet Packages

**[Twino Extensions ConsumerFactory](https://www.nuget.org/packages/Twino.Extensions.ConsumerFactory)**<br>
**[Twino Extensions Http](https://www.nuget.org/packages/Twino.Extensions.Http)**<br>
**[Twino Extensions Data](https://www.nuget.org/packages/Twino.Extensions.Data)**<br>


### Twino Extensions Consumer Factory

Consumer Factory is an extension for connecting Twino MQ easily. It creates a TmqStickyConnector and registers it to service provider. You can get that connector with ITwinoBus service type.

services.UseTwinoBus(cfg => cfg.AddHost("tmq://127.0.0.1:22200")
                               .AddTransientConsumers(typeof(Program)));


### Twino Extensions Http

HttpClient pool factory. You can use one pool as default or multiple pools with keys.

#### Basic Usage

    IServiceContainer container = new ServiceContainer();

    //pool size 32
    //configuration action is executed before each factory.Create() usage
    container.AddHttpClient(32, httpClient =>
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Token...");
    });

    //IHttpClientFactory is injectable
    IHttpClientFactory factory = container.Get<IHttpClientFactory>();
    HttpClient client = factory.Create();
    //use client here
            
            
#### Usage With Keys

    IServiceContainer container = new ServiceContainer();

    //for service a
    //pool size 32
    //configuration action is executed before each factory.Create() usage
    container.AddHttpClient("service-a", 16, httpClient =>
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Token-A...");
    });
    
    //for service b
    //pool size 32
    //configuration action is executed before each factory.Create() usage
    container.AddHttpClient("service-b", 8, httpClient =>
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Token-B...");
    });

    //IHttpClientFactory is injectable
    IHttpClientFactory factory = container.Get<IHttpClientFactory>();
    
    //get for service-a
    HttpClient clientA = factory.Create("service-a");
    
    //get for service-b
    HttpClient clientB = factory.Create("service-b");
    //use client here


### Twino Extensions Data

Data Context pool for DbContext objects. Implementation methods starts with AddData (you can still use AddDb methods of Microsoft Dependency Injection implementation).
Here is a quick example:

    IServiceContainer container = new ServiceContainer();

    //container.AddDataContextTransient
    //container.AddDataContextTransientPool
    container.AddDataContextScoped<MyContext>(o => o.UseNpgsql("connection-string"));
    container.AddDataContextScopedPool<MyContext>(o => o.UseNpgsql("connection-string"),
                                                  p =>
                                                  {
                                                      p.PoolMinSize = 20;
                                                      p.PoolMaxSize = 120;
                                                      p.MaximumLockDuration = TimeSpan.FromSeconds(60);
                                                      p.IdleTimeout = TimeSpan.FromSeconds(90);
                                                      p.WaitAvailableDuration = TimeSpan.FromMilliseconds(1500);
                                                      p.ExceedLimitWhenWaitTimeout = false;
                                                  });

    //create a scope for using Scoped registrations
    IContainerScope scope = container.CreateScope();
    
    //get db context from pool
    MyContext context = container.Get<MyContext>(scope);
