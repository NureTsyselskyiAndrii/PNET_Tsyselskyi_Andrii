using Microsoft.EntityFrameworkCore;
using ShopApp.Models;

namespace ShopApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Good> Goods => Set<Good>();
        public DbSet<Sale> Sales => Set<Sale>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<Department>(e =>
            {
                e.ToTable("Departments");
                e.HasKey(d => d.DeptId);
                e.Property(d => d.DeptId)
                 .HasColumnName("DEPT_ID")
                 .HasColumnType("int")
                 .ValueGeneratedOnAdd();
                e.Property(d => d.Name).HasColumnName("NAME").HasMaxLength(20);
                e.Property(d => d.Info).HasColumnName("INFO").HasMaxLength(40).HasDefaultValue(null);
            });

            mb.Entity<Good>(e =>
            {
                e.ToTable("Goods");
                e.HasKey(g => g.GoodId);
                e.Property(g => g.GoodId).HasColumnName("GOOD_ID").ValueGeneratedOnAdd();
                e.Property(g => g.Name).HasColumnName("NAME").HasMaxLength(20);
                e.Property(g => g.Price).HasColumnName("PRICE");
                e.Property(g => g.Quantity).HasColumnName("QUANTITY");
                e.Property(g => g.Producer).HasColumnName("PRODUCER").HasMaxLength(20);
                e.Property(g => g.DeptId).HasColumnName("DEPT_ID").HasColumnType("int");
                e.Property(g => g.Description).HasColumnName("DESCRIPTION").HasMaxLength(50);

                e.HasOne(g => g.Department)
                 .WithMany(d => d.Goods)
                 .HasForeignKey(g => g.DeptId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            mb.Entity<Sale>(e =>
            {
                e.ToTable("Sales");
                e.HasKey(s => s.SaleId);
                e.Property(s => s.SaleId).HasColumnName("SALE_ID").ValueGeneratedOnAdd();
                e.Property(s => s.CheckNo).HasColumnName("CHECK_NO");
                e.Property(s => s.GoodId).HasColumnName("GOOD_ID");
                e.Property(s => s.DateSale).HasColumnName("DATE_SALE").HasColumnType("date");
                e.Property(s => s.Quantity).HasColumnName("QUANTITY");

                e.HasOne(s => s.Good)
                 .WithMany(g => g.Sales)
                 .HasForeignKey(s => s.GoodId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            mb.Entity<Department>().HasData(
                new Department { DeptId = 1, Name = "Продукти", Info = "Харчові товари" },
                new Department { DeptId = 2, Name = "Електроніка", Info = "Техніка та гаджети" },
                new Department { DeptId = 3, Name = "Одяг", Info = "Чоловічий та жіночий" },
                new Department { DeptId = 4, Name = "Побутхімія", Info = "Засоби для дому" },
                new Department { DeptId = 5, Name = "Канцтовари", Info = "Офісні та шкільні" }
            );

            mb.Entity<Good>().HasData(
                new Good { GoodId = 1, Name = "Молоко", Price = 28.50, Quantity = 120, Producer = "Галичина", DeptId = 1, Description = "Молоко 2.5%, 1л" },
                new Good { GoodId = 2, Name = "Хліб чорний", Price = 22.00, Quantity = 80, Producer = "Київхліб", DeptId = 1, Description = "Житній хліб 400г" },
                new Good { GoodId = 3, Name = "Навушники", Price = 850.00, Quantity = 15, Producer = "Samsung", DeptId = 2, Description = "Бездротові BT 5.0" },
                new Good { GoodId = 4, Name = "Футболка", Price = 299.00, Quantity = 40, Producer = "H&M", DeptId = 3, Description = "Бавовна 100%, XL" },
                new Good { GoodId = 5, Name = "Порошок", Price = 145.00, Quantity = 55, Producer = "Tide", DeptId = 4, Description = "Автомат 1.5 кг" },
                new Good { GoodId = 6, Name = "Зошит", Price = 35.00, Quantity = 200, Producer = "Buromax", DeptId = 5, Description = "96 аркушів, клітинка" }
            );

            mb.Entity<Sale>().HasData(
                new Sale { SaleId = 1, CheckNo = 1001, GoodId = 1, DateSale = new DateTime(2024, 1, 10), Quantity = 3 },
                new Sale { SaleId = 2, CheckNo = 1001, GoodId = 2, DateSale = new DateTime(2024, 1, 10), Quantity = 2 },
                new Sale { SaleId = 3, CheckNo = 1002, GoodId = 3, DateSale = new DateTime(2024, 1, 11), Quantity = 1 },
                new Sale { SaleId = 4, CheckNo = 1003, GoodId = 4, DateSale = new DateTime(2024, 1, 12), Quantity = 2 },
                new Sale { SaleId = 5, CheckNo = 1003, GoodId = 5, DateSale = new DateTime(2024, 1, 12), Quantity = 1 },
                new Sale { SaleId = 6, CheckNo = 1004, GoodId = 6, DateSale = new DateTime(2024, 1, 13), Quantity = 5 }
            );
        }
    }
}