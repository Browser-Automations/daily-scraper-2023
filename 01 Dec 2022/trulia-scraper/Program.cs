using PuppeteerExtraSharp6;
using PuppeteerExtraSharp6.Plugins.ExtraStealth;
using PuppeteerSharp;
using HtmlAgilityPack;
using System.Web;

//var extra = new PuppeteerExtra();
//extra.Use(new StealthPlugin());
//using var browserFetcher = new BrowserFetcher();
//await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
//var browser = await extra.LaunchAsync(new LaunchOptions
//{
//    Headless = false
//});
//var page = (await browser.PagesAsync())[0] ;
//await page.GoToAsync("https://www.trulia.com/");
//
////Have to navigate solw otherwise website will not display any data
//await page.WaitForTimeoutAsync(3000);
//
////search for buy, rent or sold properties
//await page.ClickAsync("[data-testid='tab-button-buy']");
////await page.ClickAsync("[data-testid='tab-button-rent']");
////await page.ClickAsync("[data-testid='tab-button-sold']");
//
//await page.TypeAsync("[data-testid='location-search-input']", "33319",new PuppeteerSharp.Input.TypeOptions { Delay=200 });
//await page.WaitForTimeoutAsync(1500);
//await page.ClickAsync("[data-testid='location-search-button']");
//
//await page.WaitForSelectorAsync("#resultsColumn", new WaitForSelectorOptions { Timeout=30000 });
//await page.WaitForTimeoutAsync(1500);
//
//var html = await page.GetContentAsync();
var html =File.ReadAllText("data.txt");
HtmlDocument doc = new HtmlDocument();
doc.LoadHtml(html);

var totalCountNode = doc.DocumentNode.SelectSingleNode("//*[@id=\"resultsColumn\"]/div[1]/div/div[2]/div/div/div/div[2]/h2");
if (totalCountNode != null)
{
    Console.WriteLine("Total Results: "+HttpUtility.HtmlDecode(totalCountNode.InnerText));
}

var listNode = doc.DocumentNode.SelectSingleNode("//ul[@data-testid='search-result-list-container']");
if (listNode != null)
{
    foreach (var item in listNode.ChildNodes.Where(x=>x.Name=="li"))
    {
        HtmlDocument recordDoc = new HtmlDocument();
        recordDoc.LoadHtml(item.InnerHtml);

        var priceNode = doc.DocumentNode.SelectSingleNode("//div[data-testid='property-price']");
        if (priceNode != null)
        {

        }

        var bedsNode = doc.DocumentNode.SelectSingleNode("//div[data-testid='property-beds']");
        if (bedsNode != null)
        {

        }

        var bathNode = doc.DocumentNode.SelectSingleNode("//div[data-testid='property-baths']");
        if (bathNode != null)
        {

        }

        var areaNode = doc.DocumentNode.SelectSingleNode("//div[data-testid='property-floorSpace']");
        if (areaNode != null)
        {

        }

        var addressNode = doc.DocumentNode.SelectSingleNode("//div[data-testid='property-address']");
        if (addressNode != null)
        {

        }
    }
}


