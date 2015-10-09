using System.Linq;
using System.Collections.Generic;
using System;
using Sm4shTourneyLib.Config;

namespace Sm4shTourneyLib
{
    /// <summary>
    /// A single match in a tourney
    /// </summary>
    public class Game
    {
        private GameState _gameState;

        public GameState GameState
        {
            get
            {
                return _gameState;
            }
            set
            {
                _gameState = value;
                OnGameStateChanged(_gameState);
            }
        }

        private Dictionary<Player, string> PlayerRank;

        /// <summary>
        /// Instantiates a Game
        /// </summary>
        /// <param name="contestants">The players in the match</param>
        public Game(List<Player> contestants)
        {
            GameState = GameState.NotStarted;

            PlayerRank = new Dictionary<Player, string>();
        }

        public void GiveRankToPlayer(string rank, Player player)
        {
            PlayerRank[player] = rank;
        }

        public event EventHandler<GameStateChangedEventArgs> GameStateChanged;

        private void OnGameStateChanged(GameState newState)
        {
            if(GameStateChanged != null)
            {
                GameStateChanged(this, new GameStateChangedEventArgs(newState));
            }
        }

        public Player GetWinner()
        {
            Player result = null;

            foreach(Player player in PlayerRank.Keys)
            {
                if(result == null)
                {
                    result = player;
                }
                else
                {
                    if(TourneyConfig.IsWorthMoreThan(PlayerRank[player], PlayerRank[result]))
                    {
                        result = player;
                    }
                }
            }

            return result;
        }

        public List<Player> GetLosers()
        {
            List<Player> result = null;

            Player winner = GetWinner();
            foreach(Player player in PlayerRank.Keys)
            {
                if(result == null)
                {
                    result = new List<Player>();
                }

                if(player != winner)
                {
                    result.Add(player);
                }
            }

            return result;
        }

        public string GetRankForPlayer(Player player)
        {
            if (PlayerRank.ContainsKey(player))
            {
                return PlayerRank[player];
            }
            else
            {
                throw new ArgumentException("The player: " + player.Name + " could not be found.");
            }
        }
    }

    public class GameStateChangedEventArgs : EventArgs
    {
        public GameState NewState;

        public GameStateChangedEventArgs(GameState newState)
        {
            NewState = newState;
        }
    }

    public enum GameState
    {
        NotStarted,
        InProgress,
        Finished
    }
}
