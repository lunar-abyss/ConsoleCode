// importing libs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Linq.Expressions;

// nice application
namespace ConsoleCode
{
  // [redo] main class
  internal class Program
  {
    // TODO:
    // - special input processor to allow alt + arrow key
    // - rewrite the shitty input function
    // - it's a lot to do...


    // CONSTANTS
    const string HelpFileName   = "\\help.cct";
    const string ConfigFileName = "\\config.ccd";


    // VARIABLES
    // path to the executable and concode
    public static string Selfpath = "";
    public static string ConcodePath;

    // current file info
    static string       Filepath = null;
    static List<string> Contents = new List<string>() { "" };

    // all properties of the editor
    static class Settings
    {
      // design default constants
      public const string DefaultDesignColor = "6";

      // font settings
      public static int    FontSize   = 25;
      public static int    FontWeight = 400;
      public static string FontName   = "System";

      // design settings
      public static string DesignColor = DefaultDesignColor;
      
      // extensions settings
      public static Dictionary<string, string> Extbinds =
        new Dictionary<string, string>();
    }

    // editing settings
    static int    CursorX;
    static int    CursorY;
    static int    Scroll;
    static string Clipboard = "";

    // window mode
    static int WindowMode = 1;

    // FUNCTIONS
    // main function
    static void Main(string[] args)
    {
      // setting up the program
      ParseArgs(args);
      Paths();
      Config(true);
      Init();

      // main loop
      while (true) {
        Render();
        Input();
      }
    }

    // parse arguments passed to concode
    static void ParseArgs(string[] args)
    {
      // no args passed
      if (args.Length == 0)
        return;

      // first args is always path
      OpenFile(args[0]);
    }

    // functions to correct paths
    static void Paths()
    {
      // set the self path
      Selfpath = Path.GetDirectoryName(
        Process.GetCurrentProcess()
          .MainModule.FileName);

      // check if concode path exists
      if (Directory.Exists(Selfpath + "\\concode"))
        ConcodePath = Selfpath + "\\concode";
      else
        ConcodePath = Selfpath;
    }

    // fully initializes the console
    static void Init()
    {
      // setting up the cursor
      Consy.SetFont(
        Settings.FontName,
        Settings.FontSize,
        Settings.FontWeight);
      
      // set up the window
      Console.Title = "ConsoleCode";
      Consy.Resize(
        Console.LargestWindowWidth * 2 / 3,
        Console.LargestWindowHeight * 2 / 3);
      Consy.Maximize();

      // other
      Console.CursorSize = 100;
      Console.TreatControlCAsInput = true;
    }

