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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Sm4shTourneyLib;
using Sm4shTourneyLib.Config;

namespace Sm4shTourneyGUI
{
    /// <summary>
    /// Interaction logic for GameView.xaml
    /// </summary>
    public partial class GameView : UserControl
    {
        public int GameNumber { get; private set; }

        public int GamesRemaining { get; private set; }

        private Game _currentGame;

        public Game CurrentGame
        {
            get
            {
                return _currentGame;
            }
            set
            {
                _currentGame = value;
                _currentGame.GameStateChanged += OnGameStateChanged;
            }
        }

        public void Populate(int gameNumber, int gamesRemaining, Game game)
        {
            CurrentGame = game;
            if(CurrentGame.GameState != GameState.Finished)
                CurrentGame.GameState = GameState.InProgress;   
            GameNumber = gameNumber;
            GamesRemaining = gamesRemaining;
            TB_GameNumber.Text = (gameNumber + 1).ToString();
            TB_RemainingGames.Text = (gamesRemaining - 1).ToString();

            BTN_Previous.IsEnabled = gameNumber > 0;
            BTN_Next.Visibility = gamesRemaining > 1 ? Visibility.Visible : Visibility.Collapsed;
            BTN_GoToResults.Visibility = gamesRemaining > 1 ? Visibility.Collapsed : Visibility.Visible;

            PopulateResults();
        }

        private void PopulateResults()
        {
            if(CurrentGame.GameState == GameState.Finished)
            {
                TB_Winner.Visibility = Visibility.Visible;
                TB_Losers.Visibility = Visibility.Visible;
                IMG_Winner.Visibility = Visibility.Visible;
                SP_Losers.Visibility = Visibility.Visible;

                try
                {
                    IMG_Winner.Source = new BitmapImage(new Uri(CurrentGame.GetWinner().CharacterForGameNumber(GameNumber).ImageLocation));

                    SP_Losers.Children.Clear();
                    foreach(Player player in CurrentGame.GetLosers())
                    {
                        SP_Losers.Children.Add(new Image()
                        {
                            Source = new BitmapImage(new Uri(player.CharacterForGameNumber(GameNumber).ImageLocation))
                        });

                    }
                }
                catch (Exception) { }
            }
            else
            {
                TB_Winner.Visibility = Visibility.Hidden;
                TB_Losers.Visibility = Visibility.Hidden;
                IMG_Winner.Visibility = Visibility.Hidden;
                SP_Losers.Visibility = Visibility.Hidden;
            }
        }

        private void OnGameStateChanged(object sender, GameStateChangedEventArgs args)
        {
            PopulateResults();
        }

        public GameView()
        {
            InitializeComponent();
        }

        public event EventHandler<MatchChangingEventArgs> MatchChanging;

        private void OnMatchChanging(int newIndex, int oldIndex)
        {
            if (MatchChanging != null)
            {
                MatchChanging(this, new MatchChangingEventArgs(newIndex, oldIndex));
            }
        }

        private void BTN_Previous_Click(object sender, RoutedEventArgs e)
        {
            Populate(GameNumber - 1, GamesRemaining + 1, TourneyView.Tourney.PreviousMatch);
            OnMatchChanging(GameNumber, GameNumber + 1);
            PopulateResults();
        }

        private void BTN_Next_Click(object sender, RoutedEventArgs e)
        {
            if(CurrentGame.GameState != GameState.Finished)
            {
                MessageBox.Show("You must submit the results for this match before continuing", "Submit Results", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                Populate(GameNumber + 1, GamesRemaining - 1, TourneyView.Tourney.NextMatch);
                OnMatchChanging(GameNumber, GameNumber - 1);
            }            
        }

        private void BTN_GoToResults_Click(object sender, RoutedEventArgs e)
        {
            if(CurrentGame.GameState != GameState.Finished)
            {
                MessageBox.Show("You must submit the results for this match before continuing", "Submit Results", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                ResultsView results = new ResultsView(TourneyView.Tourney, TourneyConfig.ReadFromFile());
                results.Owner = Application.Current.MainWindow;
                results.Show();
            }
        }
    }

    public class MatchChangingEventArgs : EventArgs
    {
        public int NewIndex;
        public int OldIndex;

        public MatchChangingEventArgs(int newIndex, int oldIndex)
        {
            NewIndex = newIndex;
            OldIndex = oldIndex;
        }
    }
}
