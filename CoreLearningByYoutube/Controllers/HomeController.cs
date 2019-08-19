using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CoreLearningByYoutube.Models;
using CoreLearningByYoutube.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoreLearningByYoutube.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IHostingEnvironment _hostingEnviroment;
        private readonly ILogger _logger;

        public HomeController(IEmployeeRepository employeeRepository,
            IHostingEnvironment hostingEnviroment,
            ILogger<HomeController> logger)
        {
            _employeeRepository = employeeRepository;
            _hostingEnviroment = hostingEnviroment;
            _logger = logger;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            var model = _employeeRepository.GetAllEmployee();
            return View(model);
        }

        /// <summary>
        /// details view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>   
        [AllowAnonymous]
        public ViewResult Details(int? id)
        {
            //throw new Exception("有例外發生囉!!!");
            _logger.LogTrace("Trace log");
            _logger.LogDebug("Debug log");
            _logger.LogInformation("Information log");
            _logger.LogWarning("Warning log");
            _logger.LogError("Error log");
            _logger.LogCritical("Critical log");

            var employee = _employeeRepository.GetEmployee(id.Value);
            if(employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", id.Value);
            }

            var model = new HomeDetailsViewModel
            {
                Employee = employee,
                PageTitle = "Employee Details"
            };
            return View(model);
        }

        /// <summary>
        /// create view
        /// </summary>
        /// <returns></returns>
        [HttpGet]        
        public ViewResult Create()
        {
            return View();
        }

        /// <summary>
        /// edit view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ViewResult Edit(int id)
        {
            var employee = _employeeRepository.GetEmployee(id);
            EmployeeEditViewModel model = new EmployeeEditViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath
            };
            return View(model);
        }

        /// <summary>
        /// update save
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var employee = _employeeRepository.GetEmployee(model.Id);
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;
                if(model.Photo != null)
                {
                    if(model.ExistingPhotoPath != null)
                    {
                        var filePath = Path.Combine(_hostingEnviroment.WebRootPath, "images", model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }
                    employee.PhotoPath = ProcessUploadFile(model);
                }                
                
                _employeeRepository.Update(employee);
                return RedirectToAction("details", new { id = employee.Id });
            }

            return View();
        }

        /// <summary>
        /// create save
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create(EmployeeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = ProcessUploadFile(model);

                var employee = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoPath = uniqueFileName
                };
                _employeeRepository.Add(employee);
                return RedirectToAction("details", new { id = employee.Id });
            }

            return View();
        }
                
        /// <summary>
        /// 檔案上傳共用方法
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private string ProcessUploadFile(EmployeeCreateViewModel model)
        {
            string uniqueFileName = null;
            if (model.Photo != null)
            {
                string uploadsFolder = Path.Combine(_hostingEnviroment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}