using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModelTemplate.Models;

namespace ViewModelTemplate.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Customer
        public ActionResult customer()
        {
            DBRepository dbr = new DBRepository();
            List<Customer> customers = dbr.getCustomers();
            return View("customer",customers);
        }

        public ActionResult orders(string id)
        {
            DBRepository dbr = new DBRepository();
            CustomerOrders customerOrders = dbr.getCustomerOrdersEF(id);
            /*** or use SQL 
            CustomerOrders customerOrders = dbr.getCustomerOrdersSQL(id);
            /***/

            return View(customerOrders);
        }

        public ActionResult getOrderDetails(String ordNo)
        {
            DBRepository dbr = new DBRepository();
            List <OrderDetails> orderDetails = dbr.getOrderDetailsSQL(ordNo);


            //String message = "Product No: " + orderDetails[0].prodNo + ", Product Name: " + orderDetails[0].product.ProdName;
            //return PartialView("_orderDetails", message);
            return PartialView("_orderDetails", orderDetails);

        }

    }
}
