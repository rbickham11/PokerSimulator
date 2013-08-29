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
            List<int> fiveCardHands;

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

            if (rankWinners.Count == 2)  //If there's only one hand with the winning rank
                winningPlayer = hands.IndexOf(rankWinners[0]) / 2 + 1;
            else
            {
                fiveCardHands = getFiveCardHands(rankWinners, winningRank);
                var possibleWinner = new BitArray(fiveCardHands.Count / 5, true);
                int possibleCount = possibleWinner.Count;
                for (i = 4; i >= 0; i--)
                {
                    if(possibleCount == 1)
                        break;
                    for (j = 0; j < fiveCardHands.Count; j += 5)
                    {
                        if (possibleWinner[j / 5])
                        {
                            for (k = 0; k < fiveCardHands.Count; k += 5)
                            {
                                if (k != j)
                                {
                                    if (possibleWinner[k / 5] && fiveCardHands[j + i] < fiveCardHands[k + i])
                                    {
                                        possibleWinner[j / 5] = false;
                                        possibleCount--;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                if (possibleCount > 1)
                    winningPlayer = 0;
                else
                {
                    for (i = 0; i < possibleWinner.Count; i++)
                        if (possibleWinner[i] == true)
                            break;
                    winningPlayer = hands.IndexOf(rankWinners[i * 2]) / 2 + 1;
                }
            }

            if (winningRank == 0 || winningRank == 2 || winningRank == 3 || winningRank == 7)
                a = string.Empty;
            outFile.AddLine();
            outFile.AddLine();
            if (winningPlayer == 0)
                outFile.AddLine("Chop");
            else
            {
                outFile.AddLine(String.Format("The winner is Player {0} with {1}{2}", winningPlayer, a, Ranks[winningRank]));
                winCounts[winningPlayer - 1]++;
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

        public List<int> getFiveCardHands(List<int> rankWinners, int rank)
        {
            int i, j, k;
            List<int> fiveCardHands = new List<int>();
            List<int> sevenCardHand;

            for (i = 0; i < rankWinners.Count; i += 2)
            {
                sevenCardHand = new List<int>(board);
                sevenCardHand.Add(rankWinners[i]);
                sevenCardHand.Add(rankWinners[i + 1]);
                if(rank != 5 && rank != 8)
                {
                    sevenCardHand = GetValueList(sevenCardHand);
                }
                sevenCardHand.Sort();

                switch (rank)
                {
                    case 0: //High Card
                        sevenCardHand.RemoveRange(0, 2);
                        fiveCardHands.AddRange(sevenCardHand);
                        break;
                    case 1: //Pair
                    case 2: //Two Pair
                        for (j = 0; j < sevenCardHand.Count - 1; j++)
                            if(sevenCardHand[j] == sevenCardHand[j + 1])
                                break;
                        int dupCard = sevenCardHand[j];
                        sevenCardHand.RemoveRange(j, 2);
                        sevenCardHand.Add(dupCard);
                        sevenCardHand.Add(dupCard);
                        if (rank == 2)
                        {
                            for (j = 0; j < sevenCardHand.Count - 3; j++)
                                if (sevenCardHand[j] == sevenCardHand[j + 1])
                                    break;
                            dupCard = sevenCardHand[j];
                            sevenCardHand.RemoveRange(j, 2);
                            sevenCardHand.Add(dupCard);
                            sevenCardHand.Add(dupCard);
                        }
                        sevenCardHand.RemoveRange(0, 2);
                        fiveCardHands.AddRange(sevenCardHand);
                        break;
                    case 3: //Three of a Kind
                        for (j = 0; j < sevenCardHand.Count - 2; j++)
                            if (sevenCardHand[j] == sevenCardHand[j + 2])
                                break;
                        dupCard = sevenCardHand[j];
                        sevenCardHand.RemoveRange(j, 3);
                        for (k = 0; k < 3; k++)
                            sevenCardHand.Add(dupCard);
                        sevenCardHand.RemoveRange(0, 2);
                        fiveCardHands.AddRange(sevenCardHand);
                        break;
                    case 4: //Straight
                        sevenCardHand = sevenCardHand.Distinct().ToList();
                        for (j = sevenCardHand.Count - 1; j >= 4; j--)
                        {
                            if (sevenCardHand[j] == sevenCardHand[j - 4] + 4)
                            {
                                for (k = j; k > j - 5; k--)
                                    fiveCardHands.Add(sevenCardHand[k]);
                                break;
                            }
                        }
                        break;
                    case 5: //Flush
                        for (j = sevenCardHand.Count - 1; j >= 4; j--)
                        {
                            if (GetSuit(sevenCardHand[j]) == GetSuit(sevenCardHand[j - 4]))
                            {
                                for (k = j; k > j - 5; k--)
                                    fiveCardHands.Add(GetCardValue(sevenCardHand[k]));
                                break;
                            }
                        }
                        break;
                    case 6: //Full House
                        for (j = 0; j < sevenCardHand.Count - 2; j++)
                            if (sevenCardHand[j] == sevenCardHand[j + 2])
                                break;
                        dupCard = sevenCardHand[j];
                        sevenCardHand.RemoveRange(j, 3);
                        for (k = 0; k < 3; k++)
                            sevenCardHand.Add(dupCard);
                        
                        for (j = 0; j < sevenCardHand.Count - 4; j++)
                            if (sevenCardHand[j] == sevenCardHand[j + 1])
                                break;
                        dupCard = sevenCardHand[j];
                        sevenCardHand.RemoveRange(j, 2);
                        sevenCardHand.Insert(0, dupCard);
                        sevenCardHand.Insert(0, dupCard);
                        
                        sevenCardHand.RemoveRange(0, 2);
                        fiveCardHands.AddRange(sevenCardHand);
                        break;
                    case 7: //Four of a Kind
                        for (j = 0; j < sevenCardHand.Count - 3; j++)
                            if (sevenCardHand[j] == sevenCardHand[j + 3])
                                break;
                        dupCard = sevenCardHand[j];
                        sevenCardHand.RemoveRange(j, 4);
                        for (k = 0; k < 4; k++)
                            sevenCardHand.Add(dupCard);
                        
                            sevenCardHand.RemoveRange(0, 2);
                        fiveCardHands.AddRange(sevenCardHand);
                        break;
                    case 8: //Straight Flush
                        for (j = sevenCardHand.Count - 1; j >= 4; j--)
                        {
                            if (GetSuit(sevenCardHand[j]) == GetSuit(sevenCardHand[j - 4]))
                            {
                                if(GetCardValue(sevenCardHand[j]) == GetCardValue(sevenCardHand[j - 4] + 4))
                                    for (k = j; k > j - 5; k--)
                                        fiveCardHands.Add(GetCardValue(sevenCardHand[k]));
                            }
                        }
                        break;
                    default:
                        Console.WriteLine("getFiveCardHands rank error");
                        break;
                }
            }
            return fiveCardHands;
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
