using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace PokerSimulator
{
    class Dealer
    {
        const int MAX_HANDS = 10;
        private Deck deck;
        private WinnerChecker wc;
        private int randomHands;

        public Dealer()
        {
            deck = new Deck();
            wc = new WinnerChecker();
        }
       
        public void GetUserInput()
        {
            string inString;
            string[] cardStrings = new string[2];
            int card1, card2, numHands;
            bool randomChange;

            Console.WriteLine("Enter up to {0} specific hands to be dealt, separating the cards by a space (Ex. \"AS KD\"). Press Enter without typing anything when finished.\n", MAX_HANDS);

            deck.Shuffle();

            while (deck.HandsDealt <= MAX_HANDS)
            {
                Console.Write("Hand {0}: ", (deck.HandsDealt + 1));
                inString = Console.ReadLine();
                if (inString == string.Empty)
                    break;
                inString.Trim();
                if (inString.Length != 5)
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
                    card1 = deck.ConvertCardString(cardStrings[0]);
                    card2 = deck.ConvertCardString(cardStrings[1]);
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

            Console.Write("Do you want the same random hands dealt each time? (Choose no for new hands each time)");

            while (inString.Trim().ToUpper() != "Y" && inString.Trim().ToUpper() != "N")
            {
                Console.WriteLine();
                Console.Write("Y/N: ");
                inString = Console.ReadLine();
            }
            if (inString == "Y")
                randomChange = false;
            else
                randomChange = true;

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
            var stopWatch = new Stopwatch();
            stopWatch.Restart();
            List<int> specHands = new List<int>();
            int i;
            int players = deck.HandsDealt;

            if (randomChange == true)
            {
                for (i = 0; i < (deck.HandsDealt - randomHands) * 2; i++)
                    specHands.Add(deck.DealtHandList[i]);

                for (i = 0; i < numHands; i++)
                {
                    deck.CollectCards();
                    deck.Shuffle();
                    for (int j = 0; j < specHands.Count; j += 2)
                        deck.DealSpecific(specHands[j], specHands[j + 1]);
                    deck.DealRandom(randomHands);
                    deck.DealBoard();
                    wc.GetWinner(deck.DealtHandList, deck.Board);
                }
            }
            else
            {
                foreach (int k in deck.DealtHandList)
                    specHands.Add(k);

                for (i = 0; i < numHands; i++)
                {
                    deck.CollectCards();
                    deck.Shuffle();
                    for (int j = 0; j < specHands.Count; j += 2)
                        deck.DealSpecific(specHands[j], specHands[j + 1]);
                    deck.DealBoard();
                    wc.GetWinner(deck.DealtHandList, deck.Board);
                }
            }
            stopWatch.Stop();
            Console.WriteLine("({0}ms)", stopWatch.ElapsedMilliseconds);
        }
    }
}
