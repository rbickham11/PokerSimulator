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
        private int handsDealt;
        private List<int> dealtHands;
        private List<int> board;

        public Dealer()
        {
            outFile = new OutFile();
            deck = new Deck(outFile);
            dealtHands = new List<int>();
        }
       
        public void GetUserInput()
        {
            string inString;
            string[] cardStrings = new string[2];
            int card1, card2;
            int numHands;
            bool randomChange = false;
            handsDealt = 0;

            Console.WriteLine("Enter up to {0} specific hands to be dealt, separating the cards by a space (Ex. \"AS KD\"). Press Enter without typing anything when finished.\n", MAX_HANDS);

            deck.Shuffle();

            while (handsDealt <= MAX_HANDS)
            {
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
                    card1 = Deck.CardFromString(cardStrings[0]);
                    deck.ValidateCard(card1);
                    card2 = Deck.CardFromString(cardStrings[1]);
                    deck.ValidateCard(card2);

                    deck.DealSpecific(card1);
                    dealtHands.Add(card1);                  
                    deck.DealSpecific(card2);
                    dealtHands.Add(card2);

                    handsDealt++;
                    PrintPlayerHand(true, handsDealt, dealtHands.GetRange(dealtHands.Count - 2, 2));
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            if (dealtHands.Count / 2 < MAX_HANDS)
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
                        if (randomHands + handsDealt <= MAX_HANDS)
                        {
                            dealtHands.AddRange(deck.DealCards(randomHands * 2));
                            for (int j = handsDealt * 2; j < dealtHands.Count; j += 2)
                            {
                                PrintPlayerHand(true, j / 2 + 1, dealtHands.GetRange(j, 2));
                            }
                            handsDealt += randomHands;
                        }
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
                } while (inString != "Y" && inString != "N") ;
                if (inString == "Y")
                    randomChange = true;
            }
                        
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
            winnerChecker = new WinnerChecker(outFile, handsDealt);

            var stopWatch = new Stopwatch();
            stopWatch.Restart();
            List<int> specHands = new List<int>();
            int i, j;

            if (randomChange)
            {
                for (i = 0; i < (handsDealt - randomHands) * 2; i++)
                    specHands.Add(dealtHands[i]);
            }

            for (i = 0; i < numHands; i++)
            {
                outFile.AddLine("---------------------------------------------------------------");
                deck.CollectCards();
                deck.Shuffle();
                board = new List<int>();

                if (randomChange)
                {
                    dealtHands = new List<int>();
                    for (j = 0; j < specHands.Count; j++)
                    {
                        deck.DealSpecific(specHands[j]);
                        dealtHands.Add(specHands[j]);
                    }
                    dealtHands.AddRange(deck.DealCards(randomHands * 2));
                }
                else
                {
                    for(j = 0; j < dealtHands.Count; j++)
                    {
                        deck.DealSpecific(dealtHands[j]);
                    }
                }

                for (j = 0; j < dealtHands.Count; j += 2)
                {
                    PrintPlayerHand(false, j / 2 + 1, dealtHands.GetRange(j, 2));
                }
                DealFlop();
                DealCardToBoard();
                DealCardToBoard();
                
                outFile.AddLine();
                outFile.AppendLine("Board: ");
                foreach (int card in board)
                {
                    outFile.AppendLine(Deck.CardToString(card) + " ");
                }
                winnerChecker.FindWinner(dealtHands, board);
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
                outFile.AddLine(String.Format("Player {0}'s hand is: {1} {2}", playerNumber, Deck.CardToString(hand[0]), Deck.CardToString(hand[1])));
            }
        }
    }
}

