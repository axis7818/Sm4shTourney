using Sm4shTourneyLib;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Sm4shTourneyGUI.Popups
{
    public partial class CharacterRosterEditor : Window
    {
        private List<Character> Characters { get; set; }

        // initialize the page
        public CharacterRosterEditor(List<Character> chars)
        {
            InitializeComponent();
            Characters = chars;
            PopulateFields();
        }

        private void PopulateFields()
        {
            ClearFields();

            // fill the dropdown
            foreach (Character c in Characters.OrderBy(c => c.TierPosition).ToList())
            {
                CB_CharacterSelector.Items.Add(c.Name);
            }
        }

        private void ClearFields()
        {
            // clear the textbox
            TB_NewName.Text = null;

            // clear the dropdown
            CB_CharacterSelector.Items.Clear();
        }

        // page interactions
        private void BTN_AddCharacter_Click(object sender, RoutedEventArgs e)
        {
            // get the new name
            string newName = TB_NewName.Text;

            // check for valid name
            if (string.IsNullOrEmpty(newName))
            {
                MessageBox.Show("You must enter a character name.");
                return;
            }
            // check to see if the character exists already
            if (Characters.Find(c => BasicString(c.Name) == BasicString(newName)) != null)
            {
                MessageBox.Show("This character has already been added.");
                return;
            }

            Characters.Add(new Character(newName));
            PopulateFields();
            MessageBox.Show(newName + " joins the battle!");
        }

        private void BTN_RemoveCharacter_Click(object sender, RoutedEventArgs e)
        {
            // check to see if delete is valid
            if (string.IsNullOrEmpty(CB_CharacterSelector.Text))
            {
                MessageBox.Show("You must select a character to delete.");
                return;
            }

            try
            {
                // get the name of the character to be removed
                string doomed = CB_CharacterSelector.Text;
                Characters.Remove(Characters.Find(c => BasicString(c.Name) == BasicString(doomed)));
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error removing the character: " + ex.Message);
            }

            PopulateFields();
        }

        // page events
        private void TB_NewName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BTN_AddCharacter_Click(this, new RoutedEventArgs());
            }
        }

        // reduces a string to its lowercase form and removes special characters
        private string BasicString(string str)
        {
            if (str != null)
            {
                // define the strings to be removed
                string[] toBlank = { " ", ".", "/", "_" };
                foreach (string s in toBlank)
                {
                    str = str.Replace(s, string.Empty);
                }
                str = str.ToLower();
                return str;
            }
            else return null;
        }
    }
}