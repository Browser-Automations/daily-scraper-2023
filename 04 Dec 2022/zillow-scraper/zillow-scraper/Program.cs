using CsvHelper;
using HtmlAgilityPack;
using PuppeteerExtraSharp;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using PuppeteerExtraSharp6;
using PuppeteerExtraSharp6.Plugins.ExtraStealth;
using PuppeteerSharp;
using PuppeteerSharp.Input;
using System.Globalization;
using System.Web;
using zillow_scraper;

List<DataModel> entries = new List<DataModel>();

#region Main
try
{
    Console.WriteLine("Please enter search term:");
    string query = Console.ReadLine();
    await StartScraping(query);
    await Export();
}
catch (Exception ex)
{
    Console.WriteLine("Something happened. Reason: " + ex.Message);
}
Console.WriteLine("Operation has been completed. Press any key to exit");
Console.ReadKey();
#endregion
async Task StartScraping(string query)
{
    Console.WriteLine("Getting ready...");
    var extra = new PuppeteerExtra();
    extra.Use(new StealthPlugin());
    //using var browserFetcher = new BrowserFetcher();
    //await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
    var browser = await extra.LaunchAsync(new LaunchOptions
    {
        Headless = false,
        DefaultViewport = null,
        ExecutablePath = "/Users/fahadaltaf/Downloads/chromedriver 2",
    });
    var page = (await browser.PagesAsync())[0];


    try
    {
        Console.WriteLine("https://www.zillow.com/homes/");
        await page.GoToAsync("https://www.zillow.com/homes/");
        await page.WaitForTimeoutAsync(3000);
        await page.WaitForNavigationAsync();
        //Have to navigate solw otherwise website will not display any data
         
        await page.ClickAsync("input[type='text']",new ClickOptions { ClickCount = 3 });
        await page.TypeAsync("input[type='text']", query, new PuppeteerSharp.Input.TypeOptions { Delay = 200 });
        await page.Keyboard.PressAsync("Enter");
        await page.WaitForNavigationAsync();
        //await page.WaitForTimeoutAsync(5000);

        int pages = 1;
        for (int i = 1; i <= pages; i++)
        {
            var html = await page.GetContentAsync();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            if (i == 1)
            {
                var totalCountNode = doc.DocumentNode.SelectSingleNode("//*[@id=\"resultsColumn\"]/div[1]/div/div[2]/div/div/div/div[2]/h2");
                if (totalCountNode != null)
                {
                    int count = Convert.ToInt32(HttpUtility.HtmlDecode(totalCountNode.InnerText).Replace("homes", "").Trim());

                    //Website is displaying 40 results per page
                    pages = ((count - 1) / 40) + 1;
                    Console.WriteLine($"Total Results: {count} and Total Pages: {pages}");
                }
            }

            Console.WriteLine($"Extracting data from page {i}");
            var listNode = doc.DocumentNode.SelectSingleNode("//ul[@data-testid='search-result-list-container']");
            if (listNode != null)
            {
                foreach (var item in listNode.ChildNodes.Where(x => x.Name == "li"))
                {
                    //try
                    //{
                    //    DataModel entry = new DataModel();
                    //    HtmlDocument recordDoc = new HtmlDocument();
                    //    recordDoc.LoadHtml(item.InnerHtml);
                    //    var propUrl = recordDoc.DocumentNode.SelectNodes("//a").FirstOrDefault();
                    //    if (propUrl != null)
                    //    {
                    //        entry.PropertyUrl = "https://www.trulia.com" + propUrl.Attributes["href"].Value;
                    //    }

                    //    var priceNode = recordDoc.DocumentNode.SelectSingleNode("//div[@data-testid='property-price']");
                    //    if (priceNode != null)
                    //    {
                    //        entry.Price = HttpUtility.HtmlDecode(priceNode.InnerText);
                    //    }

                    //    var bedsNode = recordDoc.DocumentNode.SelectSingleNode("//div[@data-testid='property-beds']");
                    //    if (bedsNode != null)
                    //    {
                    //        entry.Beds = HttpUtility.HtmlDecode(bedsNode.InnerText);
                    //    }

                    //    var bathNode = recordDoc.DocumentNode.SelectSingleNode("//div[@data-testid='property-baths']");
                    //    if (bathNode != null)
                    //    {
                    //        entry.Baths = HttpUtility.HtmlDecode(bathNode.InnerText);
                    //    }

                    //    var areaNode = recordDoc.DocumentNode.SelectSingleNode("//div[@data-testid='property-floorSpace']");
                    //    if (areaNode != null)
                    //    {
                    //        entry.Area = HttpUtility.HtmlDecode(areaNode.InnerText);
                    //    }

                    //    var addressNode = recordDoc.DocumentNode.SelectSingleNode("//div[@data-testid='property-address']");
                    //    if (addressNode != null)
                    //    {
                    //        entry.Address = HttpUtility.HtmlDecode(addressNode.InnerText);
                    //    }

                    //    var listedByNode = recordDoc.DocumentNode.SelectSingleNode("//div[@data-testid='property-card-listing-attribution-block']");
                    //    if (listedByNode != null)
                    //    {
                    //        entry.ListedBy = HttpUtility.HtmlDecode(listedByNode.InnerText);
                    //    }

                    //    var imagesNodes = recordDoc.DocumentNode.SelectNodes("//img");
                    //    if (imagesNodes.Count > 0)
                    //    {
                    //        List<string> images = new List<string>();
                    //        foreach (var node in imagesNodes)
                    //        {
                    //            var imgUrl = node.Attributes["src"].Value.Replace("thumbs_3", "thumbs_4");
                    //            if (!images.Exists(x => x == imgUrl))
                    //                images.Add(imgUrl);
                    //        }
                    //        entry.Images = string.Join(";", images);
                    //    }
                    //    entries.Add(entry);
                    //    Console.WriteLine(entry.Address);
                    //}
                    //catch (Exception ex)
                    //{
                    //    Console.WriteLine("Unable to extract details of a record. Reason: " + ex.Message);
                    //}

                }
            }

            if (i + 1 <= pages)
            {
                await page.WaitForTimeoutAsync(3000);
                Console.WriteLine("Loading next page...");
                var paginationBtns = doc.DocumentNode.SelectNodes("//li[@data-testid='pagination-page-link']");
                if (paginationBtns.Count > 0)
                {
                    var btnToClick = paginationBtns.FirstOrDefault(x => x.InnerText == (i + 1).ToString());
                    if (btnToClick != null)
                    {
                        await (await page.XPathAsync(btnToClick.XPath))[0].ClickAsync();
                        await page.WaitForTimeoutAsync(5000);
                    }
                }
            }
        }


    }
    catch (Exception ex)
    {
        Console.WriteLine("Unable to continue. Reason: " + ex.Message);
    }

    await browser.CloseAsync();
}

async Task Export()
{
    using (var writer = new StreamWriter(DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv"))
    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
    {
        await csv.WriteRecordsAsync(entries);
    }
}
