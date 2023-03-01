/// <summary>
/// 
/// </summary>
/// 
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace freshservice_service_migration.src.types
{
  class NoteElement
  {
    public string parent_id { get; set; }
    public string? note_id { get; set; }
    public string user_id { get; set; }
    public string display_id { get; set; }
    public string? emails { get; set; }
    public string body { get; set; }
    public long index { get; set; }

    public NoteElement (string parent_id, string? note_id, string user_id, string display_id, string? emails, string body, long index)
    {
      this.parent_id = parent_id;
      this.note_id = note_id;
      this.user_id = user_id;
      this.display_id = display_id;
      this.emails = emails;
      this.body = body;
      this.index = index;
    }

  }


}