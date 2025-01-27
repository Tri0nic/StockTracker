using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using static StockTracker.Parsers.ParserService;

namespace StockTracker.Parsers
{
    public class MosigraParser : IParser
    {
        private readonly ProxyService _proxyService;

        public MosigraParser(ProxyService proxyService)
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
                    // открываем список городов
                    ClickElement(driver, "//*[@id=\"app-main\"]/header/div[2]/div/div/div[1]/button");

                    // выбираем Москву
                    ClickElement(driver, "//*[@id=\"app-main\"]/header/div[1]/div/ul/li[1]/span/a");

                    if (!IsAvailable(driver, "//*[@id=\"app-main\"]/main/section[1]/article/section[1]/noindex/div/span[2]/b"))
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
            HumanSimulation(driver);

            var element = driver.FindElement(By.XPath("//button[@class='btn btn-block btn-cart btn-cart--full-fill buy__button to-cart']"));
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].click();", element);
            Thread.Sleep(new Random().Next(2000, 2501));

            var element1 = driver.FindElement(By.XPath("//button[@class='btn btn-block btn-cart btn-cart--full-fill buy__button icon-in-cart']"));
            IJavaScriptExecutor js1 = (IJavaScriptExecutor)driver;
            js1.ExecuteScript("arguments[0].click();", element1);
            Thread.Sleep(new Random().Next(2000, 2501));

            var element2 = driver.FindElement(By.XPath("//a[@class='btn btn-change-step btn-red btn-order']"));
            IJavaScriptExecutor js2 = (IJavaScriptExecutor)driver;
            js2.ExecuteScript("arguments[0].click();", element2);
            Thread.Sleep(new Random().Next(2000, 2501));

            var count = 0;
            // Надо увеличить время между кликами!!!!!!!!!!!!!!!!!!!!!!!!
            while (true)
            {
                //Нажимаем на кнопку самовывоза
                var element3 = driver.FindElement(By.XPath("//input[@class='visually-hidden']"));
                IJavaScriptExecutor js3 = (IJavaScriptExecutor)driver;
                js3.ExecuteScript("arguments[0].click();", element3);
                Thread.Sleep(new Random().Next(3000, 4001));

                HumanSimulation(driver);

                // Получаем список магазинов
                var elements = driver.FindElements(By.XPath("//div[@class='row delivery__methods__item  ']"));

                var numberOfShops = elements.Count();

                // Подсчитать количество элементов
                foreach (var shop in elements)
                {
                    try
                    {
                        shop.FindElement(By.XPath(".//div[text()='Отсюда можно забрать только часть заказа']"));
                        numberOfShops--;
                        if (numberOfShops == 0)
                        {
                            return count;
                        }
                    }
                    catch (Exception)
                    {
                        count++;
                    }
                }

                var element4 = driver.FindElement(By.XPath("//a[@data-action='plus']"));
                IJavaScriptExecutor js4 = (IJavaScriptExecutor)driver;
                js4.ExecuteScript("arguments[0].click();", element4);
                Thread.Sleep(new Random().Next(3000, 4001));
            }
        }
    }
}
