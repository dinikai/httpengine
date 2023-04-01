using HttpEngine.Core;
using System;

namespace HttpEngine.Models
{
    internal class DbTestModel : Model
    {
        public List<string> Db { get; set; }

        public DbTestModel()
        {
            Db = new List<string>();
            Routes = new() { "/Database" };
        }

        public override ModelResponse OnRequest(ModelRequest request)
        {
            if (request.Handler == "remove")
            {
                if (request.Arguments.ContainsKey("i"))
                {
                    int index = Convert.ToInt32(request.Arguments["i"]);
                    if (index < Db.Count) Db.RemoveAt(index);
                }
            }

            byte[] responseFile = File("Pages/Database.html");
            var response = new ModelResponse();
            if (request.Method == "POST")
            {
                Db.Add(request.Arguments["add"]);
            }

            string dbString = "";
            for (int i = 0; i < Db.Count; i++)
            {
                dbString += ViewParser.GetSection(responseFile, "dbItem", new()
                {
                    ["item"] = Db[i],
                    ["index"] = i,
                });
            }

            var viewReplace = new Dictionary<string, object>()
            {
                ["title"] = "Database",
                ["db"] = dbString,
            };

            response.ResponseData = ViewParser.Parse(ref responseFile, viewReplace);
            return response;
        }
    }
}
