using System.Data;
using Common.Models;
using Serilog;

namespace OT.Assessment.Consumer.Repository
{
    public class SqlRepository
    {
        private readonly string _connectionString;
        public SqlRepository(string connectionString) { _connectionString = connectionString; }
        public async Task SaveWagerToDatabase(CasinoWagerRequest casinoWager)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.CommandText = "usp_InsertCasinoWager";
                        cmd.Parameters.AddWithValue("@AccountId", new Guid(casinoWager.AccountId));
                        cmd.Parameters.AddWithValue("@UserName", casinoWager.Username);
                        cmd.Parameters.AddWithValue("@Theme", casinoWager.Theme);
                        cmd.Parameters.AddWithValue("@Provider", casinoWager.Provider);
                        cmd.Parameters.AddWithValue("@GameName", casinoWager.GameName);
                        cmd.Parameters.AddWithValue("@CountryCode", casinoWager.CountryCode);
                        cmd.Parameters.AddWithValue("@Amount", casinoWager.Amount);
                        cmd.Parameters.AddWithValue("@NumberOfBets", casinoWager.NumberOfBets);
                        cmd.Parameters.AddWithValue("@BrandId", new Guid(casinoWager.BrandId));
                        cmd.Parameters.AddWithValue("@WagerId", new Guid(casinoWager.WagerId));
                        cmd.Parameters.AddWithValue("@TransactionId", new Guid(casinoWager.TransactionId));
                        cmd.Parameters.AddWithValue("@TransactionTypeId", new Guid(casinoWager.TransactionTypeId));
                        cmd.Parameters.AddWithValue("@CreatedDateTime", casinoWager.CreatedDateTime);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error saving Casino wager to database: {ex}", ex);
                throw;
            }
        }
    }
}
