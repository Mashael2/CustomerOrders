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

        public List<OrderDetail> getOrderDetails(string ordNo)
        {
            OrderEntryDbContext db = new OrderEntryDbContext();
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            List<SqlParameter> sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@OrdNo", ordNo));
            //query for customer orders
            string sql =
                "SELECT ot.OrdNo, ol.ProdNo, p.ProdName, p.ProdPrice, ol.Qty " +
                "FROM OrderTbl ot INNER JOIN OrdLine ol " +
                " ON ot.OrdNo = ol.OrdNo " +
                " INNER JOIN Product p " +
                " ON ol.ProdNo = p.ProdNo " +
                "WHERE l.OrdNo = @OrdNo;";
            try
            {
                orderDetails = db.orderDetails.SqlQuery(sql,sqlParams.ToArray()).ToList();
            } catch (Exception ex)
            { Console.WriteLine(ex.Message); }

            return orderDetails;

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
    }
    /***************** View Models **********************/

    public class CustomerOrders
    {
        public CustomerOrders()
        {
            this.customer = new Customer();
            this.orders = new List<OrderTbl>();
            this.products = new List<Product>();
            this.orderLines = new List<OrdLine>();
        }

        [Key]
        public string custNo { get; set; }
        public Customer customer { get; set; }
        public List<OrderTbl> orders { get; set; }
        public List<Product> products { get; set; }
        public List<OrdLine> orderLines { get; set; }
    }

}