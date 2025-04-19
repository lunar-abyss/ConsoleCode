# ConsoleCode
Console Code Beta 0.1 by Lunaryss, 2025
Public Domain

Console Code (aka. concode) is a vim-like code editor made in C# (.NET Framework)
made for using in small projects. Remember that the project is in early development
now, so a lot can change in future.

## Features
 * Simple highlighting language, literally based on regex
 * Small size (<20KB) (useful for small sized toolkits)
 * Mass of hotkeys to make your programming experience better
 * Pass a filepath via cmd to open it immediately

## How to install
1. Download the repository from GitHub (or only the `/bin/Release` folder).
2. Open `ConsoleCode.exe`. 
3. That's all!

## Hotkeys
* Functional:
  * ~Ctrl + N~: New file
  * ~Ctrl + O~: Open file
  * ~Ctrl + S~: Save file
  * ~Ctrl + H~: Open help file
  * ~Ctrl + J~: Open config file
  * ~Ctrl + R~: Reload config (may restart)
  
* Editing:
  * ~Backspace~: Remove previous character
  * ~Tab~: Tabulation (2 spaces)
  * ~Ctrl + Left~: Long jump left (skips same kind characters)
  * ~Ctrl + Right~: Long jump right
  * ~Ctrl + Up~: Skip up until first empty (or with spaces) line
  * ~Ctrl + Down~: Skip down
  * ~Ctrl + Backspace~: Long remove
  * ~Ctrl + Shift + Up~: Swap current line with the upper one
  * ~Ctrl + Shift + Down~: Swap current line with the lower one
  * ~Ctrl + C~: Copy current line
  * ~Ctrl + V~: Paste line after current
  
* Other:
  * ~Escape~: Quit the editor
  * ~Ctrl + W~: Switch window mode (Windowed -> Maximized (default) -> Fullscreen)

* Config:
  * `extbind`: binding the highlighting files (`.ccs`) with extensions for auto open
  * `font-name`: font name
  * `font-size`: font size
  * `font-weight`: font weight
  * `design-color`: color of the editor

## Higlighting
 * Types:
  * Regex: Regex expression to match the string (all matches inside of it are removed)
  * Color: Predefined constant (see Colors)
* Comments:
  * Inline: starts with `#` and ends with a new line (in patterns `#` must be escaped)
  * Word: starts with `@` and ends with a space (in patterns `@` must be escaped)
* Colors:
  * `$blue`, `$green`, `$cyan`, `$red`, `$magenta`, `$yellow`, `$gray`
  * Their dark variants (like `$dark-gray`)
  * `$black`, `$white`
* Template
  * `[word-comment] <color>: <regex>`
  * Example: `@comment $dark-gray: "\#.*(?=\n|\$)"`