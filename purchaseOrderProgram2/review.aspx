<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="review.aspx.cs" Inherits="purchaseOrderProgram2.review" %>

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
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <img src="~/images/review.png" class="titlePic" runat="server" />
    <asp:LinkButton ID="reviewInstructionsPlaceholderShowHide" Text="Show Instructions" OnClick="reviewInstructionsPlaceholderShowHide_Click" runat="server" />

    <div id="drillDownWindow" visible="false" class="drillDownWindow"  runat="server">
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

    <br />
    <asp:PlaceHolder ID="reviewInstructionsPlaceholder" Visible="false" runat="server">
        <div>
            In this area you can review PO's. Use the search critera to filter down the list. Click on the PO number to see the whole PO.
            You need "persmision" fpr each department you need to refview purchased item. Ask a Purchasing Agent to change assign you to the Review group
            for the department. At the bottom you can export PO's to excell, or prin them off. They'll automatically format themsleves and print one to a page!  
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
            <asp:BoundField DataField="poDateSubmitedForReconciliation" DataFormatString="{0:d}" HeaderText="Sumbited for reconciliation" SortExpression="poDateSubmitedForReconciliation" />
            <asp:BoundField DataField="poActive" HeaderText="Complete" SortExpression="poActive" />
        </Columns>
    </asp:GridView>
    <asp:Button ID="selectAllButton" OnClick="selectAllButton_Click" Text="Select All" runat="server" />
    <asp:Button ID="exportSelectedToExcellButton" OnClick="exportSelectedToExcellButton_Click" Text="Export selected PO's to Excell" runat="server" />
    <asp:Button ID="printSelectedPOsButton_Complete" OnClick="printSelectedPOsButton_Complete_Click" Text="Print selected PO's" runat="server" />
    <hr />

</asp:Content>
