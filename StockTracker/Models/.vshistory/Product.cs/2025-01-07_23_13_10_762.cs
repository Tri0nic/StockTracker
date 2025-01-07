using System.ComponentModel.DataAnnotations;

namespace StockTracker.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите название магазина")]
        public string Shop { get; set; }

        [Required(ErrorMessage = "Добавьте ссылку на товар")]
        [Url]
        public string Link { get; set; }

        public bool IsTracked { get; set; }
    }
}
