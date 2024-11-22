using OT.Assessment.App.Models;

namespace OT.Assessment.App.BusinessLogic.Interfaces
{
    public interface IPlayerWagerService
    {
        public Task<CasinoWagerResponse> GetCasinoWagersByPlayer(string playertId, int pageSize, int page);
        public Task<List<PlayerSpendings>> GetTopSpenders(int count);
    }
}
