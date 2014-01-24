using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PokerSimulator.Web.Models
{
    public class SimulationResultModel
    {
        public IEnumerable<int> SetHands { get; set; }
        public IEnumerable<int> PlayerWinCounts { get; set; }
        public IEnumerable<int> RankWinCounts { get; set; }
        public IEnumerable<SimulatedHandModel> SimulatedHands { get; set; }
    }
}