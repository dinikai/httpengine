﻿using System;
using System.Net;
using System.Text;

namespace HttpEngine.Core
{
    public class Model : IModel
    {
        public List<string> Routes { get; set; } = new();
        public string PublicDirectory { get; set; }
        public IModel Error404 { get; set; }
        public Layout Layout { get; set; }
        public bool UseLayout { get; set; } = true;
        public HttpApplication Application { get; set; }

        public virtual ModelResult OnRequest(ModelRequest request)
        {
            //throw new NotImplementedException();
            return new ModelResult();
        }

        public virtual void OnUse()
        {

        }

        protected ModelFile File(string fileName, ModelRequest request)
        {
            FileStream file = new FileStream(Path.Combine(PublicDirectory, fileName), FileMode.Open);
            byte[] buffer = new byte[file.Length];
            file.Read(buffer);
            file.Close();

            if (UseLayout)
            {
                ModelFile layout = Layout.OnRequest(request);
                layout.ParseView(new()
                {
                    ["body"] = Encoding.UTF8.GetString(buffer),
                }, false);
                return layout;
            } else
            {
                return new ModelFile(buffer);
            }
        }

        public static ModelFile RawFile(string fileName)
        {
            FileStream file = new FileStream(fileName, FileMode.Open);
            byte[] buffer = new byte[file.Length];
            file.Read(buffer);
            file.Close();

            return new ModelFile(buffer);
        }

        public ModelResult? CallModel<T>(ModelRequest request) where T : IModel
        {
            IModel? model = Application.Router.Models.FirstOrDefault(x => x is T);
            if (model == null)
                return null;

            return model.OnRequest(request);
        }

        public static SkipResult Skip()
        {
            return new SkipResult();
        }

        public static ModelResult Redirect(string url)
        {
            WebHeaderCollection headers = new()
            {
                { "Location", url }
            };
            return new ModelResult()
            {
                Headers = headers,
                StatusCode = 302
            };
        }

        public static ModelResult RedirectBack(ModelRequest request)
        {
            return Redirect(request.Headers["Referer"]!);
        }

        public static ModelResult Download(byte[] data, string fileName)
        {
            WebHeaderCollection headers = new()
            {
                { "Content-Disposition", $"attachment; filename={fileName}" }
            };
            return new ModelResult(data)
            {
                Headers = headers,
            };
        }

        protected void RemoveModel()
        {
            Application.RemoveModel(this);
        }
    }
}
