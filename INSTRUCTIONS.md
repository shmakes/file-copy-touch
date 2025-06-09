# File Copy Touch - Setup and Usage Instructions

## Prerequisites

- Windows 10/11
- .NET 6.0 Runtime or later
- Visual Studio 2022 or .NET 6.0 SDK

## Building the Application

### Option 1: Using Visual Studio
1. Open `FileCopyTouch.csproj` in Visual Studio 2022
2. Build the solution (Build → Build Solution or Ctrl+Shift+B)
3. Run the application (Debug → Start Debugging or F5)

### Option 2: Using Command Line
1. Open PowerShell or Command Prompt in the project directory
2. Run: `dotnet build`
3. Run: `dotnet run`

### Option 3: Creating a Standalone Executable
1. Run: `dotnet publish -c Release -r win-x64 --self-contained`
2. The executable will be in `bin\Release\net6.0-windows\win-x64\publish\`

## Configuration

The application uses a `config.json` file that must be in the same directory as the executable. If it doesn't exist, the application will create a default one.

### Configuration Parameters

```json
{
  "ApplicationTitle": "File Copy Touch",
  "MinimumInputLength": 5,
  "DebounceMilliseconds": 500,
  "SourceDirectory": "C:\\SourceFiles",
  "TargetDirectory": "C:\\TargetFiles"
}
```

- **ApplicationTitle**: The title text displayed in the application header (default: "File Copy Touch")
- **MinimumInputLength**: Minimum number of characters needed before search begins (5-20, default: 5)
- **DebounceMilliseconds**: Wait time after typing stops before search starts (100-3000ms, default: 500)
- **SourceDirectory**: Path to the directory containing files to search and copy from
- **TargetDirectory**: Path to the directory where files will be copied to

### Directory Path Examples

The application supports various path formats:

- **Local paths**: `C:\\MyFiles` or `D:\\Documents`
- **Mapped drives**: `Z:\\SharedFolder`
- **UNC paths**: `\\\\ServerName\\ShareName\\Folder`

## Usage

1. **Launch the application** - It will start in fullscreen mode for touch-screen use

2. **Configure directories** - Edit the `config.json` file to point to your source and target directories

3. **Enter search text** - Type in the text box or use a barcode scanner
   - The search will start automatically after you finish typing (debounce delay)
   - You must enter at least the minimum number of characters (default: 5)

4. **Select a file** - Touch/click on a file from the search results
   - The application will ask for confirmation before copying
   - It will clear the target directory first, then copy the selected file

5. **Clear results** - Use the "Clear" button to reset the search and start over

## Features

- **Touch-friendly interface** with large buttons and text
- **Debounced search** to avoid searching while typing
- **Case-insensitive file matching**
- **Automatic target directory clearing** before copying
- **Confirmation dialogs** for all destructive operations
- **Real-time status updates** during operations
- **Configuration validation** with helpful error messages

## Safety Features

- **Confirmation required** before copying files
- **Directory validation** on startup
- **Error handling** with user-friendly messages
- **Target directory clearing** is clearly communicated to the user

## Troubleshooting

### Application won't start
- Ensure .NET 6.0 Runtime is installed
- Check that all files are in the same directory

### Directory errors
- Verify the paths in `config.json` exist and are accessible
- Ensure the application has read/write permissions to both directories
- For network paths, ensure the network location is accessible

### Search not working
- Check that the source directory contains files
- Verify the minimum input length setting
- Ensure file names contain the search text

### Copy operations failing
- Verify write permissions to the target directory
- Ensure sufficient disk space
- Check that files aren't locked by other applications

## Customization

You can modify the `config.json` file while the application is running. Restart the application to apply changes.

For UI customization, modify the XAML files and rebuild the application. 