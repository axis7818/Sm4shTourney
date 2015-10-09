using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Sm4shTourneyLib.Config
{
    public class RankValue
    {
        public RankValue() { }

        [XmlAttribute()]
        public string Rank { get; set; }

        [XmlAttribute()]
        public int Value { get; set; }

        public RankValue(string rank, int value)
        {
            Rank = rank;
            Value = value;
        }
    }
}
