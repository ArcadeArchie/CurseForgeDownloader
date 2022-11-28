using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurseForgeDownloader.Exceptions
{
    internal class DownloadFailedException : Exception
    {
        public string ModName { get; init; }

        public DownloadFailedException(string modName, string message = "Failed to download mod") : base(message)
        {
            ModName = modName;
        }
    }
}
