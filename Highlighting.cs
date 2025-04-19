// libsss
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

// yooo it's cool
namespace ConsoleCode
{
  // [totally redo] class to highlight the text
  internal class Highlighting
  {
    // parsed highlighting program
    private static List<string[]> Parsed = new List<string[]>();

    // constants of colors
    private static Dictionary<string, string> Colors =
      new Dictionary<string, string> {
          ["$black"]     = "0", ["$dark-blue"]    = "1", ["$dark-green"]  = "2", ["$dark-cyan"] = "3",
          ["$dark-red"]  = "4", ["$dark-magenta"] = "5", ["$dark-yellow"] = "6", ["$gray"]      = "7",
          ["$dark-gray"] = "8", ["$blue"]         = "9", ["$green"]       = "a", ["$cyan"]      = "b",
          ["$red"]       = "c", ["$magenta"]      = "d", ["$yellow"]      = "e", ["$white"]     = "f",
      };
    
    // [redo] load script
    public static void LoadScript(string path)
    {
      // clear parsed list
      Parsed.Clear();

      // when no highlighting
      if (path == null)
        return;

      // reading the file
      string conts = File.ReadAllText(
        Program.ConcodePath + "\\" + path);

      // removing the comments
      conts = Regex.Replace(conts, @"(?<!\\)@[^\s]*", "");
      conts = Regex.Replace(conts, @"(?<!\\)#.*(?=(\r?\n)|$)", "");

      // two arrays
      string[] lines = Regex.Split(conts, @"\r?\n").Where(i => i != "").ToArray();

      // parsing with regex
      foreach (string line in lines) {
        Match m = Regex.Match(line, @"([^\s:]+) *: *(.*)");
        Parsed.Add(new string[2] { Colors[m.Groups[1].Value], m.Groups[2].Value.Substring(1, m.Groups[2].Value.Length - 2) });
      }
    }

    // [redo] the highlighter function
    public static string Apply(string text)
    {
      // save dollars
      text = text.Replace("$", "\0");

      // some variables
      for (int i = 0; i < Parsed.Count; i++)
        text = Regex.Replace(
          text,
          Parsed[i][1].Replace("\\$", "\\0"),
          match => "$f" + Parsed[i][0] + 
            Regex.Replace(match.Value,
              @"((?<!\$)\$(?!\$)..)", "") + "$f7");

      // return the result
      return text.Replace("\0", "$$");
    }
  }
}