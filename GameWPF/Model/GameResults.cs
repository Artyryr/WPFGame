using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace GameWPF.Model
{
    public class GameResults
    {
        public string UserName { get; set; }
        public GameResult GameResult { get; set; }
        public int NumberOfBots { get; set; }
        public double GameDuration { get; set; }

        public GameResults(string userName, GameResult result, int numberOfBots, double gameDuration)
        {
            UserName = userName;
            GameResult = result;
            NumberOfBots = numberOfBots;
            GameDuration = gameDuration;
        }
        public GameResults()
        {

        }
    }
    public enum GameResult
    {
        Win = 1,
        Lose = 2,
        InProgress = 3,
        Draw = 4
    }
}
