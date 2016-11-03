using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PredictStuff.Services;
using NHLStatsModel;
using PredictStuff.Models.VoteModel;

namespace PredictStuff.Controllers
{
    public class VoteController : Controller
    {
        PollService m_service;
       

        public VoteController(PollService service)
        {
            m_service = service;
            
        }

        public async Task<IActionResult> Index()
        { 
            return View(await m_service.GetPolls());
        }    

        [HttpGet]        
        public async void Vote(int id, int vote)
        {
            List<Poll> polls = await m_service.GetPolls();
            polls[id].options[vote].count++;
            
        }
    }

    
}