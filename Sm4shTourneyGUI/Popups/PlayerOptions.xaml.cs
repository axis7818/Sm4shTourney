using System.Collections.Generic;
using System.Linq;
using Sm4shTourneyLib;
using System.Windows;
using System;

namespace Sm4shTourneyGUI.Popups
{
    /// <summary>
    /// Interaction logic for PlayerOptions.xaml
    /// </summary>
    public partial class PlayerOptions : Window
    {
        //TODO: add a color option for the players

        public Player Player { get; private set; }

        private List<string> smashers;

        public PlayerOptions(List<Character> smasherList)
        {
            InitializeComponent();

            smashers = smasherList.Select(c => c.Name).ToList();
            CB_SuddenDeathSmasher.ItemsSource = smashers;
        }

        private void BTN_Submit_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateData())
            {
                Player newPlayer = new Player(TB_Name.Text);
                newPlayer.SuddenDeathSmasherName = CB_SuddenDeathSmasher.Text;

                //newPlayer.WriteToFile();
                Player = newPlayer;

                DialogResult = true;
                Close();
            }            
        }

        private bool ValidateData()
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrWhiteSpace(TB_Name.Text))
                errors.Add("Enter a name.");
            if (string.IsNullOrWhiteSpace(CB_SuddenDeathSmasher.Text))
                errors.Add("Select a Sudden Death Smasher.");

            if(errors.Count > 0)
            {
                string message = "Pleas fix the following errors:";
                foreach(string s in errors)
                {
                    message += "\n\t- " + s;
                }

                MessageBox.Show(this, message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void BTN_ExistingPlayer_Click(object sender, RoutedEventArgs e)
        {
            PlayerSelector selector = new PlayerSelector(Player.ReadAllPlayersFromFiles());
            selector.Owner = this;
            selector.ShowDialog();

            if(selector.DialogResult == true)
            {
                FillData(selector.SelectedPlayer);
            }
        }

        private void FillData(Player player)
        {
            try
            {
                TB_Name.Text = player.Name;
                CB_SuddenDeathSmasher.SelectedIndex = CB_SuddenDeathSmasher.Items.IndexOf(player.SuddenDeathSmasherName);
            }
            catch(Exception e)
            {
                MessageBox.Show(this, "There was an error filling the data: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
