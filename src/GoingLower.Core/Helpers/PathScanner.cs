using System;
using System.Collections.Generic;
using System.IO;

namespace GoingLower.Core.Helpers
{
    public class PathScanner
    {
        private List<string> pathHints = new List<string>();

        public PathScanner(IEnumerable<string> hints)
        {
            pathHints.AddRange(hints);
        }

        public string? ScanForFirstDirectoryExists()
        {
            foreach (var hint in pathHints)
            {
                if (Directory.Exists(hint))
                {
                    return hint;
                }
            }
            return null;
        }

        public bool TryScanFirstDirectoryExists(out string dir)
        {
            var s = ScanForFirstDirectoryExists();
            if (s != null)
            {
                dir = s;
                return true;
            }
            dir = default;
            return false;
        }

        public string ScanForFirstDirectoryExistsElseThrow()
        {
            var s = ScanForFirstDirectoryExists();
            if (s == null)
            {
                throw new Exception("Could not find any paths");
            }
            return s;
        }
    }
}