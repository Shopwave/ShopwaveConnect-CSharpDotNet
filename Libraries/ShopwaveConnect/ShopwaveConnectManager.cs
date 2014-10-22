using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.Diagnostics;
using System.IO;
using System.Web.Script.Serialization;

namespace ShopwaveConnect
{
    public sealed class ShopwaveConnectManager
    {
        private const String OAuthBaseUrl = "http://secure.merchantstack.com/";
        private const String ApiBaseUrl = "http://api.merchantstack.com/";
        private const String AuthUri = "oauth/authorize";
        private const String TokenUri = "oauth/token";
        private const String LogoutUri = "logout";
        private readonly String ClientIdentifier;
        private readonly String ClientSecret;
        private readonly Uri RedirectUrl;
        private readonly String[] Scope;

        public String AccessType;
        public String ResponseType;

        public String AuthCode;

        public class Token
        {
            public string refresh_token { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }
            public string access_token { get; set; }
        }
 
        public ShopwaveConnectManager(String clientIdentifier, String clientSecret, Uri redirectUrl, String[] scope)
        {
            this.ClientIdentifier = clientIdentifier;
            this.ClientSecret = clientSecret;
            this.RedirectUrl = redirectUrl;
            this.Scope = scope;
            this.AccessType = "online";
            this.ResponseType = "code";
        }

        public string GetAuthoriseApplicationUri ()
        {
            return OAuthBaseUrl + AuthUri + "?access_type=" + this.AccessType + "&redirect_uri=" + this.RedirectUrl.ToString() + "&client_id=" + this.ClientIdentifier + "&scope=" + string.Join(",", this.Scope) + "&response_type=" + this.ResponseType;
        }

        public string GetLogoutUri()
        {
            return OAuthBaseUrl + LogoutUri + "?access_type=" + this.AccessType + "&redirect_uri=" + this.RedirectUrl.ToString() + "&client_id=" + this.ClientIdentifier + "&scope=" + string.Join(",", this.Scope) + "&response_type=" + this.ResponseType;
        }

        public Token MakeTokenCall()
        {
            String postData = "access_type=" + this.AccessType + "&redirect_uri=" + this.RedirectUrl.ToString() + "&client_id=" + this.ClientIdentifier + "&scope=" + string.Join(",", this.Scope) + "&client_secret=" + this.ClientSecret + "&code=" + this.AuthCode + "&grant_type=authorization_code";
            
            return new JavaScriptSerializer().Deserialize<Token>(MakeWebQuery(GetTokenUri(), "POST", null, null, null, postData));
        }

        public Token RefreshTokenCall(Token tokenVar)
        {
            String postData = "access_type=" + this.AccessType + "&redirect_uri=" + this.RedirectUrl.ToString() + "&client_id=" + this.ClientIdentifier + "&scope=" + string.Join(",", this.Scope) + "&client_secret=" + this.ClientSecret + "&refresh_token=" + tokenVar.refresh_token + "&grant_type=refresh_token";

            return new JavaScriptSerializer().Deserialize<Token>(MakeWebQuery(GetTokenUri(), "POST", null, null, null, postData));
        }

        public String MakeShopwaveApiCall(String endpoint, Token tokens, string method, Dictionary<string, string> headers, string postBody)
        {
            return MakeWebQuery(GetApiEndpoint(endpoint), method, tokens.token_type, tokens.access_token, headers, postBody);
        }

        private Uri GetTokenUri()
        {
            return new Uri(OAuthBaseUrl + TokenUri);
        }

        private Uri GetApiEndpoint(String endpoint)
        {
            return new Uri(ApiBaseUrl + endpoint);
        }

        private string MakeWebQuery(Uri requestUrl, string method, string tokenType, string accessToken, Dictionary<string, string> headers, string postData)
        {
            WebRequest webRequest = WebRequest.Create(requestUrl);
            webRequest.Method = method;
            webRequest.Headers.Add("Accept-Tenant", "uk");
            webRequest.Headers.Add("Accept-Language", "en-GB");
            webRequest.Headers.Add("Accept-Charset", "utf-8");

            if(headers != null)
            {
                foreach (var headerPair in headers)
                {
                    webRequest.Headers.Add(headerPair.Key, headerPair.Value);
                }
            }

            if (accessToken != null && tokenType != null)
            {
                webRequest.Headers.Add("Authorization", tokenType + " " + accessToken);
            }

            Stream dataStream;
            
            if(method == "POST")
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(postData.ToString());

                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.ContentLength = byteArray.Length;

                dataStream = webRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
            }

            WebResponse response = webRequest.GetResponse();
            dataStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();

            reader.Close();
            response.Close();

            return responseFromServer;
        }
     }
}
