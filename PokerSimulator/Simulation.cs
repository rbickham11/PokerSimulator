using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PokerSimulator
{
    class Simulation
    {
        public const int MAX_HANDS = 10;
        private Deck deck;
        private WinnerChecker winnerChecker;
        private SimulationOutput output;
        private int randomHands;
        
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

        public void Run(int numHands, bool randomChange)
        {
            winnerChecker = new WinnerChecker(output, DealtHands.Count / 2);

            var stopWatch = new Stopwatch();
            stopWatch.Restart();
            List<int> specHands = new List<int>();
            int i, j;

            if (randomChange)
            {
                for (i = 0; i < (DealtHands.Count / 2 - randomHands) * 2; i++)
                    specHands.Add(DealtHands[i]);
            }

            for (i = 0; i < numHands; i++)
            {
                output.AddLine("---------------------------------------------------------------");
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

            for (i = 1; i < winnerChecker.winCounts.Count; i++)
                Console.WriteLine("Player {0} wins: {1} ({2:P})", i, winnerChecker.winCounts[i], (double)winnerChecker.winCounts[i] / numHands);
            Console.WriteLine("Chopped Pots: {0} ({1:P})", winnerChecker.winCounts[0], (double)winnerChecker.winCounts[0] / numHands);
            
            output.AddTopLine(String.Format("Chopped Pots: {0} ({1:P})", winnerChecker.winCounts[0], (double)winnerChecker.winCounts[0] / numHands));
            for (i = winnerChecker.winCounts.Count - 1; i > 0; i--)
                output.AddTopLine(String.Format("Player {0} wins: {1} ({2:P})", i, winnerChecker.winCounts[i], (double) winnerChecker.winCounts[i] / numHands));

            string filePath = @"SimulationResults.txt";
            output.WriteLinesToFile(filePath);
            stopWatch.Stop();
            Console.WriteLine("({0}ms)", stopWatch.ElapsedMilliseconds);
            Process.Start(filePath);
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
    }
}

