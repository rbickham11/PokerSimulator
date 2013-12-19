using System;
using System.Collections.Generic;

namespace PokerSimulator
{
    class ConsoleMain
    {
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
            string[] cardStrings = new string[2];
            int numHands;
            bool randomChange = false;
            int handsDealt = 0;
            int randomHands = 0;
            List<int> inHand = new List<int>();

            var simulation = new Simulation();

            Console.WriteLine("Enter up to {0} specific hands to be dealt, separating the cards by a space (Ex. \"AS KD\"). Press Enter without typing anything when finished.\n", Simulation.MAX_HANDS);

            while (handsDealt <= Simulation.MAX_HANDS)
            {
                inHand.Clear();
                Console.Write("Hand {0}: ", (handsDealt + 1));
                inString = Console.ReadLine();
                if (inString == string.Empty)
                    break;
                if (inString.Trim().Length != 5 || !inString.Contains(" "))
                {
                    Console.WriteLine("Invalid hand");
                    continue;
                }
                cardStrings = inString.Split(' ');

                if (cardStrings[0] == cardStrings[1])
                {
                    Console.WriteLine("Invalid Hand");
                    continue;
                }

                try
                {
                    inHand.Add(Deck.CardFromString(cardStrings[0]));
                    inHand.Add(Deck.CardFromString(cardStrings[1]));
                    simulation.AddHand(inHand);
                    handsDealt++;
                    simulation.PrintPlayerHand(true, handsDealt, inHand);
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

            while (true)
            {
                Console.Write("Number of hands to simulate: ");
                inString = Console.ReadLine();
                if (int.TryParse(inString, out numHands))
                {
                    simulation.Run(numHands, randomChange);
                    break;
                }
                else
                    Console.WriteLine("Please enter a valid number");
            }
        }
    }
}
