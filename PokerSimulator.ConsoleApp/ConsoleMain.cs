using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

using PokerSimulator.Lib;

namespace PokerSimulator.ConsoleApp
{
    class ConsoleMain
    {
        private Simulation simulation;
        private SimulationOutput simOutput;

        static void Main(string[] args)
        {
            var simConsole = new ConsoleMain();
            Console.WriteLine("Poker Hand Simulator");
            simConsole.GetUserInput();
        }

        public void GetUserInput()
        {
            string inString;
            int randomHands;
            var cards = new List<int>();

            while (true)
            {
                simulation = new Simulation();
                simOutput = new SimulationOutput();
                randomHands = 0;

                Console.WriteLine("Enter up to {0} specific hands to be dealt, separating the cards by a space (Ex. \"AS KD\"). Press Enter without typing anything when finished.\n", Simulation.MAX_HANDS);

                while (simulation.DealtHands.Count / 2 <= Simulation.MAX_HANDS)
                {
                    cards.Clear();
                    Console.Write("Hand {0}: ", (simulation.DealtHands.Count / 2 + 1));
                    inString = Console.ReadLine();
                    if (inString == string.Empty)
                        break;
                    if (inString.Trim().Length != 5 || !inString.Contains(" "))
                    {
                        Console.WriteLine("Invalid hand");
                        continue;
                    }
                    string[] handString = inString.Split(' ');

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
                        PrintHand(simulation.DealtHands.Count / 2, simulation.DealtHands.GetRange(simulation.DealtHands.Count - 2, 2));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                int handsDealt = simulation.DealtHands.Count / 2;
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
                                    PrintHand(j / 2 + 1, simulation.DealtHands.GetRange(j, 2));
                                }
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
                
                bool randomChange = false;
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

                while (true)
                {
                    Console.Write("Enter up to 4 specific cards to be dealt to the board each hand: ");
                    inString = Console.ReadLine();
                    if (inString == string.Empty)
                    {
                        break;
                    }
                    var boardStrings = new List<string>(inString.Split(' '));
                    if (boardStrings.Distinct().Count() != boardStrings.Count)
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
                        foreach (int card in cards)
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

                var stopWatch = new Stopwatch();
                while (true)
                {
                    int numHands;
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

                simOutput.PrintResults(simulation);
                simOutput.PrintHands(simulation.SimulatedHands);

                stopWatch.Stop();
                Console.WriteLine("({0}ms)", stopWatch.ElapsedMilliseconds);

                string filePath = @"SimulationResults.txt";
                simOutput.WriteLinesToFile(filePath);
                Process.Start(filePath);
                
                do
                {
                    Console.Write("Would you like to run another simulation? (Y/N): ");
                    inString = Console.ReadLine().Trim().ToUpper();
                } while (inString != "Y" && inString != "N");
                if (inString == "N")
                {
                    break;
                }
                
                Console.WriteLine("");
            }
        }

        public void PrintHand(int player, List<int> hand)
        {
            Console.WriteLine("Player {0}'s hand is: {1} {2}", player, Deck.CardToString(hand[0]), Deck.CardToString(hand[1]));
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
