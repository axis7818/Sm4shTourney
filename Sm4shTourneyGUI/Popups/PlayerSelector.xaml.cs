using Sm4shTourneyLib;
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

namespace Sm4shTourneyGUI.Popups
{
    /// <summary>
    /// Interaction logic for PlayerSelector.xaml
    /// </summary>
    public partial class PlayerSelector : Window
    {
        private List<Player> Players;

        public Player SelectedPlayer = null;

        public PlayerSelector(List<Player> players)
        {
            InitializeComponent();

            Players = players;
            CB_PlayerSelector.ItemsSource = Players.Select(p => p.Name).ToList();
        }

        private void BTN_Select_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CB_PlayerSelector.Text))
            {
                MessageBox.Show(this, "You must select a player!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                SelectedPlayer = Players.Find(p => p.Name == CB_PlayerSelector.Text);
                DialogResult = true;
                Close();
            }
        }
    }
}
