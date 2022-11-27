using CurseForgeDownloader.Models;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace CurseForgeDownloader.Services
{
    internal class CurseForgeManifestService
    {
        private readonly HttpClient _httpClient;

        public CurseForgeManifestService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CurseApi");
        }


        public async Task<CurseForgeManifest?> RetrieveManifest(string path)
        {
            bool isZip = path.EndsWith(".zip");


            return isZip ? await RetrieveManifestFromZip(path) : await RetrieveManifestFromJson(path);
        }

        internal async Task CreatePack(CurseForgeManifest currentManifest, string outputPath)
        {
            var packPath = Path.Combine(outputPath, currentManifest.Name!);

            await ExtractMods(currentManifest, Path.Combine(packPath, "mods"));
            if (!currentManifest.FromZip) return;

            //TODO: Extract config files from Zip
        }

        internal async Task ExtractMods(CurseForgeManifest currentManifest, string outputPath)
        {
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            if (currentManifest.Files == null)
                throw new InvalidOperationException("There are no files to download");
            var res = await _httpClient.PostAsJsonAsync("/v1/mods/files", new CurseFilesRequest
            {
                FileIds = currentManifest.Files.Select(x => x.FileID)
            }, options: new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });


            if (!res.IsSuccessStatusCode)
                throw new Exception("Failed to retrieve download URLs");
            var files = await res.Content.ReadFromJsonAsync<CurseFilesResponse>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });

            foreach (var url in files!.Data.Select(x => (x.DownloadUrl, x.FileName)))
            {
                var file = await _httpClient.GetAsync(url.DownloadUrl);
                if (!file.IsSuccessStatusCode)
                    continue;
                string jar = Path.Combine(outputPath, url.FileName);
                if (File.Exists(jar))
                    File.Delete(jar);
                using var fs = new FileStream(jar, FileMode.CreateNew);
                await file.Content.CopyToAsync(fs);
            }
        }

        private async Task<CurseForgeManifest?> RetrieveManifestFromJson(string path)
        {
            using var jsonFile = File.OpenRead(path);

            return await JsonSerializer.DeserializeAsync<CurseForgeManifest>(jsonFile, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
        }

        private async Task<CurseForgeManifest?> RetrieveManifestFromZip(string path)
        {
            using var zipFile = File.OpenRead(path);
            using var archive = new ZipArchive(zipFile, ZipArchiveMode.Read);
            var manifestEntry = archive.Entries.FirstOrDefault(x => x.Name == "manifest.json");

            if (manifestEntry == null)
                return null;
            using var jsonStream = manifestEntry.Open();

            var res = await JsonSerializer.DeserializeAsync<CurseForgeManifest>(jsonStream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
            if (res != null)
            {
                res.FilePath = path;
                res.FromZip = true;
            }

            return res;
        }
    }
}
