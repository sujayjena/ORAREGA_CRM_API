using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
//using System.Web.Mvc;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Office2016.Excel;

namespace OraRegaAV.Models
{
    public class ExcelGenerator
    {
        public ExcelGenerator()
        {

        }

        public byte[] DownloadExcelFormat(List<string> columns, string fileName)
        {
            byte[] byteResult = null;
            using (MemoryStream stream = new MemoryStream())
            {
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new SheetData();
                    worksheetPart.Worksheet = new Worksheet(sheetData);

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Sheet1" };

                    sheets.Append(sheet);
                    Row headerRow = new Row();
                    foreach (var column in columns)
                    {
                        Cell cell = new Cell();
                        cell.DataType = CellValues.String;
                        cell.CellValue = new CellValue(column);
                        headerRow.AppendChild(cell);
                    }

                    sheetData.AppendChild(headerRow);
                    workbookPart.Workbook.Save();

                    stream.Flush();
                    stream.Position = 0;

                    byteResult = new byte[stream.Length];
                    stream.Read(byteResult, 0, byteResult.Length);
                }
            }
            return byteResult;
        }


        //public MemoryStream DownloadExcelFormat(List<string> columns, string fileName)
        //{
        //    MemoryStream stream = new MemoryStream();
        //    using (SpreadsheetDocument document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
        //    {
        //        WorkbookPart workbookPart = document.AddWorkbookPart();
        //        workbookPart.Workbook = new Workbook();

        //        WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
        //        var sheetData = new SheetData();
        //        worksheetPart.Worksheet = new Worksheet(sheetData);

        //        Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
        //        Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Sheet1" };

        //        sheets.Append(sheet);
        //        Row headerRow = new Row();
        //        foreach (var column in columns)
        //        {
        //            Cell cell = new Cell();
        //            cell.DataType = CellValues.String;
        //            cell.CellValue = new CellValue(column);
        //            headerRow.AppendChild(cell);
        //        }

        //        sheetData.AppendChild(headerRow);
        //        workbookPart.Workbook.Save();
        //        return stream;
        //    }


        public static string WriteExcelFile<T>(List<T> exportDataList, string fileName)
        {
            string base64 = string.Empty;
            string fullFilename = fileName +"_" + DateTime.Now.ToString("yyyyMMddHHmmss").Replace(" ", "_") + ".xlsx";
            DataTable table = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(exportDataList), (typeof(DataTable)));
            byte[] byteResult = null;
            using (MemoryStream stream = new MemoryStream())
            {
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new SheetData();
                    worksheetPart.Worksheet = new Worksheet(sheetData);

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Sheet1" };

                    sheets.Append(sheet);
                    Row headerRow = new Row();
                    List<String> columns = new List<string>();
                    foreach (System.Data.DataColumn column in table.Columns)
                    {
                        columns.Add(column.ColumnName);
                        Cell cell = new Cell();
                        cell.DataType = CellValues.String;
                        cell.CellValue = new CellValue(column.ColumnName);
                        headerRow.AppendChild(cell);
                    }

                    sheetData.AppendChild(headerRow);
                    foreach (DataRow dsrow in table.Rows)
                    {
                        Row newRow = new Row();
                        foreach (String col in columns)
                        {
                            Cell cell = new Cell();
                            cell.DataType = CellValues.String;
                            cell.CellValue = new CellValue(dsrow[col].ToString());
                            newRow.AppendChild(cell);
                        }

                        sheetData.AppendChild(newRow);
                    }
                    workbookPart.Workbook.Save();
                    stream.Flush();
                    stream.Position = 0;

                    byteResult = new byte[stream.Length];
                    stream.Read(byteResult, 0, byteResult.Length);
                }
            }
            base64 = Convert.ToBase64String(byteResult, 0, byteResult.Length);
            return base64;
        }
    }
}


