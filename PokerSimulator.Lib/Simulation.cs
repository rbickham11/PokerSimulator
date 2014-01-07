using System;
using System.Collections.Generic;

namespace PokerSimulator.Lib
{
    public class Simulation
    {
        public const int MAX_HANDS = 10;

        private Deck deck = new Deck();
        private WinnerChecker winnerChecker;
        private SimulationOutput output = new SimulationOutput();
        private List<int> board;

        public int RandomHands { get; private set; }
        public int NumHands { get; private set; }
        public List<int> SetBoard { get; private set; }
        public List<int> DealtHands { get; private set; }
        public List<int> PlayerWinCounts { get; private set; }
        public List<int> RankWinCounts { get; private set; }

        public Simulation()
        {
            SetBoard = new List<int>();
            DealtHands = new List<int>();
            deck.Shuffle();
            RandomHands = 0;
            PlayerWinCounts = new List<int>();
            RankWinCounts = new List<int>();
            for(int i = 0; i < WinnerChecker.Ranks.Count; i++)
            {
                RankWinCounts.Add(0);
            }
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
            winnerChecker = new WinnerChecker(output, DealtHands.Count / 2);
            if(!randomChange)
            {
                RandomHands = 0;
            }

            List<int> specHands = new List<int>();

            int i, j;

            if (randomChange)
            {
                for (i = 0; i < (DealtHands.Count / 2 - RandomHands) * 2; i++)
                    specHands.Add(DealtHands[i]);
            }
            
            for (i = 0; i < DealtHands.Count / 2 + 1; i++ ) //For all players and at index 0 for chops
            {
                PlayerWinCounts.Add(0);
            }
            
            output.AddLine("---------------------------------------------------------------");
            output.AddLine("Simulated Hands:");
            for (i = 0; i < NumHands; i++)
            {
                output.AddLine("---------------------------------------------------------------");
                output.AddLine(string.Format("{0}.", i + 1));
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

                for (j = 0; j < DealtHands.Count; j += 2)
                {
                    PrintPlayerHand(false, j / 2 + 1, DealtHands.GetRange(j, 2));
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
                
                output.AddLine();
                output.AppendLine("Board: ");
                foreach (int card in board)
                {
                    output.AppendLine(Deck.CardToString(card) + " ");
                }
                
                winnerChecker.FindWinner(DealtHands, board);

                string a = "a ";
                if (winnerChecker.WinningRank == 0 || winnerChecker.WinningRank == 2 || winnerChecker.WinningRank == 3 || winnerChecker.WinningRank == 7)
                    a = string.Empty;
                output.AddLine();
                output.AddLine();
                if (winnerChecker.WinningPlayer == 0)
                    output.AddLine(string.Format("Chop ({0})", WinnerChecker.Ranks[winnerChecker.WinningRank]));
                else
                {
                    output.AddLine(string.Format("The winner is Player {0} with {1}{2}", winnerChecker.WinningPlayer, a, WinnerChecker.Ranks[winnerChecker.WinningRank]));
                }
                
                PlayerWinCounts[winnerChecker.WinningPlayer]++;
                RankWinCounts[winnerChecker.WinningRank]++;
            }

            PrintResults();
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
        public void PrintPlayerHand(bool console, int playerNumber, List<int> hand)
        {
            if (console)
            {
                Console.WriteLine("Player {0}'s hand is: {1} {2}", playerNumber, Deck.CardToString(hand[0]), Deck.CardToString(hand[1]));
            }
            else
            {
                output.AddLine(string.Format("Player {0}'s hand is: {1} {2}", playerNumber, Deck.CardToString(hand[0]), Deck.CardToString(hand[1])));
            }
        }

        public void PrintResults()
        {
            string hand;
            List<string> results = new List<string>();

            results.Add("---------------------------------------------------------------");
            results.Add("Simulation Results:");
            results.Add("---------------------------------------------------------------");

            results.Add(string.Format("Set Board: "));
            foreach (int card in SetBoard)
            {
                results[results.Count - 1] += Deck.CardToString(card);
            }

            for (int i = 1; i < PlayerWinCounts.Count; i++)
            {
                if (i <= DealtHands.Count / 2 - RandomHands)
                {
                    hand = string.Format("{0} {1}", Deck.CardToString(DealtHands[i * 2 - 2]), Deck.CardToString(DealtHands[i * 2 - 1]));
                }
                else
                {
                    hand = "random";
                }
                results.Add(string.Format("Player {0} ({1}) wins: {2} ({3:P})", i, hand, PlayerWinCounts[i], (double)PlayerWinCounts[i] / NumHands));
            }
            results.Add(string.Format("Chopped Pots: {0} ({1:P})", PlayerWinCounts[0], (double)PlayerWinCounts[0] / NumHands));

            results.Add("");
            results.Add("Winning Rank Frequency:");
            for (int i = 0; i < RankWinCounts.Count; i++ )
            {
                results.Add(string.Format("{0}: {1} ({2:P})", WinnerChecker.Ranks[i], RankWinCounts[i], (double)RankWinCounts[i] / NumHands));
            }
            output.AppendToTop(results);
        }

        public void PrintOutputToFile(string filePath)
        {
            output.WriteLinesToFile(filePath);
        }
    }
}

