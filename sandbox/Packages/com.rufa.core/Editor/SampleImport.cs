using System.IO;
using UnityEditor;
using UnityEditor.PackageManager.UI;

namespace Rufa.Editor
{
    // Sample.Import refreshes the Package Manager's list of imported samples, which takes every folder under
    // Assets/Samples/RUFA Core for a version number and logs an error on the practices, copied there without one.
    // A plain copy into the same folder imports the sample just the same, without the error.
    static class SampleImport
    {
        internal static bool Copy(Sample sample)
        {
            if (sample.displayName == null) return false;
            Directory.CreateDirectory(Path.GetDirectoryName(sample.importPath));
            FileUtil.CopyFileOrDirectory(sample.resolvedPath, sample.importPath);
            AssetDatabase.Refresh();
            return true;
        }
    }
}
