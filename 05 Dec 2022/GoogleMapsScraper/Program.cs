using System.Web;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using PuppeteerExtraSharp6;
using PuppeteerExtraSharp6.Plugins.ExtraStealth;
using PuppeteerSharp;

var extra = new PuppeteerExtra();
extra.Use(new StealthPlugin());
using var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
var browser = await extra.LaunchAsync(new LaunchOptions
{
    Headless = false,
    DefaultViewport=null
});
var page = (await browser.PagesAsync())[0] ;
await page.GoToAsync("https://www.google.com/maps?hl=en");
await page.WaitForTimeoutAsync(3000);

await page.TypeAsync("#searchboxinput","Resturants in 33319",new PuppeteerSharp.Input.TypeOptions { Delay=250 });
await page.Keyboard.PressAsync("Enter");

await page.WaitForSelectorAsync("[role='feed']");
await page.WaitForTimeoutAsync(5000);

//for (int i = 0; i < 1000; i++)
//{
//    await page.EvaluateExpressionAsync("document.querySelector(\"[role='feed']\").scrollBy(0, window.innerHeight);");
//    await page.WaitForTimeoutAsync(5000);
//
//    HtmlDocument doc = new HtmlDocument();
//    doc.LoadHtml(await page.GetContentAsync());
//    var section = doc.DocumentNode.SelectSingleNode("//div[@role='feed']");
//    if (section != null)
//    {
//        if(section.ChildNodes.Count()>0 &&
//            section.ChildNodes.LastOrDefault().InnerText == "You've reached the end of the list.")
//        {
//            Console.WriteLine("Reached on the last page.");
//            break;
//        }
//    }
//}

//File.WriteAllText("file.txt", await page.GetContentAsync());
HtmlDocument doc = new HtmlDocument();
doc.LoadHtml(await page.GetContentAsync());
var section = doc.DocumentNode.SelectSingleNode("//div[@role='feed']");
if (section != null)
{
    if (section.ChildNodes.Count() > 0)
    {
        foreach (var item in section.ChildNodes)
        {
            if(!string.IsNullOrEmpty(item.InnerText.Trim()) && !item.InnerText.Trim().StartsWith("Price"))
            {
                await (await page.XPathAsync(item.XPath))[0].ClickAsync();
                await page.WaitForTimeoutAsync(5000);
                //HtmlDocument recordDoc = new HtmlDocument();
                //recordDoc.LoadHtml(item.ChildNodes[0].InnerHtml);
                //
                //var businessName = recordDoc.DocumentNode.SelectSingleNode("/div[2]/div[2]/div[1]/div[1]/div[1]/div[2]/div[1]");
                //if (businessName != null)
                //{
                //    Console.WriteLine(businessName.InnerText);
                //}
                //
                //var reviewsCount = recordDoc.DocumentNode.SelectSingleNode("/div[2]/div[2]/div[1]/div[1]/div[1]/div[2]/div[3]/div[1]/span[2]/span[2]/span[2]");
                //if (reviewsCount != null)
                //{
                //   Console.WriteLine(reviewsCount.InnerText);
                //}
                //
                //var RatingsCount = recordDoc.DocumentNode.SelectSingleNode("/div[2]/div[2]/div[1]/div[1]/div[1]/div[2]/div[3]/div[1]/span[2]/span[2]/span[1]");
                //if (RatingsCount != null)
                //{
                //    Console.WriteLine(RatingsCount.InnerText);
                //}
                //
                //var priceCategory = recordDoc.DocumentNode.SelectSingleNode("/div[2]/div[2]/div[1]/div[1]/div[1]/div[2]/div[3]/div[1]/span[3]/jsl[1]/span[2]");
                //if (priceCategory != null)
                //{
                //    Console.WriteLine(priceCategory.InnerText);
                //}
                //
                //Console.WriteLine("=====================");

                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(await page.GetContentAsync());

                var businessNameNode  = document.QuerySelector("h1 > span:nth-child(1)");
                if (businessNameNode != null)
                {
                    Console.WriteLine(HttpUtility.HtmlDecode(businessNameNode.InnerText).Trim());
                }

                var ratingNode = document.QuerySelector("span:nth-child(1) > span > span:nth-child(1)");
                if (ratingNode != null)
                {
                    Console.WriteLine(HttpUtility.HtmlDecode(ratingNode.InnerText).Trim());
                }

                var reviewsCountNode = document.QuerySelector("span:nth-child(2) > span:nth-child(1) > span");
                if (reviewsCountNode != null)
                {
                    Console.WriteLine(HttpUtility.HtmlDecode(reviewsCountNode.InnerText).Trim());
                }

                var priceCategoryNode = document.QuerySelector("span > span:nth-child(2) > span:nth-child(1) > span");
                if (priceCategoryNode != null)
                {
                    Console.WriteLine(HttpUtility.HtmlDecode(priceCategoryNode.InnerText).Trim());
                }

                var categoryNode = document.QuerySelector("div:nth-child(3) > div > div > div > div > div > div > div > div > div > div > div:nth-child(2)");
                if (categoryNode != null)
                {
                    Console.WriteLine(HttpUtility.HtmlDecode(categoryNode.InnerText).Trim());
                }

                var summaryNode = document.QuerySelector("button > div > div:nth-child(1) > div > span");
                if (summaryNode != null)
                {
                    Console.WriteLine(HttpUtility.HtmlDecode(summaryNode.InnerText).Trim());
                }
                var addressNode = document.QuerySelector("[aria-label*='Address:']");
                if (addressNode != null)
                {
                    Console.WriteLine(HttpUtility.HtmlDecode(addressNode.InnerText).Trim());
                }
                var phoneNode = document.QuerySelector("[aria-label*='Phone:']");
                if (phoneNode != null)
                {
                    Console.WriteLine(HttpUtility.HtmlDecode(phoneNode.InnerText).Trim());
                }
                var websiteNode = document.QuerySelector("[aria-label*='Website:']");
                if (websiteNode != null)
                {
                    Console.WriteLine(HttpUtility.HtmlDecode(websiteNode.InnerText).Trim());

                }
                try
                {
                    var workHoursNode = document.QuerySelector("[aria-label*='Monday,']");
                    if (workHoursNode != null)
                    {
                        Console.WriteLine(HttpUtility.HtmlDecode(workHoursNode.Attributes["aria-label"].Value).Trim());
                    }
                }
                catch (Exception ex)
                {

                }
                
                Console.WriteLine("=====================");
            }
        }
    }
}


Console.ReadKey();

