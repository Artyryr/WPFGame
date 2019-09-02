using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GameWPF
{
    public class Base
    {
        public int Id { get; set; }
        public int BaseLvl { get; set; }
        public int Goods { get; set; }
        public int Population { get; set; }
        public double Credits { get; set; }
        public int PopulationLimit { get; set; }
        public int ArmyLimit { get; set; }
        public Hut Hut { get; set; }
        public Residence Residence { get; set; }
        public Workshop Workshop { get; set; }
        public Portal Portal { get; set; }
        public Wall Wall { get; set; }
        public Army Army { get; set; }
        public double Defence { get; set; }
        public int[] Position { get; set; }
        public List<Base> Enemies { get; set; }
        public bool Active { get; set; }

        double creditsNeededBase = 1000;
        int populationNeededBase = 500;
        int goodsNeededBase = 1000;

        double creditsNeeded = 102;
        int goodsNeeded = 102;
        int castleDefence = 100;

        public static int id = 0;

        public Base()
        {
            Id = id;
            BaseLvl = 1;
            Goods = 300;
            Population = 200;
            Credits = 500;
            PopulationLimit = 1000;
            ArmyLimit = 200;
            Hut = new Hut();
            Residence = new Residence();
            Workshop = new Workshop();
            Portal = new Portal();
            Wall = new Wall();
            Defence = castleDefence;
            Army = new Army();
            Active = true;

            id++;
        }
        public void CreditsStep()
        {
            double creditsIncrease = Population * 0.1;

            if (Portal.Lvl > 1)
            {
                creditsIncrease += Convert.ToInt32((Portal.Lvl - 1) * 0.0025 * creditsIncrease);
            }

            Credits += creditsIncrease;
        }
        public void PopulationStep()
        {
            int populationIncrease = 100;

            if (Residence.Lvl > 1)
            {
                populationIncrease += Convert.ToInt32(populationIncrease * 0.05 * (Residence.Lvl - 1));
            }

            if (Population + populationIncrease <= PopulationLimit)
            {
                Population += populationIncrease;
            }
            else
            {
                Population = PopulationLimit;
            }
        }
        public void GoodsStep()
        {
            int goodsIncrease = 100;

            if (Portal.Lvl > 1)
            {
                goodsIncrease += Convert.ToInt32((Portal.Lvl - 1) * 0.0025 * goodsIncrease);
            }

            if (Workshop.Lvl > 1)
            {
                goodsIncrease += Convert.ToInt32((Workshop.Lvl - 1) * 0.0175 * goodsIncrease);
            }

            Goods += goodsIncrease;
        }
        public double[] GetBaseUpdatePrice()
        {
            double credits = 1000;
            double population = 500;
            double goods = 1000;

            double[] prices = { credits, population, goods };

            if (BaseLvl > 1)
            {
                credits = PriceOfUpdate(BaseLvl, creditsNeededBase);
                population = PriceOfUpdate(BaseLvl, populationNeededBase);
                goods = PriceOfUpdate(BaseLvl, goodsNeededBase);

                prices = new double[] { credits, population, goods };
            }

            return prices;
        }
        public void BaseLvlUp()
        {
            double credits = 1000;
            int population = 500;
            int goods = 1000;

            if (BaseLvl > 1)
            {
                credits = Math.Round(Convert.ToDouble(100 * (BaseLvl - 1) + creditsNeededBase + BaseLvl / 0.985), 0);
                population = (int)Math.Round(Convert.ToDouble(100 * (BaseLvl - 1) + populationNeededBase + BaseLvl / 0.985), 0);
                goods = (int)Math.Round(Convert.ToDouble(100 * (BaseLvl - 1) + goodsNeededBase + BaseLvl / 0.985), 0);
            }

            if (Credits >= credits && Population >= population && Goods >= goods)
            {
                BaseLvl += 1;
                PopulationLimit += 1000;
                ArmyLimit += 200;

                if (Wall.Lvl > 5)
                {
                    Wall.Lvl -= 5;
                }
                else
                {
                    Wall.Lvl = 1;
                }

                Credits -= credits;
                Population -= population;
                Goods -= goods;
            }
        }
        public double[] GetUpdatePrice(Building building)
        {
            double credits = 102;
            double goods = 102;

            double[] prices = { credits, goods };

            if(building.Lvl > 1){
                credits = PriceOfUpdate(building.Lvl, creditsNeeded);
                goods = PriceOfUpdate(building.Lvl, goodsNeeded);
                prices = new double[] { credits, goods };
            }
            
            return prices;
        }
        public void BuildingLvlUp(Building building)
        {
            double credits = 102;
            double goods = 102;

            if (building.Lvl > 1)
            {
                credits = Math.Round(Convert.ToDouble(PriceOfUpdate(building.Lvl, credits)), 0);
                goods = Convert.ToInt32(PriceOfUpdate(building.Lvl, goods));
            }

            if (Credits >= credits && Goods >= goods)
            {
                Credits -= (int)Math.Ceiling(credits);
                Goods -= (int)Math.Ceiling(goods);

                if (building is Hut)
                {
                    this.Hut.Lvl += 1;
                    ArmyLimit += 100;

                    Army.Attack.Attack += 0.1;
                    Army.Attack.Defence += 0.1;
                    Army.Defence.Attack += 0.1;
                    Army.Defence.Defence += 0.1;
                    Army.Speed.Attack += 0.1;
                    Army.Speed.Defence += 0.1;
                }
                if (building is Residence)
                {
                    this.Residence.Lvl += 1;
                    PopulationLimit += 200;
                }
                if (building is Portal)
                {
                    this.Portal.Lvl += 1;
                    double difference = (Portal.BuyGood - Portal.SellGood) / 200;
                    Portal.BuyGood -= difference;
                    Portal.SellGood += difference;
                }
                if (building is Wall)
                {
                    this.Wall.Lvl += 1;
                    DefencePower();
                }
                if (building is Workshop)
                {
                    this.Workshop.Lvl += 1;
                }
            }
        }
        public void GoodsConverter(int buy, int sell)
        {
            double buyPrice = buy * Portal.BuyGood;
            double sellAmount = sell * Goods;
            if ((Credits - buyPrice) >= 0 && (Goods - sellAmount) >= 0)
            {
                Credits -= buyPrice;
                Goods += buy;

                Goods -= sell;
                Credits += sell * Portal.SellGood;

                Console.WriteLine("Buy" + buy);
                Console.WriteLine(sell);
            }
        }
        public void ArmyCreation(int speed, int attack, int defence)
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

                    DefencePower();
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("Армия не создана! \n Число военных юнитов не может быть больше: " + ArmyLimit,
                                          "Ошибка создания армии",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Exclamation);
                }
            }
        }
        public void DefencePower()
        {
            int totalAmount = Army.DefenceUnits + Army.AttackUnits + Army.SpeedUnits;
            double defencePower = castleDefence + Army.DefenceUnits * Army.Defence.Defence + Army.AttackUnits * Army.Attack.Defence + Army.SpeedUnits * Army.Speed.Defence + totalAmount * 0.05 * Wall.Lvl;

            Defence = Math.Round(defencePower);
        }
        public double PriceOfUpdate(int lvl, double initialValue)
        {
            double result = Math.Round(Convert.ToDouble(100 * (lvl - 1) + initialValue + lvl / 0.985), 0);
            return result;
        }
        public void LifeCycle(TimeSpan time, Window win)
        {

        }
        public void SetEnemies(List<Base> enemies, Base playerBase)
        {
            Enemies = new List<Base>(enemies);
            Enemies[Id - 1] = playerBase;
        }
    }
}



