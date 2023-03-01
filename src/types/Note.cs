/// <summary>
/// 
/// </summary>
/// 
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using freshservice_service_migration.src;

namespace freshservice_service_migration.src.types
{

  class Note : ParserItem<NoteElement>
  {
    static int counter = 0;
    private string[] confemails = { "cc-emails::cc-email", "fwd-emails::fwd-email", "reply-cc::reply-cc", "tkt-cc::tkt-cc", "tkt-bcc::tkt-cc" };

    public int Run(JObject obj, List<NoteElement> notes, string file)
    {
      var parent_id = obj["helpdesk-ticket"]?["id"]?["#text"]?.ToString();
      var parent_user_id = obj["helpdesk-ticket"]?["requester-id"]?["#text"]?.ToString();
      var parent_body = obj["helpdesk-ticket"]?["description"]?.ToString();
      var display_id = obj["helpdesk-ticket"]?["display-id"]?["#text"]?.ToString();


      JToken? helpdeskNoteTokenParent = obj["helpdesk-ticket"]?["helpdesk-notes"];

      string mails = this.GetCCEmail(obj["helpdesk-ticket"]?["cc-email"], obj["helpdesk-ticket"]?["to-email"]?.ToString());

      if (parent_id == null || parent_user_id == null || parent_body == null || display_id == null)
      {
        Console.WriteLine("\n THERE IS A PROBLEM MISSING PROPERTIES \n");
        Console.WriteLine(parent_body?.ToString() ?? "parent_body is null");
        Console.WriteLine(parent_user_id?.ToString() ?? "parent_user_id is null");
        Console.WriteLine(display_id?.ToString() ?? "display_id is null");
        Console.WriteLine(parent_id?.ToString() ?? "parent_id is null");
        System.Environment.Exit(1);
        return 0;
      }

      NoteElement primaryNote = new NoteElement(parent_id, null, parent_user_id, display_id, mails, parent_body, 0);

      notes.Add(primaryNote);
      if (helpdeskNoteTokenParent == null) return 0;

      if (helpdeskNoteTokenParent is JArray)
      {
        long index = 1;
        foreach (JToken helpdeskNoteTokenParentItem in helpdeskNoteTokenParent)
        {
            JToken? helpdeskNoteToken = helpdeskNoteTokenParentItem?["helpdesk-note"];
            if(helpdeskNoteToken == null) continue;
            if (helpdeskNoteToken is JArray helpdeskNoteList)
            {
                List<NoteElement> ns = this.parseArrayNotes(helpdeskNoteList, parent_id, display_id, index);
                foreach (NoteElement ns_item in ns)
                {
                    notes.Add(ns_item);
                    index++;
                }
            }
            else if (helpdeskNoteToken is JObject helpdeskNoteObject)
            {
                NoteElement? n = this.parseObjectNote(helpdeskNoteObject, parent_id, display_id, index);
                if (n == null) return 0;
                notes.Add(n);
                index++;
            }
            
        }
      }
      else
      {
        JToken? helpdeskNoteToken = helpdeskNoteTokenParent?["helpdesk-note"];
        if (helpdeskNoteToken == null) return 0;
        if (helpdeskNoteToken is JArray helpdeskNoteList)
        {
          List<NoteElement> ns = this.parseArrayNotes(helpdeskNoteList, parent_id, display_id);
          foreach (NoteElement ns_item in ns)
          {
            notes.Add(ns_item);
          }
        }
        else if (helpdeskNoteToken is JObject helpdeskNoteObject)
        {
          NoteElement? n = this.parseObjectNote(helpdeskNoteObject, parent_id, display_id);
          if (n == null) return 0;
          notes.Add(n);
        }
      }
      return 0;
    }

    private NoteElement? parseObjectNote(JObject helpdeskNoteToken, string parent_id, string display_id, long index = 1)
    {
      var note_id = helpdeskNoteToken["id"]?["#text"]?.ToString();
      var note_user_id = helpdeskNoteToken["user-id"]?["#text"]?.ToString();
      var note_body = helpdeskNoteToken["body"]?.ToString();

      if (note_id == null || note_user_id == null || note_body == null)
      {
        Console.WriteLine("\n THERE IS A PROBLEM MISSING PROPERTIES IN NOTES \n");
        Console.WriteLine(note_id?.ToString() ?? "note_id is null");
        Console.WriteLine(note_user_id?.ToString() ?? "note_user_id is null");
        Console.WriteLine(note_body?.ToString() ?? "note_body is null");
        return null;
      }
      return new NoteElement(
        parent_id, note_id, note_user_id, display_id, null, note_body, index
      );
    }

    private List<NoteElement> parseArrayNotes(JArray helpdeskNoteToken, string parent_id, string display_id, long index = 1)
    {
      List<NoteElement> ns = new List<NoteElement>();
      long i = index;
      foreach (var noteToken in helpdeskNoteToken)
      {
        var note_id = noteToken["id"]?["#text"]?.ToString();
        var note_user_id = noteToken["user-id"]?["#text"]?.ToString();
        var note_body = noteToken["body"]?.ToString();

        if (note_id == null || note_user_id == null || note_body == null)
        {
          Console.WriteLine("\n THERE IS A PROBLEM MISSING PROPERTIES IN NOTES \n");
          Console.WriteLine(note_id?.ToString() ?? "note_id is null");
          Console.WriteLine(note_user_id?.ToString() ?? "note_user_id is null");
          Console.WriteLine(note_body?.ToString() ?? "note_body is null");
          continue;
        }
        ns.Add(new NoteElement(parent_id, note_id, note_user_id, display_id, null, note_body, i));
        i++;
      }
      return ns;
    }

    private string GetCCEmail(JToken? ccmailToken, string? toemail)
    {
      List<string> finalResult = new List<string>();
      Dictionary<string, JToken>? ccmail = ccmailToken?.ToObject<Dictionary<string, JToken>>();

      try
      {
        if (!string.IsNullOrEmpty(toemail))
        {
          finalResult.Add(toemail);
        }

        if (ccmail == null)
        {
          return string.Join(",", finalResult); ;
        }

        foreach (string confemail in this.confemails)
        {
          string[] split = confemail.Split("::");
          string parent = split[0];
          string child = split[1];
          if (ccmail == null) continue;
          if (!ccmail.ContainsKey(parent)) continue;
          JToken? childToken = ccmail?[parent]?[child];
          if (childToken == null) continue;

          if (childToken.Type == JTokenType.Array)
          {
            List<string>? ccEmailArray = childToken?.ToObject<List<string>>();
            if (ccEmailArray == null)
            {
              continue;
            }
            ccEmailArray.ForEach(email => finalResult.Add($"{parent} -> {email}"));
          }
          else
          {
            finalResult.Add($"{parent} -> {childToken}");
          }
        }

        return string.Join(",", finalResult);
      }
      catch (System.Exception err)
      {
        Console.WriteLine(err.ToString(), ccmail);
        return string.Empty;
      }
    }

  }




}