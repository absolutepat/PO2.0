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
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace purchaseOrderProgram2
{
    public partial class review : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //fillDataGrid(UserAuthorizedToReviewThesePOs(ReturnRawSearchResults(new SqlCommand("select DISTINCT lineitemPoPK FROM PurchaseOrders LEFT JOIN PurchaseOrderLineItems ON PurchaseOrders.poPK = PurchaseOrderLineItems.lineitemPoPK WHERE poTotal is not null"))));
                fillListsAndDrops();
                endDateTextBox.Text = DateTime.Today.ToShortDateString();
                Session["SortDir"] = "asc";
                submitFilterButton_Click(null, null);
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
        protected void fillListsAndDrops()
        {
            fillCheckBoxList();
        }
        protected void fillCheckBoxList()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            List<Tuple<int, string, string, bool>> dataSource = GetDepartmentInfo(2);

            departmentCheckBoxList.Items.Clear();
            departmentCheckBoxList.DataSource = dataSource.OrderBy(x => x.Item2);
            departmentCheckBoxList.DataTextField = "Item2";
            departmentCheckBoxList.DataValueField = "Item1";
            departmentCheckBoxList.DataBind();

            foreach (ListItem checkbox in departmentCheckBoxList.Items)
            {

                if (dataSource.FindLast(x => x.Item1 == Int32.Parse(checkbox.Value)).Item4 == false)
                    checkbox.Attributes.Add("class", "departmentDisabled");

                if (User.IsInRole(checkbox.Text + "_ReviewAll") || User.IsInRole("_Purchasing Agent") || User.IsInRole("_Admin"))
                    checkbox.Selected = true;
                else
                    checkbox.Enabled = false;
            }

            departmentCheckBoxList.Items.Insert(0, new ListItem("All Allowed", "all"));
            departmentCheckBoxList.Items[0].Selected = true;
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
            //to do sorting we'll need this
            Session["workingPOs"] = workingPOs;

            reviewGridView.DataSource = workingPOs;
            reviewGridView.DataKeyNames = new string[] { "poPK", };
            reviewGridView.DataBind();
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

            foreach (int poPK in poPKs)
            {
                poBucket = new PurchaseOrder(poPK);

                foreach (PurchaseOrderLineItem lineItem in poBucket.lineItems)
                {
                    if (!User.IsInRole(lineItem.lineitemDepartmentName + "_ReviewAll") && !User.IsInRole("_Purchasing Agent") && !User.IsInRole("_Admin")) 
                    {
                        lineItem.lineitemDescription = "You are not allowed to review purchases for this department.";
                        lineItem.lineitemUnits = 0;
                        lineItem.lineitemUnitPrice = 0;
                        lineItem.lineitemTotalPrice = 0;
                    }
                }

                returnMe.Add(poBucket);
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
            foreach (GridViewRow row in reviewGridView.Rows)
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

        //create Where clause, update fill data grid, done?
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

            //START DEPARTMENTS
            //All departments is selected
            if (departmentCheckBoxList.Items.FindByValue("all").Selected == true)
            {
                foreach (ListItem checkbox in departmentCheckBoxList.Items)
                {
                    if (checkbox.Enabled && checkbox.Value != "all")
                        departmentParamters.Add(new SqlParameter("@lineitemDepartment" + checkbox.Value, checkbox.Value));
                }
            }
            //something(s) else is selected    
            else if (departmentCheckBoxList.SelectedIndex != -1)
            {
                foreach (ListItem checkbox in departmentCheckBoxList.Items)
                {
                    if (checkbox.Selected)
                        departmentParamters.Add(new SqlParameter("@lineitemDepartment" + checkbox.Value, checkbox.Value));
                }
            }
            //do popup telling must select at least one department
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, GetType(), "alertMessage", string.Format("alert('You have to select at least one department in your search.')"), true);
            }

            if (departmentParamters.Count > 0)
            {
                returnMe.CommandText += " AND (";
                foreach (SqlParameter paramter in departmentParamters)
                {
                    returnMe.Parameters.Add(paramter);
                    returnMe.CommandText += " lineitemDepartment = " + paramter.ParameterName + " OR";
                }

                returnMe.CommandText = returnMe.CommandText.Remove(returnMe.CommandText.Length - 3);
                returnMe.CommandText += ")";
            }
            //END DEPARTMENTS

            //START CREATORS
            //if no creator filter selected, show all
            if (purchaserListBox.SelectedIndex != -1)
            {
                foreach (ListItem creator in purchaserListBox.Items)
                {
                    if (creator.Selected)
                        creatorParamaters.Add(new SqlParameter("@poCreator" + (creator.Text.Replace('@','A')).Replace('.','D'), creator.Text));
                }
            }

            if (creatorParamaters.Count > 0)
            {
                returnMe.CommandText += " AND (";
                foreach (SqlParameter paramter in creatorParamaters)
                {
                    returnMe.Parameters.Add(paramter);
                    returnMe.CommandText += " poCreator = " + paramter.ParameterName + " OR";
                }

                returnMe.CommandText = returnMe.CommandText.Remove(returnMe.CommandText.Length - 3);
                returnMe.CommandText += ")";
            }
            //END CREATORS

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

            if (lowPriceTextBox.Text.Length > 0)
            {
                returnMe.Parameters.AddWithValue("@poTotalAbove", Decimal.Parse(lowPriceTextBox.Text));
                returnMe.CommandText += " AND @poTotalAbove <= poTotal";
            }

            if (highPriceTextBox.Text.Length > 0)
            {
                returnMe.Parameters.AddWithValue("@poTotalBelow", Decimal.Parse(highPriceTextBox.Text));
                returnMe.CommandText += " AND @poTotalBelow >= poTotal";
            }

            return returnMe;
        }

        protected bool ValidateSearchCriteria()
        {
            string validatorOutput = string.Empty;
            decimal checkBucket = 0;
            DateTime dateBucket = new DateTime();
            bool returnMe = true;

            //Check Lower Price
            if (!decimal.TryParse(lowPriceTextBox.Text, out checkBucket) && lowPriceTextBox.Text.Length != 0)
            {
                lowPriceTextBox.BorderColor = System.Drawing.Color.Red;
                validatorOutput += "Price in low total is not a number.<br>";
                returnMe = false;
            }
            else
                lowPriceTextBox.BorderColor = System.Drawing.Color.Empty;

            //Check Upper Price
            if (!decimal.TryParse(highPriceTextBox.Text, out checkBucket) && highPriceTextBox.Text.Length != 0)
            {
                highPriceTextBox.BorderColor = System.Drawing.Color.Red;
                validatorOutput += "Price in high total is not a number.<br>";
                returnMe = false;
            }
            else
                highPriceTextBox.BorderColor = System.Drawing.Color.Empty;

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

        protected void reviewGridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            List<PurchaseOrder> sort = (List < PurchaseOrder > )Session["workingPOs"];
            //I'm not keeping track of sort direction by column. Sometimes it will sort asc firt, sometimes desc.
            string sortDir = Session["SortDir"].ToString();
            sortDir = (sortDir == "asc" ? "desc" : "asc");
            Session["SortDir"] = sortDir;

            if (e.SortExpression == "poTotal")
            {
                if(sortDir == "asc")
                    sort.Sort( (x, y) => decimal.Compare(x.poTotal, y.poTotal) ) ;
                else
                    sort.Sort( (x, y) => decimal.Compare(y.poTotal, x.poTotal) );
            }

            if(e.SortExpression == "poPK")
            {
                if (sortDir == "asc")
                    sort.Sort((x, y) => decimal.Compare(x.poPK, y.poPK));
                else
                    sort.Sort((x, y) => decimal.Compare(y.poPK, x.poPK));
            }

            if(e.SortExpression == "poCreator")
            {
                if (sortDir == "asc")
                    sort.Sort((x, y) => string.Compare(x.poCreator, y.poCreator));
                else
                    sort.Sort((x, y) => string.Compare(y.poCreator, x.poCreator));
            }

            if (e.SortExpression == "poDateCreated")
            {
                if (sortDir == "asc")
                    sort.Sort((x, y) => DateTime.Compare(x.poDateCreated, y.poDateCreated));
                else
                    sort.Sort((x, y) => DateTime.Compare(y.poDateCreated, x.poDateCreated));
            }

            if (e.SortExpression == "poComments")
            {
                if (sortDir == "asc")
                    sort.Sort((x, y) => string.Compare(x.poComments, y.poComments));
                else
                    sort.Sort((x, y) => string.Compare(y.poComments, x.poComments));
            }
            if (e.SortExpression == "poDateSubmitedForReconciliation")
            {
                if (sortDir == "asc")
                    sort.Sort((x, y) => String.Compare(x.poDateSubmitedForReconciliation.ToString(), y.poDateSubmitedForReconciliation.ToString()));
                else
                    sort.Sort((x, y) => String.Compare(y.poDateSubmitedForReconciliation.ToString(), x.poDateSubmitedForReconciliation.ToString()));
            }
            if (e.SortExpression == "poActive")
            {
                if (sortDir == "asc")
                    sort.Sort((x, y) => String.Compare(x.poActive.ToString(), y.poActive.ToString()));
                else
                    sort.Sort((x, y) => String.Compare(y.poActive.ToString(), x.poActive.ToString()));
            }


            reviewGridView.DataSource = sort;
            reviewGridView.DataBind();

        }

        protected void exportSelectedToExcellButton_Click(object sender, EventArgs e)
        {
            List<int> pksOfPOsToExport = new List<int>();
            foreach (GridViewRow row in reviewGridView.Rows)
            {
                if (((CheckBox)row.Cells[0].Controls[1]).Checked)
                {
                    pksOfPOsToExport.Add(Int32.Parse(((LinkButton)row.Cells[1].Controls[1]).Text));
                }
            }

            List<PurchaseOrder> exportPOs = UserAuthorizedToReviewThesePOs(pksOfPOsToExport);

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



                foreach (PurchaseOrder thisPO in exportPOs)
                {
                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().AppendChild(new Row());
                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                    { DataType = CellValues.String, CellValue = new CellValue("PO Number") });
                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                    { DataType = CellValues.Number, CellValue = new CellValue(thisPO.poPK.ToString()) });

                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().AppendChild(new Row());
                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                    { DataType = CellValues.String, CellValue = new CellValue("Date") });
                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                    { DataType = CellValues.String, CellValue = new CellValue(thisPO.poDateCreated.ToShortDateString()) });

                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().AppendChild(new Row());
                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                    { DataType = CellValues.String, CellValue = new CellValue("Vendor") });
                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                    { DataType = CellValues.String, CellValue = new CellValue(thisPO.poVendorName) });

                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().AppendChild(new Row());
                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                    { DataType = CellValues.String, CellValue = new CellValue("Payment") });
                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                    { DataType = CellValues.String, CellValue = new CellValue(thisPO.poPaymentName) });

                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().AppendChild(new Row());
                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                    { DataType = CellValues.String, CellValue = new CellValue("Comments") });
                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                    { DataType = CellValues.String, CellValue = new CellValue(thisPO.poComments) });

                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().AppendChild(new Row());
                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                    { DataType = CellValues.String, CellValue = new CellValue("Items") });

                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().AppendChild(new Row());
                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                    { DataType = CellValues.String, CellValue = new CellValue("Decription") });
                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                    { DataType = CellValues.String, CellValue = new CellValue("Quantity") });
                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                    { DataType = CellValues.String, CellValue = new CellValue("Price Each") });
                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                    { DataType = CellValues.String, CellValue = new CellValue("Item Total") });
                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                    { DataType = CellValues.String, CellValue = new CellValue("Department") });
                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                    { DataType = CellValues.String, CellValue = new CellValue("Grant") });

                    foreach (PurchaseOrderLineItem lineItem in thisPO.lineItems)
                    {
                        spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().AppendChild(new Row());
                        spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                        { DataType = CellValues.String, CellValue = new CellValue(lineItem.lineitemDescription) });
                        spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                        { DataType = CellValues.Number, CellValue = new CellValue(lineItem.lineitemUnits.ToString()) });
                        spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                        { DataType = CellValues.Number, CellValue = new CellValue(lineItem.lineitemUnitPrice.ToString()) });
                        spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                        { DataType = CellValues.Number, CellValue = new CellValue(lineItem.lineitemTotalPrice.ToString()) });
                        spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                        { DataType = CellValues.String, CellValue = new CellValue(lineItem.lineitemDepartmentName) });
                        spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                        { DataType = CellValues.String, CellValue = new CellValue(lineItem.lineitemGrantName) });
                    }

                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().AppendChild(new Row());
                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                    { DataType = CellValues.String, CellValue = new CellValue("PO Total") });
                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                    { DataType = CellValues.String, CellValue = new CellValue("") });
                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                    { DataType = CellValues.String, CellValue = new CellValue("") });
                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().Last().AppendChild(new Cell()
                    { DataType = CellValues.Number, CellValue = new CellValue(thisPO.poTotal.ToString()) });

                    spreadSheet.WorkbookPart.WorksheetParts.First().Worksheet.First().AppendChild(new Row());
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
            foreach (GridViewRow row in reviewGridView.Rows)
            {
                ((CheckBox)row.Cells[0].Controls[1]).Checked = true;
            }
        }
    }
}