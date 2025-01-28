using OpenQA.Selenium;
using StockTracker.Services.ParsersServices;
using static StockTracker.Services.ParsersServices.JavaScriptService;
using static StockTracker.Services.ParsersServices.SeleniumService;

namespace StockTracker.Parsers
{
    public class YandexMarketParser : IParser
    {
        public string ShopName => "Яндекс Маркет";

        private readonly ProxyService _proxyService;

        public YandexMarketParser(ProxyService proxyService)
        {
            _proxyService = proxyService;
        }

        public async Task<string> Parse(string url)
        {
            using (var driver = CreateWebDriver(_proxyService, url))
            {
                try
                {
                    if (!IsElementAvailable(driver, "//*[@id=\"/content/page/fancyPage/emptyOfferSnippet\"]/div/div/div[2]/div/div/div[1]/h2"))
                    {
                        Console.WriteLine("Нет в наличии");
                        return "Нет в наличии";
                    }
                    else
                    {
                        return CountProducts(driver).ToString();
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Не удалось спарсить");
                    return "Не удалось спарсить";
                }
            }
        }

        public int CountProducts(IWebDriver driver)
        {
            CloseUncessaryWindows(driver);

            ClickElement(driver, "//button[@data-auto='cartButton']");
            JSHumanSimulation(driver);

            ClickElement(driver, "//*[@id=\"CART_ENTRY_POINT_ANCHOR\"]/a");
            JSHumanSimulation(driver);

            EnterText(driver, "//*[@id=\"/content/page/fancyPage/@chef\\/cart\\/CartLayout/@chef\\/cart\\/ChefCartList/@light\\/SlotsTheCreator/MARKET/@chef\\/cart\\/CartLazyWrapper/lazy/initialContent/slots/MARKET_0/list/0_MARKET/addToCartButton\"]/div/div/span/input", "10000");
            
            ClickElement(driver, "//button[@aria-label='Увеличить']");

            return int.Parse(GetAttribute(driver, "//input[@aria-label='Количество товара']"));
        }

        private void CloseUncessaryWindows(IWebDriver driver)
        {
            try
            {
                var closeButton = driver.FindElement(By.XPath("//div[@aria-label='Закрыть']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", closeButton);
            }
            catch (Exception)
            {
                //Заменить на Log
                Console.WriteLine("Всплывающих окон не обнаружено!");
            }
        }
    }
    
}
