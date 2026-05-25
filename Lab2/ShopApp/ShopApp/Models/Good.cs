using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopApp.Models
{
    public class Good
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GoodId { get; set; }

        [MaxLength(20)]
        [Required(ErrorMessage = "Назва обов'язкова")]
        [Display(Name = "Назва товару")]
        public string? Name { get; set; }

        [Range(0.01, 1_000_000, ErrorMessage = "Ціна має бути > 0")]
        [Display(Name = "Ціна")]
        public double? Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Кількість ≥ 0")]
        [Display(Name = "Кількість")]
        public int? Quantity { get; set; }

        [MaxLength(20)]
        [Display(Name = "Виробник")]
        public string? Producer { get; set; }

        [Display(Name = "Відділ")]
        public int? DeptId { get; set; }

        [MaxLength(50)]
        [Display(Name = "Опис")]
        public string? Description { get; set; }

        [ForeignKey(nameof(DeptId))]
        public Department? Department { get; set; }

        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}
