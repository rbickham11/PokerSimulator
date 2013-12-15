using System;
using System.Collections.Generic;

namespace PokerSimulator
{
    class Deck
    {
        const int DeckSize = 52;
        private static readonly List<char> cardValues = new List<char>() { '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A' };
        private static readonly List<char> suitValues = new List<char>() { 'D', 'H', 'C', 'S' };
        
        private Random random;
        private List<int> deck;
        private OutFile outFile;
        private List<int> dealtCards;

        public Deck(OutFile file)
        {
            deck = new List<int>();
            random = new Random();
            outFile = file;

            dealtCards = new List<int>();
            for (int i = 0; i < DeckSize; i++)
                deck.Add(i);
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
            foreach(int card in dealtCards)
            {
                deck.Add(card);
            }
            dealtCards = new List<int>();
        }

        public int DealCard()
        {
            int card = deck[0];
            dealtCards.Add(card);
            deck.Remove(card);
            return card;
        }

        public List<int> DealCards(int numCards)
        {
            var cards = new List<int>();
           
            for(int i = 0; i < numCards; i++)
            {
                cards.Add(deck[0]);
                dealtCards.Add(deck[0]);
                deck.RemoveAt(0);
            }
            return cards; 
        }
        
        public void DealSpecific(int card)
        {
            if(card < 0 || card > 51)
            {
                throw new ArgumentOutOfRangeException("Invalid card. Card must be in range 0-51");
            }
            if(!deck.Contains(card))
            {
                throw new ArgumentException(String.Format("{0} was already dealt.", CardToString(card)));
            }
            deck.Remove(card);
            dealtCards.Add(card);
        }

        public static int CardFromString(string cardString)
        {
            if (!cardValues.Contains(cardString[0]) || !suitValues.Contains(cardString[1]))
            {
                throw new ArgumentException(String.Format("{0} is not a valid card.", cardString));
            }
            return 13 * suitValues.IndexOf(cardString[1]) + cardValues.IndexOf(cardString[0]);
        }

        public static string CardToString(int card)
        {
            return String.Format("{0}{1}", cardValues[card % 13], suitValues[card / 13]);
        }
    }
}
