using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoreLearningByYoutube.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [Route("Error/{statusCode}")]
        public IActionResult ErrorHanding(int statusCode)
        {
            //取得status code
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            switch (statusCode)
            {
                case 404:
                    //呈現在view上
                    ViewBag.Message = "抱歉，你所要求的頁面不存在!";
                    ////原始request路徑
                    //ViewBag.Paht = statusCodeResult.OriginalPath;
                    ////原始request查詢字串
                    //ViewBag.QueryString = statusCodeResult.OriginalQueryString;

                    //log
                    _logger.LogError($"404 錯誤發生. 路徑為: {statusCodeResult.OriginalPath}" +
                        $" 並且查詢字串為{statusCodeResult.OriginalQueryString}");
                    break;
            }
            return View("NotFound");
        }

        [Route("Error")]
        [AllowAnonymous]
        public IActionResult Error()
        {
            //取得exception object
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            //呈現在view上
            //ViewBag.ExceptionPath = exceptionDetails.Path;
            //ViewBag.ExceptionMessage = exceptionDetails.Error.Message;
            //ViewBag.Stacktrace = exceptionDetails.Error.StackTrace;

            //記錄在log
            _logger.LogError($"The path {exceptionDetails.Path} threw an exception" + 
                $"{exceptionDetails.Error}");

            return View("Error");
        }
    }
}