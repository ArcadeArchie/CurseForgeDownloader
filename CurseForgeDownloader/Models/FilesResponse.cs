using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurseForgeDownloader.Models
{
    internal class CurseFilesResponse
    {
        public IEnumerable<CurseFile> Data { get; set; }
    }
    internal class CurseFilesRequest
    {
        public IEnumerable<int> FileIds { get; set; }
    }
    internal class CurseFile
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string FileName { get; set; }
        public string DownloadUrl { get; set; }
    }
}