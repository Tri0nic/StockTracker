using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace StockTracker.Parsers
{
    public class MosigraParser : IParser
    {
        public async Task<bool> Parse(string url)
        {
            try
            {
                #region Selenium

                IWebDriver driver = new ChromeDriver();
                //driver.Url = @"https://www.mosigra.ru/runebound-v-pautine/";
                driver.Url = @"https://www.mosigra.ru/runebound/";

                string IsAvailable = "";
                try
                {
                    //открываем список городов
                    var element1 = driver.FindElement(By.XPath("//*[@id=\"app-main\"]/header/div[2]/div/div/div[1]/button"));

                    // Выполняем клик
                    element1.Click();

                    //выбираем Москву
                    var element = driver.FindElement(By.XPath("//*[@id=\"app-main\"]/header/div[1]/div/ul/li[1]/span/a"));

                    // Выполняем клик
                    element.Click();

                    IsAvailable = (driver.FindElement(By.XPath("//*[@id=\"app-main\"]/main/section[1]/article/section[1]/noindex/div/span[2]/b"))).Text;
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
