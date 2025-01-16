using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace StockTracker.Parsers
{
    public class YandexMarketParser : IParser
    {
        public async Task<bool> Parse(string url)
        {
            try
            {
                #region Selenium

                IWebDriver driver = new ChromeDriver();
                driver.Url = @"https://market.yandex.ru/product--nastolnaia-igra-teibl-taim-2-kollektsionnyi-sbornik-igr/898017986?sku=103676455619&uniqueId=115689385&businessId=115689385&nid=59742";

                string IsAvailable = "";
                try
                {

                    IsAvailable = (driver.FindElement(By.XPath("//*[@id=\"/content/page/fancyPage/emptyOfferSnippet\"]/div/div/div[2]/div/div/div[1]/h2"))).Text;
                    await Task.Delay(1000);
                }
                catch (Exception)
                {
                    await Console.Out.WriteLineAsync("Не удалось спарсить!");
                }

                #endregion
            }
            catch { await Console.Out.WriteLineAsync("Не удалось спарсить!"); }

            //Заглушка
            return false;
        }
    }
}
