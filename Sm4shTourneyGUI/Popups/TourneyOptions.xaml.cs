using System.Windows;
using System.Data;
using System.Collections.Generic;
using Sm4shTourneyLib.Config;
using System.Collections;

namespace Sm4shTourneyGUI.Popups
{
    /// <summary>
    /// Interaction logic for TourneyOptions.xaml
    /// </summary>
    public partial class TourneyOptions : Window
    {
        private TourneyConfig Config;

        private List<RankValue> RankValues
        {
            get
            {
                return Config.RankValues;
            }
            set
            {
                Config.RankValues = value;
            }
        }

        public TourneyOptions(TourneyConfig config)
        {
            Config = config;

            InitializeComponent();
            DG_RankValues.ItemsSource = RankValues;
        }

        private void BTN_AddRankValue_Click(object sender, RoutedEventArgs e)
        {
            RankValues.Add(new RankValue("", 0));
            DG_RankValues.Items.Refresh();
        }

        private void BTN_RemoveRankValue_Click(object sender, RoutedEventArgs e)
        { 
            if(DG_RankValues.SelectedItems.Count > 0)
            {
                RankValue row = DG_RankValues.SelectedItem as RankValue;
                RankValues.Remove(RankValues.Find(rv => rv.Rank == row.Rank));
            }
            else
            {
                int count = RankValues.Count;
                if(count > 0)
                {
                    RankValues.RemoveAt(count - 1);
                }
            }

            DG_RankValues.Items.Refresh();
        }

        private void BTN_Apply_Click(object sender, RoutedEventArgs e)
        {
            Config.WriteToFile();
            MessageBox.Show(this, "Data successfully applied.", "Success!", MessageBoxButton.OK, MessageBoxImage.None);
            this.Close();
        }
    }
}
