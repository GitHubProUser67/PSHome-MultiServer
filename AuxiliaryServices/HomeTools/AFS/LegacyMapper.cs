using CustomLogger;
using MultiServerLibrary.Extension;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HomeTools.AFS
{
    public partial class LegacyMapper
    {
        private readonly List<string> filesToDelete = new List<string>();

        private static readonly ConcurrentBag<RegexPatterns> regexPatternsList = new ConcurrentBag<RegexPatterns>()
              {
                    new RegexPatterns()
                    {
                        type = ".mdl",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)mdl|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)mdl)"
                    },
                    new RegexPatterns
                    {
                        type = ".dds",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)dds|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)dds)"
                    },
                    new RegexPatterns
                    {
                        type = ".dds",
                        pattern = "(?<=\\b(?<=href=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)dds"
                    },
                    new RegexPatterns
                    {
                        type = ".dds",
                        pattern = "(?<=\\b(?<=src=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)dds"
                    },
                    new RegexPatterns
                    {
                        type = ".dds",
                        pattern = "([\\w-\\s]+\\\\)+[\\w-\\s]+\\.(?i)dds"
                    },
                    new RegexPatterns
                    {
                        type = "..dds",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*..(?i)dds|(?<=[0-9a-fA-F]{8}:)(.*\\..(?i)dds)"
                    },
                    new RegexPatterns
                    {
                        type = "..dds",
                        pattern = "(?<=\\b(?<=href=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*..(?i)dds"
                    },
                    new RegexPatterns
                    {
                        type = "..dds",
                        pattern = "(?<=\\b(?<=src=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*..(?i)dds"
                    },
                    new RegexPatterns
                    {
                        type = "..dds",
                        pattern = "([\\w-\\s]+\\\\)+[\\w-\\s]+\\..dds"
                    },
                    new RegexPatterns
                    {
                        type = "passphrase",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*(?i)passphrase|(?<=[0-9a-fA-F]{8}:)(.*(?i)passphrase)"
                    },
                    new RegexPatterns
                    {
                        type = "passphrase_EU",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*(?i)passphrase_EU|(?<=[0-9a-fA-F]{8}:)(.*(?i)passphrase_EU)"
                    },
                    new RegexPatterns
                    {
                        type = ".repertoire_circuit",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)repertoire_circuit|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)repertoire_circuit)"
                    },
                    new RegexPatterns
                    {
                        type = ".lua",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)lua|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)lua)"
                    },
                    new RegexPatterns
                    {
                        type = ".lua",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)Lua"
                    },
                    new RegexPatterns
                    {
                        type = ".luac",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)luac|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)luac)"
                    },
                    new RegexPatterns
                    {
                        type = ".json",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)json|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)json)"
                    },
                    new RegexPatterns
                    {
                        type = ".xml",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)xml|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)xml)"
                    },
                    new RegexPatterns
                    {
                        type = ".efx",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|efx_filename=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)efx|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)efx)"
                    },
                    new RegexPatterns
                    {
                        type = ".efx",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)effect|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)effect)"
                    },
                    new RegexPatterns
                    {
                        type = ".bnk",
                        pattern = "(?<=\\b(?<=source=\"|filename=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)bnk|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)bnk)"
                    },
                    new RegexPatterns
                    {
                        type = ".ttf",
                        pattern = "(?<=\\b(?<=source=\"|filename=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)ttf|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)ttf)"
                    },
                    new RegexPatterns
                    {
                        type = ".bank",
                        pattern = "(?<=\\b(?<=source=\"|filename=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)bank|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)bank)"
                    },
                    new RegexPatterns
                    {
                        type = ".hkx",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)hkx|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)hkx)"
                    },
                    new RegexPatterns
                    {
                        type = ".probe",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)probe|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)probe)"
                    },
                    new RegexPatterns
                    {
                        type = ".ocean",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)ocean|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)ocean)"
                    },
                    new RegexPatterns
                    {
                        type = ".skn",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)skn|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)skn)"
                    },
                    new RegexPatterns
                    {
                        type = ".ani",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)ani|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)ani)"
                    },
                    new RegexPatterns
                    {
                        type = ".mp3",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)mp3|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)mp3)"
                    },
                    new RegexPatterns
                    {
                        type = ".atmos",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)atmos|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)atmos)"
                    },
                    new RegexPatterns
                    {
                        type = ".png",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)png|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)png)"
                    },
                    new RegexPatterns
                    {
                        type = ".cer",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)cer|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)cer)"
                    },
                    new RegexPatterns
                    {
                        type = ".der",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)der|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)der)"
                    },
                    new RegexPatterns
                    {
                        type = ".bin",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)bin|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)bin)"
                    },
                    new RegexPatterns
                    {
                        type = ".raw",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)raw|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)raw)"
                    },
                    new RegexPatterns
                    {
                        type = ".ini",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)ini|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)ini)"
                    },
                    new RegexPatterns
                    {
                        type = ".txt",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)txt|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)txt)"
                    },
                    new RegexPatterns
                    {
                        type = ".enemy",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)enemy|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)enemy)"
                    },
                    new RegexPatterns
                    {
                        type = ".ui-setup",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)ui-setup|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)ui-setup)"
                    },
                    new RegexPatterns
                    {
                        type = ".cam-def",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)cam-def|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)cam-def)"
                    },
                    new RegexPatterns
                    {
                        type = ".level-setup",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)level-setup|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)level-setup)"
                    },
                    new RegexPatterns
                    {
                        type = ".node-def",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)node-def|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)node-def)"
                    },
                    new RegexPatterns
                    {
                        type = ".spline-def",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)spline-def|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)spline-def)"
                    },
                    new RegexPatterns
                    {
                        type = ".psd",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)psd|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)psd)"
                    },
                    new RegexPatterns
                    {
                        type = ".tmx",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)tmx|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)tmx)"
                    },
                    new RegexPatterns
                    {
                        type = ".atgi",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)atgi|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)atgi)"
                    },
                    new RegexPatterns
                    {
                        type = ".fpo",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)fpo|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)fpo)"
                    },
                    new RegexPatterns
                    {
                        type = ".bank",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)bank|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)bank)"
                    },
                    new RegexPatterns
                    {
                        type = ".bnk",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)bnk|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)bnk)"
                    },
                    new RegexPatterns
                    {
                        type = ".agf",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)agf|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)agf)"
                    },
                    new RegexPatterns
                    {
                        type = ".avtr",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)avtr|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)avtr)"
                    },
                    new RegexPatterns
                    {
                        type = ".vpo",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)vpo|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)vpo)"
                    },
                    new RegexPatterns
                    {
                        type = ".vxd",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)vxd|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)vxd)"
                    },
                    new RegexPatterns
                    {
                        type = ".jpg",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)jpg|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)jpg)"
                    },
                    new RegexPatterns
                    {
                        type = ".mp4",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)mp4|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)mp4)"
                    },
                    new RegexPatterns
                    {
                        type = ".sdat",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)sdat|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)sdat)"
                    },
                    new RegexPatterns
                    {
                        type = ".dat",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)dat|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)dat)"
                    },
                    new RegexPatterns
                    {
                        type = ".svml",
                        pattern = "(?<=\\b(?<=href=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)svml|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)svml)"
                    },
                    new RegexPatterns
                    {
                        type = ".svml",
                        pattern = "(?<=\\b(?<=src=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)svml"
                    },
                    new RegexPatterns
                    {
                        type = ".sql",
                        pattern = "(?<=\\b(?<=src=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)sql|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)sql)"
                    },
                    new RegexPatterns
                    {
                        type = ".svml",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)svml"
                    },
                    new RegexPatterns
                    {
                        type = ".fp",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)fp|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)fp)"
                    },
                    new RegexPatterns
                    {
                        type = ".sql",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)sql"
                    },
                    new RegexPatterns
                    {
                        type = ".vp",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)vp|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)vp)"
                    },
                    new RegexPatterns
                    {
                        type = ".dat",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)dat"
                    },
                    new RegexPatterns
                    {
                        type = ".sdat",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)sdat"
                    },
                    new RegexPatterns
                    {
                        type = ".bar",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)bar|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)bar)"
                    },
                    new RegexPatterns
                    {
                        type = ".odc",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)odc|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)odc)"
                    },
                    new RegexPatterns
                    {
                        type = ".scene",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)scene|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)scene)"
                    },
                    new RegexPatterns
                    {
                        type = ".scene",
                        pattern = "(?<=\\b(?<=config=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)scene"
                    },
                    new RegexPatterns
                    {
                        type = ".sharc",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)sharc|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)sharc)"
                    },
                    new RegexPatterns
                    {
                        type = ".map",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)map|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)map)"
                    },
                    new RegexPatterns
                    {
                        type = ".gui-setup",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)gui-setup|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)gui-setup)"
                    },
                    new RegexPatterns
                    {
                        type = ".oel",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)oel|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)oel)"
                    },
                    new RegexPatterns
                    {
                        type = ".wav",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)wav|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)wav)"
                    },
                    new RegexPatterns
                    {
                        type = ".sdc",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)sdc|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)sdc)"
                    },
                    new RegexPatterns
                    {
                        type = ".sdc",
                        pattern = "(?<=\\b(?<=desc=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)sdc"
                    },
                    new RegexPatterns
                    {
                        type = ".oxml",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)oxml|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)oxml)"
                    },
                    new RegexPatterns
                    {
                        type = ".ttf",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)ttf|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)ttf)"
                    },
                    new RegexPatterns
                    {
                        type = ".tga",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)tga|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)tga)"
                    },
                    new RegexPatterns
                    {
                        type = ".rig",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)rig|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)rig)"
                    },
                    new RegexPatterns
                    {
                        type = ".jsp",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)jsp|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)jsp)"
                    },
                    new RegexPatterns
                    {
                        type = ".fnt",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)fnt|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)fnt)"
                    },
                    new RegexPatterns
                    {
                        type = ".shpack",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*.(?i)shpack|(?<=[0-9a-fA-F]{8}:)(.*\\.(?i)shpack)"
                    },
                    new RegexPatterns
                    {
                        type = "noExtension",
                        pattern = "(?<=\\b(?<=source=\"|file=\"|texture\\s=\\s\"|spriteTexture\\s=\\s\"))[^\"]*?(?<!\\.[a-zA-Z0-9]{2,5})(?=\")|(?<=[0-9a-fA-F]{8}:)([^\"]*?(?<!\\.[a-zA-Z0-9]{2,5}))"
                    }
              };

        public Task MapperStart(string foldertomap, string helperfolder, string prefix, string bruteforce)
        {
            MapperPrepareFiles(foldertomap);
            try
            {
                if (bruteforce == "on")
                    CopyHelperFiles(helperfolder, foldertomap);
                if (string.IsNullOrEmpty(prefix))
                {
#if NET7_0_OR_GREATER
                    Match match = UUIDRegex().Match(foldertomap);
#else
                    Match match = new Regex(@"[0-9a-fA-F]{8}-[0-9a-fA-F]{8}-[0-9a-fA-F]{8}-[0-9a-fA-F]{8}").Match(foldertomap);
#endif
                    if (match.Success)
                        prefix = $"objects/{match.Groups[0].Value}/";
                    File.WriteAllText(foldertomap + "/ObjectXMLBruteforce.xml", "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                        $" <XML>\r\n<asset source=\"objects/{match.Groups[0].Value}/object.xml\"/>\r\n<asset source=\"objects/{match.Groups[0].Value}/res" +
                        $"ources.xml\"/>\r\n<asset source=\"objects/{match.Groups[0].Value}/localisation.xml\"/>\r\n</XML>");
                    filesToDelete.Add(foldertomap + "/ObjectXMLBruteforce.xml");
                }
                List<MappedList> mappedListList = new List<MappedList>();
                IEnumerable<string> strings = Directory.EnumerateFiles(foldertomap, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".mdl", StringComparison.InvariantCultureIgnoreCase) || s.EndsWith(".atmos", StringComparison.InvariantCultureIgnoreCase)
                || s.EndsWith(".efx", StringComparison.InvariantCultureIgnoreCase) || s.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase) || s.EndsWith(".scene", StringComparison.InvariantCultureIgnoreCase) || s.EndsWith(".map", StringComparison.InvariantCultureIgnoreCase)
                || s.EndsWith(".lua", StringComparison.InvariantCultureIgnoreCase) || s.EndsWith(".luac", StringComparison.InvariantCultureIgnoreCase) || s.EndsWith(".unknown", StringComparison.InvariantCultureIgnoreCase) || s.EndsWith(".txt", StringComparison.InvariantCultureIgnoreCase));
                foreach (string sourceFile in strings)
                {
                    using (StreamReader streamReader = File.OpenText(sourceFile))
                        mappedListList.AddRange(AFSRegexProcessor.ReturnMappedList(regexPatternsList, streamReader.ReadToEnd()));
                }
            uuidloop:
                foreach (MappedList mappedList in mappedListList)
                {
                    if (!string.IsNullOrEmpty(mappedList.file))
                    {
                        string text = AFSHash.EscapeString(mappedList.file);
                        text = Regex.Replace(text, "reward//ConfederateGeneral_M_Reward.dds", "reward/ConfederateGeneral_M_Reward.dds", RegexOptions.IgnoreCase);
                        text = Regex.Replace(text, "NATGParticles", "ATGParticles", RegexOptions.IgnoreCase);
                        text = Regex.Replace(text, "xenvironments", "environments", RegexOptions.IgnoreCase);
                        text = Regex.Replace(text, "Tenvironments", "environments", RegexOptions.IgnoreCase);
                        text = Regex.Replace(text, "netinit.svml", "host_svml\\netinit.svml", RegexOptions.IgnoreCase);
                        text = Regex.Replace(text, "home2.svml", "host_svml\\home2.svml", RegexOptions.IgnoreCase);
                        text = Regex.Replace(text, "home.svml", "host_svml\\home.svml", RegexOptions.IgnoreCase);
                        text = Regex.Replace(text, "agecheck.svml", "host_svml\\agecheck.svml", RegexOptions.IgnoreCase);
                        text = Regex.Replace(text, "ageratingfailed.svml", "host_svml\\ageratingfailed.svml", RegexOptions.IgnoreCase);
                        text = Regex.Replace(text, "GAMERETURNHOME2.svml", "host_svml\\GAMERETURNHOME2.svml", RegexOptions.IgnoreCase);
                        text = Regex.Replace(text, "HUBLOGIN.svml", "host_svml\\HUBLOGIN.svml", RegexOptions.IgnoreCase);
                        text = Regex.Replace(text, "LOGOUT.svml", "host_svml\\LOGOUT.svml", RegexOptions.IgnoreCase);
                        text = Regex.Replace(text, "nekoonline.svml", "host_svml\\nekoonline.svml", RegexOptions.IgnoreCase);
                        text = Regex.Replace(text, "onliness.svml", "host_svml\\onliness.svml", RegexOptions.IgnoreCase);
                        text = Regex.Replace(text, "onlinetext.svml", "host_svml\\onlinetext.svml", RegexOptions.IgnoreCase);
                        text = Regex.Replace(text, "placeholder.svml", "host_svml\\placeholder.svml", RegexOptions.IgnoreCase);
                        text = Regex.Replace(text, "popup.svml", "host_svml\\popup.svml", RegexOptions.IgnoreCase);
                        text = Regex.Replace(text, "unityinit.svml", "host_svml\\unityinit.svml", RegexOptions.IgnoreCase);
                        text = Regex.Replace(text, "unitymuis.svml", "host_svml\\unitymuis.svml", RegexOptions.IgnoreCase);
                        text = Regex.Replace(text, "unitytersm.svml", "host_svml\\unitytersm.svml", RegexOptions.IgnoreCase);
                        text = Regex.Replace(text, "update.svml", "host_svml\\update.svml", RegexOptions.IgnoreCase);
                        text = Regex.Replace(text, "nekomuis.svml", "host_svml\\nekomuis.svml", RegexOptions.IgnoreCase);

                        foreach (FileInfo file in new DirectoryInfo(foldertomap).GetFiles(new AFSHash(prefix + text).Value.ToString("X8") + ".*"))
                        {
                            if (File.Exists(Path.Combine(foldertomap, file.Name)))
                            {
                                string fileUri = FileSystemUtils.RemoveInvalidPathChars(text);
                                new FileInfo(Path.Combine(foldertomap, fileUri).ToUpper()).Directory?.Create();
                                string upper_casetext = fileUri.ToUpper();
                                if (!File.Exists(Path.Combine(foldertomap, upper_casetext)))
                                {
                                    File.Move(Path.Combine(foldertomap, file.Name), Path.Combine(foldertomap, upper_casetext));
#if DEBUG
                                    LoggerAccessor.LogInfo($"[Mapper] - Mapped {file.Name} -> {upper_casetext}");
#endif
                                }
                            }
                        }

                        if (text.ToLower().EndsWith(".atmos"))
                        {
                            string cdatafromatmos = text.Remove(text.Length - 6) + ".cdata";

                            foreach (FileInfo file in new DirectoryInfo(foldertomap).GetFiles(new AFSHash(cdatafromatmos).Value.ToString("X8") + ".*"))
                            {
                                if (File.Exists(Path.Combine(foldertomap, file.Name)))
                                {
                                    string fileUri = FileSystemUtils.RemoveInvalidPathChars(cdatafromatmos);
                                    new FileInfo(Path.Combine(foldertomap, fileUri).ToUpper()).Directory?.Create();
                                    string upper_casecdatafromatmos = fileUri.ToUpper();
                                    if (!File.Exists(Path.Combine(foldertomap, upper_casecdatafromatmos)))
                                    {
                                        File.Move(Path.Combine(foldertomap, file.Name), Path.Combine(foldertomap, upper_casecdatafromatmos));
#if DEBUG
                                        LoggerAccessor.LogInfo($"[Mapper] - Mapped {file.Name} -> {upper_casecdatafromatmos}");
#endif
                                    }
                                }
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(prefix) && prefix.StartsWith("objects/") && prefix.Length == 44 && prefix.EndsWith("/"))
                {
                    prefix = string.Empty;
                    goto uuidloop;
                }
                if (File.Exists(foldertomap + "/4E545585.dds") && !File.Exists(foldertomap + "/PLACEHOLDER_N.DDS"))
                    File.Move(foldertomap + "/4E545585.dds", foldertomap + "/PLACEHOLDER_N.DDS");
                if (File.Exists(foldertomap + "/4EE3523A.dds") && !File.Exists(foldertomap + "/PLACEHOLDER_S.DDS"))
                    File.Move(foldertomap + "/4EE3523A.dds", foldertomap + "/PLACEHOLDER_S.DDS");
                if (File.Exists(foldertomap + "/696E72D6.dds") && !File.Exists(foldertomap + "/HATBUBBLE.DDS"))
                    File.Move(foldertomap + "/696E72D6.dds", foldertomap + "/HATBUBBLE.DDS");
                if (File.Exists(foldertomap + "/D3A7AF9F.xml") && !File.Exists(foldertomap + "/__$manifest$__"))
                    File.Move(foldertomap + "/D3A7AF9F.xml", foldertomap + "/__$manifest$__");
                if (File.Exists(foldertomap + "/EDFBFAE9.xml") && !File.Exists(foldertomap + "/FILES.TXT"))
                    File.Move(foldertomap + "/EDFBFAE9.xml", foldertomap + "/FILES.TXT");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Mapper] - An Error happened in MapperStart - {ex}");
            }
            foreach (string file in filesToDelete)
            {
                if (File.Exists(file))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch
                    {
                    }
                }
            }
            return Task.CompletedTask;
        }

        private void MapperPrepareFiles(string foldertomap)
        {
            try
            {
                string text = null;

                foreach (string filePath in Directory.GetFiles(foldertomap))
                {
                    string newFilePath = filePath.Replace("0X", string.Empty);
                    text = File.ReadAllText(filePath);

                    if (new FileInfo(filePath).Length <= 0)
                    {
                        if (!File.Exists(newFilePath + ".CORRUPTED"))
                            File.Move(filePath, newFilePath + ".CORRUPTED");
                    }
                    else
                    {
                        if (!Path.HasExtension(filePath))
                        {
                            if (text.StartsWith("DDS |"))
                            {
                                if (!File.Exists(newFilePath + ".dds"))
                                    File.Move(filePath, newFilePath + ".dds");
                            }
                            else if (text.StartsWith("LuaQ"))
                            {
                                if (!File.Exists(newFilePath + ".luac"))
                                    File.Move(filePath, newFilePath + ".luac");
                            }
                            else if (text.StartsWith("HM") || text.StartsWith("MR04"))
                            {
                                if (!File.Exists(newFilePath + ".mdl"))
                                    File.Move(filePath, newFilePath + ".mdl");
                            }
                            else if (text.StartsWith("‰PNG") || text.Contains("Photoshop ICC profile") || text.Contains("IHDR"))
                            {
                                if (!File.Exists(newFilePath + ".png"))
                                    File.Move(filePath, newFilePath + ".png");
                            }
                            else if (text.StartsWith("WW") || text.Contains("Havok-5.0.0-r1"))
                            {
                                if (!File.Exists(newFilePath + ".hkx"))
                                    File.Move(filePath, newFilePath + ".hkx");
                            }
                            else if (text.StartsWith("AC11"))
                            {
                                if (!File.Exists(newFilePath + ".ani"))
                                    File.Move(filePath, newFilePath + ".ani");
                            }
                            else if (text.StartsWith("SK08"))
                            {
                                if (!File.Exists(newFilePath + ".skn"))
                                    File.Move(filePath, newFilePath + ".skn");
                            }
                            else if (text.Contains("CHNK"))
                            {
                                if (!File.Exists(newFilePath + ".effect"))
                                    File.Move(filePath, newFilePath + ".effect");
                            }
                            else if (text.Contains("LoadLibrary") || text.Contains("function"))
                            {
                                if (!File.Exists(newFilePath + ".lua"))
                                    File.Move(filePath, newFilePath + ".lua");
                            }
                            else if (text.Contains("klBS"))
                            {
                                if (!File.Exists(newFilePath + ".bnk"))
                                    File.Move(filePath, newFilePath + ".bnk");
                            }
                            else if (text.Contains("LAME3.") || text.Contains("SfMarkers"))
                            {
                                if (!File.Exists(newFilePath + ".mp3"))
                                    File.Move(filePath, newFilePath + ".mp3");
                            }
                            else if (text.Contains("ftypmp42"))
                            {
                                if (!File.Exists(newFilePath + ".mp4"))
                                    File.Move(filePath, newFilePath + ".mp4");
                            }
                            else if (text.Contains("DSIG"))
                            {
                                if (!File.Exists(newFilePath + ".ttf"))
                                    File.Move(filePath, newFilePath + ".ttf");
                            }
                            else if (text.Contains("<gap:game"))
                            {
                                if (!File.Exists(newFilePath + ".scene"))
                                    File.Move(filePath, newFilePath + ".scene");
                            }
                            else if (text.Contains("</") || text.Contains("/>"))
                            {
                                if (!File.Exists(newFilePath + ".xml"))
                                    File.Move(filePath, newFilePath + ".xml");
                            }
                            else
                            {
                                if (!File.Exists(newFilePath + ".unknown"))
                                    File.Move(filePath, newFilePath + ".unknown");
                            }
                        }
                        else if (!File.Exists(newFilePath))
                            File.Move(filePath, newFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Mapper] - An Error happened in MapperPrepareFiles - {ex}");
            }
        }

        private void CopyHelperFiles(string sourceDir, string targetDir)
        {
            if (string.IsNullOrEmpty(sourceDir) || !Directory.Exists(sourceDir))
                return;

            try
            {
                foreach (string file in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
                {
#if NET5_0_OR_GREATER
                    string targetPath = Path.Combine(targetDir, Path.GetRelativePath(sourceDir, file));
#else
                    string targetPath = Path.Combine(targetDir, GetRelativePath(sourceDir, file));
#endif
                    string directorytargetPath = Path.GetDirectoryName(targetPath);

                    if (!string.IsNullOrEmpty(directorytargetPath))
                    {
                        Directory.CreateDirectory(directorytargetPath);

                        File.Copy(file, targetPath, true);

                        filesToDelete.Add(targetPath);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Mapper] - An Error happened in CopyHelperFiles - {ex}");
            }
        }

#if !NET5_0_OR_GREATER
        private static string GetRelativePath(string sourceDir, string file)
        {
            return Uri.UnescapeDataString(new Uri(sourceDir.TrimEnd(Path.DirectorySeparatorChar)
                + Path.DirectorySeparatorChar).MakeRelativeUri(new Uri(file)).ToString()).Replace('/', Path.DirectorySeparatorChar);
        }
#endif
#if NET7_0_OR_GREATER
        [GeneratedRegex("[0-9a-fA-F]{8}-[0-9a-fA-F]{8}-[0-9a-fA-F]{8}-[0-9a-fA-F]{8}")]
        private static partial Regex UUIDRegex();
#endif
    }
}
