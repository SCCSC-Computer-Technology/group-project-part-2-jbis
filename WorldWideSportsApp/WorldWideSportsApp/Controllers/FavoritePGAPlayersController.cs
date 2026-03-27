using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WorldWideSportsLibrary;

public class FavoritePGAPlayersController : Controller
{
    private readonly string _conn;
    public FavoritePGAPlayersController(IConfiguration config)
    {
        _conn = config.GetConnectionString("DefaultConnection")!;
    }

    public IActionResult Index()
    {
        var favorites = new List<FavoritePGAPlayers>();
        using var conn = new SqlConnection(_conn);
        conn.Open();
        var cmd = new SqlCommand("SELECT FavoritePGAPlayerId, UserId, PlayerName FROM FavoritePGAPlayers", conn);
        var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            favorites.Add(new FavoritePGAPlayers(
                Convert.ToInt32(reader["FavoritePGAPlayerId"]),
                Convert.ToInt32(reader["UserId"]),
                reader["PlayerName"].ToString()!));
        }
        return View(favorites);
    }
}