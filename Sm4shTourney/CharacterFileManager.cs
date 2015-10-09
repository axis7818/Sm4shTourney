using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Sm4shTourneyGUI
{
    public class CharacterFileManager
    {
        public static string XmlFilePath { get { return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Characters.xml"); } }
        public static string TxtFilePath { get { return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Characters.txt"); } }
        XmlSerializer Serializer { get; set; }


        public CharacterFileManager()
        {
            Serializer = new XmlSerializer(typeof(List<Character>));
        }       


        // read/write the file
        public List<Character> ReadFile()
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
            

            return result;
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
