using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHLStatsModel;
using PredictStuff.Services;
using PredictStuff.ViewModels;
using PredictStuff.Models.VoteModel;

namespace PredictStuff.Controllers
{
    public class HomeController : Controller
    {
        NhlStatsService m_stats;

        public HomeController(NhlStatsService service)
        {
            m_stats = service;
        }

        public async Task<IActionResult> Index()
        {
            
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
