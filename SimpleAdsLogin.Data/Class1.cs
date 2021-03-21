using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SimpleAdsLogin.Data
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
    public class Ad
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public int UserId { get; set; }
        public string PhoneNumber { get; set; }
        public string Details { get; set; }
        public DateTime Date { get; set; }
    }
    public class SimpleAdsDb
    {
        private readonly string _connectionString;
        public SimpleAdsDb(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void AddUser(User user, string password)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "insert into users values (@name, @email, @passwordhash)";
                cmd.Parameters.AddWithValue("@name", user.Name);
                cmd.Parameters.AddWithValue("@email", user.Email);
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
                cmd.Parameters.AddWithValue("@passwordhash", user.PasswordHash);
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public List<Ad> GetAllAds()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"select *, a.id as 'adId' from users u
                                    join ads a
                                    on a.UserId = u.Id
                                    order by a.Date desc, a.id desc";
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                List<Ad> ads = new List<Ad>();
                while (reader.Read())
                {
                    ads.Add(new Ad
                    {
                        Id = (int)reader["adId"],
                        UserId = (int)reader["userId"],
                        UserName = (string)reader["name"],
                        PhoneNumber = (string)reader["phoneNumber"],
                        Details = (string)reader["details"],
                        Date = (DateTime)reader["date"]
                    });
                }
                return ads;
            }
        }
        public void AddAd(Ad ad)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "insert into ads values(@userId, @phonenumber, @details, @date)";
                cmd.Parameters.AddWithValue("@userId", ad.UserId);
                cmd.Parameters.AddWithValue("@phoneNumber", ad.PhoneNumber);
                cmd.Parameters.AddWithValue("@details", ad.Details);
                cmd.Parameters.AddWithValue("@date", DateTime.Now);
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public User LogIn(string email, string password)
        {
            var user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }
            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            return isValid ? user : null;
        }
        public User GetByEmail(string email)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "select * from users where email = @email";
                cmd.Parameters.AddWithValue("@email", email);
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new User
                    {
                        Id = (int)reader["id"],
                        Name = (string)reader["name"],
                        Email = (string)reader["email"],
                        PasswordHash = (string)reader["passwordHash"]
                    };
                }
                else
                {
                    return null;
                }
            }
        }
        public void DeleteAd(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "delete from ads where id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public List<Ad> MyAccount(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"select *, a.id as 'adId' from users u
                                    join ads a
                                    on a.UserId = u.Id
                                    where u.id = @id
                                    order by a.Date desc, a.id desc";
                cmd.Parameters.AddWithValue("@id", id);
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                List<Ad> ads = new List<Ad>();
                while (reader.Read())
                {
                    ads.Add(new Ad
                    {
                        Id = (int)reader["adId"],
                        UserId = (int)reader["userId"],
                        UserName = (string)reader["name"],
                        PhoneNumber = (string)reader["phoneNumber"],
                        Details = (string)reader["details"],
                        Date = (DateTime)reader["date"]
                    });
                }
                return ads;
            }
        }
    }
}
