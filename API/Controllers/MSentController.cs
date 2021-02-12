/*
=============================================
Author      : <ยุทธภูมิ ตวันนา>
Create date : <๑๑/๐๒/๒๕๖๔>
Modify date : <๑๑/๐๒/๒๕๖๔>
Description : <>
=============================================
*/

using System;
using System.IO;
using System.Net;
using System.Net.Http;
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

	[RoutePrefix("MSent")]
	public class MSentController : ApiController
	{
		[Route("Version")]
		[HttpGet]
		public HttpResponseMessage MSent(string userCode)
		{
			var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://dcu-sitapi.mahidol.ac.th/service/api/v1/version?userCode=" + userCode);
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.Method = "GET";
			httpWebRequest.Headers["language"] = "TH";
			httpWebRequest.Headers["api-client-id"] = "SIT_KEY_EPROFILE";
			httpWebRequest.Headers["api-client-secret"] = "d0d96380-aae2-4ebf-9f5b-d771395dfaf8";

			var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			using (var sr = new StreamReader(httpWebResponse.GetResponseStream()))
			{
				var result = sr.ReadToEnd();

				return Request.CreateResponse(HttpStatusCode.OK, APIResponse.GetData(JsonConvert.DeserializeObject<object>(result)));
			};
		}
	}
}