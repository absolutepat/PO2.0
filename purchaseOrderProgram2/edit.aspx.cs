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
    public partial class edit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
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

        public List<PurchaseOrder> UserAuthorizedToEditThesePOs(List<int> poPKs)
        {
            List<PurchaseOrder> returnMe = new List<PurchaseOrder>();
            PurchaseOrder poBucket = null;
            string rawDescription = string.Empty;

            foreach (int poPK in poPKs)
            {
                poBucket = new PurchaseOrder(poPK);

                foreach (PurchaseOrderLineItem lineItem in poBucket.lineItems)
                {
                    if (!User.IsInRole(lineItem.lineitemDepartmentName + "_EditAll") && !User.IsInRole("_Purchasing Agent") && !User.IsInRole("_Admin"))
                    {
                        //if they can't review, they can't review!
                        if (!User.IsInRole(lineItem.lineitemDepartmentName + "_ReviewAll") && !User.IsInRole("_Purchasing Agent") && !User.IsInRole("_Admin"))
                        {
                            rawDescription = lineItem.lineitemDescription;
                            lineItem.lineitemDescription = "*(No Edit or Review permision for department)" + rawDescription;
                            lineItem.lineitemUnits = 0;
                            lineItem.lineitemUnitPrice = 0;
                        }
                        else
                        {
                            rawDescription = lineItem.lineitemDescription;
                            lineItem.lineitemDescription = "*(No Edit permision for department)" + rawDescription;
                        }
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
            List<PurchaseOrder> workingPOs = (List<PurchaseOrder>)Session["workingPOs"];
            PurchaseOrder drillOnPO = workingPOs[((List<PurchaseOrder>)Session["workingPOs"]).FindLastIndex(x => x.poPK == Int32.Parse(((LinkButton)sender).CommandArgument))];
            ViewState["lineItems"] = drillOnPO.lineItemPKs.Count();
            Session["poNumb"] = drillOnPO.poPK;

            drillDownWindow.Visible = true;

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
            //Label lineNumb = new Label();
            //lineNumb.Text = lineNumber.ToString() + ". ";
            //lineNumb.CssClass = "item";
            //lineNumb.Height = 30;
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
            itemDepartmentDropDown.DataSourceID = "departmentsDataSource";
            itemDepartmentDropDown.DataTextField = "departmentName";
            itemDepartmentDropDown.SelectedIndex = departmentDropDown.SelectedIndex;
            itemDepartmentDropDown.CssClass = "item";
            DropDownList itemGrantsDropDown = new DropDownList();
            itemGrantsDropDown.DataSourceID = "grantsDataSource";
            itemGrantsDropDown.DataTextField = "grantName";
            itemGrantsDropDown.CssClass = "item";
            CheckBox deleteChecked = new CheckBox();
            deleteChecked.ID = "deleteChecked" + lineNumber.ToString();
            deleteChecked.CssClass = "item";
            deleteChecked.Height = 30;           

            //Then we add created items to thier div on the page
            lineNumbDiv.Controls.Add(deleteChecked);
           // lineNumbDiv.Controls.Add(lineNumb);            
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
                //check authorized to edit line item
                if (lineItem.lineitemDescription[0] != '*')
                {
                    itemDescription.Text = lineItem.lineitemDescription;
                    itemDescription.ToolTip = lineItem.lineitemPK.ToString();
                    itemUnits.Text = lineItem.lineitemUnits.ToString("0.#####");
                    itemUnitsPrice.Text = lineItem.lineitemUnitPrice.ToString("0.00###");
                    itemTotal.Text = lineItem.lineitemTotalPrice.ToString("0.00###");
                    itemDepartmentDropDown.DataBind();
                    itemDepartmentDropDown.SelectedValue = lineItem.lineitemDepartmentName;
                    itemGrantsDropDown.DataBind();
                    itemGrantsDropDown.SelectedValue = lineItem.lineitemGrantName;
                }
                else
                {
                    deleteChecked.Enabled = false;
                    itemDescription.Text = lineItem.lineitemDescription;
                    itemDescription.ToolTip = lineItem.lineitemPK.ToString();
                    itemDescription.Enabled = false;
                    itemUnits.Text = lineItem.lineitemUnits.ToString("0.#####");
                    itemUnits.Enabled = false;
                    itemUnitsPrice.Text = lineItem.lineitemUnitPrice.ToString("0.00###");
                    itemUnitsPrice.Enabled = false;
                    itemTotal.Text = lineItem.lineitemTotalPrice.ToString("0.00###");                    
                    itemTotal.Enabled = false;
                    itemDepartmentDropDown.DataBind();
                    itemDepartmentDropDown.SelectedValue = lineItem.lineitemDepartmentName;
                    itemDepartmentDropDown.Enabled = false;
                    itemGrantsDropDown.DataBind();
                    itemGrantsDropDown.SelectedValue = lineItem.lineitemGrantName;
                    itemGrantsDropDown.Enabled = false;
                }
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
                fillDataGrid(UserAuthorizedToEditThesePOs(ReturnRawSearchResults(CreateWhereSqlCommand())));
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
                        creatorParamaters.Add(new SqlParameter("@poCreator" + (creator.Text.Replace('@', 'A')).Replace('.', 'D'), creator.Text));
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
            List<PurchaseOrder> sort = (List<PurchaseOrder>)Session["workingPOs"];
            string sortDir = Session["SortDir"].ToString();
            sortDir = (sortDir == "asc" ? "desc" : "asc");
            Session["SortDir"] = sortDir;

            if (e.SortExpression == "poTotal")
            {
                if (sortDir == "asc")
                    sort.Sort((x, y) => decimal.Compare(x.poTotal, y.poTotal));
                else
                    sort.Sort((x, y) => decimal.Compare(y.poTotal, x.poTotal));
            }

            if (e.SortExpression == "poPK")
            {
                if (sortDir == "asc")
                    sort.Sort((x, y) => decimal.Compare(x.poPK, y.poPK));
                else
                    sort.Sort((x, y) => decimal.Compare(y.poPK, x.poPK));
            }

            if (e.SortExpression == "poCreator")
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

            List<PurchaseOrder> exportPOs = UserAuthorizedToEditThesePOs(pksOfPOsToExport);

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

        protected void vendorDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            vendorInfoDisplay();
        }
        protected void vendorDropDownList_DataBound(object sender, EventArgs e)
        {
            vendorInfoDisplay();
        }
        protected void vendorEditSubmitButton_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
            SqlCommand updateVendor = new SqlCommand("UPDATE Vendors SET vendorName = @vendorName, vendorAddress = @vendorAddress , vendorPhone = @vendorPhone, vendorComments = @vendorComments WHERE vendorPK = @vendorPK", conn);
            updateVendor.Parameters.AddWithValue("@vendorName", vendorEditNameTextBox.Text);
            updateVendor.Parameters.AddWithValue("@vendorAddress", vendorEditAddressTextBox.Text);
            updateVendor.Parameters.AddWithValue("@vendorPhone", vendorEditPhoneTextBox.Text);
            updateVendor.Parameters.AddWithValue("@vendorComments", vendorEditCommentsTextBox.Text);
            updateVendor.Parameters.AddWithValue("@vendorPK", vendorComboBox.SelectedValue);

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                updateVendor.ExecuteNonQuery();
                conn.Close();
            }

            vendorComboBox.RaisePostDataChangedEvent();
            cancelNewOrEditVendorButton_Click(sender, e);
        }
        protected void cancelNewOrEditVendorButton_Click(object sender, EventArgs e)
        {
            vendorComboBox.Visible = true;
            selectedVendorPlaceholder.Visible = true;
            editVendorPlaceholder.Visible = false;
            newVendorPlaceholder.Visible = false;
            editVendorButton.Visible = true;
            addNewVendorButton.Visible = true;
            cancelNewOrEditVendorButton.Visible = false;
            submitButton.Enabled = true;
        }
        protected void vendorNewSubmitButton_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
            SqlCommand insertNewVendor = new SqlCommand("INSERT INTO Vendors (vendorName, vendorAddress, vendorPhone, vendorComments) VALUES (@vendorName, @vendorAddress, @vendorPhone, @vendorComments);", conn);
            insertNewVendor.Parameters.AddWithValue("@vendorName", vendorNewNameTextBox.Text);
            insertNewVendor.Parameters.AddWithValue("@vendorAddress", vendorNewAddressTextBox.Text);
            insertNewVendor.Parameters.AddWithValue("@vendorPhone", vendorNewPhoneTextBox.Text);
            insertNewVendor.Parameters.AddWithValue("@vendorComments", vendorNewCommentsTextBox.Text);

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                insertNewVendor.ExecuteNonQuery();
                conn.Close();
            }

            vendorComboBox.DataBind();
            cancelNewOrEditVendorButton_Click(sender, e);
        }
        protected void editVendorButton_Click(object sender, EventArgs e)
        {
            vendorComboBox.Visible = false;
            selectedVendorPlaceholder.Visible = false;
            editVendorPlaceholder.Visible = true;
            newVendorPlaceholder.Visible = false;
            editVendorButton.Visible = false;
            addNewVendorButton.Visible = false;
            cancelNewOrEditVendorButton.Visible = true;
            submitButton.Enabled = false;

            vendorEditNameTextBox.Text = vendorComboBox.SelectedItem.Text;
            vendorEditAddressTextBox.Text = vendorAddressLabel.Text;
            vendorEditPhoneTextBox.Text = vendorPhoneLabel.Text;
            vendorEditCommentsTextBox.Text = vendorCommentsLabel.Text;
        }
        protected void addNewVendorButton_Click(object sender, EventArgs e)
        {
            vendorComboBox.Visible = false;
            selectedVendorPlaceholder.Visible = false;
            editVendorPlaceholder.Visible = false;
            newVendorPlaceholder.Visible = true;
            editVendorButton.Visible = false;
            addNewVendorButton.Visible = false;
            cancelNewOrEditVendorButton.Visible = true;
            submitButton.Enabled = false;
        }
        protected void poDateLinkButton_Click(object sender, EventArgs e)
        {
            if (poDateCalendar.Visible)
                poDateCalendar.Visible = false;
            else
                poDateCalendar.Visible = true;
        }
        protected void poDateCalendar_SelectionChanged(object sender, EventArgs e)
        {
            Session["poDate"] = poDateCalendar.SelectedDate.ToShortDateString();
            poDateCalendar.VisibleDate = poDateCalendar.SelectedDate;
            poDateCalendar.Visible = false;
            poDateLinkButton.Text = Session["poDate"].ToString();
        }
        protected void departmentDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (int i in createListOfLineItemIndexes())
            {
                if( ((DropDownList)lineItemsDepartmentDiv.Controls[i]).Enabled == true )
                    ((DropDownList)lineItemsDepartmentDiv.Controls[i]).SelectedIndex = departmentDropDown.SelectedIndex;                
            }
        }
        protected List<int> createListOfLineItemIndexes()
        {
            List<int> indexesOfTextBoxes = new List<int>();

            for (int i = 0; i < lineItemDescDiv.Controls.Count; i++)
            {
                if (lineItemDescDiv.Controls[i] is TextBox)
                {
                    indexesOfTextBoxes.Add(i);
                }
            }

            return indexesOfTextBoxes;
        }
        protected void grantDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (int i in createListOfLineItemIndexes())
            {
                if(((DropDownList)lineItemsGrantDiv.Controls[i]).Enabled == true)
                    ((DropDownList)lineItemsGrantDiv.Controls[i]).SelectedIndex = grantDropDown.SelectedIndex;
            }
        }
        protected void newLineButton_Click(object sender, EventArgs e)
        {
            int numberOfLineItems = (int)ViewState["lineItems"];
            ViewState["lineItems"] = numberOfLineItems + 1;
            addLineItems((int)ViewState["lineItems"]);
        }
        protected void removeLineButton_Click(object sender, EventArgs e)
        {
            int numberOfLineItems = (int)ViewState["lineItems"];
            if (numberOfLineItems > 1)
            {
                ViewState["lineItems"] = numberOfLineItems - 1;
                removeLastLineItem();
            }
        }

        protected void DeleteLineItem(int lineitemPK)
        {
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
            SqlCommand updatePO = new SqlCommand("DELETE FROM PurchaseOrderLineItems WHERE lineitemPK = @lineitemPK", conn);
            updatePO.Parameters.AddWithValue("@lineitemPK", lineitemPK);

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                updatePO.ExecuteNonQuery();
                conn.Close();
            }
        }

        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            if (ValidateAll())
            {
                updatePO();
                submitLineItems();
                closeDrillDownWindowButton_Click(null, null);
                submitFilterButton_Click(null, null);
            }
        }

        protected void submitLineItems()
        {
            int poNumb = (int)Session["poNumb"];      
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
            SqlCommand selectPK = new SqlCommand("", conn);
            selectPK.CommandType = CommandType.StoredProcedure;
            SqlParameter @name = new SqlParameter("@name", SqlDbType.NVarChar);
            selectPK.Parameters.Add(@name);

            SqlDataReader readPK = null;

            SqlCommand updateLineItem = new SqlCommand("UPDATE PurchaseOrderLineItems SET lineItemDepartment = @lineitemDepartment, lineitemGrant = @lineitemGrant, lineitemDescription = @lineitemDescription, lineitemUnits = @lineitemUnits, lineitemUnitPrice = @lineitemUnitPrice, lineitemTotalPrice = @lineitemTotalPrice WHERE lineitemPK = @lineitemPK", conn);
            updateLineItem.Parameters.Add("@lineitemDepartment", SqlDbType.Int);
            updateLineItem.Parameters.Add("@lineitemGrant", SqlDbType.Int);
            updateLineItem.Parameters.Add("@lineitemDescription", SqlDbType.NVarChar);
            updateLineItem.Parameters.Add("@lineitemUnits", SqlDbType.Decimal);
            updateLineItem.Parameters.Add("@lineitemUnitPrice", SqlDbType.Decimal);
            updateLineItem.Parameters.Add("@lineitemTotalPrice", SqlDbType.Decimal);
            updateLineItem.Parameters.Add("@lineitemPK", SqlDbType.Int);

            SqlCommand insertLineItems = new SqlCommand("INSERT INTO PurchaseOrderLineItems (lineitemPoPK, lineitemDepartment, lineitemGrant, lineitemDescription, lineitemUnits, lineitemUnitPrice, lineitemTotalPrice) VALUES (@lineitemPoPK, @lineitemDepartment, @lineitemGrant, @lineitemDescription, @lineitemUnits, @lineitemUnitPrice, @lineitemTotalPrice);", conn);
            insertLineItems.Parameters.AddWithValue("@lineitemPoPK", poNumb);
            insertLineItems.Parameters.Add("@lineitemDepartment", SqlDbType.Int);
            insertLineItems.Parameters.Add("@lineitemGrant", SqlDbType.Int);
            insertLineItems.Parameters.Add("@lineitemDescription", SqlDbType.NVarChar);
            insertLineItems.Parameters.Add("@lineitemUnits", SqlDbType.Decimal);
            insertLineItems.Parameters.Add("@lineitemUnitPrice", SqlDbType.Decimal);
            insertLineItems.Parameters.Add("@lineitemTotalPrice", SqlDbType.Decimal);




            foreach (int lineItem in createListOfLineItemIndexes())
            {
                if (lineNumbDiv.Controls[lineItem] is CheckBox)
                {
                    if (((CheckBox)lineNumbDiv.Controls[lineItem]).Enabled)
                    {
                        //delete checked
                        if (((CheckBox)lineNumbDiv.Controls[lineItem]).Checked)
                            DeleteLineItem(Int32.Parse(((TextBox)lineItemDescDiv.Controls[lineItem]).ToolTip));
                        //new line item
                        else if(((TextBox)lineItemDescDiv.Controls[lineItem]).ToolTip == "")
                        {
                            //get deparmentPK
                            if (conn.State == ConnectionState.Closed)
                            {
                                conn.Open();
                                selectPK.Parameters["@name"].Value = ((DropDownList)lineItemsDepartmentDiv.Controls[lineItem]).SelectedItem.Text;
                                selectPK.CommandText = "GetDepartmentPkFromDepartmentName";
                                readPK = selectPK.ExecuteReader();
                                while (readPK.Read())
                                {
                                    insertLineItems.Parameters["@lineitemDepartment"].Value = readPK["departmentPK"];
                                }
                                conn.Close();
                            }

                            //get grantPK
                            if (conn.State == ConnectionState.Closed)
                            {
                                conn.Open();
                                selectPK.Parameters["@name"].Value = ((DropDownList)lineItemsGrantDiv.Controls[lineItem]).SelectedItem.Text;
                                selectPK.CommandText = "GetGrantPkFromGrantName";
                                readPK = selectPK.ExecuteReader();
                                while (readPK.Read())
                                {
                                    insertLineItems.Parameters["@lineitemGrant"].Value = readPK["grantPK"];
                                }
                                conn.Close();
                            }

                            //insert poLineItem
                            if (conn.State == ConnectionState.Closed)
                            {
                                conn.Open();
                                insertLineItems.Parameters["@lineitemDescription"].Value = ((TextBox)lineItemDescDiv.Controls[lineItem]).Text;
                                insertLineItems.Parameters["@lineitemUnits"].Value = decimal.Parse(((TextBox)lineItemsUnitsDiv.Controls[lineItem]).Text);
                                insertLineItems.Parameters["@lineitemUnitPrice"].Value = decimal.Parse(((TextBox)lineItemsUnitPriceDiv.Controls[lineItem]).Text);
                                insertLineItems.Parameters["@lineitemTotalPrice"].Value = decimal.Parse(((TextBox)lineItemsTotalDiv.Controls[lineItem]).Text);
                                insertLineItems.ExecuteNonQuery();
                                conn.Close();
                            }
                        }
                        //existing line items
                        else
                        {

                            updateLineItem.Parameters["@lineitemPK"].Value = Int32.Parse(((TextBox)lineItemDescDiv.Controls[lineItem]).ToolTip);

                            //get deparmentPK
                            if (conn.State == ConnectionState.Closed)
                            {
                                conn.Open();
                                selectPK.Parameters["@name"].Value = ((DropDownList)lineItemsDepartmentDiv.Controls[lineItem]).SelectedItem.Text;
                                selectPK.CommandText = "GetDepartmentPkFromDepartmentName";
                                readPK = selectPK.ExecuteReader();
                                while (readPK.Read())
                                {
                                    updateLineItem.Parameters["@lineitemDepartment"].Value = readPK["departmentPK"];
                                }
                                conn.Close();
                            }

                            //get grantPK
                            if (conn.State == ConnectionState.Closed)
                            {
                                conn.Open();
                                selectPK.Parameters["@name"].Value = ((DropDownList)lineItemsGrantDiv.Controls[lineItem]).SelectedItem.Text;
                                selectPK.CommandText = "GetGrantPkFromGrantName";
                                readPK = selectPK.ExecuteReader();
                                while (readPK.Read())
                                {
                                    updateLineItem.Parameters["@lineitemGrant"].Value = readPK["grantPK"];
                                }
                                conn.Close();
                            }

                            //update poLineItem
                            if (conn.State == ConnectionState.Closed)
                            {
                                conn.Open();
                                updateLineItem.Parameters["@lineitemDescription"].Value = ((TextBox)lineItemDescDiv.Controls[lineItem]).Text;
                                updateLineItem.Parameters["@lineitemUnits"].Value = decimal.Parse(((TextBox)lineItemsUnitsDiv.Controls[lineItem]).Text);
                                updateLineItem.Parameters["@lineitemUnitPrice"].Value = decimal.Parse(((TextBox)lineItemsUnitPriceDiv.Controls[lineItem]).Text);
                                updateLineItem.Parameters["@lineitemTotalPrice"].Value = decimal.Parse(((TextBox)lineItemsTotalDiv.Controls[lineItem]).Text);
                                updateLineItem.ExecuteNonQuery();
                                conn.Close();
                            }
                        }
                    }
                }
            }
        }

        protected void updatePO()
        {
            decimal total = 0;
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
            SqlCommand updatePO = new SqlCommand("UPDATE PurchaseOrders SET poPayment = @payment, poTotal = @poTotal, poDateCreated = @dateCreated , poVendor = @vendor, poVendorPoNumb = @vendorPoNumb , poComments = @comments WHERE poPK = @poNumb", conn);

            updatePO.Parameters.AddWithValue("@poNumb", (int)Session["poNumb"]);
            updatePO.Parameters.AddWithValue("@payment", paymentDropDown.SelectedValue);
            updatePO.Parameters.AddWithValue("@dateCreated", DateTime.Parse(poDateLinkButton.Text));
            updatePO.Parameters.AddWithValue("@vendor", vendorComboBox.SelectedValue);
            updatePO.Parameters.AddWithValue("@vendorPoNumb", (vendInvNumbTextBox.Text == "" ? (object)DBNull.Value : vendInvNumbTextBox.Text));
            updatePO.Parameters.AddWithValue("@comments", (poComments.Text == "" ? (object)DBNull.Value : poComments.Text));

            foreach (int i in createListOfLineItemIndexes())
            {
                if (lineNumbDiv.Controls[i] is CheckBox)
                {
                    if (!((CheckBox)lineNumbDiv.Controls[i]).Checked)
                        total += decimal.Parse(((TextBox)lineItemsTotalDiv.Controls[i]).Text);
                }
            }

            updatePO.Parameters.AddWithValue("@poTotal", total);

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                updatePO.ExecuteNonQuery();
                conn.Close();
            }
        }
        protected string CheckForBlanks()
        {
            string validatorOutput = string.Empty;

            foreach (int i in createListOfLineItemIndexes())
            {
                if (((TextBox)lineItemDescDiv.Controls[i]).Text.Trim().Length == 0)
                {
                    ((TextBox)lineItemDescDiv.Controls[i]).BorderColor = System.Drawing.Color.Red;
                    validatorOutput += "Item Description for " + ((TextBox)lineItemDescDiv.Controls[i]).Text.TrimEnd(' ', '.') + " can\'t be blank. <br>";
                }
                else
                {
                    ((TextBox)lineItemDescDiv.Controls[i]).BorderColor = System.Drawing.Color.Empty;
                }

                if (((TextBox)lineItemsUnitsDiv.Controls[i]).Text.Trim().Length == 0)
                {
                    ((TextBox)lineItemsUnitsDiv.Controls[i]).BorderColor = System.Drawing.Color.Red;
                    validatorOutput += "Number of units for " + ((TextBox)lineItemDescDiv.Controls[i]).Text.TrimEnd(' ', '.') + " can\'t be blank. <br>";
                }
                else
                {
                    ((TextBox)lineItemsUnitsDiv.Controls[i]).BorderColor = System.Drawing.Color.Empty;
                }

                if (((TextBox)lineItemsUnitPriceDiv.Controls[i]).Text.Trim().Length == 0)
                {
                    ((TextBox)lineItemsUnitPriceDiv.Controls[i]).BorderColor = System.Drawing.Color.Red;
                    validatorOutput += "Unit Price for " + ((TextBox)lineItemDescDiv.Controls[i]).Text.TrimEnd(' ', '.') + " can\'t be blank. <br>";
                }
                else
                {
                    ((TextBox)lineItemsUnitPriceDiv.Controls[i]).BorderColor = System.Drawing.Color.Empty;
                }

                if (((TextBox)lineItemsTotalDiv.Controls[i]).Text.Trim().Length == 0)
                {
                    ((TextBox)lineItemsTotalDiv.Controls[i]).BorderColor = System.Drawing.Color.Red;
                    validatorOutput += "Line Total for " + ((TextBox)lineItemDescDiv.Controls[i]).Text.TrimEnd(' ', '.') + " can\'t be blank. <br>";
                }
                else
                {
                    ((TextBox)lineItemsTotalDiv.Controls[i]).BorderColor = System.Drawing.Color.Empty;
                }
            }

            if (totalTextBox.Text.Length == 0)
            {
                totalTextBox.BorderColor = System.Drawing.Color.Red;
                validatorOutput += "PO Total is empty. <br>";
            }
            else
            {
                totalTextBox.BorderColor = System.Drawing.Color.Empty;
            }

            return validatorOutput;
        }

        protected string CheckIfNumbers()
        {
            string validatorOutput = string.Empty;
            decimal checkBucket = 0;

            foreach (int i in createListOfLineItemIndexes())
            {
                //TryParse doesn't fail if blank, gives double error 'is blank and not a number' if we don't check that again here.
                if (!decimal.TryParse(((TextBox)lineItemsUnitsDiv.Controls[i]).Text, out checkBucket) && ((TextBox)lineItemsUnitsDiv.Controls[i]).Text.Length != 0)
                {
                    ((TextBox)lineItemsUnitsDiv.Controls[i]).BorderColor = System.Drawing.Color.Red;
                    validatorOutput += "Number of units for " + ((TextBox)lineItemDescDiv.Controls[i]).Text.TrimEnd(' ', '.') + " is not a number. <br>";
                }
                else
                {
                    //changes color back if empy unless we check that
                    if (((TextBox)lineItemsUnitsDiv.Controls[i]).Text.Length == 0)
                        ((TextBox)lineItemsUnitsDiv.Controls[i]).BorderColor = System.Drawing.Color.Red;
                    else
                        ((TextBox)lineItemsUnitsDiv.Controls[i]).BorderColor = System.Drawing.Color.Empty;
                }

                if (!decimal.TryParse(((TextBox)lineItemsUnitPriceDiv.Controls[i]).Text, out checkBucket) && ((TextBox)lineItemsUnitPriceDiv.Controls[i]).Text.Length != 0)
                {
                    ((TextBox)lineItemsUnitPriceDiv.Controls[i]).BorderColor = System.Drawing.Color.Red;
                    validatorOutput += "Unit Price for " + ((TextBox)lineItemDescDiv.Controls[i]).Text.TrimEnd(' ', '.') + " is not a number. <br>";
                }
                else
                {
                    if (((TextBox)lineItemsUnitPriceDiv.Controls[i]).Text.Length == 0)
                        ((TextBox)lineItemsUnitPriceDiv.Controls[i]).BorderColor = System.Drawing.Color.Red;
                    else
                        ((TextBox)lineItemsUnitPriceDiv.Controls[i]).BorderColor = System.Drawing.Color.Empty;
                }

                if (!decimal.TryParse(((TextBox)lineItemsTotalDiv.Controls[i]).Text, out checkBucket) && ((TextBox)lineItemsTotalDiv.Controls[i]).Text.Length != 0)
                {
                    ((TextBox)lineItemsTotalDiv.Controls[i]).BorderColor = System.Drawing.Color.Red;
                    validatorOutput += "Line Total for " + ((TextBox)lineItemDescDiv.Controls[i]).Text.TrimEnd(' ', '.') + " is not a number. <br>";
                }
                else
                {
                    if (((TextBox)lineItemsTotalDiv.Controls[i]).Text.Length == 0)
                        ((TextBox)lineItemsTotalDiv.Controls[i]).BorderColor = System.Drawing.Color.Red;
                    else
                        ((TextBox)lineItemsTotalDiv.Controls[i]).BorderColor = System.Drawing.Color.Empty;
                }
            }

            if (!decimal.TryParse(totalTextBox.Text, out checkBucket) && totalTextBox.Text.Length != 0)
            {
                totalTextBox.BorderColor = System.Drawing.Color.Red;
                validatorOutput += "PO Total is not a number. <br>";
            }
            else
            {
                if (totalTextBox.Text.Length == 0)
                    totalTextBox.BorderColor = System.Drawing.Color.Red;
                else
                    totalTextBox.BorderColor = System.Drawing.Color.Empty;
            }

            return validatorOutput;
        }

        protected string CheckTotals()
        {
            string validatorOutput = string.Empty;
            decimal units = 0;
            decimal price = 0;
            decimal lineTotal = 0;
            decimal total = 0;
            decimal totalCheck = 0;
            decimal closeEnough = (decimal)0.001;

            foreach (int i in createListOfLineItemIndexes())
            {

                if ((decimal.TryParse(((TextBox)lineItemsUnitsDiv.Controls[i]).Text, out units)
                    && decimal.TryParse(((TextBox)lineItemsUnitPriceDiv.Controls[i]).Text, out price)
                    && decimal.TryParse(((TextBox)lineItemsTotalDiv.Controls[i]).Text, out lineTotal)) && ((TextBox)lineItemsTotalDiv.Controls[i]).Enabled)
                {

                    if ( Math.Abs((units * price)-lineTotal) > closeEnough  )
                    {
                        ((TextBox)lineItemsTotalDiv.Controls[i]).BorderColor = System.Drawing.Color.Red;
                        validatorOutput += "A line Total in item is not correct (Units * Price = Total). <br>";
                    }
                    else
                    {
                        ((TextBox)lineItemsTotalDiv.Controls[i]).BorderColor = System.Drawing.Color.Empty;
                    }
                }

                //Put in second if statment, so if units or prie is blank, but there's still a total, it'll use it
                if (decimal.TryParse(((TextBox)lineItemsTotalDiv.Controls[i]).Text, out lineTotal))
                {
                    totalCheck += lineTotal;
                }
            }

            if (decimal.TryParse(totalTextBox.Text, out total))
            {
                if (Math.Abs(total - totalCheck) > closeEnough )
                {
                    totalTextBox.BorderColor = System.Drawing.Color.Red;
                    validatorOutput += "PO Total doesn't add up. <br>";
                }
                else
                {
                    totalTextBox.BorderColor = System.Drawing.Color.Empty;
                }
            }

            return validatorOutput;
        }
        protected bool ValidateAll()
        {
            validatorOutput.Text = string.Empty;

            validatorOutput.Text += CheckForBlanks();
            validatorOutput.Text += CheckIfNumbers();
            validatorOutput.Text += CheckTotals();

            //show message if anything fails
            if (validatorOutput.Text != string.Empty)
            {
                validatorOutput.Visible = true;
                return false;
            }
            else
            {
                validatorOutput.Visible = false;
                return true;
            }
        }
        
        protected void fillInButton_Click(object sender, EventArgs e)
        {
            validatorOutput.Text = string.Empty;

            //Make sure it's only numbers in the text boxes we're abotu to do math on.
            validatorOutput.Text += CheckIfNumbers();

            //If anything fails the checkIfNumbers() check, show the error.
            if (validatorOutput.Text != string.Empty)
            {
                validatorOutput.Visible = true;
            }
            else
            {
                validatorOutput.Visible = false;
                doMath(createListOfLineItemIndexes());
            }
        }
        protected void doMath(List<int> indexes)
        {
            decimal runningTotal = 0;

            foreach (int i in indexes)
            {
                //have units and unit price, but missing line total
                if (((TextBox)lineItemsTotalDiv.Controls[i]).Enabled)
                {
                    if (((TextBox)lineItemsUnitsDiv.Controls[i]).Text != "" && ((TextBox)lineItemsUnitPriceDiv.Controls[i]).Text != "" && ((TextBox)lineItemsTotalDiv.Controls[i]).Text == "")
                    {
                        decimal units = decimal.Parse(((TextBox)lineItemsUnitsDiv.Controls[i]).Text);
                        decimal unitPrice = decimal.Parse(((TextBox)lineItemsUnitPriceDiv.Controls[i]).Text);
                        decimal lineTotal = units * unitPrice;
                        ((TextBox)lineItemsTotalDiv.Controls[i]).Text = lineTotal.ToString();
                        ((TextBox)lineItemsTotalDiv.Controls[i]).BackColor = System.Drawing.Color.Lavender;
                    }

                    //have units and line total, but missing unit price
                    if (((TextBox)lineItemsUnitsDiv.Controls[i]).Text != "" && ((TextBox)lineItemsUnitPriceDiv.Controls[i]).Text == "" && ((TextBox)lineItemsTotalDiv.Controls[i]).Text != "")
                    {
                        decimal units = decimal.Parse(((TextBox)lineItemsUnitsDiv.Controls[i]).Text);
                        decimal lineTotal = decimal.Parse(((TextBox)lineItemsTotalDiv.Controls[i]).Text);
                        decimal unitPrice = lineTotal / units;
                        ((TextBox)lineItemsUnitPriceDiv.Controls[i]).Text = unitPrice.ToString();
                        ((TextBox)lineItemsUnitPriceDiv.Controls[i]).BackColor = System.Drawing.Color.Lavender;
                    }

                    //have line total and unit price, but missing units
                    if (((TextBox)lineItemsUnitsDiv.Controls[i]).Text == "" && ((TextBox)lineItemsUnitPriceDiv.Controls[i]).Text != "" && ((TextBox)lineItemsTotalDiv.Controls[i]).Text != "")
                    {
                        decimal lineTotal = decimal.Parse(((TextBox)lineItemsTotalDiv.Controls[i]).Text);
                        decimal unitPrice = decimal.Parse(((TextBox)lineItemsUnitPriceDiv.Controls[i]).Text);
                        decimal units = lineTotal / unitPrice;
                        ((TextBox)lineItemsUnitsDiv.Controls[i]).Text = units.ToString();
                        ((TextBox)lineItemsUnitsDiv.Controls[i]).BackColor = System.Drawing.Color.Lavender;
                    }
                }

                //add up all line totals
                if (((TextBox)lineItemsTotalDiv.Controls[i]).Text != "")
                {
                    runningTotal += decimal.Parse(((TextBox)lineItemsTotalDiv.Controls[i]).Text);
                }
            }

            totalTextBox.Text = runningTotal.ToString();
            totalTextBox.BackColor = System.Drawing.Color.Lavender;
        }

        protected void deletePOButton_Click(object sender, EventArgs e)
        {
            int poNumb = (int)Session["poNumb"];
            bool checkIfCan = true;
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
            SqlCommand deleteLineItems = new SqlCommand("DELETE FROM PurchaseOrderLineItems WHERE lineitemPoPK = @poPK", conn);
            deleteLineItems.Parameters.AddWithValue("@poPK", poNumb);
            SqlCommand deletePO = new SqlCommand("DELETE FROM PurchaseOrders WHERE poPK = @poPK", conn);
            deletePO.Parameters.AddWithValue("@poPK", poNumb);

            foreach (int i in createListOfLineItemIndexes())
            {
                if (lineNumbDiv.Controls[i] is CheckBox)
                {
                    if (!((CheckBox)lineNumbDiv.Controls[i]).Enabled)
                        checkIfCan = false;
                }
            }

            if (checkIfCan)
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                    deleteLineItems.ExecuteNonQuery();
                    conn.Close();

                    conn.Open();
                    deletePO.ExecuteNonQuery();
                    conn.Close();
                }

                Response.Redirect(HttpContext.Current.Request.Url.ToString(), true);
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this, GetType(), "alertMessage", string.Format("alert('YOu need edit permision for all departmerts in a PO to delete it.')"), true);
            }
        }
    }
}