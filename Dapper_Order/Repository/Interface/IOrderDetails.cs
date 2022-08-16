using Dapper_Order.Model;

namespace Dapper_Order.Repository.Interface
{
    public interface IOrderDetails
    {
        public Task<IEnumerable<Order>> GetOrders();

        public Task<Order> GetOrderById(int id);

        public Task<int> Insert(Order order);

        public Task<int> Update(Order order);

        public Task<int> Delete(int id);
    }
}
