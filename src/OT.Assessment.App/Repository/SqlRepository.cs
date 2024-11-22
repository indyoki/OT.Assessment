using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using OT.Assessment.App.Models;
using OT.Assessment.App.Repository.Interfaces;
using Serilog;

namespace OT.Assessment.App.Repository
{
    public class SqlRepository : ISqlRepository
    {
        private readonly IConfiguration _config;
        public SqlRepository(IConfiguration config)
        {
            _config = config;
        }

        public async Task<List<PlayerWager>> GetCasinoWagersByAccountId(string accountId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_config.GetConnectionString("DatabaseConnection")))
                {
                    var playerWagers = new List<PlayerWager>();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.CommandText = "usp_GetCasinoWagersByAccountId";
                        cmd.Parameters.AddWithValue("@AccountId", new Guid(accountId));
                        con.Open();

                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                playerWagers.Add(new PlayerWager
                                {
                                    WagerId = Convert.ToString(sdr["WagerId"]),
                                    Game = Convert.ToString(sdr["GameName"]),
                                    Provider = Convert.ToString(sdr["Provider"]),
                                    Amount = Convert.ToDouble(sdr["Amount"]),
                                    CreatedDateTime = Convert.ToDateTime(sdr["CreatedDateTime"])
                                });
                            }
                        }
                        await con.CloseAsync();
                    }
                    return playerWagers;
                }
            }
            catch (SqlException ex)
            {
                Log.Error("Error occured while retrieving data from database, Exception: {ex}", ex);
                throw;
            }
        }

        public async Task<List<PlayerSpendings>> GetTopSpenders(int count)
        {
            try
            {
                var topSpenders = new List<PlayerSpendings>();
                using (SqlConnection con = new SqlConnection(_config.GetConnectionString("DatabaseConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.CommandText = "usp_GetTopSpendingPlayers";
                        cmd.Parameters.AddWithValue("@Count", count);
                        con.Open();

                        using (SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            while (sdr.Read())
                            {
                                topSpenders.Add(new PlayerSpendings
                                {
                                    AccountId = new Guid(Convert.ToString(sdr["AccountId"])),
                                    Username = Convert.ToString(sdr["Username"]),
                                    TotalAmount = Convert.ToDouble(sdr["totalAmountSpend"])
                                });
                            }
                        }
                        await con.CloseAsync();
                    }
                }
                return topSpenders;
            }
            catch (Exception ex)
            {
                Log.Error("Error occured while retrieving data from database, Exception: {ex}", ex);
                throw;
            }
        }
    }
}
