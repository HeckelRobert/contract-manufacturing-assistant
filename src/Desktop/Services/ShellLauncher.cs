namespace QuotationAccelerator.Desktop.Services;

using System.Diagnostics;
using System.IO;

public static class ShellLauncher
{
    public static bool TryOpenFolder(string folderPath, out string? errorMessage)
    {
        errorMessage = null;

        if (string.IsNullOrWhiteSpace(folderPath))
        {
            errorMessage = "Folder path is empty.";
            return false;
        }

        if (!Directory.Exists(folderPath))
        {
            errorMessage = $"Folder not found: {folderPath}";
            return false;
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = folderPath,
            UseShellExecute = true,
        });

        return true;
    }

    public static bool TryOpenFile(string filePath, out string? errorMessage)
    {
        errorMessage = null;

        if (string.IsNullOrWhiteSpace(filePath))
        {
            errorMessage = "File path is empty.";
            return false;
        }

        if (!File.Exists(filePath))
        {
            errorMessage = $"File not found: {filePath}";
            return false;
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = filePath,
            UseShellExecute = true,
        });

        return true;
    }
}
