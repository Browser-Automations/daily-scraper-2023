using bop.gov_scraper;
using CsvHelper;
using HtmlAgilityPack;
using PuppeteerSharp;
using System.Formats.Asn1;
using System.Globalization;
using System.Web;

List<DataModel> entries = new List<DataModel>();
HtmlWeb web = new HtmlWeb();
HtmlDocument doc = web.Load("https://www.bop.gov/locations/list.jsp");

try
{
    var fileName = DateTime.Now.ToString("yyyyMMddHHmmss")+".csv";
    using var browserFetcher = new BrowserFetcher();
    await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
    var browser = await Puppeteer.LaunchAsync(new LaunchOptions
    {
        Headless = true
    });
    var page = await browser.NewPageAsync();


    var columns = doc.DocumentNode.SelectNodes("//div[@class='colDisplayList']");
    if (columns != null)
    {
        foreach (var col in columns)
        {
            var anchors = col.ChildNodes.Where(x => x.Name == "a");
            foreach (var anchor in anchors)
            {
                try
                {
                    DataModel entry = new DataModel
                    {
                        Url = "https://www.bop.gov" + anchor.Attributes["href"].Value,
                        FacilityName = HttpUtility.HtmlDecode(anchor.InnerText)
                    };
                    await page.GoToAsync("https://www.bop.gov" + anchor.Attributes["href"].Value);
                    await page.WaitForTimeoutAsync(1500);
                    var detailsDoc = new HtmlDocument();
                    detailsDoc.LoadHtml(await page.GetContentAsync());

                    var addressNode = detailsDoc.DocumentNode.SelectSingleNode("//div[@id='address']");
                    if (addressNode != null)
                    {
                        entry.Address = addressNode.InnerText;
                    }

                    var address2Node = detailsDoc.DocumentNode.SelectSingleNode("//div[@id='address2']");
                    if (address2Node != null)
                    {
                        entry.Address2 = address2Node.InnerText;
                    }

                    var cityNode = detailsDoc.DocumentNode.SelectSingleNode("//span[@id='city']");
                    if (cityNode != null)
                    {
                        entry.City = cityNode.InnerText;
                    }

                    var stateNode = detailsDoc.DocumentNode.SelectSingleNode("//span[@id='state']");
                    if (stateNode != null)
                    {
                        entry.State = stateNode.InnerText;
                    }

                    var zipNode = detailsDoc.DocumentNode.SelectSingleNode("//span[@id='zip_code']");
                    if (zipNode != null)
                    {
                        entry.Zip = zipNode.InnerText;
                    }
                    entries.Add(entry);
                    Console.WriteLine(entry.Url);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            using (var writer = new StreamWriter(fileName))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(entries);
            }
        }
    }

}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
