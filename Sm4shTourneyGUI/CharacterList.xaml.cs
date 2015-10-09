using Sm4shTourneyGUI.Popups;
using Sm4shTourneyLib;
using Sm4shTourneyLib.CharacterList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Sm4shTourneyGUI
{
    public partial class CharacterList : UserControl
    {
        private CharacterFileManager FileManager { get; set; }

        public List<Character> Smashers { get; private set; }

        public CharacterList()
        {
            InitializeComponent();

            FileManager = new CharacterFileManager();

            Smashers = new List<Character>(CharacterFileManager.ReadFile(true));
            if (Smashers == null)
            {
                Smashers = new List<Character>();
            }
            if (Smashers.Count <= 0)
            {
                UpdateCharacters();
            }

            DG_Characters.ItemsSource = Smashers;
        }

        private async void UpdateCharacters()
        {
            EnableLoadingFilm();
            List<Character> result = new List<Character>();
            bool succeed = false;

            CharacterUpdater updater = new CharacterUpdater();
            updater.UpdateTierFailed += Updater_UpdateTierFailed;
            updater.UpdateTierEnd += () => { succeed = true; };

            result = await Task.Run(() => updater.GetOrderedCharacters());

            if (succeed)
            {
                Smashers = result;
                FileManager.WriteFile(Smashers);
                DG_Characters.ItemsSource = Smashers;
            }

            DisableLoadingFilm();
        }

        private void Updater_UpdateTierFailed(string message)
        {
            ShowError("There was an error updating the characters", message);
        }

        public void EnableLoadingFilm()
        {
            LoadingFilm.Visibility = Visibility.Visible;
        }

        public void DisableLoadingFilm()
        {
            LoadingFilm.Visibility = Visibility.Collapsed;
        }

        private void ShowError(string context, string error)
        {
            MessageBox.Show(context + "\n\n", error, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void BTN_Randomize_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            List<Character> result = new List<Character>();

            Random rand = new Random();

            int count = Smashers.Count;
            for (int i = 0; i < count; i++)
            {
                int index = rand.Next(Smashers.Count);
                result.Add(Smashers[index]);
                Smashers.RemoveAt(index);
            }

            Smashers = result;
            DG_Characters.ItemsSource = Smashers;
        }

        private void BTN_Update_Click(object sender, RoutedEventArgs e)
        {
            UpdateCharacters();
        }

        private void BTN_Print_Click(object sender, RoutedEventArgs e)
        {
            // write the text file to storage
            FileManager.WriteTextFile(Smashers);

            // show the dialog
            PrintDialog printDialog = new PrintDialog();
            if ((bool)printDialog.ShowDialog())
            {
                try
                {
                    // generate the data
                    TextReader reader = new StreamReader(CharacterFileManager.TxtFilePath);
                    string[] names = reader.ReadToEnd().Split('\n');
                    reader.Close();

                    // build the document
                    FlowDocument doc = new FlowDocument();
                    int lineCount = 1;
                    Paragraph paragraph = new Paragraph();
                    paragraph.Margin = new Thickness(50);
                    paragraph.LineHeight = 15;
                    foreach (string s in names)
                    {
                        string lineText = lineCount.ToString() + ") " + s + "\n";
                        lineCount++;
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            paragraph.Inlines.Add(new Run(lineText));
                        }
                    }
                    doc.Blocks.Add(paragraph);

                    DocumentPaginator paginator = ((IDocumentPaginatorSource)doc).DocumentPaginator;
                    printDialog.PrintDocument(paginator, "Sm4sh Character List");
                }
                catch (Exception ex) { MessageBox.Show("There was an error printing the file: " + ex.Message); }
            }
        }

        private void BTN_AddRemove_Click(object sender, RoutedEventArgs e)
        {
            CharacterRosterEditor cre = new CharacterRosterEditor(Smashers);
            cre.Owner = Application.Current.MainWindow;
            cre.ShowDialog();

            FileManager.WriteFile(Smashers);
            Smashers = CharacterFileManager.ReadFile();
            DG_Characters.ItemsSource = Smashers;
        }
    }
}