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
            var response = new ModelResponse();
            if (request.Method == "POST")
            {
                Db.Add(request.Arguments["add"]);
            }

            string dbString = "";
            foreach (var item in Db)
            {
                dbString += $"<li>{item}</li>";
            }

            var viewReplace = new Dictionary<string, object>()
            {
                ["title"] = "Database",
                ["db"] = dbString,
            };

            response.ResponseData = ViewParser.Parse(File("Pages/Database.html"), viewReplace);
            return response;
        }
    }
}
