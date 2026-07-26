using UnityEditor;
using UnityEditor.Callbacks;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

public class PostBuildScript {
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
        
        string appName = Path.GetFileNameWithoutExtension(pathToBuiltProject);
        
        // =========================================================================
        // 1. HANDLE WINDOWS BUILDS
        // =========================================================================
        if (target == BuildTarget.StandaloneWindows64 || target == BuildTarget.StandaloneWindows) {
            string buildDir = Path.GetDirectoryName(pathToBuiltProject);
            string parentDir = Path.GetDirectoryName(buildDir);
            string zipOutputPath = Path.Combine(parentDir, $"{Path.GetFileName(buildDir)}.zip");

            if (File.Exists(zipOutputPath)) File.Delete(zipOutputPath);

            UnityEngine.Debug.Log($"[PostBuild] Zipping Windows build to: {zipOutputPath}");

            using (ZipArchive archive = ZipFile.Open(zipOutputPath, ZipArchiveMode.Create)) {
                string[] allFiles = Directory.GetFiles(buildDir, "*.*", SearchOption.AllDirectories);

                foreach (string file in allFiles) {
                    string relativePath = Path.GetRelativePath(buildDir, file);

                    if (relativePath.Contains("BurstDebugInformation_DoNotShip") || 
                        relativePath.Contains("BackUpThisFolder_ButDontShipItWithYourGame")) {
                        continue; 
                    }

                    archive.CreateEntryFromFile(file, relativePath);
                }
            }
            UnityEngine.Debug.Log("[PostBuild] Windows .zip creation complete!");
        }
        
        // =========================================================================
        // 2. HANDLE MAC BUILDS (Perfect Alignment with your exact directory layout)
        // =========================================================================
        else if (target == BuildTarget.StandaloneOSX) {
            // Under your settings:
            // pathToBuiltProject is:  .../Builds/WCTG_Alpha_v0.1_macOS/WCTG_Alpha_v0.1_macOS.app
            string appFile = Path.GetFileName(pathToBuiltProject);       // WCTG_Alpha_v0.1_macOS.app
            string targetFolder = Path.GetDirectoryName(pathToBuiltProject); // .../Builds/WCTG_Alpha_v0.1_macOS
            string mainBuildsDir = Path.GetDirectoryName(targetFolder);    // .../Builds

            // Target compressed path output: .../Builds/WCTG_Alpha_v0.1_macOS.app.tar.gz
            string tarGzOutputPath = Path.Combine(mainBuildsDir, $"{appFile}.tar.gz");
            
            // Explicitly look for the Burst folder sitting next to the .app file
            string burstFolderPattern = $"*{appName}_BurstDebugInformation_DoNotShip*";

            UnityEngine.Debug.Log($"[PostBuild] Correctly tarring Mac bundle to main folder: {tarGzOutputPath}");

            Process process = new Process();
            process.StartInfo.FileName = "tar";
            // -C changes the working directory of the tar engine to your targetFolder.
            // This ensures it packages only the .app while leaving the DoNotShip folders out of the stream.
            process.StartInfo.Arguments = $"-czf \"{tarGzOutputPath}\" -C \"{targetFolder}\" --exclude=\"{burstFolderPattern}\" \"{appFile}\"";
            process.StartInfo.UseShellExecute = false;
            process.Start();
            process.WaitForExit();
            
            UnityEngine.Debug.Log("[PostBuild] Mac .app.tar.gz placement successful!");
        }
        
        // =========================================================================
        // 3. HANDLE LINUX BUILDS
        // =========================================================================
        else if (target == BuildTarget.StandaloneLinux64) {
            string linuxBuildDir = Path.GetDirectoryName(pathToBuiltProject);
            string parentDir = Path.GetDirectoryName(linuxBuildDir);
            string folderName = Path.GetFileName(linuxBuildDir);
            
            string tarGzOutputPath = Path.Combine(parentDir, $"{folderName}.tar.gz");
            
            string burstPattern = $"*{appName}_BurstDebugInformation_DoNotShip*";
            string il2cppPattern = "*BackUpThisFolder_ButDontShipItWithYourGame*";
            
            UnityEngine.Debug.Log($"[PostBuild] Tarring Linux build to: {tarGzOutputPath}");

            Process process = new Process();
            process.StartInfo.FileName = "tar";
            process.StartInfo.Arguments = $"-czf \"{tarGzOutputPath}\" -C \"{parentDir}\" --exclude=\"{burstPattern}\" --exclude=\"{il2cppPattern}\" \"{folderName}\"";
            process.StartInfo.UseShellExecute = false;
            process.Start();
            process.WaitForExit();
            
            UnityEngine.Debug.Log("[PostBuild] Linux .tar.gz creation complete!");
        }
    }
}
