using Common.Models;
using Microsoft.AspNetCore.Mvc;
using OT.Assessment.App.BusinessLogic.Interfaces;
using OT.Assessment.App.Models;
using Serilog;
namespace OT.Assessment.App.Controllers
{
  
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly IQueueManager _queueManager;
        private readonly IPlayerWagerService _playerService;
        public PlayerController(IQueueManager queueManager, IPlayerWagerService playerService) 
        {
            _queueManager = queueManager;
            _playerService = playerService;
        }
        //POST api/player/casinowager
        [HttpPost("casinowager")]
        public async Task<ActionResult> CasinoWager([FromBody] CasinoWagerRequest request)
        {
            try
            {
                await _queueManager.PublishPlayerWager(request);
                return Ok("Wager captured successfully");
            }
            catch (Exception ex)
            {
                Log.Error("An error occured while attempting to capture the wager, with the following exception: {ex}", ex);
                throw;
            }
        }
        //GET api/player/{playerId}/wagers
        [HttpGet("{playerId}/casino/{pageSize?}/{page?}")]
        public async Task<CasinoWagerResponse> Get(string playerId, [FromQuery] int pageSize, [FromQuery] int page)
        {
            try
            {
                return await _playerService.GetCasinoWagersByPlayer(playerId, pageSize, page);
            }
            catch (Exception ex)
            {
                Log.Error("An error occured while attempting to retrieve Player wagers, exception: {ex}", ex);
                throw;
            }
        }

        //GET api/player/topSpenders?count=10
        [HttpGet("topSpenders/{count?}")]
        public async Task<List<PlayerSpendings>> Get([FromQuery] int count)
        {
            try
            {
                return await _playerService.GetTopSpenders(count);
            }
            catch (Exception ex)
            {
                Log.Error("An error occured while attempting to get top spenders, exception: {ex}", ex);
                throw;
            }
        }
    }
}
