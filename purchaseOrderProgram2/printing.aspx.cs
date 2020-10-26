using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace purchaseOrderProgram2
{
    public partial class printing : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["POsToPrint"] == null)
            {
                nothingToPrintMessage.Visible = true;
            }
            else
            {
                displayPOs();
                //htmlBody.Attributes.on
            }
        }

        protected void displayPOs()
        {
            int lineNumb = 1;

            foreach(int pk in ((List<int>)Session["POsToPrint"]))
            {
                lineNumb = 1;
                PurchaseOrder thisPO = new PurchaseOrder(pk);
                Literal poDiv = new Literal();
                poDiv.Text = "<div>" +
                    "<div class=\"line\"><div class=\"label\">PO Number:</div><div class=\"value\">" + thisPO.poPK.ToString() + "</div></div>" +
                    "<div class=\"line\"><div class=\"label\">Created on:</div><div class=\"value\">" + thisPO.poDateCreated.ToString().Split(' ')[0] + "</div></div>" +
                    "<div class=\"line\"><div class=\"label\">By:</div><div class=\"value\">" + thisPO.poCreator + "</div></div>" +
                    "<div class=\"line\"><div class=\"label\">Vendor:</div><div class=\"value\">" + thisPO.poVendorName + "</div></div>" +
                    "<div class=\"line\"><div class=\"label\">Vendor PO Number:</div><div class=\"value\">" + thisPO.poVendorNumb + "</div></div>" +
                    "<div class=\"line\"><div class=\"label\">Paid for with:</div><div class=\"value\">" + thisPO.poPaymentName + "</div></div>" +
                    "<div class=\"line\"><div class=\"label\">Comments:</div><div class=\"value\">" + thisPO.poComments + "</div></div>" +
                    "<hr>";

                foreach (int lineItemPK in thisPO.lineItemPKs)
                {
                    PurchaseOrderLineItem thisLineItem = new PurchaseOrderLineItem(lineItemPK);
                    poDiv.Text += "<div class=\"line\">" + "<div class=\"lineNumber\">" + lineNumb + "</div>" +
                        "<div class=\"lineitemLabel\">Item Name:</div>" + "<div class=\"lineitemValue\">" + thisLineItem.lineitemDescription + "</div>" +
                        "<div class=\"lineitemLabel\">Quantity:</div>" + "<div class=\"lineitemValue\">" + thisLineItem.lineitemUnits.ToString("F2") + "</div>" +
                        "<div class=\"lineitemLabel\">Price Each:</div>$" + "<div class=\"lineitemValue\">" + thisLineItem.lineitemUnitPrice.ToString("F2") + "</div>" +
                        "</div>" +
                        "<div class=\"line\">" + "<div class=\"lineNumber\">&nbsp;</div>" +
                        "<div class=\"lineitemLabel\">Department:</div>" + "<div class=\"lineitemValue\">" + thisLineItem.lineitemDepartmentName + "</div>" +
                        "<div class=\"lineitemLabel\">Grant:</div>" + "<div class=\"lineitemValue\">" + thisLineItem.lineitemGrantName + "</div>" +
                        "</div><br>";
                    lineNumb++;
                }

                poDiv.Text += "<hr>" + 
                    "<div class=\"line\"><div class=\"label\">Total:" + "</div><div class=\"value\">" + thisPO.poTotal.ToString("F2") + "</div></div>" +
                    "</div><p class=\"pageBreak\">Klamath Health Partnership</p>";
                body.Controls.Add(poDiv);
            }
        }
    }
}