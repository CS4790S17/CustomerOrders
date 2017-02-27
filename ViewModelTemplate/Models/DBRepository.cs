using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using ViewModelTemplate.Models;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace ViewModelTemplate.Models
{
    public class DBRepository
    {
      /*************  Repository Methods ******************/

        public List<Customer> getCustomers()
        {
            OrderEntryDbContext db = new OrderEntryDbContext();
            List<Customer> customers = new List<Customer>();
            try
            {
                customers = db.customers.ToList();
            } catch (Exception ex)
            { Console.WriteLine(ex.Message); }
            return customers;
        }

        /***** Use EF to get the customer orders *****/
        public CustomerOrders getCustomerOrdersEF(string custNo)
        {
            CustomerOrders customerOrders = new CustomerOrders();
            OrderEntryDbContext db = new OrderEntryDbContext();
            try
            {
                customerOrders.customer = db.customers.Find(custNo);
                var query = (from ot in db.orders where ot.CustNo == custNo select ot);
                customerOrders.orders = query.ToList();
                customerOrders.orderDetails = getOrderDetailsEF(custNo);
            } catch (Exception ex) { Console.WriteLine(ex.Message); }

            return customerOrders;
        }

        /***** Use SQL to get the customer orders *****/
        public CustomerOrders getCustomerOrdersSQL(string custNo)
        {
            CustomerOrders customerOrders = new CustomerOrders();
            OrderEntryDbContext db = new OrderEntryDbContext();
            List<SqlParameter> sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@CustNo", custNo));

            try
            {
                string sql = "SELECT * FROM Customer WHERE CustNo = @CustNo";
                customerOrders.customer =
                    db.customers.SqlQuery(sql, sqlParams.ToArray()).First();

                sqlParams.Clear();
                sqlParams.Add(new SqlParameter("@CustNo", custNo));
                sql = "SELECT * FROM OrderTbl WHERE CustNo = @CustNo";
                customerOrders.orders =
                    db.orders.SqlQuery(sql, sqlParams.ToArray()).ToList();
            } catch (Exception ex) { Console.WriteLine(ex.Message); }
            return customerOrders;
        }

        /*
         * SELECT ot.OrdNo, p.ProdName, ol.Qty, p.ProdPrice
         * FROM OrderTbl ot
         * JOIN OrdLine ol ON ot.OrdNo = ol.OrdNo
         * JOIN Product p ON ol.ProdNo = p.ProdNo
         * WHERE CustNo = 'C0954327';
         */

        /***** Use EF to get the customer orders *****/
        public List<OrderDetail> getOrderDetailsEF(string custNo) {
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            OrderEntryDbContext db = new OrderEntryDbContext();
            try {
                var query = (
                    from ot in db.orders
                    join ol in db.orderLines on ot.OrdNo equals ol.OrdNo
                    join p in db.products on ol.ProdNo equals p.ProdNo
                    where ot.CustNo == custNo
                    select new { OrdNo = ot.OrdNo, ProdName = p.ProdName, Qty = ol.Qty, ProdPrice = p.ProdPrice }
                );

                foreach (var x in query) {
                    OrderDetail item = new OrderDetail();
                    item.OrdNo = x.OrdNo;
                    item.ProdName = x.ProdName;
                    item.Qty = x.Qty;
                    item.ProdPrice = x.ProdPrice;
                    orderDetails.Add(item);
                    Console.WriteLine(orderDetails.Count);
                }

                //orderDetails = query.ToList().Select(x => new OrderDetail { OrdNo = x.OrdNo, ProdName = x.ProdName, Qty = x.Qty, ProdPrice = x.ProdPrice });
                //orderDetails = query.ToList();
            } catch (Exception ex) { Console.WriteLine(ex.Message); }

            return orderDetails;
        }
    }

    /***************** View Models **********************/

    public class CustomerOrders
    {
        public CustomerOrders()
        {
            this.customer = new Customer();
            this.orders = new List<OrderTbl>();
            this.orderDetails = new List<OrderDetail>();
        }

        [Key]
        public string custNo { get; set; }
        public Customer customer { get; set; }
        public List<OrderTbl> orders { get; set; }
        public List<OrderDetail> orderDetails { get; set; }
    }

    public class OrderDetail {
        /*public OrderDetail(string ordNo, string prodName, int? qty, decimal? prodPrice) {
            OrdNo = ordNo;
            ProdName = prodName;
            Qty = qty;
            ProdPrice = prodPrice;
        }*/

        [Key]
        [Display(Name = "Order Number")]
        public string OrdNo { get; set; }

        [Display(Name = "Product")]
        public string ProdName { get; set; }

        [Display(Name = "Qty")]
        public int? Qty { get; set; }

        [Display(Name = "Price")]
        public decimal? ProdPrice { get; set; }
    }

}