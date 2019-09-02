using GameWPF.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GameWPF.Model
{
    public class Enemy : Base
    {
        public BehaviorType Behavior { get; set; }
        public double SpeedOfMovement { get; set; }
        public int Distance { get; set; }
        public int numberOfSteps { get; set; }

        int attackCycle = 0;
        Random random = new Random(DateTime.Now.Millisecond);

        public Enemy(int id)
        {
            Id = id;
        }
        public Enemy(int id, int[] position)
        {
            Id = id;
            Position = position;
            lock (random)
            {
                Behavior = (BehaviorType)random.Next(1, 4);
            }
            Active = true;
        }
        public Enemy(int[] position)
        {
            Position = position;
            Active = true;
            lock (random)
            {
                Behavior = (BehaviorType)random.Next(1, 4);
            }
        }
        public Enemy(int id, int[] position, int behavior)
        {
            Id = id;
            Position = position;
            Behavior = (BehaviorType)behavior;
            Active = true;
        }
        public Enemy(int[] position, int behavior)
        {
            Position = position;
            Behavior = (BehaviorType)behavior;
            Active = true;
        }
        public void LifeCycle(TimeSpan GameStepDuration, MainWindow window)
        {
            if (Active == true)
            {
                CreditsStep();
                PopulationStep();
                GoodsStep();

                if (Behavior == BehaviorType.Aggressor)
                {
                    if (attackCycle % 4 == 0 && Army.TotalArmy() >= ArmyLimit / 2 && Hut.Lvl >= 2)
                    {
                        attackCycle++;
                        Army army = new Army(Army.SpeedUnits / 3, Army.AttackUnits / 3, Army.DefenceUnits / 3);
                        BattleLogic logic = new BattleLogic(this, army, Enemies, GameStepDuration, false, window);
                        logic.AttackEnemy();
                    }
                    else if (attackCycle % 3 == 0 && Army.TotalArmy() >= ArmyLimit / 2 && Hut.Lvl >= 3)
                    {
                        attackCycle++;
                        Army army = new Army(Army.SpeedUnits / 2, Army.AttackUnits / 2, Army.DefenceUnits / 2);
                        BattleLogic logic = new BattleLogic(this, army, Enemies, GameStepDuration, false, window);
                        logic.AttackEnemy();
                    }
                    else if (attackCycle % 3 == 0 && Army.TotalArmy() >= ArmyLimit / 2 && Hut.Lvl >= 4)
                    {
                        attackCycle++;
                        Army army = new Army(Army.SpeedUnits, Army.AttackUnits, Army.DefenceUnits);
                        BattleLogic logic = new BattleLogic(this, army, Enemies, GameStepDuration, false, window);
                        logic.AttackEnemy();
                    }

                    if (Army.TotalArmy() <= ArmyLimit / 2 && Hut.Lvl >= 2)
                    {
                        attackCycle++;
                        ArmyCreation(0, GetMaxNumberOfArmyCreation(), 0);
                    }

                    if (GetUpdatePrice(Hut)[0] <= Credits && GetUpdatePrice(Hut)[1] <= Goods && Hut.Lvl <= 5)
                    {
                        attackCycle++;
                        BuildingLvlUp(Hut);
                    }
                    else if (Wall.Lvl <= 3)
                    {
                        attackCycle++;
                        BuildingLvlUp(Wall);
                    }
                    else if (BaseLvl < 3)
                    {
                        attackCycle++;
                        BaseLvlUp();
                    }
                }
                else if (Behavior == BehaviorType.Builder)
                {
                    Building building = GetBuildingWithMinimalLvl();

                    if (GetUpdatePrice(building)[0] <= Credits / 2 && GetUpdatePrice(building)[1] <= Goods / 2 && building.Lvl < 3)
                    {
                        attackCycle++;
                        BuildingLvlUp(building);
                    }

                    if (Army.TotalArmy() > ArmyLimit / 3 && attackCycle % 3 == 0)
                    {
                        attackCycle++;
                        Army army = new Army(Army.SpeedUnits / 2, Army.AttackUnits, Army.DefenceUnits / 2);
                        BattleLogic logic = new BattleLogic(this, army, Enemies, GameStepDuration, false, window);
                        logic.AttackEnemy();
                    }

                    if ((BaseLvl <= building.Lvl - 2) && BaseLvl <= 3)
                    {
                        attackCycle++;
                        BaseLvlUp();
                    }
                    else if (building.Lvl == 2 && Army.TotalArmy() < ArmyLimit / 2)
                    {
                        attackCycle++;
                        ArmyCreation(0, 0, GetMaxNumberOfArmyCreation());
                    }
                    else if (GetUpdatePrice(building)[0] <= Credits && GetUpdatePrice(building)[1] <= Goods && building.Lvl < 3)
                    {
                        attackCycle++;
                        BuildingLvlUp(building);
                    }
                    else if (building.Lvl >= 3 && Army.TotalArmy() < ArmyLimit * 0.7)
                    {
                        attackCycle++;
                        ArmyCreation(0, GetMaxNumberOfArmyCreation() / 2, GetMaxNumberOfArmyCreation() / 3);
                    }
                    else if (attackCycle % 2 == 0)
                    {
                        attackCycle++;
                        ArmyCreation(0, GetMaxNumberOfArmyCreation(), 0);
                        Army army = new Army(Army.SpeedUnits / 2, Army.AttackUnits, Army.DefenceUnits / 2);
                        BattleLogic logic = new BattleLogic(this, army, Enemies, GameStepDuration, false, window);
                        logic.AttackEnemy();
                    }
                }
                else if (Behavior == BehaviorType.Traider)
                {
                    if (GetUpdatePrice(Workshop)[0] <= Credits && GetUpdatePrice(Workshop)[1] <= Goods && Workshop.Lvl < 4)
                    {
                        attackCycle++;
                        BuildingLvlUp(Workshop);
                    }
                    if (GetUpdatePrice(Portal)[0] <= Credits && GetUpdatePrice(Portal)[1] <= Goods && Portal.Lvl < 4)
                    {
                        attackCycle++;
                        BuildingLvlUp(Portal);
                    }

                    if (Army.TotalArmy() < ArmyLimit * 0.7 && BaseLvl < 3)
                    {
                        attackCycle++;
                        ArmyCreation(0, 0, GetMaxNumberOfArmyCreation());
                    }
                    else if (GetBaseUpdatePrice()[0] == Credits && GetBaseUpdatePrice()[1] == Goods && BaseLvl <= 3)
                    {
                        attackCycle++;
                        BaseLvlUp();
                    }
                    else if (GetMaxNumberOfArmyCreation() >= (Army.TotalArmy() - ArmyLimit) / 2)
                    {
                        attackCycle++;
                        ArmyCreation(0, GetMaxNumberOfArmyCreation(), 0);
                    }
                    else if (Army.TotalArmy() == ArmyLimit && GetMaxNumberOfArmyCreation() == ArmyLimit)
                    {
                        attackCycle++;
                        Army army = new Army(Army.SpeedUnits, Army.AttackUnits, Army.DefenceUnits);
                        BattleLogic logic = new BattleLogic(this, army, Enemies, GameStepDuration, false, window);

                        ArmyCreation(0, GetMaxNumberOfArmyCreation(), 0);
                    }
                }
            }
            else
            {
                SetImage(Position[0], Position[1], "Image/grass.png", window);
            }
        }
        public new void SetEnemies(List<Base> enemies, Base playerBase)
        {
            Enemies = new List<Base>(enemies);
            Enemies[Id] = playerBase;
        }

        public new void ArmyCreation(int speed, int attack, int defence)
        {
            int totalPrice = 10 * (speed + attack + defence);
            int totalGoods = 10 * (speed + attack + defence);
            int totalPeople = speed + attack + defence;

            if (Credits - totalPrice >= 0 && Goods - totalGoods >= 0 && Population - totalPeople >= 0)
            {
                if (Army.TotalArmy() + totalPeople <= ArmyLimit)
                {
                    Credits -= totalPrice;
                    Goods -= totalGoods;
                    Population -= totalPeople;
                    Army.AttackUnits += attack;
                    Army.DefenceUnits += defence;
                    Army.SpeedUnits += speed;
                }
            }
        }
        public int GetMaxNumberOfArmyCreation()
        {
            int price = (int)Math.Floor(Credits / 10);
            int goods = (int)Math.Floor((double)Goods / 10);
            int population = (int)Math.Floor((double)Population / 10);
            int min = new int[] { price, goods, population }.Min();
            return min;
        }
        public Building GetBuildingWithMinimalLvl()
        {
            List<Building> buildings = new List<Building> { this.Hut, Portal, Residence, Wall, Workshop };
            return buildings.Where(building => building.Lvl == buildings.Min(b => b.Lvl)).FirstOrDefault();
        }
        private void SetImage(int row, int col, string image, MainWindow window)
        {
            Button btn = new Button();
            Image img = new Image();

            BitmapImage bitImg = new BitmapImage();
            bitImg.BeginInit();
            bitImg.UriSource = new Uri(image, UriKind.Relative);
            bitImg.EndInit();
            img.Stretch = Stretch.Fill;
            img.Source = bitImg;

            btn.Content = img;

            Grid.SetRow(btn, row);
            Grid.SetColumn(btn, col);

            window.grid.Children.Remove(btn);
            window.grid.Children.Add(btn);
        }
    }
}