    // [I HOPE CODE GOD WILL FORGIVE ME] processes the input
    static void Input()
    {
      // get the info
      ConsoleKeyInfo cki =
        Console.ReadKey(true);

      // if only with control
      if (cki.Modifiers == ConsoleModifiers.Control)
      {
        // switching window modes
        if (cki.Key == ConsoleKey.W)
          SwitchWindowMode();

        // opening the file
        else if (cki.Key == ConsoleKey.O)
          OpenFile();

        // save the file
        else if (cki.Key == ConsoleKey.S)
          SaveFile();

        // open the help
        else if (cki.Key == ConsoleKey.H)
          OpenFile(ConcodePath + HelpFileName);

        // open configs
        else if (cki.Key == ConsoleKey.J)
          OpenFile(ConcodePath + ConfigFileName);

        // reapply config
        else if (cki.Key == ConsoleKey.R)
          Config();

        // copying the line
        else if (cki.Key == ConsoleKey.C)
          Clipboard = Contents[CursorY];

        // pasting the line
        else if (cki.Key == ConsoleKey.V)
          Contents.Insert(++CursorY, Clipboard);

        // new file creation
        else if (cki.Key == ConsoleKey.N)
          NewFile();

        // crtl + key left
        else if (cki.Key == ConsoleKey.LeftArrow) {
          if (CursorX > 0) {
            int charMask = CharMask(Contents[CursorY][CursorX - 1]);
            while (CursorX > 0 && CharMask(Contents[CursorY][CursorX - 1]) == charMask)
              CursorX--;
          }
          else if (CursorY != 0) {
            CursorY--;
            CursorX = Contents[CursorY].Length;
          }
        }

        // ctrl + key right
        else if (cki.Key == ConsoleKey.RightArrow) {
          if (CursorX < Contents[CursorY].Length) {
            int charMask = CharMask(Contents[CursorY][CursorX]);
            while (CursorX < Contents[CursorY].Length && CharMask(Contents[CursorY][CursorX]) == charMask)
              CursorX++;
          }
          else if (CursorY < Contents.Count) {
            CursorY++;
            CursorX = 0;
          }
        }

        // ctrl + up
        else if (cki.Key == ConsoleKey.UpArrow) {
          if (CursorY > 0) {
            CursorY--;
            while (CursorY > 0 && Contents[CursorY].Trim() != "")
              CursorY--;
            CursorX = Contents[CursorY].Length;
          }
        }

        // ctrl + down
        else if (cki.Key == ConsoleKey.DownArrow) {
          if (CursorY < Contents.Count - 1) {
            CursorY++;
            while (CursorY < Contents.Count - 1 && Contents[CursorY].Trim() != "")
              CursorY++;
            CursorX = Contents[CursorY].Length;
          }
        }

        // ctrl + backspace
        else if (cki.Key == ConsoleKey.Backspace)
        {
          // removing newline
          if (CursorX == 0 && CursorY != 0) {
            int len = Contents[CursorY - 1].Length;
            Contents[CursorY - 1] += Contents[CursorY];
            Contents.RemoveAt(CursorY);
            CursorY--;
            CursorX = len;
          }

          // removing a usual character
          else if (!(CursorX == 0 && CursorY == 0)) {
            int charMask = CharMask(Contents[CursorY][CursorX - 1]);
            while (CursorX > 0 && CharMask(Contents[CursorY][CursorX - 1]) == charMask)
              Contents[CursorY] =
                Contents[CursorY]
                  .Substring(0, CursorX - 1) +
                Contents[CursorY]
                  .Substring(CursorX--);
          }
        }

      }

      // ctrl + shift + key controls
      else if (cki.Modifiers == (ConsoleModifiers.Control | ConsoleModifiers.Shift))
      {
        // moving the line up
        if (cki.Key == ConsoleKey.UpArrow) {
          if (CursorY != 0) {
            Contents.Insert(CursorY - 1, Contents[CursorY]);
            Contents.RemoveAt(CursorY-- + 1);
          }
        }

        // moving the line down
        else if (cki.Key == ConsoleKey.DownArrow) {
          if (CursorY != Contents.Count - 1) {
            Contents.Insert(CursorY + 2, Contents[CursorY]);
            Contents.RemoveAt(CursorY++);
          }
        }
      }

      // entring usual characters
      else
      {
        // if left key
        if (cki.Key == ConsoleKey.LeftArrow) {
          CursorX--;
          if (CursorX < 0) {
            if (CursorY != 0) {
              CursorY--;
              CursorX = Contents[CursorY].Length;
            }
            else
              CursorX = 0;
          }
        }

        // right key
        else if (cki.Key == ConsoleKey.RightArrow) {
          CursorX++;
          if (CursorX > Contents[CursorY].Length) {
            if (CursorY != Contents.Count - 1) {
              CursorY++;
              CursorX = 0;
            }
            else
              CursorX = Contents[CursorY].Length;
          }
        }

        // up and down keys
        else if (cki.Key == ConsoleKey.UpArrow)
          CursorY--;
        else if (cki.Key == ConsoleKey.DownArrow)
          CursorY++;

        // escape to exit
        else if (cki.Key == ConsoleKey.Escape)
          Environment.Exit(0);

        // tab
        else if (cki.Key == ConsoleKey.Tab) {
          Contents[CursorY] =
            Contents[CursorY].Substring(0, CursorX) +
            (CursorX % 2 == 0 ? "  " : " ") +
            Contents[CursorY].Substring(CursorX);
          CursorX += CursorX % 2 == 0 ? 2 : 1;
        }

        // entering new line
        else if (cki.Key == ConsoleKey.Enter)
        {
          // collecting data
          string line = Contents[CursorY];
          int padding = line.Length - line.TrimStart().Length;

          // creating new line
          string newLine =
            new string(' ', padding) +
            line.Substring(CursorX);

          // setting the current line
          Contents[CursorY] = line.Substring(0, CursorX);
          Contents.Insert(CursorY + 1, newLine);

          // setting the position of the cursor
          CursorX = padding;
          CursorY++;
        }

        // removing the character before
        else if (cki.Key == ConsoleKey.Backspace)
        {
          // removing newline
          if (CursorX == 0 && CursorY != 0) {
            int len = Contents[CursorY - 1].Length;
            Contents[CursorY - 1] += Contents[CursorY];
            Contents.RemoveAt(CursorY);
            CursorY--;
            CursorX = len;
          }

          // removing a usual character
          else if (!(CursorX == 0 && CursorY == 0))
            Contents[CursorY] =
              Contents[CursorY]
                .Substring(0, CursorX - 1) +
              Contents[CursorY]
                .Substring(CursorX--);
        }

        // insert charater
        else if (cki.KeyChar > 31 && cki.KeyChar != 127) {
          string line = Contents[CursorY];
          Contents[CursorY] =
            line.Substring(0, CursorX) +
            cki.KeyChar +
            line.Substring(CursorX);
          CursorX++;
        }
      }

      // safe the cursor
      SafeCursor();
    }

