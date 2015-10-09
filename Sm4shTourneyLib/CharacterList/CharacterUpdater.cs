using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sm4shTourneyLib.CharacterList
{
    public class CharacterUpdater
    {
        // events
        public event Action UpdateTierStart;

        public event Action UpdateTierEnd;

        public event UpdateTierFailedTemplate UpdateTierFailed;

        public List<Character> GetOrderedCharacters()
        {
            try
            {
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

                // build the result
                List<Character> result = new List<Character>();
                for (int i = 0; i < names.Count; i++)
                {
                    Character character = new Character(names[i]) { TierPosition = i + 1 };
                    result.Add(character);
                }

                if (UpdateTierEnd != null) UpdateTierEnd();

                return result;
            }
            catch (Exception e)
            {
                if (UpdateTierFailed != null) UpdateTierFailed(e.Message);
                return null;
            }
        }
    }

    public delegate void UpdateTierFailedTemplate(string message);
}