using StockTracker.Models;
using System.Collections.Generic;

namespace StockTracker.Parsers
{
    public class YandexMarketParser : IParser
    {
        public bool Parse(Product products)
        {
            if (ProductIsAvailable(products))
                return true; // Т.е. товар появился в продаже, нужно отправить уведомление
            else
                return false;
        }
        public bool ProductIsAvailable(Product products)
        {
            return true;
        }



    }
}
