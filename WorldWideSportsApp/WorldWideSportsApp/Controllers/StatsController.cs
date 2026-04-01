using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WorldWideSportsLibrary;

namespace WorldWideSportsApp.Controllers
{
    public class StatsController : Controller
    {
        private readonly string _conn;

        public StatsController(IConfiguration config)
        {
            _conn = config.GetConnectionString("DefaultConnection")!;
        }

        //getting the main NFL stats page with optional filters for team, player name, and season — capped at 50 rows
        public IActionResult NflDetails(string? team, string? player, int? season)
        {
            // default to 2024 season so the page always has something to show on load
            season ??= 2024;

            var players = new List<NFLPlayers>();
            var teams = new List<string>();
            var seasons = new List<int>();

            using var conn = new SqlConnection(_conn);
            conn.Open();

            // load distinct team abbreviations for the filter dropdown
            var teamCmd = new SqlCommand(
                "SELECT DISTINCT team FROM NFL_Player_Stats_Season ORDER BY team", conn);
            var tr = teamCmd.ExecuteReader();
            while (tr.Read()) teams.Add(tr["team"].ToString()!);
            tr.Close();

            // load distinct seasons for the season filter dropdown
            var seasonCmd = new SqlCommand(
                "SELECT DISTINCT season FROM NFL_Player_Stats_Season ORDER BY season DESC", conn);
            var sr = seasonCmd.ExecuteReader();
            while (sr.Read()) seasons.Add(Convert.ToInt32(sr["season"]));
            sr.Close();

            //build the query — always requires a season to avoid loading all rows
            //getting the top 50 dumping thousands of rows into the page at once
            var query = @"SELECT TOP 50
                            player_display_name, position, season, team,
                            games_played, fantasy_points
                          FROM NFL_Player_Stats_Season
                          WHERE season = @season";

            //add optional filters if the user selected them
            if (!string.IsNullOrEmpty(team))
                query += " AND team = @team";

            if (!string.IsNullOrEmpty(player))
                query += " AND player_display_name LIKE @player";

            //order by fantasy points so best players show at the top
            query += " ORDER BY fantasy_points DESC";

            var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@season", season);

            if (!string.IsNullOrEmpty(team))
                cmd.Parameters.AddWithValue("@team", team);

            if (!string.IsNullOrEmpty(player))
                cmd.Parameters.AddWithValue("@player", $"%{player}%");

            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                //mapping each row to the NFLPlayers class
                players.Add(new NFLPlayers(
                    reader["player_display_name"].ToString()!,
                    reader["position"].ToString()!,
                    reader["season"].ToString()!,
                    reader["team"].ToString()!,
                    reader["games_played"] == DBNull.Value ? "0" : reader["games_played"].ToString()!,
                    reader["fantasy_points"] == DBNull.Value ? 0.0 : Convert.ToDouble(reader["fantasy_points"])
                ));
            }
            reader.Close();

            //pass data to the view
            ViewBag.Teams = teams;
            ViewBag.Seasons = seasons;
            ViewBag.SelTeam = team ?? "";
            ViewBag.SelPlayer = player ?? "";
            ViewBag.SelSeason = season;

            return View(players);
        }

        //getting the main PGA stats page with optional filters for team, player name, and season — capped at 50 rows
        public IActionResult PgaDetails(string? player, int? season)
        {
            // default to 2024 season so the page always has something on load
            season ??= 2024;

            var results = new List<PGAPlayerStats>();
            var seasons = new List<int>();

            using var conn = new SqlConnection(_conn);
            conn.Open();

            //load distinct seasons for the season filter dropdown
            var seasonCmd = new SqlCommand(
                "SELECT DISTINCT season FROM PGA_Player_Stats ORDER BY season DESC", conn);
            var sr = seasonCmd.ExecuteReader();
            while (sr.Read()) seasons.Add(Convert.ToInt32(sr["season"]));
            sr.Close();

            //build the query — always requires a season to avoid loading all rows
            //getting the top 50 prevents dumping thousands of rows into the page at once
            var query = @"SELECT TOP 50
                            season, player_name, events_played, wins, top10_finishes,
                            avg_score, total_earnings, total_fedex_points
                          FROM PGA_Player_Stats
                          WHERE season = @season";

            //optional player name filter if the user searched for someone
            if (!string.IsNullOrEmpty(player))
                query += " AND player_name LIKE @player";

            //order by points so best players show at the top
            query += " ORDER BY total_fedex_points DESC";

            var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@season", season);

            if (!string.IsNullOrEmpty(player))
                cmd.Parameters.AddWithValue("@player", $"%{player}%");

            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                // map each row to the PGAPlayerStats class
                results.Add(new PGAPlayerStats(
                    Convert.ToInt32(reader["season"]),
                    reader["player_name"].ToString()!,
                    reader["events_played"] == DBNull.Value ? 0 : Convert.ToInt32(reader["events_played"]),
                    reader["wins"] == DBNull.Value ? 0 : Convert.ToInt32(reader["wins"]),
                    reader["top10_finishes"] == DBNull.Value ? 0 : Convert.ToInt32(reader["top10_finishes"]),
                    reader["avg_score"] == DBNull.Value ? 0.0 : Convert.ToDouble(reader["avg_score"]),
                    reader["total_earnings"] == DBNull.Value ? 0 : Convert.ToInt32(reader["total_earnings"]),
                    reader["total_fedex_points"] == DBNull.Value ? 0 : Convert.ToInt32(reader["total_fedex_points"])
                ));
            }
            reader.Close();

            //pass data to the view
            ViewBag.Seasons = seasons;
            ViewBag.SelPlayer = player ?? "";
            ViewBag.SelSeason = season;

            return View(results);
        }

        //getting player names for the NFL player search autocomplete — returns JSON
        [HttpGet]
        public IActionResult SearchNflPlayers(string term)
        {
            var results = new List<object>();
            using var conn = new SqlConnection(_conn);
            conn.Open();

            var cmd = new SqlCommand(
                @"SELECT DISTINCT TOP 20 player_display_name
                  FROM NFL_Player_Stats_Season
                  WHERE player_display_name LIKE @term
                  ORDER BY player_display_name", conn);
            cmd.Parameters.AddWithValue("@term", $"%{term}%");

            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                results.Add(new
                {
                    id = reader["player_display_name"].ToString(),
                    text = reader["player_display_name"].ToString()
                });
            }
            return Json(new { results });
        }
    }
}