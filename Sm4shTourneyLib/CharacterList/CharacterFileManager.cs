using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace Sm4shTourneyLib.CharacterList
{
    public class CharacterFileManager
    {
        public static string XmlFilePath { get { return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Characters.xml"); } }
        public static string TxtFilePath { get { return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Characters.txt"); } }
        private static XmlSerializer Serializer = new XmlSerializer(typeof(List<Character>));

        public CharacterFileManager()
        {

        }

        // read/write the file
        public static List<Character> ReadFile(bool SortByTier = false)
        {
            List<Character> result = null;

            // make the file if it doesn't exist
            if (!File.Exists(XmlFilePath))
            {
                TextWriter writer = new StreamWriter(XmlFilePath);
                Serializer.Serialize(writer, new List<Character>());
                writer.Close();
            }

            // read the file
            TextReader reader = new StreamReader(XmlFilePath);
            result = Serializer.Deserialize(reader) as List<Character>;
            reader.Close();


            return SortByTier ? result.OrderBy(c => c.TierPosition).ToList() : result;
        }

        public void WriteFile(List<Character> characters)
        {
            // write the file
            TextWriter writer = new StreamWriter(XmlFilePath);
            Serializer.Serialize(writer, characters);
            writer.Close();
        }

        public void WriteTextFile(List<Character> chars)
        {
            TextWriter writer = new StreamWriter(TxtFilePath);
            foreach (Character c in chars)
            {
                writer.WriteLine("[" + c.TierPosition.ToString() + "] " + c.Name);
            }
            writer.Close();
        }
    }
}