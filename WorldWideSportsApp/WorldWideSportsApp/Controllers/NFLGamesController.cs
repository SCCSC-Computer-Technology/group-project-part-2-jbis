using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WorldWideSportsLibrary;

public class NFLGamesController : Controller
{
    private readonly string _conn;
    public NFLGamesController(IConfiguration config)
    {
        _conn = config.GetConnectionString("DefaultConnection")!;
    }


    public IActionResult Index()
    {
        var games = new List<NFLGames>();
        using var conn = new SqlConnection(_conn);
        conn.Open();
        var cmd = new SqlCommand(@"SELECT game_id, season, week, gameday, gametime, away_team, home_team, stadium, away_score, home_score, 
        total, overtime, away_coach, home_coach, referee 
        FROM NFL_Games", conn);
        var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            games.Add(new NFLGames(
                reader["game_id"].ToString()!,
                reader["season"].ToString()!,
                Convert.ToByte(reader["week"]),
                Convert.ToDateTime(reader["gameday"]),
                TimeSpan.Parse(reader["gametime"].ToString())!,
                reader["away_team"].ToString()!,
                reader["home_team"].ToString()!,
                reader["stadium"].ToString()!,
                reader["away_score"] == DBNull.Value ? 0 : Convert.ToInt32(reader["away_score"]),
                reader["home_score"] == DBNull.Value ? 0 : Convert.ToInt32(reader["home_score"]),
                reader["total"] == DBNull.Value ? 0 : Convert.ToInt32(reader["total"]),
                reader["overtime"] == DBNull.Value ? false : Convert.ToBoolean(reader["overtime"]),
                reader["away_coach"].ToString()!,
                reader["home_coach"].ToString()!,
                reader["referee"].ToString()!
            ));
        }
        return View(games);
    }
}
