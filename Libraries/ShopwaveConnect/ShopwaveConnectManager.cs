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
using System.Collections.Specialized;

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
            var postData = new NameValueCollection();
            postData["access_type"] = this.AccessType;
            postData["redirect_uri"] = this.RedirectUrl.ToString();
            postData["client_id"] = this.ClientIdentifier;
            postData["scope"] = string.Join(",", this.Scope);
            postData["client_secret"] = this.ClientSecret;
            postData["code"] = this.AuthCode;
            postData["grant_type"] = "authorization_code";
            //String postData = "access_type=" + this.AccessType + "&redirect_uri=" + this.RedirectUrl.ToString() + "&client_id=" + this.ClientIdentifier + "&scope=" + string.Join(",", this.Scope) + "&client_secret=" + this.ClientSecret + "&code=" + this.AuthCode + "&grant_type=authorization_code";

            return new JavaScriptSerializer().Deserialize<Token>(MakeWebQuery(GetTokenUri(), "POST", null, null, null, postData));
        }

        public Token RefreshTokenCall(Token tokenVar)
        {

            var postData = new NameValueCollection();
            postData["access_type"] = this.AccessType;
            postData["redirect_uri"] = this.RedirectUrl.ToString();
            postData["client_id"] = this.ClientIdentifier;
            postData["scope"] = string.Join(",", this.Scope);
            postData["client_secret"] = this.ClientSecret;
            postData["refresh_token"] = tokenVar.refresh_token;
            postData["grant_type"] = "refresh_token";
            //String postData = "access_type=" + this.AccessType + "&redirect_uri=" + this.RedirectUrl.ToString() + "&client_id=" + this.ClientIdentifier + "&scope=" + string.Join(",", this.Scope) + "&client_secret=" + this.ClientSecret + "&refresh_token=" + tokenVar.refresh_token + "&grant_type=refresh_token";

            return new JavaScriptSerializer().Deserialize<Token>(MakeWebQuery(GetTokenUri(), "POST", null, null, null, postData));
        }

        public String MakeShopwaveApiCall(String endpoint, Token tokens, string method, Dictionary<string, string> headers, string postBody)
        {
            var postData = new NameValueCollection();
            postData["postBody"] = postBody;
            return MakeWebQuery(GetApiEndpoint(endpoint), method, tokens.token_type, tokens.access_token, headers, postData);
        }

        private Uri GetTokenUri()
        {
            return new Uri(OAuthBaseUrl + TokenUri);
        }

        private Uri GetApiEndpoint(String endpoint)
        {
            return new Uri(ApiBaseUrl + endpoint);
        }

        private string MakeWebQuery(Uri requestUrl, string method, string tokenType, string accessToken, Dictionary<string, string> headers, NameValueCollection postData)
        {
           
            var wb = new WebClient();

            if (headers != null)
            {
                foreach (var headerPair in headers)
                {
                    wb.Headers.Set(headerPair.Key, headerPair.Value);
                }
            }

            if (accessToken != null && tokenType != null)
            {
                wb.Headers.Set("Authorization", tokenType + " " + accessToken);
            }

            if (method == "POST" || method == "PUT")
            {

                wb.Headers.Set("Content-type", "application/x-www-form-urlencoded");
                var response = wb.UploadValues(requestUrl, method, postData);
                string result = System.Text.Encoding.UTF8.GetString(response);
                return result;
            }
            else
            {
                var response = wb.DownloadString(requestUrl);
                return response;
            }
            
        }
     }
}
