using System.Security.Cryptography;
using System.Text;



namespace WorldWideSportsApp.Helpers
{
    public static class PasswordHelper
    {

        //hash the password with random salt and return the combined string of salt and hash
        public static string HashPassword(string password)
        {
            //creating the random salt
            byte[] saltBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            // Convert the salt to a string for storage
            string salt = Convert.ToBase64String(saltBytes);


            //this will hash the password with the salt using SHA256 and return the combined string of salt and hash
            using (var sha256 = SHA256.Create())
            {
                string combined = password + salt;
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
                string hash = Convert.ToBase64String(hashBytes);

                //storing the salt and hash together separated by a colon, this way we can easily retrieve the salt when verifying the password
                return salt + ":" + hash;
            }
        }

        public static bool VerifyPassword(string enteredPassword, string storedValue)
        {
            //this will split the stored value into salt and hash, then hash the entered password with the salt and compare it to the stored hash
            string[] parts = storedValue.Split(':');
            if (parts.Length != 2) return false;

            string salt = parts[0];
            string storedHash = parts[1];

            //compares the login atttempt against the one saved in the database.
            using (var sha256 = SHA256.Create())
            {
                string combined = enteredPassword + salt;
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
                string hashOfEntered = Convert.ToBase64String(hashBytes);

                return hashOfEntered == storedHash;
            }
        }
    }
}
