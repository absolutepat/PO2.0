using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using purchaseOrderProgram2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//using DocumentFormat.OpenXml.Packaging;
//using DocumentFormat.OpenXml.Spreadsheet;
//using DocumentFormat.OpenXml;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Spire.Xls;

namespace purchaseOrderProgram2
{
    public partial class oldData : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (endDateCalendar.SelectedDate.ToShortDateString().Equals("1/1/1900"))
            {
                endDateCalendar.SelectedDate = DateTime.Today.Date;
                endDateCalendar.VisibleDate = DateTime.Today.Date;
                endDateCalendarLabel.Text = "End Date: " + DateTime.Today.Date.ToShortDateString();
            }
            if (startDateCalendar.SelectedDate.ToShortDateString().Equals("1/1/1900"))
            {
                startDateCalendar.SelectedDate = new DateTime(2002, 11, 10);
                startDateCalendar.VisibleDate = new DateTime(2002, 11, 10);
                startDateCalendarLabel.Text = "Start Date: 11/10/2002";
            }

            if (ViewState["sortdr"] == null)
            { ViewState["sortdr"] = "Asc"; }
            if (Session["dirState"] != null)
            { overviewGridView.DataSource = (DataTable)Session["dirState"]; overviewGridView.DataBind(); }
            else
            { Select0r(); }

        }

        protected void DrillDown_Click(object sender, EventArgs e)
        {
            string[] arg = new string[2];
            arg = ((LinkButton)sender).CommandArgument.ToString().Split(',');


            DataTable noNulls = new DataTable();
            List<string> deleteTheseColumns = new List<string>();
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["oldDataConection"].ConnectionString);
            SqlCommand cmd = new SqlCommand(string.Format("SELECT [po{0}].*, [Details{0}].* FROM [Details{0}] JOIN [po{0}] on [Details{0}].[po#]=[po{0}].[po#] WHERE [po{0}].[po#] = {1};", arg[1], arg[0]), conn);

            if (conn.State == ConnectionState.Closed)
            { conn.Open(); }
            noNulls.Load(cmd.ExecuteReader());
            if (conn.State == ConnectionState.Open)
            { conn.Close(); }

            for (int x = 0; x < noNulls.Rows.Count; x++)
            {
                for (int y = 0; y < noNulls.Columns.Count; y++)
                {
                    if (string.IsNullOrWhiteSpace(noNulls.Rows[x][y].ToString()))
                    {
                        deleteTheseColumns.Add(noNulls.Columns[y].ToString());
                    }

                }
            }

            foreach (string column in deleteTheseColumns)
            {
                noNulls.Columns.Remove(column);
            }

            drillDownGridView.DataSource = noNulls;
            drillDownGridView.DataBind();
            drillDownWindow.Visible = true;
        }



        protected void CloseDrillDownWindow_Click(object sender, EventArgs e)
        {
            drillDownWindow.Visible = false;
        }

        protected void Select0r()
        {
            overviewDataSource.SelectCommand = string.Format("SELECT * FROM [PO{0}] ORDER BY[PO{0}].[po#]", dbSelct0rDropDownList.SelectedValue.ToString());

            DataTable addDepartmentNameColumn = new DataTable();
            DataColumn deptNameColumn = new DataColumn("Department", typeof(string));
            deptNameColumn.DefaultValue = "x";
            addDepartmentNameColumn.Columns.Add(deptNameColumn);
            List<string> fromTheseDepartments = new List<string>();
            //string selectCommand = ""; 
            SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["oldDataConection"].ConnectionString);

            conn.Open();
            //foreach (DropDownList department in dbSelct0rDropDownList.Items)
            if (dbSelct0rDropDownList.SelectedItem.Text == "All Departments")
            {
                for (int i = 0; i < dbSelct0rDropDownList.Items.Count; i++)
                {
                    if (dbSelct0rDropDownList.Items[i].Text != dbSelct0rDropDownList.SelectedItem.Text)
                    {
                        deptNameColumn.DefaultValue = dbSelct0rDropDownList.Items[i].Text;
                        SqlCommand cmd = new SqlCommand(string.Format("SELECT * FROM [po{0}]", dbSelct0rDropDownList.Items[i].Text), conn);
                        addDepartmentNameColumn.Load(cmd.ExecuteReader());
                    }
                }
            }
            else
            {
                deptNameColumn.DefaultValue = dbSelct0rDropDownList.SelectedItem.Text;
                SqlCommand cmd = new SqlCommand(string.Format("SELECT * FROM [po{0}]", dbSelct0rDropDownList.SelectedItem.Text), conn);
                addDepartmentNameColumn.Load(cmd.ExecuteReader());
            }
            conn.Close();
            overviewGridView.DataSourceID = null;
            DataView dv = addDepartmentNameColumn.DefaultView;
            dv.Sort = "Department DESC";
            overviewGridView.DataSource = addDepartmentNameColumn;
            Session["dirState"] = addDepartmentNameColumn;

            overviewGridView.DataBind();

        }

        protected void overviewGridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable dtrslt = (DataTable)Session["dirState"];
            if (dtrslt.Rows.Count > 0)
            {
                if (ViewState["sortdr"].ToString() == "Asc")
                {
                    dtrslt.DefaultView.Sort = e.SortExpression + " Desc";
                    ViewState["sortdr"] = "Desc";
                }
                else
                {
                    dtrslt.DefaultView.Sort = e.SortExpression + " Asc";
                    ViewState["sortdr"] = "Asc";
                }
                Session["dirState"] = dtrslt;
                overviewGridView.DataSource = dtrslt;
                overviewGridView.DataBind();
            }
        }

        protected void dbSelct0rDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Select0r();
        }

        protected void submitFilterButton_Click(object sender, EventArgs e)
        {

        }

        protected void exportToExcelButton_Click(object sender, EventArgs e) {

            string fileName = "Export from PO2_" + DateTime.Now.ToString().Replace(@"/", "").Replace(":", "").Replace(" ", ".").Replace("PM", "").Replace("AM", "") + ".xlsx";
            string tempDir = (AppDomain.CurrentDomain.BaseDirectory + "fileSwap\\");

            Workbook workbook = new Workbook();
            Worksheet sheet = workbook.Worksheets[0];
            sheet.InsertDataTable((DataTable)Session["dirState"], true, 1, 1);
            workbook.SaveToFile(tempDir + fileName, ExcelVersion.Version2013);
            //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //Response.AppendHeader("content-disposition", string.Format("attachment; filename={0}", fileName));
            //workbook.SaveToHttpResponse("gffgfgfg.xlsx", HttpContext.Current.Response, HttpContentType.Excel2010 );
            //Response.End();

        }

        //protected void exportToExcelButton_Click(object sender, EventArgs e)
        //{
        //    DataTable dt = (DataTable)Session["dirState"];
        //    string fileName = "Export from PO2_" + DateTime.Now.ToString().Replace(@"/", "").Replace(":", "").Replace(" ", ".").Replace("PM", "").Replace("AM", "") + ".xlsx";
        //    string tempDir = (AppDomain.CurrentDomain.BaseDirectory + "fileSwap\\");
        //    // Create a spreadsheet document by supplying the filepath.
        //    // By default, AutoSave = true, Editable = true, and Type = xlsx.
        //    SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create( tempDir + fileName , SpreadsheetDocumentType.Workbook);

        //    // Add a WorkbookPart to the document.
        //    WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
        //    workbookpart.Workbook = new Workbook();

        //    // Add a WorksheetPart to the WorkbookPart.
        //    WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
        //    worksheetPart.Worksheet = new Worksheet(new SheetData());

        //    // Add Sheets to the Workbook.
        //    Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.
        //        AppendChild<Sheets>(new Sheets());

        //    // Append a new worksheet and associate it with the workbook.
        //    Sheet sheet = new Sheet()
        //    {
        //        Id = spreadsheetDocument.WorkbookPart.
        //        GetIdOfPart(worksheetPart),
        //        SheetId = 1,
        //        Name = "mySheeeet"
        //    };
        //    //sheet.Insert
        //    sheets.Append(sheet);

        //    workbookpart.Workbook.Save();

        //    // Close the document.
        //    spreadsheetDocument.Close();

        //    saveFile(tempDir, fileName);

        //    //SaveFileDialog saveFileDialog = new SaveFileDialog();
        //    //
        //    //Response.

        //    //spreadsheetDocument.Close();

        //    System.IO.File.Delete(tempDir + fileName);
        //}

        private void saveFile(string path, string fileName) {
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AppendHeader("content-disposition", string.Format("attachment; filename={0}", fileName));
            Response.WriteFile(path + fileName);
            Response.End();
        }

       

        public static byte[] GetBytesFromDataSet(DataTable ds)
        {
            byte[] data = null;
            using (MemoryStream stream = new MemoryStream())
            {
                IFormatter bf = new BinaryFormatter();
                ds.RemotingFormat = SerializationFormat.Binary;
                bf.Serialize(stream, ds);
                data = stream.ToArray();
            }
            return data;
        }

        //public static void CreateSpreadsheetWorkbook(MemoryStream filepath)
        //{
        //    // Create a spreadsheet document by supplying the filepath.
        //    // By default, AutoSave = true, Editable = true, and Type = xlsx.
        //    SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.
        //        Create(filepath, SpreadsheetDocumentType.Workbook);

        //    // Add a WorkbookPart to the document.
        //    WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
        //    workbookpart.Workbook = new Workbook();

        //    // Add a WorksheetPart to the WorkbookPart.
        //    WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
        //    worksheetPart.Worksheet = new Worksheet(new SheetData());

        //    // Add Sheets to the Workbook.
        //    Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.
        //        AppendChild<Sheets>(new Sheets());

        //    // Append a new worksheet and associate it with the workbook.
        //    Sheet sheet = new Sheet()
        //    {
        //        Id = spreadsheetDocument.WorkbookPart.
        //        GetIdOfPart(worksheetPart),
        //        SheetId = 1,
        //        Name = "mySheet"
        //    };
        //    sheets.Append(sheet);

        //    workbookpart.Workbook.Save();

        //    // Close the document.
        //    spreadsheetDocument.Close();

        //    //Response.AppendHeader("content-disposition", "attachment; filename=grrr.xlsx");
        //    //Response.Write(filepath);
        //}
    }
}
