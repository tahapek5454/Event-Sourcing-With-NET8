namespace Shared.Events
{
    public class PriceIncreasedEvent
    {
        public string ProductId { get; set; }
        public decimal IncrementPrice { get; set; }
    }
}
