using CMSClientApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CMSClientApp.Controllers
{
    public class UserListingController : Controller
    {
        public static string baseUrl = "https://localhost:44385/api/users/";
        public async Task<IActionResult> Index()
        {
            var users = await GetAllUser();
            return View(users);
        }
        [HttpGet]
        public async Task<List<Users>> GetAllUser()
        {
            //use access token to call api
            var accessToken = HttpContext.Session.GetString("JWTToken");
            var url = baseUrl;
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            string jsonstr = await client.GetStringAsync(url);

            var res = JsonConvert.DeserializeObject<List<Users>>(jsonstr).ToList();

            return res;
        }
    }
}
