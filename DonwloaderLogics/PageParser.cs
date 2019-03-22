using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;

namespace DonwloaderLogics
{
    public class PageParser
    { 
        private List<string> TagsHref { get; set; }

        private List<string> TagsSrc { get; set; }

        private List<string> Separators { get; set; }

        private List<string> References { get; set; }

        public PageParser()
        {
            Separators = new List<string>() { "htm", "html", "php" };

            TagsHref = new List<string>() { "a", "link", "script" };

            TagsSrc = new List<string>() { "img" };

            References = new List<string>();
        }

        public async Task<List<string>> GetLinksOnPages()
        {
            return await Task.Run(async () =>
            {
                List<string> pages = new List<string>();

                foreach (var reference in References)
                {
                    var parts = reference.Split('/');

                    var isPage = await IsPageLink(reference);

                    if (isPage)
                    {
                        pages.Add(reference);
                    }
                }

                return pages;
            });
        }

        public async Task<List<string>> GetLinksOnResources()
        {
            return await Task.Run(async () =>
            {
                List<string> resources = new List<string>();

                foreach (var reference in References)
                {
                    var parts = reference.Split('/');

                    var isPage = await IsPageLink(reference);

                    if (!isPage)
                    {
                        resources.Add(reference);
                    }
                }

                return resources;
            });
        }

        public async Task ParsePage(string url)
        {
            var context = BrowsingContext.New(Configuration.Default.WithDefaultLoader());

            var document = await context.OpenAsync(url);

            await HandleHref(document, References);

            await HandleSrc(document, References);

            References.RemoveAll((row) => string.IsNullOrEmpty(row));
        }

        private async Task<bool> IsPageLink(string link)
        {
            return await Task.Run(() =>
            {
                var parts = link.Split('/');

                if (!parts[parts.Length - 1].Contains("."))
                {
                    return true;
                }

                foreach (var separator in Separators)
                {
                    if (parts[parts.Length - 1].Contains("." + separator))
                    {
                        return true;
                    }
                }

                return false;
            });
        }

        private async Task HandleHref(IDocument document, List<string> references)
        {
            await Task.Run(() =>
            {
                foreach (var tag in TagsHref)
                {
                    var links = document.QuerySelectorAll(tag);

                    references.AddRange(links.Select(l => l.GetAttribute("href")).ToList());
                }
            });
        }

        private async Task HandleSrc(IDocument document, List<string> references)
        {
            await Task.Run(() =>
            {
                foreach (var tag in TagsSrc)
                {
                    var links = document.QuerySelectorAll(tag);

                    references.AddRange(links.Select(l => l.GetAttribute("src")).ToList());
                }
            });
        }
    }
}
