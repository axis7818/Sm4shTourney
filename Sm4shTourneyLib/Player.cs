using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace Sm4shTourneyLib
{
    /// <summary>
    /// A player within a tournament
    /// </summary>
    public class Player : INotifyPropertyChanged
    {
        /// <summary>
        /// the R value of the color for the player
        /// </summary>
        [XmlAttribute()]
        public byte ColorR { get; set; }

        /// <summary>
        /// The G value of the color for the player
        /// </summary>
        [XmlAttribute()]
        public byte ColorG { get; set; }

        /// <summary>
        /// The B value of the color for the player
        /// </summary>
        [XmlAttribute()]
        public byte ColorB { get; set; }

        /// <summary>
        /// The name of the player
        /// </summary>
        private string _name;

        /// <summary>
        /// The order of the characters for the player to use in the tourney
        /// </summary>
        [XmlIgnore()]
        public List<Character> Smashers;

        /// <summary>
        /// The name of the Smasher that this player uses for sudden deaths 
        /// </summary>
        [XmlAttribute(AttributeName = "SuddenDeathSmasher")]
        public string SuddenDeathSmasherName;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Gets the sudden death smasher as a Character Object
        /// </summary>
        [XmlIgnore()]
        public Character SuddenDeathSmasher
        {
            get
            {
                return Smashers.Find(character => character.Name == SuddenDeathSmasherName);
            }
            set
            {
                SuddenDeathSmasherName = value.Name;
            }
        }

        /// <summary>
        /// instantiates a Player
        /// </summary>
        public Player()
        {
            Name = "NONAME";
            Smashers = new List<Character>();
        }

        /// <summary>
        /// instantiates a Player
        /// </summary>
        /// <param name="name">The name of the player</param>
        /// <param name="smashers">the list of smashers</param>
        public Player(string name, List<Character> smashers)
        {
            Name = name;
            Smashers = smashers;
        }

        /// <summary>
        /// instantiates a Player
        /// </summary>
        /// <param name="name">The name of the player</param>
        public Player(string name)
        {
            Name = name;
            Smashers = new List<Character>();
        }

        [XmlIgnore()]
        public string FileLocation
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Players/" + Name + ".xml");
            }
        }

        /// <summary>
        /// The name of the player
        /// </summary>
        [XmlAttribute(AttributeName = "Name")]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// The number of smashers for this player
        /// </summary>
        [XmlIgnore()]
        public int NumberOfSmashers
        {
            get
            {
                return Smashers.Count;
            }
        }

        public static Player ReadFromFile(string name)
        {
            string fileLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Players/" + name + ".xml");

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Player));
                StreamReader reader = new StreamReader(fileLocation);
                Player result = serializer.Deserialize(reader) as Player;
                reader.Close();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// reads all available player files and returns them as a List of Players
        /// </summary>
        /// <returns>A List of Players read from thier .xml files</returns>
        public static List<Player> ReadAllPlayersFromFiles()
        {
            List<Player> result = new List<Player>();

            try
            {
                string directory = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Players");

                foreach (string file in Directory.EnumerateFiles(directory))
                {
                    string name = Path.GetFileNameWithoutExtension(file);
                    Player player = ReadFromFile(name);
                    result.Add(player);
                }
            }
            catch (Exception) { }
            
            return result;
        }

        /// <summary>
        /// Gets the character that this Player should use for the specified game
        /// </summary>
        /// <param name="gameNumber">the number of the game that is being played in the tournament</param>
        /// <returns>the character that this player is using for the specified game</returns>
        public Character CharacterForGameNumber(int gameNumber)
        {
            if (gameNumber < 0 || gameNumber >= Smashers.Count)
            {
                throw new ArgumentException("the gameNumber is out of range", "gameNumber");
            }

            return Smashers[gameNumber];
        }

        public void WriteToFile()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Player));

            string directory = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Players");
            if(!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            StreamWriter writer = new StreamWriter(FileLocation);
            serializer.Serialize(writer, this);
            writer.Close();
        }
    }
}