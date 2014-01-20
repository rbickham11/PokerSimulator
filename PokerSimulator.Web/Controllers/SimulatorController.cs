using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using PokerSimulator.Web.Models;
using PokerSimulator.Lib;

namespace PokerSimulator.Web.Controllers
{
    public class SimulatorController : Controller
    {
        //
        // GET: /Simulator/
        public ActionResult Index()
        {
            return View();
        }

        //POST: /Simulator/
        [HttpPost]
        public ActionResult Index(SimulationRequestModel request)
        {
            return View(request);
        }
	}
}