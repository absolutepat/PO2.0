<%@ Page Language="C#" Title="PO 2.0: Home" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="purchaseOrderProgram2.Home" %>

<asp:Content ID="addToHead" ContentPlaceHolderID="head" runat="server">
    <style>
        .itemBoxes {
            border: solid;
            margin: 0px;
            padding: 0px;
            border-width: 2px;
            border-color: black;
            border-radius: 10px;
            display: flex;
            justify-content: center;
            align-items: center;
            overflow: auto;
        }

            .itemBoxes:hover {
                background-color: rgba(205, 24, 241, 0.44);
            }

        .itemBoxesHeader {
            margin: 0px;
        }

        .bigBoxes {
            width: 400px;
            float: left;
            margin: 10px;
            display: inline-block;
        }
    </style>
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <img src="~/images/homeArea.png" class="titlePic" runat="server" />
    <hr />
    <div style="display: flex; justify-content: center; align-items: center; overflow: auto;">
        <div class="bigBoxes">
            <a href="/newPO.aspx">
                <div class="itemBoxes">
                    <h3>New PO</h3>
                </div>
            </a>
            <hr />
            <a href="/review.aspx">
                <div class="itemBoxes">
                    <h3>Review PO's</h3>
                </div>
            </a>
            <hr />
            <a href="/edit.aspx">
                <div class="itemBoxes">
                    <h3>Edit PO's</h3>
                </div>
            </a>
        </div>

        <div class="bigBoxes">
            <a href="/reports.aspx">
                <div class="itemBoxes">
                    <h3>Reporting</h3>
                </div>
            </a>
            <hr />
            <a href="/purchasingAgent.aspx">
                <div class="itemBoxes">
                    <h3>Purchasing Agent</h3>
                </div>
            </a>
            <hr />
            <a href="/admin.aspx">
                <div class="itemBoxes">
                    <h3>Admin</h3>
                </div>
            </a>
        </div>
    </div>
</asp:Content>
