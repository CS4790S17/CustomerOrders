using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using ViewModelTemplate.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure;
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



        /***** Use EF to get the order details *****/
        public List<OrderDetails> getOrderDetailsEF(string ordNo) {
            List<OrderDetails> orderDetails = new List<OrderDetails>();
            OrderEntryDbContext db = new OrderEntryDbContext();
            try
            {
                var query = (from ol in db.orderLines where ol.OrdNo == ordNo select ol);
                List<OrdLine> orderLines = query.ToList();

                foreach (OrdLine ordLine in orderLines)
                {
                    string prodNo = ordLine.ProdNo;

                    var query2 = (from p in db.products where p.ProdNo == prodNo select p);
                    Product theProduct = query2.ToList().First();

                    orderDetails.Add(new OrderDetails() {
                        ordNo = ordNo,
                        prodNo = ordLine.ProdNo,
                        product = theProduct,
                        quantity = (int)ordLine.Qty
                    });
                }
            } catch (Exception ex) { Console.WriteLine(ex.Message); }

            return orderDetails;
        }


        /***** Use SQL to get the Order Details *****/
        public List<OrderDetails> getOrderDetailsSQL(string ordNo) {

            //http://stackoverflow.com/questions/23153260/entity-framework-the-sqlparameter-is-already-contained-by-another-sqlparameter
            //**SqlQuery does not return a query result until you use a linq extension like any(), tolist().....on the other hand when you 
            //    use SqlQuery, the result is an IEnumerable when you use any(), tolist(), first() it's converted to a result


           List <OrderDetails> orderDetails = new List<OrderDetails>();
            OrderEntryDbContext db = new OrderEntryDbContext();
            List<SqlParameter> sqlParams = new List<SqlParameter>();

            sqlParams.Add(new SqlParameter("@OrdNo", ordNo));

            try {
                string sql = "SELECT * FROM OrdLine WHERE OrdNo = @OrdNo";
                List<OrdLine> orderLines =  db.orderLines.SqlQuery(sql, sqlParams.ToArray()).ToList();

                foreach (OrdLine ordLine in orderLines)
                {
                    string prodNo = ordLine.ProdNo;

                    sqlParams.Clear();
                    sqlParams.Add(new SqlParameter("@ProdNo", prodNo));

                    sql = "SELECT * FROM Product WHERE ProdNo = @ProdNo";
                    Product theProduct = db.products.SqlQuery(sql, sqlParams.ToArray()).First();

                    orderDetails.Add(new OrderDetails()
                    {
                        ordNo = ordNo,
                        prodNo = ordLine.ProdNo,
                        product = theProduct,
                        quantity = (int)ordLine.Qty
                    });
                }
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
        }

        [Key]
        public string custNo { get; set; }
        public Customer customer { get; set; }
        public List<OrderTbl> orders { get; set; }
    }


    public class OrderDetails {
        public OrderDetails() {
            this.product = new Product();
        }

        public string ordNo { get; set; }
        public string prodNo { get; set; }

        public int quantity { get; set; }
        public Product product { get; set; }
    }

}