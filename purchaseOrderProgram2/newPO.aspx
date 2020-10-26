<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="newPO.aspx.cs" Inherits="purchaseOrderProgram2.demoNewPO" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="addToHead" ContentPlaceHolderID="head" runat="server">
    <style>
        .itemBoxes {
            border: 5px solid red;
            margin: 5px;
            padding: 5px;
            border-width: 0px;
            border-color: black;
            border-radius: 10px;
            position: relative;
            float: left;
        }

        .itemBoxesHeader {
            margin: 0px;
            font-size: medium;
            font-weight: bold;
            text-decoration: underline;
        }

        .item {
            height: 30px;
            margin: 5px;
        }

        .forceInline {
            display: inline-block;
        }

        .ajax__combobox_itemlist {
            max-height: 120px;
            overflow: scroll;
            overflow-y: scroll;
        }
    </style>
</asp:Content>

<asp:Content ID="Body" ContentPlaceHolderID="MainContent" runat="server">
    <div class="itemBoxes" style="width: 100%; min-width: 1200px; border-width: 10px;">
        <h2>New Purchase Order:
            <asp:Label ID="poNumbLabel" Text="x" runat="server" /></h2>
        <div style="clear: both; width: 100%;">
            <hr />
            <div class="itemBoxes" style="border-right: 1px solid black; margin-right: 10px; padding-right: 10px; border-top-right-radius: 0; border-bottom-right-radius: 0;">
                <h3 class="itemBoxesHeader">Vendor</h3>
                <ajaxToolkit:ComboBox ID="vendorComboBox" DropDownStyle="DropDownList" AutoCompleteMode="SuggestAppend" DataSourceID="VendorsDataSource" DataTextField="vendorName" DataValueField="vendorPK" OnSelectedIndexChanged="vendorDropDownList_SelectedIndexChanged" OnDataBound="vendorDropDownList_DataBound" AutoPostBack="true" runat="server"></ajaxToolkit:ComboBox>
                <br />
                <asp:PlaceHolder ID="selectedVendorPlaceholder" Visible="true" runat="server">
                    <asp:Label ID="vendorAddressLabel" runat="server" /><br />
                    <asp:Label ID="vendorPhoneLabel" runat="server" /><br />
                    <asp:Label ID="vendorCommentsLabel" runat="server" /><br />
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="editVendorPlaceholder" Visible="false" runat="server">
                    <asp:Label ID="vendorEditNameLabel" Text="Name: " Width="100" runat="server" /><asp:TextBox ID="vendorEditNameTextBox" Width="200" runat="server"></asp:TextBox><br />
                    <asp:Label ID="vendorEditAddressLabel" Text="Address: " Width="100" runat="server" /><asp:TextBox ID="vendorEditAddressTextBox" Width="200" Rows="4" TextMode="MultiLine" runat="server"></asp:TextBox><br />
                    <asp:Label ID="vendorEditPhoneLabel" Text="Phone: " Width="100" runat="server" /><asp:TextBox ID="vendorEditPhoneTextBox" Width="200" runat="server"></asp:TextBox><br />
                    <asp:Label ID="vendorEditCommentsLabel" Text="Comments: " Width="100" runat="server" /><asp:TextBox ID="vendorEditCommentsTextBox" Width="200" runat="server"></asp:TextBox><br />
                    <asp:Button ID="vendorEditSubmitButton" Text="Submit Vendor Edits" OnClick="vendorEditSubmitButton_Click" runat="server" />
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="newVendorPlaceholder" Visible="false" runat="server">
                    <asp:Label ID="vendorNewNameLabel" Text="Name: " Width="100" runat="server" /><asp:TextBox ID="vendorNewNameTextBox" Width="200" runat="server"></asp:TextBox><br />
                    <asp:Label ID="vendorNewAddressLabel" Text="Address: " Width="100" runat="server" /><asp:TextBox ID="vendorNewAddressTextBox" Width="200" Rows="4" TextMode="MultiLine" runat="server"></asp:TextBox><br />
                    <asp:Label ID="vendorNewPhoneLabel" Text="Phone: " Width="100" runat="server" /><asp:TextBox ID="vendorNewPhoneTextBox" Width="200" runat="server"></asp:TextBox><br />
                    <asp:Label ID="vendorNewCommentsLabel" Text="Comments: " Width="100" runat="server" /><asp:TextBox ID="vendorNewCommentsTextBox" Width="200" runat="server"></asp:TextBox><br />
                    <asp:Button ID="vendorNewSubmitButton" Text="Add New Vendor" OnClick="vendorNewSubmitButton_Click" runat="server" />
                </asp:PlaceHolder>
                <br />
                <asp:Button ID="editVendorButton" Text="Edit Vendor" OnClick="editVendorButton_Click" runat="server" />
                <asp:Button ID="addNewVendorButton" Text="Add New Nevdor" OnClick="addNewVendorButton_Click" runat="server" />
                <asp:Button ID="cancelNewOrEditVendorButton" Text="Cancel" OnClick="cancelNewOrEditVendorButton_Click" Visible="false" runat="server" />
            </div>

            <div class="itemBoxes">
                <h3 class="itemBoxesHeader">Date</h3>
                <asp:LinkButton ID="poDateLinkButton" OnClick="poDateLinkButton_Click" runat="server" />
                <asp:Calendar ID="poDateCalendar" Visible="false" SelectedDate="01/01/1900" SelectionMode="Day" OnSelectionChanged="poDateCalendar_SelectionChanged" runat="server"></asp:Calendar>
            </div>
            <div id="paymentDiv" class="itemBoxes" runat="server">
                <h3 class="itemBoxesHeader">Payment</h3>
                <asp:DropDownList CssClass="item" ID="paymentDropDown" DataSourceID="paymentDataSource" DataTextField="paymentName" DataValueField="paymentPK" runat="server" />
            </div>
            <div id="vendorPoNumbDiv" class="itemBoxes" runat="server">
                <h3 class="itemBoxesHeader">Vendor Invoice #</h3>
                <asp:TextBox CssClass="item" ID="vendInvNumbTextBox" runat="server" />
            </div>

            <div style="border-top: 1px solid black; display: inline-block; margin-top: 10px; background-color: rgba(255, 255, 185, 0.75);">
                <div class="itemBoxes" style="font-style: italic;">
                </div>
                <div class="itemBoxes">
                    <h3 class="itemBoxesHeader" style="text-decoration: none;">&nbsp;</h3>
                    <i>Set all Line Items to:</i>
                </div>
                <div class="itemBoxes">
                    <h3 class="itemBoxesHeader">Department</h3>
                    <asp:DropDownList ID="departmentDropDown" DataSourceID="departmentsDataSource" DataTextField="departmentName" OnSelectedIndexChanged="departmentDropDown_SelectedIndexChanged" AutoPostBack="true" CssClass="item" runat="server" />
                </div>
                <div class="itemBoxes">
                    <h3 class="itemBoxesHeader">Grant</h3>
                    <asp:DropDownList ID="grantDropDown" DataSourceID="grantsDataSource" DataTextField="grantName" OnSelectedIndexChanged="grantDropDown_SelectedIndexChanged" AutoPostBack="true" CssClass="item" runat="server" />
                </div>
            </div>
        </div>
        <div style="clear: both; width: 100%;">
            <hr />
            <div>
                <div id="lineNumbDiv" class="itemBoxes" runat="server">
                    <h3 class="itemBoxesHeader" style="text-decoration: none;">&nbsp;</h3>
                </div>
                <div id="lineItemDescDiv" class="itemBoxes" runat="server">
                    <h3 class="itemBoxesHeader">Item Description</h3>
                </div>
                <div id="lineItemsUnitsDiv" class="itemBoxes" runat="server">
                    <h3 class="itemBoxesHeader">Units</h3>
                </div>
                <div id="lineItemsUnitPriceDiv" class="itemBoxes" runat="server">
                    <h3 class="itemBoxesHeader">Unit Price</h3>
                </div>
                <div class="itemBoxes" style="margin: 0; padding: 0; border: none;">
                    <div id="lineItemsTotalDiv" class="itemBoxes" runat="server">
                        <h3 class="itemBoxesHeader">Line Total</h3>
                    </div>
                    <div class="itemBoxes" style="position: absolute; bottom: -50px; padding-left: 10px;">
                        <h3 class="itemBoxesHeader">PO Total</h3>
                        <asp:TextBox ID="totalTextBox" Columns="8" runat="server"></asp:TextBox>
                    </div>
                </div>
                <div id="lineItemsDepartmentDiv" class="itemBoxes" runat="server">
                    <h3 class="itemBoxesHeader">Deparment</h3>
                </div>
                <div id="lineItemsGrantDiv" class="itemBoxes" runat="server">
                    <h3 class="itemBoxesHeader">Grant</h3>
                </div>

                <div style="clear: both; width: 100%;">
                    <asp:Button ID="newLineButton" Text="New Line" OnClick="newLineButton_Click" CausesValidation="false" runat="server" />
                    <asp:Button ID="removeLineButton" Text="Remove Line" OnClick="removeLineButton_Click" CausesValidation="false" runat="server" />
                </div>
            </div>
        </div>

        <div style="clear: both; width: 100%;">
            <hr />
            <div class="itemBoxes">
                <h3 class="itemBoxesHeader">Comments</h3>
                <asp:TextBox ID="poComments" Columns="100" Rows="5" TextMode="MultiLine" runat="server"></asp:TextBox>
            </div>

            <div style="clear: both; width: 100%;">
                <hr />
                <asp:Button ID="submitButton" Text="submit" OnClick="SubmitButton_Click" CausesValidation="true" runat="server" />
                <asp:Button ID="fillInButton" Text="Fill in Missing" OnClick="fillInButton_Click" runat="server" />
                <asp:Button ID="clearButton" Text="clear" OnClick="clearButton_Click" runat="server" />
                <br />
                <asp:Label ID="validatorOutput" BorderColor="Red" BorderWidth="5px" Visible="false" runat="server" />
            </div>
        </div>
    </div>
    <div style="clear: both;">&nbsp;</div>
    <asp:SqlDataSource ID="vendorsDataSource" ConnectionString="<%$ ConnectionStrings:DataConnection %>" SelectCommand="SELECT * FROM Vendors WHERE vendorActive = 1 ORDER BY vendorName asc" runat="server" />
    <asp:SqlDataSource ID="departmentsDataSource" ConnectionString="<%$ ConnectionStrings:DataConnection %>" SelectCommand="SELECT * FROM Departments WHERE departmentActive = 1 ORDER BY departmentName asc" runat="server" />
    <asp:SqlDataSource ID="grantsDataSource" ConnectionString="<%$ ConnectionStrings:DataConnection %>" SelectCommand="SELECT * FROM Grants WHERE grantActive = 1 ORDER BY grantName asc" runat="server" />
    <asp:SqlDataSource ID="paymentDataSource" ConnectionString="<%$ ConnectionStrings:DataConnection %>" SelectCommand="SELECT * FROM PaymentSource WHERE paymentActive = 1 ORDER BY paymentName asc" runat="server" />
</asp:Content>
