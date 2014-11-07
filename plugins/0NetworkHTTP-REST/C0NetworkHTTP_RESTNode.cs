#region usings
using System;
using System.ComponentModel.Composition;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

using VVVV.Core.Logging;

using RestSharp;
#endregion usings

// ToDo
//// Nice way to enter URL URI as in MQTT Node

namespace VVVV.Nodes
{
	#region PluginInfo
	[PluginInfo(Name = "HTTP-REST", Category = "Network", Version = "0", Help = "Basic template with one string in/out", Tags = "")]
	#endregion PluginInfo
	public class C0NetworkHTTP_RESTNode : IPluginEvaluate
	{
		#region enums
		public enum HttpMethod { GET, POST, PUT, DELETE }
		//public enum RequestFormat { JSON, XML }
		#endregion enums
		
		#region fields & pins
		[Input("BaseURL", StringType = StringType.URL, DefaultString = "http://localhost")]
		public ISpread<string> FInputBaseURL;
		
		[Input("Ressource", DefaultString = null)]
		public ISpread<string> FInputResourcePath;
		
		[Input("HttpMethod", DefaultEnumEntry = "GET")]
		public ISpread<HttpMethod> FInputHttpMethod;
		
		[Input("MimeType", DefaultString ="")]
		public ISpread<string> FInputMimeType;
		
		//[Input("RequestFormat", DefaultEnumEntry = "JSON")]
		//public ISpread<RequestFormat> FInputRequestFormat;
		
		[Input("Use Basic Authentication", IsBang = false, DefaultValue = 0, IsSingle = false)]
		IDiffSpread<bool> FInputUseBasicAuthentication;
		
		[Input("Username", DefaultString = null)]
		public ISpread<string> FInputUsername;
		
		[Input("Password", DefaultString = null)]
		public ISpread<string> FInputPassword;
		
		[Input("Content", DefaultString = null)]
		public ISpread<string> FInputContent;
		
		[Input("Execute", IsBang = true, DefaultValue = 0, IsSingle = false)]
		IDiffSpread<bool> FInputExecute;

		[Output("Output")]
		public ISpread<string> FOutputResponse;
		
		[Output("Status")]
		public ISpread<string> FOutputStatus;

		[Import()]
		public ILogger FLogger;
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			
			FOutputResponse.SliceCount = SpreadMax;
			FOutputStatus.SliceCount = SpreadMax;
			//// start doing stuff foreach spread item
			for (int i = 0; i < SpreadMax; i++)
			{
				try{
					var client = new RestClient();
					if (FInputExecute[i])
					{
						client.BaseUrl = new Uri(FInputBaseURL[i]);
						
						if(FInputUseBasicAuthentication[i])
						{
							client.Authenticator = new HttpBasicAuthenticator("username", "password");
						}
						
						
						var request = new RestRequest();
						switch(FInputHttpMethod[i].ToString())
						{
						case "GET":
							request.Method = Method.GET;
							break;
							
						case "POST":
							request.Method = Method.POST;
							break;
							
						case "PUT":
							request.Method = Method.PUT;
							break;
							
						case "DELETE":
							request.Method = Method.DELETE;
							break;
						}
						
						
						/*
						//  defines the Request serialisation format
						switch(FInputRequestFormat[i].ToString())
						{
						case "JSON":
							request.RequestFormat = DataFormat.Json;
							break;
							
						case "XML":
							request.RequestFormat = DataFormat.Xml;
							break;
						}*/
						

						request.Resource = FInputResourcePath[i];
						//request.AddHeader("Content-type", "application/json; charset=utf-8");
						//request.AddBody(FInputContent[i]);
						
						
						request.AddParameter(FInputMimeType[i], FInputContent[i], ParameterType.RequestBody);
						
						
						
						
						//request.RequestFormat = DataFormat.Json;
						//request.AddParameter(FInputContent[i]);
						//request.AddUrlSegment("customer", "CUSTOMER");
						//request.AddQueryParameterw
						
						
						IRestResponse response = client.Execute(request);
						FOutputResponse[i] = response.Content;
						FOutputStatus[i] = "ResponseStatus:       "+response.ResponseStatus.ToString()+"\n";
						FOutputStatus[i] += "StatusDescription:   "+response.StatusDescription +"\n";
						FOutputStatus[i] += "Request:             "+response.Request +"\n";
						FOutputStatus[i] += "ResponseUri:         "+response.ResponseUri +"\n";
						FOutputStatus[i] += "ErrorException:      "+response.ErrorException +"\n";
						FOutputStatus[i] += "ErrorMessage:        "+response.ErrorMessage +"\n";
					}
					
				}
				catch (Exception e)
				{
					FLogger.Log(e);
				}
				
			}	
		}

		//FLogger.Log(LogType.Debug, "Logging to Renderer (TTY)");
	}
}