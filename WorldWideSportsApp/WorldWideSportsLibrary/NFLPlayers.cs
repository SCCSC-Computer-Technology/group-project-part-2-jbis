using System;
using System.Collections.Generic;
using System.Text;

namespace WorldWideSportsLibrary
{
    public class NFLPlayers
    {
        public string PlayerName { get; set; }
        public string Position { get; set; }
        public string Season { get; set; }
        public string Team { get; set; }
        public string GamesPlayed { get; set; }
        public double FantasyPoints { get; set; }

        public NFLPlayers()
        {
            PlayerName = "";
            Position = "";
            Season = "";
            Team = "";
            GamesPlayed = "";
            FantasyPoints = 0.0;
        }
        public NFLPlayers(string playerName, string position, string season, string team, string gamesPlayed, double fantasyPoints)
        {
            PlayerName = playerName;
            Position = position;
            Season = season;
            Team = team;
            GamesPlayed = gamesPlayed;
            FantasyPoints = fantasyPoints;
        }
        public override string ToString()
        {
            return $"Player Name: {PlayerName} Position: {Position} Season: {Season} Team: {Team} Games Played: {GamesPlayed} Fantasy points: {FantasyPoints} ";
        }
    }
}
