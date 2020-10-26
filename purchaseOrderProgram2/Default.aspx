<%@ Page Title="PO 2.0: Welcome" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="purchaseOrderProgram2._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron" style="background-color:mediumpurple;">
        <img src="images/logo.jpg" style="width: 100%; height: 300px; object-fit: cover; object-position:0px 50%;" runat="server" />
        </div>

    <div class="row">
        <div class="col-md-4">
            <h2>PO2.0</h2>
            <hr />
            Wlecome to the new PO program! <br />
            I really hope you like it. There are some great new features that should help make life better. You need to make an account before you can log in, which you can do directly from this page. 
        </div>

        <div class="col-md-4">
            <h2>Log in, yo!</h2>
            <hr />
            <asp:PlaceHolder ID="loginErrorMessagePlaceHolder" Visible="false" runat="server">
                <asp:Label ID="loginErrorMessageLabel" CssClass="text-danger" runat="server" />
                <br />
            </asp:PlaceHolder>

            <asp:Label ID="loginEmailLabel" AssociatedControlID="loginEmailTextBox" runat="server">Email</asp:Label>
            <br />
            <asp:TextBox ID="loginEmailTextBox" TextMode="Email" Width="100%" runat="server" />
            <br />
            <asp:RequiredFieldValidator ID="loginEmailRequiredValidtor" ValidationGroup="loginValidationGroup"  ControlToValidate="loginEmailTextBox" ErrorMessage="Email is required." CssClass="text-danger" runat="server" />
            <br />

            <asp:Label ID="loginPasswordLabel" AssociatedControlID="loginPasswordTextBox" runat="server">Password</asp:Label>
            <br />
            <asp:TextBox ID="loginPasswordTextBox" TextMode="Password" Width="100%" runat="server" />
            <br />
            <asp:RequiredFieldValidator ID="loginPasswordRequiredValidator" ValidationGroup="loginValidationGroup" ControlToValidate="loginPasswordTextBox" ErrorMessage="Password is required." CssClass="text-danger" runat="server" />
            <br />

            <asp:Button ID="loginButton" ValidationGroup="loginValidationGroup" OnClick="Login_Click" Text="Log in" runat="server" />
            <br />

            <%--Enable this once you have account confirmation enabled for password reset functionality--%>
            <br />
            <br />
            <asp:HyperLink runat="server" ID="ForgotPasswordHyperLink"  ViewStateMode="Enabled">Forgot your password?</asp:HyperLink>
        </div>

        <div class="col-md-4">
            <h2>Needa register?</h2>
            <hr />
            <asp:PlaceHolder ID="registerErrorMessagePlaceHolder" Visible="false" runat="server">
                <asp:Label ID="registerErrorMessageLabel" CssClass="text-danger" runat="server" />
                <br />
            </asp:PlaceHolder>

            <asp:Label ID="registerEmailLabel" AssociatedControlID="registerEmailTextBox" runat="server">Email</asp:Label>
            <br />
            <asp:TextBox ID="registerEmailTextBox" TextMode="Email" Width="100%" runat="server" />
            <br />
            <asp:RequiredFieldValidator ValidationGroup="registerValidationGroup" ControlToValidate="registerEmailTextBox" ErrorMessage="The email field is required." CssClass="text-danger" runat="server" />
            <br />

            <asp:Label ID="registerPasswordLabel" AssociatedControlID="registerPasswordTextBox" runat="server">Password</asp:Label>
            <br />
            <asp:TextBox ID="registerPasswordTextBox" TextMode="Password" Width="100%" runat="server" />
            <br />
            <asp:RequiredFieldValidator ValidationGroup="registerValidationGroup" ControlToValidate="registerPasswordTextBox" ErrorMessage="The password field is required." CssClass="text-danger" runat="server" />
            <br />

            <asp:Label ID="registerPasswordConfirmLabel" AssociatedControlID="registerPasswordConfirmTextBox" runat="server">Confirm password</asp:Label>
            <br />
            <asp:TextBox ID="registerPasswordConfirmTextBox" TextMode="Password" Width="100%" runat="server" />
            <br />
            <asp:RequiredFieldValidator ValidationGroup="registerValidationGroup" ControlToValidate="registerPasswordConfirmTextBox" ErrorMessage="The confirm password field is required." CssClass="text-danger" runat="server" />
            <br />
            
            <asp:Button ID="registerButton" ValidationGroup="registerValidationGroup" OnClick="RegisterUser_Click" Text="Register" runat="server" />
            <br />
            <asp:CompareValidator ValidationGroup="registerValidationGroup" ControlToCompare="registerPasswordTextBox" ControlToValidate="registerPasswordConfirmTextBox" ErrorMessage="The password and confirmation password do not match." CssClass="text-danger" runat="server" />
            <br />
        </div>
    </div>
</asp:Content>
