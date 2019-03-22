using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DonwloaderLogics
{
    public class PageHandler
    {
        private string _domainPath;

        private string defaultExtension;

        private List<string> Extensions;
               
        public PageHandler(string domainPath)
        {
            _domainPath = domainPath;

            defaultExtension = ".html";

            Extensions = new List<string>() { "htm", "html", "php" };
        }

        public async Task HandlePage(string pageLink)
        {
            var bytes = await LoadPage(pageLink);

            var parts = pageLink.Split('/');

            _domainPath = _domainPath + "\\" + parts[parts.Length - 1];

            var pageName = await AddPageExtension(parts[parts.Length - 1]);

            await SavePage(pageName, bytes);
        }

        public async Task HandlePages(List<string> linksOfPages)
        {
            foreach (var pageLink in linksOfPages)
            {
                await HandlePage(pageLink);
            }
        }

        public async Task<byte[]> LoadPage(string url)
        {
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                "AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.121 Safari/537.36");

            try
            {
                var responce = await client.GetAsync(url);
                var stream = await responce.Content.ReadAsByteArrayAsync();

                return stream;
            }
            catch (InvalidOperationException exception)
            {
                return null;
            }
        }

        public async Task SavePage(string name, byte[] resource)
        {
            await Task.Run(() => 
            {
                Directory.CreateDirectory(_domainPath);

                try
                {
                    File.WriteAllBytes(_domainPath + "\\" + name, resource);
                }
                catch (ArgumentNullException exception)
                { }
            });
        }

        private async Task<string> AddPageExtension(string pageName)
        {
            var result = await IsPageExtensionExists(pageName);

            if (!result)
            {
                return pageName + defaultExtension;
            }

            return pageName;
        }

        private async Task<bool> IsPageExtensionExists(string pageName)
        {
            return await Task.Run(() =>
            {
                var parts = pageName.Split('/');

                foreach (var extension in Extensions)
                {
                    if (parts[parts.Length - 1].EndsWith(extension))
                    {
                        return true;
                    }
                }

                return false;
            });
        }
    }
}
