using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WorldWideSportsLibrary;

public class PGAPlayerStatsController : Controller
{
    private readonly string _conn;
    public PGAPlayerStatsController(IConfiguration config)
    {
        _conn = config.GetConnectionString("DefaultConnection")!;
    }

    public IActionResult Index()
    {
        var stats = new List<PGAPlayerStats>();
        using var conn = new SqlConnection(_conn);
        conn.Open();
        var cmd = new SqlCommand("SELECT season, player_name, events_played, wins, top10_finishes, avg_score, total_earnings, total_fedex_points FROM PGA_PLAYER_STATS", conn);
        var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            stats.Add(new PGAPlayerStats
            (
                Convert.ToInt32(reader["season"]),
                reader["player_name"].ToString()!,
                Convert.ToInt32(reader["events_played"]),
                Convert.ToInt32(reader["wins"]),
                Convert.ToInt32(reader["top10_finishes"]),
                Convert.ToDouble(reader["avg_score"]),
                Convert.ToInt32(reader["total_earnings"]),
                Convert.ToInt32(reader["total_fedex_points"])
            ));
        }
        return View(stats);
    }
}