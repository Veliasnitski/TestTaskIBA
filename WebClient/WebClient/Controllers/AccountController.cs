using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.Stores.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebClient.Models;
using WebClient.Repositories;


namespace WebClient.Controllers
{
    public class AccountController : Controller
    {
        private WebAPIRepository<User> apiAccess = new WebAPIRepository<User>("users");
        HttpClient client = new HttpClient();

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            String messages = String.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors)
                                                          .Select(v => v.ErrorMessage + " " + v.Exception));

            if (ModelState.IsValid)
            {
                apiAccess.UrlAPI = "users";
                bool isLogin = (await apiAccess.GetAsync("login" + "/" + model.Login)) != null;
                if (!isLogin)
                {
                    User newUser = new User()
                    {
                        Login = model.Login,
                        Password = model.Password
                    };
                    await apiAccess.AddAsync(newUser);
                    await Authenticate(new User()
                    {
                        Login = model.Login,
                        Password = model.Password
                    });
                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "The login is already there..");
            }
            else ModelState.AddModelError("", messages);

            return View(model);
        }
        public async Task<IActionResult> Logout()
        {
         /*   foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
        */
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        private async Task<bool> Authenticate(User user)
        {

            apiAccess.UrlAPI = "identity/claim";

            var tmpj = JsonConvert.SerializeObject(user);
            var content = new StringContent(tmpj.ToString(), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(apiAccess.UrlAPI, content);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return false;
            }

            string responseBody = await response.Content.ReadAsStringAsync();
            IReadOnlyCollection<Claim> claims = JsonConvert.DeserializeObject<List<Claim>>(responseBody, new ClaimConverter());
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
            return true;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            String messages = String.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors)
                                                         .Select(v => v.ErrorMessage + " " + v.Exception));
            if (ModelState.IsValid)
            {
                apiAccess.UrlAPI = "users";
                bool isLogin = (await apiAccess.GetAsync("login" + "/" + model.Login)) != null;
                if (isLogin)
                {
                    User user = new User()
                    {
                        Login = model.Login,
                        Password = model.Password
                    };
                    bool isAut = await Authenticate(user);

                    if (isAut) return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Incorrect login and/or password");
            }
            else ModelState.AddModelError("", messages);
            return View(model);
        }
    }
}



