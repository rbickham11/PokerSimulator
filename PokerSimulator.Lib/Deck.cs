using System;
using System.Collections.Generic;

namespace PokerSimulator.Lib
{
    public class Deck
    {
        const int DeckSize = 52;
        private static readonly List<char> cardValues = new List<char>() { '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A' };
        private static readonly List<char> suitValues = new List<char>() { 'D', 'H', 'C', 'S' };
        
        private List<int> deck;
        private List<int> dealtCards;
        private Random random;

        public Deck()
        {
            random = new Random();
            deck = new List<int>();
            dealtCards = new List<int>();
            for (int i = 0; i < DeckSize; i++)
                deck.Add(i);
        }

        public void Print()
        {
            foreach (int card in deck)
            {
                Console.WriteLine(CardToString(card) + "(" + card + ")");
            }
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

        public void DealSpecific(List<int> cards)
        {
            try
            {
                foreach(int card in cards)
                {
                    ValidateCard(card);
                }
            }
            catch(Exception)
            {
                throw;
            }
            
            foreach (int card in cards)
            {
                deck.Remove(card);
                dealtCards.Add(card);
            }
        }

        public void DealSpecific(int card)
        {
            try
            {
                ValidateCard(card);
            }
            catch(Exception)
            {
                throw;
            }
            deck.Remove(card);
            dealtCards.Add(card);
        }

        public void ValidateCard(int card)
        {
            if (card < 0 || card > 51)
            {
                throw new ArgumentOutOfRangeException("Invalid card. Card must be in range 0-51");
            }
            if (!deck.Contains(card))
            {
                throw new ArgumentException(String.Format("{0} was already dealt.", CardToString(card)));
            }
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
