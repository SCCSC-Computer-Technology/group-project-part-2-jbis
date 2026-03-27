using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WorldWideSportsLibrary;

public class NFLPlayersController : Controller
{
    private readonly string _conn;
    public NFLPlayersController(IConfiguration config)
    {
        _conn = config.GetConnectionString("DefaultConnection")!;
    }

    public IActionResult Index()
    {
        var players = new List<NFLPlayers>();
        using var conn = new SqlConnection(_conn);
        conn.Open();
        var cmd = new SqlCommand("SELECT player_display_name, position, season, team, games_played, fantasy_points FROM NFL_Player_Stats_Season", conn);
        var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            players.Add(new NFLPlayers(
                reader["player_display_name"].ToString()!,
                reader["position"].ToString()!,
                reader["season"].ToString()!,
                reader["team"].ToString()!,
                reader["games_played"].ToString()!,
                Convert.ToDouble(reader["fantasy_points"])
            ));
        }
        return View(players);
    }
}
