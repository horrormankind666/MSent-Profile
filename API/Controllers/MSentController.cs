/*
=============================================
Author      : <ยุทธภูมิ ตวันนา>
Create date : <๑๑/๐๒/๒๕๖๔>
Modify date : <๑๘/๐๒/๒๕๖๔>
Description : <>
=============================================
*/

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;

namespace API.Controllers
{
	public class APIResponse
	{
		public bool status { get; set; }

		public object data { get; set; }

		public string message { get; set; }

		public APIResponse(bool status = true, string message = null)
		{
			this.status = status;
			this.message = (!status ? message : null);
		}

		public static APIResponse GetData(dynamic ob, bool isAuthen = true, string message = null)
		{
			APIResponse obj = null;

			try
			{
				obj = new APIResponse
				{
					data = ob
				};

				if (!isAuthen)
					obj = new APIResponse(false, (String.IsNullOrEmpty(message) ? "permissionNotFound" : message));
			}
			catch (Exception ex)
			{
				obj = new APIResponse(false, ex.Message);
			}

			return obj;
		}
	}

	public class MSentController : ApiController
	{
		[Route("~/Accepted")]
		[HttpGet]
		public HttpResponseMessage MSent(
			string route,
			string id = "",
			string userCode = ""
		)
		{
			string domain = System.Configuration.ConfigurationManager.AppSettings["MSentAPIURL"].ToString();
			string param = String.Empty;

			if (route.Equals("Version"))						param = ("?userCode=" + userCode);
			if (route.Equals("TermsAndConditions")) param += ("?termsAndCondId=" + id + "&userCode=" + userCode);
			if (route.Equals("PrivacyPolicy"))			param += ("?privacyPolicyId=" + id + "&userCode=" + userCode);
			if (route.Equals("Consent"))						param += ("?consentId=" + id + "&userCode=" + userCode);

			var httpWebRequest = (HttpWebRequest)WebRequest.Create(domain + route.ToLower() + param);
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.Method = "GET";
			httpWebRequest.Headers["language"] = HttpContext.Current.Request.Headers["lang"];
			httpWebRequest.Headers["api-client-id"] = HttpContext.Current.Request.Headers["clientid"];
			httpWebRequest.Headers["api-client-secret"] = HttpContext.Current.Request.Headers["clientsecret"];

			var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			using (var sr = new StreamReader(httpWebResponse.GetResponseStream()))
			{
				var result = sr.ReadToEnd();

				return Request.CreateResponse(HttpStatusCode.OK, APIResponse.GetData(JsonConvert.DeserializeObject<object>(result)));
			};
		}
	}
}