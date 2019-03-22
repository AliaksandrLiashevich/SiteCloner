using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DonwloaderLogics
{
    public class ResourceHandler
    {
        private string _domainPath;

        private string _pageUrl;

        private string directory = "resources";

        public ResourceHandler(string domainPath, string pageUrl)
        {
            var parts = pageUrl.Split('/');

            _domainPath = domainPath + "\\" + parts[parts.Length - 1] + "\\" + directory;

            _pageUrl = pageUrl;
        }

        public async Task HandleResources(List<string> linksOfResources)
        {
            foreach (var resourceLink in linksOfResources)
            {
                var bytes = await LoadResource(resourceLink);

                var parts = resourceLink.Split('/');

                await SaveResource(parts[parts.Length - 1], bytes);
            }
        }

        public async Task<byte[]> LoadResource(string resourceUrl)
        {
            HttpClient client = new HttpClient();

            try
            {
                var responce = await client.GetAsync(_pageUrl + resourceUrl);
                var stream = await responce.Content.ReadAsByteArrayAsync();

                return stream;
            }
            catch (InvalidOperationException exception)
            {
                return null;
            }
        }

        public async Task SaveResource(string name, byte[] resource)
        {
            Directory.CreateDirectory(_domainPath);

            try
            {
                await Task.Run(() => File.WriteAllBytes(_domainPath + "\\" + name, resource));
            }
            catch (ArgumentNullException exception)
            { }
        }
    }
}
