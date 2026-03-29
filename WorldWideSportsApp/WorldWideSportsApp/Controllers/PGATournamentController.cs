using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WorldWideSportsLibrary;

public class PGATournamentController : Controller
{
    private readonly string _conn;
    public PGATournamentController(IConfiguration config)
    {
        _conn = config.GetConnectionString("DefaultConnection")!;
    }

    public IActionResult Index()
    {
        var tournaments = new List<PGATournament>();
        using var conn = new SqlConnection(_conn);
        conn.Open();
        var cmd = new SqlCommand("SELECT season, tournament, location, position, score, round1, round2, round3, round4, total, earnings FROM PGA_ALL_TOURNAMENTS", conn);
        var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            tournaments.Add(new PGATournament(
                Convert.ToInt32(reader["season"]),
                reader["tournament"].ToString()!,
                reader["location"].ToString()!,
                reader["position"].ToString()!,
                reader["score"].ToString()!,
                Convert.ToInt32(reader["round1"]),
                Convert.ToInt32(reader["round2"]),
                Convert.ToInt32(reader["round3"]),
                Convert.ToInt32(reader["round4"]),
                Convert.ToInt32(reader["total"]),
                Convert.ToDouble(reader["earnings"])
            ));
        }
        return View(tournaments);
    }
}
