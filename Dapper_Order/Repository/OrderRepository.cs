using Dapper;
using Dapper_Order.Context;
using Dapper_Order.Model;
using Dapper_Order.Repository.Interface;

namespace Dapper_Order.Repository
{
    public class OrderRepository: IOrderDetails
    {
        private readonly Dappercontext _context;
        public OrderRepository(Dappercontext context)
        {
            _context = context;
        }

        public async Task<Order> GetOrderById(int id)
        {
            Order ord = new Order();

            var query = "Select * from Order_Table where orderId=@Id";
            using (var connection = _context.CreateConnection())
            {
                var raworder = await connection.QueryAsync<Order>(query, new { Id = id });
                ord = raworder.FirstOrDefault();
                if (ord != null)
                {

                    var orderdetailsrow = await connection.QueryAsync<OrderDetails>("select * from order_details_table where orderid=@Id", new { Id = id });
                    ord.orderDetailsList = orderdetailsrow.ToList();
                }
                return ord;
            }
        }

        public async Task<IEnumerable<Order>> GetOrders()
        {
            List<Order> odlist = new List<Order>();
            var query = "SELECT * FROM Order_Table";
            using (var connection = _context.CreateConnection())
            {
                var raworders = await connection.QueryAsync<Order>(query);
                odlist = raworders.ToList();
                foreach (var order in odlist)
                {
                    var res = await connection.QueryAsync<OrderDetails>(@"select * from order_details_table
                                                                        where orderid=@Id", new { Id = order.orderId });
                    order.orderDetailsList = res.ToList();
                }

                return odlist;
            }
        }

        public async Task<int> Insert(Order order)
        {
            double ret = 0;
            var query = @"Insert into Order_Table(invoiceno,customername,mobileno,shippingaddress,
                           billingaddress,totalorderamount) values (@invoiceNo,@customerName,@mobileNo,
                           @shippingAddress,@billingAddress,@totalOrderAmount);SELECT CAST(SCOPE_IDENTITY() as int)";

            List<OrderDetails> odlist = new List<OrderDetails>();
            odlist = order.orderDetailsList.ToList();


            using (var connection = _context.CreateConnection())
            {

                {
                    int result = await connection.QuerySingleAsync<int>(query, order);
                    if (result != 0)
                    {
                        ret = await InsertUpdateOrder(odlist, result);
                        order.totalOrderAmount = ret;

                    }

                }


            }
            return Convert.ToInt32(ret);

        }

        public async Task<double> InsertUpdateOrder(List<OrderDetails> odlist, int result)
        {
            int res = 0;
            double grandtotal = 0;
            if (result != 0)
            {
                using (var connection = _context.CreateConnection())
                {
                    foreach (OrderDetails od in odlist)
                    {
                        od.orderId = result;
                        var pquery = "select productprice from MProduct_Table where productid = @pid";
                        var resprice = await connection.QuerySingleAsync<int>(pquery, new { pid = od.productId });
                        od.totalAmount = resprice * od.quantity;
                        var qry = @"Insert into Order_Details_Table(orderid,
                                productid,quantity,totalamount ) values (@orderId,
                                @productId,@quantity,@totalAmount) ";

                        var result1 = await connection.ExecuteAsync(qry, od);

                        res = res + result1;
                        grandtotal = grandtotal + od.totalAmount;
                    }
                }
            }
            return grandtotal;
        }

        public async Task<int> Update(Order order)
        {
            double ret;
            var query = @"Update Order_Table set invoiceno=@invoiceNo,customername=@customerName,mobileno=@mobileNo,
                           shippingaddress=@shippingAddress,billingaddress=@billingAddress,
                            totalorderamount=@totalOrderAmount where orderId=@orderId ";
            using (var connection = _context.CreateConnection())
            {

                var result = await connection.ExecuteAsync(query, order);
                ret = await InsertUpdateOrder(order.orderDetailsList, order.orderId);
                order.totalOrderAmount = ret;

                return result;

            }
        }

        public async Task<int> Delete(int id)
        {
            var query = @"Delete from order_table where orderid=@Id
                          Delete from order_details_Table where orderid=@Id";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, new { Id = id });
                return result;
            }
        }

    }
}
