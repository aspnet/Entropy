using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;

namespace Microsoft.AspNetCore.Mvc
{
    public class SmartJsonResult : ActionResult
    {
        public SmartJsonResult() : base()
        {
        }

        public JsonSerializerSettings Settings { get; set; }

        public object Data { get; set; }

        public int? StatusCode { get; set; }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            //if (!context.IsChildAction)
            //{
            //    if (StatusCode.HasValue)
            //    {
            //        context.HttpContext.Response.StatusCode = StatusCode.Value;
            //    }
            //    context.HttpContext.Response.ContentType = "application/json";
            //    context.HttpContext.Response.ContentEncoding = Encoding.UTF8;
            //}
            var settings = Settings ?? JsonSerializerSettingsProvider.CreateSerializerSettings();
            return context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(Data, settings));
        }
    }
}