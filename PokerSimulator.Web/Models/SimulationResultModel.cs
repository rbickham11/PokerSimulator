﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using PokerSimulator.Lib;

namespace PokerSimulator.Web.Models
{
    public class SimulationResultModel
    {
        public IEnumerable<int> SetHands { get; set; }
        public IEnumerable<int> PlayerWinCounts { get; set; }
        public IEnumerable<int> RankWinCounts { get; set; }
        public IEnumerable<SimulatedHand> SimulatedHands { get; set; }
    }
}