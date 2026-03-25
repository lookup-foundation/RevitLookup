using Nice3point.Revit.Injector;
using Nice3point.TUnit.Revit;
using Nice3point.TUnit.Revit.Executors;
using TUnit.Core.Executors;

namespace RevitLookup.Tests.Unit.Abstractions;

public class RevitFamilySampleTest : RevitApiTest
{
    private static readonly string SamplesPath = $@"C:\Program Files\Autodesk\Revit {RevitEnvironment.MajorVersion}\Samples";

    private protected Dictionary<string, Document> FamilyDocuments { get; } = [];
    public static string[] RevitFamilies { get; } = Directory.Exists(SamplesPath) ? Directory.EnumerateFiles(SamplesPath, "*.rfa").ToArray() : [];

    [Before(Test)]
    [HookExecutor<RevitThreadExecutor>]
    public void OpenDocuments()
    {
        foreach (var path in RevitFamilies)
        {
            var isolatedPath = Path.Combine(Path.GetTempPath(), $"{Path.GetRandomFileName()}.rfa");
            File.Copy(path, isolatedPath);

            using (RevitApiContext.BeginFailureSuppressionScope())
            {
                FamilyDocuments[path] = Application.OpenDocumentFile(isolatedPath);
            }
        }
    }

    [After(Test)]
    [HookExecutor<RevitThreadExecutor>]
    public void CloseDocuments()
    {
        foreach (var document in FamilyDocuments.Values)
        {
            var filePath = document.PathName;
            document.Close(false);

            File.SetAttributes(filePath, FileAttributes.Normal);
            File.Delete(filePath);
        }
    }
}