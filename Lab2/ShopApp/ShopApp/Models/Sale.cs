using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopApp.Models
{
    public class Sale
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SaleId { get; set; }

        [Required(ErrorMessage = "Номер чеку обов'язковий")]
        [Display(Name = "Чек №")]
        public int CheckNo { get; set; }

        [Required(ErrorMessage = "Товар обов'язковий")]
        [Display(Name = "Товар")]
        public int GoodId { get; set; }

        [Required(ErrorMessage = "Дата обов'язкова")]
        [Display(Name = "Дата продажу")]
        public DateTime DateSale { get; set; } = DateTime.Today;

        [Range(1, int.MaxValue, ErrorMessage = "Кількість ≥ 1")]
        [Display(Name = "Кількість")]
        public int? Quantity { get; set; }

        [ForeignKey(nameof(GoodId))]
        public Good? Good { get; set; }
    }
}
