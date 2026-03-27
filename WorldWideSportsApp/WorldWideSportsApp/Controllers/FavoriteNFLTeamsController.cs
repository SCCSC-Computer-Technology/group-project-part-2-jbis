using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WorldWideSportsLibrary;

public class FavoriteNFLTeamsController : Controller
{
    private readonly string _conn;
    public FavoriteNFLTeamsController(IConfiguration config)
    {
        _conn = config.GetConnectionString("DefaultConnection")!;
    }

    public IActionResult Index()
    {
        var favorites = new List<FavoriteNFLTeams>();
        using var conn = new SqlConnection(_conn);
        conn.Open();
        var cmd = new SqlCommand("SELECT FavoriteTeamId, UserId, TeamAbbr FROM FavoriteNFLTeams", conn);
        var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            favorites.Add(new FavoriteNFLTeams(
                Convert.ToInt32(reader["FavoriteTeamId"]),
                Convert.ToInt32(reader["UserId"]),
                reader["TeamAbbr"].ToString()!));
        }
        return View(favorites);
    }
}
