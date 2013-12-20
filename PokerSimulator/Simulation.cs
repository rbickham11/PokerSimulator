using System;
using System.Collections.Generic;

namespace PokerSimulator
{
    class Simulation
    {
        public const int MAX_HANDS = 10;
        private Deck deck;
        private WinnerChecker winnerChecker;
        private SimulationOutput output;
        private int randomHands;
        private int numHands;
        
        public List<int> DealtHands { get; private set; }
        private List<int> board;

        public Simulation()
        {
            output = new SimulationOutput();
            deck = new Deck();
            deck.Shuffle();
            DealtHands = new List<int>();
            randomHands = 0;
        }

        public void AddHand(List<int> hand)
        {
            try
            {
                deck.ValidateCard(hand[0]);
                deck.ValidateCard(hand[1]);
                deck.DealSpecific(hand);
                DealtHands.AddRange(hand);
            }
            catch(Exception)
            {
                throw;
            }

        }

        public void AddRandomHands(int numHands)
        {
            DealtHands.AddRange(deck.DealCards(numHands * 2));
            randomHands += numHands;
        }

        public void Run(int hands, bool randomChange)
        {
            numHands = hands;
            winnerChecker = new WinnerChecker(output, DealtHands.Count / 2);
            if(!randomChange)
            {
                randomHands = 0;
            }

            List<int> specHands = new List<int>();
            int i, j;

            if (randomChange)
            {
                for (i = 0; i < (DealtHands.Count / 2 - randomHands) * 2; i++)
                    specHands.Add(DealtHands[i]);
            }

            output.AddLine("---------------------------------------------------------------");
            output.AddLine("Simulated Hands:");
            for (i = 0; i < numHands; i++)
            {
                output.AddLine("---------------------------------------------------------------");
                output.AddLine(string.Format("{0}.", i + 1));
                deck.CollectCards();
                deck.Shuffle();
                board = new List<int>();

                if (randomChange)
                {
                    DealtHands = new List<int>();
                    deck.DealSpecific(specHands);
                    DealtHands.AddRange(specHands);
                    DealtHands.AddRange(deck.DealCards(randomHands * 2));
                }
                else
                {
                    deck.DealSpecific(DealtHands);
                }

                for (j = 0; j < DealtHands.Count; j += 2)
                {
                    PrintPlayerHand(false, j / 2 + 1, DealtHands.GetRange(j, 2));
                }
                DealFlop();
                DealCardToBoard();
                DealCardToBoard();
                
                output.AddLine();
                output.AppendLine("Board: ");
                foreach (int card in board)
                {
                    output.AppendLine(Deck.CardToString(card) + " ");
                }
                winnerChecker.FindWinner(DealtHands, board);
            }
            PrintResults(false);
        }

        public void DealFlop()
        {
            deck.DealCard(); //Burn
            board.AddRange(deck.DealCards(3));
        }

        public void DealCardToBoard()
        {
            deck.DealCard(); //Burn
            board.Add(deck.DealCard());
        }

        public void PrintPlayerHand(bool console, int playerNumber, List<int> hand)
        {
            if (console)
            {
                Console.WriteLine("Player {0}'s hand is: {1} {2}", playerNumber, Deck.CardToString(hand[0]), Deck.CardToString(hand[1]));
            }
            else
            {
                output.AddLine(String.Format("Player {0}'s hand is: {1} {2}", playerNumber, Deck.CardToString(hand[0]), Deck.CardToString(hand[1])));
            }
        }

        public void PrintResults(bool console)
        {
            string hand;

            if (console)
            {
                for (int i = 1; i < winnerChecker.WinCounts.Count; i++)
                {
                    if(i <= DealtHands.Count / 2 - randomHands)
                    {
                        hand = string.Format("{0} {1}", Deck.CardToString(DealtHands[i * 2 - 2]), Deck.CardToString(DealtHands[i * 2 - 1]));
                    }
                    else
                    {
                        hand = "random";
                    }
                    Console.WriteLine("Player {0} ({1}) wins: {2} ({3:P})", i, hand, winnerChecker.WinCounts[i], (double)winnerChecker.WinCounts[i] / numHands);
                }
                Console.WriteLine("Chopped Pots: {0} ({1:P})", winnerChecker.WinCounts[0], (double)winnerChecker.WinCounts[0] / numHands);
            }
            else
            {
                output.AddTopLine();
                output.AddTopLine(String.Format("Chopped Pots: {0} ({1:P})", winnerChecker.WinCounts[0], (double)winnerChecker.WinCounts[0] / numHands));
                for (int i = winnerChecker.WinCounts.Count - 1; i > 0; i--)
                {
                    if (i <= DealtHands.Count / 2 - randomHands)
                    {
                        hand = string.Format("{0} {1}", Deck.CardToString(DealtHands[i * 2 - 2]), Deck.CardToString(DealtHands[i * 2 - 1]));
                    }
                    else
                    {
                        hand = "random";
                    }
                    output.AddTopLine(String.Format("Player {0} ({1}) wins: {2} ({3:P})", i, hand, winnerChecker.WinCounts[i], (double)winnerChecker.WinCounts[i] / numHands));
                }
                output.AddTopLine("---------------------------------------------------------------");
                output.AddTopLine("Simulation Results:");
                output.AddTopLine("---------------------------------------------------------------");
            }
        }

        public void PrintOutputToFile(string filePath)
        {
            output.WriteLinesToFile(filePath);
        }
    }
}

