<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="purchasingAgent.aspx.cs" Inherits="purchaseOrderProgram2.purchasingAgent" %>

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
        .aspNetDisabled{
            color:black;
            background-color: rgba(242, 242, 242, 0.5);
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <img src="~/images/purchaseingAgentArea.png" class="titlePic" runat="server" />

    <div id="drillDownWindow" visible="false" style="z-index: 100; overflow: visible; border: double; border-width: thick; border-color: red; width: auto; height: auto; top: 10%; left: 5%; position: fixed; padding-right: 10px; background-color: antiquewhite;" runat="server">
        <asp:Button ID="closeDrillDownWindowButton" OnClick="closeDrillDownWindowButton_Click" Text="Close window" runat="server" />
        <div class="itemBoxes" style="width: 100%; min-width: 1100px; border-width: 10px; background-color: white;">
            <h2>Purchase Order:
                    <asp:Label ID="poNumbLabel" Text="x" runat="server" /></h2>
            <div style="clear: both; width: 100%;">
                <hr />
                <div class="itemBoxes" style="border-right: 1px solid black; margin-right: 10px; padding-right: 10px; border-top-right-radius: 0; border-bottom-right-radius: 0;">
                    <h3 class="itemBoxesHeader">Vendor</h3>
                    <ajaxToolkit:ComboBox ID="vendorComboBox" Enabled="false" DataSourceID="VendorsDataSource" DataTextField="vendorName" DataValueField="vendorPK" runat="server"></ajaxToolkit:ComboBox>
                    <br />
                    <asp:PlaceHolder ID="selectedVendorPlaceholder" Visible="true" runat="server">
                        <asp:Label ID="vendorAddressLabel" runat="server" /><br />
                        <asp:Label ID="vendorPhoneLabel" runat="server" /><br />
                        <asp:Label ID="vendorCommentsLabel" runat="server" /><br />
                    </asp:PlaceHolder>
                </div>
                <div class="itemBoxes">
                    <h3 class="itemBoxesHeader">Date Submited</h3>
                    <asp:LinkButton ID="poDateLinkButton" Enabled="false" runat="server" />
                </div>
                <div id="paymentDiv" class="itemBoxes" runat="server">
                    <h3 class="itemBoxesHeader">Payment</h3>
                    <asp:DropDownList CssClass="item" Enabled="false" ID="paymentDropDown" DataSourceID="paymentDataSource" DataTextField="paymentName" DataValueField="paymentPK" runat="server" />
                </div>
                <div id="vendorPoNumbDiv" class="itemBoxes" runat="server">
                    <h3 class="itemBoxesHeader">Vendor Invoice #</h3>
                    <asp:TextBox CssClass="item" Enabled="false" ID="vendInvNumbTextBox" runat="server" />
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
                            <asp:TextBox ID="totalTextBox" Enabled="false" Columns="8" runat="server"></asp:TextBox>
                        </div>
                    </div>
                    <div id="lineItemsDepartmentDiv" class="itemBoxes" runat="server">
                        <h3 class="itemBoxesHeader">Deparment</h3>
                    </div>
                    <div id="lineItemsGrantDiv" class="itemBoxes" runat="server">
                        <h3 class="itemBoxesHeader">Grant</h3>
                    </div>
                </div>
            </div>
            <div style="clear: both; width: 100%;">
                <hr />
                <div class="itemBoxes">
                    <h3 class="itemBoxesHeader">Comments</h3>
                    <asp:TextBox ID="poComments" Enabled="false" Columns="100" Rows="5" TextMode="MultiLine" runat="server"></asp:TextBox>
                </div>
            </div>
        </div>
        <div style="clear: both;">&nbsp;</div>
        <asp:SqlDataSource ID="vendorsDataSource" ConnectionString="<%$ ConnectionStrings:DataConnection %>" SelectCommand="SELECT * FROM Vendors;" runat="server" />
        <asp:SqlDataSource ID="paymentDataSource" ConnectionString="<%$ ConnectionStrings:DataConnection %>" SelectCommand="SELECT * FROM PaymentSource;" runat="server" />
    </div>

    <asp:LinkButton ID="markCompletePlaceholderShowHideButton" OnClick="markCompletePlaceholderShowHideButton_Click" runat="server">
        <h2>Mark PO's Complete</h2>
    </asp:LinkButton>
    <asp:PlaceHolder ID="markCompletePlaceholder" Visible="false" runat="server">
        <asp:LinkButton ID="markCompleteInstructionsPlaceholderShowHide" Text="Show Instructions" OnClick="markCompleteInstructionsPlaceholderShowHide_Click" runat="server" />
        <asp:PlaceHolder ID="markCompleteInstructionsPlaceholder" Visible="false" runat="server">
            <div>
                PO's can be marked "complete". A complete PO is one where all ordered items have been recieved. 
                This intended to be a helpfull tool to make sure we actually get everything we order.
            </div>
            <br />
        </asp:PlaceHolder>
        <asp:GridView ID="markCompleteGridView" DataSourceID="markCompleteDataSource" AllowSorting="true" AutoGenerateColumns="false" CellPadding="5" runat="server">
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
            </Columns>
        </asp:GridView>
        <asp:Button ID="printSelectedPOsButton_Complete" OnClick="printSelectedPOsButton_Complete_Click" Text="Print selected PO's" runat="server" />
        <asp:Button ID="markCompleteButton" OnClick="markCompleteButton_Click" Text="Mark selected PO's Complete" runat="server" />
        <asp:SqlDataSource ID="markCompleteDataSource" ConnectionString="<%$ ConnectionStrings:DataConnection %>" SelectCommand="select * from PurchaseOrders WHERE poActive = 1 AND poPayment is not null;" runat="server" />
    </asp:PlaceHolder>
    <hr />

    <asp:LinkButton ID="markSubmitedPlaceholderShowHideButton" OnClick="markSubmitedPlaceholderShowHideButton_Click" runat="server">
        <h2>Mark PO's Submited for Reconciliation</h2>
    </asp:LinkButton>
    <asp:PlaceHolder ID="markSubmitedPlaceholder" Visible="false" runat="server">
        <asp:LinkButton ID="markSubmitedInstructionsPlaceholderShowHideButton" Text="Show Instructions" OnClick="markSubmitedInstructionsPlaceholderShowHideButton_Click" runat="server" />
        <asp:PlaceHolder ID="markSubmitedInstrucstionsPlaceholder" Visible="false" runat="server">
            <div>
                Here you can both print off PO's and mark that you've given them to accounting to reconcile the books. 
                It will record the date you marked them submited. 
                This is intended to be a helpfull tool to keep track of which PO's still need to be submited.
            </div>
            <br />
        </asp:PlaceHolder>
        <asp:GridView ID="markSubmitedGridView" DataSourceID="markSubmitedDataSource" AllowSorting="true" AutoGenerateColumns="false" CellPadding="5" runat="server">
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
            </Columns>
        </asp:GridView>
        <asp:Button ID="printSelectedPOsButton_Submited" OnClick="printSelectedPOsButton_Submited_Click" Text="Print selected PO's" runat="server" />
        <asp:Button ID="markSubmitedButton" OnClick="markSubmitedButton_Click" Text="Mark selected PO's Submited" runat="server" />
        <asp:SqlDataSource ID="markSubmitedDataSource" ConnectionString="<%$ ConnectionStrings:DataConnection %>" SelectCommand="select * from PurchaseOrders WHERE poDateSubmitedForReconciliation is null AND poPayment is not null;" runat="server" />
    </asp:PlaceHolder>
    <hr />

        <asp:LinkButton ID="changeUserPermisionPlaceholderShowHideButton" OnClick="changeUserPermisionPlaceholderShowHideButton_Click" runat="server">
        <h2>Change a user's Permisions</h2>
    </asp:LinkButton>
    <asp:PlaceHolder ID="changeUserPermisionPlaceholder" Visible="false" runat="server">
        <asp:LinkButton ID="changeUserPermisionInstructionsShowHideButton" Text="Show Instructions" OnClick="changeUserPermisionInstructionsShowHideButton_Click" runat="server"></asp:LinkButton>
        <asp:PlaceHolder ID="changeUserPermisionInstructionsPlaceholder" Visible="false" runat="server">
            <div>
                <br />
                First, select a user from the drop down list. It will automatically check all the boxes for what the user can already do.
            <br />
                <br />
                <i>When a new user signs up, they can't do much until you decide what they do.</i>
                <br />
                <br />
                Each department has three options: Edit all; Make PO; Review All.
        <ul>
            <li>"Edit All" means the user can edit any purchased item assigned to the department.</li>
            <li>"Make PO" means they can assign purchased items to that department. They need to be assigned "Make_PO" for at least one department to make a PO.</li>
            <li>"Review All" means they can see all the purchased items for that department.</li>
            <li><i>"Purchasing Agent" can edit, make, and review for any department - as well as do everything on this page.</i></li>
            <li>"Admin" can only be changed from the admin section. They can do everything a purchasing agent can, as well as the stuff on the admin page.</li>
        </ul>
                It will likely be common to assign people the make/review/edit for all departments. So there is a button to do that.
                But you should probably be a little discering about who you make a purchasing agent.
                <br />
                After making choices"Submit Permision Changes" button. All done!
            <br />
            </div>
        </asp:PlaceHolder>
        <br />
        <br />
        <asp:DropDownList ID="usersDropDown" AutoPostBack="true" OnSelectedIndexChanged="usersDropDown_SelectedIndexChanged" runat="server" />
        <br />
        <asp:CheckBoxList ID="groupsCheckBoxList" BorderColor="black" BorderWidth="2" BackColor="Wheat" RepeatDirection="Horizontal" RepeatColumns="3" CellPadding="7" AppendDataBoundItems="true" runat="server" />
        <br />
        <asp:Button ID="changeUserGroupButton" Text="Submit Permision Changes" OnClick="changeUserGroupButton_Click" runat="server" /><asp:Label ID="changeUserGroupResultLabel" ForeColor="Red" runat="server" />
    </asp:PlaceHolder>
    <hr />

    <asp:LinkButton ID="manageDepartmentPlaceholderShowHideButton" OnClick="manageDepartmentPlaceholderShowHideButton_Click" runat="server">
        <h2>Manage Departments</h2>
    </asp:LinkButton>
    <asp:PlaceHolder ID="manageDepartmentsPlaceholder" Visible="false" runat="server">
        <asp:LinkButton ID="manageDepartmentsInstructionsShowHideButton" Text="Show Instructions" OnClick="manageDepartmentsInstructionsShowHideButton_Click" runat="server" />
        <asp:PlaceHolder ID="manageDepartmentsInstructionsPlaceholder" Visible="false" runat="server">
            <div>
                In this area, you can create a new department or edit the name/comments of an existing department. 
                <br />
                When you create a new department, the "Edit All", "MakePO, and "Review All" groups are also automatically created. Don't forget to assign user's to use the new department in the "Change a user's Permisions" area below.
                <br />
                Editing a department makes no changes to permisions. Editiding is intended to for fixing things like spelling errors and updating the comments.
                <br />
                Departments can be deleted in the "Admin" section of the PO Program.   
            </div>
        </asp:PlaceHolder>
        <br />
        <br />
        <asp:Label ID="createDepartmentLabel" Text="Create Department" Font-Underline="true" runat="server" />
        <br />
        <asp:Label Text="Deparment Name: " Width="200" runat="server" /><asp:TextBox ID="createDepartmentNameTextBox" runat="server" />
        <br />
        <asp:Label Text="Department Comments: " Width="200" runat="server" />
        <asp:TextBox ID="createDepartmentCommentsTextBox" runat="server" />
        <br />
        <asp:Button ID="createDepartmentButton" Text="Create Department" OnClick="createDepartmentButton_Click" ValidationGroup="createDepartmentValidationGroup" runat="server" />
        <asp:RequiredFieldValidator ID="createDepartmentNameTextBoxRequiredValidator" ValidationGroup="createDepartmentValidationGroup" ControlToValidate="createDepartmentNameTextBox" ErrorMessage="Department Name can't be empty." ForeColor="Red" runat="server" />
        <hr style="width: 200px; text-align: left;" />
        <asp:Label Text="Edit Department" Font-Underline="true" runat="server" />
        <br />
        <asp:DropDownList ID="editDepartmentDropDown" OnSelectedIndexChanged="editDepartmentDropDown_SelectedIndexChanged" AutoPostBack="true" runat="server" />
        <br />
        <asp:Label Text="Department Name" Width="200" runat="server" />
        <asp:TextBox ID="editDepartmentNameTextBox" runat="server" />
        <br />
        <asp:Label Text="Department Comments" Width="200" runat="server" />
        <asp:TextBox ID="editDepartmentCommentsTextBox" runat="server" />
        <br />
        <asp:Button ID="editDepartmentSubmitButtons" Text="Edit Department" OnClick="editDepartmentSubmitButtons_Click" runat="server" />
    </asp:PlaceHolder>
    <hr />

    <asp:LinkButton ID="manageGrantsPlaceholderShowHideButton" OnClick="manageGrantsPlaceholderShowHideButton_Click" runat="server"><h2>Manage Grants</h2></asp:LinkButton>
    <asp:PlaceHolder ID="manageGrantsPlaceholder" Visible="false" runat="server">
        <asp:LinkButton ID="manageGrantsInstructionsPlaceholderShowHideButton" Text="Show Instructions" OnClick="manageGrantsInstructionsPlaceholderShowHideButton_Click" runat="server" />
        <asp:PlaceHolder ID="manageGrantsInstructionsPlaceholer" Visible="false" runat="server">
            <div>
                In this area, you can create a new grant or edit the name/comments of an existing department. 
                <br />
                Grants can also be a good tool for tracking spending on internal projects. For example, create a "Holiday Party" grant, and you can easily track all holiday party spending on the reports page!
                <br />
                Editiding is intended to for fixing things like spelling errors and updating the comments.
                <br />
                Grants can be deleted in the "Admin" section of the PO Program.   
            </div>
        </asp:PlaceHolder>
        <br />
        <br />
        <asp:Label Text="New Grant" Font-Underline="true" runat="server" />
        <br />
        <asp:Label Text="Grant Name: " Width="200" runat="server" /><asp:TextBox ID="createGrantNameTextBox" runat="server" />
        <br />
        <asp:Label Text="Grant Start Date (mm/dd/yyyy): " Width="200" runat="server" /><asp:TextBox ID="createGrantStartTextBox" runat="server" />
        <br />
        <asp:Label Text="Grant End Date (mm/dd/yyyy): " Width="200" runat="server" /><asp:TextBox ID="createGrantEndTextBox" runat="server" />
        <br />
        <asp:Label Text="Grant Amount: " Width="200" runat="server" /><asp:TextBox ID="createGrantAmountTextBox" TextMode="Number" runat="server" />
        <br />
        <asp:Label Text="Grant Comments: " Width="200" runat="server" /><asp:TextBox ID="createGrantCommentsTextBox" runat="server" />
        <br />
        <asp:Button ID="createGrantSubmitButton" Text="Create New Grant" OnClick="createGrantSubmitButton_Click" runat="server" />
        <br />
        <asp:Label ID="createGrantErrorsLabel" Text="" ForeColor="Red" runat="server" />
        <hr style="width: 200px; text-align: left;" />
        <asp:Label Text="Edit Grant" Font-Underline="true" runat="server" />
        <br />
        <asp:DropDownList ID="editGrantDropDown" OnSelectedIndexChanged="editGrantDropDown_SelectedIndexChanged" AutoPostBack="true" runat="server" />
        <br />
        <asp:Label Text="Grant Name: " Width="200" runat="server" /><asp:TextBox ID="editGrantNameTextBox" runat="server" />
        <br />
        <asp:Label Text="Grant Start Date (mm/dd/yyyy): " Width="200" runat="server" /><asp:TextBox ID="editGrantStartTextBox" runat="server" />
        <br />
        <asp:Label Text="Grant End Date (mm/dd/yyyy): " Width="200" runat="server" /><asp:TextBox ID="editGrantEndTextBox" runat="server" />
        <br />
        <asp:Label Text="Grant Amount: " Width="200" runat="server" /><asp:TextBox ID="editGrantAmountTextBox" TextMode="Number" runat="server" />
        <br />
        <asp:Label Text="Grant Comments: " Width="200" runat="server" /><asp:TextBox ID="editGrantCommentsTextBox" runat="server" />
        <br />
        <asp:Button ID="editGrantSubmitButton" Text="Submit Changes to Grant" OnClick="editGrantSubmitButton_Click" runat="server" />
        <br />
        <asp:Label ID="editGrantErrorsLabel" Text="" ForeColor="Red" runat="server" />
    </asp:PlaceHolder>
    <hr />

    <asp:LinkButton ID="managePaymentsPlaceholderShowHideButton" OnClick="managePaymentsPlaceholderShowHideButton_Click" runat="server"><h2>Manage Payments</h2></asp:LinkButton>
    <asp:PlaceHolder ID="managePaymentsPlaceholder" Visible="false" runat="server">
        <asp:LinkButton ID="managePaymentsInstructionsShowHieButton" Text="Show Instructions" OnClick="managePaymentsInstructionsShowHieButton_Click" runat="server" />
        <asp:PlaceHolder ID="managePaymentsInstructionsPlaceholder" Visible="false" runat="server">
            <div>
                Use this section to add a new payments source people can use when creating/editing purchase orders. You can also use this section to edit payment sources.
        <br />
                When deleting a payment source, if there are any purchase orders using that payment source, it will be made "inactive" insteasd of deleted. 
                <br />
                An inactive payment source is as good as a deleted payment source. No one can assign purchases to an inactive payment source. 
                But this way, old PO's assigned to the payment source that doesn't exist anymore aren't effected.
        <br />
                An inactive payment source can be reactivaed and it's just like any other payment source.
        <br />
            </div>
        </asp:PlaceHolder>
        <br />
        <br />
        <asp:Label Text="New payment source" Font-Underline="true" runat="server" />
        <br />
        <asp:Label Text="payment source Name: " Width="200" runat="server" /><asp:TextBox ID="newPaymentSourceNameTextBox" runat="server" />
        <br />
        <asp:Label Text="Payment source type: " Width="200" runat="server" /><asp:DropDownList ID="newPaymentSourceTypeDropDown" runat="server" />
        <br />
        <asp:Label Text="Payment source comments: " Width="200" runat="server" /><asp:TextBox ID="newPaymentSourceCommentsTextBox" runat="server" />
        <br />
        <asp:Button ID="newPaymentSourceSubmitButton" Text="Create New Payment Source" OnClick="newPaymentSourceSubmitButton_Click" runat="server" />
        <br />
        <asp:Label ID="newPaymentSourceErrorsLabel" Text="" ForeColor="Red" runat="server" />
        <hr style="width: 200px; text-align: left;" />
        <asp:Label Text="Edit PaymentSource" Font-Underline="true" runat="server" />
        <br />
        <asp:DropDownList ID="editPaymentSourceDropDown" OnSelectedIndexChanged="editPaymentSourceDropDown_SelectedIndexChanged" AutoPostBack="true" runat="server" />
        <br />
        <asp:Label Text="Payment source Name: " Width="200" runat="server" /><asp:TextBox ID="editPaymentSourceNameTextbox" runat="server" />
        <br />
        <asp:Label Text="Payment source type: " Width="200" runat="server" /><asp:DropDownList ID="editPaymentSourceTypeDropDown" runat="server" />
        <br />
        <asp:Label Text="Payment source comments: " Width="200" runat="server" /><asp:TextBox ID="editPaymentSourceCommentsTextbox" runat="server" />
        <br />
        <asp:Button ID="editPaymentSourceSubmitButton" Text="Submit changes to payment source" OnClick="editPaymentSourceSubmitButton_Click" runat="server" />
        <br />
        <asp:Label ID="editPaymentSourceErrorsLabel" Text="" ForeColor="Red" runat="server" />
        <hr style="width: 200px; text-align: left;" />
        <asp:Label Text="Delete payment source: " CssClass="labely" Width="200" runat="server" /><asp:DropDownList ID="deletePaymentSourceDropDown" runat="server" />
        <br />
        <asp:Label CssClass="labely" Width="200" runat="server" /><asp:Button ID="deletePaymentSourceSubmitButton" Text="Delete payment source" OnClick="deletePaymentSourceSubmitButton_Click" runat="server" />
        <br />
        <br />
        <asp:Label Text="Inactive payment sources: " CssClass="labely" Width="200" runat="server" /><asp:DropDownList ID="inactivePaymentSourcesDropDown" runat="server" />
        <br />
        <asp:Label CssClass="labely" Width="200" runat="server" /><asp:Button ID="inactivePaymentSourcesSubmitButton" Text="Re-activate payment source" OnClick="inactivePaymentSourcesSubmitButton_Click" runat="server" />
        <hr />
    </asp:PlaceHolder>

</asp:Content>
