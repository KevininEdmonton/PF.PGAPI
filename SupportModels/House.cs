namespace PFAPI.SupportModels
{
    public class House

    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string? Photo { get; init; }
    }

    public class Bid
    {
        public int Id { get; set; }
        public int HouseId { get; set; }
        public string Bidder { get; set; }
        public decimal Amount { get; set; }
    }
}
