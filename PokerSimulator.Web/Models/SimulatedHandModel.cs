using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PokerSimulator.Web.Models
{
    public class SimulatedHandModel
    {
        public IEnumerable<int> Hands { get; set; }
        public IEnumerable<int> Board { get; set; }
        public int WinningPlayer { get; set; }
        public string WinningRank { get; set; }
    }
}