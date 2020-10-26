<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="printing.aspx.cs" Inherits="purchaseOrderProgram2.printing" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        .pageBreak{             
                text-align: center;
                font-style:italic;
                font-size:small;

        }
        .label{
            width:200px;
            text-align:right;
            display:inline-block;
            font-size:larger;
            font-style:italic;
            padding-right:5px;
            
        }
        .value{
            width:300px;
            text-align:left;
            display:inline-block;
            font-size:larger;
            font-weight:600;
        }

        .lineitemLabel{
            width:100px;
            text-align:right;
            display:inline-block;
            font-size:larger;
            font-style:italic;
            padding-right:5px;
            bottom: 0px;

        }
        .lineitemValue{
            width:100px;
            text-align:left;
            display:inline-block;
            font-size:larger;
            font-weight:600;
        }
        .lineNumber{
            width:10px;
            text-align:left;
            display:inline-block;
            font-size:larger;
            font-weight:600;
        }

        .line{
            display:block;
        }

        @media print {
            .pageBreak {
                page-break-after: always;
            }
        }
    </style>
</head>
<body onload="window.print(); setTimeout('window.close()', 500);">
    <form id="body" runat="server">
        <asp:Label ID="nothingToPrintMessage" Text="Nothing Selected to Print" Visible="false" runat="server" />
    </form>
</body>
</html>
