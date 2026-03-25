using System;
using System.Collections.Generic;
using System.Text;

namespace WorldWideSportsLibrary
{
    public class NFLPlayers
    {
        public string PlayerName { get; set; }
        public string PlayerAge { get; set; }
        public string Position { get; set; }
        public string Season { get; set; }
        public string GamesPlayed { get; set; }
        public string GamesStarted { get; set; }
        public string TotalPoints { get; set; }
        public string PointsPerGame { get; set; }

        public NFLPlayers()
        {
            PlayerName = "";
            PlayerAge = "";
            Position = "";
            Season = "";
            GamesPlayed = "";
            GamesStarted = "";
            GamesStarted = "";
            TotalPoints = "";
            PointsPerGame = "";
        }
        public NFLPlayers(string playerName, string playerAge, string position, string season, string gamesPlayed,
            string gamesStarted, string totalPoints, string pointsper)
        {
            PlayerName = playerName;
            PlayerAge = playerAge;
            Position = position;
            Season = season;
            GamesPlayed = gamesPlayed;
            GamesStarted = gamesStarted;
            TotalPoints = totalPoints;
            PointsPerGame = pointsper;
        }
        public override string ToString()
        {
            return $"Player Name: {PlayerName} Age: {PlayerAge} Position: {Position} Season: {Season} Games Played:{GamesPlayed} " +
                $"Games Started: {GamesStarted} Points per Game: {PointsPerGame} Total Points: {TotalPoints}";
        }
    }
}
