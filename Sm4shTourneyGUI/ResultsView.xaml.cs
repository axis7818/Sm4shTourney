using Sm4shTourneyLib;
using Sm4shTourneyLib.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Sm4shTourneyGUI
{
    /// <summary>
    /// Interaction logic for ResultsView.xaml
    /// </summary>
    public partial class ResultsView : Window
    {
        Tourney Tourney;
        TourneyConfig Config;

        Dictionary<Player, int> PlayerScores;

        bool NotAllGamesFinished = false;

        public ResultsView(Tourney tourney, TourneyConfig config)
        {
            InitializeComponent();

            Tourney = tourney;
            Config = config;

            PlayerScores = new Dictionary<Player, int>();

            GetPlayerScores();

            DisplayResults();
        }

        private void DisplayResults()
        {
            foreach(Player player in PlayerScores.Keys)
            {
                PlayerCell cell = new PlayerCell(player);
                SP_PlayerScores.Children.Add(cell);
                cell.SwitchOnResults(PlayerScores[player]);
            }


            if (NotAllGamesFinished)
            {
                MessageBox.Show("Not all games were finished... Results may not be accurate.", "Not All Games Finished", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void GetPlayerScores()
        {
            foreach(Player player in Tourney.Contenstants)
            {
                PlayerScores[player] = GetScore(player);
            }
        }

        private int GetScore(Player player)
        {
            int score = 0; 

            foreach(Game game in Tourney.Matches)
            {
                if(game.GameState != GameState.Finished)
                {
                    NotAllGamesFinished = true;
                }

                int? delta = Config.GetValue(game.GetRankForPlayer(player));
                score += delta ?? 0;
            }

            return score;
        }
    }
}
