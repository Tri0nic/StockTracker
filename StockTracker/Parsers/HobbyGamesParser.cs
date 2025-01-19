using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace StockTracker.Parsers
{
    public class HobbyGamesParser// : IParser
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
                    string IsAvailable = (driver.FindElement(By.XPath("/html/body/div[2]/div/aside/div[1]/div[1]/div/div[3]/button[1]/span"))).Text;
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
