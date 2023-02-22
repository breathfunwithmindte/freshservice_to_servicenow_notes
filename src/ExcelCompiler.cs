/**
* ******************************
*
*
*
* ******************************
*/

using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace freshservice_to_servicenow_notes.src
{


  class ExcelCompiler
  {
    private string buildDirectory { get; set; }
    public ExcelCompiler ()
    {
      this.buildDirectory = Environment.CurrentDirectory + "/build/";
    }

    public void compile(string fileName)
    {
    string json = File.ReadAllText(this.buildDirectory + fileName + ".min.json");
    JArray jsonArray = JArray.Parse(json);

    // Create a new Excel workbook
    using (SpreadsheetDocument document = SpreadsheetDocument.Create(this.buildDirectory + fileName + ".min.xlsx", SpreadsheetDocumentType.Workbook))
    {
        WorkbookPart workbookPart = document.AddWorkbookPart();
        workbookPart.Workbook = new Workbook();

        WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
        Worksheet worksheet = new Worksheet();
        SheetData sheetData = new SheetData();

        //worksheet.SheetFormatProperties.DefaultColumnWidth = 16.86;

        // Add header row to the worksheet
        Row headerRow = new Row();
        headerRow.Append(CreateCell("Parent ID"));
        headerRow.Append(CreateCell("Note ID"));
        headerRow.Append(CreateCell("User ID"));
        headerRow.Append(CreateCell("Display ID"));
        headerRow.Append(CreateCell("Emails"));
        headerRow.Append(CreateCell("Body"));
        headerRow.Append(CreateCell("Index"));
        sheetData.Append(headerRow);

        // Write data to the cells
        foreach (JObject item in jsonArray)
        {
            Row dataRow = new Row();
            dataRow.Append(CreateCell(item["parent_id"]?.ToString() ?? ""));
            dataRow.Append(CreateCell(item["note_id"]?.ToString() ?? ""));
            dataRow.Append(CreateCell(item["user_id"]?.ToString() ?? ""));
            dataRow.Append(CreateCell(item["display_id"]?.ToString() ?? ""));
            dataRow.Append(CreateCell(item["emails"]?.ToString() ?? ""));
            dataRow.Append(CreateCell(item["body"]?.ToString() ?? ""));
            dataRow.Append(CreateCell(item["index"]?.ToString() ?? ""));
            sheetData.Append(dataRow);
        }

        worksheet.Append(sheetData);
        worksheetPart.Worksheet = worksheet;
        Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
        Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Sheet 1" };
        sheets.Append(sheet);
        workbookPart.Workbook.Save();
    }
    
    }

    private Cell CreateCell(string text)
    {
        Cell cell = new Cell();
        cell.DataType = CellValues.String;
        cell.CellValue = new CellValue(text);
        return cell;
    }

  }

}