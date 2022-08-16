namespace Dapper_Order.Model
{
    public class OrderDetails
    {
        public int orderDetailsId { get; set; }
        public int orderId { get; set; }
        public int productId { get; set; }
        public int quantity { get; set; }
        public double totalAmount { get; set; }
    }
}
