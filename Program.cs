using System;
using freshservice_service_migration.src;
using freshservice_service_migration.src.types;

namespace freshservice_service_migration
{
  class Program
  {
    static void Main(string[] args)
    {
      var command = args[0];
      var commandProp = args[1];

      Console.WriteLine(command);
      Console.WriteLine(commandProp);

      XMLReader xmlReader = new XMLReader("/source_xml", "/build/json/");
      Parser<NoteElement> parser = new Parser<NoteElement>("/build/json", "/build/json_min/");
      ExcelCompiler excelCompiler = new ExcelCompiler("/build/json_min", "/build/");

      if (command == "build")
      {
        switch (commandProp)
        {
          case "json":
            xmlReader.parseAllFilesAndWrite(150); break;
          case "json_min":
            parser.readJSONListFormat<Note>(); break;
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