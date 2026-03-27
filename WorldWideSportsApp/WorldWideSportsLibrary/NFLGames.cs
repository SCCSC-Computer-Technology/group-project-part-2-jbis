using System;
using System.Collections.Generic;
using System.Text;

namespace WorldWideSportsLibrary
{
    public class NFLGames
    {
        public string GameId { get; set; }
        public string Season { get; set; }
        public byte Week { get; set; }
        public DateTime Gameday { get; set; }
        public TimeSpan Gametime { get; set; }
        public string AwayTeam { get; set; }
        public string HomeTeam { get; set; }
        public string Stadium { get; set; }
        public int AwayScore { get; set; }
        public int HomeScore { get; set; }
        public int Total { get; set; }
        public Boolean OverTime { get; set; }
        public string AwayCoach { get; set; }
        public string HomeCoach { get; set; }
        public string Referee { get; set; }

        //default constructor
        public NFLGames()
        {
            GameId = "";
            Season = "";
            Week = 0;
            Gameday = DateTime.MinValue;
            Gametime = TimeSpan.MinValue;
            AwayTeam = "";
            HomeTeam = "";
            Stadium = "HubCityIT Stadium";
            AwayScore = 0;
            HomeScore = 0;
            Total = 0;
            OverTime = false;
            AwayCoach = "N/A";
            HomeCoach = "N/A";
            Referee = "N/A";
        }
        public NFLGames(string gameId, string season, byte week, DateTime gameday, TimeSpan gametime, string awayTeam, string homeTeam, string stadium, int awayScore, int homeScore, int total, bool overTime, string awayCoach, string homeCoach, string referee)
        {
            GameId = gameId;
            Season = season;
            Week = week;
            Gameday = gameday;
            Gametime = gametime;
            AwayTeam = awayTeam;
            HomeTeam = homeTeam;
            Stadium = stadium;
            AwayScore = awayScore;
            HomeScore = homeScore;
            Total = total;
            OverTime = overTime;
            AwayCoach = awayCoach;
            HomeCoach = homeCoach;
            Referee = referee;
        }
        public override string ToString()
        {
            if (OverTime == true)
            {
                return $"Season:{Season} Week:{Week} Gameday: {Gameday:MM/dd/yyyy} Gametime: {Gametime:hh\\:mm}" +
                $"\nHomeTeam: {HomeTeam}, Score:{HomeScore}, Coach: {HomeCoach}" +
                $"\nAwayTeam: {AwayTeam}, Score:{AwayScore}, Coach: {AwayCoach}" +
                $"Total Score: {Total} Overtime: Yes referee: {Referee}";
            }
            else
            {
                return $"Season:{Season} Week:{Week} Gameday: {Gameday:MM/dd/yyyy} Gametime: {Gametime:hh\\:mm}" +
                $"\nHomeTeam: {HomeTeam}, Score:{HomeScore}, Coach: {HomeCoach}" +
                $"\nAwayTeam: {AwayTeam}, Score:{AwayScore}, Coach: {AwayCoach}" +
                $"Total Score: {Total} Overtime: No referee: {Referee}";
            }
        }
    }
}
