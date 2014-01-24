using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerSimulator.Lib
{
    public class SimulatedHand
    {
        public List<int> Hands { get; set; }
        public List<int> Board { get; set; }
        public int WinningPlayer { get; set; }
        public string WinningRank { get; set; }
    }
}
