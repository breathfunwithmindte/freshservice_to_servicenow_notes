using System;
using freshservice_to_servicenow_notes.src;
using freshservice_to_servicenow_notes.src.types;

namespace freshservice_to_servicenow_notes
{
  class Program
  {
    static void Main(string[] args)
    {
      var command = args[0];
      var commandProp = args[1];

      Console.WriteLine(command);
      Console.WriteLine(commandProp);

      XMLReader xmlReader = new XMLReader("/source_xml");
      Parser<NoteElement> parser = new Parser<NoteElement>();
      ExcelCompiler excelCompiler = new ExcelCompiler();

      if (command == "build")
      {
        switch (commandProp)
        {
          case "json":
            xmlReader.parseToSingleList(2);
            xmlReader.writeToJsonFile("primary");
            //xmlReader.logInstance();
            break;
          case "json_min":
            parser.ReadJson<Note>("primary");
            break;
          case "excel":
            excelCompiler.compile("primary");
            break;
          default: break;
        }
      }



      Console.WriteLine("\n\n\nProgram executed with no errors ... \n\n");
    }
  }
}