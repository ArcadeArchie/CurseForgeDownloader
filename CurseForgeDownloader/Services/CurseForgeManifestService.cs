using CurseForgeDownloader.Config;
using CurseForgeDownloader.Exceptions;
using CurseForgeDownloader.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace CurseForgeDownloader.Services
{
    internal class CurseForgeManifestService
    {
        private readonly CurseFileService _curseFileService;
        private readonly HttpClient _httpClient;

        public CurseForgeManifestService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CurseApi");
            _curseFileService = new(_httpClient);
        }

        #region Methods

        /// <summary>
        /// Retrieve and parse the manifes.json
        /// </summary>
        /// <param name="path">Path to the manifest.json</param>
        /// <returns></returns>
        internal async Task<CurseForgeManifest?> RetrieveManifest(string path, bool retrievModInfos = false)
        {
            var manifest = path.EndsWith(".zip") ? await RetrieveManifestFromZip(path) : await RetrieveManifestFromJson(path);

            if(retrievModInfos)
            {
                //TODO: load mod info
            }
            return manifest;
        }

        /// <summary>
        /// Create modpack folder from given manifest.json
        /// </summary>
        /// <param name="currentManifest">The modpack manifest</param>
        /// <param name="outputPath">The output directory</param>
        /// <returns></returns>
        internal async Task CreatePack(CurseForgeManifest currentManifest, string outputPath)
        {
            var packPath = Path.Combine(outputPath, currentManifest.Name!);

            ExtractConfigs(currentManifest.FilePath, packPath);

            await ExtractMods(currentManifest, Path.Combine(packPath, "mods"));
        }
        
        /// <summary>
        /// Download all mods from the given Manifest instance to the output directory
        /// </summary>
        /// <param name="currentManifest"></param>
        /// <param name="outputPath">The output directory</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">When the given Manifest doesnt contain any mods to download</exception>
        /// <exception cref="DownloadFailedException">Failed to retrieve download URls from CurseForge API</exception>
        internal async Task ExtractMods(CurseForgeManifest currentManifest, string outputPath)
        {
            //Check if the output dir exists if not create it
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            if (currentManifest.Files == null)
                throw new InvalidOperationException("There are no files to download");

            var files = await _curseFileService.GetFilesAsync(currentManifest.Files);

            //loop thru the parsed response and download each file to output dir
            foreach (var curseFile in files)
            {
                await _curseFileService.DownloadFileAsync(curseFile, outputPath);
            }
        }
        
        #region Util

        /// <summary>
        /// Parse a given manifest.json
        /// </summary>
        /// <param name="jsonFile">Manifest.json data</param>
        /// <returns>Parsed Manifest</returns>
        private static async Task<CurseForgeManifest?> RetrieveManifestFromJson(string path)
        {
            using var jsonFile = File.OpenRead(path);

            return await RetrieveManifestFromJsonInternal(jsonFile);
        }

        /// <summary>
        /// Parse a given manifest.json
        /// </summary>
        /// <param name="jsonFile">Manifest.json data</param>
        /// <returns>Parsed Manifest</returns>
        private static async Task<CurseForgeManifest?> RetrieveManifestFromJsonInternal(Stream jsonFile)
        {
            return await JsonSerializer.DeserializeAsync<CurseForgeManifest>(jsonFile);
        }

        /// <summary>
        /// Retreive and Parse a manifest.json contained in a .zip file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static async Task<CurseForgeManifest?> RetrieveManifestFromZip(string path)
        {
            //open the zip and look for the manifest.json
            using var zipFile = File.OpenRead(path);
            using var archive = new ZipArchive(zipFile, ZipArchiveMode.Read);
            var manifestEntry = archive.Entries.FirstOrDefault(x => x.Name == "manifest.json");
            //if no json is present return null
            if (manifestEntry == null)
                return null;

            using var jsonStream = manifestEntry.Open();
            var res = await RetrieveManifestFromJsonInternal(jsonStream);
            //set some flags for later use
            if (res != null)
            {
                res.FilePath = path;
                res.FromZip = true;
            }

            return res;
        }
    
        /// <summary>
        /// Extract configs from Modpack zip
        /// </summary>
        /// <param name="zipPath">Path to the Modpack zip</param>
        /// <param name="packPath">Output path</param>
        private static void ExtractConfigs(string zipPath, string packPath)
        {
            using var zipFile = File.OpenRead(zipPath);
            using var archive = new ZipArchive(zipFile, ZipArchiveMode.Read);
            //get all folders
            var folders = archive.Entries.Where(x => string.IsNullOrWhiteSpace(x.Name));
            //get all files except manifest and modlist
            var files = archive.Entries.Where(x => !string.IsNullOrWhiteSpace(x.Name) && x.Name != "manifest.json" && x.Name != "modlist.html");

            //create folders
            foreach (var folder in folders)
            {
                var path = folder.FullName.Remove(0, 10);
                if (string.IsNullOrWhiteSpace(path))
                    continue;
                var dir = Path.Combine(packPath, path);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }
            //extract files
            foreach (var file in files)
            {
                string destFileName = Path.GetFullPath(Path.Combine(packPath, file.FullName.Remove(0, 10)));
                string outputPath = Path.GetFullPath(packPath + Path.DirectorySeparatorChar);
                if (!destFileName.StartsWith(outputPath))
                    throw new InvalidOperationException("Entry outside of target directory");
                file.ExtractToFile(destFileName);
            }
        }

        #endregion

        #endregion
    }
}
