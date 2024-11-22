using OT.Assessment.App.Models;

namespace OT.Assessment.App.Repository.Interfaces
{
    public interface ISqlRepository
    {
        public Task<List<PlayerWager>> GetCasinoWagersByAccountId(string accountId);
        public Task<List<PlayerSpendings>> GetTopSpenders(int count);
    }
}
