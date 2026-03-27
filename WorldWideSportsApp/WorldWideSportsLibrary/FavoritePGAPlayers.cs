using System;
using System.Collections.Generic;
using System.Text;

namespace WorldWideSportsLibrary
{
    public class FavoritePGAPlayers
    {
        public int FavoritePGAPlayerId { get; set; }
        public int UserId { get; set; }
        public string PlayerName { get; set; }

        public FavoritePGAPlayers()
        {
            FavoritePGAPlayerId = 0;
            UserId = 0;
            PlayerName = "";
        }
        public FavoritePGAPlayers(int favoritePGAPlayerId, int userId, string playerName)
        {
            FavoritePGAPlayerId = favoritePGAPlayerId;
            UserId = userId;
            PlayerName = playerName;
        }
        public override string ToString()
        {
            return $"FavoritePGAPlayerId: {FavoritePGAPlayerId} UserId: {UserId} PlayerName: {PlayerName}";
        }
    }
}
