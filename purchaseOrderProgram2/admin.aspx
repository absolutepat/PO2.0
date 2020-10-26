<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="admin.aspx.cs" Inherits="purchaseOrderProgram2.admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .labely {
            text-align: right;
            font-weight: 600;
            margin: 10px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <img src="~/images/adminArea.png" class="titlePic" runat="server" />
    <br />
    <h3>Be thoughtfull and delibrate when using this page.</h3>
    <hr />

       <h3>Mint an Admin</h3>
    <asp:LinkButton ID="adminUsersNotesShowHideButton" Text="Show Notes" OnClick="adminUsersNotesShowHideButton_Click" runat="server" />
    <br />
    <asp:PlaceHolder ID="adminUsersNotesPlaceholder" Visible="false" runat="server">
        Here an admin can add another user to the admin role. You can also remove a user from admin. There is nothing wrong with there being many admins; just remember 
        an admin has power to mess things up, so be discering.
        <br />
        Becuase it takes an admin to make an admin, there always has to be at least 1 admin user. To enforce this rule, you cannot remove yourself from the admin role. If you need to be removed from admin, have another admin user remove you.
        <br />
    </asp:PlaceHolder>
 
    <asp:Label Text="Make user an admin: " CssClass="labely" Width="200" runat="server" /><asp:DropDownList ID="nonAdminUsersDropDown" runat="server" />
    <br />
    <asp:Label CssClass="labely" Width="200" runat="server" /><asp:Button Text="Make an admin" ID="makeAnAdminButton" OnClick="makeAnAdminButton_Click" runat="server" />
    <br />
    <br />
    <asp:Label Text="Remove user from admin: " CssClass="labely" Width="200" runat="server" /><asp:DropDownList ID="adminUsersDropDown" runat="server" />
    <br />
    <asp:Label CssClass="labely" Width="200" runat="server" /><asp:Button Text="Remove from admin" ID="removeFromAdminButton" OnClick="removeFromAdminButton_Click" runat="server" />
    <hr />

    <h3>Delete a Department</h3>
    <asp:LinkButton ID="deleteDepartmentNotesShowHideButton" Text="Show Notes" OnClick="deleteDepartmentNotesShowHideButton_Click" runat="server" />
    <br />
    <asp:PlaceHolder ID="deleteDeparmentNotesPlaceholder" Visible="false" runat="server">
        If a department has no purchases assigned to it, this will delete the department. However, if a department does have purchases, it will mark the department "inactive". 
        This does not effect any PO's already made. 
        <br />
        An inactive department is as good as a deleted department. No one can assign purchases to an inactive department. But this way, old PO's assigned to departments that don't exist anymore can still be accessed for reports.
        <br />
        An inactive department can be reactivated anytime, and it's just like any other department.
        <br />
    </asp:PlaceHolder>
    <asp:Label Text="Delete department:" CssClass="labely" Width="200" runat="server" /><asp:DropDownList ID="deleteDepartmentDropDown" runat="server" />
    <br />
    <asp:Label CssClass="labely" Width="200" runat="server" /><asp:Button ID="deleteDepartmentButton" Text="Delete department" OnClick="deleteDepartmentButton_Click" runat="server" />
    <br />
    <br />
    <asp:Label Text="Inactive departments: " CssClass="labely" Width="200" runat="server" /><asp:DropDownList ID="inactiveDepartmentsDropDown" runat="server" />
    <br />
    <asp:Label CssClass="labely" Width="200" runat="server" /><asp:Button ID="inactiveDepartmentsButton" Text="Re-activate department" OnClick="inactiveDepartmentsButton_Click" runat="server" />
    <hr />

    <h3>Delete a Grant</h3>
    <asp:LinkButton ID="deleteGrantNotesShowHideButton" Text="Show Notes" OnClick="deleteGrantNotesShowHideButton_Click" runat="server" />
    <br />
    <asp:PlaceHolder ID="deleteGrantNotesPlaceholder" Visible="false" runat="server">
        If a grant has no purchases assigned to it, this will delete the grant. However, if a grant does have purchases, it will mark the department "inactive". 
        This does not effect any PO's already made. 
        <br />
        An inactive grant is as good as a deleted grant. No one can assign purchases to an inactive grant. But this way, old PO's assigned to the grant that doesn't exist anymore can still be accessed for reports.
        <br />
        An inactive grant can be reactivaed and it's just like any other department.
        <br />
    </asp:PlaceHolder>
    <br />
    <asp:Label Text="Delete grant: " CssClass="labely" Width="200" runat="server" /><asp:DropDownList ID="deleteGrantDropDown" runat="server" />
    <br />
    <asp:Label CssClass="labely" Width="200" runat="server" /><asp:Button ID="deleteGrantSubmitButton" Text="Delete grant" OnClick="deleteGrantSubmitButton_Click" runat="server" />
    <br />
    <br />
    <asp:Label Text="Inactive grants: " CssClass="labely" Width="200" runat="server" /><asp:DropDownList ID="inactiveGrantsDropDown" runat="server" />
    <br />
    <asp:Label CssClass="labely" Width="200" runat="server" /><asp:Button ID="inactiveGrantsSubmitButton" Text="Re-activate grant" OnClick="inactiveGrantsSubmitButton_Click" runat="server" />
    <hr />

    <h3>Change a Password</h3>
    <asp:LinkButton ID="resetPasswordNotesShowHideButton" Text="Show Notes" OnClick="resetPasswordNotesShowHideButton_Click" runat="server" />
    <br />
    <asp:PlaceHolder ID="resetPasswordNotesPlaceholder" Visible="false" runat="server">
        Users can reset thier own password if they confirmed thier email. If they didn't, you'll need to reset it for them. 
        <br />
    </asp:PlaceHolder>
    <br />
    <asp:Label Text="Change user's password: " CssClass="labely" Width="200" runat="server" /><asp:DropDownList ID="resetPasswordUserDropDown" runat="server" />
    <br />
    <asp:Label Text="User's new password: " CssClass="labely" Width="200" runat="server" /><asp:TextBox ID="resetPasswordTextBox" runat="server" />
    <br />
    <asp:Label CssClass="labely" Width="200" runat="server" /><asp:Button Text="Change Password" ID="resetPasswordButton" OnClick="resetPasswordButton_Click" runat="server" />
    <asp:Label ID="resetPasswordErrorLabel" ForeColor="Red" runat="server" />
    <hr />

    <h3>Make a User</h3>
    <asp:LinkButton ID="makeUserNotesShowHideButton" Text="Show Notes" OnClick="makeUserNotesShowHideButton_Click" runat="server" />
    <br />
    <asp:PlaceHolder ID="makeUserNotesPlaceholder" Visible="false" runat="server">
        Anyone with a KODFP email can create thier own account on the homepage. If someone is too important to do that, you can create an account for them here.
        <br />
        Accounts made here aren't limited to @kodfp.org emails, and automatically have the email marked "confirmed".  
        <br />
    </asp:PlaceHolder>
    <br />
    <asp:Label Text="New user's email: " CssClass="labely" Width="200" runat="server" /><asp:TextBox ID="newUserEmailTextBox" runat="server" />
    <br />
    <asp:Label Text="New user's password: " CssClass="labely" Width="200" runat="server" /><asp:TextBox ID="newUserPasswordTextBox" runat="server" />
    <br />
    <asp:Label CssClass="labely" Width="200" runat="server" /><asp:Button Text="Create new user" ID="newUserCreateButton" OnClick="newUserCreateButton_Click" runat="server" />
    <asp:Label ID="newUserErrorLabel" ForeColor="Red" runat="server" />
    <hr />

    <h3>Delete a User</h3>
        <asp:LinkButton ID="deleteUserNotesShowHideButton" Text="Show Notes" OnClick="deleteUserNotesShowHideButton_Click" runat="server" />
    <br />
    <asp:PlaceHolder ID="deleteUserNotesPlaceholder" Visible="false" runat="server">
        Might as well delete people after they leave. 
        <br />
    </asp:PlaceHolder>
    <br />
    <asp:Label Text="Delete User: " CssClass="labely" Width="200" runat="server" /><asp:DropDownList ID="deleteUserDropDown" runat="server" />
    <br />
    <asp:Label CssClass="labely" Width="200" runat="server" /><asp:Button Text="Delete user" ID="deleteUserButton" OnClick="deleteUserButton_Click"  runat="server" />
    <asp:Label ID="deleteUserErrorLabel" ForeColor="Red" runat="server" />
    <br />
</asp:Content>
