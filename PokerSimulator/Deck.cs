using System;
using System.Collections.Generic;

namespace PokerSimulator
{
    class Deck
    {
        const int DeckSize = 52;
        private readonly List<char> cardValues = new List<char>() { '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A' };
        private readonly List<char> suitValues = new List<char>() { 'D', 'H', 'C', 'S' };
        private readonly Random random = new Random();

        private List<int> deck;
        private OutFile outFile;

        public List<int> DealtHandList { get; private set; }
        public List<int> Board { get; private set; }
        public int HandsDealt { get; private set; }
        public bool GettingUserInput { get; set; }

        public Deck(OutFile file)
        {
            deck = new List<int>();
            outFile = file;
            GettingUserInput = true;

            DealtHandList = new List<int>();
            Board = new List<int>();
            for (int i = 0; i < DeckSize; i++)
                deck.Add(i);

            HandsDealt = 0;
        }

        public void Print()
        {
            foreach (int i in deck)
                Console.WriteLine(CardToString(i));
        }
        
        public void Shuffle()
        {
            int i, j, temp;
            for(i = deck.Count - 1; i > 0; i--)
            {
                j = random.Next(i + 1);
                temp = deck[i];
                deck[i] = deck[j];
                deck[j] = temp;
            }
        }

        public void CollectCards()
        {
            deck = new List<int>();
            Board = new List<int>();
            for (int i = 0; i < DeckSize; i++)
                deck.Add(i);

            HandsDealt = 0;
            DealtHandList = new List<int>();
        }

        public void DealRandom(int numHands)
        {
            int card1, card2;
            for (int i = 0; i < numHands; i++)
            {
                HandsDealt++;
                card1 = deck[0];
                deck.RemoveAt(0);
                DealtHandList.Add(card1);             
                card2 = deck[0];
                deck.RemoveAt(0);
                DealtHandList.Add(card2);

                if (GettingUserInput)
                    Console.WriteLine("Player {0}'s hand is: {1}{2}", HandsDealt, CardToString(card1), CardToString(card2));
                else
                    outFile.AddLine(String.Format("Player {0}'s hand is: {1}{2}", HandsDealt, CardToString(card1), CardToString(card2)));
            }
        }

        public void DealSpecific(int card1, int card2)
        {
            HandsDealt++;
            deck.Remove(card1);
            deck.Remove(card2);
            DealtHandList.Add(card1);
            DealtHandList.Add(card2);

            if (GettingUserInput)
                Console.WriteLine("Player {0}'s hand is: {1}{2}", HandsDealt, CardToString(card1), CardToString(card2));
            else
                outFile.AddLine(String.Format("Player {0}'s hand is: {1}{2}", HandsDealt, CardToString(card1), CardToString(card2)));
        }

        public void DealBoard()
        {
            int card;

            outFile.AppendLine("Board: ");
            for (int i = 0; i < 8; i++)
            {
                if (i == 0 || i == 4 || i == 6) //Burn Cards
                    deck.RemoveAt(0);
                else
                {
                    card = deck[0];
                    deck.RemoveAt(0);
                    Board.Add(card);
                    outFile.AppendLine(CardToString(card));
                }
            }
            outFile.AddLine();
        }

        public bool CardValid(string card)
        {
            bool cardValid = true;

            if (!cardValues.Contains(card[0]))
                cardValid = false;
            else if (!suitValues.Contains(card[1]))
                cardValid = false;

            if (!cardValid)
            {
                Console.WriteLine("{0} is invalid", card);
                return false;
            }

            return true;
        }

        public bool InDeck(int card)
        {
            if (!deck.Contains(card))
            {
                Console.Write(CardToString(card));
                Console.WriteLine("was already dealt");
                return false;
            }

            return true;
        }

        public int CardFromString(string card)
        {
            return 13 * suitValues.IndexOf(card[1]) + cardValues.IndexOf(card[0]);
        }

        public string CardToString(int card)
        {
            return String.Format("{0}{1} ", cardValues[card % 13], suitValues[card / 13]);
        }
    }
}
