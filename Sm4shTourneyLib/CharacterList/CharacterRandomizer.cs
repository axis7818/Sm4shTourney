using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sm4shTourneyLib.CharacterList
{
    public static class CharacterRandomizer
    {
        public static List<Character> Randomize(List<Character> smashers)
        {
            List<string> names = smashers.Select(c => c.Name).ToList();
            List<Character> result = new List<Character>();

            Random rand = new Random(DateTime.Now.Millisecond);

            int count = smashers.Count;
            for(int i = 0; i < count; i++)
            {
                int index = rand.Next(names.Count);
                result.Add(smashers.Find(c => c.Name == names[index]));
                names.RemoveAt(index);
            }

            return result;
        }
    }
}
