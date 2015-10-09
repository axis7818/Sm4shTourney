using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace Sm4shTourneyLib
{
    public class Character
    {
        [XmlAttribute()]
        public string Name { get; set; }

        [XmlAttribute()]
        public int TierPosition { get; set; }

        public Character()
        {
        }

        public Character(string name)
        {
            Name = name;
        }

        [XmlIgnore()]
        public string ImageLocation
        {
            get
            {
                string result = @"http://media.eventhubs.com/images/characters/ssb4/";
                result += GetImageFileName();
                return result;
            }
        }

        private string GetImageFileName()
        {
            Regex r = new Regex(@"[\w\-]+");
            MatchCollection matches = r.Matches(Name.ToLower());

            string result = "";
            foreach(Match match in matches)
            {
                result += match.Value.ToString();
                result += "_";
            }
            result = result.TrimEnd('_');

            result += ".png";

            return result;
        }
    }
}