using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace PokerSimulator
{
    class ConsoleMain
    {
        private Simulation simulation = new Simulation();

        static void Main(string[] args)
        {
            var simConsole = new ConsoleMain();
            Console.WriteLine("Poker Hand Simulator");
            simConsole.GetUserInput();
            Console.ReadKey();
        }
        
        public void GetUserInput()
        {
            string inString;
            string[] handString = new string[2];
            List<string> boardStrings;
            int numHands;
            bool randomChange = false;
            int handsDealt = 0;
            int randomHands = 0;
            List<int> cards = new List<int>();

            var stopWatch = new Stopwatch();

            Console.WriteLine("Enter up to {0} specific hands to be dealt, separating the cards by a space (Ex. \"AS KD\"). Press Enter without typing anything when finished.\n", Simulation.MAX_HANDS);

            while (handsDealt <= Simulation.MAX_HANDS)
            {
                cards.Clear();
                Console.Write("Hand {0}: ", (handsDealt + 1));
                inString = Console.ReadLine();
                if (inString == string.Empty)
                    break;
                if (inString.Trim().Length != 5 || !inString.Contains(" "))
                {
                    Console.WriteLine("Invalid hand");
                    continue;
                }
                handString = inString.Split(' ');

                if (handString[0] == handString[1])
                {
                    Console.WriteLine("Invalid Hand");
                    continue;
                }

                try
                {
                    cards.Add(Deck.CardFromString(handString[0]));
                    cards.Add(Deck.CardFromString(handString[1]));
                    simulation.AddHand(cards);
                    handsDealt++;
                    simulation.PrintPlayerHand(true, handsDealt, cards);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            if (handsDealt < Simulation.MAX_HANDS)
            {
                while (true)
                {
                    Console.Write("Additional random hands: ");
                    inString = Console.ReadLine();
                    if (int.TryParse(inString, out randomHands))
                    {
                        if (randomHands + handsDealt == 0)
                        {
                            Console.WriteLine("There must be at least one hand, try again.");
                            continue;
                        }
                        if (randomHands + handsDealt <= Simulation.MAX_HANDS)
                        {
                            simulation.AddRandomHands(randomHands);
                            for (int j = handsDealt * 2; j < simulation.DealtHands.Count; j += 2)
                            {
                                simulation.PrintPlayerHand(true, j / 2 + 1, simulation.DealtHands.GetRange(j, 2));
                            }
                            handsDealt += randomHands;
                        }
                        else
                        {
                            Console.WriteLine("Hand limit of {0} exceeded, try again.", Simulation.MAX_HANDS);
                            continue;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Please enter a valid number");
                        continue;
                    }
                    break;
                }
            }
            if (randomHands > 0)
            {
                Console.WriteLine("Do you want different random hands dealt each time?");
                Console.Write("(Choose no for the same {0} hands each time)", randomHands);

                do
                {
                    Console.WriteLine();
                    Console.Write("Y/N: ");
                    inString = Console.ReadLine().Trim().ToUpper();
                } while (inString != "Y" && inString != "N");
                if (inString == "Y")
                    randomChange = true;
            }

            while(true)
            {
                Console.Write("Enter up to 4 specific cards to be dealt to the board each hand: ");
                inString = Console.ReadLine();
                if(inString == string.Empty)
                {
                    break;
                }
                boardStrings = new List<string>(inString.Split(' '));
                if(boardStrings.Distinct().Count() != boardStrings.Count)
                {
                    Console.WriteLine("All cards must be distinct");
                    continue;
                }
                try
                {
                    cards.Clear();
                    foreach (string cardString in boardStrings)
                    {
                        cards.Add(Deck.CardFromString(cardString));
                    }
                    simulation.AddCardsToBoard(cards);
                    Console.Write("Set board cards: ");
                    foreach(int card in cards)
                    {
                        Console.Write(Deck.CardToString(card) + " ");
                    }
                    Console.WriteLine();
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            while (true)
            {
                Console.Write("Number of hands to simulate: ");
                inString = Console.ReadLine();
                if (int.TryParse(inString, out numHands))
                {
                    stopWatch.Restart();
                    simulation.Run(numHands, randomChange);
                    break;
                }
                else
                    Console.WriteLine("Please enter a valid number");
            }
            PrintResults();
            stopWatch.Stop();
            Console.WriteLine("({0}ms)", stopWatch.ElapsedMilliseconds);
            
            string filePath = @"SimulationResults.txt";
            simulation.PrintOutputToFile(filePath);
            Process.Start(filePath);
        }

        public void PrintResults()
        {
            string hand;
            for (int i = 1; i < simulation.PlayerWinCounts.Count; i++)
            {
                if (i <= simulation.DealtHands.Count / 2 - simulation.RandomHands)
                {
                    hand = string.Format("{0} {1}", Deck.CardToString(simulation.DealtHands[i * 2 - 2]), Deck.CardToString(simulation.DealtHands[i * 2 - 1]));
                }
                else
                {
                    hand = "random";
                }
                Console.WriteLine("Player {0} ({1}) wins: {2} ({3:P})", i, hand, simulation.PlayerWinCounts[i], (double)simulation.PlayerWinCounts[i] / simulation.NumHands);
            }
            Console.WriteLine("Chopped Pots: {0} ({1:P})", simulation.PlayerWinCounts[0], (double)simulation.PlayerWinCounts[0] / simulation.NumHands);
        }
    }
}
