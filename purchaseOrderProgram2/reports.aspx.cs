using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using purchaseOrderProgram2.Models;
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
    public partial class reports : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["SortDir"] = "asc";
                fillDrops();
                endDateTextBox.Text = DateTime.Today.ToShortDateString();
            }

            if (ViewState["lineItems"] == null)
            {
                ViewState["lineItems"] = 0;
            }

            for (int i = 1; i <= (int)ViewState["lineItems"]; i++)
            {
                addLineItems(i);
            }
        }   

        protected void fillDrops()
        {
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
            SqlCommand fillDrop = new SqlCommand();
            fillDrop.Connection = conn;
            SqlDataReader read = null;

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                fillDrop.CommandText = "SELECT DISTINCT departmentName, departmentPK FROM Departments ORDER BY departmentName;";
                read = fillDrop.ExecuteReader();
                while (read.Read())
                    searchDepartmentDropDown.Items.Add(new ListItem(read.GetString(0), read.GetInt32(1).ToString()));
                searchDepartmentDropDown.Items.Add(new ListItem() { Selected = true, Text = "All Depts", Value = "all" });
                conn.Close();
            }

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                fillDrop.CommandText = "SELECT DISTINCT grantName, grantPK FROM Grants ORDER BY grantName;";
                read = fillDrop.ExecuteReader();
                while (read.Read())
                    searchGrantDropDown.Items.Add(new ListItem(read.GetString(0), read.GetInt32(1).ToString()));
                searchGrantDropDown.Items.Add(new ListItem() { Selected = true, Text = "All Grants", Value = "all" });
                conn.Close();
            }
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                fillDrop.CommandText = "SELECT DISTINCT paymentName, paymentPK FROM PaymentSource ORDER BY paymentName;";
                read = fillDrop.ExecuteReader();
                while (read.Read())
                    searchPaymentDropDown.Items.Add(new ListItem(read.GetString(0), read.GetInt32(1).ToString()));
                searchPaymentDropDown.Items.Add(new ListItem() { Selected = true, Text = "All Payment Sources", Value = "all" });
                conn.Close();
            }
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                fillDrop.CommandText = "SELECT DISTINCT vendorName, vendorPK FROM Vendors ORDER BY vendorName;";
                read = fillDrop.ExecuteReader();
                while (read.Read())
                    searchVendorDropDown.Items.Add(new ListItem(read.GetString(0), read.GetInt32(1).ToString()));
                searchVendorDropDown.Items.Add(new ListItem() { Selected = true, Text = "All Vendors", Value = "all" });
                conn.Close();
            }
        }

        protected List<Tuple<int, string, string, bool>> GetDepartmentInfo(int inactiveOrActiveOrBoth = 1)
        {
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
            SqlCommand getDepartmentInfo = new SqlCommand();
            getDepartmentInfo.Connection = conn;

            switch (inactiveOrActiveOrBoth)
            {
                case 0:
                    getDepartmentInfo.CommandText = "SELECT departmentPK, departmentName, departmentComments, departmentActive FROM Departments WHERE departmentActive = 0;";
                    break;
                case 2:
                    getDepartmentInfo.CommandText = "SELECT departmentPK, departmentName, departmentComments, departmentActive FROM Departments;";
                    break;
                case 1:
                default:
                    getDepartmentInfo.CommandText = "SELECT departmentPK, departmentName, departmentComments, departmentActive FROM Departments WHERE departmentActive = 1;";
                    break;
            }

            List<Tuple<int, string, string, bool>> departmentInfo = new List<Tuple<int, string, string, bool>>();
            Tuple<int, string, string, bool> addThis;

            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SqlDataReader readDepartmentInfo = getDepartmentInfo.ExecuteReader();
            while (readDepartmentInfo.Read())
            {
                addThis = new Tuple<int, string, string, bool>(readDepartmentInfo.GetInt32(0), readDepartmentInfo.GetString(1), readDepartmentInfo.GetString(2), readDepartmentInfo.GetBoolean(3));
                departmentInfo.Add(addThis);
            }

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return departmentInfo;
        }

        protected void fillDataGrid(List<PurchaseOrder> workingPOs)
        {
            Session["workingPOs"] = workingPOs;
            decimal total = 0;

            //Item1=poPk, Item2=poDateCreated, 
            //Item3=lineitemDescription, Item4=lineitemTotal, Item5=lineitemDepartmentName, 
            //Item6=poPaymentName, Item7=lineitemGrantName 
            List<Tuple<int, DateTime, string, decimal, string, string, string>> workingLineItems = new List<Tuple<int, DateTime, string, decimal, string, string, string>>();

            foreach(PurchaseOrder po in workingPOs)
            {
                foreach(PurchaseOrderLineItem lineitem in po.lineItems)
                {
                    workingLineItems.Add(new Tuple<int, DateTime, string, decimal, string, string, string>(po.poPK, po.poDateCreated, lineitem.lineitemDescription, lineitem.lineitemTotalPrice, lineitem.lineitemDepartmentName, po.poPaymentName, lineitem.lineitemGrantName));
                }
            }

            if (searchDepartmentDropDown.SelectedValue != "all")
                workingLineItems.RemoveAll(x => x.Item5 != searchDepartmentDropDown.SelectedItem.Text);
            if (searchGrantDropDown.SelectedValue != "all")
                workingLineItems.RemoveAll(x => x.Item7 != searchGrantDropDown.SelectedItem.Text);
            if (searchPaymentDropDown.SelectedValue != "all")
                workingLineItems.RemoveAll(x => x.Item6 != searchPaymentDropDown.SelectedItem.Text);

            total = workingLineItems.Sum(x => x.Item4);

            reportGridViewTotalPrice.Text = "Total $" + total.ToString("0.00");

            Session["workingLineItems"] = workingLineItems;
            reportGridView.DataSource = workingLineItems;
            reportGridView.DataKeyNames = new string[] { "Item1" };
            reportGridView.DataBind();
        }

        protected List<int> ReturnRawSearchResults(SqlCommand sqlCommand)
        {
            List<int> listOfPKResults = new List<int>();
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
            sqlCommand.Connection = conn;
            //SqlCommand getPKs = new SqlCommand();

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                SqlDataReader readPK = sqlCommand.ExecuteReader();

                while (readPK.Read())
                    listOfPKResults.Add(readPK.GetInt32(0));

                conn.Close();
            }

            return listOfPKResults;
        }

        public List<PurchaseOrder> UserAuthorizedToReviewThesePOs(List<int> poPKs)
        {
            List<PurchaseOrder> returnMe = new List<PurchaseOrder>();
            PurchaseOrder poBucket = null;
            bool authorized = true;

            foreach (ListItem department in searchDepartmentDropDown.Items)
            {
                if (!User.IsInRole(searchDepartmentDropDown.SelectedItem.Text + "_ReviewAll") && !User.IsInRole("_Purchasing Agent") && !User.IsInRole("_Admin"))
                {
                    authorized = false;
                    ScriptManager.RegisterClientScriptBlock(this, GetType(), "alertMessage", string.Format("alert('You need to be in the Review All group for the {0} department')", searchDepartmentDropDown.SelectedItem.Text), true);
                }
            }

            if (authorized)
            {

                foreach (int poPK in poPKs)
                {
                    poBucket = new PurchaseOrder(poPK);
                    returnMe.Add(poBucket);
                }
            }

            return returnMe;
        }

        protected void closeDrillDownWindowButton_Click(object sender, EventArgs e)
        {
            drillDownWindow.Visible = false;

            for (int i = (int)ViewState["lineItems"]; i >= 1; i--)
            {
                removeLastLineItem();
            }

            ViewState["lineItems"] = null;
        }

        protected void reviewInstructionsPlaceholderShowHide_Click(object sender, EventArgs e)
        {
            reviewInstructionsPlaceholder.Visible = (reviewInstructionsPlaceholder.Visible == true ? false : true);
            reviewInstructionsPlaceholderShowHide.Text = (reviewInstructionsPlaceholder.Visible == true ? "Hide Instructions" : "Show Instructions");
        }
        protected void poDrillDown(object sender, EventArgs e)
        {
            drillDownWindow.Visible = true;

            List<PurchaseOrder> test = (List<PurchaseOrder>)Session["workingPOs"];
            PurchaseOrder drillOnPO = test[((List<PurchaseOrder>)Session["workingPOs"]).FindLastIndex(x => x.poPK == Int32.Parse(((LinkButton)sender).CommandArgument))];

            for (int i = 1; i <= drillOnPO.lineItemPKs.Count(); i++)
            {
                addLineItems(i, drillOnPO.lineItems[i - 1]);
            }

            poNumbLabel.Text = drillOnPO.poPK.ToString();
            vendorComboBox.SelectedValue = drillOnPO.poVendor.ToString();
            vendorComboBox.DataBind();
            vendorInfoDisplay();
            poDateLinkButton.Text = drillOnPO.poDateCreated.ToString().Split(' ')[0];
            paymentDropDown.SelectedValue = drillOnPO.poPayment.ToString();
            vendInvNumbTextBox.Text = drillOnPO.poVendorNumb;
            poComments.Text = drillOnPO.poComments;
            totalTextBox.Text = drillOnPO.poTotal.ToString("0.00###");
        }
        protected void removeLastLineItem()
        {
            //Each line has two controls in each div, a litteral for the line break and the control itself.
            lineNumbDiv.Controls.RemoveAt(lineNumbDiv.Controls.Count - 1);
            lineNumbDiv.Controls.RemoveAt(lineNumbDiv.Controls.Count - 1);

            lineItemDescDiv.Controls.RemoveAt(lineItemDescDiv.Controls.Count - 1);
            lineItemDescDiv.Controls.RemoveAt(lineItemDescDiv.Controls.Count - 1);

            lineItemsUnitsDiv.Controls.RemoveAt(lineItemsUnitsDiv.Controls.Count - 1);
            lineItemsUnitsDiv.Controls.RemoveAt(lineItemsUnitsDiv.Controls.Count - 1);

            lineItemsUnitPriceDiv.Controls.RemoveAt(lineItemsUnitPriceDiv.Controls.Count - 1);
            lineItemsUnitPriceDiv.Controls.RemoveAt(lineItemsUnitPriceDiv.Controls.Count - 1);

            lineItemsTotalDiv.Controls.RemoveAt(lineItemsTotalDiv.Controls.Count - 1);
            lineItemsTotalDiv.Controls.RemoveAt(lineItemsTotalDiv.Controls.Count - 1);

            lineItemsDepartmentDiv.Controls.RemoveAt(lineItemsDepartmentDiv.Controls.Count - 1);
            lineItemsDepartmentDiv.Controls.RemoveAt(lineItemsDepartmentDiv.Controls.Count - 1);

            lineItemsGrantDiv.Controls.RemoveAt(lineItemsGrantDiv.Controls.Count - 1);
            lineItemsGrantDiv.Controls.RemoveAt(lineItemsGrantDiv.Controls.Count - 1);
        }

        protected void addLineItems(int lineNumber, PurchaseOrderLineItem lineItem = null)
        {
            //First we make objects to add

            //Can only use each thing once, or errors. Each linebreak needs to be a sperate object.
            Literal lineBreak0 = new Literal();
            lineBreak0.Text = "<br>";
            Literal lineBreak1 = new Literal();
            lineBreak1.Text = "<br>";
            Literal lineBreak2 = new Literal();
            lineBreak2.Text = "<br>";
            Literal lineBreak3 = new Literal();
            lineBreak3.Text = "<br>";
            Literal lineBreak4 = new Literal();
            lineBreak4.Text = "<br>";
            Literal lineBreak5 = new Literal();
            lineBreak5.Text = "<br>";
            Literal lineBreak6 = new Literal();
            lineBreak6.Text = "<br>";
            Literal lineBreak7 = new Literal();
            lineBreak7.Text = "<br>";
            Literal lineBreak8 = new Literal();
            lineBreak8.Text = "<br>";
            Literal lineBreak9 = new Literal();
            lineBreak9.Text = "<br>";
            Literal lineBreak10 = new Literal();
            lineBreak10.Text = "<br>";
            Literal lineBreak11 = new Literal();
            lineBreak11.Text = "<br>";
            Literal lineBreak12 = new Literal();
            lineBreak12.Text = "<br>";

            //making actual controls
            Label lineNumb = new Label();
            lineNumb.Text = lineNumber.ToString() + ". ";
            lineNumb.CssClass = "item";
            lineNumb.Height = 30;
            TextBox itemDescription = new TextBox();
            itemDescription.Columns = 50;
            itemDescription.CssClass = "item";
            itemDescription.ID = "itemDescription" + lineNumber.ToString();
            TextBox itemUnits = new TextBox();
            itemUnits.Columns = 4;
            itemUnits.CssClass = "item";
            itemUnits.ID = "itemUnits" + lineNumber.ToString();
            TextBox itemUnitsPrice = new TextBox();
            itemUnitsPrice.Columns = 8;
            itemUnitsPrice.CssClass = "item";
            itemUnitsPrice.ID = "itemUnitsPrice" + lineNumber.ToString();
            TextBox itemTotal = new TextBox();
            itemTotal.Columns = 8;
            itemTotal.CssClass = "item";
            itemTotal.ID = "itemTotal" + lineNumber.ToString();
            DropDownList itemDepartmentDropDown = new DropDownList();
            itemDepartmentDropDown.CssClass = "item";
            DropDownList itemGrantsDropDown = new DropDownList();
            itemGrantsDropDown.CssClass = "item";

            //Then we add created items to thier div on the page
            lineNumbDiv.Controls.Add(lineNumb);
            lineNumbDiv.Controls.Add(lineBreak0);

            lineItemDescDiv.Controls.Add(itemDescription);
            lineItemDescDiv.Controls.Add(lineBreak1);

            lineItemsUnitsDiv.Controls.Add(itemUnits);
            lineItemsUnitsDiv.Controls.Add(lineBreak2);

            lineItemsUnitPriceDiv.Controls.Add(itemUnitsPrice);
            lineItemsUnitPriceDiv.Controls.Add(lineBreak3);

            lineItemsTotalDiv.Controls.Add(itemTotal);
            lineItemsTotalDiv.Controls.Add(lineBreak4);

            lineItemsDepartmentDiv.Controls.Add(itemDepartmentDropDown);
            lineItemsDepartmentDiv.Controls.Add(lineBreak5);

            lineItemsGrantDiv.Controls.Add(itemGrantsDropDown);
            lineItemsGrantDiv.Controls.Add(lineBreak6);

            //Put values into lineItem
            if (lineItem != null)
            {
                itemDescription.Text = lineItem.lineitemDescription;
                itemDescription.Enabled = false;
                itemUnits.Text = lineItem.lineitemUnits.ToString("0.#####");
                itemUnits.Enabled = false;
                itemUnitsPrice.Text = lineItem.lineitemUnitPrice.ToString("0.00###");
                itemUnitsPrice.Enabled = false;
                itemTotal.Text = lineItem.lineitemTotalPrice.ToString("0.00###");
                itemTotal.Enabled = false;
                itemDepartmentDropDown.Items.Add(lineItem.lineitemDepartmentName);
                itemDepartmentDropDown.Enabled = false;
                itemGrantsDropDown.Items.Add(lineItem.lineitemGrantName);
                itemGrantsDropDown.Enabled = false;
            }
        }
        protected void vendorInfoDisplay()
        {
            string selectedVendorName = string.Empty;
            if (!object.Equals(null, vendorComboBox.SelectedItem))
                selectedVendorName = vendorComboBox.SelectedItem.Text;

            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
            SqlCommand vendorInfo = new SqlCommand("SELECT * FROM Vendors WHERE vendorName = @vendorName", conn);
            vendorInfo.Parameters.AddWithValue("@vendorName", selectedVendorName);

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();

                SqlDataReader readVendors = vendorInfo.ExecuteReader();
                while (readVendors.Read())
                {
                    vendorAddressLabel.Text = readVendors["vendorAddress"].ToString();
                    vendorPhoneLabel.Text = readVendors["vendorPhone"].ToString();
                    vendorCommentsLabel.Text = readVendors["vendorComments"].ToString();
                }

                conn.Close();
            }
        }

        protected void printSelectedPOsButton_Complete_Click(object sender, EventArgs e)
        {
            List<int> pksOfPOsToPrint = new List<int>();
            foreach (GridViewRow row in reportGridView.Rows)
            {
                if (((CheckBox)row.Cells[0].Controls[1]).Checked)
                {
                    pksOfPOsToPrint.Add(Int32.Parse(((LinkButton)row.Cells[1].Controls[1]).Text));
                }
            }

            Session["POsToPrint"] = pksOfPOsToPrint;
            Response.Write("<script>window.open ('/printing.aspx','_blank');</script>");
        }

        protected void submitFilterButton_Click(object sender, EventArgs e)
        {
            if (ValidateSearchCriteria())
            {
                fillDataGrid(UserAuthorizedToReviewThesePOs(ReturnRawSearchResults(CreateWhereSqlCommand())));
            }
        }

        protected SqlCommand CreateWhereSqlCommand()
        {
            SqlCommand returnMe = new SqlCommand();
            returnMe.CommandText = "select DISTINCT lineitemPoPK FROM PurchaseOrders LEFT JOIN PurchaseOrderLineItems ON PurchaseOrders.poPK = PurchaseOrderLineItems.lineitemPoPK WHERE poTotal is not null";
            //for departments
            List<SqlParameter> departmentParamters = new List<SqlParameter>();
            //for creators
            List<SqlParameter> creatorParamaters = new List<SqlParameter>();
            //for the rest
            List<SqlParameter> simpleAnds = new List<SqlParameter>();

            //DEPARTMENTS
            //If all is selected, just leave the department filter out. Dropdowns are so much easier than checkbox lists!
            if (searchDepartmentDropDown.SelectedValue != "all")
            {
                returnMe.Parameters.AddWithValue("@lineitemDepartment", searchDepartmentDropDown.SelectedValue);
                returnMe.CommandText += " AND lineitemDepartment = @lineitemDepartment";
            }

            //GRANTS
            //If all is selected, just leave the grant filter out.
            if (searchGrantDropDown.SelectedValue != "all")
            {
                returnMe.Parameters.AddWithValue("@lineitemGrant", searchGrantDropDown.SelectedValue);
                returnMe.CommandText += " AND lineitemGrant = @lineitemGrant";
            }

            //Payment
            //If all is selected, just leave the grant filter out.
            if (searchPaymentDropDown.SelectedValue != "all")
            {
                returnMe.Parameters.AddWithValue("@poPayment", searchPaymentDropDown.SelectedValue);
                returnMe.CommandText += " AND poPayment = @poPayment";
            }

            //Vendor
            //If all is selected, just leave the grant filter out.
            if (searchVendorDropDown.SelectedValue != "all")
            {
                returnMe.Parameters.AddWithValue("@poVendor", searchVendorDropDown.SelectedValue);
                returnMe.CommandText += " AND poVendor = @poVendor";
            }

            //everything else is just a simple AND and can be grouped together
            if (startDateTextBox.Text.Length > 0)
            {
                returnMe.Parameters.AddWithValue("@poDateCreatedAfter", DateTime.Parse(startDateTextBox.Text));
                returnMe.CommandText += " AND @poDateCreatedAfter <= poDateCreated";
            }

            if (endDateTextBox.Text.Length > 0)
            {
                returnMe.Parameters.AddWithValue("@poDateCreatedBefore", DateTime.Parse(endDateTextBox.Text));
                returnMe.CommandText += " AND @poDateCreatedBefore >= poDateCreated";
            }

            return returnMe;
        }

        protected bool ValidateSearchCriteria()
        {
            string validatorOutput = string.Empty;
            DateTime dateBucket = new DateTime();
            bool returnMe = true;

            //Check Start Date
            if (!DateTime.TryParse(startDateTextBox.Text, out dateBucket) && startDateTextBox.Text.Length != 0)
            {
                startDateTextBox.BorderColor = System.Drawing.Color.Red;
                validatorOutput += "Date in Start Date is not a date.<br>";
                returnMe = false;
            }
            else
                startDateTextBox.BorderColor = System.Drawing.Color.Empty;

            //Check End Date
            if (!DateTime.TryParse(endDateTextBox.Text, out dateBucket) && endDateTextBox.Text.Length != 0)
            {
                endDateTextBox.BorderColor = System.Drawing.Color.Red;
                validatorOutput += "Date in End Date is not a date.<br>";
                returnMe = false;
            }
            else
                endDateTextBox.BorderColor = System.Drawing.Color.Empty;

            searchValidation.Text = validatorOutput;
            return returnMe;
        }
        protected void searchPlaceholderPlaceholderShowHideButton_Click(object sender, EventArgs e)
        {
            searchPlaceholder.Visible = (searchPlaceholder.Visible == true ? false : true);
        }

        protected void reportGridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            List<Tuple<int, DateTime, string, decimal, string, string, string>> sort = (List<Tuple<int, DateTime, string, decimal, string, string, string>>)Session["workingLineItems"];
            //I'm not keeping track of sort direction by column. Sometimes it will sort asc firt, sometimes desc.
            string sortDir = Session["SortDir"].ToString();
            sortDir = (sortDir == "asc" ? "desc" : "asc");
            Session["SortDir"] = sortDir;

            if (e.SortExpression == "lineitemTotal") 
            {
                if (sortDir == "asc")
                    sort.Sort((x, y) => decimal.Compare(x.Item4, y.Item4));
                else
                    sort.Sort((x, y) => decimal.Compare(y.Item4, x.Item4));
            }

            if (e.SortExpression == "poPK") 
            {
                if (sortDir == "asc")
                    sort.Sort((x, y) => decimal.Compare(x.Item1, y.Item1));
                else
                    sort.Sort((x, y) => decimal.Compare(y.Item1, x.Item1));
            }

            if (e.SortExpression == "poDateCreated")
            {
                if (sortDir == "asc")
                    sort.Sort((x, y) => DateTime.Compare(x.Item2, y.Item2));
                else
                    sort.Sort((x, y) => DateTime.Compare(y.Item2, x.Item2));
            }
            if (e.SortExpression == "lineitemDepartmentName")
            {
                if (sortDir == "asc")
                    sort.Sort((x, y) => string.Compare(x.Item5, y.Item5));
                else
                    sort.Sort((x, y) => string.Compare(y.Item5, x.Item5));
            }
            if (e.SortExpression == "poPaymentName")
            {
                if (sortDir == "asc")
                    sort.Sort((x, y) => string.Compare(x.Item6, y.Item6));
                else
                    sort.Sort((x, y) => string.Compare(y.Item6, x.Item6));
            }
            if (e.SortExpression == "lineitemGrantName")
            {
                if (sortDir == "asc")
                    sort.Sort((x, y) => string.Compare(x.Item7, y.Item7));
                else
                    sort.Sort((x, y) => string.Compare(y.Item7, x.Item7));
            }
            if (e.SortExpression == "lineitemDescription")
            {
                if (sortDir == "asc")
                    sort.Sort((x, y) => string.Compare(x.Item3, y.Item3));
                else
                    sort.Sort((x, y) => string.Compare(y.Item3, x.Item3));
            }

            reportGridView.DataSource = sort;
            reportGridView.DataBind();

        }

        protected void exportSelectedToExcellButton_Click(object sender, EventArgs e)
        {            
            List<Tuple<int, DateTime, string, decimal, string, string, string>> workingLineItems = new List<Tuple<int, DateTime, string, decimal, string, string, string>>();

            foreach (GridViewRow row in reportGridView.Rows)
            {
                if (((CheckBox)row.Cells[0].Controls[1]).Checked)
                {
                    workingLineItems.Add(new Tuple<int, DateTime, string, decimal, string, string, string>(
                        Int32.Parse(((LinkButton)row.Cells[1].Controls[1]).Text),
                        DateTime.Parse(((TableCell)row.Cells[2]).Text),
                        ((TableCell)row.Cells[3]).Text,
                        decimal.Parse(((TableCell)row.Cells[4]).Text),
                        ((TableCell)row.Cells[5]).Text,
                        ((TableCell)row.Cells[6]).Text,
                        ((TableCell)row.Cells[7]).Text
                        ));
                }
            }                      

            string serverLocalPath = AppDomain.CurrentDomain.BaseDirectory + "/fileSwap/";
            string filename = User.Identity.GetUserId().ToString() + ".xlsx";
            using (SpreadsheetDocument spreadSheet = SpreadsheetDocument.Create(serverLocalPath + filename, SpreadsheetDocumentType.Workbook))
            {
                // create the workbook
                spreadSheet.AddWorkbookPart();
                spreadSheet.WorkbookPart.Workbook = new Workbook();
                // create the worksheet
                spreadSheet.WorkbookPart.AddNewPart<WorksheetPart>();
                spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet = new Worksheet();
                // create sheet data
                spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.AppendChild(new SheetData());
                
                //Header Row
                spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().AppendChild(new Row());

                spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                { DataType = CellValues.String, CellValue = new CellValue("From PO Number") });
                spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                { DataType = CellValues.String, CellValue = new CellValue("Date") });
                spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                { DataType = CellValues.String, CellValue = new CellValue("Item Description") });
                spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                { DataType = CellValues.String, CellValue = new CellValue("Total") });
                spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                { DataType = CellValues.String, CellValue = new CellValue("Department") });
                spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                { DataType = CellValues.String, CellValue = new CellValue("Payment") });
                spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                { DataType = CellValues.String, CellValue = new CellValue("Grant") });

                //Data
                foreach (Tuple<int, DateTime, string, decimal, string, string, string> thisLine in workingLineItems)
                {
                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().AppendChild(new Row());

                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                    { DataType = CellValues.Number, CellValue = new CellValue(thisLine.Item1.ToString()) });
                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                    { DataType = CellValues.String, CellValue = new CellValue(thisLine.Item2.ToShortDateString()) });
                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                    { DataType = CellValues.String, CellValue = new CellValue(thisLine.Item3) });
                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                    { DataType = CellValues.Number, CellValue = new CellValue(thisLine.Item4.ToString()) });
                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                    { DataType = CellValues.String, CellValue = new CellValue(thisLine.Item5) });
                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                    { DataType = CellValues.String, CellValue = new CellValue(thisLine.Item6) });
                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                    { DataType = CellValues.String, CellValue = new CellValue(thisLine.Item7) });
                }

                // save worksheet
                spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.Save();
                // create the worksheet to workbook relation
                spreadSheet.WorkbookPart.Workbook.AppendChild(new Sheets());
                spreadSheet.WorkbookPart.Workbook.GetFirstChild<Sheets>().AppendChild(new Sheet()
                {
                    Id = spreadSheet.WorkbookPart.GetIdOfPart(spreadSheet.WorkbookPart.WorksheetParts.First()),
                    SheetId = 1,
                    Name = "po results"
                });
                spreadSheet.WorkbookPart.Workbook.Save();
            }

            ScriptManager.RegisterClientScriptBlock(this, GetType(), "windowOPen", "window.open('/fileSwap/" + filename + "?download')", true);
        }

        protected void selectAllButton_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in reportGridView.Rows)
            {
                ((CheckBox)row.Cells[0].Controls[1]).Checked = true;
            }
        }
    }
}