using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerSimulator
{
    class WinnerChecker
    {
        private readonly List<char> CardValues = new List<char>() { '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A' };
        private readonly List<string> Ranks = new List<string>() { "High Card", "Pair", "Two Pair", "Three of a Kind", "Straight", "Flush", "Full House", "Four of a Kind", "Straight Flush" };
        
        private List<int> hands;
        private List<int> board;     
        
        private List<int> thisHand;
        private BitArray dontCheck;

        public WinnerChecker()
        {
            thisHand = new List<int>();
            dontCheck = new BitArray(9);
        }

        public void GetWinner(List<int> inHands, List<int> inBoard)
        {
            hands = inHands;
            board = inBoard;

            int i, j;
            int thisRankCount= 0;

            eliminateHands();

            for (i = 0; i < 5; i++)
                thisHand.Add(board[i]);

            for (i = 8; thisRankCount == 0; i--)
            {
                if (!dontCheck[i])
                {
                    for (j = 0; i < hands.Count; j += 2)
                    {
                        thisHand.Insert(5, hands[i]);
                        thisHand.Insert(6, hands[i + 1]);

                    }
                }
            }
        }

        public void eliminateHands()
        {
            int i;
            bool draw = false;

            List<int> BoardValues = GetValueList(board);
            List<int> BoardSuits = GetSuitList(board);

            switch (BoardValues.Distinct().Count())
            {
                case 2: //Must be Four of a Kind
                    for (i = 0; i < dontCheck.Count && i != 7; i++)
                        dontCheck[i] = true;
                    return;
                case 3:
                case 4:
                    break;
                case 5: //Eliminate Full House and Four of a Kind
                    dontCheck[7] = true;
                    dontCheck[6] = true;
                    break;
                default:
                    Console.WriteLine("Error, Invalid Board");
                    break;
            }
            
            //Eliminate Flush Hands
            BoardSuits.Sort();
            for (i = 2; i < BoardSuits.Count; i++)
            {
                if (BoardSuits[i] == BoardSuits[i - 2])
                {
                    draw = true;
                    break;
                }
            }
            if (!draw)
            {
                dontCheck[5] = true;
                dontCheck[8] = true;
            }

            //Eliminate Straight Hands
            draw = false;
            BoardValues.Sort();
            for (i = 2; i < board.Count; i++)
            {
                if (BoardValues[i] == BoardValues[i - 2] + 2)
                {
                    draw = true;
                    break;
                }

                if (!draw)
                {
                    dontCheck[4] = true;
                    dontCheck[8] = true;
                }
            }

        }
        public int GetCardValue(int card)
        {
            return card % 13;
        }

        public int GetSuit(int card)
        {
            return card / 13;
        }

        public List<int> GetValueList(List<int> cards)
        {
            return (from c in cards select c % 13).ToList();
        }

        public List<int> GetSuitList(List<int> cards)
        {
            return (from c in cards select c / 13).ToList();
        }
    }
}
