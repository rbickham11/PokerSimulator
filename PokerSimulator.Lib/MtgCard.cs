using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerSimulator.Lib
{
    public class MtgCard
    {
        private int power;
        public string Name;

        public void setPower(int powNum)
        {
            if(powNum > 0 && powNum < 25)
            {
                power = powNum;
            }
            else
            {
                throw new Exception();
            }
        }
    }
}
