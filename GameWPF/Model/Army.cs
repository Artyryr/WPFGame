using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameWPF
{
    public class Army
    {
        public int SpeedUnits { get; set; }
        public int DefenceUnits { get; set; }
        public int AttackUnits { get; set; }
        public AttackUnit Attack { get; set; }
        public DefenceUnit Defence { get; set; }
        public SpeedUnit Speed { get; set; }
        public int SpeedOfMovement { get; set; }

        public Army()
        {
            SpeedUnits = 0;
            AttackUnits = 0;
            DefenceUnits = 0;
            Attack = new AttackUnit();
            Defence = new DefenceUnit();
            Speed = new SpeedUnit();
        }
        public Army(int speed, int attack, int defence)
        {
            SpeedUnits = speed;
            AttackUnits = attack;
            DefenceUnits = defence;
            Attack = new AttackUnit();
            Defence = new DefenceUnit();
            Speed = new SpeedUnit();
        }

        public double GetPower()
        {
            double power = AttackUnits * Attack.Attack + SpeedUnits * Speed.Attack + DefenceUnits * Defence.Attack;
            return power;
        }
        public int TotalArmy()
        {
            return SpeedUnits + AttackUnits + DefenceUnits;
        }
    }
}
