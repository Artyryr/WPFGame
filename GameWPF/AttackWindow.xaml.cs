using GameWPF.Logic;
using GameWPF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GameWPF
{
    /// <summary>
    /// Interaction logic for AttackWindow.xaml
    /// </summary>
    public partial class AttackWindow : Window
    {
        public Enemy Enemy { get; set; }
        public MainWindow MainWindow { get; set; }
        public string URL { get; set; }
        public object UIHelper { get; set; }
        public int Distance { get; set; }
        public double SpeedOfMovement { get; set; }

        int attackUnits;
        int defenceUnits;
        int speedUnits;

        public AttackWindow()
        {
            InitializeComponent();
        }

        public AttackWindow(Enemy enemy, MainWindow window)
        {
            InitializeComponent();
            Enemy = enemy;
            MainWindow = window;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                attackUnits = Convert.ToInt32(AttackUnitsTxt.Text);
                defenceUnits = Convert.ToInt32(DefenceUnitsTxt.Text);
                speedUnits = Convert.ToInt32(SpeedUnitsTxt.Text);

                if (attackUnits > 0 || defenceUnits > 0 || speedUnits > 0)
                {
                    if (attackUnits <= MainWindow.Base.Army.AttackUnits
                    && defenceUnits <= MainWindow.Base.Army.DefenceUnits && speedUnits <= MainWindow.Base.Army.SpeedUnits)
                    {
                        Army army = new Army(speedUnits, attackUnits, defenceUnits);
                        BattleLogic logic = new BattleLogic(MainWindow.Base, Enemy, army, MainWindow.enemies, MainWindow.GetGameStepDuration(), true, MainWindow, this);
                        logic.WarProcess();
                    }
                    else
                    {
                        MessageBoxResult result = MessageBox.Show("Вы не можете отправить больше юнитове, чем есть в вашей армии.",
                                               "Confirmation",
                                               MessageBoxButton.OK,
                                               MessageBoxImage.Exclamation);
                    }
                    }
                }
            catch (Exception ex)
            {
                MessageBoxResult result = MessageBox.Show("Количество юнитов не может быть меньше 0. \n",
                                           "Confirmation",
                                           MessageBoxButton.OK,
                                           MessageBoxImage.Exclamation);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Speedlbl.Content = "Скорости: " + Convert.ToInt32(MainWindow.Base.Army.SpeedUnits);
            Attacklbl.Content = "Атаки: " + Convert.ToInt32(MainWindow.Base.Army.AttackUnits);
            Defencelbl.Content = "Защиты: " + Convert.ToInt32(MainWindow.Base.Army.DefenceUnits);
        }
    }
}

