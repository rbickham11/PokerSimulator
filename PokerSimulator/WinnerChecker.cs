using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerSimulator
{
    class WinnerChecker
    {
        private readonly List<char> CardValues = new List<char>() { '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A' };
        public List<int> hands { get; set; }
        public List<int> board { get; set; }


        public void GetWinner(List<int> inHands, List<int> inBoard)
        {
            hands = inHands;
            board = inBoard;
        }
    }
}
