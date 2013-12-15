using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PokerSimulator
{
    class Dealer
    {
        const int MAX_HANDS = 10;
        private Deck deck;
        private WinnerChecker winnerChecker;
        private int randomHands;
        private OutFile outFile;

        public Dealer()
        {
            outFile = new OutFile();
            deck = new Deck(outFile);
        }
       
        public void GetUserInput()
        {
            string inString;
            string[] cardStrings = new string[2];
            int card1, card2, numHands;
            bool randomChange = false;

            Console.WriteLine("Enter up to {0} specific hands to be dealt, separating the cards by a space (Ex. \"AS KD\"). Press Enter without typing anything when finished.\n", MAX_HANDS);

            deck.Shuffle();

            while (deck.HandsDealt <= MAX_HANDS)
            {
                Console.Write("Hand {0}: ", (deck.HandsDealt + 1));
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
                if (deck.CardValid(cardStrings[0]) && deck.CardValid(cardStrings[1]))
                {
                    card1 = Deck.CardFromString(cardStrings[0]);
                    card2 = Deck.CardFromString(cardStrings[1]);
                    if (deck.InDeck(card1) && deck.InDeck(card2))
                    {
                        deck.DealSpecific(card1, card2);
                    }
                }
            }

            while (true)
            {
                Console.WriteLine("Additional random hands: ");
                inString = Console.ReadLine();
                if (int.TryParse(inString, out randomHands))
                {
                    if (randomHands + deck.HandsDealt == 0)
                    {
                        Console.WriteLine("There must be at least one hand, try again.");
                        continue;
                    }
                    if (randomHands + deck.HandsDealt <= MAX_HANDS)
                        deck.DealRandom(randomHands);
                    else
                    {
                        Console.WriteLine("Hand limit of {0} exceeded, try again.", MAX_HANDS);
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

            if (randomHands > 0)
            {
                Console.WriteLine("Do you want different random hands dealt each time?");
                Console.Write("(Choose no for the same {0} hands each time)", randomHands);

                while (inString != "Y" && inString != "N")
                {
                    Console.WriteLine();
                    Console.Write("Y/N: ");
                    inString = Console.ReadLine().Trim().ToUpper();
                }
                if (inString == "Y")
                    randomChange = true;
            }
            
            deck.GettingUserInput = false;
            
            while (true)
            {
                Console.Write("Number of hands to simulate: ");
                inString = Console.ReadLine();
                if (int.TryParse(inString, out numHands))
                {
                    RunHands(numHands, randomChange);
                    break;
                }
                else
                    Console.WriteLine("Please enter a valid number");
            }
        }

        public void RunHands(int numHands, bool randomChange)
        {
            winnerChecker = new WinnerChecker(outFile, deck.HandsDealt);

            var stopWatch = new Stopwatch();
            stopWatch.Restart();
            List<int> specHands = new List<int>();
            int i;

            if (randomChange)
            {
                for (i = 0; i < (deck.HandsDealt - randomHands) * 2; i++)
                    specHands.Add(deck.DealtHandList[i]);

                for (i = 0; i < numHands; i++)
                {
                    outFile.AddLine("---------------------------------------------------------------");
                    deck.CollectCards();
                    deck.Shuffle();
                    for (int j = 0; j < specHands.Count; j += 2)
                        deck.DealSpecific(specHands[j], specHands[j + 1]);
                    deck.DealRandom(randomHands);
                    deck.DealBoard();
                    winnerChecker.FindWinner(deck.DealtHandList, deck.Board);
                }
            }
            else
            {
                foreach (int k in deck.DealtHandList)
                    specHands.Add(k);

                for (i = 0; i < numHands; i++)
                {
                    outFile.AddLine("---------------------------------------------------------------");
                    deck.CollectCards();
                    deck.Shuffle();
                    for (int j = 0; j < specHands.Count; j += 2)
                        deck.DealSpecific(specHands[j], specHands[j + 1]);
                    deck.DealBoard();
                    winnerChecker.FindWinner(deck.DealtHandList, deck.Board);
                }
            }

            for (i = 1; i < winnerChecker.winCounts.Count; i++)
                Console.WriteLine("Player {0} wins: {1} ({2:P})", i, winnerChecker.winCounts[i], (double)winnerChecker.winCounts[i] / numHands);
            Console.WriteLine("Chopped Pots: {0} ({1:P})", winnerChecker.winCounts[0], (double)winnerChecker.winCounts[0] / numHands);
            
            outFile.AddTopLine(String.Format("Chopped Pots: {0} ({1:P})", winnerChecker.winCounts[0], (double)winnerChecker.winCounts[0] / numHands));
            for (i = winnerChecker.winCounts.Count - 1; i > 0; i--)
                outFile.AddTopLine(String.Format("Player {0} wins: {1} ({2:P})", i, winnerChecker.winCounts[i], (double) winnerChecker.winCounts[i] / numHands));

            string filePath = @"SimulationResults.txt";
            outFile.WriteLinesToFile(filePath);
            stopWatch.Stop();
            Console.WriteLine("({0}ms)", stopWatch.ElapsedMilliseconds);
            Process.Start(filePath);
        }
    }
}
