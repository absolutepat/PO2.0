<%@ Page Language="C#" Title="PO 2.0: OldData" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="oldData.aspx.cs" Inherits="purchaseOrderProgram2.oldData" %>

<asp:Content ID="addToHead" ContentPlaceHolderID="head" runat="Server">
    <style>
        .itemBoxes {
            border: solid;
            margin: 15px;
            padding: 5px;
            border-width: 2px;
            border-color: black;
            border-radius: 10px;
        }

        .itemBoxesHeader {
            margin: 0px;
        }

        .forceInline {
            display: inline-block;
        }
    </style>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div id="drillDownWindow" visible="false" style="z-index: 100; overflow: scroll; border: double; border-width: thick; border-color: red; width: 90%; height: 300px; top: 10%; left: 5%; background-color: bisque; position: fixed;" runat="server">
        <div style="position: fixed">
            <asp:Button ID="closeDrillDownWindowButton" OnClick="CloseDrillDownWindow_Click" Text="Close window" runat="server" />
        </div>
        <br />
        <br />
        <asp:GridView ID="drillDownGridView" runat="server" />
    </div>
    <div style="clear: both">

        <h1>Data from the Old PO Program</h1>
        <br />
    </div>


    <div style="position: relative; float: left;" class="itemBoxes">
        <h2>Filter Criteria:</h2>

        <div class="itemBoxes">
            <h3 class="itemBoxesHeader">From which departments:</h3>
            <asp:CheckBoxList ID="departmentCheckBox" RepeatColumns="3" RepeatDirection="Horizontal" runat="server">
                <asp:ListItem Text="Admin" Value="Admin" />
                <asp:ListItem Text="Billing" Value="Billing" />
                <asp:ListItem Text="Bly" Value="Bly" />
                <asp:ListItem Text="Dental" Value="Dental" />
                <asp:ListItem Text="FD" Value="FD" />
                <asp:ListItem Text="IS" Value="IS" />
                <asp:ListItem Text="Medical" Value="Medical" />
                <asp:ListItem Text="MedRecs" Value="MedRecs" />
                <asp:ListItem Text="MH" Value="MH" />
                <asp:ListItem Text="Outreach" Value="Outreach" />
                <asp:ListItem Text="All Departments" Selected="True" Value="All Departments" />
            </asp:CheckBoxList>
        </div>

        <div class="itemBoxes">
            <h3 class="itemBoxesHeader">From this date range:</h3>
            <div class="forceInline">
                <asp:Label ID="startDateCalendarLabel" Text="Start Date" runat="server"></asp:Label>
                <asp:Calendar ID="startDateCalendar" SelectedDate="01/01/1900" SelectionMode="Day" runat="server"></asp:Calendar>
            </div>
            <b>&nbsp;-to-&nbsp;</b>
            <div class="forceInline">
                <asp:Label ID="endDateCalendarLabel" Text="End Date" runat="server"></asp:Label>
                <asp:Calendar ID="endDateCalendar" SelectedDate="01/01/1900" runat="server"></asp:Calendar>
            </div>
        </div>

        <div class="itemBoxes">
            <h3 class="itemBoxesHeader">Total from:</h3><br />
            Low:&nbsp;
            <asp:TextBox ID="lowPriceTextBox" TextMode="Number" Text="0" runat="server"></asp:TextBox>
            <b>&nbsp;-to-&nbsp;</b> High:&nbsp;
            <asp:TextBox ID="highPriceTextBox" TextMode="Number" Text="1000000000" runat="server"></asp:TextBox>
        </div>

        <div class="itemBoxes">
            <h3 class="itemBoxesHeader">Buyer was:</h3>
            <asp:ListBox ID="purchaserListBox" runat="server"></asp:ListBox><br />
            <asp:Button ID="submitFilterButton" Text="Filter!" OnClick="submitFilterButton_Click" runat="server" />
        </div>
    </div>



    <div style="position: relative; float: left; margin: 15px; padding: 10px; border: solid; border-width: 2px; border-color: black; border-radius: 10px;">
        <h2>Results</h2>
        <asp:DropDownList ID="dbSelct0rDropDownList" AutoPostBack="true" OnSelectedIndexChanged="dbSelct0rDropDownList_SelectedIndexChanged" runat="server">
            <asp:ListItem Text="Admin" Value="Admin" />
            <asp:ListItem Text="Billing" Value="Billing" />
            <asp:ListItem Text="Bly" Value="Bly" />
            <asp:ListItem Text="Dental" Value="Dental" />
            <asp:ListItem Text="FD" Value="FD" />
            <asp:ListItem Text="IS" Value="IS" />
            <asp:ListItem Text="Medical" Value="Medical" />
            <asp:ListItem Text="MedRecs" Value="MedRecs" />
            <asp:ListItem Text="MH" Value="MH" />
            <asp:ListItem Text="Outreach" Value="Outreach" />
            <asp:ListItem Text="All Departments" Value="All Departments" />
        </asp:DropDownList>

        <asp:GridView ID="overviewGridView" AutoGenerateColumns="false" AllowSorting="true" OnSorting="overviewGridView_Sorting" AllowPaging="true" PageSize="100" runat="server">

            <Columns>
                <asp:BoundField DataField="Department" HeaderText="Department" SortExpression="Department" />
                <asp:TemplateField HeaderText="PO #" SortExpression="po#">
                    <ItemTemplate>
                        <asp:LinkButton OnClick="DrillDown_Click" CommandArgument='<%#Eval("po#") + "," + Eval("Department") %>' runat="server">
                        <asp:Label Text='<%#Eval("po#") %>' runat="server"/></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Date" SortExpression="Date">
                    <ItemTemplate>
<%--                        <itemtemplate>--%>
                        <asp:Label text='<%#Eval("Date", "{0:dd, MMM yyyy}") %>' runat="server" />
                    <%--</itemtemplate>--%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Buyer" HeaderText="Buyer" SortExpression="Buyer" />
                <asp:TemplateField HeaderText="Total" SortExpression="Total">
                    <ItemTemplate>
                        <%--<itemtemplate>--%>
                        <asp:Label text='<%#Eval("Total") %>' runat="server" />
                    <%--</itemtemplate>--%>
                    </ItemTemplate>
                </asp:TemplateField>


            </Columns>
        </asp:GridView>
        <br />
        <asp:Button ID="exportToExcelButton" Text="Export to Excel" OnClick="exportToExcelButton_Click" runat="server" />
    </div>
    <asp:SqlDataSource ID="overviewDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:oldDataConection %>" />

    <div style="clear: both;">&nbsp;</div>
</asp:Content>
