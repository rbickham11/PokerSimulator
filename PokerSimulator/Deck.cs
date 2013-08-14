using System;
using System.Collections.Generic;

namespace PokerSimulator
{
    class Deck
    {
        const int DeckSize = 52;
  
        public List<int> DealtHandList { get; private set; }
        public List<int> Board { get; private set; }
        public int HandsDealt { get; private set; }
        
        private List<int> deck;
        private readonly List<char> CardValues = new List<char>() { '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A' };
        private readonly List<char> SuitValues = new List<char>() { 'D', 'H', 'C', 'S' };
        private readonly Random random = new Random();

        public Deck()
        {
            deck = new List<int>();
            DealtHandList = new List<int>();
            Board = new List<int>();
            for (int i = 0; i < DeckSize; i++)
                deck.Add(i);

            HandsDealt = 0;
        }

        public void Print()
        {
            foreach (int i in deck)
                Console.WriteLine("{0}{1}", CardValues[i % 13], SuitValues[i / 13]);
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
            int card;
            for (int i = 0; i < numHands; i++)
            {
                HandsDealt++;
                Console.Write("Player {0}'s hand is: ", HandsDealt);
                card = deck[0];
                deck.RemoveAt(0);
                DealtHandList.Add(card);
                PrintCard(card);
                
                card = deck[0];
                deck.RemoveAt(0);
                DealtHandList.Add(card);
                PrintCard(card);
                Console.WriteLine();
            }
        }

        public void DealSpecific(int card1, int card2)
        {
            HandsDealt++;
            deck.Remove(card1);
            deck.Remove(card2);
            DealtHandList.Add(card1);
            DealtHandList.Add(card2);

            Console.Write("Player {0}'s hand is: ", HandsDealt);
            PrintCard(card1);
            PrintCard(card2);
            Console.WriteLine();
        }

        public void DealBoard()
        {
            int card;

            Console.Write("Board: ");
            for (int i = 0; i < 5; i++)
            {
                HandsDealt++;
                card = deck[0];
                deck.RemoveAt(0);
                Board.Add(card);
                PrintCard(card);
            }
            Console.WriteLine();
        }

        public void PrintCard(int i)
        {
            Console.Write("{0}{1} ", CardValues[i % 13], SuitValues[i / 13]);
        }

        public bool CardValid(string card)
        {
            bool cardValid = true;

            if (!CardValues.Contains(card[0]))
                cardValid = false;
            else if (!SuitValues.Contains(card[1]))
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
                PrintCard(card);
                Console.WriteLine("was already dealt");
                return false;
            }

            return true;
        }

        public int ConvertCardString(string card)
        {
            return 13 * SuitValues.IndexOf(card[1]) + CardValues.IndexOf(card[0]);
        }
    }
}
