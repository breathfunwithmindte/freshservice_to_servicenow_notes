/**
* ******************************
*
*
*
* ******************************
*/

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace freshservice_service_migration.src
{


  class ExcelCompiler
  {

    private string jsonDirectory { get; set; }
    private string buildDirectory { get; set; }
    private List<string> jsonMinFileNames;
    private List<JObject> notes;

    public ExcelCompiler(string directory, string buildDirectory = "/build/")
    {
      this.jsonDirectory = Environment.CurrentDirectory + directory;
      this.buildDirectory = Environment.CurrentDirectory + buildDirectory;
      this.notes = new List<JObject>();
      this.jsonMinFileNames = Directory.GetFiles(this.jsonDirectory, "*.json").ToList<string>();
    }

    public void compile(string fileName)
    {
      foreach (var file in this.jsonMinFileNames)
      {
        var buffer = new StringBuilder();
        Console.WriteLine("Openning: " + file + "...");
        foreach (var line in File.ReadLines(file))
        {
          buffer.AppendLine(line);
        }
        JArray jsonArray = JArray.Parse(buffer.ToString());
        foreach (JObject item in jsonArray)
        {
          this.notes.Add(item);
        }

      }
      Console.WriteLine("Total notes after parsing was = " + this.notes.Count().ToString());

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
        headerRow.Append(CreateCell("ParentID"));
        headerRow.Append(CreateCell("NoteID"));
        headerRow.Append(CreateCell("UserID"));
        headerRow.Append(CreateCell("DisplayID"));
        headerRow.Append(CreateCell("Emails"));
        headerRow.Append(CreateCell("Body"));
        headerRow.Append(CreateCell("Index"));
        sheetData.Append(headerRow);

        // Write data to the cells
        foreach (JObject item in this.notes)
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