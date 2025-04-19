// importing libs
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// my project
namespace ConsoleCode
{
  // CONSY Library for console
  class Consy
  {
    // CONSY::WINDOW
    #region section CONSY::WINDOW

    // CONSY::WINDOW
    //   Module used to control window's behavior
    //
    //   TODO:
    //   - fix the shit happening with windowed mode
    //     when it's combined with resizing and fonts
    //   - reimplement the resize function


    // PUBLIC

    // windowed console
    public static void Restore() {
      SetFullscreen(false);
      Native.ShowWindow(Native.GetConsoleWindow(), 10);
    }

    // maximize the console
    public static void Maximize() {
      SetFullscreen(false);
      Native.ShowWindow(Native.WindowHandle, 3);
    }

    // fullscreen the console
    public static void Fullscreen() =>
      SetFullscreen(true);

    // set size of the console
    public static void Resize(int width, int height)
    {
      // making it ok
      width = Math.Min(width, Console.LargestWindowWidth);
      height = Math.Min(height, Console.LargestWindowHeight);

      // in case it's becoming bigger
      if (width > Console.WindowWidth)
        Console.SetBufferSize(width, height);

      // setting the window size
      Console.SetWindowSize(width, height);

      // in case it's becoming smaller
      if (width <= Console.WindowWidth)
        Console.SetBufferSize(width, height);
    }


    // PRIVATE
    // private version of fullscreen function
    private static void SetFullscreen(bool active) =>
      Native.SetConsoleDisplayMode(
        Native.OutputHandle, active ? 1u : 2u, out _);

    // end of the section
    #endregion

    // CONSY::OTHER
    #region section CONSY::OTHER

    // CONSY::OTHER
    //   Module that contains functions that have
    //   no their own modules yet
    //
    //   TODO:
    //   - Restart() better code?
    //   - FileDialog() more parameters and stuff
    //   - WriteColoredBuffer() better code

    // restart application
    public static void Restart() {
      Process.Start(
        Process.GetCurrentProcess()
          .MainModule.FileName);
      Environment.Exit(0);
    }

    // [redo] show file dialog from console application
    public static string FileDialog(string title)
    {
      // initializing the dialog
      var ofn = new Native.OpenFileName();

      // setting up it
      ofn.lStructSize = Marshal.SizeOf(ofn);
      ofn.lpstrFile = new string(new char[256]);
      ofn.nMaxFile = ofn.lpstrFile.Length;
      ofn.lpstrFileTitle = new string(new char[64]);
      ofn.nMaxFileTitle = ofn.lpstrFileTitle.Length;
      ofn.lpstrTitle = title;

      // getting the result
      if (Native.GetOpenFileName(ref ofn))
        return ofn.lpstrFile;
      return null;
    }
  
    // [redo] writing a colored buffer
    public static void WriteColoredBuffer(string buffer, int x = 0, int y = 0, int width = -1, int height = -1)
    {
      // default parameters
      width  = Math.Max(width,  Console.WindowWidth);
      height = Math.Max(height, Console.WindowHeight);

      // initializing the array
      var ci = new Native.CHAR_INFO[width * height];
      int cx = 0, cy = 0, attr = 8;

      // iterating through string
      for (int i = 0; i < buffer.Length; i++)
      {
        // special sequence
        if (buffer[i] == '$')
        {
          // getting the command
          char f = buffer[++i];

          // another $
          if (f == '$') {
            ci[cy * width + cx++] = new Native.CHAR_INFO {
              charData = buffer[i],
              attributes = (short)attr,
            };
            continue;
          }

          // underline
          if (f == 'u' || f == 'U') {
            if (f == 'u')
              attr |= 0x8000;
            else if (f == 'U')
              attr &= ~0x8000;
            continue;
          }

          // zerofier
          if (f == '0') {
            attr = 0;
            continue;
          }

          // getting the color  
          int c =
            Convert.ToInt32(
              buffer[++i].ToString(), 16);

          // foreground and background
          if (f == 'f')
            attr = (attr & ~0x0f) | c;
          else if (f == 'b')
            attr = (attr & ~0xf0) | c << 4;

          // next
          continue;
        }

        // newline char
        else if (buffer[i] == '\n') {
          cx = 0;
          cy++;
          continue;
        }

        // x out of range
        if (cx >= width)
          continue;

        // setting the character
        ci[cy * width + cx++] = new Native.CHAR_INFO { 
          charData = buffer[i],
          attributes = (short)attr,
        };

        // y out of range
        if (cy >= height)
          break;
      }

      // useless variable
      Native.SMALL_RECT r = new Native.SMALL_RECT {
        Left   = (short)x,
        Top    = (short)y,
        Right  = (short)(x + width),
        Bottom = (short)(y + height),
      };
      
      // write to the console
      Native.WriteConsoleOutput(
        Native.OutputHandle,
        ci,
        new Native.COORD((short)width, (short)height),
        new Native.COORD((short)x, (short)y),
        ref r);
    }

    // set current font
    public static void SetFont(string fontName, int fontSize, int fontWeight)
    {
      // defining the properties
      Native.FontInfo nfi = new Native.FontInfo {
        cbSize = Marshal.SizeOf<Native.FontInfo>(),
        FontIndex = 0,
        FontFamily = 54,
        FontName = fontName,
        FontWeight = fontWeight,
        FontSize = (short)fontSize,
      };

      // setting the actual font size
      Native.SetCurrentConsoleFontEx(
        Native.OutputHandle,
        false, ref nfi);
    }

    #endregion
  }
}