using GameWPF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GameWPF.Logic
{
    class BattleLogic
    {
        public double SpeedOfMovement { get; set; }
        public int Distance { get; set; }
        public int numberOfSteps { get; set; }
        public Base OwnBase { get; set; }
        public Base Enemy { get; set; }
        public Army Army { get; set; }
        public List<Base> Enemies { get; set; }
        public TimeSpan GameStepDuration { get; set; }
        public MainWindow Window { get; set; }
        public AttackWindow AttackWindow { get; set; }
        public bool IsUserPlaying { get; set; }

        Random random = new Random();

        public BattleLogic(Base ownBase, Army army, List<Base> enemies, TimeSpan gameStepDuration, bool isUserPlaying, MainWindow window)
        {
            OwnBase = ownBase;
            Army = army;
            Enemies = enemies;
            GameStepDuration = gameStepDuration;
            Window = window;
            IsUserPlaying = isUserPlaying;
        }
        public BattleLogic(Base ownBase, Base enemyBase, Army army, List<Base> enemies, TimeSpan gameStepDuration, bool isUserPlaying, MainWindow window, AttackWindow attackWindow)
        {
            OwnBase = ownBase;
            Enemy = enemyBase;
            Army = army;
            Enemies = enemies;
            GameStepDuration = gameStepDuration;
            Window = window;
            IsUserPlaying = isUserPlaying;
            AttackWindow = attackWindow;
        }

        public void AttackEnemy()
        {
            int attackedEnemy = random.Next(0, Enemies.Count());
            Enemy = Enemies[attackedEnemy];

            if (Enemy.Active != false)
            {
                WarProcess();
            }
        }

        public async void WarProcess()
        {
            if (OwnBase.Active == true)
            {
                int attackUnits = Army.AttackUnits;
                int defenceUnits = Army.DefenceUnits;
                int speedUnits = Army.SpeedUnits;

                if (attackUnits > 0 || defenceUnits > 0 || speedUnits > 0)
                {
                    if (OwnBase.Equals(Window.Base))
                    {
                        AttackWindow.Hide();
                    }

                    OwnBase.Army.AttackUnits -= attackUnits;
                    OwnBase.Army.SpeedUnits -= speedUnits;
                    OwnBase.Army.DefenceUnits -= defenceUnits;

                    CalculateDistance(OwnBase.Position, Enemy.Position);
                    GetSpeed(Army);

                    numberOfSteps = (int)Math.Ceiling(Distance / SpeedOfMovement);

                    TimeSpan stepDuration = GameStepDuration;
                    stepDuration = TimeSpan.FromTicks(stepDuration.Ticks * numberOfSteps);

                    ArmyMovement(OwnBase.Position, Enemy.Position, false, Army, GameStepDuration, Window);
                    await Task.Delay(stepDuration);

                    double attack = GetArmyPower(Army);
                    Enemy.DefencePower();
                    double defence = Enemy.Defence;

                    if (attack > defence)
                    {
                        if (OwnBase.Equals(Window.Base))
                        {
                            AttackWindow.Hide();
                        }
                        double survived = GetSurvived(Army, Enemy);

                        if (Enemy.Equals(Window.Base))
                        {
                            Window.destroyed = true;
                            SetImage(Enemy.Position[0], Enemy.Position[1], "Image/grass.png", Window);
                        }
                        else
                        {
                                SetImage(Enemy.Position[0], Enemy.Position[1], "Image/grass.png", Window);
                                Window.enemies[Enemy.Id - 1].Active = false;                       
                        }

                        if (OwnBase.Equals(Window.Base))
                        {
                            MessageBoxResult result = MessageBox.Show("Вы победили! \nВам будет начислено 1000 кредитов и 1000 товаров. ",
                                                  "Confirmation",
                                                  MessageBoxButton.OK,
                                                  MessageBoxImage.Exclamation);
                        }

                        ArmyMovement(Enemy.Position, OwnBase.Position, true, Army, GameStepDuration, Window);
                        await Task.Delay(stepDuration);

                        SetImage(Enemy.Position[0], Enemy.Position[1], "Image/grass.png", Window);

                        OwnBase.Credits += 1000;
                        OwnBase.Goods += 1000;

                        OwnBase.Army.DefenceUnits += (int)Math.Round(defenceUnits * survived, 0);
                        OwnBase.Army.AttackUnits += (int)Math.Round(attackUnits * survived, 0);
                        OwnBase.Army.SpeedUnits += (int)Math.Round(speedUnits * survived, 0);

                        if (OwnBase.Equals(Window.Base))
                        {
                            SetImage(OwnBase.Position[0], OwnBase.Position[1], "Image/ownCastle.png", Window);
                            SetImage(Enemy.Position[0], Enemy.Position[1], "Image/grass.png", Window);
                            AttackWindow.Close();
                        }
                        else
                        {
                            Window.SetEnemyImage(OwnBase.Position[0], OwnBase.Position[1], "Image/enemiesCastle.png", OwnBase.Id);
                            SetImage(Enemy.Position[0], Enemy.Position[1], "Image/grass.png", Window);
                        }
                    }
                    else
                    {
                        if (OwnBase.Equals(Window.Base))
                        {
                            AttackWindow.Hide();
                        }

                        if (defence - attack > 100)
                        {
                            double defenceLeftPercent = GetDefenceLeft(Army, Enemy);
                            Enemy.Army.AttackUnits = (int)Math.Floor(Enemy.Army.AttackUnits * defenceLeftPercent);
                            Enemy.Army.DefenceUnits = (int)Math.Floor(Enemy.Army.DefenceUnits * defenceLeftPercent);
                            Enemy.Army.SpeedUnits = (int)Math.Floor(Enemy.Army.SpeedUnits * defenceLeftPercent);
                        }
                        else
                        {
                            Enemy.Army.AttackUnits = 0;
                            Enemy.Army.DefenceUnits = 0;
                            Enemy.Army.SpeedUnits = 0;
                        }

                        if (Enemy.Equals(Window.Base))
                        {
                            Window.Base.Army = Enemy.Army;
                            SetImage(Enemy.Position[0], Enemy.Position[1], "Image/ownCastle.png", Window);
                            if (OwnBase.Active == true)
                            {
                                Window.SetEnemyImage(OwnBase.Position[0], OwnBase.Position[1], "Image/enemiesCastle.png", OwnBase.Id);
                            }
                            else
                            {
                                SetImage(OwnBase.Position[0], OwnBase.Position[1], "Image/grass.png", Window);
                            }
                        }
                        else
                        {
                            if (Enemy.Active == true)
                            {
                                double defenceLeftPercent = GetDefenceLeft(Army, Enemy);
                                Window.enemies[Enemy.Id - 1].Army = Enemy.Army;
                                Window.SetEnemyImage(Enemy.Position[0], Enemy.Position[1], "Image/enemiesCastle.png", Enemy.Id);
                            }
                            else
                            {
                                SetImage(Enemy.Position[0], Enemy.Position[1], "Image/grass.png", Window);
                            }

                            if (OwnBase.Equals(Window.Base))
                            {
                                SetImage(OwnBase.Position[0], OwnBase.Position[1], "Image/ownCastle.png", Window);
                            }
                            else if (OwnBase.Active == true)
                            {
                                Window.SetEnemyImage(OwnBase.Position[0], OwnBase.Position[1], "Image/enemiesCastle.png", OwnBase.Id);
                            }
                            else
                            {
                                SetImage(OwnBase.Position[0], OwnBase.Position[1], "Image/grass.png", Window);
                            }
                        }

                        if (OwnBase.Equals(Window.Base))
                        {
                            MessageBoxResult result = MessageBox.Show("Увы, победа за противником! \nНаберите более сильную армию и попробуйте снова... ",
                                               "Confirmation",
                                               MessageBoxButton.OK,
                                               MessageBoxImage.Exclamation);

                            AttackWindow.Close();
                        }
                    }
                }
            }
        }
        private void CalculateDistance(int[] ownPosition, int[] enemyPosition)
        {
            Distance = Math.Abs(ownPosition[0] - enemyPosition[0]) + Math.Abs(ownPosition[1] - enemyPosition[1]);
        }
        private void GetSpeed(Army army)
        {
            if (Convert.ToInt32(army.DefenceUnits) > 0)
            {
                SpeedOfMovement = army.Defence.Speed;
            }
            else if (Convert.ToInt32(army.AttackUnits) > 0)
            {
                SpeedOfMovement = army.Attack.Speed;
            }
            else if (Convert.ToInt32(army.SpeedUnits) > 0)
            {
                SpeedOfMovement = army.Speed.Speed;
            }
        }
        private async void ArmyMovement(int[] firstPoint, int[] secondPoint, bool backMovement, Army army, TimeSpan GameStepDuration, MainWindow window)
        {
            bool movementRight = false;
            bool movementDown = false;

            string knightImg;
            string castleImg;

            if (IsUserPlaying == true)
            {
                knightImg = "ownKnight.png";
                castleImg = "ownCastle.png";
            }
            else
            {
                knightImg = "enemiesKnight.png";
                castleImg = "enemiesCastle.png";
            }

            CalculateDistance(firstPoint, secondPoint);
            GetSpeed(army);

            int speed = (int)SpeedOfMovement;

            TimeSpan stepDuration = GameStepDuration;
            stepDuration = TimeSpan.FromTicks(stepDuration.Ticks / speed);

            if (firstPoint[0] - secondPoint[0] > 0)
            {
                movementRight = false;
            }
            else
            {
                movementRight = true;
            }

            if (firstPoint[1] - secondPoint[1] > 0)
            {
                movementDown = false;
            }
            else
            {
                movementDown = true;
            }
            int i = 0;
            int j = 0;
            int jMax = Math.Abs(firstPoint[1] - secondPoint[1]);
            int iMax = Math.Abs(firstPoint[0] - secondPoint[0]);
            int max = Math.Max(iMax, jMax);

            for (int move = 0; move < max; move++)
            {
                if (backMovement == false)
                {
                    if (IsUserPlaying == true)
                    {
                        SetImage(firstPoint[0], firstPoint[1], "Image/" + castleImg, window);
                    }
                    else
                    {
                        Window.SetEnemyImage(firstPoint[0], firstPoint[1], "Image/" + castleImg, OwnBase.Id);
                        if (Enemy.Equals(Window.Base))
                        {
                            SetImage(secondPoint[0], secondPoint[1], "Image/ownCastle.png", Window);
                        }
                        else {
                            Window.SetEnemyImage(secondPoint[0], secondPoint[1], "Image/" + castleImg, Enemy.Id);
                        }
                    }
                }
                if (movementDown == false)
                {
                    if (movementRight == false)
                    {
                        if (i < iMax)
                        {
                            SetImage(firstPoint[0] - i - 1, firstPoint[1] - j, "Image/" + knightImg, window);
                            SetImage(firstPoint[0] - i, firstPoint[1] - j, "Image/grass.png", window);

                            if (backMovement == false)
                            {
                                if (IsUserPlaying == true)
                                {
                                    SetImage(firstPoint[0], firstPoint[1], "Image/" + castleImg, window);
                                }
                                else
                                {
                                    Window.SetEnemyImage(firstPoint[0], firstPoint[1], "Image/" + castleImg, OwnBase.Id);
                                }
                            }
                            await Task.Delay(stepDuration);
                            i++;
                        }
                        if (j < jMax)
                        {
                            SetImage(firstPoint[0] - i, firstPoint[1] - j - 1, "Image/" + knightImg, window);
                            SetImage(firstPoint[0] - i, firstPoint[1] - j, "Image/grass.png", window);

                            if (backMovement == false)
                            {
                                if (IsUserPlaying == true)
                                {
                                    SetImage(firstPoint[0], firstPoint[1], "Image/" + castleImg, window);
                                }
                                else
                                {
                                    Window.SetEnemyImage(firstPoint[0], firstPoint[1], "Image/" + castleImg, OwnBase.Id);
                                }
                            }
                            await Task.Delay(stepDuration);
                            j++;
                        }
                    }
                    else if (movementRight == true)
                    {
                        if (i < iMax)
                        {
                            SetImage(firstPoint[0] + i + 1, firstPoint[1] - j, "Image/" + knightImg, window);
                            SetImage(firstPoint[0] + i, firstPoint[1] - j, "Image/grass.png", window);

                            if (backMovement == false)
                            {
                                if (IsUserPlaying == true)
                                {
                                    SetImage(firstPoint[0], firstPoint[1], "Image/" + castleImg, window);
                                }
                                else
                                {
                                    Window.SetEnemyImage(firstPoint[0], firstPoint[1], "Image/" + castleImg, OwnBase.Id);
                                }
                            }
                            await Task.Delay(stepDuration);
                            i++;
                        }
                        if (j < jMax)
                        {
                            SetImage(firstPoint[0] + i, firstPoint[1] - j - 1, "Image/" + knightImg, window);
                            SetImage(firstPoint[0] + i, firstPoint[1] - j, "Image/grass.png", window);

                            if (backMovement == false)
                            {
                                if (IsUserPlaying == true)
                                {
                                    SetImage(firstPoint[0], firstPoint[1], "Image/" + castleImg, window);
                                }
                                else
                                {
                                    Window.SetEnemyImage(firstPoint[0], firstPoint[1], "Image/" + castleImg, OwnBase.Id);
                                }
                            }
                            await Task.Delay(stepDuration);
                            j++;
                        }
                    }
                }
                else if (movementDown == true)
                {
                    if (movementRight == false)
                    {
                        if (i < iMax)
                        {
                            SetImage(firstPoint[0] - i - 1, firstPoint[1] + j, "Image/" + knightImg, window);
                            SetImage(firstPoint[0] - i, firstPoint[1] + j, "Image/grass.png", window);

                            if (backMovement == false)
                            {
                                if (IsUserPlaying == true)
                                {
                                    SetImage(firstPoint[0], firstPoint[1], "Image/" + castleImg, window);
                                }
                                else
                                {
                                    Window.SetEnemyImage(firstPoint[0], firstPoint[1], "Image/" + castleImg, OwnBase.Id);
                                }
                            }
                            await Task.Delay(stepDuration);
                            i++;
                        }
                        if (j < jMax)
                        {
                            SetImage(firstPoint[0] - i, firstPoint[1] + j + 1, "Image/" + knightImg, window);
                            SetImage(firstPoint[0] - i, firstPoint[1] + j, "Image/grass.png", window);

                            if (backMovement == false)
                            {
                                if (IsUserPlaying == true)
                                {
                                    SetImage(firstPoint[0], firstPoint[1], "Image/" + castleImg, window);
                                }
                                else
                                {
                                    Window.SetEnemyImage(firstPoint[0], firstPoint[1], "Image/" + castleImg, OwnBase.Id);
                                }
                            }
                            await Task.Delay(stepDuration);
                            j++;
                        }
                    }
                    else if (movementRight == true)
                    {
                        if (i < iMax)
                        {
                            SetImage(firstPoint[0] + i + 1, firstPoint[1] + j, "Image/" + knightImg, window);
                            SetImage(firstPoint[0] + i, firstPoint[1] + j, "Image/grass.png", window);

                            if (backMovement == false)
                            {
                                if (IsUserPlaying == true)
                                {
                                    SetImage(firstPoint[0], firstPoint[1], "Image/" + castleImg, window);
                                }
                                else
                                {
                                    Window.SetEnemyImage(firstPoint[0], firstPoint[1], "Image/" + castleImg, OwnBase.Id);
                                }
                            }
                            await Task.Delay(stepDuration);
                            i++;
                        }
                        if (j < jMax)
                        {
                            SetImage(firstPoint[0] + i, firstPoint[1] + j + 1, "Image/" + knightImg, window);
                            SetImage(firstPoint[0] + i, firstPoint[1] + j, "Image/grass.png", window);

                            if (backMovement == false)
                            {
                                if (IsUserPlaying == true)
                                {
                                    SetImage(firstPoint[0], firstPoint[1], "Image/" + castleImg, window);
                                }
                                else
                                {
                                    Window.SetEnemyImage(firstPoint[0], firstPoint[1], "Image/" + castleImg, OwnBase.Id);
                                }
                            }
                            await Task.Delay(stepDuration);
                            j++;
                        }
                    }
                }
                if (backMovement == true)
                {
                    if (IsUserPlaying == true)
                    {
                        SetImage(secondPoint[0], secondPoint[1], "Image/" + castleImg, window);
                    }
                    else if (OwnBase.Active == true)
                    {
                        Window.SetEnemyImage(secondPoint[0], secondPoint[1], "Image/" + castleImg, OwnBase.Id);
                    }
                    else
                    {
                        SetImage(secondPoint[0], secondPoint[1], "Image/grass.png", window);
                    }
                }
            }
        }
        private double GetSurvived(Army army, Base enemy)
        {
            double attack = GetArmyPower(army);
            enemy.DefencePower();
            double defence = enemy.Defence;

            double difference = attack - defence;
            double survived = difference / attack;
            return survived;
        }
        public double GetArmyPower(Army army)
        {
            double power = army.AttackUnits * army.Attack.Attack + army.SpeedUnits * army.Speed.Attack + army.DefenceUnits * army.Defence.Attack;
            return power;
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
        public double GetDefenceLeft(Army army, Base enemy)
        {
            double attack = GetArmyPower(army);
            enemy.DefencePower();
            double defence = enemy.Defence;

            double difference = defence - attack - 100;
            double survived = difference / defence;
            return survived;
        }
    }
}
