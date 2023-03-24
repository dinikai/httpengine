using HttpEngine.Core;
using System;

namespace HttpEngine.Models
{
    internal class DbTestModel : IModel
    {
        public List<string> Db { get; set; }

        public DbTestModel()
        {
            Db = new List<string>();
        }

        public ModelResponse OnRequest(ModelRequest request)
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

            response.ViewData = new()
            {
                ["db"] = dbString
            };

            response.ResponseFile = "Pages/Database.html";
            return response;
        }
    }
}