    // [redo] rendering the screen
    static void Render()
    {
      // initializing the render buffer
      string[] buffer = new string[Console.WindowHeight];

      // join contents
      //string joined = string.Join("\n", Contents);
      string joined = string.Join("\n",
        Contents.GetRange(
          Scroll,
          Math.Min(
            Console.WindowHeight - 2,
            Contents.Count - Scroll)));

      // top bar
      buffer[0] = 
        $"$b{Settings.DesignColor}$f0" + 
        (Filepath == null
          ? "No File Open :: Ctrl + H: Help, Escape: Quit" 
          : "ConsoleCode :: " + Filepath) +
        new string(' ', Console.WindowWidth) +
        "$b0$f0";

      // bottom bar
      buffer[Console.WindowHeight - 1] =
        $"$b{Settings.DesignColor}$f0" +
          "Line: " + (CursorY + 1).ToString().PadLeft(4, ' ') +
        ", Char: " + (CursorX + 1).ToString().PadLeft(4, ' ') +
        ", Size: " + joined.Length + " (" + (joined.Length / 1024) + "KB)" +
        new string(' ', Console.WindowWidth);

      // prepare the code
      string[] conts =
        Highlighting.Apply(joined)
        .Split('\n');

      // drawing the contents
      for (int i = 0; i < Console.WindowHeight - 2; i++)
      {
        // writing the line
        string currentLine =
          (Scroll + i + 1).ToString().PadLeft(4) + " ";

        // line in contents
        if (i < conts.Length)
          buffer[i + 1] =
            "$f7" + currentLine + conts[i];

        // no line
        else
          buffer[i + 1] =
            "$f8" + currentLine + ".";
      }

      // draw the buffer
      Consy.WriteColoredBuffer(
        string.Join("\n", buffer));

      // move the cursor to the required position
      Console.CursorLeft = Clamp(CursorX + 5, 5, Console.WindowWidth - 1);
      Console.CursorTop = Clamp(CursorY + 1 - Scroll, 1, Console.WindowHeight - 2);
    }

    // [redo] get and apply configuration settings
    static void Config(bool initialize = false)
    {
      // getting all settings
      var dict = ParseDataFile(ConcodePath + ConfigFileName);

      // setting console font size
      if (dict.TryGetValue("font-size", out string fontSize)) {
        if (initialize)
          Settings.FontSize = int.Parse(fontSize);
        else if (int.Parse(fontSize) != Settings.FontSize)
          Consy.Restart();
      }

      // setting console font name
      if (dict.TryGetValue("font-name", out string fontName)) {
        if (initialize)
          Settings.FontName = fontName;
        else if (fontName != Settings.FontName)
          Consy.Restart();
      }

      // setting console font weight
      if (dict.TryGetValue("font-weight", out string fontWeight)) {
        if (initialize)
          Settings.FontWeight = int.Parse(fontWeight);
        else if (int.Parse(fontWeight) != Settings.FontWeight)
          Consy.Restart();
      }

      // setting bar color
      if (dict.ContainsKey("design-color"))
        Settings.DesignColor = dict["design-color"];

      // setting the binds
      if (dict.ContainsKey("extbinds")) {
        Settings.Extbinds.Clear();
        string[] pairs = Regex.Split(dict["extbinds"], @"(?<=[^=]) +(?=[^=])");
        for (int i = 0; i < pairs.Length; i++)
          Settings.Extbinds.Add(
            pairs[i].Split('=')[0].Trim(),
            pairs[i].Split('=')[1].Trim());
      }
    }

