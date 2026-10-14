using System;
using System.Collections.Generic;
using System.Linq;

namespace Rufa.Editor
{
    /// <summary>One page of a lesson tutorial.</summary>
    public sealed class LessonPage
    {
        public string chapter = "", title = "", body = "";
        public bool alone;                              // a page of a "# Da soli ..." chapter: shown with its chapter as one list of tasks
        public string highlightWindow, highlightLabel;  // "> evidenzia: Inspector | Move Speed"
        public string highlightObject;                  // "> evidenzia: Inspector | Use Gravity | Gravity": the object the field must be on, when others have a field with the same label
        public string check;                            // "> verifica: id arg | arg": an entry of LessonChecks; null = nothing to do
        public string[] checkArgs = Array.Empty<string>();
        public string action, actionLabel;              // "> azione: Button text | id | arg": a button that runs an entry of LessonChecks
        public string[] actionArgs = Array.Empty<string>();
    }

    /// <summary>
    /// Reads a lesson tutorial from its markdown file. "# " starts a chapter ("# Da soli ..." = a list of tasks to do alone, in any order;
    /// a task titled "Extra..." is a bonus, out of the score), "## " a page, plain lines are the page text. Lines starting with "> " are directives: evidenzia, verifica and azione drive the
    /// editor; attesa and lavoro feed only the time estimate (tools/stima-tempi.py). Lines before the first page are
    /// notes for whoever edits the file.
    /// </summary>
    public static class Lesson
    {
        public static List<LessonPage> Parse(string markdown)
        {
            var pages = new List<LessonPage>();
            string chapter = "";
            LessonPage page = null;
            foreach (var raw in markdown.Replace("\r", "").Split('\n'))
            {
                var line = raw.TrimEnd();
                if (line.StartsWith("# ")) { chapter = line.Substring(2); page = null; }
                else if (line.StartsWith("## ")) pages.Add(page = new LessonPage { chapter = chapter, title = line.Substring(3), alone = chapter.StartsWith("Da soli") });
                else if (page == null || line.StartsWith("> attesa:") || line.StartsWith("> lavoro:")) continue;
                else if (line.StartsWith("> evidenzia:"))
                {
                    var p = Parts(Value(line));
                    page.highlightWindow = p[0];
                    page.highlightLabel = p.Length > 1 ? p[1] : "";
                    page.highlightObject = p.Length > 2 ? p[2] : null;
                }
                else if (line.StartsWith("> azione:"))
                {
                    var p = Parts(Value(line));
                    page.actionLabel = p[0];
                    page.action = p.Length > 1 ? p[1] : null;
                    page.actionArgs = p.Skip(2).ToArray();
                }
                else if (line.StartsWith("> verifica:"))
                {
                    var v = Value(line);
                    int space = v.IndexOf(' ');
                    page.check = space < 0 ? v : v.Substring(0, space);
                    page.checkArgs = space < 0 ? Array.Empty<string>() : Parts(v.Substring(space + 1));
                }
                else page.body += line + "\n";
            }
            foreach (var p in pages) p.body = p.body.Trim();
            return pages;
        }

        static string Value(string line) => line.Substring(line.IndexOf(':') + 1).Trim();
        static string[] Parts(string text) => Array.ConvertAll(text.Split('|'), s => s.Trim());
    }
}
