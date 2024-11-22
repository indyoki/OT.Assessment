using OT.Assessment.App.BusinessLogic.Interfaces;
using OT.Assessment.App.Models;
using OT.Assessment.App.Repository.Interfaces;
using Serilog;

namespace OT.Assessment.App.BusinessLogic
{
    public class PlayerWagerService : IPlayerWagerService
    {
        private readonly ISqlRepository _sqlRepository;
        public PlayerWagerService(ISqlRepository sqlRepository)
        {
            _sqlRepository = sqlRepository;
        }
        public async Task<CasinoWagerResponse> GetCasinoWagersByPlayer(string playerId, int pageSize, int page)
        {
            try
            {
                var results = await _sqlRepository.GetCasinoWagersByAccountId(playerId);

                if (results.Count == 0)
                    return new CasinoWagerResponse();

                int totalItems = results.Count;
                int pageCount = ( totalItems + pageSize - 1) / pageSize;

                results = results.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                return new CasinoWagerResponse { data = results, page = page, pageSize = pageSize, total = totalItems, totalPages = pageCount };
            }
            catch(Exception ex)
            {
                Log.Error("An error occured with the following exception: {ex}", ex);
                throw;
            }
        }

        public async Task<List<PlayerSpendings>> GetTopSpenders(int count)
        {
            try
            {
                var topSpenders = await _sqlRepository.GetTopSpenders(count);
                if (topSpenders.Count == 0)
                    return new List<PlayerSpendings>();

                return topSpenders;
            }
            catch(Exception ex)
            {
                Log.Error("An error occured with the following exception: {ex}", ex);
                throw;
            }
        }
    }
}
