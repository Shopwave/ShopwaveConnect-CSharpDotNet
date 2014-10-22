using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using System.Web.Script.Serialization;
using ShopwaveConnect;

namespace ASPDotNetWebApplication.Controllers
{
    public class HomeController : Controller
    {
        /* Your Shopwave ClientIdentifier (e.g. js7woa9ro028djsnakf778sn3wiam3ond274knao) */
        public const string ClientIdentifier = "SHOPWAVE_CLIENT_IDENTIFIER";

        /* Your Shopwave ClientSecret (e.g. 76h4389732ru2039r20ruju023r9u2309jk8sna0) */
        public const string ClientSecret = "SHOPWAVE_CLIENT_SECRET";

        /* Your Shopwave RedirectUri (e.g. http://my.app) */
        public readonly Uri RedirectUri = new Uri("SHOPWAVE_REDIRECT_URI");

        /* Your Shopwave Scope */
        public readonly string[] Scope =  {"user","application", "merchant","store", "product", "category", "basket", "promotion", "log", "supplierStore", "supplier", "invoice", "stock"};

        /* Example Shopwave API Objects */

        public class Employee
        {
            public int roleId { get; set; }
            public int merchantId { get; set; }
        }

        public new class User
        {
            public string id { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string email { get; set; }
            public Employee employee { get; set; }
        }

        public class UserGet
        {
            public User user { get; set; }
        }


        public ActionResult Index()
        {
            string code = Request.QueryString["code"];
            ShopwaveConnectManager connector = new ShopwaveConnectManager(ClientIdentifier, ClientSecret, RedirectUri, Scope);

            /* Check for existing session */

            if(code != null && code.Length > 0)
            {
                if (Session["Token"] == null)
                {
                    connector.AuthCode = code;

                    /* Fetch access and refresh token */

                    ShopwaveConnectManager.Token token = connector.MakeTokenCall();

                    /* Store the token within the user session */ 

                    Session["Token"] = token;

                    return Redirect("/");
                }
            }

            /* Check for existing session */

            if (Session["Token"] != null)
            {
                ViewBag.LoggedIn = true;

                Dictionary<string, string> headers = new Dictionary<string, string>()
                {
                    { "x-accept-version", "0.4"}
                };

                string responseString = connector.MakeShopwaveApiCall("user",(ShopwaveConnectManager.Token) Session["Token"], "GET", headers, null);

                UserGet userGet = new JavaScriptSerializer().Deserialize<UserGet>(responseString);

                ViewBag.User = userGet.user;
            }
            else
            {
                ViewBag.LoggedIn = false;
                ViewBag.User = null;
            }

            return View();
        }

        public ActionResult RedirectAuth()
        {
            ShopwaveConnectManager connector = new ShopwaveConnectManager(ClientIdentifier, ClientSecret, RedirectUri, Scope);

            /**
             * As we are building a web app, we are redirecting the client to the auth URL.
             * If you are building a desktop or windows phone application, you must create a WebView 
             * and load the GetAuthoriseApplicationUri() on the the WebView where the user will be 
             * presented with the login screen
             */

            return Redirect(connector.GetAuthoriseApplicationUri());
        }

        public ActionResult Logout()
        {
            ShopwaveConnectManager connector = new ShopwaveConnectManager(ClientIdentifier, ClientSecret, RedirectUri, Scope);

            Session.Remove("Token");

            /**
             * As we are building a web app, we are redirecting the client to the logout URL.
             * If you are building a desktop or windows phone application, you must create a WebView 
             * and load the GetLogoutUri() on the the WebView where the user will be presented with 
             * the login screen
             */

            return Redirect(connector.GetLogoutUri());
        }
    }
}
