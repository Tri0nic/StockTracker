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
                driver.Url = url;

                try
                {
                    string IsAvailable = (driver.FindElement(By.XPath("//*[@id=\"/content/page/fancyPage/emptyOfferSnippet\"]/div/div/div[2]/div/div/div[1]/h2"))).Text;
                    await Console.Out.WriteLineAsync("\nЗакончил парсинг!\n");
                    return false;
                }
                catch (Exception)
                {
                    return true;
                }

                #endregion
            }
            catch
            {
                await Console.Out.WriteLineAsync($"Не удалось подключиться к сайту магазина по ссылке: {url}");
            }

            return false;
        }
    }
}
