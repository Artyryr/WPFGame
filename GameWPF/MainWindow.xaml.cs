using GameWPF.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GameWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        public Grid grid = new Grid();
        private TimeSpan gameStepDuration = new TimeSpan(0, 0, 4);
        private int numberOfEnemies;
        public List<Base> enemies = new List<Base>();
        private bool gameOver = false;
        private bool winnedGame = false;
        private Stopwatch stopwatch = new Stopwatch();
        private string PlayerName;
        private Random random = new Random(DateTime.Now.Millisecond);
        public List<GameResults> gameResults = new List<GameResults>();
        public bool destroyed = false;
        public bool enemiesLeft = false;

        public Base Base = new Base();

        public MainWindow()
        {
            InitializeComponent();
        }
        public MainWindow(int gameStep, int enemiesNumber)
        {
            gameStepDuration = new TimeSpan(0, 0, gameStep);
            numberOfEnemies = enemiesNumber;

            InitializeComponent();
        }
        public MainWindow(int gameStep, int enemiesNumber, string playerName)
        {
            gameStepDuration = new TimeSpan(0, 0, gameStep);
            numberOfEnemies = enemiesNumber;
            PlayerName = playerName;

            InitializeComponent();
        }
        public MainWindow(int gameStep, int enemiesNumber, string playerName, List<GameResults> gameResults)
        {
            gameStepDuration = new TimeSpan(0, 0, gameStep);
            numberOfEnemies = enemiesNumber;
            PlayerName = playerName;
            this.gameResults = gameResults;

            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetUpTimer();
            grid = mapGrid;
            InitializeMap();
            foreach (Base enemy in enemies.ToList())
            {
                enemy.SetEnemies(enemies, Base);
            }
            stopwatch.Start();
        }
        private void SetUpTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = gameStepDuration;
            timer.Tick += new EventHandler(GameStepChange);
            timer.Start();
        }
        private void GameStepChange(object sender, EventArgs e)
        {
            if (gameOver == false)
            {
                enemiesLeft = false;
                SetPopulation();
                SetGoods();
                SetCredits();
                SetData();
                SetImage(Base.Position[0], Base.Position[1], "Image/ownCastle.png");
                CheckWinningCombination();
                foreach (Base enemy in enemies.ToList())
                {
                    if (enemy != null)
                    {
                        enemy.SetEnemies(enemies, Base);
                        if (enemy is Enemy)
                        {
                            Enemy enemyBase = (Enemy)enemy;
                            enemyBase.LifeCycle(gameStepDuration, this);
                        }
                    }
                }
                CommandManager.InvalidateRequerySuggested();
            }
        }
        private void InitializeMap()
        {
            SetGrass();

            if (numberOfEnemies == 3)
            {
                SetImage(2, 2, "Image/ownCastle.png");
                SetEnemyImage(2, 12, "Image/enemiesCastle.png", 1);
                SetEnemyImage(12, 2, "Image/enemiesCastle.png", 2);
                SetEnemyImage(12, 12, "Image/enemiesCastle.png", 3);

                Base.Position = new int[] { 2, 2 };

                List<int[]> positions = new List<int[]>() { new int[] { 2, 12 }, new int[] { 12, 2 }, new int[] { 12, 12 } };

                for (int id = 1; id <= 3; id++)
                {
                    enemies.Add(new Enemy(id, positions[id - 1], random.Next(1, 4)));
                }
            }
            else if (numberOfEnemies == 4)
            {
                SetImage(7, 7, "Image/ownCastle.png");
                SetEnemyImage(2, 2, "Image/enemiesCastle.png", 1);
                SetEnemyImage(2, 12, "Image/enemiesCastle.png", 2);
                SetEnemyImage(12, 2, "Image/enemiesCastle.png", 3);
                SetEnemyImage(12, 12, "Image/enemiesCastle.png", 4);

                Base.Position = new int[] { 7, 7 };

                List<int[]> positions = new List<int[]>() { new int[] { 2, 2 }, new int[] { 2, 12 }, new int[] { 12, 2 }, new int[] { 12, 12 } };

                for (int id = 1; id <= 4; id++)
                {
                    enemies.Add(new Enemy(id, positions[id - 1], random.Next(1, 4)));
                }
            }
            else if (numberOfEnemies == 5)
            {
                SetImage(7, 4, "Image/ownCastle.png");
                SetEnemyImage(7, 10, "Image/enemiesCastle.png", 1);
                SetEnemyImage(2, 2, "Image/enemiesCastle.png", 2);
                SetEnemyImage(2, 12, "Image/enemiesCastle.png", 3);
                SetEnemyImage(12, 2, "Image/enemiesCastle.png", 4);
                SetEnemyImage(12, 12, "Image/enemiesCastle.png", 5);

                Base.Position = new int[] { 7, 4 };

                List<int[]> positions = new List<int[]>() { new int[] { 7, 10 }, new int[] { 2, 2 }, new int[] { 2, 12 }, new int[] { 12, 2 }, new int[] { 12, 12 } };

                for (int id = 1; id <= 5; id++)
                {
                    enemies.Add(new Enemy(id, positions[id - 1], random.Next(1, 4)));
                }
            }
        }
        private void SetCredits()
        {
            Base.CreditsStep();
            creditsText.Text = Base.Credits.ToString();
        }
        private void SetPopulation()
        {
            Base.PopulationStep();
            populationText.Text = Base.Population.ToString() + " / " + Base.PopulationLimit.ToString();
        }
        private void SetGoods()
        {
            Base.GoodsStep();
            goodsText.Text = Base.Goods.ToString();
        }
        private void SetGrass()
        {
            for (int row = 0; row < 15; row++)
            {
                for (int col = 0; col < 15; col++)
                {
                    SetImage(row, col, "Image/grass.png");
                }
            }
        }
        private void SetData()
        {
            double[] baseUpdatePrices = Base.GetBaseUpdatePrice();
            BasePriceTxt.Content = baseUpdatePrices[0] + "кр.," + baseUpdatePrices[1] + "лд., " + baseUpdatePrices[2] + "тов.";
            BaseLvlLabel.Content = "Lvl " + this.Base.BaseLvl;

            double[] updatePrices = Base.GetUpdatePrice(Base.Hut);
            HutPriceLbl.Content = updatePrices[0] + "кр., " + updatePrices[1] + "тов.";
            HutLvlLabel.Content = "Lvl " + Base.Hut.Lvl;

            updatePrices = Base.GetUpdatePrice(Base.Residence);
            ResidencePriceLbl.Content = updatePrices[0] + "кр., " + updatePrices[1] + "тов.";
            ResidenceLvlLabel.Content = "Lvl " + Base.Residence.Lvl;

            updatePrices = Base.GetUpdatePrice(Base.Wall);
            WallPriceLbl.Content = updatePrices[0] + "кр., " + updatePrices[1] + "тов.";
            WallLvlLabel.Content = "Lvl " + Base.Wall.Lvl;

            updatePrices = Base.GetUpdatePrice(Base.Workshop);
            WorkshopPriceLbl.Content = updatePrices[0] + "кр., " + updatePrices[1] + "тов.";
            WorkshopLvlLabel.Content = "Lvl " + Base.Workshop.Lvl;

            updatePrices = Base.GetUpdatePrice(Base.Portal);
            PortalPriceLbl.Content = updatePrices[0] + "кр., " + updatePrices[1] + "тов.";
            PortalLvlLabel.Content = "Lvl " + Base.Portal.Lvl;

            Base.DefencePower();
            DeffenceLbl.Content = Base.Defence;

            SpeedAmount.Content = Base.Army.SpeedUnits;
            DeffenceAmount.Content = Base.Army.DefenceUnits;
            AttackAmount.Content = Base.Army.AttackUnits;
        }
        private void SetImage(int row, int col, string URL)
        {
            Button btn = new Button();
            Image img = new Image();

            BitmapImage bitImg = new BitmapImage();
            bitImg.BeginInit();
            bitImg.UriSource = new Uri(URL, UriKind.Relative);
            bitImg.EndInit();
            img.Stretch = Stretch.Fill;
            img.Source = bitImg;

            btn.Content = img;

            Grid.SetRow(btn, row);
            Grid.SetColumn(btn, col);

            grid.Children.Add(btn);
        }
        public void SetEnemyImage(int row, int col, string URL, int id)
        {
            Button btn = new Button();
            Image img = new Image();

            BitmapImage bitImg = new BitmapImage();
            bitImg.BeginInit();
            bitImg.UriSource = new Uri(URL, UriKind.Relative);
            bitImg.EndInit();
            img.Stretch = Stretch.Fill;
            img.Source = bitImg;

            btn.Content = img;
            btn.Click += EnemiesCastleButton_Click;
            btn.Name += "EnemyButton" + id;

            Grid.SetRow(btn, row);
            Grid.SetColumn(btn, col);

            grid.Children.Add(btn);
        }
        private void EnemiesCastleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                int enemyId = Convert.ToInt32(btn.Name.ElementAt(11).ToString());
                Base enemy = enemies[enemyId - 1];

                if (enemy != null && enemy.Active != false)
                {
                    AttackWindow attackWindow = new AttackWindow((Enemy)enemy, this);
                    attackWindow.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async void BaseLvlButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            btn.IsEnabled = false;
            await Task.Delay(gameStepDuration);
            btn.IsEnabled = true;

            Base.BaseLvlUp();
        }
        private async void HutLvlButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            btn.IsEnabled = false;
            await Task.Delay(gameStepDuration);
            btn.IsEnabled = true;

            Base.BuildingLvlUp(Base.Hut);
        }
        private async void ResidenceLvlButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            btn.IsEnabled = false;
            await Task.Delay(gameStepDuration);
            btn.IsEnabled = true;

            Base.BuildingLvlUp(Base.Residence);
        }
        private async void WallLvlButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            btn.IsEnabled = false;
            await Task.Delay(gameStepDuration);
            btn.IsEnabled = true;

            Base.BuildingLvlUp(Base.Wall);
        }
        private async void WorkshopLvlButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            btn.IsEnabled = false;
            await Task.Delay(gameStepDuration);
            btn.IsEnabled = true;

            Base.BuildingLvlUp(Base.Workshop);
        }
        private async void PortalLvlButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            btn.IsEnabled = false;
            await Task.Delay(gameStepDuration);
            btn.IsEnabled = true;

            Base.BuildingLvlUp(Base.Portal);
        }
        private async void PortalButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                btn.IsEnabled = false;
                await Task.Delay(gameStepDuration);
                btn.IsEnabled = true;

                int buyGoods = Convert.ToInt32(BuyGoodsTxt.Text);
                int sellGoods = Convert.ToInt32(SellGoodsTxt.Text);

                Base.GoodsConverter(buyGoods, sellGoods);

                BuyGoodsTxt.Text = "0";
                SellGoodsTxt.Text = "0";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private async void HutButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                btn.IsEnabled = false;
                await Task.Delay(gameStepDuration);
                btn.IsEnabled = true;

                int speedAmount = Convert.ToInt32(SpeedUnitTxt.Text);
                int defenceAmount = Convert.ToInt32(DefenceUnitTxt.Text);
                int attackAmount = Convert.ToInt32(AttackUnitTxt.Text);

                Base.ArmyCreation(speedAmount, attackAmount, defenceAmount);

                SpeedUnitTxt.Text = "0";
                DefenceUnitTxt.Text = "0";
                AttackUnitTxt.Text = "0";

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void CheckWinningCombination()
        {
            foreach (var item in enemies.ToList())
            {
                if (item.Active != false)
                {
                    enemiesLeft = true;
                }
            }

            if (destroyed == true)
            {
                gameOver = true;

                Menu menu = new Menu();
                SetImage(Base.Position[0], Base.Position[1], "Image/grass.png");

                MessageBoxResult result = MessageBox.Show("Поражение!. \nВаша база была уничтожена. ",
                                           "Confirmation",
                                           MessageBoxButton.OK,
                                           MessageBoxImage.Exclamation);

                menu.Activate();
                menu.Show();
                this.Close();
            }
            if (enemiesLeft == false && destroyed == false)
            {
                winnedGame = true;
                gameOver = true;
                Menu menu = new Menu();

                MessageBoxResult result = MessageBox.Show("Игра пройдена! \nВы победили. ",
                                           "Confirmation",
                                           MessageBoxButton.OK,
                                           MessageBoxImage.Exclamation);
                menu.Activate();
                menu.Show();
                this.Close();
            }
        }
        public TimeSpan GetGameStepDuration()
        {
            return gameStepDuration;
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            stopwatch.Stop();
            double executionDuration = Math.Round(stopwatch.Elapsed.TotalMinutes, 2);

            if (destroyed == true)
            {
                GameResults result = new GameResults(PlayerName, GameResult.Lose, numberOfEnemies, executionDuration);
                gameResults.Add(result);
            }
            else if (winnedGame == true)
            {
                GameResults result = new GameResults(PlayerName, GameResult.Win, numberOfEnemies, executionDuration);
                gameResults.Add(result);
            }
            else if (winnedGame == false)
            {
                GameResults result = new GameResults(PlayerName, GameResult.InProgress, numberOfEnemies, executionDuration);
                gameResults.Add(result);
            }

            WriteGameResults();

            this.Close();
        }
        private void WriteGameResults()
        {
            using (TextWriter writer = new StreamWriter("PreviousGames.JSON"))
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                writer.Write(serializer.Serialize(gameResults));
            }
        }
    }
}
