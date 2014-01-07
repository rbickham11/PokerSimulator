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
