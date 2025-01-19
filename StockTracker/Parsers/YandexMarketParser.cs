using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using static StockTracker.Parsers.ParserService;
using static StockTracker.Parsers.ProxyService;
using static OpenQA.Selenium.BiDi.Modules.BrowsingContext.Locator;
using static System.Net.Mime.MediaTypeNames;

namespace StockTracker.Parsers
{
    public class YandexMarketParser : IParser
    {
        private readonly ProxyService _proxyService;

        public YandexMarketParser(ProxyService proxyService)
        {
            _proxyService = proxyService;
        }

        public async Task<string> Parse(string url)
        {
            try
            {
                #region Selenium

                var chromeOptions = _proxyService.GetRandomProxy();
                var driver = new ChromeDriver(chromeOptions);
                driver.Url = url;

                try
                {
                    if (!IsAvailable(driver, "//*[@id=\"/content/page/fancyPage/emptyOfferSnippet\"]/div/div/div[2]/div/div/div[1]/h2"))
                    {
                        driver.Quit();
                        return "Нет в наличии";
                    }
                    else
                    {
                        var numberOfAvailableProducts = CountTheNumberOfAvailableProducts(driver);
                        driver.Quit();
                        return numberOfAvailableProducts.ToString();
                    }
                }
                catch (Exception)
                {
                    driver.Quit();
                    return "Не удалось спарсить";
                }

                #endregion
            }
            catch
            {
                await Console.Out.WriteLineAsync($"Не удалось подключиться к сайту магазина по ссылке: {url}");
            }

            return "Не удалось подключиться к сайту";
        }

        public int CountTheNumberOfAvailableProducts(IWebDriver driver)
        {
            try
            {
                var closeButton = driver.FindElement(By.XPath("//div[@aria-label='Закрыть']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", closeButton);
            }
            catch (Exception)
            {
                Console.WriteLine("Всплывающих окон не обнаружено!");
            }
            
            ClickElement(driver, "//button[@aria-label='В корзину']");
            HumanSimulation(driver);
            ClickElement(driver, "//*[@id=\"CART_ENTRY_POINT_ANCHOR\"]/a");
            HumanSimulation(driver);
            EnterText(driver, "//*[@id=\"/content/page/fancyPage/@chef\\/cart\\/CartLayout/@chef\\/cart\\/ChefCartList/@light\\/SlotsTheCreator/MARKET/@chef\\/cart\\/CartLazyWrapper/lazy/initialContent/slots/MARKET_0/list/0_MARKET/addToCartButton\"]/div/div/span/input", "10000");
            ClickElement(driver, "//button[@aria-label='Увеличить']");

            return int.Parse(GetAttribute(driver, "//input[@aria-label='Количество товара']"));
        }
    }
    
}
