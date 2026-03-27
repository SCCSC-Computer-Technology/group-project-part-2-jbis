using System;
using System.Collections.Generic;
using System.Text;

namespace WorldWideSportsLibrary
{
    public class Users
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Email { get; set; }
        public bool Verified { get; set; }
        public string VerificationCode { get; set; }
        public DateTime? VerificationExpiresAt { get; set; }

        public Users()
        {
            UserId = 0;
            Username = "";
            PasswordHash = "";
            CreatedAt = DateTime.MinValue;
            Email = "";
            Verified = false;
            VerificationCode = "";
            VerificationExpiresAt = null;
        }
        public Users(int userId, string username, string passwordHash, DateTime createdAt, string email, bool verified, string verificationCode, DateTime? verificationExpiresAt)
        {
            UserId = userId;
            Username = username;
            PasswordHash = passwordHash;
            CreatedAt = createdAt;
            Email = email;
            Verified = verified;
            VerificationCode = verificationCode;
            VerificationExpiresAt = verificationExpiresAt;
        }
        public override string ToString()
        {
            return $"UserId: {UserId} Username: {Username} Email: {Email} Verified: {Verified} CreatedAt: {CreatedAt}";
        }
    }
}
