using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameWPF
{
    public class AttackUnit : Unit
    {
        public AttackUnit()
        {
            Speed = 2;
            Attack = 5;
            Defence = 1;
        }
    }
}
