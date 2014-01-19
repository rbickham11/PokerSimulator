using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PokerSimulator.Web.Models
{
    public class SimulationRequestModel
    {
        public IEnumerable<int> SetHands { get; set; }
        public int RandomHands { get; set; }
        public IEnumerable<int> SetBoard { get; set; }
        public int HandsToSimulate { get; set; }
        public bool RandomChange { get; set; }
    }
}