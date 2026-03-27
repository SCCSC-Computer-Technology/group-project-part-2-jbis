using System;
using System.Collections.Generic;
using System.Text;

namespace WorldWideSportsLibrary
{
    public class FavoriteNFLTeams
    {
        public int FavoriteTeamId { get; set; }
        public int UserId { get; set; }
        public string TeamAbbr { get; set; }

        public FavoriteNFLTeams()
        {
            FavoriteTeamId = 0;
            UserId = 0;
            TeamAbbr = "";
        }
        public FavoriteNFLTeams(int favoriteTeamId, int userId, string teamAbbr)
        {
            FavoriteTeamId = favoriteTeamId;
            UserId = userId;
            TeamAbbr = teamAbbr;
        }
        public override string ToString()
        {
            return $"FavoriteTeamId: {FavoriteTeamId} UserId: {UserId} TeamAbbr: {TeamAbbr}";
        }
    }
}
