namespace QuotationAccelerator.Desktop.Services;

using System.IO;
using QuotationAccelerator.Catalog.Domain;

public static class ProjectDocumentPaths
{
    public static string? GetDrawingPath(string projectFolder)
    {
        if (string.IsNullOrWhiteSpace(projectFolder))
        {
            return null;
        }

        var drawingPath = Path.Combine(projectFolder, ProjectDocumentFileNames.Drawing);
        return File.Exists(drawingPath) ? drawingPath : null;
    }

    public static string ToFileUri(string filePath) =>
        new Uri(Path.GetFullPath(filePath)).AbsoluteUri;
}
