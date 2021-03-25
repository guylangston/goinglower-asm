using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GoingLower.Core.CMS
{
    public abstract class FileSystemRepo : IReadOnlyContentRepo
    {
        protected readonly string baseDir;

        protected FileSystemRepo(string baseDir)
        {
            this.baseDir = baseDir;
        }

        protected abstract string MapIdToFile(string id);
        protected abstract string MapFileToId(string file);
        
        protected abstract Task<IContent> LoadFromFile(string id, string file);
        protected abstract Task<ContentSummary> LoadSummary(string id, string file);
        protected abstract Task<ExtendedContentSummary> LoadSummaryExt(string id, string file);

        public Task<IContent> Load(string id)
        {
            var f = MapIdToFile(id);
            if (!File.Exists(f)) throw new Exception($"No Found: {id}");

            return LoadFromFile(id, f);
        }


        public async IAsyncEnumerable<ContentSummary> LoadAll()
        {
            foreach (var file in Directory.EnumerateFiles(baseDir))
            {
                var id = MapFileToId(file);
                if (id != null)
                {
                    yield return await LoadSummary(id, file);
                }
            }
        }
    }
    
    
    public class SimpleDirectoryBound : FileSystemRepo
    {
        public SimpleDirectoryBound(string baseDir) : base(baseDir)
        {
        }

        protected override string MapIdToFile(string id)
        {
            foreach (var file in Directory.GetFiles(baseDir, $"{id}.*"))
            {
                return Path.GetRelativePath(baseDir, file);
            }
            throw new Exception($"No Found: {id}");
        }

        protected override string MapFileToId(string file)
        {
            return Path.GetFileNameWithoutExtension(file);
        }

        protected override async Task<IContent> LoadFromFile(string id, string file)
        {
            var sum = await LoadSummaryExt(id, file);
            
            return new Content(sum, null);
        }

        protected override async Task<ContentSummary> LoadSummary(string id, string file)
        {
            throw new NotImplementedException();
        }

        protected override Task<ExtendedContentSummary> LoadSummaryExt(string id, string file)
        {
            throw new NotImplementedException();
        }
    }
}