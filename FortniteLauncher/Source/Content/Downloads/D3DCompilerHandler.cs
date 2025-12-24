using System;
using System.IO;
using System.Threading.Tasks;

class D3DCompilerCheck
{
    const string FileName = "D3DCompiler_43.dll";
    public static async Task DownloadFile()
    {
        string DirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.System);

        if (!File.Exists(Path.Combine(DirectoryPath, FileName)))
        {
            await DownloadService.File($"{Definitions.CDN_URL}/{FileName}", DirectoryPath, FileName);
        }
    }
}