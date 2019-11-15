using CoreLearningByYoutube.Models;
using CoreLearningByYoutube.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoreLearningByYoutube.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
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
                    //產生user token
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //產生驗證連結
                    var confirmationLink = Url.Action("ConfirmEmail", "Account",
                        new { userId = user.Id, token = token }, Request.Scheme);
                    _logger.Log(LogLevel.Warning, confirmationLink);

                    //註冊新使用者後，假設目前已登入且擁有"Admin"角色權限，重新導向ListUsers
                    if (_signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListUsers", "Administration");
                    }
                    ////登入 需要先驗證後才能登入
                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    //return RedirectToAction("index", "home");
                    ViewBag.ErrorTitle = "註冊成功";
                    ViewBag.ErrorMessage = "登入前，請先點擊的我們發出的驗證連結";
                    return View("Error");
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
        /// 忘記密碼view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// 忘記密碼 action
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                //取得使用者
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && await _userManager.IsEmailConfirmedAsync(user))
                {
                    //產生忘記密碼token
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    //產生忘記密碼確認連結
                    var passwordResetLink = Url.Action("ResetPassword", "Account",
                        new { email = model.Email, token = token }, Request.Scheme);

                    _logger.Log(LogLevel.Warning, passwordResetLink);
                    return View("ForgotPasswordConfirmation");
                }
                return View("ForgotPasswordConfirmation");
            }
            return View(model);

        }

        /// <summary>
        /// 重設密碼 view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token,string email)
        {
            if(token == null || email == null)
            {
                ModelState.AddModelError("", "無效重設密碼token");
            }
            return View();
        }

        /// <summary>
        /// 重設密碼 action
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                //取得使用者
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        if(await _userManager.IsLockedOutAsync(user))
                        {
                            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                        }
                        return View("ResetPasswordConfirmation");
                    }
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View("ResetPasswordConfirmation");
                }
                return View("ForgotPasswordConfirmation");
            }
            return View(model);

        }

        /// <summary>
        /// 登入view
        /// </summary>
        /// <returns></returns>        
        [HttpGet]
        [AllowAnonymous]
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
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                //email是否已驗證
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && !user.EmailConfirmed
                                && (await _userManager.CheckPasswordAsync(user, model.Password)))
                {
                    ModelState.AddModelError(string.Empty, "Email尚未驗證");
                    return View(model);
                }
                //取得登入結果
                //第三個參數為是否使用cookie
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, true);

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

                if (result.IsLockedOut)
                {
                    return View("AccountLocked");
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
        [HttpPost]
        [AllowAnonymous]
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
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View("Login", loginViewModel);
            }

            //2.取得外部登入資訊
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, $"Error loading external login information.");
                return View("Login", loginViewModel);
            }

            //3.取得登入者google email
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            ApplicationUser user = null;

            //4.檢查email
            if (email != null)
            {
                //5.取得使用者資訊by email
                user = await _userManager.FindByEmailAsync(email);

                //6.使用者email是否已驗證
                if (user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "Email尚未驗證");
                    return View("Login", loginViewModel);
                }
            }

            //7.使用外部資訊登入網站
            //isPersisten:cookie
            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                                        info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            //8.檢查登入結果
            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                //9.檢查email
                if (email != null)
                {
                    //10.無法取得使用者資訊則新增
                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                        };

                        await _userManager.CreateAsync(user);
                        //產生user token
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        //產生驗證連結
                        var confirmationLink = Url.Action("ConfirmEmail", "Account",
                            new { userId = user.Id, token = token }, Request.Scheme);
                        _logger.Log(LogLevel.Warning, confirmationLink);

                        ViewBag.ErrorTitle = "註冊成功";
                        ViewBag.ErrorMessage = "登入前，請先點擊的我們發出的驗證連結";
                        return View("Error");
                    }

                    //11.新增使用者資訊，AspNetUsers,AspNetUserLogin資料表
                    await _userManager.AddLoginAsync(user, info);
                    //12.有使用者資訊則直接登入
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }

                //13.無法從外部登入結果取得email
                ViewBag.ErrorTitle = $"Email claim not received from: {info.LoginProvider}";
                ViewBag.ErrorMessage = "請聯繫資訊人員";

                return View("Error");
            }
        }

        /// <summary>
        /// 驗證連結action
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null && token == null)
            {
                return RedirectToAction("index", "home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"使用者id{userId}不存在";
                return View("Error");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return View();
            }
            ViewBag.ErrorMessage = "信箱驗證無效";
            return View("Error");
        }

        /// <summary>
        /// 修改密碼view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToAction("AddPassword");
            }
            return View();
        }

        /// <summary>
        /// 修改密碼submit
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if(user == null)
                {
                    return RedirectToAction("Login");
                }

                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View();
                }

                await _signInManager.RefreshSignInAsync(user);
                return View("ChangePasswordConfirmation");
            }

            return View(model);
        }

        /// <summary>
        /// 設定本地密碼view
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> AddPassword()
        {
            var user = await _userManager.GetUserAsync(User);
            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (hasPassword)
            {
                return RedirectToAction("ChangePassword");
            }
            return View();
        }

        /// <summary>
        /// 設定本地密碼submit
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddPassword(AddPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                var result = await _userManager.AddPasswordAsync(user, model.NewPassword);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View();
                }

                await _signInManager.RefreshSignInAsync(user);
                return View("AddPasswordConfirmation");
            }

            return View(model);
        }
    }
}