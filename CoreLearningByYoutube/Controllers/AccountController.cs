using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreLearningByYoutube.Models;
using CoreLearningByYoutube.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CoreLearningByYoutube.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// 檢查email是否已使用
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        /// 為什麼要同時存在post和get?因為當使用者輸入email欄位時就可以發出get 請求檢查;form submit為post請求        
        //[HttpPost][HttpGet]//此行可以使用下行替代
        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email:{email} 已存在");
            }
        }

        /// <summary>
        /// 註冊view
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// 註冊
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                //新增IdentityUuser
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    City = model.City
                };
                var result = await _userManager.CreateAsync(user, model.Password);

                //新增成功
                if (result.Succeeded)
                {
                    //註冊新使用者後，假設目前已登入且擁有"Admin"角色權限，重新導向ListUsers
                    if (_signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListUsers", "Administration");
                    }
                    //登入
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("index", "home");
                }

                //新增不成功
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        /// <summary>
        /// 登入view
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View(model);
        }

        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl">當未登入時，點擊功能跳轉登入view，此時returnUrl不為空</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                //取得登入結果
                //第三個參數為是否使用cookie
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    //1.Url.IsLocalUrl(returnUrl):檢查是否為本機網站
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        /*
                         * open redirect attack...
                         * 假設returnUrl是釣魚網站網頁，登入後將會重新導覽至該釣魚網站
                        */

                        return Redirect(returnUrl);
                        //2.或者導覽至本機網站
                        //return LocalRedirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("index", "home");
                    }
                }

                //key用來表示哪個欄位的錯誤
                //ModelState.AddModelError("Password", "無效登入");

                //假設key為空，則須有地方承接顯示錯誤訊息，e.g.:<div asp-validation-summary="All" class="text-danger"></div>
                ModelState.AddModelError(string.Empty, "無效登入");
            }

            return View(model);
        }

        /// <summary>
        /// 取得google登入
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl">當未登入時，點擊功能跳轉登入view，此時returnUrl不為空</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            //google登入後的導向網址
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account",
                                    new { ReturnUrl = returnUrl });

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        /// <summary>
        /// google驗證後的導向
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [AllowAnonymous]        
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            var loginViewModel = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            //1.檢查google是否回傳錯誤
            if(remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View("Login", loginViewModel);
            }

            //2.取得外部登入資訊
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if(info == null)
            {
                ModelState.AddModelError(string.Empty, $"Error loading external login information.");
                return View("Login", loginViewModel);
            }

            //3.使用外部資訊登入網站
            //isPersisten:cookie
            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                                        info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            //4.檢查登入結果
            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                //5.取得登入者google email
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);

                //6.檢查email
                if(email != null)
                {
                    //7.取得使用者資訊by email
                    var user = await _userManager.FindByEmailAsync(email);

                    //8.無法取得使用者資訊則新增
                    if(user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email =info.Principal.FindFirstValue(ClaimTypes.Email)
                        };

                        await _userManager.CreateAsync(user);
                    }

                    //9.新增使用者資訊，AspNetUsers,AspNetUserLogin資料表
                    await _userManager.AddLoginAsync(user, info);
                    //10.有使用者資訊則直接登入
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }

                //6.無法從外部登入結果取得email
                ViewBag.ErrorTitle = $"Email claim not received from: {info.LoginProvider}";
                ViewBag.ErrorMessage = "請聯繫資訊人員";

                return View("Error");
            }
        }
    }
}