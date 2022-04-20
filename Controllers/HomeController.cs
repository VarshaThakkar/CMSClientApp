using CMSClientApp.Models;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace CMSClientApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _clientFactory;
        public HomeController(ILogger<HomeController> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> LoginUser(LoginInfo user)
        {
            if (ModelState.IsValid)
            {
                if (user != null)
                {
                    using (var httpClient = new HttpClient())
                    {
                        StringContent stringContent = new(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                        using (var response = await httpClient.PostAsync("https://localhost:44385/api/users/login", stringContent))
                        {
                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                string token = await response.Content.ReadAsStringAsync();
                                if (token == null)
                                {
                                    TempData["message"] = "Incorrect Email or Password!";
                                    return RedirectToAction("LoginUser", "Home");
                                }
                                else
                                {
                                    TempData["message"] = "Successfully Logged In !";
                                    HttpContext.Session.SetString("JWTToken", token);
                                    return Redirect("~/Dashboard/Index");
                                }
                            }
                            else
                            {
                                TempData["message"] = "Not Found Such a User";
                                return RedirectToAction("LoginUser", "Home");
                            }
                        }
                    }
                }
            }
            return Redirect("~/Home/Index");
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(Users user)
        {
            if (ModelState.IsValid)
            {

                var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:44385/api/users/register");
                if (user != null)
                {
                    request.Content = new StringContent(JsonConvert.SerializeObject(user),
                        System.Text.Encoding.UTF8, "application/json");
                }
                var client = _clientFactory.CreateClient();
                HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    TempData["message"] = "Registered Successfully!";
                    return RedirectToAction("Register", "Home");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    TempData["message"] = "User With The Same Name Already Exists !";
                    return RedirectToAction("Register", "Home");
                }
            }
            return RedirectToAction("Register", "Home");

        }
        public IActionResult Logoff()
        {
            //app user
            HttpContext.Session.Clear();//clear token
            TempData["message"] = "You are logged out";

            //google user

            if (HttpContext.Request.Cookies.Count > 0)
            {
                //Check for the cookie value with the name mentioned for authentication and delete each cookie
                var siteCookies = HttpContext.Request.Cookies.Where(c => c.Key.Contains(".AspNetCore.") || c.Key.Contains("Microsoft.Authentication"));
                foreach (var cookie in siteCookies)
                {
                    Response.Cookies.Delete(cookie.Key);
                }
            }
            //signout with any cookie present 
            HttpContext.SignOutAsync();

            return Redirect("~/Home/Index");
        }
       
        public IActionResult Privacy()
        {
            return View();
        }
        [AllowAnonymous]       
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse"), AllowRefresh = true };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }
        public async Task<IActionResult> GoogleResponse()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");
            var expiresAt = DateTimeOffset.Parse(await HttpContext.GetTokenAsync("expires_at"));

            if (!authenticateResult.Succeeded)
            {
                return BadRequest();
            }
            if (authenticateResult.Principal.Identities.ToList()[0].AuthenticationType.ToLower() == "google")
            {
                if (authenticateResult.Principal != null)
                {
                    var googleAccountId = authenticateResult.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    var claimsIdentity = new ClaimsIdentity();
                    if (authenticateResult.Principal != null)
                    {
                        var details = authenticateResult.Principal.Claims.ToList();
                        // claimsIdentity.AddClaim(authenticateResult.Principal.FindFirst(ClaimTypes.NameIdentifier));// Full Name Of The User
                        claimsIdentity.AddClaim(authenticateResult.Principal.FindFirst(ClaimTypes.Email)); // Email Address of The User

                        ValidateGoogleToken(accessToken);
                        // await HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));
                        // return RedirectToAction("Index", "Dashboard");
                    }
                }
            }
            return RedirectToAction("Index", "Home");
            //    var claims = authenticateResult.Principal.Identities
            //.FirstOrDefault().Claims.Select(claim => new
            //{
            //    claim.Issuer,
            //    claim.OriginalIssuer,
            //    claim.Type,
            //    claim.Value
            //});

            //    return Json(claims);
        }
        private const string GoogleApiTokenInfoUrl = "https://www.googleapis.com/oauth2/v1/userinfo?alt=json&access_token={0}";

        public JsonResult ValidateGoogleToken(string providerToken)
        {
            var httpClient = new HttpClient();
            var requestUri = new Uri(string.Format(GoogleApiTokenInfoUrl, providerToken));
            HttpResponseMessage httpResponseMessage;
            try
            {
                httpResponseMessage = httpClient.GetAsync(requestUri).Result;
            }
            catch (Exception ex)
            {
                return null;
            }

            if (httpResponseMessage.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }
            var response = httpResponseMessage.Content.ReadAsStringAsync().Result;

            return Json(response);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
