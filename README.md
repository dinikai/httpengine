# httpengine
Simple **HTTP** MVC server with Router

## Views and public files
All public files are in /Public directiory.

### Files with tags
```html
<span>@tag</span>
```
That **@tag** will be replaced with model's ViewData dictionary

## Models
All models must realize `IModel` interface: it has OnRequest method which call when client requests a page with this model.
That method returns a `ModelResponse` class:
```c#
public class ModelResponse
{
  public string ResponseFile { get; set; } = string.Empty; // Path of file to load
  public Dictionary<string, object> ViewData { get; set; } = new(); // Tag replace dictionary
}
```

## *HttpApplication* and *Router*
The `HttpApplication` class represents web-application, it takes **Router** object as argument:
```c#
HttpApplication app = new HttpApplication(router);
```
`Run()` method runs an application:
```c#
app.Run();
```

`Router` class routes client request by dictionary. It takes public files directory, dictionary with routes and error 404 model:
```c#
Router router = new Router(
    publicDirectory: $@"{Environment.CurrentDirectory}/Public",
    routes: routes,
    error404Page: error404Model
);
```

Routes dictionary has `Dictionary<string, IModel>` signature. Example:
```c#
Dictionary<string, IModel> routes = new()
{
    ["/"] = indexModel,
    ["/Index"] = indexModel,
    ["/Database"] = dbTestModel
};
```
