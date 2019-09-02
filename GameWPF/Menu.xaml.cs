using GameWPF.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
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
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        public List<GameResults> gameResults = new List<GameResults>();
        public Menu()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string playerName = PlayerNameTxt.Text;

                if(playerName == "")
                {
                    playerName = "GeneralPlayer";
                }

                int gs = Convert.ToInt32(GSDurationTxt.Text);
                int enemiesNumber = Convert.ToInt32(EnemiesNumberTxt.Text);
                if (gs >= 1 && gs <= 10 && enemiesNumber >= 3 && enemiesNumber <= 5)
                {
                    MainWindow window = new MainWindow(gs, enemiesNumber, playerName, gameResults);
                    window.Show();
                    this.Close();
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("Количество противников может быть от 3 до 5 \n" +
                        "Длительность игрового шага от 1 до 10",
                                          "Confirmation",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Exclamation);
                }

            }
            catch (Exception ex)
            {
                MessageBoxResult result = MessageBox.Show("Количество противников может быть от 3 до 5 \n" +
                         "Длительность игрового шага от 1 до 10",
                                           "Confirmation",
                                           MessageBoxButton.OK,
                                           MessageBoxImage.Exclamation);
            }
        }
        private void GetResults()
        {
            try
            {
                using (TextReader reader = new StreamReader("PreviousGames.JSON"))
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    gameResults = serializer.Deserialize<List<GameResults>>(reader.ReadLine());
                }
            }
            catch ( Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetResults();
            List<GameResults> currentResult = gameResults;
            currentResult.Reverse();
            resultsGrid.ItemsSource = currentResult;
        }
    }
}
