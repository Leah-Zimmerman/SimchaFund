using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace SimchaFund.Data
{
    public class Simcha
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public int ContributorCount { get; set; }
        public decimal Total { get; set; }
    }
    public class Contributor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Cell { get; set; }
        public decimal Balance { get; set; }
        public bool AlwaysInclude { get; set; }
        public decimal Amount { get; set; }
        public bool Contribute { get; set; }
    }
    public class Contribution
    {
        public int SimchaId { get; set; }
        public int ContributorId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string SimchaName { get; set; }
    }
    public class Deposit
    {
        public int Id { get; set; }
        public int ContributorId { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
    public class ContributorRow
    {
        public bool Contribute { get; set; }
        public int ContributorId { get; set; }
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
        public List<Simcha> GetSimchasWithContributorCountAndTotal()
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT s.Id, s.Name, s.Date, ISNULL(c.ContributorCount,0) AS ContributorCount, " +
                "ISNULL(c.SimchaId, 0) AS SimchaId, ISNULL(Total, 0) AS Total FROM Simchas s " +
                "LEFT JOIN(SELECT cn.SimchaId, COUNT(cn.SimchaId) AS ContributorCount, SUM(cn.Amount) AS Total FROM Contributions cn " +
                "GROUP BY cn.SimchaId) AS c " +
                "ON c.SimchaId = s.Id";
            connection.Open();
            var reader = command.ExecuteReader();
            var simchas = new List<Simcha>();
            while (reader.Read())
            {
                var simcha = new Simcha
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    Date = (DateTime)reader["Date"],
                    ContributorCount = (int)reader["ContributorCount"],
                    Total = (decimal)reader["Total"]
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
            command.CommandText = "SELECT * FROM Contributors " +
                "ORDER BY LastName";
            connection.Open(); 
            var reader = command.ExecuteReader();
            var contributors = new List<Contributor>();
            while (reader.Read())
            {
                var contributor = new Contributor
                {
                    Id = (int)reader["Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    Cell = (string)reader["Cell"],
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
        //public decimal GetTotalDeposits(int contributorId)
        //{
        //    using var connection = new SqlConnection(_connectionString);
        //    using var command = connection.CreateCommand();
        //    command.CommandText = "SELECT SUM(Amount) FROM Deposits Where ContributorId = @contributorId";
        //    command.Parameters.AddWithValue("@contributorId", contributorId);
        //    connection.Open();
        //    var total = command.ExecuteScalar();
        //    connection.Close();
        //    return (decimal)total;
        //}
        //public decimal GetTotalContributions(int contributorId)
        //{
        //    using var connection = new SqlConnection(_connectionString);
        //    using var command = connection.CreateCommand();
        //    command.CommandText = "SELECT SUM(Amount) FROM Contributions Where ContributorId = @contributorId";
        //    command.Parameters.AddWithValue("@contributorId", contributorId);
        //    connection.Open();
        //    var total = command.ExecuteScalar();
        //    connection.Close();
        //    return (decimal)total;
        //}
        //public decimal GetBalance(int contributorId)
        //{
        //    decimal totalDeposits = GetTotalDeposits(contributorId);
        //    decimal totalContributions = GetTotalContributions(contributorId);
        //    return totalDeposits - totalContributions;
        //}
        public List<Contributor> GetContributorsWithBalance()
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT c.Id, c.FirstName,c.LastName,c.Cell, c.AlwaysInclude, ISNULL(d.DepositTotal,0),ISNULL(cn.ContributionTotal,0), " +
                "ISNULL(DepositTotal,0)-ISNULL(ContributionTotal,0)  AS Balance FROM Contributors c " +
                "LEFT JOIN (SELECT d.ContributorId, SUM(d.Amount) AS DepositTotal FROM Deposits d GROUP BY d.ContributorId) AS d " +
                "ON c.Id = d.ContributorId " +
                "LEFT JOIN (SELECT cn.ContributorId, SUM(cn.Amount) AS ContributionTotal FROM Contributions cn GROUP BY cn.ContributorId) AS cn " +
                "ON c.Id = cn.ContributorId " +
                "GROUP BY c.Id, c.FirstName,c.LastName, c.Cell, c.AlwaysInclude, d.DepositTotal, cn.ContributionTotal " +
                "ORDER BY c.LastName";
            connection.Open();
            var reader = command.ExecuteReader();
            var contributors = new List<Contributor>();
            while (reader.Read())
            {
                var contributor = new Contributor
                {
                    Id = (int)reader["Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    Cell = (string)reader["Cell"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"],
                    Balance = (decimal)reader["Balance"]
                };
                contributors.Add(contributor);
            }
            return contributors;
        }
        public void AddContributor(string firstName, string lastName, string cell, bool alwaysInclude)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Contributors (FirstName, LastName, Cell, AlwaysInclude) " +
                "VALUES (@firstname, @lastname, @cell, @alwaysinclude)";
            command.Parameters.AddWithValue("@firstname", firstName);
            command.Parameters.AddWithValue("@lastname", lastName);
            command.Parameters.AddWithValue("@cell", cell);
            command.Parameters.AddWithValue("@alwaysinclude", alwaysInclude);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
        public void UpdateContributor(int contributorId, string firstName, string lastName, string cell, bool alwaysInclude)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "UPDATE Contributors SET FirstName = @firstName, LastName = @lastName, Cell=@cell,AlwaysInclude=@alwaysInclude " +
                "WHERE Id=@contributorId";
            command.Parameters.AddWithValue("@firstName", firstName);
            command.Parameters.AddWithValue("@lastName", lastName);
            command.Parameters.AddWithValue("@cell", cell);
            command.Parameters.AddWithValue("@alwaysInclude", alwaysInclude);
            command.Parameters.AddWithValue("@contributorId", contributorId);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
        public List<Deposit> GetDepositsForId(int contributorId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Deposits WHERE ContributorId=@contributorId";
            command.Parameters.AddWithValue("@contributorId", contributorId);
            connection.Open();
            var reader = command.ExecuteReader();
            var deposits = new List<Deposit>();
            while (reader.Read())
            {
                var deposit = new Deposit
                {
                    Id = (int)reader["Id"],
                    ContributorId = (int)reader["ContributorId"],
                    Date = (DateTime)reader["Date"],
                    Amount = (decimal)reader["Amount"]
                };
                deposits.Add(deposit);
            }
            return deposits;
        }
        public List<Contribution> GetContributionsForId(int contributorId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Contributions WHERE ContributorId=@contributorId";
            command.Parameters.AddWithValue("@contributorId", contributorId);
            connection.Open();
            var reader = command.ExecuteReader();
            var deposits = new List<Contribution>();
            while (reader.Read())
            {
                var deposit = new Contribution
                {
                    SimchaId = (int)reader["SimchaId"],
                    ContributorId = (int)reader["ContributorId"],
                    Amount = (decimal)reader["Amount"],
                    Date = (DateTime)reader["Date"]
                };
                deposits.Add(deposit);
            }
            return deposits;
        }
        public string GetFullNameForId(int contributorId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT FirstName, LastName FROM Contributors WHERE Id=@contributorId";
            command.Parameters.AddWithValue("@contributorId", contributorId);
            connection.Open();
            var reader = command.ExecuteReader();
            string firstName = "";
            string lastName = "";
            while (reader.Read())
            {
                firstName = (string)reader["FirstName"];
                lastName = (string)reader["LastName"];
            }
            return $"{firstName} {lastName}";
        }
        public decimal GetBalanceForId(int contributorId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT c.Id, ISNULL(d.DepositTotal,0) AS DepositTotal,ISNULL(cn.ContributionTotal,0) AS ContributionTotal, " +
                "ISNULL(DepositTotal,0) - ISNULL(ContributionTotal,0) AS Balance FROM Contributors c " +
                "LEFT JOIN (SELECT d.ContributorId, SUM(d.Amount) AS DepositTotal FROM Deposits d GROUP BY d.ContributorId) AS d " +
                "ON c.Id = d.ContributorId " +
                "LEFT JOIN (SELECT cn.ContributorId, SUM(cn.Amount) AS ContributionTotal FROM Contributions cn " +
                "GROUP BY cn.ContributorId) AS cn " +
                "ON c.Id = cn.ContributorId " +
                "WHERE Id=@id " +
                "GROUP BY c.id, d.DepositTotal, cn.ContributionTotal";
            command.Parameters.AddWithValue("@id", contributorId);
            connection.Open();
            var reader = command.ExecuteReader();
            decimal balance = 0;
            while (reader.Read())
            {
                balance = (decimal)reader["Balance"];
            }
            return balance;
        }
        public string GetSimchaNameById(int simchaId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT s.Name, c.SimchaId FROM Contributions c " +
                "JOIN Simchas s " +
                "ON s.Id = c.SimchaId " +
                "WHERE s.Id= @id " +
                "GROUP BY s.Name, c.SimchaId";
            command.Parameters.AddWithValue("@id", simchaId);
            connection.Open();
            var reader = command.ExecuteReader();
            string name = "";
            while (reader.Read())
            {
                name = (string)reader["Name"];
            }
            return name;
        }
        public List<Contributor> GetContributorsForSimcha(int simchaId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT DISTINCT c.Id,c.FirstName,c.LastName,c.AlwaysInclude, s.Id AS Contribute, cn.ContributorId, " +
                "SUM(cn.Amount) AS Amount, ISNULL(d.DepositTotal,0)-ISNULL(cn2.ContributionTotal,0) AS Balance FROM  Contributors c " +
                "LEFT JOIN Contributions cn " +
                "ON c.Id=cn.ContributorId " +
                "LEFT JOIN Simchas s " +
                "ON s.Id = cn.SimchaId AND s.Id=@id " +
                "LEFT JOIN (SELECT d.ContributorId, SUM(d.Amount) AS DepositTotal FROM Deposits d GROUP BY d.ContributorId) AS d " +
                "ON c.Id = d.ContributorId " +
                "LEFT JOIN (SELECT cn.ContributorId, SUM(cn.Amount) AS ContributionTotal FROM Contributions cn GROUP BY cn.ContributorId) AS cn2 " +
                "ON c.Id = cn2.ContributorId " +
                "GROUP BY c.Id,c.FirstName,c.LastName,c.AlwaysInclude, s.Id, cn.ContributorId, d.DepositTotal,cn2.ContributionTotal " +
                "ORDER BY LastName";
            command.Parameters.AddWithValue("@id", simchaId);
            connection.Open();
            var contributors = new List<Contributor>();
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var contributor = new Contributor
                {
                    Id = (int)reader["Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"],
                    Balance = (decimal)reader["Balance"]
                };
                if (!DBNull.Value.Equals(reader["Contribute"]))
                {
                    contributor.Contribute = true;
                }
                else
                {
                    contributor.Contribute = false;
                }
                if (!DBNull.Value.Equals(reader["Amount"]))
                {
                    contributor.Amount = (decimal)reader["Amount"];
                }
                else
                {
                    contributor.Amount = 5;
                }
                contributors.Add(contributor);
            }
            return contributors;
        }
        public void AddContributionsForSimcha(int simchaId, int contributorId, decimal amount)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Contributions (SimchaId,ContributorId, Amount,Date) " +
                "VALUES(@simchaId, @contributorId, @amount,CURRENT_TIMESTAMP)";
            command.Parameters.AddWithValue("@simchaId", simchaId);
            command.Parameters.AddWithValue("@contributorId", contributorId);
            command.Parameters.AddWithValue("@amount", amount);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
        public void DeleteContributionsForSimchaId(int simchaId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Contributions WHERE SimchaId = @simchaId";
            command.Parameters.AddWithValue("@simchaId", simchaId);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
    }

}