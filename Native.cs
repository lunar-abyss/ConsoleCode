using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static ConsoleCode.Native;

namespace ConsoleCode
{
  class Native
  {
    // VARIABLES
    // output handle
    public static readonly IntPtr
      OutputHandle = GetStdHandle(-11);

    // input handle
    public static readonly IntPtr
      InputHandle = GetStdHandle(-10);

    // console handle
    public static readonly
      IntPtr WindowHandle = GetConsoleWindow();


    // FUNCTIONS
    // GetStdHandle
    [DllImport("kernel32.dll")]
      public static extern 
        IntPtr GetStdHandle(int nStdHandle);

    // GetConsoleWindow
    [DllImport("kernel32.dll", ExactSpelling = true)]
      public static extern
        IntPtr GetConsoleWindow();


    // AllocConsole
    //[DllImport("kernel32")]
    //  public static extern bool
    //    AllocConsole();

    // FreeConsole
    //[DllImport("kernel32.dll")]
    //  public static extern bool
    //    FreeConsole();

    // AttachConsole
    //[DllImport("kernel32.dll", SetLastError = true)]
    //  public static extern bool
    //    AttachConsole(uint dwProcessId);


    // SetCurrentConsoleFontEx
    [return: MarshalAs(UnmanagedType.Bool)]
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern
          bool SetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool MaximumWindow, ref FontInfo ConsoleCurrentFontEx);

    // GetCurrentConsoleFontEx
    //[return: MarshalAs(UnmanagedType.Bool)]
    //  [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    //    public static extern
    //      bool GetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool MaximumWindow, ref FontInfo ConsoleCurrentFontEx);

    // GetConsoleMode
    //[DllImport("kernel32.dll", SetLastError = true)]
    //  public static extern
    //    bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    // GetConsoleMode
    //[DllImport("kernel32.dll", SetLastError = true)]
    //  public static extern
    //    bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);


    // ShowWindow
    [DllImport("user32.dll")]
      public static extern
        bool ShowWindow(IntPtr hWnd, int cmdShow);

    // SetConsoleDisplayMode
    [DllImport("kernel32.dll", SetLastError = true)]
      public static extern
        bool SetConsoleDisplayMode(IntPtr ConsoleOutput, uint Flags, out COORD NewScreenBufferDimensions);


    // GetConsoleScreenBufferInfoEx
    //[DllImport("kernel32.dll", SetLastError = true)]
    //  public static extern bool
    //    GetConsoleScreenBufferInfoEx(IntPtr hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX lpConsoleScreenBufferInfoEx);


    // GetOpenFileName
    [DllImport("comdlg32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern
        bool GetOpenFileName(ref OpenFileName ofn);


    // WriteConsoleOutputAttribute
    [DllImport("kernel32.dll", SetLastError = true)]
      public static extern
        bool WriteConsoleOutput(IntPtr hConsoleOutput, CHAR_INFO[] lpBuffer, COORD dwBufferSize, COORD dwBufferCoord, ref SMALL_RECT lpWriteRegion);

    
    // ReadConsoleInput
    //[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    //  public static extern
    //    bool ReadConsoleInput(IntPtr hConsoleInput, out INPUT_RECORD lpBuffer, uint nLength, out uint lpNumberOfEventsRead);

    // STRUCTS

    // font information structure
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
      public struct FontInfo {
        public int cbSize;
        public int FontIndex;
        public short FontWidth;
        public short FontSize;
        public int FontFamily;
        public int FontWeight;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string FontName;
      }

    // coordinates
    [StructLayout(LayoutKind.Sequential)]
      public struct COORD {
        public short X;
        public short Y;
        public COORD(short x, short y) 
          { X = x; Y = y; }
      }

    // file dialog
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
      public struct OpenFileName {
        public int lStructSize;
        public IntPtr hwndOwner;
        public IntPtr hInstance;
        public string lpstrFilter;
        public string lpstrCustomFilter;
        public int nMaxCustFilter;
        public int nFilterIndex;
        public string lpstrFile;
        public int nMaxFile;
        public string lpstrFileTitle;
        public int nMaxFileTitle;
        public string lpstrInitialDir;
        public string lpstrTitle;
        public int Flags;
        public short nFileOffset;
        public short nFileExtension;
        public string lpstrDefExt;
        public IntPtr lCustData;
        public IntPtr lpfnHook;
        public string lpTemplateName;
        public IntPtr pvReserved;
        public int dwReserved;
        public int flagsEx;
      }

    // information about character in console
    [StructLayout(LayoutKind.Sequential)]
      public struct CHAR_INFO {
        public ushort charData;
        public short attributes;
      }

    // information about character in console
    [StructLayout(LayoutKind.Sequential)]
      public struct SMALL_RECT {
        public short Left;
        public short Top;
        public short Right;
        public short Bottom;
      }
    
    // color
    //[StructLayout(LayoutKind.Sequential)]
    //  public struct COLORREF {
    //    public byte R;
    //    public byte G;
    //    public byte B;
    //  }

    // console buffer info
    //[StructLayout(LayoutKind.Sequential)]
    //  public struct CONSOLE_SCREEN_BUFFER_INFO_EX {
    //    public uint cbSize;
    //    public COORD dwSize;
    //    public COORD dwCursorPosition;
    //    public ushort wAttributes;
    //    public SMALL_RECT srWindow;
    //    public COORD dwMaximumWindowSize;
    //    public ushort wPopupAttributes;
    //    public bool bFullscreenSupported;
    //    public COLORREF ColorTable;
    //  }

    // idk really
    //[StructLayout(LayoutKind.Sequential)]
    //  public struct INPUT_RECORD {
    //    public short EventType;
    //    public KEY_EVENT_RECORD KeyEvent;
    //  }

    // key event data
    //[StructLayout(LayoutKind.Sequential)]
    //  public struct KEY_EVENT_RECORD {
    //    public bool KeyDown;
    //    public ushort RepeatCount;
    //    public ushort VirtualKeyCode;
    //    public ushort VirtualScanCode;
    //    public char UnicodeChar;
    //    public byte AsciiChar;
    //    public uint ControlKeyState;
    //  }
  }
}
