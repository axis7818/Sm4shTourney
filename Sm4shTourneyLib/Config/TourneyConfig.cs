using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace Sm4shTourneyLib.Config
{
    public class TourneyConfig
    {
        public TourneyConfig()
        {
        }

        public TourneyConfig(List<RankValue> rankValues)
        {
            RankValues = rankValues;
        }

        [XmlIgnore()]
        public static string ConfigPath
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "TourneyConfig.xml");
            }
        }

        public List<RankValue> RankValues { get; set; }

        public static TourneyConfig ReadFromFile()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TourneyConfig));
                if (!File.Exists(ConfigPath))
                {
                    WriteBlankFile();
                }
                StreamReader reader = new StreamReader(ConfigPath);
                TourneyConfig result = serializer.Deserialize(reader) as TourneyConfig;
                reader.Close();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static void WriteBlankFile()
        {
            TourneyConfig temp = new TourneyConfig(new List<RankValue>());
            temp.WriteToFile();
        }

        public static bool IsWorthMoreThan(string rank1, string rank2)
        {
            TourneyConfig config = TourneyConfig.ReadFromFile();

            try
            {
                RankValue firstOne = config.RankValues.Find(rv => rv.Rank == rank1);
                RankValue secondOne = config.RankValues.Find(rv => rv.Rank == rank2);
                return firstOne.Value > secondOne.Value;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public int? GetValue(string rank)
        {
            try
            {
                return RankValues.Find(rv => rv.Rank == rank).Value;
            }
            catch (Exception)
            {
                return null;
            }            
        }

        public void WriteToFile()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TourneyConfig));
            StreamWriter writer = new StreamWriter(ConfigPath);
            serializer.Serialize(writer, this);
            writer.Close();
        }

        public void AddRankValue(RankValue rankValue)
        {
            if(RankValues.Any(rv => rv.Rank == rankValue.Rank))
            {
                throw new ArgumentException("the rank: " + rankValue.Rank + " already exists within the configuration");
            }
            else
            {
                RankValues.Add(rankValue);
            }
        }
    }
}