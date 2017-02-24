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

        /***** Use SQL to get the customer orders *****/
        public OrderDetails getOrderDetailsSQL(string ordNo)
        {
            OrderDetails orderDetails = new OrderDetails();
            OrderEntryDbContext db = new OrderEntryDbContext();
            List<SqlParameter> sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@OrdNo", ordNo));

            try
            {
                string sql = "SELECT * FROM Order WHERE OrdNo = @OrdNo";
                orderDetails.order =
                    db.orders.SqlQuery(sql, sqlParams.ToArray()).First();

                sqlParams.Clear();
                sqlParams.Add(new SqlParameter("@OrdNo", ordNo));
                sql = @"SELECT ot.OrdNo, p.ProdName, ol.Qty, p.ProdPrice 
                        FROM OrderTbl AS ot 
                        INNER JOIN OrdLine AS ol 
                        ON ot.OrdNo = ol.OrdNo 
                        INNER JOIN Product AS p 
                        ON ol.ProdNo = p.ProdNo 
                        WHERE ot.OrdNo = = @OrdNo";
                orderDetails.orderItems =
                    db.Database.SqlQuery<OrderDetails.OrderItem>(sql, sqlParams.ToArray()).ToList();
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
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
        }

        [Key]
        public string custNo { get; set; }
        public Customer customer { get; set; }
        public List<OrderTbl> orders { get; set; }
    }


    public class OrderDetails
    {
        public OrderTbl order { get; set; }
        public List<OrderItem> orderItems { get; set; }

        public class OrderItem
        {
            public string ProdName { get; set; }
            public int? Qty { get; set; }
            public decimal? ProdPrice { get; set; }
        }
    }

}