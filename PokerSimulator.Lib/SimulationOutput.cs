using System.Collections.Generic;
using System.IO;

namespace PokerSimulator.Lib
{
    public class SimulationOutput
    {
        private List<string> lines;

        public SimulationOutput()
        {
            lines = new List<string>();
            lines.Add("");
            lines.Add("");
        }

        public void PrintHands(List<SimulatedHand> simulatedHands)
        {
            string a;
            AddLine("---------------------------------------------------------------");
            AddLine("Simulated Hands");
            int i = 0;
            foreach(var handSim in simulatedHands)
            {
                AddLine("---------------------------------------------------------------");
                i++;
                AddLine(i + ".");
                for(int j = 0; j < handSim.Hands.Count; j += 2)
                {
                    AddLine(string.Format("Player {0}'s hand is: {1} {2}", j / 2 + 1, 
                        Deck.CardToString(handSim.Hands[j]), Deck.CardToString(handSim.Hands[j + 1])));
                }
                AddLine();
                AppendLine("Board: ");
                foreach(int card in handSim.Board)
                {
                    AppendLine(Deck.CardToString(card) + " ");
                }
                AddLine();
                AddLine();
                if (handSim.WinningPlayer == 0)
                {
                    AddLine(string.Format("Chop ({0})", handSim.WinningRank));
                }
                else
                {
                    a = "a ";
                    if (handSim.WinningRank == "High Card" || handSim.WinningRank == "Two Pair" || handSim.WinningRank == "Three of a Kind" || handSim.WinningRank == "Four of a Kind")
                    {
                        a = string.Empty;
                    }
                    AddLine(string.Format("The winner is Player {0} with {1}{2}", handSim.WinningPlayer, a, handSim.WinningRank));
                }
            }
        }
        public void PrintResults(Simulation simulation)
        {
            AddLine("---------------------------------------------------------------");
            AddLine("Simulation Results:");
            AddLine("---------------------------------------------------------------");

            AppendLine("Set Board: ");
            foreach (int card in simulation.SetBoard)
            {
                AppendLine(Deck.CardToString(card) + " ");
            }
            AddLine();
            for (int i = 1; i < simulation.PlayerWinCounts.Count; i++)
            {
                string hand;
                if (i <= simulation.DealtHands.Count / 2 - simulation.RandomHands)
                {
                    hand = string.Format("{0} {1}", Deck.CardToString(simulation.DealtHands[i * 2 - 2]), Deck.CardToString(simulation.DealtHands[i * 2 - 1]));
                }
                else
                {
                    hand = "random";
                }
                AddLine(string.Format("Player {0} ({1}) wins: {2} ({3:P})", i, hand, simulation.PlayerWinCounts[i], (double)simulation.PlayerWinCounts[i] / simulation.NumHands));
            }
            AddLine(string.Format("Chopped Pots: {0} ({1:P})", simulation.PlayerWinCounts[0], (double)simulation.PlayerWinCounts[0] / simulation.NumHands));

            AddLine("");
            AddLine("Winning Rank Frequency:");
            for (int i = 0; i < simulation.RankWinCounts.Count; i++ )
            {
                AddLine(string.Format("{0}: {1} ({2:P})", WinnerChecker.Ranks[i], simulation.RankWinCounts[i], (double)simulation.RankWinCounts[i] / simulation.NumHands));
            }
        }
        public void AddLine()
        {
            lines.Add("");
        }

        public void AddLine(string outString)
        {
            lines[lines.Count - 1] = lines[lines.Count - 1] + outString;
            lines.Add("");
        }

        public void AddTopLine()
        {
            lines.Insert(0, "");
        }

        public void AddTopLine(string outString)
        {
            lines[0] = lines[0] + outString;
            lines.Insert(0, "");
        }

        public void AppendToTop(List<string> topLines)
        {
            lines.InsertRange(0, topLines);
        }

        public void AppendLine(string outString)
        {
            lines[lines.Count - 1] = lines[lines.Count - 1] + outString;
        }

        public void AppendTopLine(string outString)
        {
            lines[0] = lines[0] + outString;
        }
        
        public void WriteLinesToFile(string path)
        {
            File.WriteAllLines(path, lines);
        }
    }
}
