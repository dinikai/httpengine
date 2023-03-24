using HttpEngine.Core;
using HttpEngine.Models;

IModel indexModel = new IndexModel();
IModel error404Model = new Error404Model();
IModel dbTestModel = new DbTestModel();

Dictionary<string, IModel> routes = new()
{
    ["/"] = indexModel,
    ["/Index"] = indexModel,
    ["/Database"] = dbTestModel
};

Router router = new Router(
    publicDirectory: $@"{Environment.CurrentDirectory}/Public",
    routes: routes,
    error404Page: error404Model
);

HttpApplication app = new HttpApplication(router);

app.Run();