using System;

namespace PokerSimulator
{
    class PokerSimulatorMain
    {
        static void Main(string[] args)
        {
            var dealer = new Dealer();
            Console.WriteLine("Poker Hand Simulator");
            dealer.GetUserInput();
            Console.ReadKey();
        }
    }
}
