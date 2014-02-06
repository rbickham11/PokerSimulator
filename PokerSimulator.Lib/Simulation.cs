using System;
using System.Collections.Generic;

namespace PokerSimulator.Lib
{
    public class Simulation
    {
        public const int MAX_HANDS = 10;

        private Deck deck = new Deck();
        private WinnerChecker winnerChecker;
        private List<int> board;

        public int RandomHands { get; private set; }
        public int NumHands { get; private set; }
        public List<int> SetBoard { get; private set; }
        public List<int> DealtHands { get; private set; }
        public List<int> PlayerWinCounts { get; private set; }
        public List<int> RankWinCounts { get; private set; }
        public List<SimulatedHand> SimulatedHands { get; private set; }

        public Simulation()
        {
            SetBoard = new List<int>();
            DealtHands = new List<int>();
            PlayerWinCounts = new List<int>();
            RankWinCounts = new List<int>();
            SimulatedHands = new List<SimulatedHand>();
            RandomHands = 0;
            for(int i = 0; i < WinnerChecker.Ranks.Count; i++)
            {
                RankWinCounts.Add(0);
            }
            deck.Shuffle();
        }

        public void AddHand(List<int> hand)
        {
            try
            {
                deck.ValidateCard(hand[0]);
                deck.ValidateCard(hand[1]);
                deck.DealSpecific(hand);
                DealtHands.AddRange(hand);
            }
            catch(Exception)
            {
                throw;
            }

        }

        public void AddRandomHands(int numHands)
        {
            DealtHands.AddRange(deck.DealCards(numHands * 2));
            RandomHands += numHands;
        }

        public void AddCardsToBoard(List<int> cards)
        {
            try
            {
                foreach(int card in cards)
                {
                    deck.ValidateCard(card);
                }
                SetBoard.AddRange(cards);
            }
            catch(Exception)
            {
                throw;
            }
        }
        public void Run(int hands, bool randomChange)
        {
            NumHands = hands;
            winnerChecker = new WinnerChecker();
            if(!randomChange)
            {
                RandomHands = 0;
            }

            List<int> specHands = new List<int>();

            int i;

            if (randomChange)
            {
                for (i = 0; i < (DealtHands.Count / 2 - RandomHands) * 2; i++)
                    specHands.Add(DealtHands[i]);
            }
            
            for (i = 0; i < DealtHands.Count / 2 + 1; i++ ) //For all players and at index 0 for chops
            {
                PlayerWinCounts.Add(0);
            }
            
            for (i = 0; i < NumHands; i++)
            {
                deck.CollectCards();
                deck.Shuffle();
                board = new List<int>();
                deck.DealSpecific(SetBoard);  //Remove set board cards from deck to avoid dealing them in random hands.
                if (randomChange)
                {
                    DealtHands = new List<int>();
                    deck.DealSpecific(specHands);
                    DealtHands.AddRange(specHands);
                    DealtHands.AddRange(deck.DealCards(RandomHands * 2));
                }
                else
                {
                    deck.DealSpecific(DealtHands);
                }

                if (SetBoard.Count != 0)
                {
                    DealFlop(SetBoard);
                    if (SetBoard.Count == 4)
                    {
                        DealCardToBoard(SetBoard[3]);
                    }
                    else
                    {
                        DealCardToBoard();
                    }
                }
                else
                {
                    DealFlop();
                    DealCardToBoard();
                }
                DealCardToBoard();  //River
                
                winnerChecker.FindWinner(DealtHands, board);
                
                SimulatedHands.Add(new SimulatedHand
                {
                    Hands = DealtHands,
                    Board = board,
                    WinningPlayer = winnerChecker.WinningPlayer,
                    WinningRank = WinnerChecker.Ranks[winnerChecker.WinningRank]
                });
                
                PlayerWinCounts[winnerChecker.WinningPlayer]++;
                RankWinCounts[winnerChecker.WinningRank]++;
            }
        }

        public void DealFlop()
        {
            deck.DealCard(); //Burn
            board.AddRange(deck.DealCards(3));
        }

        public void DealFlop(List<int> cards)
        {
            deck.DealCard(); //Burn
            for (int i = 0; i < 3; i++)  
            {
                if(cards.Count > i)
                    board.Add(cards[i]);   //Specific cards were already removed from deck to avoid dealing in random hands.
            }
            while(board.Count < 3)
            {
                DealCardToBoard();
            }
        }

        public void DealCardToBoard()
        {
            deck.DealCard(); //Burn
            board.Add(deck.DealCard());
        }

        public void DealCardToBoard(int card)
        {
            deck.DealCard(); //Burn
            board.Add(card); //Specific cards were already removed from deck to avoid dealing in random hands.
        }
    }
}

