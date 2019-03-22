using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DonwloaderLogics;

namespace Downloader
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string url = "https://metanit.com";

            Processor processor = new Processor(url, 0);

            processor.Download().Wait();

            Console.WriteLine("The work is done");

            Console.ReadKey();
        }
    }
}
