using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DonwloaderLogics
{
    public class Processor
    {
        private int _depth;

        private HashSet<string> SavedLinks { get; set; }

        private List<string> CurrentLevelLinks { get; set; }

        private List<string> NextLevelLinks { get; set; }

        private Processor()
        {
            SavedLinks = new HashSet<string>();

            CurrentLevelLinks = new List<string>();

            NextLevelLinks = new List<string>();
        }

        public Processor(string url) : this() => CurrentLevelLinks.Add(url);

        public Processor(string url, int depth) : this(url) => _depth = depth;

        public async Task Download()
        {
            await FormLinks();
            await LoadPagesAndResources();
        }

        private async Task LoadPagesAndResources()
        {
            foreach (var link in SavedLinks)
            {
                string currentPath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

                PageParser parser = new PageParser();
                await parser.ParsePage(link);

                var linksOnPages = await parser.GetLinksOnPages();
                var linksOnResources = await parser.GetLinksOnResources();

                PageHandler pageHandler = new PageHandler(currentPath);
                await pageHandler.HandlePage(link);

                ResourceHandler resourceHandler = new ResourceHandler(currentPath, link);
                await resourceHandler.HandleResources(linksOnResources);
            }
        }

        private async Task FormLinks()
        {
            int level = 0;

            PageParser parser = new PageParser();

            while (level <= _depth)
            {
                await AddToSet(CurrentLevelLinks);

                foreach (var currentLink in CurrentLevelLinks)
                {
                    await parser.ParsePage(currentLink);
                    var linksOnPages = await parser.GetLinksOnPages();

                    NextLevelLinks.AddRange(linksOnPages);
                }

                await Swap();
                level++;
            }
        }

        private async Task AddToSet(List<string> linksOnPages)
        {
            await Task.Run(() =>
            {
                foreach (var link in linksOnPages)
                {
                    SavedLinks.Add(link);
                }
            });
        }

        private async Task Swap()
        {
            await Task.Run(() =>
            {
                CurrentLevelLinks.Clear();
                CurrentLevelLinks.AddRange(NextLevelLinks);
                NextLevelLinks.Clear();
            });
        }
    }
}
