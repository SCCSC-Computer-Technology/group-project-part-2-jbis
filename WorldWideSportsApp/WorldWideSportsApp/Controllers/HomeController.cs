using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WorldWideSportsApp.Models;
using Microsoft.Data.SqlClient;

using WorldWideSportsLibrary;

namespace WorldWideSportsApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _connString;

        public HomeController(IConfiguration config)
        {
            _connString = config.GetConnectionString("DefaultConnection")!;
        }
        public IActionResult Index()
        {
            var games = new List<NFLGames>();
            var tournaments = new List<PGATournament>();

            using var conn = new SqlConnection(_connString);
            conn.Open();

            // Pull 10 NFL games from 2025
            var nflCmd = new SqlCommand(
                "SELECT TOP 12 game_id, season, week, gameday, gametime, away_team, home_team, stadium " +
                "FROM NFL_Games WHERE season = '2025' ORDER BY week, gameday", conn);

            var nflReader = nflCmd.ExecuteReader();
            while (nflReader.Read())
            {
                games.Add(new NFLGames
                {
                    GameId = nflReader["game_id"].ToString()!,
                    Season = nflReader["season"].ToString()!,
                    Week = Convert.ToByte(nflReader["week"]),
                    Gameday = Convert.ToDateTime(nflReader["gameday"]),
                    Gametime = TimeSpan.Parse(nflReader["gametime"].ToString()),
                    AwayTeam = nflReader["away_team"].ToString()!,
                    HomeTeam = nflReader["home_team"].ToString()!,
                    Stadium = nflReader["stadium"].ToString()!
                });
            }
            nflReader.Close();

            // Pull 10 PGA tournaments
            //i pulled only each instance of a tournament based on the position
            var pgaCmd = new SqlCommand(
                "SELECT TOP 10 season, tournament, location, position, score, " +
                "round1, round2, round3, round4, total, earnings " +
                "FROM PGA_ALL_TOURNAMENTS " +
                "WHERE position = '1' " +
                "ORDER BY season DESC", conn);

            var pgaReader = pgaCmd.ExecuteReader();
            while (pgaReader.Read())
            {
                tournaments.Add(new PGATournament(
                    Convert.ToInt32(pgaReader["season"])!,
                    pgaReader["tournament"].ToString()!,
                    pgaReader["location"].ToString()!,
                    pgaReader["position"].ToString()!,
                    pgaReader["score"].ToString()!,
                    Convert.ToInt32(pgaReader["round1"]),
                    Convert.ToInt32(pgaReader["round2"]),
                    Convert.ToInt32(pgaReader["round3"]),
                    Convert.ToInt32(pgaReader["round4"]),
                    Convert.ToInt32(pgaReader["total"]),
                    Convert.ToDouble(pgaReader["earnings"])
                ));
            }

            ViewBag.Games = games;
            ViewBag.Tournaments = tournaments;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
