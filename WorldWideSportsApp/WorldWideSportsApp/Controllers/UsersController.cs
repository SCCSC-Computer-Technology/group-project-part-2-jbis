using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using WorldWideSportsLibrary;

public class UsersController : Controller
{
    private readonly string _conn;
    public UsersController(IConfiguration config)
    {
        _conn = config.GetConnectionString("DefaultConnection")!;
    }

    public IActionResult Index()
    {
        var users = new List<Users>();
        using var conn = new SqlConnection(_conn);
        conn.Open();
        var cmd = new SqlCommand("SELECT UserId, Username, PasswordHash, CreatedAt, email, verified, verification_code, verification_expires_at FROM Users", conn);
        var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            users.Add(new Users(
                Convert.ToInt32(reader["UserId"]),
                reader["Username"].ToString()!,
                reader["PasswordHash"].ToString()!,
                Convert.ToDateTime(reader["CreatedAt"]),
                reader["email"] == DBNull.Value ? "" : reader["email"].ToString()!,
                Convert.ToBoolean(reader["verified"]),
                reader["verification_code"] == DBNull.Value ? "" : reader["verification_code"].ToString()!,
                reader["verification_expires_at"] == DBNull.Value ? null : Convert.ToDateTime(reader["verification_expires_at"])
            ));
        }
        return View(users);
    }
}