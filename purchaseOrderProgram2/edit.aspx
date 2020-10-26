<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="edit.aspx.cs" Inherits="purchaseOrderProgram2.edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
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

        .searchItemBoxes {
            border: 5px solid red;
            margin: 5px;
            padding: 5px;
            border-width: 0px;
            border-color: black;
            border-radius: 10px;
            position: relative;
            float: left;
            border-left: 1px solid black;
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

        .aspNetDisabled {
            color: black;
            background-color: rgba(242, 242, 242, 0.5);
        }

        .departmentDisabled {
            font-style: italic;
            color: aquamarine;
        }
        .floatRight{
            float:right;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <img src="~/images/edit.png" class="titlePic" runat="server" />
    <asp:LinkButton ID="reviewInstructionsPlaceholderShowHide" Text="Show Instructions" OnClick="reviewInstructionsPlaceholderShowHide_Click" runat="server" />

    <div id="drillDownWindow"  visible="false" class="drillDownWindow" runat="server">
        
        <div class="itemBoxes" style="width: 100%; min-width: 1100px; border-width: 10px; background-color: white;">
            <asp:Button ID="closeDrillDownWindowButton" OnClick="closeDrillDownWindowButton_Click" Text="Close window" runat="server" />
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
                        <h3 class="itemBoxesHeader" style="text-decoration: none;">Delete</h3>
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
                        <%--<asp:Button ID="removeLineButton" Text="Remove Line" OnClick="removeLineButton_Click" CausesValidation="false" runat="server" />--%>
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
                    <asp:Button ID="submitButton" Text="submit changes" OnClick="SubmitButton_Click" CausesValidation="true" runat="server" />
                    <asp:Button ID="fillInButton" Text="Fill in Missing" OnClick="fillInButton_Click" runat="server" />
                    <%--<asp:Button ID="clearButton" Text="clear" OnClick="clearButton_Click" runat="server" />--%>
                    <asp:Button ID="deletePOButton" Text="Delete Entire PO" CssClass="floatRight" ForeColor="Red" OnClick="deletePOButton_Click" runat="server" />
                    <br />
                    <asp:Label ID="validatorOutput" BorderColor="Red" BorderWidth="5px" Visible="false" runat="server" />
                </div>
            </div>
        </div>
        <div style="clear: both;">&nbsp;</div>
        <asp:SqlDataSource ID="vendorsDataSource" ConnectionString="<%$ ConnectionStrings:DataConnection %>" SelectCommand="SELECT * FROM Vendors ORDER BY vendorName asc" runat="server" />
        <asp:SqlDataSource ID="departmentsDataSource" ConnectionString="<%$ ConnectionStrings:DataConnection %>" SelectCommand="SELECT * FROM Departments ORDER BY departmentName asc" runat="server" />
        <asp:SqlDataSource ID="grantsDataSource" ConnectionString="<%$ ConnectionStrings:DataConnection %>" SelectCommand="SELECT * FROM Grants ORDER BY grantName asc" runat="server" />
        <asp:SqlDataSource ID="paymentDataSource" ConnectionString="<%$ ConnectionStrings:DataConnection %>" SelectCommand="SELECT * FROM PaymentSource ORDER BY paymentName asc" runat="server" />
    </div>

    <br />
    <asp:PlaceHolder ID="reviewInstructionsPlaceholder" Visible="false" runat="server">
        <div>
            In this area you can edit PO's. Use the search critera to filter down the list. Click on the PO number to see the whole PO.
            You need "persmision" for each department you need to edit a purchased item. Ask a Purchasing Agent to change assign you to the Edit group
            for the department. At the bottom you can export PO's to excell, or print them off.
        </div>
        <br />
    </asp:PlaceHolder>
    <asp:LinkButton ID="searchPlaceholderPlaceholderShowHideButton" OnClick="searchPlaceholderPlaceholderShowHideButton_Click" runat="server">
        <h2>Search for PO's</h2>
    </asp:LinkButton>
    <asp:PlaceHolder ID="searchPlaceholder" Visible="true" runat="server">
        <div style="width: 100%; display: block; border-left: 0;" class="itemBoxes">
            <div class="searchItemBoxes">
                <h3 class="itemBoxesHeader">From which departments:</h3>
                <asp:CheckBoxList ID="departmentCheckBoxList" RepeatDirection="Horizontal" RepeatColumns="3" CellPadding="7" runat="server"></asp:CheckBoxList>
            </div>

            <div class="searchItemBoxes">
                <h3 class="itemBoxesHeader">From this date range:</h3>
                <div class="forceInline">
                    <div class="itemBoxes">
                        <p>Start Date</p>
                        <asp:TextBox ID="startDateTextBox" runat="server" />
                    </div>
                </div>

                <div class="forceInline">
                    <div class="itemBoxes">
                        <p>End Date</p>
                        <asp:TextBox ID="endDateTextBox" runat="server" />
                    </div>
                </div>
            </div>

            <div class="searchItemBoxes">
                <h3 class="itemBoxesHeader">PO total range:</h3>
                <div class="forceInline">
                    <div class="itemBoxes">
                        <p>Low</p>
                        <asp:TextBox ID="lowPriceTextBox" runat="server" />
                    </div>

                </div>
                <div class="forceInline">
                    <div class="itemBoxes">
                        <p>High</p>
                        <asp:TextBox ID="highPriceTextBox" runat="server" />
                    </div>
                </div>
            </div>

            <div class="searchItemBoxes">
                <h3 class="itemBoxesHeader">Po created by:</h3>
                <asp:ListBox ID="purchaserListBox" DataSourceID="usersDataSource" DataTextField="poCreator" SelectionMode="Multiple" runat="server"></asp:ListBox><br />
            </div>
        </div>
        <hr />
        <asp:Button ID="submitFilterButton" Text="Search!" OnClick="submitFilterButton_Click" runat="server" />
        <br />
        <asp:Label ID="searchValidation" ForeColor="Red" runat="server" />
        <asp:SqlDataSource ID="usersDataSource" ConnectionString="<%$ ConnectionStrings:DataConnection %>" SelectCommand="SELECT DISTINCT poCreator FROM PurchaseOrders WHERE poPayment is not null;" runat="server" />
    </asp:PlaceHolder>

    <h2>Search Results</h2>
    <asp:GridView ID="reviewGridView" AllowSorting="true" OnSorting="reviewGridView_Sorting" AutoGenerateColumns="false" CellPadding="5" runat="server">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:CheckBox runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="PO Number" SortExpression="poPK">
                <ItemTemplate>
                    <asp:LinkButton OnClick="poDrillDown" CommandArgument='<%#Eval("poPK") %>' Text='<%#Eval("poPK") %>' runat="server" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="poCreator" HeaderText="PO Made by" SortExpression="poCreator" />
            <asp:BoundField DataField="poDateCreated" HeaderText="Date Made" DataFormatString="{0:d}" SortExpression="poDateCreated" />
            <asp:BoundField DataField="poTotal" HeaderText="Total" DataFormatString="{0:F2}" SortExpression="poTotal" />
            <asp:BoundField DataField="poComments" HeaderText="Comments" SortExpression="poComments" />
            <asp:BoundField DataField="poDateSubmitedForReconciliation" HeaderText="Date Sumbited for Reconciliation" DataFormatString="{0:d}" SortExpression="poDateSubmitedForReconciliation" />
            <asp:BoundField DataField="poActive" HeaderText="Complete" SortExpression="poActive" />
        </Columns>
    </asp:GridView>
    <asp:Button ID="selectAllButton" OnClick="selectAllButton_Click" Text="Select All" runat="server" />
    <asp:Button ID="exportSelectedToExcellButton" OnClick="exportSelectedToExcellButton_Click" Text="Export selected PO's to Excell" runat="server" />
    <asp:Button ID="printSelectedPOsButton_Complete" OnClick="printSelectedPOsButton_Complete_Click" Text="Print selected PO's" runat="server" />
    <hr />
</asp:Content>
