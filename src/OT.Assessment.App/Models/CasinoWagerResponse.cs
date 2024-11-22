namespace OT.Assessment.App.Models
{
    public class CasinoWagerResponse
    {
        public List<PlayerWager> data { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
        public int total { get; set; }
        public int totalPages { get; set; }
    }
}
