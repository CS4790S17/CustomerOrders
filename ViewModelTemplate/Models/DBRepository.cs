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

                //for each order on for the customer
                foreach(var order in customerOrders.orders)
                {
                    //get their the details of each order
                    sqlParams.Clear();
                    sqlParams.Add(new SqlParameter("@OrdNo", order.OrdNo));
                    sql = "SELECT o.OrdNo, l.ProdNo, p.ProdName, l.Qty, p.ProdPrice " +
                          "FROM OrderTbl o " +
                             "INNER JOIN OrdLine l " +
                                "ON o.OrdNo = l.OrdNo " +
                             "INNER JOIN Product p " +
                                "ON l.ProdNo = p.ProdNo " +
                          "WHERE l.OrdNo = @OrdNo";
                    List<OrderDetail> results = db.details.SqlQuery(sql, sqlParams.ToArray()).ToList();
                    
                    //and add each one to the entire list of details for that customer
                    foreach(var item in results)
                    {
                        customerOrders.details.Add(item);
                    }
                }
            } catch (Exception ex) { Console.WriteLine(ex.Message); }
            return customerOrders;
        }
    }
    /***************** View Models **********************/

    public class CustomerOrders
    {
        public CustomerOrders()
        {
            this.customer = new Customer();
            this.orders = new List<OrderTbl>();
            this.details = new List<OrderDetail>();
        }

        [Key]
        public string custNo { get; set; }
        public Customer customer { get; set; }
        public List<OrderTbl> orders { get; set; }
        public List<OrderDetail> details { get; set; }
    }

    public class OrderDetail
    {
        public OrderDetail()
        {
            OrdNo = "";
            ProdNo = "";
            ProdName = "";
            Qty = 0;
            ProdPrice = 0;
    }
        [Key]
        [Display(Name ="Order Number")]
        public string OrdNo { get; set; }
        [Display(Name = "Product Number")]
        public string ProdNo { get; set; }
        [Display(Name = "Product")]
        public string ProdName { get; set; }
        [Display(Name = "Quantity")]
        public int Qty { get; set; }
        [Display(Name = "Price")]
        public decimal ProdPrice { get; set; }
    }
}