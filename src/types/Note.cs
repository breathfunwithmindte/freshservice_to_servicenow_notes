/// <summary>
/// 
/// </summary>
/// 
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using freshservice_to_servicenow_notes.src;

namespace freshservice_to_servicenow_notes.src.types
{

  class Note : ParserItem<NoteElement>
  {

    private string[] confemails = { "cc-emails::cc-email", "fwd-emails::fwd-email", "reply-cc::reply-cc", "tkt-cc::tkt-cc", "tkt-bcc::tkt-cc" };

    public void Run(JObject obj, List<NoteElement> notes)
    {
      // Console.WriteLine(obj["helpdesk-ticket"]?["helpdesk-notes"]?["helpdesk-note"]);

      var parent_id = obj["helpdesk-ticket"]?["id"]?["#text"]?.ToString();
      var parent_user_id = obj["helpdesk-ticket"]?["requester-id"]?["#text"]?.ToString();
      var parent_body = obj["helpdesk-ticket"]?["description"]?.ToString();
      var display_id = obj["helpdesk-ticket"]?["display-id"]?["#text"]?.ToString();
      JToken? helpdeskNoteToken = obj["helpdesk-ticket"]?["helpdesk-notes"]?["helpdesk-note"];

      if (helpdeskNoteToken is JArray helpdeskNotes)
      {
          foreach (var helpdeskNote in helpdeskNotes)
          {
              // Access helpdeskNote properties here
              Console.WriteLine("msaidjiasjdisjad");
              Console.WriteLine(helpdeskNote);
          }
      }
      else
      {
          // Handle case where helpdesk-note is not an array
      }

      string mails = this.GetCCEmail(obj["helpdesk-ticket"]?["cc-email"], obj["helpdesk-ticket"]?["to-email"]?.ToString());

      if (parent_id == null || parent_user_id == null || parent_body == null || display_id == null)
      {
        Console.WriteLine("\n THERE IS A PROBLEM MISSING PROPERTIES \n");
        Console.WriteLine(parent_body?.ToString() ?? "parent_body is null");
        Console.WriteLine(parent_user_id?.ToString() ?? "parent_user_id is null");
        Console.WriteLine(display_id?.ToString() ?? "display_id is null");
        Console.WriteLine(parent_id?.ToString() ?? "parent_id is null");
        return;
      }

      NoteElement primaryNote = new NoteElement(parent_id, null, parent_user_id, display_id, mails, parent_body, 0);

      notes.Add(primaryNote);

      if (helpdeskNoteToken == null)
      {
        Console.WriteLine("notes are missing");
        return;
      }

      // if(helpdeskNoteToken != null ) return;

      if (helpdeskNoteToken is JArray helpdeskNoteList)
      {
        int i = 1;
        foreach (var noteToken in helpdeskNoteList)
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
          notes.Add(new NoteElement(
              parent_id, note_id, note_user_id, display_id, null, note_body, i
          ));
          i++;
        }
      }
      else if (helpdeskNoteToken is JObject helpdeskNoteObject)
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
          return;
        }
        notes.Add(new NoteElement(
          parent_id, note_id, note_user_id, display_id, null, note_body, 1
        ));
      }
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

            if(ccmail == null) 
            {
              return string.Join(",", finalResult);;
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
                    if(ccEmailArray == null) 
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