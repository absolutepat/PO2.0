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
    public partial class purchasingAgent : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!User.IsInRole("_Admin") && !User.IsInRole("_Purchasing Agent"))
            {
                Response.Redirect("~/Default.aspx");
            }

            if (!Page.IsPostBack)
            {
                fillListsAndDrops();
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
            fillUsersDropDown();
            fillCheckBoxList();
            fillDeparartmentsDropDown();
            fillGrantsDropDown();
            fillPaymentsDropDown();
        }

        protected void fillUsersDropDown()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            usersDropDown.DataSource = userManager.Users.ToList();
            usersDropDown.DataTextField = "Email";
            usersDropDown.DataValueField = "ID";
            usersDropDown.DataBind();
        }

        protected void fillCheckBoxList()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var rolesList = roleManager.Roles.ToList();
            //rolesList.Remove(roleManager.FindByName("_Admin"));
            //rolesList.Add(new IdentityRole("_x"));
            rolesList.Add(new IdentityRole("_x"));
            rolesList.Sort((x, y) => string.Compare(x.Name, y.Name));// OrderBy(x => x.Name);

            groupsCheckBoxList.Items.Clear();
            groupsCheckBoxList.DataSource = rolesList;
            groupsCheckBoxList.DataTextField = "Name";
            groupsCheckBoxList.DataValueField = "ID";
            groupsCheckBoxList.DataBind();
            groupsCheckBoxList.Items.FindByText("_Admin").Enabled = false;
            groupsCheckBoxList.Items.FindByText("_x").Value = "X";
            groupsCheckBoxList.Items.FindByValue("X").Text = "Everything but Purchasing.";            

            if (userManager.FindById(usersDropDown.SelectedValue).Roles.Count > 0)
            {
                foreach (IdentityUserRole role in userManager.FindById(usersDropDown.SelectedValue).Roles)
                {
                    foreach (ListItem checkbox in groupsCheckBoxList.Items)
                    {
                        if (checkbox.Value == role.RoleId)
                            checkbox.Selected = true;
                    }
                }
            }

            groupsCheckBoxList.Items.FindByText("_Admin").Value = "cantCheangeOnThisPage";
            groupsCheckBoxList.Items.FindByText("_Admin").Text = "Admin";
        }

        protected void fillDeparartmentsDropDown()
        {
            editDepartmentDropDown.DataSource = GetDepartmentInfo();
            editDepartmentDropDown.DataTextField = "Item2";
            editDepartmentDropDown.DataValueField = "Item1";
            editDepartmentDropDown.DataBind();
            editDepartmentDropDown.Items.Add("Select Department");
            editDepartmentDropDown.Items.FindByText("Select Department").Selected = true;
        }

        protected void fillGrantsDropDown()
        {
            editGrantDropDown.DataSource = getGrantInfo();
            editGrantDropDown.DataTextField = "Item2";
            editGrantDropDown.DataValueField = "Item1";
            editGrantDropDown.DataBind();
            editGrantDropDown.Items.FindByText(" None").Selected = true;
        }
        protected void fillPaymentsDropDown()
        {
            newPaymentSourceTypeDropDown.DataSource = GetPaymentTypeInfo();
            newPaymentSourceTypeDropDown.DataTextField = "Item2";
            newPaymentSourceTypeDropDown.DataValueField = "Item1";
            newPaymentSourceTypeDropDown.DataBind();

            editPaymentSourceDropDown.DataSource = GetPaymentInfo(1);            
            editPaymentSourceDropDown.DataTextField = "Item4";
            editPaymentSourceDropDown.DataValueField = "Item1";
            editPaymentSourceDropDown.DataBind();
            editPaymentSourceDropDown.Items.Add("Select Department");
            editPaymentSourceDropDown.Items.FindByText("Select Department").Selected = true;

            editPaymentSourceTypeDropDown.DataSource = GetPaymentTypeInfo();
            editPaymentSourceTypeDropDown.DataTextField = "Item2";
            editPaymentSourceTypeDropDown.DataValueField = "Item1";
            editPaymentSourceTypeDropDown.DataBind();

            deletePaymentSourceDropDown.DataSource = GetPaymentInfo(1);
            deletePaymentSourceDropDown.DataTextField = "Item4"; ;
            deletePaymentSourceDropDown.DataValueField = "Item1";
            deletePaymentSourceDropDown.DataBind();

            inactivePaymentSourcesDropDown.DataSource = GetPaymentInfo(0);
            inactivePaymentSourcesDropDown.DataTextField = "Item4"; ;
            inactivePaymentSourcesDropDown.DataValueField = "Item1";
            inactivePaymentSourcesDropDown.DataBind();
        }

        protected void changeUserPermisionPlaceholderShowHideButton_Click(object sender, EventArgs e)
        {
            changeUserPermisionPlaceholder.Visible = (changeUserPermisionPlaceholder.Visible == true ? false : true);
        }

        protected void changeUserPermisionInstructionsShowHideButton_Click(object sender, EventArgs e)
        {
            changeUserPermisionInstructionsPlaceholder.Visible = (changeUserPermisionInstructionsPlaceholder.Visible == true ? false : true);
            changeUserPermisionInstructionsShowHideButton.Text = (changeUserPermisionInstructionsPlaceholder.Visible == true ? "Hide Instructions" : "Show Instructions");
        }

        protected void usersDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            fillCheckBoxList();
            changeUserGroupResultLabel.Text = string.Empty;
        }

        protected void changeUserGroupButton_Click(object sender, EventArgs e)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            RoleStore<IdentityRole> makeUserToTheseRoles = new RoleStore<IdentityRole>();
            List<IdentityResult> results = new List<IdentityResult>();

            if (groupsCheckBoxList.Items.FindByValue("X").Selected == true)
            {
                foreach (ListItem checkbox in groupsCheckBoxList.Items)
                {
                    if (checkbox.Text != "_Purchasing Agent" && roleManager.RoleExists(checkbox.Text))
                        results.Add(userManager.AddToRole(usersDropDown.SelectedValue, checkbox.Text));
                    else if (checkbox.Text == "_Purchasing Agent" && roleManager.RoleExists(checkbox.Text))
                        results.Add(userManager.RemoveFromRole(usersDropDown.SelectedValue, checkbox.Text));
                }
            }
            else
            {
                foreach (ListItem checkbox in groupsCheckBoxList.Items)
                {
                    if (checkbox.Selected && roleManager.RoleExists(checkbox.Text))
                        results.Add(userManager.AddToRole(usersDropDown.SelectedValue, checkbox.Text));
                    else
                        results.Add(userManager.RemoveFromRole(usersDropDown.SelectedValue, checkbox.Text));
                }
            }

            changeUserGroupResultLabel.Text = "Changes submited.";
        }

        protected void manageDepartmentPlaceholderShowHideButton_Click(object sender, EventArgs e)
        {
            manageDepartmentsPlaceholder.Visible = (manageDepartmentsPlaceholder.Visible == true ? false : true);
        }

        protected void manageDepartmentsInstructionsShowHideButton_Click(object sender, EventArgs e)
        {
            manageDepartmentsInstructionsPlaceholder.Visible = (manageDepartmentsInstructionsPlaceholder.Visible == true ? false : true);
            manageDepartmentsInstructionsShowHideButton.Text = (manageDepartmentsInstructionsPlaceholder.Visible == true ? "Hide Instructions" : "Show Instructions");
        }

        protected void createDepartmentButton_Click(object sender, EventArgs e)
        {
            if (checkIfDepartmentExists(createDepartmentNameTextBox.Text))
            {
                SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
                SqlCommand createDepartment = new SqlCommand("INSERT INTO Departments (departmentName, departmentComments) VALUES (@departmentName, @departmentComments);", conn);
                createDepartment.Parameters.AddWithValue("@departmentName", createDepartmentNameTextBox.Text);
                createDepartment.Parameters.AddWithValue("@departmentComments", createDepartmentCommentsTextBox.Text);

                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                createDepartment.ExecuteNonQuery();

                if (conn.State == ConnectionState.Open)
                    conn.Close();

                createRolesForDepartment(createDepartmentNameTextBox.Text);
                Response.Redirect(HttpContext.Current.Request.Url.ToString(), true);
            }
        }

        protected bool checkIfDepartmentExists(string deptName)
        {
            List<Tuple<int, string, string, bool>> departmentInfo = GetDepartmentInfo(2);
            bool returnMe = true;

            foreach (Tuple<int, string, string, bool> department in departmentInfo)
            {
                //check Name collision
                if (department.Item2 == deptName)
                {
                    //check if just inactive, give differnt error message
                    if (department.Item4 == false)
                        ScriptManager.RegisterClientScriptBlock(this, GetType(), "alertMessage", string.Format("alert('A department named {0} already exists but is inactive. Either change it to active, or create the new department with a differnt name.')", deptName), true);
                    else
                        ScriptManager.RegisterClientScriptBlock(this, GetType(), "alertMessage", string.Format("alert('A department named {0} already exists and is active. Try creating the new department with a differnt name.')", deptName), true);

                    returnMe = false;
                }
            }

            return returnMe;
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

        public void createRolesForDepartment(string deptName)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            IdentityRole newRoleBucket = null;
            string make = deptName + "_MakePO";
            string review = deptName + "_ReviewAll";
            string edit = deptName + "_EditAll";

            if (!roleManager.RoleExists(make))
            {
                newRoleBucket = new IdentityRole();
                newRoleBucket.Name = make;
                roleManager.Create(newRoleBucket);
            }
            if (!roleManager.RoleExists(review))
            {
                newRoleBucket = new IdentityRole();
                newRoleBucket.Name = review;
                roleManager.Create(newRoleBucket);
            }
            if (!roleManager.RoleExists(edit))
            {
                newRoleBucket = new IdentityRole();
                newRoleBucket.Name = edit;
                roleManager.Create(newRoleBucket);
            }
        }

        protected void editDepartmentDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<Tuple<int, string, string, bool>> departmentInfo = GetDepartmentInfo();

            foreach (Tuple<int, string, string, bool> department in departmentInfo)
            {
                if (department.Item1.ToString() == editDepartmentDropDown.SelectedValue)
                {
                    editDepartmentNameTextBox.Text = department.Item2;
                    editDepartmentCommentsTextBox.Text = department.Item3;
                }
            }
        }
        protected void editDepartmentSubmitButtons_Click(object sender, EventArgs e)
        {
            if (checkIfDepartmentExists(editDepartmentNameTextBox.Text) || editDepartmentNameTextBox.Text == editDepartmentDropDown.SelectedItem.Text)
            {
                SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
                SqlCommand editDepartment = new SqlCommand("UPDATE Departments SET departmentName = @departmentName, departmentComments = @departmentComments WHERE departmentPK = @departmentPK;", conn);

                editDepartment.Parameters.AddWithValue("@departmentName", editDepartmentNameTextBox.Text);
                editDepartment.Parameters.AddWithValue("@departmentComments", editDepartmentCommentsTextBox.Text);
                editDepartment.Parameters.AddWithValue("@departmentPK", editDepartmentDropDown.SelectedValue);

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                    editDepartment.ExecuteNonQuery();
                    conn.Close();
                }

                if (editDepartmentDropDown.SelectedItem.Text != editDepartmentNameTextBox.Text)
                {
                    updateRoleNamesOnDepartmentNameEdit(editDepartmentDropDown.SelectedItem.Text, editDepartmentNameTextBox.Text);
                }

                Response.Redirect(HttpContext.Current.Request.Url.ToString(), true);


            }
        }

        protected void updateRoleNamesOnDepartmentNameEdit(string oldDeptName, string newDeptName)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            IdentityRole theMakeRole = roleManager.FindByName(oldDeptName + "_MakePO");
            IdentityRole theReviewRole = roleManager.FindByName(oldDeptName + "_ReviewAll");
            IdentityRole theEditRole = roleManager.FindByName(oldDeptName + "_EditAll");

            theMakeRole.Name = newDeptName + "_MakePO";
            theReviewRole.Name = newDeptName + "_ReviewAll";
            theEditRole.Name = newDeptName + "_EditAll";

            roleManager.Update(theMakeRole);
            roleManager.Update(theReviewRole);
            roleManager.Update(theEditRole);
        }

        protected void manageGrantsPlaceholderShowHideButton_Click(object sender, EventArgs e)
        {
            manageGrantsPlaceholder.Visible = (manageGrantsPlaceholder.Visible == true ? false : true);
        }

        protected void manageGrantsInstructionsPlaceholderShowHideButton_Click(object sender, EventArgs e)
        {
            manageGrantsInstructionsPlaceholer.Visible = (manageGrantsInstructionsPlaceholer.Visible == true ? false : true);
            manageGrantsInstructionsPlaceholderShowHideButton.Text = (manageGrantsInstructionsPlaceholer.Visible == true ? "Hide Instructions" : "Show Instructions");
        }

        protected void createGrantSubmitButton_Click(object sender, EventArgs e)
        {
            string validationErrors = validateGrantTextBoxes(createGrantNameTextBox.Text, createGrantStartTextBox.Text, createGrantEndTextBox.Text, createGrantAmountTextBox.Text);

            if (validationErrors.Length <= 0)
            {
                if (checkIfGrantExists(createGrantNameTextBox.Text))
                {
                    SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
                    SqlCommand createGrant = new SqlCommand("INSERT INTO Grants (grantName, grantStartDate, grantEndDate, grantAmount, grantComments) VALUES (@grantName, @grantStartDate, @grantEndDate, @grantAmount, @grantComments);", conn);
                    createGrant.Parameters.AddWithValue("@grantName", createGrantNameTextBox.Text);
                    createGrant.Parameters.AddWithValue("@grantStartDate", Convert.ToDateTime(createGrantStartTextBox.Text));
                    createGrant.Parameters.AddWithValue("@grantEndDate", Convert.ToDateTime(createGrantEndTextBox.Text));
                    createGrant.Parameters.AddWithValue("@grantAmount", Convert.ToDecimal(createGrantAmountTextBox.Text));
                    createGrant.Parameters.AddWithValue("@grantComments", createGrantCommentsTextBox.Text);

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                        createGrant.ExecuteNonQuery();
                        conn.Close();
                    }

                    Response.Redirect(HttpContext.Current.Request.Url.ToString(), true);
                }
            }
            else
                createGrantErrorsLabel.Text = validationErrors;
        }

        protected void editGrantSubmitButton_Click(object sender, EventArgs e)
        {
            string validationErrors = validateGrantTextBoxes(editGrantNameTextBox.Text, editGrantStartTextBox.Text, editGrantEndTextBox.Text, editGrantAmountTextBox.Text);
            if (editGrantDropDown.SelectedItem.Text != " None")
                if (validationErrors.Length <= 0)
                {

                    if (checkIfGrantExists(editGrantNameTextBox.Text) || editGrantNameTextBox.Text == editGrantDropDown.SelectedItem.Text)
                    {
                        SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
                        SqlCommand createGrant = new SqlCommand("UPDATE Grants SET grantName = @grantName, grantStartDate = @grantStartDate , grantEndDate = @grantEndDate, grantAmount = @grantAmount, grantComments = @grantComments WHERE grantPK = @grantPK;", conn);
                        createGrant.Parameters.AddWithValue("@grantName", editGrantNameTextBox.Text);
                        createGrant.Parameters.AddWithValue("@grantStartDate", Convert.ToDateTime(editGrantStartTextBox.Text));
                        createGrant.Parameters.AddWithValue("@grantEndDate", Convert.ToDateTime(editGrantEndTextBox.Text));
                        createGrant.Parameters.AddWithValue("@grantAmount", Convert.ToDecimal(editGrantAmountTextBox.Text));
                        createGrant.Parameters.AddWithValue("@grantComments", editGrantCommentsTextBox.Text);
                        createGrant.Parameters.AddWithValue("@grantPK", editGrantDropDown.SelectedValue);

                        if (conn.State == ConnectionState.Closed)
                        {
                            conn.Open();
                            createGrant.ExecuteNonQuery();
                            conn.Close();
                        }

                        Response.Redirect(HttpContext.Current.Request.Url.ToString(), true);
                    }
                }
                else
                    editGrantErrorsLabel.Text = validationErrors;
            else
                ScriptManager.RegisterClientScriptBlock(this, GetType(), "alertMessage", string.Format("alert('Dont edit the None grant.')"), true);
        }

        protected string validateGrantTextBoxes(string name, string startDate, string endDate, string amount)
        {
            string returnMe = string.Empty;

            if (name.Length <= 0)
                returnMe += "Name can't be empty. ";

            try { Convert.ToDateTime(startDate); }
            catch { returnMe += "Start Date is not right. "; }

            try { Convert.ToDateTime(endDate); }
            catch { returnMe += "End Date is not right. "; }

            try { Convert.ToDecimal(amount); }
            catch { returnMe += "Grant amount is not right. "; }

            return returnMe;
        }

        protected bool checkIfGrantExists(string grantName)
        {
            List<Tuple<int, string, DateTime, DateTime, decimal, string, bool>> grantInfo = getGrantInfo();
            bool returnMe = true;

            foreach (Tuple<int, string, DateTime, DateTime, decimal, string, bool> grant in grantInfo)
            {
                if (grant.Item2.ToUpper() == grantName.ToUpper())
                {
                    //check if just inactive, give differnt error message
                    if (grant.Item7 == false)
                        ScriptManager.RegisterClientScriptBlock(this, GetType(), "alertMessage", string.Format("alert('A grant named {0} already exists but is inactive. Either change it to active, or create the new grant with a differnt name.')", grantName), true);
                    else
                        ScriptManager.RegisterClientScriptBlock(this, GetType(), "alertMessage", string.Format("alert('A grant named {0} already exists and is active. Try creating the new grant with a differnt name.')", grantName), true);

                    returnMe = false;
                }
            }

            return returnMe;
        }
        protected List<Tuple<int, string, DateTime, DateTime, decimal, string, bool>> getGrantInfo(int inactiveOrActiveOrBoth = 1)
        {

            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
            SqlCommand getGrantInfo = new SqlCommand();
            List<Tuple<int, string, DateTime, DateTime, decimal, string, bool>> grantInfo = new List<Tuple<int, string, DateTime, DateTime, decimal, string, bool>>();
            Tuple<int, string, DateTime, DateTime, decimal, string, bool> addThis;
            getGrantInfo.Connection = conn;

            switch (inactiveOrActiveOrBoth)
            {
                case 0:
                    getGrantInfo.CommandText = "SELECT grantPK, grantName, grantStartDate, grantEndDate, grantAmount, grantComments, grantActive FROM Grants WHERE grantActive = 0;";
                    break;
                case 2:
                    getGrantInfo.CommandText = "SELECT grantPK, grantName, grantStartDate, grantEndDate, grantAmount, grantComments, grantActive FROM Grants;";
                    break;
                case 1:
                default:
                    getGrantInfo.CommandText = "SELECT grantPK, grantName, grantStartDate, grantEndDate, grantAmount, grantComments, grantActive FROM Grants WHERE grantActive = 1;";
                    break;
            }

            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SqlDataReader readGrantInfo = getGrantInfo.ExecuteReader();
            while (readGrantInfo.Read())
            {
                addThis = new Tuple<int, string, DateTime, DateTime, decimal, string, bool>(readGrantInfo.GetInt32(0), readGrantInfo.GetString(1), readGrantInfo.GetDateTime(2), readGrantInfo.GetDateTime(3), readGrantInfo.GetDecimal(4), readGrantInfo.GetString(5), readGrantInfo.GetBoolean(6));
                grantInfo.Add(addThis);
            }

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return grantInfo;
        }

        protected void editGrantDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<Tuple<int, string, DateTime, DateTime, decimal, string, bool>> grantInfo = getGrantInfo();

            foreach (Tuple<int, string, DateTime, DateTime, decimal, string, bool> grant in grantInfo)
            {
                if (grant.Item1.ToString() == editGrantDropDown.SelectedValue)
                {
                    editGrantNameTextBox.Text = grant.Item2;
                    editGrantStartTextBox.Text = grant.Item3.ToString().Split(' ')[0];
                    editGrantEndTextBox.Text = grant.Item4.ToString().Split(' ')[0];
                    editGrantAmountTextBox.Text = grant.Item5.ToString("F2");
                    editGrantCommentsTextBox.Text = grant.Item6;
                }
            }
        }

        protected void markSubmitedPlaceholderShowHideButton_Click(object sender, EventArgs e)
        {
            markSubmitedPlaceholder.Visible = (markSubmitedPlaceholder.Visible == true ? false : true);
        }

        protected void markSubmitedInstructionsPlaceholderShowHideButton_Click(object sender, EventArgs e)
        {
            markSubmitedInstrucstionsPlaceholder.Visible = (markSubmitedInstrucstionsPlaceholder.Visible == true ? false : true);
            markSubmitedInstructionsPlaceholderShowHideButton.Text = (markSubmitedInstrucstionsPlaceholder.Visible == true ? "Hide Instructions" : "Show Instructions");
        }

        protected void printSelectedPOsButton_Submited_Click(object sender, EventArgs e)
        {
            List<int> pksOfPOsToPrint = new List<int>();
            foreach (GridViewRow row in markSubmitedGridView.Rows)
            {
                if (((CheckBox)row.Cells[0].Controls[1]).Checked)
                {
                    pksOfPOsToPrint.Add(Int32.Parse(((LinkButton)row.Cells[1].Controls[1]).Text));
                }
            }

            Session["POsToPrint"] = pksOfPOsToPrint;
            Response.Write("<script>window.open ('/printing.aspx','_blank');</script>");
        }

        protected void printSelectedPOsButton_Complete_Click(object sender, EventArgs e)
        {
            List<int> pksOfPOsToPrint = new List<int>();
            foreach (GridViewRow row in markCompleteGridView.Rows)
            {
                if (((CheckBox)row.Cells[0].Controls[1]).Checked)
                {
                    pksOfPOsToPrint.Add(Int32.Parse(((LinkButton)row.Cells[1].Controls[1]).Text));
                }
            }

            Session["POsToPrint"] = pksOfPOsToPrint;
            Response.Write("<script>window.open ('/printing.aspx','_blank');</script>");
        }

        protected void markCompletePlaceholderShowHideButton_Click(object sender, EventArgs e)
        {
            markCompletePlaceholder.Visible = (markCompletePlaceholder.Visible == true ? false : true);
        }

        protected void markCompleteInstructionsPlaceholderShowHide_Click(object sender, EventArgs e)
        {
            markCompleteInstructionsPlaceholder.Visible = (markCompleteInstructionsPlaceholder.Visible == true ? false : true);
            markCompleteInstructionsPlaceholderShowHide.Text = (markCompleteInstructionsPlaceholder.Visible == true ? "Hide Instructions" : "Show Instructions");
        }

        protected void markCompleteButton_Click(object sender, EventArgs e)
        {
            List<int> pkList = new List<int>();

            foreach (GridViewRow row in markCompleteGridView.Rows)
            {
                if (((CheckBox)row.Cells[0].Controls[1]).Checked)
                    pkList.Add(Int32.Parse(((LinkButton)row.Cells[1].Controls[1]).Text));
            }

            foreach(int pk in pkList)
            {
                MarkPOComplete(pk);
            }

            markCompleteGridView.DataBind();
        }

        protected void MarkPOComplete(int poPK)
        {
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
            SqlCommand completePO = new SqlCommand("UPDATE PurchaseOrders SET poActive = 0 WHERE poPK = @poPK;", conn);
            completePO.Parameters.AddWithValue("@poPK", poPK);

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                completePO.ExecuteNonQuery();
                conn.Close();
            }
        }

        protected void markSubmitedButton_Click(object sender, EventArgs e)
        {
            List<int> pkList = new List<int>();

            foreach (GridViewRow row in markSubmitedGridView.Rows)
            {
                if (((CheckBox)row.Cells[0].Controls[1]).Checked)
                    pkList.Add(Int32.Parse(((LinkButton)row.Cells[1].Controls[1]).Text));
            }

            foreach (int pk in pkList)
            {
                MarkPOSubmited(pk);
            }

            markSubmitedGridView.DataBind();
        }

        protected void MarkPOSubmited(int poPK)
        {
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
            SqlCommand submitPO = new SqlCommand("UPDATE PurchaseOrders SET poDateSubmitedForReconciliation = @today WHERE poPK = @poPK;", conn);
            submitPO.Parameters.AddWithValue("today", DateTime.Today);
            submitPO.Parameters.AddWithValue("@poPK", poPK);

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                submitPO.ExecuteNonQuery();
                conn.Close();
            }
        }

        protected void poDrillDown(object sender, EventArgs e)
        {
            drillDownWindow.Visible = true;
            PurchaseOrder drillOnPO = new PurchaseOrder(Int32.Parse(((LinkButton)sender).CommandArgument));
            List<PurchaseOrderLineItem> lineItems = GetLineItems(drillOnPO.lineItemPKs);
            ViewState["lineItems"] = lineItems.Count();

            for (int i = 1; i <= (int)ViewState["lineItems"]; i++)
            {
                addLineItems(i, lineItems[i - 1]);
            }

            poNumbLabel.Text = drillOnPO.poPK.ToString();
            vendorComboBox.SelectedValue = drillOnPO.poVendor.ToString();
            vendorComboBox.DataBind();
            vendorInfoDisplay();
            poDateLinkButton.Text = drillOnPO.poDateCreated.ToString().Split(' ')[0];       
            paymentDropDown.SelectedValue = drillOnPO.poPayment.ToString();
            vendInvNumbTextBox.Text = drillOnPO.poVendorNumb;
            poComments.Text = drillOnPO.poComments;
            totalTextBox.Text = drillOnPO.poTotal.ToString("F2");
            
        }

        protected List<PurchaseOrderLineItem> GetLineItems(List<int> pks)
        {
            List<PurchaseOrderLineItem> retuenMe = new List<PurchaseOrderLineItem>();

            foreach (int pk in pks)
            {
                retuenMe.Add(new PurchaseOrderLineItem(pk));
            }

            return retuenMe;
        }
        protected void closeDrillDownWindowButton_Click(object sender, EventArgs e)
        {
            drillDownWindow.Visible = false;

            for(int i = (int)ViewState["lineItems"]; i >= 1; i--)
            { 
                removeLastLineItem();
            }
            
            ViewState["lineItems"] = null;
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
            //lineItemDescDiv.Controls.Add(itemDescriptionRequiredValidator);
            lineItemDescDiv.Controls.Add(lineBreak1);

            lineItemsUnitsDiv.Controls.Add(itemUnits);
            //lineItemsUnitsDiv.Controls.Add(itemUnitsRequiredValidator);
            lineItemsUnitsDiv.Controls.Add(lineBreak2);

            lineItemsUnitPriceDiv.Controls.Add(itemUnitsPrice);
            //lineItemsUnitPriceDiv.Controls.Add(itemUnitsPriceRequiredValidator);
            lineItemsUnitPriceDiv.Controls.Add(lineBreak3);

            lineItemsTotalDiv.Controls.Add(itemTotal);
            //lineItemsTotalDiv.Controls.Add(itemTotalRequiredValidator);
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
                itemTotal.Text = lineItem.lineitemTotalPrice.ToString("0.#####");
                itemTotal.Enabled = false;
                itemDepartmentDropDown.Items.Add(lineItem.lineitemDepartmentName);
                itemDepartmentDropDown.Enabled = false;
                itemGrantsDropDown.Items.Add(lineItem.lineitemGrantName);
                itemGrantsDropDown.Enabled = false;
            }
        }

        protected void vendorInfoDisplay()
        {
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
            SqlCommand vendorInfo = new SqlCommand("SELECT * FROM Vendors WHERE vendorName = @vendorName", conn);
            vendorInfo.Parameters.AddWithValue("@vendorName", vendorComboBox.SelectedItem.Text);

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

        protected void managePaymentsPlaceholderShowHideButton_Click(object sender, EventArgs e)
        {
            managePaymentsPlaceholder.Visible = (managePaymentsPlaceholder.Visible == true ? false : true);
        }

        protected void managePaymentsInstructionsShowHieButton_Click(object sender, EventArgs e)
        {
            managePaymentsInstructionsPlaceholder.Visible = (managePaymentsInstructionsPlaceholder.Visible == true ? false : true);
            managePaymentsInstructionsShowHieButton.Text = (managePaymentsInstructionsPlaceholder.Visible == true ? "Hide Instructions" : "Shoe Instructions");
        }

        protected void newPaymentSourceSubmitButton_Click(object sender, EventArgs e)
        {
            if (checkIfPaymentExists( newPaymentSourceNameTextBox.Text))
            {
                SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
                SqlCommand createPayment = new SqlCommand("INSERT INTO PaymentSource (paymentName, paymentType, paymentComments) VALUES (@paymentName, @paymentTypePK, @paymentComments);", conn);
                createPayment.Parameters.AddWithValue("@PaymentName", newPaymentSourceNameTextBox.Text);
                createPayment.Parameters.AddWithValue("@paymentTypePK", newPaymentSourceTypeDropDown.SelectedValue);
                createPayment.Parameters.AddWithValue("@PaymentComments", newPaymentSourceCommentsTextBox.Text);

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                    createPayment.ExecuteNonQuery();
                    conn.Close();
                }

                Response.Redirect(HttpContext.Current.Request.Url.ToString(), true);
            }
        }

        protected bool checkIfPaymentExists(string paymentName)
        {
            List<Tuple<int, int, string, string, string, bool>> paymentInfo = GetPaymentInfo(2);
            bool returnMe = true;

            foreach (Tuple<int, int, string, string, string, bool> payment in paymentInfo)
            {
                //check Name collision
                if (payment.Item4 == paymentName)
                {
                    //check if just inactive, give differnt error message
                    if (payment.Item6 == false)
                        ScriptManager.RegisterClientScriptBlock(this, GetType(), "alertMessage", string.Format("alert('A payment named {0} already exists but is inactive. Either change it to active, or create the new payment with a differnt name.')", paymentName), true);
                    else
                        ScriptManager.RegisterClientScriptBlock(this, GetType(), "alertMessage", string.Format("alert('A payment named {0} already exists and is active. Try creating the new payment with a differnt name.')", paymentName), true);

                    returnMe = false;
                }
            }

            return returnMe;
        }
        protected List<Tuple<int, int, string, string, string, bool>> GetPaymentInfo(int inactiveOrActiveOrBoth = 1)
        {

            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
            SqlCommand getPaymentInfo = new SqlCommand();
            List<Tuple<int, int, string, string, string, bool>> paymentInfo = new List<Tuple<int, int, string, string, string, bool>>();
            Tuple<int, int, string, string, string, bool> addThis;
            getPaymentInfo.Connection = conn;

            switch (inactiveOrActiveOrBoth)
            {
                case 0:
                    getPaymentInfo.CommandText = "SELECT paymentPK, paymentTypePK, paymentTypeName, paymentName, paymentComments, paymentActive FROM PaymentSource LEFT JOIN PaymentTypes on PaymentSource.paymentType = PaymentTypes.paymentTypePK WHERE paymentActive = 0;";
                    break;
                case 2:
                    getPaymentInfo.CommandText = "SELECT paymentPK, paymentTypePK, paymentTypeName, paymentName, paymentComments, paymentActive FROM PaymentSource LEFT JOIN PaymentTypes on PaymentSource.paymentType = PaymentTypes.paymentTypePK;";
                    break;
                case 1:
                default:
                    getPaymentInfo.CommandText = "SELECT paymentPK, paymentTypePK, paymentTypeName, paymentName, paymentComments, paymentActive FROM PaymentSource LEFT JOIN PaymentTypes on PaymentSource.paymentType = PaymentTypes.paymentTypePK WHERE paymentActive = 1;";
                    break;
            }

            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SqlDataReader readGrantInfo = getPaymentInfo.ExecuteReader();
            while (readGrantInfo.Read())
            {
                addThis = new Tuple<int, int, string, string, string, bool>(readGrantInfo.GetInt32(0), readGrantInfo.GetInt32(1), readGrantInfo.GetString(2), readGrantInfo.GetString(3), readGrantInfo.GetString(4), readGrantInfo.GetBoolean(5));
                paymentInfo.Add(addThis);
            }

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return paymentInfo;
        }

    protected List<Tuple<int, string>> GetPaymentTypeInfo()
    {

            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
            SqlCommand getPaymentTypes = new SqlCommand("SELECT paymentTypePK, PaymentTypeName FROM PaymentTypes;", conn);
            SqlDataReader readPaymentTypes = null;
            List<Tuple<int, string>> paymentTypes = new List<Tuple<int, string>>();

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                readPaymentTypes = getPaymentTypes.ExecuteReader();

                while (readPaymentTypes.Read())
                    paymentTypes.Add(new Tuple<int, string>(readPaymentTypes.GetInt32(0), readPaymentTypes.GetString(1)));

                conn.Close();
            }

            return paymentTypes;
    }

    protected void editPaymentSourceDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<Tuple<int, int, string, string, string, bool>> paymentInfo = GetPaymentInfo(1);

            foreach (Tuple<int, int, string, string, string, bool> paymentType in paymentInfo)
            {
                if ( paymentType.Item1.ToString() == editPaymentSourceDropDown.SelectedValue)
                {
                    editPaymentSourceNameTextbox.Text = paymentType.Item4;
                    editPaymentSourceTypeDropDown.SelectedValue = paymentType.Item2.ToString();
                    editPaymentSourceCommentsTextbox.Text = paymentType.Item5;
                }
            }
        }

        protected void editPaymentSourceSubmitButton_Click(object sender, EventArgs e)
        {
            if (checkIfPaymentExists(editPaymentSourceNameTextbox.Text) || editPaymentSourceNameTextbox.Text == editPaymentSourceDropDown.SelectedItem.Text)
            {
                SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
                SqlCommand editPayment = new SqlCommand("UPDATE PaymentSource SET paymentName = @paymentName, paymentType = @paymentTypePK, paymentComments = @paymentComments WHERE paymentPK = @paymentPK;", conn);

                editPayment.Parameters.AddWithValue("@paymentName", editPaymentSourceNameTextbox.Text);
                editPayment.Parameters.AddWithValue("@paymentTypePK", editPaymentSourceTypeDropDown.SelectedValue);
                editPayment.Parameters.AddWithValue("@paymentComments", editPaymentSourceCommentsTextbox.Text);
                editPayment.Parameters.AddWithValue("@paymentPK", editPaymentSourceDropDown.SelectedValue);

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                    editPayment.ExecuteNonQuery();
                    conn.Close();
                }

                Response.Redirect(HttpContext.Current.Request.Url.ToString(), true);
            }
        }

        protected void deletePaymentSourceSubmitButton_Click(object sender, EventArgs e)
        {
                SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
                SqlCommand poLineItemsAssignedToPayment = new SqlCommand("SELECT * FROM PurchaseOrders WHERE poPayment = @paymentPK;", conn);
                SqlCommand setPaymentInactive = new SqlCommand("UPDATE PaymentSource SET paymentActive = 0 WHERE paymentPK = @paymentPK", conn);
                SqlCommand deletePayment = new SqlCommand("DELETE FROM PaymentSource WHERE paymentPK = @PaymentPK", conn);
                poLineItemsAssignedToPayment.Parameters.AddWithValue("@paymentPK", deletePaymentSourceDropDown.SelectedValue);
                setPaymentInactive.Parameters.AddWithValue("@paymentPK", deletePaymentSourceDropDown.SelectedValue);
                deletePayment.Parameters.AddWithValue("@paymentPK", deletePaymentSourceDropDown.SelectedValue);
                bool hasRows;

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                    SqlDataReader readLineItems = poLineItemsAssignedToPayment.ExecuteReader();
                    hasRows = readLineItems.HasRows;
                    readLineItems.Close();

                    if (hasRows)
                        setPaymentInactive.ExecuteNonQuery();
                    else
                        deletePayment.ExecuteNonQuery();

                    conn.Close();
                }

                Response.Redirect(HttpContext.Current.Request.Url.ToString(), true);         
        }

        protected void inactivePaymentSourcesSubmitButton_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
            SqlCommand setPaymentActive = new SqlCommand("UPDATE PaymentSource SET paymentActive = 1 WHERE paymentPK = @paymentPK", conn);
            setPaymentActive.Parameters.AddWithValue("@paymentPK", inactivePaymentSourcesDropDown.SelectedValue);

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                setPaymentActive.ExecuteNonQuery();
                conn.Close();
            }

            Response.Redirect(HttpContext.Current.Request.Url.ToString(), true);
        }
    }
}
