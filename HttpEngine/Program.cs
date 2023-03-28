using HttpEngine.Core;
using HttpEngine.Models;
using System.Net;

var builder = new HttpApplicationBuilder();
var app = builder.Build();
app.Listener.Prefixes.Add("http://localhost:8008/");

app.UseModel<IndexModel>();
app.UseModel<DbTestModel>();

app.Run();