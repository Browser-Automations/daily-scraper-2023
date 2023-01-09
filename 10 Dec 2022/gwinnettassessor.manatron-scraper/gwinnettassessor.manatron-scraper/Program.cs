using gwinnettassessor.manatron_scraper;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using PuppeteerSharp;
using System.Web;

using var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
var browser = await Puppeteer.LaunchAsync(new LaunchOptions
{
    Headless = false, DefaultViewport=null
});
var page = await browser.NewPageAsync();
List<DataModel> entries = new List<DataModel>();

List<string> parcelIds = new List<string>() {
"R1001 001",
"R1001 002",
"R1001 003"
};
foreach (var parcelId in parcelIds)
{
	try
	{
        DataModel entry = new DataModel() { ParcelId=parcelId };
        await page.GoToAsync($"https://gwinnettassessor.manatron.com/IWantTo/PropertyGISSearch/PropertyDetail.aspx?p={parcelId}&a=232335", new NavigationOptions { Timeout = 180000 });
        await page.WaitForTimeoutAsync(2000);

        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(await page.GetContentAsync());

        var addressSection = doc.DocumentNode.QuerySelector("#lxT1385 > table > tbody > tr:nth-child(1) > td");
        if (addressSection != null)
        {
            var parts = addressSection.InnerHtml.Replace("\n", "").Trim().Split("<br>");

            switch (parts.Length)
            {
                case 3:
                    entry.OwnerName = parts[0];
                    entry.MailingAddress = parts[1];
                    var pieces = parts[2].Trim().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (pieces.Length >= 3)
                    {
                        entry.MailingZip = pieces[pieces.Length - 1];
                        entry.MailingState = pieces[pieces.Length - 2];
                        entry.MailingCity = parts[2].Replace(entry.MailingState, "").Replace(entry.MailingZip, "").Trim();
                    }
                    break;
                case 4:
                    entry.OwnerName = (parts[0] + " & " + parts[1]).Trim();
                    entry.MailingAddress = parts[2];
                    pieces = parts[3].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    if (pieces.Length >= 3)
                    {
                        entry.MailingZip = pieces[pieces.Length - 1];
                        entry.MailingState = pieces[pieces.Length - 2];
                        entry.MailingCity = parts[3].Replace(entry.MailingState, "").Replace(entry.MailingZip, "").Trim();
                    }
                    break;
                default:
                    break;
            }
        }

        var propertyAddressNode = doc.DocumentNode.SelectSingleNode("//*[@id=\"lxT1385\"]/table/tbody/tr[4]/td");
        if (propertyAddressNode != null)
        {
            entry.PropertyStreet = HttpUtility.HtmlDecode(propertyAddressNode.InnerText);
        }

        var propertyClassNode = doc.DocumentNode.SelectSingleNode("//*[@id=\"lxT1385\"]/table/tbody/tr[5]/td");
        if (propertyClassNode != null)
        {
            entry.PropertyClass = HttpUtility.HtmlDecode(propertyClassNode.InnerText);
        }

        var yearBuiltNode = doc.DocumentNode.SelectSingleNode("//*[@id=\"1388gallerywrap\"]/table[2]/tbody/tr[3]");
        if (yearBuiltNode != null)
        {
           entry.YearBuilt = HttpUtility.HtmlDecode(yearBuiltNode.ChildNodes[1].InnerText);
        }
        entries.Add(entry);
    }
	catch (Exception ex)
	{
        Console.WriteLine("Unable to scrape record. Reason: "+ex.Message);
	}
}
