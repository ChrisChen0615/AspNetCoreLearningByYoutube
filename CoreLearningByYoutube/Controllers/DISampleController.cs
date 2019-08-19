using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLearningByYoutube.Models.DISample;
using Microsoft.AspNetCore.Mvc;

namespace CoreLearningByYoutube.Controllers
{
    /// <summary>
    /// Singleton,Scope,Transient差別
    /// </summary>
    public class DISampleController : Controller
    {
        public readonly ISampleTransient _sampleTransient;
        public readonly ISampleScoped _sampleScoped;
        public readonly ISampleSingleton _sampleSingleton;

        public DISampleController(ISampleTransient sampleTransient, ISampleScoped sampleScoped, ISampleSingleton sampleSingleton)
        {
            _sampleTransient = sampleTransient;
            _sampleScoped = sampleScoped;
            _sampleSingleton = sampleSingleton;
        }

        public IActionResult Index()
        {
            var message = $"<tr><td>Transient</td><td>{_sampleTransient.Id}</td></tr>"
                        + $"<tr><td>Scoped</td><td>{_sampleScoped.Id}</td></tr>"
                        + $"<tr><td>Singleton</td><td>{_sampleSingleton.Id}</td></tr>";
            return View(model: message);
        }
    }
}