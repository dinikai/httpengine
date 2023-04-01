using HttpEngine.Core;
using HttpEngine.Models;

var builder = new HttpApplicationBuilder();
var app = builder.Build();

app.UseModel<IndexModel>();
app.UseModel<DbTestModel>();

app.Run();