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

        private OutFile outFile;
        private List<int> hands;
        private List<int> board;     
        private List<int> thisHand;
        private List<int> handValues;
        
        public List<int> winCounts { get; private set; }

        private BitArray dontCheck;

        public WinnerChecker(OutFile file, int handsDealt)
        {
            outFile = file;
            thisHand = new List<int>() { -1, -1, -1, -1, -1, -1, -1 };
            winCounts = new List<int>();
            for (int i = 0; i < handsDealt; i++)
                winCounts.Add(0);
        }

        public void FindWinner(List<int> inHands, List<int> inBoard)
        {
            hands = inHands;
            board = inBoard;

            int i, j, k;
            int winningPlayer = 0;
            bool handFound = false;
            string a = "a ";
            List<int> rankWinners = new List<int>();

            dontCheck = new BitArray(Ranks.Count);
            

            eliminateHands();

            for (i = 0; i < 5; i++)
                thisHand[i] = board[i];

            for (i = 8; handFound == false; i--)
            {
                if (!dontCheck[i])
                {
                    for (j = 0; j < hands.Count; j += 2)
                    {
                        thisHand[5] = hands[j];
                        thisHand[6] = hands[j + 1];

                        handValues = GetValueList(thisHand);
                        handValues.Sort();
                        if (rankCheck(i))
                        {
                            handFound = true;
                            rankWinners.Add(thisHand[5]);
                            rankWinners.Add(thisHand[6]);
                        }
                    }
                }
            }
            int winningRank = i + 1;
            if (winningRank == 0 || winningRank == 2 || winningRank == 3 || winningRank == 7)
                a = string.Empty;
            outFile.AddLine();
            outFile.AddLine(String.Format("The players who have the high ranked hand ({0}{1}) are: ", a, Ranks[winningRank])); 
            for(j = 0; j < rankWinners.Count; j += 2)
                outFile.AddLine(String.Format("Player {0}", hands.IndexOf(rankWinners[j]) / 2 + 1));
            //winCounts[winningPlayer - 1]++;
        }

        public void eliminateHands()
        {
            int i;
            bool draw = false;

            List<int> BoardValues = GetValueList(board);
            List<int> BoardSuits = GetSuitList(board);

            switch (BoardValues.Distinct().Count())
            {
                case 2: //Must be Four of a Kind or Full House
                    for (i = 0; i < dontCheck.Count; i++)
                    {
                        if (i != 7 && i != 6)
                            dontCheck[i] = true;
                    }
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
            }
            if (!draw)
            {
                dontCheck[4] = true;
                dontCheck[8] = true;
            }

        }

        public bool rankCheck(int rank)
        {
            int i, j, k, temp;
            List<int> tempHand;
            bool pairFound = false;
            switch (rank)
            {
                case 8:  //Straight Flush
                    tempHand = new List<int>(thisHand);
                    tempHand.Sort();
                    if (isStraight(tempHand) && isStraight(GetValueList(tempHand))) //This is a straight flush becuase thisHand values are still numbered 0-51
                        return true;                                                //Second check eliminates overlap (Ex: Q-K-A-2-3)
                    return false;
                case 7:  //Four of a Kind
                    for (i = 3; i < handValues.Count; i++)
                        if (handValues[i] == handValues[i - 3])
                            return true;
                    return false;
                case 6:  //Full House
                    for (i = 2; i < handValues.Count; i++)
                    {
                        if (handValues[i] == handValues[i - 2])
                        {
                            temp = handValues[i];
                            handValues.RemoveRange(i - 2, 3);
                            for (j = 1; j < handValues.Count; j++)
                            {
                                if (handValues[j] == handValues[j - 1])
                                {
                                    for (k = 0; k < 3; k++)
                                        handValues.Add(temp);
                                    handValues.Sort();
                                    return true;
                                }
                            }
                            for (k = 0; k < 3; k++)
                                handValues.Add(temp);
                            handValues.Sort();
                            return false;
                        }
                    }
                    return false;
                case 5:  //Flush
                    if (isFlush(thisHand))
                        return true;
                    return false;
                case 4:  //Straight
                    if (isStraight(handValues))
                        return true;
                    return false;
                case 3:  //Three of a Kind
                    for (i = 2; i < handValues.Count; i++)
                    {
                        if (handValues[i] == handValues[i - 2])
                            return true;
                    }
                    return false;
                case 2:  //Two Pair
                    for (i = 1; i < handValues.Count; i++)
                    {
                        if (!pairFound)
                        {
                            if (handValues[i] == handValues[i - 1])
                            {
                                pairFound = true;
                            }
                        }
                        else
                        {
                            for (j = i; j < handValues.Count; j++)
                            {
                                if (handValues[j] == handValues[j - 1])
                                    return true;
                            }

                        }
                    }
                    return false;
                case 1:  //Pair
                    for (i = 1; i < handValues.Count; i++)
                    {
                        if (handValues[i] == handValues[i - 1])
                            return true;
                    }
                    return false;
                case 0:  //High Card
                    return true;
                default:
                    Console.WriteLine("Invalid rank");
                    break;
            }
            return false;
        }
        public bool isStraight(List<int> hand)
        {
            var distinctHand = new List<int>(hand.Distinct());

            if (distinctHand[distinctHand.Count - 1] == 12 && distinctHand[0] == 0 && distinctHand[1] == 1 && distinctHand[2] == 2 && distinctHand[3] == 3) //Checking for A-2-3-4-5 straight
                return true;
            for (int i = 0; i < distinctHand.Count - 4; i++)
            {
                if (distinctHand[i] == distinctHand[i + 4] - 4)
                    return true;
            }
            return false;
        }

        public bool isFlush(List<int> hand)
        {
            hand = GetSuitList(hand);
            hand.Sort();

            for (int i = 4; i < hand.Count; i++)
                if (hand[i] == hand[i - 4])
                    return true;
            return false;
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
