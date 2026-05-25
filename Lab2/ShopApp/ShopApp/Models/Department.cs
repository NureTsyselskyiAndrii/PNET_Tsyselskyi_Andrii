using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopApp.Models
{
    public class Department
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DeptId { get; set; }

        [MaxLength(20)]
        [Display(Name = "Назва відділу")]
        public string? Name { get; set; }

        [MaxLength(40)]
        [Display(Name = "Опис")]
        public string? Info { get; set; }

        public ICollection<Good> Goods { get; set; } = new List<Good>();
    }
}
