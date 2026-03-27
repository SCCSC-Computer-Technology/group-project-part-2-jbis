using System;
using System.Collections.Generic;
using System.Text;

namespace WorldWideSportsLibrary
{
    public class PGAPlayerStats
    {
        //Fields
        public int Season { get; set; }
        public string PlayerName { get; set; }
        public int EventsPlayed { get; set; }
        public int Wins { get; set; }
        public int TopTenFinish { get; set; }
        public double AverageScore { get; set; }
        public int TotalEarnings { get; set; }
        public int TotalFedXPoints { get; set; }

        //Default
        public PGAPlayerStats()
        {
            Season = 2000;
            PlayerName = "";
            Wins = 0;
            TopTenFinish = 0;
            AverageScore = 0;
            EventsPlayed = 0;
            TotalEarnings = 0;
            TotalFedXPoints = 0;
        }
        //Constructor
        public PGAPlayerStats(int season, string playerName, int eventsPlayed, int wins, int topTenFinish, double average,
            int totalEarnings, int totalFedXPoints)
        {
            Season = season;
            PlayerName = playerName;
            Wins = wins;
            TopTenFinish = topTenFinish;
            AverageScore = average;
            EventsPlayed = eventsPlayed;
            TotalEarnings = totalEarnings;
            TotalFedXPoints = totalFedXPoints;
        }
        //Override toString to output the class
        public override string ToString()
        {
            return " \nSeason Played: " + Season + "Player Name: " + PlayerName + " \nWins: " + Wins + " \nTop 10 Finishes: "
                + TopTenFinish + " \nAverage Scores: " + AverageScore.ToString("F2") + " \nEvents Played: " + EventsPlayed
                + " \nTotal Earnings: " + TotalEarnings.ToString("C") + " \nTotal Fedex Points: " + TotalFedXPoints.ToString("N0");
        }
    }
}
