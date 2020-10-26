using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace purchaseOrderProgram2
{
    public class PurchaseOrder
    {
        public int poPK { get; }
        public int poPayment { get; }
        public string poPaymentName { get; }
        public string poCreator { get; }
        public decimal poTotal { get; }
        public DateTime poDateCreated { get; }
        public DateTime? poDateOrdered { get; }
        public int poVendor { get; }
        public string poVendorName { get; }
        public string poVendorNumb { get; }
        public DateTime? poDateSubmitedForReconciliation { get; }
        public string poComments { get; }
        public bool poActive { get; }
        public List<int> lineItemPKs { get; }
        public List<PurchaseOrderLineItem> lineItems { get; }

        public PurchaseOrder(int PK)
        {
            poPK = PK;
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
            SqlCommand getPurchaseOrder = new SqlCommand("SELECT poPayment, poCreator, poTotal, poDateCreated, poDateOrdered, poVendor, poVendorPoNumb, poDateSubmitedForReconciliation, poComments, poActive FROM PurchaseOrders WHERE poPK = @poPK;", conn);
            getPurchaseOrder.Parameters.AddWithValue("@poPK", PK);

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                SqlDataReader readPo = getPurchaseOrder.ExecuteReader();

                while (readPo.Read())
                {
                    poPayment = readPo.GetInt32(0);
                    poCreator = readPo.GetString(1);
                    poTotal = readPo.GetDecimal(2);
                    poDateCreated = readPo.GetDateTime(3);

                    if (readPo.IsDBNull(4))
                        poDateOrdered = null;
                    else
                        poDateOrdered = readPo.GetDateTime(4);

                    poVendor = readPo.GetInt32(5);

                    if (readPo.IsDBNull(6))
                        poVendorNumb = null;
                    else
                        poVendorNumb = readPo.GetString(6);

                    if (readPo.IsDBNull(7))
                        poDateSubmitedForReconciliation = null;
                    else
                        poDateSubmitedForReconciliation = readPo.GetDateTime(7);

                    if (readPo.IsDBNull(8))
                        poComments = null;
                    else
                        poComments = readPo.GetString(8);

                    poActive = readPo.GetBoolean(9);
                }

                conn.Close();
            }

            poPaymentName = getPaymentNameFromPK(poPayment);
            poVendorName = getVendorNameFromPK(poVendor);
            lineItemPKs = getLineItemPKsFrompoPK(PK);
            lineItems = new List<PurchaseOrderLineItem>();

            foreach (int lineItemPK in lineItemPKs)
            {
                lineItems.Add(new PurchaseOrderLineItem(lineItemPK));
            }
        }

        private string getPaymentNameFromPK(int pk)
        {
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
            SqlCommand getPaymentName = new SqlCommand("SELECT paymentName FROM PaymentSource WHERE paymentPK = @paymentPK;", conn);
            getPaymentName.Parameters.AddWithValue("@paymentPK", pk);
            string returnMe = string.Empty;

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                SqlDataReader readPaymentName = getPaymentName.ExecuteReader();

                while (readPaymentName.Read())
                {
                    returnMe = readPaymentName.GetString(0);
                }
                conn.Close();
            }

            return returnMe;
        }

        private string getVendorNameFromPK(int pk)
        {
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
            SqlCommand getVendorName = new SqlCommand("SELECT vendorName FROM Vendors WHERE vendorPK = @vendorPK;", conn);
            getVendorName.Parameters.AddWithValue("@vendorPK", pk);
            string returnMe = string.Empty;

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                SqlDataReader readVendorName = getVendorName.ExecuteReader();

                while (readVendorName.Read())
                {
                    returnMe = readVendorName.GetString(0);
                }
                conn.Close();
            }

            return returnMe;
        }

        private List<int> getLineItemPKsFrompoPK(int pk)
        {
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
            SqlCommand getLineItemPks = new SqlCommand("SELECT lineitemPK FROM PurchaseOrderLineItems WHERE lineitemPoPK = @pk;", conn);
            getLineItemPks.Parameters.AddWithValue("@pk", pk);
            List<int> returnMe = new List<int>();

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                SqlDataReader readLineItemPks = getLineItemPks.ExecuteReader();

                while (readLineItemPks.Read())
                {
                    returnMe.Add(readLineItemPks.GetInt32(0));
                }
                conn.Close();
            }

            return returnMe;
        }
    }
}
public class PurchaseOrderLineItem
{
    public int lineitemPK { get; }
    public int lineitemPoPk { get; }
    public int lineitemDepartment { get; }
    public int lineitemGrant { get; }
    public string lineitemDescription { get; set; }
    public decimal lineitemUnits { get; set; }
    public decimal lineitemUnitPrice { get; set; }
    public decimal lineitemTotalPrice { get; set; }
    public string lineitemDepartmentName { get; }
    public string lineitemGrantName { get; }

    public PurchaseOrderLineItem(int aLineItemPK)
    {
        lineitemPK = aLineItemPK;
        SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
        SqlCommand getLineItem = new SqlCommand("SELECT lineitemPoPK, lineitemDepartment, lineitemGrant, lineitemDescription, lineitemUnits, lineitemUnitPrice, lineitemTotalPrice FROM PurchaseOrderLineItems WHERE lineitemPK = @lineitemPK;", conn);
        getLineItem.Parameters.AddWithValue("@lineitemPK", aLineItemPK);

        if (conn.State == ConnectionState.Closed)
        {
            conn.Open();
            SqlDataReader readLineItem = getLineItem.ExecuteReader();

            while (readLineItem.Read())
            {
                lineitemPoPk = readLineItem.GetInt32(0);
                lineitemDepartment = readLineItem.GetInt32(1);
                lineitemGrant = readLineItem.GetInt32(2);
                lineitemDescription = readLineItem.GetString(3);
                lineitemUnits = readLineItem.GetDecimal(4);
                lineitemUnitPrice = readLineItem.GetDecimal(5);
                lineitemTotalPrice = readLineItem.GetDecimal(6);
            }

            lineitemDepartmentName = getDepartmentNameFromDepartmentPK(lineitemDepartment);
            lineitemGrantName = getGrantNameFromGrantPK(lineitemGrant);
        }
    }

    private string getDepartmentNameFromDepartmentPK(int dPK)
    {
        string returnMe = string.Empty;
        SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
        SqlCommand getName = new SqlCommand("GetDeparmentNameFromDepartmentPK", conn);
        getName.CommandType = CommandType.StoredProcedure;
        getName.Parameters.AddWithValue("@pk", dPK);

        if (conn.State == ConnectionState.Closed)
        {
            conn.Open();
            SqlDataReader readName = getName.ExecuteReader();

            while (readName.Read())
            {
                returnMe = readName.GetString(0);
            }

            conn.Close();
        }

        return returnMe;
    }

    private string getGrantNameFromGrantPK(int gPK)
    {
        SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
        SqlCommand getGrantName = new SqlCommand("SELECT grantName FROM Grants WHERE grantPK = @grantPK;", conn);
        getGrantName.Parameters.AddWithValue("@grantPK", gPK);
        string returnMe = string.Empty;

        if (conn.State == ConnectionState.Closed)
        {
            conn.Open();
            SqlDataReader readGrantName = getGrantName.ExecuteReader();

            while (readGrantName.Read())
            {
                returnMe = readGrantName.GetString(0);
            }
            conn.Close();
        }

        return returnMe;
    }
}
