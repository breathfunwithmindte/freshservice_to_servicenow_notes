/// <summary>
/// 
/// </summary>


using freshservice_to_servicenow_notes.src.types;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace freshservice_to_servicenow_notes.src
{
  
  interface ParserItem <T>
  {
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    void Run(JObject obj, List<T> notes);


  }


  class Parser <T>
  {

    /// <summary>
    ///   The build folder inside root directory;
    /// </summary>
    /// <value>{root}/build</value>
    private string buildDirectory { get; set; }
    private List<T> notes;

    public Parser ()
    {
      this.buildDirectory = Environment.CurrentDirectory + "/build/";
      this.notes = new List<T>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public void ReadJson<N>(string fileName) where N : ParserItem<T>, new()
    {
        using (var fileStream = new FileStream(this.buildDirectory + fileName + ".json", FileMode.Open))
        {
            using (var streamReader = new StreamReader(fileStream))
            {
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    var serializer = new JsonSerializer();
                    JArray? jsonArray = serializer.Deserialize<JArray>(jsonReader);

                    if(jsonArray == null)
                    {
                      Console.WriteLine("JSON IS NOT ARRAY TYPE.");
                      return;
                    }

                    foreach (JObject item in jsonArray)
                    {
                        N note = new N();
                        note.Run(item, this.notes);
                    }

                    string notesJson = JsonConvert.SerializeObject(this.notes);
                    File.WriteAllText(this.buildDirectory + fileName + ".min.json", notesJson);
                    Console.WriteLine(this.notes.Count());
                }
            }
        }
    }


  }

}