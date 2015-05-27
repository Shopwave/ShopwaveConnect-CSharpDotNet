ShopwaveConnect-CSharpDotNet
============================

<p>A C#.net library for ShopwaveConnect with an accompanying ASP.net example project.</p>

<h2>Required Class Libraries</h2>

<p>Each of the following libraries must be included in your C# implementation file. An example of this can be found in <strong>Examples/ASPDotNetWebApplication/ASPDotNetWebApplication/Controllers/HomeController.cs</strong>.</p>

```C#
using ShopwaveConnect;
using System.Web;
```

<h2>Required Parameters</h2>

<p>Each of the following parameters will have to be supplied in the code in order to communicate with the ShopwaveConnect API. An example of this can be found in <strong>Examples/ASPDotNetWebApplication/ASPDotNetWebApplication/Controllers/HomeController.cs</strong>.</p>

```C#
/* Your Shopwave ClientIdentifier (e.g. js7woa9ro028djsnakf778sn3wiam3ond274knao) */
public const string ClientIdentifier = "SHOPWAVE_CLIENT_IDENTIFIER";

/* Your Shopwave ClientSecret (e.g. 76h4389732ru2039r20ruju023r9u2309jk8sna0) */
public const string ClientSecret = "SHOPWAVE_CLIENT_SECRET";

/* Your Shopwave RedirectUri (e.g. http://my.app) */
public readonly Uri RedirectUri = new Uri("SHOPWAVE_REDIRECT_URI");

/* Your Shopwave Scope */
public readonly string[] Scope =  {"user","application", "merchant","store", "product", "category", "basket", "promotion", "log", "supplierStore", "supplier", "invoice", "stock"};

```
<h2>Using the Library</h2>

<p>Each of the following code snipets can be found in <strong>Examples/ASPDotNetWebApplication/ASPDotNetWebApplication/Controllers/HomeController.cs</strong>.

<h3>Initialisation</h3>

```C#
ShopwaveConnectManager connector = new ShopwaveConnectManager(ClientIdentifier, ClientSecret, RedirectUri, Scope);
```

<h3>Authorise</h3>

<h4>ASP.net</h4>

```C#
Redirect(connector.getAuthoriseApplicationUri());
```

<h3>Fetch Token</h3>

```C#
ShopwaveConnectManager.Token token = connector.MakeTokenCall();
```

<h3>Make API Call</h3>

```C#
connector.makeShopwaveApiCall("API_ENDPOINT", "OAUTH2_TOKEN", "METHOD", "HEADERS_DICTIONARY", "postBody={POST_BODY_JSON}")
connector.makeShopwaveApiCall("user",(OAuth2.Token) Session["Token"], "GET", headers, null);
```
