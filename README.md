# httpengine
Simple **HTTP** server with routing

## Views and public files
All public files are in /Public directiory.

### Files with tags
```html
<span>@tag</span>
```
That **@tag** will be replaced with *ViewParser*'s `Parse` method.

## Models
All models must inherit `Model` class: it has OnRequest method which call when client requests a page with this model.
That method returns a `ModelResponse` class:
```c#
public class ModelResponse
{
  public byte[] ResponseData { get; set; } // Response buffer
}
```

## *HttpApplication*, *HttpApplicationBuilder* and *Router*
The `HttpApplicationBuilder` is class which builds a HttpApplication. This class represents web-application:
```c#
var builder = new HttpApplicationBuilder();
var app = builder.Build();
```

`HttpApplication` has a *UseModel* method, that connecting a model to application:
```c#
app.UseModel<MyModel>();
```
you also can explicitly declare an model if you need:
```c#
app.UseModel(new MyModel());
```

`Run()` method runs an application:
```c#
app.Run();
```

`Router` class routes client request. It takes public files directory and error 404 model:
```c#
Router router = new Router(
    publicDirectory: $@"{Environment.CurrentDirectory}/Public",
    error404Page: error404Model,
);
```
> **Note**
> Router will be declared implicitly in `HttpApplicationBuilder`, but you can explicitly declare it in `HttpApplicationBuilderOptions
