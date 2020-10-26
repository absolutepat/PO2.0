using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using purchaseOrderProgram2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace purchaseOrderProgram2
{
    public partial class admin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!User.IsInRole("_Admin"))
            {
                Response.Redirect("~/Default.aspx");
            }

            if (!Page.IsPostBack)
            {
                fillListsAndDrops();
            }
        }

        protected void fillListsAndDrops()
        {
            fillUsersDropDown();
            fillDeparartmentsDropDown();
            fillGrantsDropDown();
            fillNonAdminUsersDropDown();
            fillAdminUsersDropDown();
        }

        protected void fillUsersDropDown()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            resetPasswordUserDropDown.DataSource = userManager.Users.ToList();
            resetPasswordUserDropDown.DataTextField = "Email";
            resetPasswordUserDropDown.DataValueField = "ID";
            resetPasswordUserDropDown.DataBind();

            deleteUserDropDown.DataSource = userManager.Users.ToList();
            deleteUserDropDown.DataTextField = "Email";
            deleteUserDropDown.DataValueField = "ID";
            deleteUserDropDown.DataBind();
        }

        protected void fillNonAdminUsersDropDown()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            List<ApplicationUser> users = userManager.Users.ToList();

            foreach (IdentityUser user in users)
            {
                if (!userManager.IsInRole(user.Id, "_Admin"))
                    nonAdminUsersDropDown.Items.Add(new ListItem(user.UserName, user.Id));
            }
        }

        protected void fillAdminUsersDropDown()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            List<ApplicationUser> users = userManager.Users.ToList();

            foreach (IdentityUser user in users)
            {
                if (userManager.IsInRole(user.Id, "_Admin"))
                    adminUsersDropDown.Items.Add(new ListItem(user.UserName, user.Id));
            }
        }

        protected void fillDeparartmentsDropDown()
        {
            deleteDepartmentDropDown.DataSource = GetDepartmentInfo();
            deleteDepartmentDropDown.DataTextField = "Item2";
            deleteDepartmentDropDown.DataValueField = "Item1";
            deleteDepartmentDropDown.DataBind();

            inactiveDepartmentsDropDown.DataSource = GetDepartmentInfo(0);
            inactiveDepartmentsDropDown.DataTextField = "Item2";
            inactiveDepartmentsDropDown.DataValueField = "Item1";
            inactiveDepartmentsDropDown.DataBind();
        }

        protected void fillGrantsDropDown()
        {

            deleteGrantDropDown.DataSource = getGrantInfo();
            deleteGrantDropDown.DataTextField = "Item2";
            deleteGrantDropDown.DataValueField = "Item1";
            deleteGrantDropDown.DataBind();

            inactiveGrantsDropDown.DataSource = getGrantInfo(0);
            inactiveGrantsDropDown.DataTextField = "Item2";
            inactiveGrantsDropDown.DataValueField = "Item1";
            inactiveGrantsDropDown.DataBind();
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

        protected void deleteDepartmentButton_Click(object sender, EventArgs e)
        {
            //Setup for SQL Stuff
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
            SqlCommand poLineItemsAssignedToDept = new SqlCommand("SELECT * FROM PurchaseOrderLineItems WHERE lineItemDepartment = @deptPK;", conn);
            SqlCommand setDepartmentInactive = new SqlCommand("UPDATE Departments SET departmentActive = 0 WHERE departmentPK = @deptPK", conn);
            SqlCommand deleteDepartment = new SqlCommand("DELETE FROM Departments WHERE departmentPK = @deptPK", conn);
            poLineItemsAssignedToDept.Parameters.AddWithValue("@deptPK", deleteDepartmentDropDown.SelectedValue);
            setDepartmentInactive.Parameters.AddWithValue("@deptPK", deleteDepartmentDropDown.SelectedValue);
            deleteDepartment.Parameters.AddWithValue("@deptPK", deleteDepartmentDropDown.SelectedValue);
            bool hasRows;
            //Setup for Identity stuff
            ApplicationDbContext context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            string deptName = deleteDepartmentDropDown.SelectedItem.Text;
            string make = deptName + "_MakePO";
            string review = deptName + "_ReviewAll";
            string edit = deptName + "_EditAll";

            //SQL Stuff
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                SqlDataReader readLineItems = poLineItemsAssignedToDept.ExecuteReader();
                hasRows = readLineItems.HasRows;
                readLineItems.Close();

                if (hasRows)
                    setDepartmentInactive.ExecuteNonQuery();
                else
                {
                    //Idenitty Stuff
                    if (roleManager.RoleExists(make))
                        roleManager.Delete(roleManager.FindByName(make));
                    if (roleManager.RoleExists(review))
                        roleManager.Delete(roleManager.FindByName(review));
                    if (roleManager.RoleExists(edit))
                        roleManager.Delete(roleManager.FindByName(edit));

                    deleteDepartment.ExecuteNonQuery();
                }
                    
                conn.Close();
            }

            Response.Redirect(HttpContext.Current.Request.Url.ToString(), true);
        }

        protected void inactiveDepartmentsButton_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
            SqlCommand setDepartmentActive = new SqlCommand("UPDATE Departments SET departmentActive = 1 WHERE departmentPK = @deptPK", conn);
            setDepartmentActive.Parameters.AddWithValue("@deptPK", inactiveDepartmentsDropDown.SelectedValue);

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                setDepartmentActive.ExecuteNonQuery();
                conn.Close();
            }

            createRolesForDepartment(inactiveDepartmentsDropDown.SelectedItem.Text);

            Response.Redirect(HttpContext.Current.Request.Url.ToString(), true);
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

        protected void deleteGrantSubmitButton_Click(object sender, EventArgs e)
        {
            if (deleteGrantDropDown.SelectedItem.Text != " None")
            {
                SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
                SqlCommand poLineItemsAssignedToGrant = new SqlCommand("SELECT * FROM PurchaseOrderLineItems WHERE lineItemGrant = @grantPK;", conn);
                SqlCommand setGrantInactive = new SqlCommand("UPDATE Grants SET grantActive = 0 WHERE grantPK = @grantPK", conn);
                SqlCommand deleteGrant = new SqlCommand("DELETE FROM Grants WHERE grantPK = @grantPK", conn);
                poLineItemsAssignedToGrant.Parameters.AddWithValue("@grantPK", deleteGrantDropDown.SelectedValue);
                setGrantInactive.Parameters.AddWithValue("@grantPK", deleteGrantDropDown.SelectedValue);
                deleteGrant.Parameters.AddWithValue("@grantPK", deleteGrantDropDown.SelectedValue);
                bool hasRows;

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                    SqlDataReader readLineItems = poLineItemsAssignedToGrant.ExecuteReader();
                    hasRows = readLineItems.HasRows;
                    readLineItems.Close();

                    if (hasRows)
                        setGrantInactive.ExecuteNonQuery();
                    else
                        deleteGrant.ExecuteNonQuery();

                    conn.Close();
                }

                Response.Redirect(HttpContext.Current.Request.Url.ToString(), true);
            }
            else
                ScriptManager.RegisterClientScriptBlock(this, GetType(), "alertMessage", string.Format("alert('Dont delete the None grant.')"), true);
        }

        protected void inactiveGrantsSubmitButton_Click(object sender, EventArgs e)
        {

            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DataConnection"].ConnectionString);
            SqlCommand setGrantActive = new SqlCommand("UPDATE Grants SET grantActive = 1 WHERE grantPK = @grantPK", conn);
            setGrantActive.Parameters.AddWithValue("@grantPK", inactiveGrantsDropDown.SelectedValue);

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                setGrantActive.ExecuteNonQuery();
                conn.Close();
            }

            Response.Redirect(HttpContext.Current.Request.Url.ToString(), true);
        }

        protected void deleteDepartmentNotesShowHideButton_Click(object sender, EventArgs e)
        {
            deleteDeparmentNotesPlaceholder.Visible = (deleteDeparmentNotesPlaceholder.Visible == true ? false : true);
            deleteDepartmentNotesShowHideButton.Text = (deleteDeparmentNotesPlaceholder.Visible == true ? "Hide Notes" : "Show Notes");
        }

        protected void deleteGrantNotesShowHideButton_Click(object sender, EventArgs e)
        {
            deleteGrantNotesPlaceholder.Visible = (deleteGrantNotesPlaceholder.Visible == true ? false : true);
            deleteGrantNotesShowHideButton.Text = (deleteGrantNotesPlaceholder.Visible == true ? "Hide Notes" : "Show Notes");
        }

        protected void adminUsersNotesShowHideButton_Click(object sender, EventArgs e)
        {
            adminUsersNotesPlaceholder.Visible = (adminUsersNotesPlaceholder.Visible == true ? false : true);
            adminUsersNotesShowHideButton.Text = (adminUsersNotesPlaceholder.Visible == true ? "Hide Notes" : "Show Notes");
        }

        protected void resetPasswordNotesShowHideButton_Click(object sender, EventArgs e)
        {
            resetPasswordNotesPlaceholder.Visible = (resetPasswordNotesPlaceholder.Visible == true ? false : true);
            resetPasswordNotesShowHideButton.Text = (resetPasswordNotesPlaceholder.Visible == true ? "Hide Notes" : "Show Notes"); 
        }

        protected void makeUserNotesShowHideButton_Click(object sender, EventArgs e)
        {
            makeUserNotesPlaceholder.Visible = (makeUserNotesPlaceholder.Visible == true ? false : true);
            makeUserNotesShowHideButton.Text = (makeUserNotesPlaceholder.Visible == true ? "Hide Notes" : "Show Notes");
        }

        protected void makeAnAdminButton_Click(object sender, EventArgs e)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            userManager.AddToRole(nonAdminUsersDropDown.SelectedValue, "_Admin");
            Response.Redirect(HttpContext.Current.Request.Url.ToString(), true);
        }

        protected void removeFromAdminButton_Click(object sender, EventArgs e)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (User.Identity.GetUserId() != adminUsersDropDown.SelectedValue)
            {
                userManager.RemoveFromRole(adminUsersDropDown.SelectedValue, "_Admin");
                Response.Redirect(HttpContext.Current.Request.Url.ToString(), true);
            }
            else
                ScriptManager.RegisterClientScriptBlock(this, GetType(), "alertMessage", string.Format("alert('You cant remove yourself from the admin role. You have to get another admin to remove you. See notes for more info.')"), true);
            
        }


        protected void resetPasswordButton_Click(object sender, EventArgs e)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            string selectedUserID = resetPasswordUserDropDown.SelectedValue;

            IdentityResult removeResult = userManager.RemovePassword(selectedUserID);
            if (removeResult.Succeeded)
            {
                IdentityResult addResult = userManager.AddPassword(selectedUserID, resetPasswordTextBox.Text);
                if (addResult.Succeeded)
                {
                    resetPasswordErrorLabel.Text = "Password change sucsesfull.";
                    resetPasswordTextBox.Text = string.Empty;
                }
                else
                {
                    foreach (string error in addResult.Errors)
                    {
                        resetPasswordErrorLabel.Text += error + " ";
                    }
                }
            }
            else
            {
                foreach (string error in removeResult.Errors)
                {
                    resetPasswordErrorLabel.Text += error + " ";
                 }
            }
        }

        protected void newUserCreateButton_Click(object sender, EventArgs e)
        {

           var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
           var signInManager = Context.GetOwinContext().Get<ApplicationSignInManager>();
           var user = new ApplicationUser() { UserName = newUserEmailTextBox.Text, Email = newUserEmailTextBox.Text };
           IdentityResult result = manager.Create(user, newUserPasswordTextBox.Text);

            if (result.Succeeded)
            {
                newUserErrorLabel.Text = "Account Created!";
                newUserErrorLabel.Visible = true;
                manager.ConfirmEmail(user.Id, manager.GenerateEmailConfirmationToken(user.Id));
            }
            else
            {
                newUserErrorLabel.Text = result.Errors.FirstOrDefault();
                newUserErrorLabel.Visible = true;
            }
        }

        protected void deleteUserButton_Click(object sender, EventArgs e)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (User.Identity.GetUserId() != deleteUserDropDown.SelectedValue)
            {
                IdentityResult result =  userManager.Delete( userManager.FindById(deleteUserDropDown.SelectedValue));

                if (result.Succeeded)
                {
                    Response.Redirect(HttpContext.Current.Request.Url.ToString(), true);
                }
                else
                {
                    deleteUserErrorLabel.Text = result.Errors.FirstOrDefault();
                    deleteUserErrorLabel.Visible = true;
                }                
            }
            else
                ScriptManager.RegisterClientScriptBlock(this, GetType(), "alertMessage", string.Format("alert('You cant delete youself.')"), true);
        }

        protected void deleteUserNotesShowHideButton_Click(object sender, EventArgs e)
        {
            deleteUserNotesPlaceholder.Visible = (deleteUserNotesPlaceholder.Visible == true ? false : true);
            deleteUserNotesShowHideButton.Text = (deleteUserNotesPlaceholder.Visible == true ? "Hide Notes" : "Show Notes");
        }
    }
}
