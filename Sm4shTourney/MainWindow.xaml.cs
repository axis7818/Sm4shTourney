using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml.Serialization;
using HtmlAgilityPack;

namespace Sm4shTourneyGUI
{
    public partial class MainWindow : Window
    {
        CharacterFileManager FileManager { get; set; }
        List<Character> Smashers { get; set; }

        // events
        private event UpdateTierStartTemplate UpdateTierStart;
        private event UpdateTierEndTemplate UpdateTierEnd;
        private event UpdateTierFailedTemplate UpdateTierFailed;

        // initializers
        public MainWindow()
        {
            InitializeComponent();

            FileManager = new CharacterFileManager();
            TB_Instructions.Text = "Click 'Randomize' to generate a new random list of characters.";

            Smashers = new List<Character>(FileManager.ReadFile());
            if (Smashers == null)
                Smashers = new List<Character>();
            if (Smashers.Count <= 0)
                UpdateCharacters();

            DG_Characters.ItemsSource = Smashers;

            // hook up events
            UpdateTierStart += UpdateTierStartEventHandler;
            UpdateTierEnd += UpdateTierEndEventHandler;
            UpdateTierFailed += UpdateTierFailedEventHandler;
        }

        // private functions
        #region UpdateCharactersAccordingToTier
        private void UpdateCharacters()
        {
            Thread updateThread = new Thread(new ThreadStart(delegate()
            {
                try
                {
                    // start update
                    if (UpdateTierStart != null) UpdateTierStart();

                    // get data
                    HtmlWeb source = new HtmlWeb();
                    HtmlDocument htmlDoc = source.Load("http://www.eventhubs.com/tiers/ssb4/");
                    HtmlNode tierTable = (htmlDoc.DocumentNode.Descendants("table").Where(x => x.Attributes["id"] != null && x.Attributes["id"].Value.Contains("tiers1")).ToList())[0];
                    HtmlNode tableBody = tierTable.Descendants("tbody").ToList()[0];
                    List<HtmlNode> rows = tableBody.Descendants("tr").ToList();
                    List<string> names = new List<string>();
                    foreach (HtmlNode row in rows)
                    {
                        string name = row.Descendants("a").ToList()[0].InnerText;
                        names.Add(name);
                    }

                    // end update
                    if (UpdateTierEnd != null) UpdateTierEnd(names);
                }
                catch (Exception e)
                {
                    // update failed
                    if (UpdateTierFailed != null) UpdateTierFailed(e.Message);
                }
            }));
            updateThread.Start();

            //updateThread.Join();            
        }
        private void UpdateTierStartEventHandler()
        {
            this.Dispatcher.BeginInvoke((Action)(() => 
            {
                LoadingFilm.Visibility = Visibility.Visible;
            }));
        }
        private void UpdateTierEndEventHandler(List<string> names)
        {
            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                Smashers.Clear();
                for (int i = 0; i < names.Count; i++)
                {
                    Character smasher = new Character(names[i]) { TierPosition = i + 1 };
                    Smashers.Add(smasher);
                }

                FileManager.WriteFile(Smashers);
                DG_Characters.Items.Refresh();

                LoadingFilm.Visibility = Visibility.Hidden;
            }));
        }
        private void UpdateTierFailedEventHandler(string message)
        {
            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                LoadingFilm.Visibility = Visibility.Hidden;
                MessageBox.Show(this, "Error updating: " + message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }));
        }
        #endregion

        // page interactions
        private void BTN_Randomize_Click(object sender, RoutedEventArgs e)
        {
            // create a new list to store the result
            List<Character> result = new List<Character>();

            // make an instance of the Random class
            Random rand = new Random();

            // randomly take one character out of Smashers and put it into result
            int count = Smashers.Count;
            for (int i = 0; i < count; i++)
            {
                // get a random index
                int index = rand.Next(Smashers.Count);

                // move element into result
                result.Add(Smashers[index]);
                Smashers.RemoveAt(index);
            }

            // restore the data into Smashers
            Smashers = result;
            DG_Characters.ItemsSource = Smashers;
        }
        private void MI_AddRemoveCharacter_Click(object sender, RoutedEventArgs e)
        {
            CharacterRosterEditor cre = new CharacterRosterEditor(Smashers);
            cre.Owner = this;
            cre.ShowDialog();
            FileManager.WriteFile(Smashers);
            DG_Characters.Items.Refresh();
        }
        private void MI_Print_Click(object sender, RoutedEventArgs e)
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
        private void MI_UpdateCharacters_Click(object sender, RoutedEventArgs e)
        {
            UpdateCharacters();
            FileManager.WriteFile(Smashers);
            DG_Characters.Items.Refresh();
        }
    }

    public delegate void UpdateTierStartTemplate();
    public delegate void UpdateTierEndTemplate(List<string> names);
    public delegate void UpdateTierFailedTemplate(string message);

    public class Character
    {
        [XmlAttribute()]
        public string Name { get; set; }
        [XmlAttribute()]
        public int TierPosition { get; set; }


        public Character() { }
        public Character(string name)
        {
            Name = name;
        }
    }
}
