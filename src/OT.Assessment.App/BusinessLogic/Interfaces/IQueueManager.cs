using Common.Models;

namespace OT.Assessment.App.BusinessLogic.Interfaces
{
    public interface IQueueManager
    {
        public Task PublishPlayerWager(CasinoWagerRequest request);
    }
}
