using Nice3point.Revit.Injector;
using Nice3point.TUnit.Revit;
using Nice3point.TUnit.Revit.Executors;
using TUnit.Core.Executors;

namespace RevitLookup.Tests.Unit.Abstractions;

public class RevitModelSampleTest : RevitApiTest
{
    private static readonly string SamplesPath = $@"C:\Program Files\Autodesk\Revit {RevitEnvironment.MajorVersion}\Samples";

    private protected Dictionary<string, Document> ModelDocuments { get; } = [];

    public static string[] RevitModels { get; } = Directory.Exists(SamplesPath)
        ? Directory.EnumerateFiles(SamplesPath, "*.rvt")
            .Select(path => new FileInfo(path))
            .OrderBy(file => file.Length)
            .Take(1)
            .Select(file => file.FullName)
            .ToArray()
        : [];

    [Before(Test)]
    [HookExecutor<RevitThreadExecutor>]
    public void OpenDocuments()
    {
        foreach (var path in RevitModels)
        {
            var isolatedPath = Path.Combine(Path.GetTempPath(), $"{Path.GetRandomFileName()}.rvt");
            File.Copy(path, isolatedPath);

            using (RevitApiContext.BeginFailureSuppressionScope())
            {
                ModelDocuments[path] = Application.OpenDocumentFile(isolatedPath);
            }
        }
    }

    [After(Test)]
    [HookExecutor<RevitThreadExecutor>]
    public void CloseDocuments()
    {
        foreach (var document in ModelDocuments.Values)
        {
            var filePath = document.PathName;
            document.Close(false);

            File.SetAttributes(filePath, FileAttributes.Normal);
            File.Delete(filePath);
        }
    }
}