using System.Data.SqlClient;
using System.Xml.Linq;

namespace SimchaFund.Data
{
    public class Simcha
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
    }
    public class Contributor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Cell { get; set; }
        public bool AlwaysInclude { get; set; }
    }
    public class Contribution
    {
        public int SimchaId { get; set; }
        public int ContributorId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
    public class Deposit
    {
        public int Id { get; set; }
        public int ContributorId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
    public class SimchaManager
    {
        private string _connectionString;
        public SimchaManager(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void AddSimcha(string name, DateTime date)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Simchas (Name, Date) " +
                "VALUES (@name, @date)";
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@date", date);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
        public List<Simcha> GetSimchas()
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Simchas";
            connection.Open();
            var reader = command.ExecuteReader();
            var simchas = new List<Simcha>();
            while (reader.Read())
            {
                var simcha = new Simcha
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    Date = (DateTime)reader["Date"]
                };
                simchas.Add(simcha);
            }
            return simchas;
        }
        public int GetContributorTotal()
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM Contributors";
            connection.Open();
            var count = command.ExecuteScalar();
            return (int)count;
        }
        public List<Contributor> GetContributors()
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Contributors";
            connection.Open();
            var reader = command.ExecuteReader();
            var contributors = new List<Contributor>();
            while (reader.Read())
            {
                var contributor = new Contributor
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    Cell = (int)reader["Cell"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"]
                };
                contributors.Add(contributor);
            }
            return contributors;
        }
        public void MakeDeposit(int contributorId, decimal amount, DateTime date)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Deposits (ContributorId, Amount, Date) " +
                "VALUES (@contributorId, @amount, @date)";
            command.Parameters.AddWithValue("@contributorId", contributorId);
            command.Parameters.AddWithValue("@amount", amount);
            command.Parameters.AddWithValue("@date", date);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
    }

}