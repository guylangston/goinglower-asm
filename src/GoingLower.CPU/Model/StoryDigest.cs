using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GoingLower.CPU.Model
{
    public record StoryDigest  // Json Serialisable
    {
        public string Id            { get; set; }
        public string Title         { get; set; }
        public string DescText      { get; set; }
        public string DescFormat    { get; set; }
        public string Arch          { get; set; }
        public string OS            { get; set; }
        public string IconPath      { get; set; }
        public string ThumbnailPath { get; set; }
        public string CompileRoot   { get; set; }

        public string Author     { get; set; }
        public string AuthorUrl { get; set; }
        public string License    { get; set; }
        public string ProjectUrl { get; set; }
        
        public DateTime Created     { get; set; }  // Utc
        public DateTime Modified    { get; set; } // Utc
        public int      Version     { get; set; }

        public Dictionary<string, string> Props { get; set; }
    }

    
    public class StoryDigestRepo
    {
        private readonly JsonSerializerOptions options = new JsonSerializerOptions()
        {
            WriteIndented    = true,
            IgnoreNullValues = false,

        };

        public StoryDigest Load(string file)
        {
            // TODO: Async
            return JsonSerializer.Deserialize<StoryDigest>(File.ReadAllText(file), options);
        }

        public void Write(StoryDigest digest, string file)
        {
            using FileStream openStream = File.OpenWrite(file);
            JsonSerializer.Serialize(new Utf8JsonWriter(openStream), digest, options);
        }
    }
}