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
    public partial class demoNewPO : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //If date not set, set it to today.
            if (poDateCalendar.SelectedDate.ToShortDateString().Equals("1/1/1900"))
            {
                poDateCalendar.SelectedDate = DateTime.Today.Date;
                poDateCalendar.VisibleDate = DateTime.Today.Date;
            }
            if (Session["poDate"] == null)
            {
                poDateLinkButton.Text = DateTime.Today.Date.ToShortDateString();
            }
            else
            {
                poDateLinkButton.Text = Session["poDate"].ToString();
            }

            //These are only null on first page load
            if (ViewState["lineItems"] == null)
            {
                ViewState["lineItems"] = 1;
            }
            //Create one empty line item on first page load
            for (int i = 1; i <= (int)ViewState["lineItems"]; i++)
            {
                addLineItems(i);
            }

            if (ViewState["poTotal"] == null)
            {
                ViewState["poTotal"] = 0;
            }

            if (vendorComboBox.SelectedItem == null)
            {
                vendorComboBox.SelectedIndex = 0;
            }

            //Make the PO in the DB on page load! Best way I could come up with to give an internal PO number for the user to use, while still supporting multiple simulatsous users. 
            //If user loads page, but doesn't finish creating a PO before leaving, it will leave an 'abonded' PO in the DB. That just means it's a PO with lots of null values.
            //These don't cause any problems, but if you want to get rid of them, use this SQL command: 
            //DELETE PurchaseOrders FROM PurchaseOrders LEFT JOIN PurchaseOrderLineItems ON PurchaseOrders.poPK = PurchaseOrderLineItems.lineitemPoPK WHERE lineitemPK is NULL; 
            if (Session["poRandSessionNumb"] == null)
            {
                CreateBlankPO();
                poNumbLabel.Text = Session["poNumb"].ToString();
            }
            else
            {
                poNumbLabel.Text = Session["poNumb"].ToString();
            }
        }

        protected void CreateBlankPO()
        {
            int randNumb = (new Random()).Next(1000, 1000000);
            int poNumb = 0;

            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
            SqlCommand createNewPO = new SqlCommand("INSERT INTO PurchaseOrders (poCreator, poRandSessionNumb) VALUES (@user, @randSessionNumb);", conn);
            SqlCommand getPoNumb = new SqlCommand("SELECT * FROM PurchaseOrders WHERE poRandSessionNumb = @randSessionNumb;", conn);
            createNewPO.Parameters.AddWithValue("@randSessionNumb", randNumb);
            createNewPO.Parameters.AddWithValue("@user", User.Identity.Name);
            getPoNumb.Parameters.AddWithValue("@randSessionNumb", randNumb);

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();

                createNewPO.ExecuteNonQuery();
                SqlDataReader readPO = getPoNumb.ExecuteReader();

                while (readPO.Read())
                {
                    poNumb = (int)readPO["poPK"];
                }

                conn.Close();
            }
            Session["poNumb"] = poNumb;
            Session["poRandSessionNumb"] = randNumb;
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

        protected void newLineButton_Click(object sender, EventArgs e)
        {
            int numberOfLineItems = (int)ViewState["lineItems"];
            ViewState["lineItems"] = numberOfLineItems + 1;
            addLineItems((int)ViewState["lineItems"]);
        }

        protected void addLineItems(int lineNumber)
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
            itemDepartmentDropDown.DataSourceID = "departmentsDataSource";
            itemDepartmentDropDown.DataTextField = "departmentName";
            itemDepartmentDropDown.SelectedIndex = departmentDropDown.SelectedIndex;
            itemDepartmentDropDown.CssClass = "item";
            DropDownList itemGrantsDropDown = new DropDownList();
            itemGrantsDropDown.DataSourceID = "grantsDataSource";
            itemGrantsDropDown.DataTextField = "grantName";
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

        protected void vendorDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            vendorInfoDisplay();
        }

        protected void vendorInfoDisplay()
        {
            //Makes it work "right" with add new vendor
            string selectedVendorName = string.Empty;
            if (object.Equals(null, vendorComboBox.SelectedItem))
            {
                if (vendorNewNameTextBox.Text.Length != 0)
                {
                    selectedVendorName = vendorNewNameTextBox.Text;
                    vendorComboBox.SelectedIndex = vendorComboBox.Items.IndexOf(vendorComboBox.Items.FindByText(selectedVendorName));
                    vendorNewNameTextBox.Text = string.Empty;
                    vendorNewAddressTextBox.Text = string.Empty;
                    vendorNewPhoneTextBox.Text = string.Empty;
                    vendorNewCommentsTextBox.Text = string.Empty;
                }
            }
            else
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

        protected void vendorDropDownList_DataBound(object sender, EventArgs e)
        {
            vendorInfoDisplay();
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

        protected void doMath(List<int> indexes)
        {
            decimal runningTotal = 0;

            foreach (int i in indexes)
            {
                //have units and unit price, but missing line total
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

                //add up all line totals
                if (((TextBox)lineItemsTotalDiv.Controls[i]).Text != "")
                {
                    runningTotal += decimal.Parse(((TextBox)lineItemsTotalDiv.Controls[i]).Text);
                }
            }

            totalTextBox.Text = runningTotal.ToString();
            totalTextBox.BackColor = System.Drawing.Color.Lavender;
        }

        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            if (checkAuthorizedToMakePO())
            {
                if (ValidateAll())
                {
                    updatePO();
                    submitLineItems();

                    //Need to clear these to make new PO
                    Session["poNumb"] = null;
                    Session["poRandSessionNumb"] = null;
                    clearButton_Click(sender, e);
                }
            }
        }

        protected void updatePO()
        {
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
            SqlCommand updatePO = new SqlCommand("UPDATE PurchaseOrders SET poPayment = @payment, poTotal = @poTotal, poDateCreated = @dateCreated , poVendor = @vendor, poVendorPoNumb = @vendorPoNumb , poComments = @comments WHERE poPK = @poNumb", conn);

            updatePO.Parameters.AddWithValue("@poNumb", (int)Session["poNumb"]);
            updatePO.Parameters.AddWithValue("@payment", paymentDropDown.SelectedValue);
            updatePO.Parameters.AddWithValue("@poTotal", totalTextBox.Text);
            updatePO.Parameters.AddWithValue("@dateCreated", poDateCalendar.SelectedDate);
            updatePO.Parameters.AddWithValue("@vendor", vendorComboBox.SelectedValue);
            updatePO.Parameters.AddWithValue("@vendorPoNumb", (vendInvNumbTextBox.Text == "" ? (object)DBNull.Value : vendInvNumbTextBox.Text));
            updatePO.Parameters.AddWithValue("@comments", (poComments.Text == "" ? (object)DBNull.Value : poComments.Text));

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                updatePO.ExecuteNonQuery();
                conn.Close();
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

            SqlCommand insertLineItems = new SqlCommand(
                "INSERT INTO PurchaseOrderLineItems (lineitemPoPK, lineitemDepartment, lineitemGrant, lineitemDescription, lineitemUnits, lineitemUnitPrice, lineitemTotalPrice) " +
                                            "VALUES (@lineitemPoPK, @lineitemDepartment, @lineitemGrant, @lineitemDescription, @lineitemUnits, @lineitemUnitPrice, @lineitemTotalPrice);", conn);
            insertLineItems.Parameters.AddWithValue("@lineitemPoPK", poNumb);
            SqlParameter lineitemDepartment = new SqlParameter("@lineitemDepartment", SqlDbType.Int);
            SqlParameter lineitemGrant = new SqlParameter("@lineitemGrant", SqlDbType.Int);
            SqlParameter lineitemDescription = new SqlParameter("@lineitemDescription", SqlDbType.NVarChar);
            SqlParameter lineitemUnits = new SqlParameter("@lineitemUnits", SqlDbType.Decimal);
            SqlParameter lineitemUnitPrice = new SqlParameter("@lineitemUnitPrice", SqlDbType.Decimal);
            SqlParameter lineitemTotalPrice = new SqlParameter("@lineitemTotalPrice", SqlDbType.Decimal);
            insertLineItems.Parameters.Add(lineitemDepartment);
            insertLineItems.Parameters.Add(lineitemGrant);
            insertLineItems.Parameters.Add(lineitemDescription);
            insertLineItems.Parameters.Add(lineitemUnits);
            insertLineItems.Parameters.Add(lineitemUnitPrice);
            insertLineItems.Parameters.Add(lineitemTotalPrice);

            foreach (int i in createListOfLineItemIndexes())
            {
                //get deparmentPK
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                    selectPK.Parameters["@name"].Value = ((DropDownList)lineItemsDepartmentDiv.Controls[i]).SelectedItem.Text;
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
                    selectPK.Parameters["@name"].Value = ((DropDownList)lineItemsGrantDiv.Controls[i]).SelectedItem.Text;
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
                    insertLineItems.Parameters["@lineitemDescription"].Value = ((TextBox)lineItemDescDiv.Controls[i]).Text;
                    insertLineItems.Parameters["@lineitemUnits"].Value = decimal.Parse(((TextBox)lineItemsUnitsDiv.Controls[i]).Text);
                    insertLineItems.Parameters["@lineitemUnitPrice"].Value = decimal.Parse(((TextBox)lineItemsUnitPriceDiv.Controls[i]).Text);
                    insertLineItems.Parameters["@lineitemTotalPrice"].Value = decimal.Parse(((TextBox)lineItemsTotalDiv.Controls[i]).Text);
                    insertLineItems.ExecuteNonQuery();

                    conn.Close();
                }
            }

            if (conn.State == ConnectionState.Open)
                conn.Close();
        }

        protected void clearButton_Click(object sender, EventArgs e)
        {
            ViewState.Clear();
            Response.Redirect(Request.RawUrl);
        }

        protected void departmentDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (int i in createListOfLineItemIndexes())
            {
                ((DropDownList)lineItemsDepartmentDiv.Controls[i]).SelectedIndex = departmentDropDown.SelectedIndex;
            }
        }

        protected void grantDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (int i in createListOfLineItemIndexes())
            {
                ((DropDownList)lineItemsGrantDiv.Controls[i]).SelectedIndex = grantDropDown.SelectedIndex;
            }
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

        protected string CheckForBlanks()
        {
            string validatorOutput = string.Empty;

            foreach (int i in createListOfLineItemIndexes())
            {
                if (((TextBox)lineItemDescDiv.Controls[i]).Text.Trim().Length == 0)
                {
                    ((TextBox)lineItemDescDiv.Controls[i]).BorderColor = System.Drawing.Color.Red;
                    validatorOutput += "Item Description in line #" + ((Label)lineNumbDiv.Controls[i]).Text.TrimEnd(' ', '.') + " can\'t be blank. <br>";
                }
                else
                {
                    ((TextBox)lineItemDescDiv.Controls[i]).BorderColor = System.Drawing.Color.Empty;
                }

                if (((TextBox)lineItemsUnitsDiv.Controls[i]).Text.Trim().Length == 0)
                {
                    ((TextBox)lineItemsUnitsDiv.Controls[i]).BorderColor = System.Drawing.Color.Red;
                    validatorOutput += "Number of units in line #" + ((Label)lineNumbDiv.Controls[i]).Text.TrimEnd(' ', '.') + " can\'t be blank. <br>";
                }
                else
                {
                    ((TextBox)lineItemsUnitsDiv.Controls[i]).BorderColor = System.Drawing.Color.Empty;
                }

                if (((TextBox)lineItemsUnitPriceDiv.Controls[i]).Text.Trim().Length == 0)
                {
                    ((TextBox)lineItemsUnitPriceDiv.Controls[i]).BorderColor = System.Drawing.Color.Red;
                    validatorOutput += "Unit Price in line #" + ((Label)lineNumbDiv.Controls[i]).Text.TrimEnd(' ', '.') + " can\'t be blank. <br>";
                }
                else
                {
                    ((TextBox)lineItemsUnitPriceDiv.Controls[i]).BorderColor = System.Drawing.Color.Empty;
                }

                if (((TextBox)lineItemsTotalDiv.Controls[i]).Text.Trim().Length == 0)
                {
                    ((TextBox)lineItemsTotalDiv.Controls[i]).BorderColor = System.Drawing.Color.Red;
                    validatorOutput += "Line Total in line #" + ((Label)lineNumbDiv.Controls[i]).Text.TrimEnd(' ', '.') + " can\'t be blank. <br>";
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
                    validatorOutput += "Number of units in line #" + ((Label)lineNumbDiv.Controls[i]).Text.TrimEnd(' ', '.') + " is not a number. <br>";
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
                    validatorOutput += "Unit Price in line #" + ((Label)lineNumbDiv.Controls[i]).Text.TrimEnd(' ', '.') + " is not a number. <br>";
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
                    validatorOutput += "Line Total in line #" + ((Label)lineNumbDiv.Controls[i]).Text.TrimEnd(' ', '.') + " is not a number. <br>";
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

            foreach (int i in createListOfLineItemIndexes())
            {
                if (decimal.TryParse(((TextBox)lineItemsUnitsDiv.Controls[i]).Text, out units)
                    && decimal.TryParse(((TextBox)lineItemsUnitPriceDiv.Controls[i]).Text, out price)
                    && decimal.TryParse(((TextBox)lineItemsTotalDiv.Controls[i]).Text, out lineTotal))
                {

                    if (units * price != lineTotal)
                    {
                        ((TextBox)lineItemsTotalDiv.Controls[i]).BorderColor = System.Drawing.Color.Red;
                        validatorOutput += "Line Total in item #" + ((Label)lineNumbDiv.Controls[i]).Text.TrimEnd(' ', '.') + " is not correct (Units * Price = Total). <br>";
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
                if (total != totalCheck)
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

        public bool checkAuthorizedToMakePO()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            IList<string> usersRoles = userManager.GetRoles(User.Identity.GetUserId());
            string department = string.Empty;
            List<string> notInTheseDepartments = new List<string>();

            if (!userManager.IsInRole(User.Identity.GetUserId(), "_Admin") && !userManager.IsInRole(User.Identity.GetUserId(), "_Purchasing Agent"))
            {
                foreach (int i in createListOfLineItemIndexes())
                {
                    department = ((DropDownList)lineItemsDepartmentDiv.Controls[i]).SelectedItem.Text;
                    if (notInTheseDepartments.IndexOf(department) == -1)
                        notInTheseDepartments.Add(department);


                    foreach (string role in usersRoles)
                    {
                        if (role.IndexOf("_MakePO") != -1)
                        {
                            if (role == department + "_MakePO")
                                notInTheseDepartments.RemoveAll(x => x == department);
                        }
                    }
                }
            }

            if (notInTheseDepartments.Count > 0)
            {
                if (notInTheseDepartments.Count == 1)
                {
                    ScriptManager.RegisterClientScriptBlock(this, GetType(), "alertMessage", string.Format("alert('You are not allowed to make POs for the {0} department. Ask a PO Admin to add you to the MakePO group for the {0} department.')", notInTheseDepartments[0].ToString()), true);
                }
                else if (notInTheseDepartments.Count > 1)
                {
                    department = string.Empty;
                    foreach (string notDept in notInTheseDepartments)
                        department += notDept + ", ";
                    ScriptManager.RegisterClientScriptBlock(this, GetType(), "alertMessage", string.Format("alert('You are not allowed to make POs for the {0}departments. Ask a PO Admin to add you to the MakePO group for the {0}departments.')", department), true);
                }
                return false;
            }
            else
                return true;
        }
    }
}