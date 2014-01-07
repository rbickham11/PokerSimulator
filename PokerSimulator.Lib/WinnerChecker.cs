using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PokerSimulator.Lib
{
    public class WinnerChecker
    {
        public static readonly List<string> Ranks = new List<string>() { "High Card", "Pair", "Two Pair", "Three of a Kind", "Straight", "Flush", "Full House", "Four of a Kind", "Straight Flush" };

        private List<int> board;

        public int WinningPlayer { get; private set; }
        public int WinningRank { get; private set; }

        public void FindWinner(List<int> hands, List<int> inBoard)
        {
            board = new List<int>(inBoard);
            List<int> thisHand = new List<int>() { -1, -1, -1, -1, -1, -1, -1 };

            int i, j, k;
            bool handFound = false;
            List<int> rankWinners = new List<int>();
            List<int> fiveCardHands;

            BitArray dontCheck = EliminateHands(); 

            //Assigning board values to first 5 positions of hand being checked
            for (i = 0; i < 5; i++)
            {
                thisHand[i] = board[i];
            }

            //For each rank, from high to low, until a matching hand is found.
            for (i = 8; handFound == false; i--)
            {
                if (!dontCheck[i])
                {
                    for (j = 0; j < hands.Count; j += 2)
                    {
                        thisHand[5] = hands[j];
                        thisHand[6] = hands[j + 1];

                        if (RankCheck(thisHand, i))
                        {
                            handFound = true;
                            rankWinners.Add(thisHand[5]);
                            rankWinners.Add(thisHand[6]);
                        }
                    }
                }
            }
            WinningRank = i + 1;

            if (rankWinners.Count == 2)  //If there's only one hand with the winning rank
            {
                WinningPlayer = hands.IndexOf(rankWinners[0]) / 2 + 1;
            }
            else
            {
                fiveCardHands = GetFiveCardHands(rankWinners, WinningRank);
                var possibleWinner = new BitArray(fiveCardHands.Count / 5, true);
                int possibleCount = possibleWinner.Count;

                //For each card position (highest ranking card to lowest)
                for (i = 4; i >= 0; i--)
                {
                    //If there's only one hand left (best hand found)
                    if (possibleCount == 1)
                    { 
                        break; 
                    }

                    //For each five card hand
                    for (j = 0; j < fiveCardHands.Count; j += 5)
                    {
                        //If the hand hasn't been eliminated 
                        if (possibleWinner[j / 5])
                        {
                            //For each other 5 card hand
                            for (k = 0; k < fiveCardHands.Count; k += 5)
                            {
                                //If the hand isn't the current one we are checking
                                if (k != j)
                                {
                                    //If the hand we're checking this one against is a possible winner and the current card in this hand
                                    //is less than the card in the same position in the hand we are checking this one against, eliminate this hand.
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

                //If there is more than one winner (More than one of the same 5 card hand)
                if (possibleCount > 1)
                {
                    WinningPlayer = 0;  //Chopped pot
                }
                else
                {
                    for (i = 0; i < possibleWinner.Count; i++)
                    {
                        if (possibleWinner[i] == true)
                        {
                            WinningPlayer = hands.IndexOf(rankWinners[i * 2]) / 2 + 1;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Eliminate impossible hands based on board.
        /// <returns>Returns a <c>BitArray</c> with ranks not to check for set to <c>true</c></returns>
        /// </summary>
        private BitArray EliminateHands()
        {
            int i;
            bool draw = false;

            List<int> BoardValues = GetValueList(board);
            List<int> BoardSuits = GetSuitList(board);
            BitArray dontCheck = new BitArray(Ranks.Count);
            switch (BoardValues.Distinct().Count())
            {
                case 2: //Must be Four of a Kind or Full House
                    for (i = 0; i < dontCheck.Count; i++)
                    {
                        if (i != 7 && i != 6)
                        {
                            dontCheck[i] = true;
                        }
                    }
                    return dontCheck;
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
            var DistinctBoardValues = BoardValues.Distinct().ToList();
            DistinctBoardValues.Sort();
            for (i = 2; i < DistinctBoardValues.Count; i++)
            {
                if (DistinctBoardValues[i] == DistinctBoardValues[i - 2] + 2)
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
            return dontCheck;
        }

        /// <summary>
        /// Determines whether a given seven card hand (board + hole cards) is of a certain rank.
        /// </summary>
        /// <param name="hand">The hand to check.</param>
        /// <param name="rank">The rank to check the hand against.</param>
        /// <returns></returns>
        private bool RankCheck(List<int> hand, int rank)
        {
            if(hand.Count != 7)
            {
                throw new ArgumentException("Hand to check must be of length 7.");
            }

            int i, j, k, temp;
            bool pairFound = false;

            List<int> handValues = GetValueList(hand);
            handValues.Sort();

            switch (rank)
            {
                case 8:  //Straight Flush
                    var tempHand = new List<int>(hand);
                    tempHand.Sort();
                    if (IsStraight(tempHand) && IsStraight(GetValueList(tempHand))) //This is a straight flush becuase "hand" values are still numbered 0-51
                    {                                                               //Second check eliminates overlap (Ex: Q-K-A-2-3)
                        return true;                                                
                    }
                    return false;
                case 7:  //Four of a Kind
                    for (i = 3; i < handValues.Count; i++)
                    {
                        if (handValues[i] == handValues[i - 3])
                        {
                            return true;
                        }
                    }
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
                                    {
                                        handValues.Add(temp);
                                    }
                                    handValues.Sort();
                                    return true;
                                }
                            }
                            for (k = 0; k < 3; k++)
                            {
                                handValues.Add(temp);
                            }
                            handValues.Sort();
                            return false;
                        }
                    }
                    return false;
                case 5:  //Flush
                    if (IsFlush(hand))
                    {
                        return true;
                    }
                    return false;
                case 4:  //Straight
                    if (IsStraight(handValues))
                    {
                        return true;
                    }
                    return false;
                case 3:  //Three of a Kind
                    for (i = 2; i < handValues.Count; i++)
                    {
                        if (handValues[i] == handValues[i - 2])
                        {
                            return true;
                        }
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
                                {
                                    return true;
                                }
                            }

                        }
                    }
                    return false;
                case 1:  //Pair
                    for (i = 1; i < handValues.Count; i++)
                    {
                        if (handValues[i] == handValues[i - 1])
                        {
                            return true;
                        }
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

        /// <summary>
        /// Gets the best five card hand from each seven card hand with the winning rank for final comparison.
        /// </summary>
        /// <param name="highestRankedHands">List of all hole cards that have the winning rank.</param>
        /// <param name="winningRank">The winning rank</param>
        /// <returns>A list of the five card hands, ordered from lowest ranking card to highest.</returns>
        private List<int> GetFiveCardHands(List<int> highestRankedHands, int winningRank)
        {
            int i, j, k, dupCard;
            List<int> fiveCardHands = new List<int>();
            List<int> sevenCardHand;

            for (i = 0; i < highestRankedHands.Count; i += 2)
            {
                sevenCardHand = new List<int>(board);
                sevenCardHand.Add(highestRankedHands[i]);
                sevenCardHand.Add(highestRankedHands[i + 1]);
                if(winningRank != 5 && winningRank != 8)
                {
                    sevenCardHand = GetValueList(sevenCardHand);
                }
                sevenCardHand.Sort();

                switch (winningRank)
                {
                    case 0: //High Card
                        sevenCardHand.RemoveRange(0, 2);
                        fiveCardHands.AddRange(sevenCardHand);
                        break;
                    case 1: //Pair
                    case 2: //Two Pair
                        for (j = sevenCardHand.Count - 2; j >= 0; j--)
                        {
                            if (sevenCardHand[j] == sevenCardHand[j + 1])
                            {
                                break;
                            }
                        }
                        dupCard = sevenCardHand[j];
                        sevenCardHand.RemoveRange(j, 2);
                        sevenCardHand.Add(dupCard);
                        sevenCardHand.Add(dupCard);
                        if (winningRank == 2)
                        {
                            for (j = sevenCardHand.Count - 4; j >= 0; j--)
                            {
                                if (sevenCardHand[j] == sevenCardHand[j + 1])
                                {
                                    break;
                                }
                            }
                            dupCard = sevenCardHand[j];
                            sevenCardHand.RemoveRange(j, 2);
                            sevenCardHand.Insert(3, dupCard);
                            sevenCardHand.Insert(3, dupCard);
                        }
                        sevenCardHand.RemoveRange(0, 2);
                        fiveCardHands.AddRange(sevenCardHand);
                        break;
                    case 3: //Three of a Kind
                        for (j = sevenCardHand.Count - 3; j >= 0; j--)
                        {
                            if (sevenCardHand[j] == sevenCardHand[j + 2])
                            {
                                break;
                            }
                        }
                        dupCard = sevenCardHand[j];
                        sevenCardHand.RemoveRange(j, 3);
                        for (k = 0; k < 3; k++)
                        {
                            sevenCardHand.Add(dupCard);
                        }
                        sevenCardHand.RemoveRange(0, 2);
                        fiveCardHands.AddRange(sevenCardHand);
                        break;
                    case 4: //Straight
                        sevenCardHand = sevenCardHand.Distinct().ToList();
                        if (sevenCardHand[sevenCardHand.Count - 1] == 12 && sevenCardHand[0] == 0 && sevenCardHand[1] == 1 && sevenCardHand[2] == 2 && sevenCardHand[3] == 3)  //A-2-3-4-5
                        {
                            for (k = 3; k >= 0; k--)
                            {
                                fiveCardHands.Add(k);
                            }
                            fiveCardHands.Add(12);
                            break;
                        }
                        for (j = sevenCardHand.Count - 1; j >= 4; j--)
                        {
                            if (sevenCardHand[j] == sevenCardHand[j - 4] + 4)
                            {
                                for (k = j; k > j - 5; k--)
                                {
                                    fiveCardHands.Add(sevenCardHand[k]);
                                }
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
                                {
                                    fiveCardHands.Add(GetCardValue(sevenCardHand[k]));
                                }
                                break;
                            }
                        }
                        break;
                    case 6: //Full House
                        dupCard = -1;
                        for (j = 0; j < sevenCardHand.Count - 2; j++)
                        {
                            if (sevenCardHand[j] == sevenCardHand[j + 2])
                            {
                                if (sevenCardHand[j] > dupCard) 
                                {
                                    dupCard = sevenCardHand[j];
                                }
                            }
                        }
                        sevenCardHand.RemoveRange(sevenCardHand.IndexOf(dupCard), 3);
                        for (k = 0; k < 3; k++)
                        {
                            sevenCardHand.Add(dupCard);
                        }

                        for (j = sevenCardHand.Count - 4; j >= 0; j--)
                        {
                            if (sevenCardHand[j] == sevenCardHand[j + 1])
                            {
                                break;
                            }
                        }
                        dupCard = sevenCardHand[j];
                        sevenCardHand.RemoveRange(j, 2);
                        sevenCardHand.Insert(2, dupCard);
                        sevenCardHand.Insert(2, dupCard);
                        
                        sevenCardHand.RemoveRange(0, 2);
                        fiveCardHands.AddRange(sevenCardHand);
                        break;
                    case 7: //Four of a Kind
                        for (j = 0; j < sevenCardHand.Count - 3; j++)
                        {
                            if (sevenCardHand[j] == sevenCardHand[j + 3])
                            {
                                break;
                            }
                        }
                        dupCard = sevenCardHand[j];
                        sevenCardHand.RemoveRange(j, 4);
                        for (k = 0; k < 4; k++)
                        {
                            sevenCardHand.Add(dupCard);
                        }
                        sevenCardHand.RemoveRange(0, 2);
                        fiveCardHands.AddRange(sevenCardHand);
                        break;
                    case 8: //Straight Flush
                        for (j = sevenCardHand.Count - 1; j >= 4; j--)
                        {
                            if (GetSuit(sevenCardHand[j]) == GetSuit(sevenCardHand[j - 4]))
                            {
                                if (GetCardValue(sevenCardHand[j]) == GetCardValue(sevenCardHand[j - 4]) + 4)
                                {
                                    for (k = j; k > j - 5; k--)
                                    {
                                        fiveCardHands.Add(GetCardValue(sevenCardHand[k]));
                                    }
                                    break;
                                }
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
        private bool IsStraight(List<int> hand)
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

        private bool IsFlush(List<int> hand)
        {
            hand = GetSuitList(hand);
            hand.Sort();

            for (int i = 4; i < hand.Count; i++)
                if (hand[i] == hand[i - 4])
                    return true;
            return false;
        }

        private int GetCardValue(int card)
        {
            return card % 13;
        }

        private int GetSuit(int card)
        {
            return card / 13;
        }

        private List<int> GetValueList(List<int> cards)
        {
            return (from c in cards select c % 13).ToList();
        }

        private List<int> GetSuitList(List<int> cards)
        {
            return (from c in cards select c / 13).ToList();
        }
    }
}
