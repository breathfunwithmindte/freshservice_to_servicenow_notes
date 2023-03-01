/// <summary>
/// 
/// </summary>


using freshservice_service_migration.src.types;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;

namespace freshservice_service_migration.src
{

  interface ParserItem<T>
  {
    int Run(JObject obj, List<T> notes, string file);

  }


  class Parser<T>
  {
    private string jsonDirectory { get; set; }
    private string buildDirectory { get; set; }
    private List<string> jsonFileNames;
    private List<T> notes;

    private long totalNotes = 0;

    public Parser(string directory, string buildDirectory = "/build/json_min/")
    {
      this.jsonDirectory = Environment.CurrentDirectory + directory;
      this.buildDirectory = Environment.CurrentDirectory + buildDirectory;
      this.notes = new List<T>();
      this.jsonFileNames = Directory.GetFiles(this.jsonDirectory, "*.json").ToList<string>();

    }

    public void readJSONListFormat<N>() where N : ParserItem<T>, new()
    {
      foreach (var file in this.jsonFileNames)
      {
        var buffer = new StringBuilder();
        this.notes = new List<T>();
        Console.WriteLine("Openning: " + file + "...");
        foreach (var line in File.ReadLines(file))
        {
          buffer.AppendLine(line);
        }
        JArray jsonArray = JArray.Parse(buffer.ToString());
        foreach (JObject item in jsonArray)
        {
          N note = new N();
          int result = note.Run(item, this.notes, file);
        }
        totalNotes = totalNotes + this.notes.Count();
        string notesJson = JsonConvert.SerializeObject(this.notes);
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
        // logs
        string incidentsWas = "\t\tIncident Counted: " + jsonArray.Count().ToString() + ";\t";
        string notesWas = "Notes Counted: " + this.notes.Count().ToString() + ";\t";
        Console.WriteLine("file processed:" + fileNameWithoutExtension + incidentsWas + notesWas + "\n");
        File.WriteAllText(this.buildDirectory + fileNameWithoutExtension + ".min.json", notesJson);
      }
      Console.WriteLine("Total notes after parsing was = " + this.totalNotes.ToString());

    }

    public void logFiles()
    {
      foreach (string i in this.jsonFileNames)
      {
        Console.WriteLine(i);
      }
    }

  }

}

/**
 if (!File.Exists(this.buildDirectorypr + "primary_min.json"))
                {
                    File.WriteAllText(this.buildDirectorypr + "primary_min.json", notesJson);
                }
                else
                {
                    File.AppendAllText(this.buildDirectorypr + "primary_min.json", notesJson);
                }
*/