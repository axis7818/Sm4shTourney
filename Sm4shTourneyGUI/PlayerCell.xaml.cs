using Sm4shTourneyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Sm4shTourneyGUI
{
    /// <summary>
    /// Interaction logic for PlayerCell.xaml
    /// </summary>
    public partial class PlayerCell : UserControl
    {
        private PlayerCellWrapper Data;

        /// <summary>
        /// initialize the Cell
        /// </summary>
        public PlayerCell(Player player)
        {
            InitializeComponent();
            Data = new PlayerCellWrapper(player);

            //try
            //{
            //    Color fill = Color.FromArgb(255, player.ColorR, player.ColorG, player.ColorB);
            //    R_Background.Fill = new SolidColorBrush(fill);
            //}
            //catch (Exception) { }
            

            CB_PlaceSelector.ItemsSource = PlacementOptions;
            try
            {
                OnPlayerChanged();
                OnSmasherChanged();
            }
            catch (Exception e)
            {
                MessageBox.Show("There was an error creating the PlayerCell: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public List<string> PlacementOptions
        {
            get
            {
                return TourneyView.Config.RankValues.Select(x => x.Rank).ToList();
            }
        }

        public Player GetPlayer()
        {
            return Data.Player;
        }

        public string PlayerMatchRank
        {
            get
            {
                return CB_PlaceSelector.Text;
            }
        }

        public Player Player
        {
            set
            {
                Data.Player = value;
                OnPlayerChanged();
            }
        }

        public string PlayerName
        {
            get
            {
                return Data.Player.Name;
            }
        }

        public Character Smasher
        {
            set
            {
                Data.Smasher = value;
                OnSmasherChanged();
            }
        }

        public string SmasherImageURL
        {
            get
            {
                return Data.Smasher.ImageLocation;
            }
        }

        public string SmasherName
        {
            get
            {
                return Data.Smasher.Name;
            }
        }

        public string SmasherRank
        {
            get
            {
                return "#" + Data.Smasher.TierPosition.ToString();
            }
        }

        private void OnPlayerChanged()
        {
            TB_NameText.Text = PlayerName;
        }

        private void OnSmasherChanged()
        {
            TB_CurrentSmasher.Text = SmasherName;
            TB_Ranking.Text = SmasherRank;
            IMG_Smasher.Source = new BitmapImage(new Uri(SmasherImageURL));
        }

        public void OnStartTourney()
        {
            CB_PlaceSelector.IsEnabled = true;
            Refresh();
        }

        public void Refresh()
        {
            CB_PlaceSelector.Items.Refresh();
            CB_PlaceSelector.SelectedIndex = -1;            
        }

        public void OnQuitTourney()
        {
            CB_PlaceSelector.IsEnabled = false;
            Refresh();
        }

        public int? GetPoints()
        {
            return TourneyView.Config.GetValue(CB_PlaceSelector.Text);
        }

        public void SwitchOnResults(int score)
        {
            G_Results.Visibility = Visibility.Visible;
            TB_Score.Text = score.ToString();

            CB_PlaceSelector.Visibility = Visibility.Collapsed;
        }

        public void SwitchOnNewChallengerShroud()
        {
            G_NewChallengerShroud.Visibility = Visibility.Visible;
        }

        public void SwitchOffNewChallengerShroud()
        {
            G_NewChallengerShroud.Visibility = Visibility.Collapsed;
        }

        public int FinalScore
        {
            get
            {
                try
                {
                    return int.Parse(TB_Score.Text);
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public void SwitchOffResults()
        {
            G_Results.Visibility = Visibility.Collapsed;
            TB_Score.Text = "##";

            CB_PlaceSelector.Visibility = Visibility.Visible;
        }
    }

    internal class PlayerCellWrapper
    {
        public PlayerCellWrapper(Player player)
        {
            Player = player;
            Smasher = Player.SuddenDeathSmasher;
        }

        public Player Player { get; set; }

        public Character Smasher { get; set; }
    }
}