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
            }
            catch (Exception ex)
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
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

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
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            return customerOrders;
        }


        //public OrderDetails getOrderDetails(string ordNo)
        //{
        //    OrderDetails orderDetails = new OrderDetails();
        //    OrderEntryDbContext db = new OrderEntryDbContext();
        //    List<SqlParameter> sqlParams = new List<SqlParameter>();
        //    sqlParams.Add(new SqlParameter("@ordNo", ordNo));

        //    try
        //    {
        //        string sql = "SELECT * FROM OrdLine WHERE ordNo = @ordNo";
        //        orderDetails.orderLine = db.orderLines.SqlQuery(sql, sqlParams.ToArray()).ToList();                

        //    }
        //    catch (Exception ex) { Console.WriteLine(ex.Message); }
        //    return orderDetails;
        //}
        
        public OrderDetails getOrderDetails(string ordNo)
        {
            OrderDetails orderDetails = new OrderDetails();

            OrderEntryDbContext db = new OrderEntryDbContext();
            List<SqlParameter> sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@OrdNo", ordNo));
            string sql = "SELECT o.OrdNo, l.ProdNo, p.ProdName, l.Qty, p.ProdPrice " +
            "FROM OrderTbl o INNER JOIN OrdLine l " +
            " ON o.OrdNo = l.OrdNo " +
            " INNER JOIN Product p " +
            " ON l.ProdNo = p.ProdNo " +
            "WHERE l.OrdNo = @OrdNo;";
            try
            {
                orderDetails.orderDetail =
                    db.orderDetails.SqlQuery(sql, sqlParams.ToArray()).ToList();
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
                //this.orderLine = new List<OrdLine>();
            }

            [Key]
            public string custNo { get; set; }
            public Customer customer { get; set; }
            public List<OrderTbl> orders { get; set; }
            //public List<OrdLine> orderLine { get; set; }
            //public string ordNo { get; set; }
        }

    public class OrderDetails
    {
        public OrderDetails()
        {
            this.orderLine = new List<OrdLine>();
            this.products = new List<Product>();
            this.orderDetail = new List<OrderDetail>();
        }

        public List<OrdLine> orderLine { get; set; }
        public List<Product> products { get; set; }
        public List<OrderDetail> orderDetail { get; set; }
       


    }  

       

    }
