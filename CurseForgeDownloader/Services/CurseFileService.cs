using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Web;
using CurseForgeDownloader.Config;
using CurseForgeDownloader.Exceptions;
using CurseForgeDownloader.Models;
using Microsoft.Extensions.Options;

namespace CurseForgeDownloader.Services;

/// <summary>
/// Helper Service for interacting with the CurseForge API for file downloads and info
/// </summary>
internal class CurseFileService
{
    private readonly HttpClient _httpClient;
    public CurseFileService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Retrieve Mod File info from the CurseForge API for a given set of <see cref="CurseManifestFile"/>
    /// </summary>
    /// <param name="files">Set of file IDs to send to the API</param>
    /// <exception cref="DownloadFailedException">Failed to retrieve file infos from API</exception>
    internal async Task<IEnumerable<CurseFile>> GetFilesAsync(IEnumerable<CurseManifestFile> files)
    {
        //retrieve download URLs from API
        var res = await _httpClient.PostAsJsonAsync("/v1/mods/files", new CurseFilesRequest
        {
            FileIds = files.Select(x => x.FileID)
        });

        if(res.StatusCode == System.Net.HttpStatusCode.Forbidden)
            throw new InvalidOperationException("Recieved forbidden response, API may be invalid");

        //if no URLs were retrieved throw and bail
        if (!res.IsSuccessStatusCode)
            throw new DownloadFailedException("", "Failed to retrieve download URLs");

        var data = await res.Content.ReadFromJsonAsync<CurseFilesResponse>();

        return data!.Data;
    }
    
    /// <summary>
    /// Download a Mod file to given folder path
    /// </summary>
    /// <param name="curseFile">Info about the mod file</param>
    /// <param name="outputPath">output folder path</param>
    internal async Task DownloadFileAsync(CurseFile curseFile, string outputPath)
    {
        string? url = curseFile.DownloadUrl;
        if (string.IsNullOrEmpty(url)) // build direct url if the api response didnt give us one
            url = GetDownloadUrl(curseFile);
        var file = await _httpClient.GetAsync(url);
        if (!file.IsSuccessStatusCode)
            return;
        string jar = Path.Combine(outputPath, curseFile.FileName);
        //if we got duplicates delete the old one
        if (File.Exists(jar))
            File.Delete(jar);
        using var fs = new FileStream(jar, FileMode.CreateNew);
        await file.Content.CopyToAsync(fs);
    }

    /// <summary>
    /// Build the direct download url for a Given <see cref="CurseFile"/>
    /// </summary>
    /// <param name="curseFile"></param>
    /// <returns>Direct download URL as a string</returns>
    /// <remarks>F U CurseForge</remarks>
    private static string GetDownloadUrl(CurseFile curseFile)
    {
        var strId = curseFile.Id.ToString();
        var part = strId[..4];
        var part2 = strId[^3..];
        return $"https://mediafilez.forgecdn.net/files/{part}/{part2}/{HttpUtility.UrlEncode(curseFile.FileName)}";
    }
}