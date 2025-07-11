#if NET6_0_OR_GREATER
using CustomLogger;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Drawing;

namespace MultiServerLibrary.Upscalers;

public static class ImageOptimizer
{
    public const string defaultOptimizerParams = "-filter Catrom -quality 92 -modulate 105,103 -sigmoidal-contrast 3,50%";

    private static string tmpDir = $"{Path.GetTempPath()}/ImageOptimizer";

    public static Stream OptimizeImage(string convertersDir, string imagePath,
        string extension, string CommandLineParametersConvert,
        string CommandLineParametersFsr = "-QualityMode UltraQuality -Scale 2x 2x",
        bool PreferFSR = true)
    {
        if (string.IsNullOrEmpty(convertersDir) || !Directory.Exists(convertersDir))
            return File.OpenRead(imagePath);
        string magickDirPath = $"{convertersDir}/ImageMagick/";
        string sourcefilePath = Path.Combine(tmpDir, $"{Guid.NewGuid()}{extension}");
        string tempfilePath = Path.Combine(tmpDir, $"{Guid.NewGuid()}_tmp{extension}");
        string tempScaledfilePath = Path.Combine(tmpDir, $"{Guid.NewGuid()}_Scaled{extension}");
        string tempSharpenedfilePath = Path.Combine(tmpDir, $"{Guid.NewGuid()}_Sharpened{extension}");
        string tempDownScaledfilePath = Path.Combine(tmpDir, $"{Guid.NewGuid()}_DownScaled{extension}");
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
        if (!string.IsNullOrEmpty(convertFilePath) && Directory.Exists(magickDirPath) && Directory.GetFiles(magickDirPath, $"convert{convertFilePath}*").Length > 0)
        {
            convertFilePath = $"{convertersDir}/ImageMagick/convert{convertFilePath}";
            try
            {
                extension = extension.Substring(1).ToLower();
                Directory.CreateDirectory(tmpDir);
                File.Copy(imagePath, sourcefilePath);
                if (extension == "dds")
                {
                    var ddsHeaderData = ExtractDDSProperties(sourcefilePath);
                    // Check for potential errors reading the DDS header, and also forbid special DDS data (indicated by the Caps3 flag).
                    if (ddsHeaderData.Item1 || ddsHeaderData.Item2 == -1 || ddsHeaderData.Item3 == -1)
                        return File.OpenRead(imagePath);
                }
                using (Process magickProc = Process.Start(new ProcessStartInfo
                {
                    FileName = convertFilePath,
                    Arguments = $"\"{sourcefilePath}\" {CommandLineParametersConvert} \"{tempfilePath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }))
                {
                    magickProc?.WaitForExit();
                    if (magickProc?.ExitCode is 0)
                    {
                        if (Extension.Microsoft.Win32API.IsWindows)
                        {
                            // FidelityFx doesn't work well with transparency data...
                            bool isFidelityFxCompatible;
                            try
                            {
                                isFidelityFxCompatible = !HasTransparentPixels(tempfilePath, extension);
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
                                                fidelityFilePath = $"{convertersDir}/FidelityFx/FidelityFX_CLI.exe";
                                                break;
                                        }
                                        if (!string.IsNullOrEmpty(fidelityFilePath) && File.Exists(fidelityFilePath))
                                        {
                                            if (PreferFSR)
                                            {
                                                try
                                                {
                                                    using (Process upscaleProc = Process.Start(new ProcessStartInfo
                                                    {
                                                        FileName = fidelityFilePath,
                                                        Arguments = $"-Mode EASU {CommandLineParametersFsr} \"{tempfilePath}\" \"{tempScaledfilePath}\"",
                                                        RedirectStandardOutput = true,
                                                        RedirectStandardError = true,
                                                        UseShellExecute = false,
                                                        CreateNoWindow = true
                                                    }))
                                                    {
                                                        upscaleProc?.WaitForExit();
                                                        if (upscaleProc?.ExitCode is 0)
                                                        {
                                                            try
                                                            {
                                                                using (Process sharpenProc = Process.Start(new ProcessStartInfo
                                                                {
                                                                    FileName = fidelityFilePath,
                                                                    Arguments = $"-Mode RCAS \"{tempScaledfilePath}\" \"{tempSharpenedfilePath}\"",
                                                                    RedirectStandardOutput = true,
                                                                    RedirectStandardError = true,
                                                                    UseShellExecute = false,
                                                                    CreateNoWindow = true
                                                                }))
                                                                {
                                                                    sharpenProc?.WaitForExit();
                                                                    if (sharpenProc?.ExitCode is 0)
                                                                    {
                                                                        try
                                                                        {
                                                                            using (Process magickDownSampleProc = Process.Start(new ProcessStartInfo
                                                                            {
                                                                                FileName = convertFilePath,
                                                                                Arguments = $"\"{tempSharpenedfilePath}\" -resize 50% \"{tempDownScaledfilePath}\"",
                                                                                RedirectStandardOutput = true,
                                                                                RedirectStandardError = true,
                                                                                UseShellExecute = false,
                                                                                CreateNoWindow = true
                                                                            }))
                                                                            {
                                                                                magickDownSampleProc?.WaitForExit();
                                                                                if (magickDownSampleProc?.ExitCode is 0)
                                                                                    return new MemoryStream(File.ReadAllBytes(tempDownScaledfilePath));
                                                                                else
                                                                                    LoggerAccessor.LogWarn($"[ImageOptimizer] - ImageMagick downsample process failed with status code: {magickDownSampleProc?.ExitCode}");
                                                                            }
                                                                        }
                                                                        catch (Exception ex)
                                                                        {
                                                                            LoggerAccessor.LogWarn($"[ImageOptimizer] - ImageMagick downsample process failed - {ex}");
                                                                        }
                                                                        return new MemoryStream(File.ReadAllBytes(tempSharpenedfilePath));
                                                                    }
                                                                    else
                                                                        LoggerAccessor.LogWarn($"[ImageOptimizer] - FidelityFX_CLI sharpen process failed with status code: {sharpenProc?.ExitCode}");
                                                                }
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                LoggerAccessor.LogWarn($"[ImageOptimizer] - FidelityFX_CLI sharpen process failed - {ex}");
                                                            }
                                                        }
                                                        else
                                                            LoggerAccessor.LogWarn($"[ImageOptimizer] - FidelityFX_CLI upscale process failed with status code: {upscaleProc?.ExitCode}");
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    LoggerAccessor.LogWarn($"[ImageOptimizer] - FidelityFX_CLI upscale process failed - {ex}");
                                                }
                                            }
                                            else
                                            {
                                                try
                                                {
                                                    using (Process sharpenProc = Process.Start(new ProcessStartInfo
                                                    {
                                                        FileName = fidelityFilePath,
                                                        Arguments = $"-Mode RCAS \"{tempfilePath}\" \"{tempSharpenedfilePath}\"",
                                                        RedirectStandardOutput = true,
                                                        RedirectStandardError = true,
                                                        UseShellExecute = false,
                                                        CreateNoWindow = true
                                                    }))
                                                    {
                                                        sharpenProc?.WaitForExit();
                                                        if (sharpenProc?.ExitCode is 0)
                                                            return new MemoryStream(File.ReadAllBytes(tempSharpenedfilePath));
                                                        else
                                                            LoggerAccessor.LogWarn($"[ImageOptimizer] - FidelityFX_CLI sharpen process failed with status code: {sharpenProc?.ExitCode}");
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    LoggerAccessor.LogWarn($"[ImageOptimizer] - FidelityFX_CLI sharpen process failed - {ex}");
                                                }
                                            }
                                        }
                                        else
                                            LoggerAccessor.LogWarn("[ImageOptimizer] - Could not find FidelityFX_CLI for current architecture, aborting process.");
                                        break;
                                    default:
#if DEBUG
                                        LoggerAccessor.LogWarn("[ImageOptimizer] - Input file is not compatible with FidelityFX_CLI, trying ffmpeg CAS instead.");
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
                                if (Directory.GetFiles(convertersDir, $"ffmpeg.*").Length > 0)
                                {
                                    try
                                    {
                                        using (Process sharpenProc = Process.Start(new ProcessStartInfo
                                        {
                                            FileName = $"{convertersDir}/ffmpeg",
                                            Arguments = $"-i \"{tempfilePath}\" -filter_complex \"[0:v]split=2[fg][alpha];[fg]cas=0.3[fg];[alpha]alphaextract[alpha];[fg][alpha]alphamerge\" \"{tempSharpenedfilePath}\"",
                                            RedirectStandardOutput = true,
                                            RedirectStandardError = true,
                                            UseShellExecute = false,
                                            CreateNoWindow = true
                                        }))
                                        {
                                            sharpenProc?.WaitForExit();
                                            if (sharpenProc?.ExitCode is 0)
                                                return new MemoryStream(File.ReadAllBytes(tempSharpenedfilePath));
                                            else
                                                LoggerAccessor.LogWarn($"[ImageOptimizer] - ffmpeg sharpen process failed with status code: {sharpenProc?.ExitCode}");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        LoggerAccessor.LogWarn($"[ImageOptimizer] - ffmpeg sharpen process failed - {ex}");
                                    }
                                }
                                break;
                        }
                        return new MemoryStream(File.ReadAllBytes(tempfilePath));
                    }
                    else
                        LoggerAccessor.LogWarn($"[ImageOptimizer] - ImageMagick conversion process failed with status code: {magickProc?.ExitCode}");
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogWarn($"[ImageOptimizer] - ImageMagick conversion process failed - {ex}");
            }
            finally
            {
                _ = Task.Run(() =>
                {
                    if (File.Exists(sourcefilePath)) File.Delete(sourcefilePath);
                    if (File.Exists(tempfilePath)) File.Delete(tempfilePath);
                    if (File.Exists(tempScaledfilePath)) File.Delete(tempScaledfilePath);
                    if (File.Exists(tempSharpenedfilePath)) File.Delete(tempSharpenedfilePath);
                    if (File.Exists(tempDownScaledfilePath)) File.Delete(tempDownScaledfilePath);
                });
            }
        }
        else
            LoggerAccessor.LogWarn("[ImageOptimizer] - Could not find ImageMagick for current architecture, aborting convert process.");
        return File.OpenRead(imagePath);
    }

    private static bool HasTransparentPixels(string imagePath, string extension)
    {
        byte[] imageBytes = File.ReadAllBytes(imagePath);

        if (extension == "ico")
            return HasIcoTransparency(imageBytes);
        else if (SixLabors.ImageSharp.Image.DetectFormat(imageBytes) == null)
            throw new NotSupportedException($"[ImageOptimizer] - HasTransparentPixels - The file format '{extension}' is not supported by ImageSharp.");

        bool HasTransparency = false;

        using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load<Rgba32>(imageBytes))
            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    Span<Rgba32> pixelRow = accessor.GetRowSpan(y);

                    // pixelRow.Length has the same value as accessor.Width,
                    // but using pixelRow.Length allows the JIT to optimize away bounds checks:
                    for (int x = 0; x < pixelRow.Length; x++)
                    {
                        // Get a reference to the pixel at position x
                        ref Rgba32 pixel = ref pixelRow[x];
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
        using (MemoryStream ms = new MemoryStream(icoBytes))
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
            LoggerAccessor.LogError($"[ImageOptimizer] - ExtractDDSProperties - File:{filePath} not found.");
            return (false, -1, -1);
        }

        try
        {
            using (FileStream fs = File.OpenRead(filePath))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                if (fs.Length < 128)
                {
                    LoggerAccessor.LogError($"[ImageOptimizer] - ExtractDDSProperties - File:{filePath} is too small to be a valid DDS.");
                    return (false, -1, -1);
                }

                // DDS files start with "DDS " (0x20534444 in little-endian).
                uint magic = reader.ReadUInt32();
                if (magic != 0x20534444) // "DDS "
                {
                    LoggerAccessor.LogError($"[ImageOptimizer] - ExtractDDSProperties - File:{filePath} is not a valid DDS.");
                    return (false, -1, -1);
                }

                // Skip 8 bytes to get to height and width.
                reader.BaseStream.Seek(8, SeekOrigin.Current);

                int height = reader.ReadInt32();
                int width = reader.ReadInt32();

                // Reads the Caps3 flag to detect special DDS formats (seen in PS Home).
                reader.BaseStream.Seek(0x70, SeekOrigin.Begin);

                return (reader.ReadInt32() != 0, height, width);
            }
        }
        catch (Exception ex)
        {
            LoggerAccessor.LogError($"[ImageOptimizer] - ExtractDDSProperties - Error while reading DDS File:{filePath}. (Exception:{ex})");
        }

        return (false, -1, -1);
    }
}
#endif