    // [redo] moves safe cursor from getting out of bounds
    static void SafeCursor()
    {
      // moving up from top
      if (CursorY < 0) {
        CursorY = 0;
        CursorX = 0;
      }

      // moving down from bottom
      else if (CursorY >= Contents.Count) {
        CursorY = Contents.Count - 1;
        CursorX = Contents[CursorY].Length;
      }

      // can't go left
      if (CursorX < 0)
        CursorX = 0;

      // can't go right
      else if (CursorX > Contents[CursorY].Length)
        CursorX = Contents[CursorY].Length;

      // cursor is out of scroll
      int offset = Console.WindowHeight / 5;
      while (Scroll > 0 && CursorY < Scroll + offset)
        Scroll--;
      while (CursorY > Scroll + Console.WindowHeight - 3 - offset)
        Scroll++;
    }

    // [redo] switching window modes
    static void SwitchWindowMode()
    {
      // cycle through window mode
      WindowMode = (WindowMode + 1) % 3;

      // different window modes
      switch (WindowMode)
      {
        // windowed mode
        case 0:
          Consy.Restore();
          Consy.Resize(
            Console.LargestWindowWidth * 2 / 3,
            Console.LargestWindowHeight * 2 / 3);
          break;

        // maximized mode
        case 1:
          Consy.Maximize();
          break;

        // fullscreen mode
        case 2:
          Consy.Fullscreen();
          break;
      }

    }

    // [redo] clear data about current file
    static void NewFile() {
      Filepath = null;
      Contents = new List<string>() { "" };
    }

    // [redo] open file
    static void OpenFile(string file = "")
    {
      // time file path
      string filepath = Filepath;

      // open file dialog
      if (file == "")
        Filepath = Consy.FileDialog("Open File...");
      else
        Filepath = file;

      // in case of failure
      if (Filepath == null) {
        Filepath = filepath;
        return;
      }

      // getting the file
      Contents = Regex.Replace(
        File.ReadAllText(Filepath),
        @"(\r\n)|\r", "\n")
        .Replace("\t", "  ")
        .Split('\n')
        .ToList();

      // init pos of the cursor
      CursorX = 0;
      CursorY = 0;

      // extension
      string ext = Path.GetExtension(Filepath).Substring(1);

      // load highlighter, or use default one
      if (Settings.Extbinds.ContainsKey(ext))
        Highlighting.LoadScript(Settings.Extbinds[ext]);
      else
        Highlighting.LoadScript(null);
    }

    // [redo] save file
    static void SaveFile()
    {
      // redirect to saveas
      if (Filepath == null) {
        SaveAsFile();
        return;
      }

      // just save
      else
        File.WriteAllText(
          Filepath,
          string.Join("\n", Contents));
    }

    // [redo] save as file
    static void SaveAsFile()
    {
      // open file dialog
      Filepath = Consy.FileDialog("Save File...");

      // no path given
      if (Filepath == null)
        return;

      // writing the contents to the file
      File.WriteAllText(
        Filepath,
        string.Join("\n", Contents));
    }

    // [redo] parsing data files of console code
    static Dictionary<string, string> ParseDataFile(string filepath)
    {
      // getting file contents
      string contents =
         File.ReadAllText(filepath);

      // preprocessing
      contents = Regex.Replace(contents, @"#.*(?=\r?\n)", "");
      contents = Regex.Replace(contents, @":", " : ");
      contents = contents.Trim();

      // parse to tokens
      string[] tokens =
        Regex.Split(contents, @"\s+");

      // the dictionary to write properties in
      var dict = new Dictionary<string, string>();
      string last = "";

      // filling the dict
      for (int i = 0; i < tokens.Length; i++)
      {
        // the colon
        if (tokens[i + 1] == ":") {
          dict.Add(tokens[i], tokens[i + 2]);
          last = tokens[i];
          i += 2;
        }

        // not a colon
        else
          dict[last] += " " + tokens[i];
      }

      // result in dictionary
      return dict;
    }

    // UTILITIES
    // clamp utility
    static int Clamp(int value, int min, int max) =>
      Math.Min(Math.Max(value, min), max);

    // char mask
    static int CharMask(char value) =>
      (char.IsDigit(value)     ? 1 : 0) |
      (char.IsSeparator(value) ? 2 : 0) |
      (char.IsLetter(value)    ? 4 : 0) |
      (value == ' '            ? 8 : 0);

  }
}
