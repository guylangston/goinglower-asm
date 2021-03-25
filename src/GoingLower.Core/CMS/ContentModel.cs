using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoingLower.Core.CMS
{
    public class ContentSummary
    {
        public ContentSummary(string id, string contentType, string? title)
        {
            Id          = id;
            ContentType = contentType;
            Title       = title;
        }

        public string  Id          { get;  }
        public string  ContentType { get; }
        public string? Title       { get; }
    }

    public class ExtendedContentSummary : ContentSummary
    {
        public ExtendedContentSummary(string id, string contentType, string? title, 
            string? digest, IReadOnlyDictionary<string, object> props) : base(id, contentType, title)
        {
            Digest = digest;
            Props  = props;
        }

        public string?                             Digest { get;  }
        public IReadOnlyDictionary<string, object> Props  { get; }
    }
    
    public interface IContent
    {
        ExtendedContentSummary Header { get; }
        IContentBody           Body   { get; }
    }

    public enum BodyFormat
    {
        Binary,
        Text
    }

    public interface IContentBody
    {
        BodyFormat Format { get; }
        Task<byte[]> ReadAllBytes();
        Task<string> ReadAllText();
        IAsyncEnumerable<string> ReadLines();
    }

    public class ContentBodyTextLines : IContentBody
    {
        public ContentBodyTextLines(string[] lines)
        {
            this.lines = lines;
        }

        public BodyFormat Format => BodyFormat.Text;
        public Task<byte[]> ReadAllBytes()
        {
            throw new System.NotImplementedException();
        }

        public Task<string> ReadAllText()
        {
            throw new System.NotImplementedException();
        }

        private readonly string[] lines;

        public async IAsyncEnumerable<string> ReadLines()
        {
            foreach (var line in lines)
            {
                yield return line;
            }
        }
    }

    
    
    public class Content : IContent
    {
        public Content(ExtendedContentSummary header, IContentBody body)
        {
            Header = header;
            Body   = body;
        }

        public ExtendedContentSummary Header { get; }
        public IContentBody           Body   { get; }
    }
    
    public interface IReadOnlyContentRepo
    {
        Task<IContent> Load(string id);
        IAsyncEnumerable<ContentSummary> LoadAll();
    }
}