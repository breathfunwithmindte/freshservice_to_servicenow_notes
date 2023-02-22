using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// 
/// </summary>
namespace freshservice_to_servicenow_notes.src
{

  class XMLReader
  {

    private string xmlDirectory { get; set; }
    private string buildDirectory { get; set; }
    private List<string> xmlFileNames;
    private List<object> items;

    public XMLReader (string directory)
    {
      this.xmlDirectory = Environment.CurrentDirectory + directory;
      this.buildDirectory = Environment.CurrentDirectory + "/build/";
      this.items = new List<object>();
      this.xmlFileNames = Directory.GetFiles(this.xmlDirectory, "*.xml").ToList<string>();
    }

    public void parseToSingleList (int limiter)
    {
        if(limiter == 0) 
        {
          limiter = this.xmlFileNames.Count();
        }
        foreach (string file in this.xmlFileNames.Take(limiter))
        {
            using (StreamReader reader = new StreamReader(file))
            {
              XDocument doc = XDocument.Load(reader);
              XElement elements = doc.Root ?? new XElement("default");

              if(elements.HasElements == false)
              {
                Console.WriteLine("NOT AN ARRAY");
              }

              foreach (var element in elements.Elements())
              {
                  this.items.Add(element);
              }
            }
        }
        Console.WriteLine(this.items.Count());
    }

    public void writeToJsonFile(string fileName)
    {
        string json = JsonConvert.SerializeObject(this.items);
        using (StreamWriter writer = new StreamWriter(this.buildDirectory + fileName + ".json"))
        {
            writer.Write(json);
        }
    }


    public void logInstance ()
    {
      Console.WriteLine(this.buildDirectory);
      foreach (string file in this.xmlFileNames)
      {
        Console.WriteLine(file);
      }
    }

  }


}