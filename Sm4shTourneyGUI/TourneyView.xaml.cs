using System.Windows;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Controls;
using Sm4shTourneyGUI.Popups;
using Sm4shTourneyLib.Config;
using Sm4shTourneyLib.CharacterList;
using Sm4shTourneyLib;
using System.Collections.ObjectModel;

namespace Sm4shTourneyGUI
{
    /// <summary>
    /// Interaction logic for TourneyStartSplash.xaml
    /// </summary>
    public partial class TourneyView : UserControl
    {
        public static TourneyConfig Config { get; private set; }

        public static Tourney Tourney { get; private set; }

        /// <summary>
        /// The contestants in the tournament
        /// </summary>
        public List<PlayerCell> Contestants
        {
            get
            {
                List<PlayerCell> result = new List<PlayerCell>();
                foreach(var item in SP_Contestants.Children)
                {
                    result.Add(item as PlayerCell);
                }
                return result;
            }
        }

        public TourneyView()
        {
            InitializeComponent();

            Config = TourneyConfig.ReadFromFile();

            GV_GameView.MatchChanging += OnMatchChanging;
        }

        private void OnMatchChanging(object sender, MatchChangingEventArgs args)
        {
            PopulatePlayerView();
            Tourney.MatchIndex = args.NewIndex;
        }

        private void BTN_AddPlayer_Click(object sender, RoutedEventArgs e)
        {
            PlayerOptions playerOptions = new PlayerOptions(CharacterFileManager.ReadFile(true));
            playerOptions.Owner = Application.Current.MainWindow;
            playerOptions.ShowDialog();

            if(playerOptions.DialogResult == true)
            {
                Player newPlayer = playerOptions.Player;
                if(!Contestants.Any(pc => pc.PlayerName == newPlayer.Name))
                {
                    newPlayer.Smashers = CharacterRandomizer.Randomize(CharacterFileManager.ReadFile());

                    PlayerCell newCell = new PlayerCell(newPlayer);
                    SP_Contestants.Children.Add(newCell);
                }
                else
                {
                    MessageBox.Show("There is already a player with this name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BTN_Options_Click(object sender, RoutedEventArgs e)
        {
            TourneyOptions options = new TourneyOptions(Config);
            options.Owner = Application.Current.MainWindow;
            options.ShowDialog();
        }

        private void PopulateGameView()
        {
            int gameNum = Tourney.MatchIndex;
            int remaining = Tourney.NumberOfMatches- gameNum;
            GV_GameView.Populate(gameNum, remaining, Tourney.CurrentMatch);
        }

        private void PopulatePlayerView()
        {
            foreach(PlayerCell pc in Contestants)
            {
                pc.Smasher = pc.GetPlayer().CharacterForGameNumber(GV_GameView.GameNumber);
                pc.Refresh();
            }
        }

        private void BTN_StartTourney_Click(object sender, RoutedEventArgs e)
        {
            if(Contestants.Count <= 1)
            {
                MessageBox.Show("Please Select the contestants for the tourney.", "Select Contestants", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // get the contestants 
                List<Player> contestants = new List<Player>();
                foreach (PlayerCell pc in Contestants)
                {
                    contestants.Add(pc.GetPlayer());
                }

                // set up the tourney
                Tourney = new Tourney(contestants);

                //set the GUI
                SetGameScreen();
                PopulateGameView();
                PopulatePlayerView();
                foreach(PlayerCell pc in Contestants)
                {
                    pc.SwitchOffNewChallengerShroud();
                }
            }            
        }

        private void BTN_QuitTourney_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to quit the tournament?", "Quit Tournament", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

            if(result == MessageBoxResult.Yes)
            {
                SetStartScreen();
            }
        }

        private void SetStartScreen()
        {
            BTN_AddPlayer.Visibility = Visibility.Visible;
            BTN_Options.Visibility = Visibility.Visible;
            BTN_StartTourney.Visibility = Visibility.Visible;
            BTN_QuitTourney.Visibility = Visibility.Collapsed;
            BTN_SubmitGame.Visibility = Visibility.Collapsed;

            IMG_StartSplash.Visibility = Visibility.Visible;
            GV_GameView.Visibility = Visibility.Collapsed;

            SP_Contestants.Children.Clear();
        }

        private void SetGameScreen()
        {
            BTN_AddPlayer.Visibility = Visibility.Collapsed;
            BTN_Options.Visibility = Visibility.Collapsed;
            BTN_StartTourney.Visibility = Visibility.Collapsed;
            BTN_QuitTourney.Visibility = Visibility.Visible;
            BTN_SubmitGame.Visibility = Visibility.Visible;

            IMG_StartSplash.Visibility = Visibility.Collapsed;
            GV_GameView.Visibility = Visibility.Visible;

            foreach (PlayerCell player in Contestants)
            {
                player.OnStartTourney();
            }
        }

        private void BTN_SubmitGame_Click(object sender, RoutedEventArgs e)
        {
            if (VerifyGameResults())
            {
                Game game = Tourney.CurrentMatch;
                foreach(PlayerCell pc in Contestants)
                {
                    game.GiveRankToPlayer(pc.PlayerMatchRank, pc.GetPlayer());
                }
                game.GameState = GameState.Finished;
            }
        }

        private bool VerifyGameResults()
        {
            List<string> errors = new List<string>();

            if(Contestants.Any(x => x.GetPoints() == null))
            {
                errors.Add("All players must have a placement selected.");
            }

            if(errors.Count > 0)
            {
                string message = "Please fix the following:";

                foreach(string s in errors)
                {
                    message += "\n\t- " + s;
                }

                MessageBox.Show(message, "Can't submit Game!", MessageBoxButton.OK, MessageBoxImage.Information);

                return false;
            }
            return true;
        }

        private void BTN_HelpButton_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow help = new HelpWindow();
            help.Owner = Application.Current.MainWindow;
            help.ShowDialog();
        }
    }
}
