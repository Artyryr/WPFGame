using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameWPF
{
    public class Portal : Building
    {
        public double BuyGood { get; set; }
        public double SellGood { get; set; }
        public Portal()
        {
            Lvl = 1;
            BuyGood = 1.5;
            SellGood = 0.5;
        }
    }
}
