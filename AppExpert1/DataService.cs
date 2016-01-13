using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;

using Newtonsoft.Json;


namespace AppExpert1
{
	public class DataService
	{
		public static async Task<dynamic> getDataFromService(string queryString)
		{
			
			/*
			 * Pas sur Mac
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create (queryString);

			var response = await request.GetResponseAsync ().ConfigureAwait (false);
			var stream = response.GetResponseStream ();

			var streamReader = new StreamReader (stream);
			string responseText = streamReader.ReadToEnd ();
			*/

			HttpRequestMessage request = new HttpRequestMessage (HttpMethod.Get, queryString);
			HttpClient httpClient = new HttpClient ();

			HttpResponseMessage httpResponse = await httpClient.SendAsync (request);

			string responseText = await httpResponse.Content.ReadAsStringAsync ();

			dynamic data = JsonConvert.DeserializeObject (responseText);
			return data;
		}
	}
}

