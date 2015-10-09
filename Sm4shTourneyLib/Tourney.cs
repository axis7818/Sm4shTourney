using System;
using System.Linq;
using System.Collections.Generic;

namespace Sm4shTourneyLib
{
    public class Tourney
    {
        /// <summary>
        /// The number of matches in the tournament
        /// </summary>
        public int NumberOfMatches { get; private set; }

        /// <summary>
        /// The Contestents competing in the tourney
        /// </summary>
        public List<Player> Contenstants { get; private set; }

        /// <summary>
        /// The matches within the game
        /// </summary>
        public List<Game> Matches { get; private set; }

        public int MatchIndex { get; set; }

        public Game CurrentMatch
        {
            get
            {
                return Matches[MatchIndex];
            }
        }

        public Game NextMatch
        {
            get
            {
                try
                {
                    return Matches[MatchIndex + 1];
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public Game PreviousMatch
        {
            get
            {
                try
                {
                    return Matches[MatchIndex - 1];
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// instantiate a new Tourney
        /// </summary>
        /// <param name="contenstants">The Contestents competing in the tourney</param>
        public Tourney(List<Player> contenstants)
        {
            Contenstants = contenstants;

            NumberOfMatches = Contenstants[0].NumberOfSmashers;
            if(!Contenstants.All(player => player.NumberOfSmashers == NumberOfMatches))
            {
                throw new ArgumentException("All contestants must have the same number of smashers.");
            }

            Matches = new List<Game>();
            for(int i = 0; i < NumberOfMatches; i++)
            {
                Matches.Add(new Game(Contenstants));
            }
            MatchIndex = 0;
        }
    }
}
