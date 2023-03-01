using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// 
/// </summary>
namespace freshservice_service_migration.src
{

  class XMLReader
  {

    private string xmlDirectory { get; set; }
    private string buildDirectory { get; set; }
    private List<string> xmlFileNames;
    private List<object> items;

    public XMLReader (string directory, string buildDirectory = "/build/")
    {
      this.xmlDirectory = Environment.CurrentDirectory + directory;
      this.buildDirectory = Environment.CurrentDirectory + buildDirectory;
      this.items = new List<object>();
      this.xmlFileNames = Directory.GetFiles(this.xmlDirectory, "*.xml").ToList<string>();
    }

    public void parseToSingleList (int limiter)
    {
        int countFiles  = 0;

        if(limiter == 0) 
        {
          limiter = this.xmlFileNames.Count();
        }
        //List<string> allFiles = this.xmlFileNames;
        //foreach (string file in allFiles)
        foreach (string file in this.xmlFileNames.Take(200))
        {   
            countFiles++;
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
            Console.WriteLine("counter = " + countFiles.ToString() + " file processed:" + file);
        }
        Console.WriteLine(this.items.Count());
    }
        public void parseAllFilesAndWrite(int maxAmount = 0)
        {
          int countLoop = 0;
          maxAmount = maxAmount == 0 ? this.xmlFileNames.Count : maxAmount;
          for(int i = 0; i < this.xmlFileNames.Count; i++)
          {
            parseOneFileAndWrite(i);
            countLoop++;
            if(countLoop > maxAmount) break;
          }
        }
        public void parseOneFileAndWrite(int pos)
        {
            string filePath = this.xmlFileNames[pos];
            this.items = new List<object>();

            using (StreamReader reader = new StreamReader(filePath))
            {
              XDocument doc = XDocument.Load(reader);
              XElement elements = doc.Root ?? new XElement("default");

              if (elements.HasElements == false)
              {
                  Console.WriteLine("NOT AN ARRAY");
              }

              foreach (var element in elements.Elements())
              {
                  this.items.Add(element);
              }
            }
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            Console.WriteLine("file processed:" + fileNameWithoutExtension);
            writeToJsonFile(fileNameWithoutExtension);
        }

        public void writeToJsonFile(string fileName)
    {
            using (var fileStream = new FileStream(this.buildDirectory + fileName + ".json", FileMode.Create))
            {
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    using (var jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        var serializer = new JsonSerializer();
                        serializer.Serialize(jsonWriter, this.items);
                        jsonWriter.Flush();
                    }
                }
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