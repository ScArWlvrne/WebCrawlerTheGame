using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;

public class PostBuildScript {
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
        string appName = Path.GetFileNameWithoutExtension(pathToBuiltProject);

        // =========================================================================
        // 1. WINDOWS
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

                    if (ShouldExclude(relativePath)) continue;
                    archive.CreateEntryFromFile(file, relativePath);
                }
            }

            UnityEngine.Debug.Log("[PostBuild] Windows .zip creation complete!");
        }

        // =========================================================================
        // 2. macOS
        // =========================================================================
        else if (target == BuildTarget.StandaloneOSX) {
            string appFile = Path.GetFileName(pathToBuiltProject);
            string targetFolder = Path.GetDirectoryName(pathToBuiltProject);
            string mainBuildsDir = Path.GetDirectoryName(targetFolder);
            string tarGzOutputPath = Path.Combine(mainBuildsDir, $"{appFile}.tar.gz");

            // Path as it appears inside the tar archive.
            string macExecutablePrefix = $"{appFile}/Contents/MacOS/";

            UnityEngine.Debug.Log($"[PostBuild] Creating Mac archive: {tarGzOutputPath}");

            CreateTarGzWithPatchedModes(
                workingDirectory: targetFolder,
                itemToArchive: appFile,
                outputPath: tarGzOutputPath,
                shouldBeExecutable: archivePath =>
                    NormalizeArchivePath(archivePath).StartsWith(macExecutablePrefix, StringComparison.Ordinal)
            );

            UnityEngine.Debug.Log("[PostBuild] Mac .app.tar.gz creation complete; launcher mode set to 755.");
        }

        // =========================================================================
        // 3. LINUX
        // =========================================================================
        else if (target == BuildTarget.StandaloneLinux64) {
            string linuxExecutable = pathToBuiltProject;
            string linuxBuildDir = Path.GetDirectoryName(linuxExecutable);
            string parentDir = Path.GetDirectoryName(linuxBuildDir);
            string folderName = Path.GetFileName(linuxBuildDir);
            string executableName = Path.GetFileName(linuxExecutable);
            string tarGzOutputPath = Path.Combine(parentDir, $"{folderName}.tar.gz");
            string executableArchivePath = $"{folderName}/{executableName}";

            UnityEngine.Debug.Log($"[PostBuild] Creating Linux archive: {tarGzOutputPath}");

            CreateTarGzWithPatchedModes(
                workingDirectory: parentDir,
                itemToArchive: folderName,
                outputPath: tarGzOutputPath,
                shouldBeExecutable: archivePath =>
                    NormalizeArchivePath(archivePath) == executableArchivePath
            );

            UnityEngine.Debug.Log("[PostBuild] Linux .tar.gz creation complete; launcher mode set to 755.");
        }
    }

    private static bool ShouldExclude(string path) {
        return path.Contains("BurstDebugInformation_DoNotShip") ||
               path.Contains("BackUpThisFolder_ButDontShipItWithYourGame");
    }

    // Let the system tar implementation build a completely normal archive with all
    // file contents, then patch ONLY the POSIX mode fields in its headers. This avoids
    // depending on Windows filesystem permissions while also avoiding a homemade tar writer.
    private static void CreateTarGzWithPatchedModes(
        string workingDirectory,
        string itemToArchive,
        string outputPath,
        Func<string, bool> shouldBeExecutable
    ) {
        if (File.Exists(outputPath)) File.Delete(outputPath);

        string tempTar = outputPath + ".tmp.tar";
        if (File.Exists(tempTar)) File.Delete(tempTar);

        try {
            RunTar(workingDirectory, itemToArchive, tempTar);
            PatchTarModes(tempTar, shouldBeExecutable);

            using (FileStream input = new FileStream(tempTar, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (FileStream output = new FileStream(outputPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            using (GZipStream gzip = new GZipStream(output, CompressionLevel.Optimal)) {
                input.CopyTo(gzip);
            }
        }
        finally {
            if (File.Exists(tempTar)) File.Delete(tempTar);
        }
    }

    private static void RunTar(string workingDirectory, string itemToArchive, string tempTar) {
        // We archive one build directory/.app and exclude Unity's generated debug/backup folders.
        string args =
            $"-cf \"{tempTar}\" " +
            $"--exclude=\"*BurstDebugInformation_DoNotShip*\" " +
            $"--exclude=\"*BackUpThisFolder_ButDontShipItWithYourGame*\" " +
            $"-C \"{workingDirectory}\" \"{itemToArchive}\"";

        ProcessStartInfo startInfo = new ProcessStartInfo {
            FileName = "tar",
            Arguments = args,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        using (Process process = Process.Start(startInfo)) {
            string stdout = process.StandardOutput.ReadToEnd();
            string stderr = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0) {
                throw new Exception(
                    $"tar failed with exit code {process.ExitCode}.\n" +
                    $"stdout:\n{stdout}\n" +
                    $"stderr:\n{stderr}"
                );
            }
        }

        if (!File.Exists(tempTar) || new FileInfo(tempTar).Length == 0) {
            throw new Exception("tar reported success but did not produce a non-empty archive.");
        }
    }

    private static void PatchTarModes(string tarPath, Func<string, bool> shouldBeExecutable) {
        using (FileStream stream = new FileStream(tarPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) {
            byte[] header = new byte[512];

            while (stream.Position + 512 <= stream.Length) {
                long headerPosition = stream.Position;
                ReadExactly(stream, header, 0, 512);

                if (IsZeroBlock(header)) break;

                string name = ReadNullTerminatedString(header, 0, 100);
                string prefix = ReadNullTerminatedString(header, 345, 155);
                string archivePath = string.IsNullOrEmpty(prefix) ? name : prefix + "/" + name;
                archivePath = NormalizeArchivePath(archivePath);

                byte typeFlag = header[156];
                long size = ReadOctal(header, 124, 12);

                int mode;
                if (typeFlag == (byte)'5' || archivePath.EndsWith("/", StringComparison.Ordinal)) {
                    mode = 493; // 0755 in decimal: rwxr-xr-x
                }
                else if ((typeFlag == 0 || typeFlag == (byte)'0') &&
                         shouldBeExecutable != null && shouldBeExecutable(archivePath)) {
                    mode = 493; // 0755 in decimal: rwxr-xr-x
                }
                else {
                    mode = 420; // 0644 in decimal: rw-r--r--
                }

                WriteOctalField(header, 100, 8, mode);
                RecalculateChecksum(header);

                stream.Position = headerPosition;
                stream.Write(header, 0, 512);

                // Move to the next header. Tar payloads are padded to 512-byte blocks.
                long payloadBlocks = (size + 511) / 512;
                stream.Position = headerPosition + 512 + payloadBlocks * 512;
            }
        }
    }

    private static string NormalizeArchivePath(string path) {
        return (path ?? string.Empty).Replace('\\', '/').TrimStart('.', '/');
    }

    private static bool IsZeroBlock(byte[] block) {
        for (int i = 0; i < block.Length; i++) {
            if (block[i] != 0) return false;
        }
        return true;
    }

    private static string ReadNullTerminatedString(byte[] buffer, int offset, int length) {
        int count = 0;
        while (count < length && buffer[offset + count] != 0) count++;
        return Encoding.UTF8.GetString(buffer, offset, count).TrimEnd(' ');
    }

    private static long ReadOctal(byte[] buffer, int offset, int length) {
        string text = Encoding.ASCII.GetString(buffer, offset, length).Trim('\0', ' ');
        if (string.IsNullOrEmpty(text)) return 0;
        return Convert.ToInt64(text, 8);
    }

    private static void WriteOctalField(byte[] buffer, int offset, int length, long value) {
        string octal = Convert.ToString(value, 8);
        if (octal.Length > length - 1) {
            throw new ArgumentOutOfRangeException(nameof(value), "Value does not fit in tar octal field.");
        }

        string field = octal.PadLeft(length - 1, '0');
        byte[] bytes = Encoding.ASCII.GetBytes(field);
        Array.Clear(buffer, offset, length);
        Buffer.BlockCopy(bytes, 0, buffer, offset, bytes.Length);
        buffer[offset + length - 1] = 0;
    }

    private static void RecalculateChecksum(byte[] header) {
        for (int i = 148; i < 156; i++) header[i] = 0x20;

        int checksum = 0;
        for (int i = 0; i < header.Length; i++) checksum += header[i];

        string octal = Convert.ToString(checksum, 8).PadLeft(6, '0');
        if (octal.Length > 6) {
            throw new InvalidDataException("Tar header checksum overflow.");
        }

        byte[] bytes = Encoding.ASCII.GetBytes(octal);
        Buffer.BlockCopy(bytes, 0, header, 148, 6);
        header[154] = 0;
        header[155] = 0x20;
    }

    private static void ReadExactly(Stream stream, byte[] buffer, int offset, int count) {
        int total = 0;
        while (total < count) {
            int read = stream.Read(buffer, offset + total, count - total);
            if (read == 0) throw new EndOfStreamException();
            total += read;
        }
    }
}
