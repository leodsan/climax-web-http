# Climax.Web.Http

A set of add ons and extensions for ASP.NET Web API.

## Contents

### Handlers

 - `ThreadCultureMessageHandler` - ensures culture is set (` Thread.CurrentThread.CurrentCulture` and `Thread.CurrentThread.CurrentUICulture`. Parsed from `Accept-Language` header or `AcceptLanguage` querystring
 
     config.MessageHandlers.Add(new ThreadCultureMessageHandler(setThreadCulture: true, setThreadUiCulture: true,  fallBackCulture: new CultureInfo("en-US")));
 
 - `HeadMessageHandler` - adds support for HEAD HTTP verb
 
    config.MessageHandlers.Add(new HeadMessageHandler());
 
### Services

 - `NonControllerHttpControllerTypeResolver` - allows you to use API controllers without "Controller" suffix
 
    config.Services.Replace(typeof(IHttpControllerTypeResolver), new NonControllerHttpControllerTypeResolver());
 
 - `PerControllerConfigActivator` - supports HttpConfiguration per controller that can be set up at runtime. Normally Web API only allows static per controller configuration
 
        config.AddControllerConfigurationMap(new Dictionary<Type, Action<HttpControllerSettings>>{ {typeof(TestController), settings =>
        {
            settings.Formatters.Clear();
            settings.Formatters.Add(new JsonMediaTypeFormatter());                    
        }});
        config.Services.Replace(typeof(IHttpControllerActivator), new PerControllerConfigActivator());
 
 - `SmartHttpActionInvoker` - saves the information of the action return type under `RuntimeReturnType` key of request properties. Web API normally only exposes the type information from the action descriptor.
 
    config.Services.Replace(typeof(IHttpActionInvoker), new SmartHttpActionInvoker());
    //then anywhere:
    config.Get<Type>("RuntimeReturnType");
 
 - `JsonContentNegotiator` - switch off content negotiation in your API completely - support JSON only, in the most efficient way
 
    config.Services.Replace(typeof(IContentNegotiator), new JsonContentNegotiator(jsonFormatter));
 
 - `RouteDataValuesOnlyAttribute` - disables binding of simple parameters from querystring (only from route data). Applied at controller level.
 
    [RouteDataValuesOnly]
    public class TestController : ApiController
    {
        //foo will only be bound from route data not from querystring
        public string Get(string foo)
        {
            return foo;
        }
    }
 
 - `InjectParameterBinding` - allows you to inject your dependencies (services and such, that are resolved from Web API dependency resolver) directly into actions, rather than through constructor

Instead of:
    
    public class MyController : ApiController
    {
        private readonly IFooService _fooService;
        private readonly IBarService _barService;

        public MyController(IFooService fooService, IBarService barService)
        {
            _fooService = fooService;
            _barService = barService;
        }

        [Route("foo")]
        public void PostFoo(FooModel foomodel)
        {
            _fooService.Process(foomodel);
        }

        [Route("bar")]
        public void PostBar(BarModel barmodel)
        {
            _barService.Process(barmodel);
        }
    }
    
You can do:

    public class MyController : ApiController
    {
        [Route("foo")]
        public void PostFoo(FooModel foomodel, [Inject]IFooService fooService)
        {
            fooService.Process(foomodel);
        }

        [Route("bar")]
        public void PostBar(BarModel barmodel, [Inject]IBarService barService)
        {
            barService.Process(barmodel);
        }
    }

You don't have to do anything else - as long as Web API dependency injection is wired up globally (which it should - for constructor injection), this will just work.

 - `Versioning` - support versioning based on attribute routing - using URI versioning, header versioning or content type versioning
 
### Action Results
 
 - `FileHttpActionResult` - return a file as `StreamContent`
 - `FileWithDispositionHttpActionResult` - return a file as `ByteArrayContent`, with a relevant `Content-Disposition` header
 - `PlainTextResult` - return plain text result
 
### Extensions
 
A bunch of extension methods for `HttpRequestMessage` and `HttpConfiguration`.

### Other

 - `StringArrayModelBinder` - bind a collection in an action from a comma separated querystring value
 - `CheckModelStateAttribute` - validate model state through action filter