using System.Collections.Generic;
using System.Web.Mvc;

using PokerSimulator.Web.Models;
using PokerSimulator.Lib;

namespace PokerSimulator.Web.Controllers
{
    public class SimulatorController : Controller
    {
        //
        // GET: /Simulator/Holdem
        public ActionResult Holdem()
        {
            return View();
        }

        //POST: /Simulator/HoldemResult
        [HttpPost]
        public ActionResult HoldemResult(SimulationRequestModel request)
        {
            var simulation = new Simulation();
            var rawOutput = new SimulationOutput();

            var hands = new List<int>(request.SetHands);
            for (int i = 0; i < hands.Count; i += 2 )
            {
                simulation.AddHand(hands.GetRange(i, 2));
            }
            simulation.AddRandomHands(request.RandomHands);
            if (request.SetBoard != null)
            {
                simulation.AddCardsToBoard(new List<int>(request.SetBoard));
            }
            simulation.Run(request.HandsToSimulate, request.RandomChange);
            
            rawOutput.PrintResults(simulation);
            rawOutput.PrintHands(simulation.SimulatedHands);
            var response = new SimulationResultModel
            {
                SetHands = request.SetHands,
                PlayerWinCounts = simulation.PlayerWinCounts,
                RankWinCounts = simulation.RankWinCounts,
                SimulatedHands = (IEnumerable<SimulatedHand>)simulation.SimulatedHands
            };
            //rawOutput.WriteLinesToFile(Server.MapPath("~") + @"/SimulationResults.txt");
            //return new FilePathResult("~/SimulationResults.txt", "text/plain");
            return View(response);
        }
	}
}