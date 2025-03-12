namespace ShopOnline.Models.StripeHelpers
{
    public class CheckoutRequest
    {
        public List<CheckoutItem> Items { get; set; }
        public int UserId { get; set; }
        public int Shipping { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
    }
}
