using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

class Anticheat
{
    public enum EPlayStatus { Corrupted, Playable }

    public static EPlayStatus CheckForCorruption(string GamePath)
    {
        try
        {
            var ContentPath = Path.Combine(GamePath, "FortniteGame", "Content", "Paks");
            if (!Directory.Exists(ContentPath))
            {
                DialogService.ShowSimpleDialog(string.Empty, "Corrupted Data Detected");
                return EPlayStatus.Corrupted;
            }

            var MissingFiles = GetAllowedContentFiles().Where(File => !Directory.GetFiles(ContentPath).Select(Path.GetFileName).ToHashSet(StringComparer.OrdinalIgnoreCase).Contains(File)).ToList();
            if (MissingFiles.Any())
            {
                DialogService.ShowSimpleDialog(string.Empty, "Corrupted Data Detected");
                return EPlayStatus.Corrupted;
            }

            return EPlayStatus.Playable;
        }
        catch (Exception Error)
        {
            DialogService.ShowSimpleDialog(Error.ToString(), "CheckForCorruption");
            return EPlayStatus.Corrupted;
        }
    }

    private static HashSet<string> GetAllowedContentFiles()
    {
        var ContentFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var Extensions = new[] { ".pak", ".sig", ".ucas", ".utoc" };

        ContentFiles.Add("global.ucas");
        ContentFiles.Add("global.utoc");

        foreach (var Extension in Extensions)
        {
            ContentFiles.Add($"pakChunkEarly-WindowsClient{Extension}");
        }

        foreach (var Extension in Extensions)
        {
            ContentFiles.Add($"pakchunkEon-WindowsClient_p{Extension}");

            if (GlobalSettings.Options.IsBubbleBuildsEnabled)
            {
                ContentFiles.Add($"pakchunkLowMesh-WindowsClient_p{Extension}");
            }
        }

        var MainChunkIds = new[] { "0", "2", "5", "7", "8", "9" };
        foreach (var ChunkId in MainChunkIds)
        {
            foreach (var Extension in Extensions)
            {
                ContentFiles.Add($"pakchunk{ChunkId}-WindowsClient{Extension}");
                ContentFiles.Add($"pakchunk{ChunkId}optional-WindowsClient{Extension}");
            }
        }

        foreach (var Extension in Extensions)
        {
            ContentFiles.Add($"pakchunk10-WindowsClient{Extension}");
            ContentFiles.Add($"pakchunk10optional-WindowsClient{Extension}");
        }

        for (int SubChunk = 1; SubChunk <= 29; SubChunk++)
        {
            foreach (var Extension in Extensions)
            {
                ContentFiles.Add($"pakchunk10_s{SubChunk}-WindowsClient{Extension}");
                ContentFiles.Add($"pakchunk10_s{SubChunk}optional-WindowsClient{Extension}");
            }
        }

        foreach (var Extension in Extensions)
        {
            ContentFiles.Add($"pakchunk11-WindowsClient{Extension}");
            ContentFiles.Add($"pakchunk11optional-WindowsClient{Extension}");
        }

        foreach (var Extension in Extensions)
        {
            ContentFiles.Add($"pakchunk11_s1-WindowsClient{Extension}");
            ContentFiles.Add($"pakchunk11_s1optional-WindowsClient{Extension}");
        }

        var HighChunkIds = new[]
        {
            "1000", "1001", "1002", "1003", "1004", "1005", "1006",
            "1007", "1008", "1009", "1010", "1011", "1012", "1013", "1014"
        };

        var OptionalHighChunkIds = new HashSet<string>
        {
            "1002", "1004", "1007", "1009", "1010", "1011", "1012", "1013", "1014"
        };

        foreach (var ChunkId in HighChunkIds)
        {
            foreach (var Extension in Extensions)
            {
                ContentFiles.Add($"pakchunk{ChunkId}-WindowsClient{Extension}");

                if (OptionalHighChunkIds.Contains(ChunkId))
                {
                    ContentFiles.Add($"pakchunk{ChunkId}optional-WindowsClient{Extension}");
                }
            }
        }

        return ContentFiles;
    }
}