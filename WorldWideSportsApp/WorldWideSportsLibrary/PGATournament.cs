using System;
using System.Collections.Generic;
using System.Text;

namespace WorldWideSportsLibrary
{
    public class PGATournament
    {
        public int Season { get; set; }
        public string Tournament { get; set; }
        public string Location { get; set; }
        public string Position { get; set; }
        public string Score { get; set; }
        public int Round1 { get; set; }
        public int Round2 { get; set; }
        public int Round3 { get; set; }
        public int Round4 { get; set; }
        public int Total { get; set; }
        public double Earnings { get; set; }
        public int FedXPoints { get; set; }

        public PGATournament()
        {
            Season = 2000;
            Tournament = "";
            Location = "N/A";
            Position = "N/A";
            Score = "N/A";
            Round1 = 0;
            Round2 = 0;
            Round3 = 0;
            Round4 = 0;
            Total = 0;
            Earnings = 0.00;
            FedXPoints = 0;
        }
        public PGATournament(int season, string tournament, string location, string position, string score,
            int round1, int round2, int round3, int round4, int total, double earnings, int fedXPoints)
        {
            Season = season;
            Tournament = tournament;
            Location = location;
            Position = position;
            Score = score;
            Round1 = round1;
            Round2 = round2;
            Round3 = round3;
            Round4 = round4;
            Total = total;
            Earnings = earnings;
            FedXPoints = fedXPoints;
        }
        public override string ToString()
        {
            return $"Season: {Season} \nTournament: {Tournament} \nLocation: {Location} \nPosition {Position} \nScore {Score}" +
                $" \nRound Scores: {Round1}, {Round2}, {Round3}, {Round4} \nTotal: {Total} \nEarnings: {Earnings} \nFedXPoints: {FedXPoints}";
        }
    }
}
