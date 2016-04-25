using System.Web.Mvc;
using Newtonsoft.Json;

namespace Article.WebSite.Component.SystemFramework.Models
{
	public class JsonpResult : ActionResult
	{
		public string Callback { get; private set; }
		public object Data { get; private set; }

		public JsonpResult(string callback, object data)
		{
			this.Callback = callback;
			this.Data = data;
		}

		public override void ExecuteResult(ControllerContext context)
		{
			(new JavaScriptResult() { Script = string.IsNullOrWhiteSpace(Callback) ? JsonConvert.SerializeObject(this.Data) : Callback + "(" + JsonConvert.SerializeObject(this.Data) + ")" }).ExecuteResult(context);
		}
	}
}
