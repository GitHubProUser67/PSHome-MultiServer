using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using CustomLogger;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace MultiServerLibrary.Upscalers;

public static class ImageOptimizer
{
    public const string defaultOptimizerParams =
        "-filter Catrom -quality 92 -modulate 105,103 -sigmoidal-contrast 3,50%";

    private static readonly string tmpDir = $"{Path.GetTempPath()}/ImageOptimizer";

    public static Stream OptimizeImagesToPdf(
        string convertersDir,
        IEnumerable<string> imagePaths,
        string CommandLineParametersConvert,
        string CommandLineParametersFsr = "-QualityMode UltraQuality -Scale 2x 2x",
        bool PreferFSR = true
    )
    {
        if (string.IsNullOrEmpty(convertersDir) || !Directory.Exists(convertersDir))
            throw new DirectoryNotFoundException(
                $"[ImageOptimizer] - Converters directory not found: {convertersDir}."
            );
        var imageMagickDir = Path.Combine(convertersDir, "ImageMagick");
        string convertFilePath = null;
        switch (RuntimeInformation.OSArchitecture)
        {
            case Architecture.X86:
                convertFilePath = "32";
                break;
            case Architecture.X64:
                convertFilePath = "64";
                break;
            case Architecture.Arm64:
                convertFilePath = "ARM";
                break;
        }
        if (
            string.IsNullOrEmpty(convertFilePath)
            || !Directory.Exists(imageMagickDir)
            || Directory.GetFiles(imageMagickDir, $"convert{convertFilePath}*").Length == 0
        )
            throw new FileNotFoundException(
                "[ImageOptimizer] - ImageMagick convert binary not found for this architecture."
            );
        convertFilePath = $"{imageMagickDir}/convert{convertFilePath}";
        Directory.CreateDirectory(tmpDir);
        var outputPdfPath = Path.Combine(tmpDir, $"{Guid.NewGuid()}.pdf");
        var optimizedFiles = new List<string>();
        try
        {
            foreach (var img in imagePaths)
            {
                var ext = Path.GetExtension(img);
                using var optimizedStream = OptimizeImage(
                    convertersDir,
                    imageMagickDir,
                    img,
                    ext,
                    CommandLineParametersConvert,
                    CommandLineParametersFsr,
                    PreferFSR
                );
                var tmpOut = Path.Combine(tmpDir, $"{Guid.NewGuid()}{ext}");
                using var fs = File.Create(tmpOut);
                optimizedStream.CopyTo(fs);
                optimizedFiles.Add(tmpOut);
            }
            using (
                var pdfProc = Process.Start(
                    new ProcessStartInfo
                    {
                        FileName = convertFilePath,
                        Arguments =
                            string.Join(" ", optimizedFiles.Select(f => $"\"{f}\""))
                            + $" \"{outputPdfPath}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    }
                )
            )
            {
                pdfProc?.WaitForExit();

                if (pdfProc?.ExitCode is not 0)
                {
                    LoggerAccessor.LogError(
                        $"[ImageOptimizer] - PDF creation failed. Error: {pdfProc?.StandardError.ReadToEnd()}"
                    );
                    return null;
                }
            }
            return new MemoryStream(File.ReadAllBytes(outputPdfPath));
        }
        finally
        {
            Task.Run(() =>
            {
                foreach (var file in optimizedFiles)
                    if (File.Exists(file))
                        File.Delete(file);
                if (File.Exists(outputPdfPath))
                    File.Delete(outputPdfPath);
            });
        }
    }

    public static Stream OptimizeImage(
        string convertersDir,
        string imageMagickDir,
        string imagePath,
        string extension,
        string CommandLineParametersConvert,
        string CommandLineParametersFsr = "-QualityMode UltraQuality -Scale 2x 2x",
        bool PreferFSR = true
    )
    {
        if (
            string.IsNullOrEmpty(convertersDir)
            || string.IsNullOrEmpty(imageMagickDir)
            || !Directory.Exists(convertersDir)
            || !Directory.Exists(imageMagickDir)
        )
            return File.OpenRead(imagePath);
        var sourcefilePath = Path.Combine(tmpDir, $"{Guid.NewGuid()}{extension}");
        var tempfilePath = Path.Combine(tmpDir, $"{Guid.NewGuid()}_tmp{extension}");
        var tempScaledfilePath = Path.Combine(tmpDir, $"{Guid.NewGuid()}_Scaled{extension}");
        var tempSharpenedfilePath = Path.Combine(tmpDir, $"{Guid.NewGuid()}_Sharpened{extension}");
        var tempDownScaledfilePath = Path.Combine(
            tmpDir,
            $"{Guid.NewGuid()}_DownScaled{extension}"
        );
        string convertFilePath = null;
        switch (RuntimeInformation.OSArchitecture)
        {
            case Architecture.X86:
                convertFilePath = "32";
                break;
            case Architecture.X64:
                convertFilePath = "64";
                break;
            case Architecture.Arm64:
                convertFilePath = "ARM";
                break;
        }
        if (
            !string.IsNullOrEmpty(convertFilePath)
            && Directory.GetFiles(imageMagickDir, $"convert{convertFilePath}*").Length > 0
        )
        {
            convertFilePath = $"{imageMagickDir}/convert{convertFilePath}";
            try
            {
                extension = extension.Substring(1).ToLower();
                Directory.CreateDirectory(tmpDir);
                File.Copy(imagePath, sourcefilePath);
                if (extension == "dds")
                {
                    var ddsHeaderData = ExtractDDSProperties(sourcefilePath);
                    // Check for potential errors reading the DDS header, and also forbid special DDS data (indicated by the Caps3 flag).
                    if (
                        ddsHeaderData.Item1
                        || ddsHeaderData.Item2 == -1
                        || ddsHeaderData.Item3 == -1
                    )
                        return File.OpenRead(imagePath);
                }
                using (
                    var magickProc = Process.Start(
                        new ProcessStartInfo
                        {
                            FileName = convertFilePath,
                            Arguments =
                                $"\"{sourcefilePath}\" {CommandLineParametersConvert} \"{tempfilePath}\"",
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                        }
                    )
                )
                {
                    magickProc?.WaitForExit();
                    if (magickProc?.ExitCode is 0)
                    {
                        if (Extension.Windows.Win32API.IsWindows)
                        {
                            // FidelityFx doesn't work well with transparency data...
                            bool isFidelityFxCompatible;
                            try
                            {
                                isFidelityFxCompatible = !HasTransparentPixels(
                                    tempfilePath,
                                    extension
                                );
                            }
                            catch
                            {
                                isFidelityFxCompatible = false;
                            }
                            if (isFidelityFxCompatible)
                            {
                                switch (extension)
                                {
                                    case "bmp":
                                    case "png":
                                    case "ico":
                                    case "jpg":
                                    case "tif":
                                    case "gif":
                                        string fidelityFilePath = null;
                                        switch (RuntimeInformation.OSArchitecture)
                                        {
                                            case Architecture.X86:
                                            case Architecture.X64:
                                                fidelityFilePath =
                                                    $"{convertersDir}/FidelityFx/FidelityFX_CLI.exe";
                                                break;
                                        }
                                        if (
                                            !string.IsNullOrEmpty(fidelityFilePath)
                                            && File.Exists(fidelityFilePath)
                                        )
                                        {
                                            if (PreferFSR)
                                            {
                                                try
                                                {
                                                    using (
                                                        var upscaleProc = Process.Start(
                                                            new ProcessStartInfo
                                                            {
                                                                FileName = fidelityFilePath,
                                                                Arguments =
                                                                    $"-Mode EASU {CommandLineParametersFsr} \"{tempfilePath}\" \"{tempScaledfilePath}\"",
                                                                RedirectStandardOutput = true,
                                                                RedirectStandardError = true,
                                                                UseShellExecute = false,
                                                                CreateNoWindow = true,
                                                            }
                                                        )
                                                    )
                                                    {
                                                        upscaleProc?.WaitForExit();
                                                        if (upscaleProc?.ExitCode is 0)
                                                        {
                                                            try
                                                            {
                                                                using (
                                                                    var sharpenProc = Process.Start(
                                                                        new ProcessStartInfo
                                                                        {
                                                                            FileName =
                                                                                fidelityFilePath,
                                                                            Arguments =
                                                                                $"-Mode RCAS \"{tempScaledfilePath}\" \"{tempSharpenedfilePath}\"",
                                                                            RedirectStandardOutput =
                                                                                true,
                                                                            RedirectStandardError =
                                                                                true,
                                                                            UseShellExecute = false,
                                                                            CreateNoWindow = true,
                                                                        }
                                                                    )
                                                                )
                                                                {
                                                                    sharpenProc?.WaitForExit();
                                                                    if (sharpenProc?.ExitCode is 0)
                                                                    {
                                                                        try
                                                                        {
                                                                            using (
                                                                                var magickDownSampleProc =
                                                                                    Process.Start(
                                                                                        new ProcessStartInfo
                                                                                        {
                                                                                            FileName =
                                                                                                convertFilePath,
                                                                                            Arguments =
                                                                                                $"\"{tempSharpenedfilePath}\" -resize 50% \"{tempDownScaledfilePath}\"",
                                                                                            RedirectStandardOutput =
                                                                                                true,
                                                                                            RedirectStandardError =
                                                                                                true,
                                                                                            UseShellExecute =
                                                                                                false,
                                                                                            CreateNoWindow =
                                                                                                true,
                                                                                        }
                                                                                    )
                                                                            )
                                                                            {
                                                                                magickDownSampleProc?.WaitForExit();
                                                                                if (
                                                                                    magickDownSampleProc?.ExitCode
                                                                                    is 0
                                                                                )
                                                                                    return new MemoryStream(
                                                                                        File.ReadAllBytes(
                                                                                            tempDownScaledfilePath
                                                                                        )
                                                                                    );
                                                                                else
                                                                                    LoggerAccessor.LogWarn(
                                                                                        $"[ImageOptimizer] - ImageMagick downsample process failed with status code: {magickDownSampleProc?.ExitCode}"
                                                                                    );
                                                                            }
                                                                        }
                                                                        catch (Exception ex)
                                                                        {
                                                                            LoggerAccessor.LogWarn(
                                                                                $"[ImageOptimizer] - ImageMagick downsample process failed - {ex}"
                                                                            );
                                                                        }
                                                                        return new MemoryStream(
                                                                            File.ReadAllBytes(
                                                                                tempSharpenedfilePath
                                                                            )
                                                                        );
                                                                    }
                                                                    else
                                                                        LoggerAccessor.LogWarn(
                                                                            $"[ImageOptimizer] - FidelityFX_CLI sharpen process failed with status code: {sharpenProc?.ExitCode}"
                                                                        );
                                                                }
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                LoggerAccessor.LogWarn(
                                                                    $"[ImageOptimizer] - FidelityFX_CLI sharpen process failed - {ex}"
                                                                );
                                                            }
                                                        }
                                                        else
                                                            LoggerAccessor.LogWarn(
                                                                $"[ImageOptimizer] - FidelityFX_CLI upscale process failed with status code: {upscaleProc?.ExitCode}"
                                                            );
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    LoggerAccessor.LogWarn(
                                                        $"[ImageOptimizer] - FidelityFX_CLI upscale process failed - {ex}"
                                                    );
                                                }
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    using (
                                                        var sharpenProc = Process.Start(
                                                            new ProcessStartInfo
                                                            {
                                                                FileName = fidelityFilePath,
                                                                Arguments =
                                                                    $"-Mode RCAS \"{tempfilePath}\" \"{tempSharpenedfilePath}\"",
                                                                RedirectStandardOutput = true,
                                                                RedirectStandardError = true,
                                                                UseShellExecute = false,
                                                                CreateNoWindow = true,
                                                            }
                                                        )
                                                    )
                                                    {
                                                        sharpenProc?.WaitForExit();
                                                        if (sharpenProc?.ExitCode is 0)
                                                            return new MemoryStream(
                                                                File.ReadAllBytes(
                                                                    tempSharpenedfilePath
                                                                )
                                                            );
                                                        else
                                                            LoggerAccessor.LogWarn(
                                                                $"[ImageOptimizer] - FidelityFX_CLI sharpen process failed with status code: {sharpenProc?.ExitCode}"
                                                            );
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    LoggerAccessor.LogWarn(
                                                        $"[ImageOptimizer] - FidelityFX_CLI sharpen process failed - {ex}"
                                                    );
                                                }
                                            }
                                        }
                                        else
                                            LoggerAccessor.LogWarn(
                                                "[ImageOptimizer] - Could not find FidelityFX_CLI for current architecture, aborting process."
                                            );
                                        break;
                                    default:
#if DEBUG
                                        LoggerAccessor.LogWarn(
                                            "[ImageOptimizer] - Input file is not compatible with FidelityFX_CLI, trying ffmpeg CAS instead."
                                        );
#endif
                                        break;
                                }
                            }
                        }
                        switch (extension)
                        {
                            case "dds":
                                // ffmpeg can't output a DDS, only convert "from" a DDS is supported.
                                break;
                            default:
                                var ffmpegDir = Path.Combine(convertersDir, "ffmpeg");
                                if (
                                    Directory.Exists(ffmpegDir)
                                    && Directory.GetFiles(ffmpegDir, $"ffmpeg.*").Length > 0
                                )
                                {
                                    try
                                    {
                                        using (
                                            var sharpenProc = Process.Start(
                                                new ProcessStartInfo
                                                {
                                                    FileName = $"{ffmpegDir}/ffmpeg",
                                                    Arguments =
                                                        $"-i \"{tempfilePath}\" -filter_complex \"[0:v]split=2[fg][alpha];[fg]cas=0.3[fg];[alpha]alphaextract[alpha];[fg][alpha]alphamerge\" \"{tempSharpenedfilePath}\"",
                                                    RedirectStandardOutput = true,
                                                    RedirectStandardError = true,
                                                    UseShellExecute = false,
                                                    CreateNoWindow = true,
                                                }
                                            )
                                        )
                                        {
                                            sharpenProc?.WaitForExit();
                                            if (sharpenProc?.ExitCode is 0)
                                                return new MemoryStream(
                                                    File.ReadAllBytes(tempSharpenedfilePath)
                                                );
                                            else
                                                LoggerAccessor.LogWarn(
                                                    $"[ImageOptimizer] - ffmpeg sharpen process failed with status code: {sharpenProc?.ExitCode}"
                                                );
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        LoggerAccessor.LogWarn(
                                            $"[ImageOptimizer] - ffmpeg sharpen process failed - {ex}"
                                        );
                                    }
                                }
                                break;
                        }
                        return new MemoryStream(File.ReadAllBytes(tempfilePath));
                    }
                    else
                        LoggerAccessor.LogWarn(
                            $"[ImageOptimizer] - ImageMagick conversion process failed with status code: {magickProc?.ExitCode}"
                        );
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogWarn(
                    $"[ImageOptimizer] - ImageMagick conversion process failed - {ex}"
                );
            }
            finally
            {
                _ = Task.Run(() =>
                {
                    if (File.Exists(sourcefilePath))
                        File.Delete(sourcefilePath);
                    if (File.Exists(tempfilePath))
                        File.Delete(tempfilePath);
                    if (File.Exists(tempScaledfilePath))
                        File.Delete(tempScaledfilePath);
                    if (File.Exists(tempSharpenedfilePath))
                        File.Delete(tempSharpenedfilePath);
                    if (File.Exists(tempDownScaledfilePath))
                        File.Delete(tempDownScaledfilePath);
                });
            }
        }
        else
            LoggerAccessor.LogWarn(
                "[ImageOptimizer] - Could not find ImageMagick for current architecture, aborting convert process."
            );
        return File.OpenRead(imagePath);
    }

    private static bool HasTransparentPixels(string imagePath, string extension)
    {
        var imageBytes = File.ReadAllBytes(imagePath);

        if (extension == "ico")
            return HasIcoTransparency(imageBytes);
        else if (SixLabors.ImageSharp.Image.DetectFormat(imageBytes) == null)
            throw new NotSupportedException(
                $"[ImageOptimizer] - HasTransparentPixels - The file format '{extension}' is not supported by ImageSharp."
            );

        var HasTransparency = false;

        using (var image = SixLabors.ImageSharp.Image.Load<Rgba32>(imageBytes))
            image.ProcessPixelRows(accessor =>
            {
                for (var y = 0; y < accessor.Height; y++)
                {
                    var pixelRow = accessor.GetRowSpan(y);

                    // pixelRow.Length has the same value as accessor.Width,
                    // but using pixelRow.Length allows the JIT to optimize away bounds checks:
                    for (var x = 0; x < pixelRow.Length; x++)
                    {
                        // Get a reference to the pixel at position x
                        ref var pixel = ref pixelRow[x];
                        if (pixel.A < byte.MaxValue)
                        {
                            HasTransparency = true;
                            return;
                        }
                    }
                }
            });

        return HasTransparency;
    }

    private static bool HasIcoTransparency(byte[] icoBytes)
    {
        using (var ms = new MemoryStream(icoBytes))
#pragma warning disable
        using (Icon icon = new Icon(ms))
        using (Bitmap bitmap = icon.ToBitmap())
        {
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    System.Drawing.Color pixel = bitmap.GetPixel(x, y);
#pragma warning restore
                    if (pixel.A < byte.MaxValue)
                        return true;
                }
            }
        }
        return false;
    }

    private static (bool, int, int) ExtractDDSProperties(string filePath)
    {
        if (!File.Exists(filePath))
        {
            LoggerAccessor.LogError(
                $"[ImageOptimizer] - ExtractDDSProperties - File:{filePath} not found."
            );
            return (false, -1, -1);
        }

        try
        {
            using (var fs = File.OpenRead(filePath))
            using (var reader = new BinaryReader(fs))
            {
                if (fs.Length < 128)
                {
                    LoggerAccessor.LogError(
                        $"[ImageOptimizer] - ExtractDDSProperties - File:{filePath} is too small to be a valid DDS."
                    );
                    return (false, -1, -1);
                }

                // DDS files start with "DDS " (0x20534444 in little-endian).
                var magic = reader.ReadUInt32();
                if (magic != 0x20534444) // "DDS "
                {
                    LoggerAccessor.LogError(
                        $"[ImageOptimizer] - ExtractDDSProperties - File:{filePath} is not a valid DDS."
                    );
                    return (false, -1, -1);
                }

                // Skip 8 bytes to get to height and width.
                reader.BaseStream.Seek(8, SeekOrigin.Current);

                var height = reader.ReadInt32();
                var width = reader.ReadInt32();

                // Reads the Caps3 flag to detect special DDS formats (seen in PS Home).
                reader.BaseStream.Seek(0x70, SeekOrigin.Begin);

                return (reader.ReadInt32() != 0, height, width);
            }
        }
        catch (Exception ex)
        {
            LoggerAccessor.LogError(
                $"[ImageOptimizer] - ExtractDDSProperties - Error while reading DDS File:{filePath}. (Exception:{ex})"
            );
        }

        return (false, -1, -1);
    }
}
