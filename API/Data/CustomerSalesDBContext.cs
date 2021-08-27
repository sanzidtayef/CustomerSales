using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;


namespace CustomerSales.Models
{
    public partial class CustomerSalesDBContext : DbContext
    {
        public CustomerSalesDBContext()
        {
        }

        public CustomerSalesDBContext(DbContextOptions<CustomerSalesDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Sale> Sales { get; set; }
        public virtual DbSet<SalesDetail> SalesDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=.;Database=CustomerSalesDB;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.VCustomerId)
                    .HasName("PK__Customer__F0D4FAF243608BAE");

                entity.ToTable("Customer");

                entity.Property(e => e.VCustomerId)
                    .IsUnicode(false)
                    .HasColumnName("vCustomerId");

                entity.Property(e => e.VCustomerAddress)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("vCustomerAddress");

                entity.Property(e => e.VCustomerFirstName)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("vCustomerFirstName");

                entity.Property(e => e.VCustomerLastName)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("vCustomerLastName");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.VProductId)
                    .HasName("PK__Product__0992F02CAD17EB4F");

                entity.ToTable("Product");

                entity.Property(e => e.VProductId)
                    .IsUnicode(false)
                    .HasColumnName("vProductId");

                entity.Property(e => e.NProductPrice)
                    .HasColumnType("numeric(18, 2)")
                    .HasColumnName("nProductPrice");

                entity.Property(e => e.VProductName)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("vProductName");
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasKey(e => e.VSalesId)
                    .HasName("PK__Sales__CCE0334A385940D2");

                entity.Property(e => e.VSalesId)
                    .IsUnicode(false)
                    .HasColumnName("vSalesId");

                entity.Property(e => e.DDate)
                    .HasColumnType("datetime")
                    .HasColumnName("dDate");

                entity.Property(e => e.NInvoiceNo)
                    .HasColumnType("numeric(18, 0)")
                    .HasColumnName("nInvoiceNo");

                entity.Property(e => e.VCustomerId)
                    .IsRequired()
                    .HasMaxLength(900)
                    .IsUnicode(false)
                    .HasColumnName("vCustomerId");

                entity.HasOne(d => d.VCustomer)
                    .WithMany(p => p.Sales)
                    .HasForeignKey(d => d.VCustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Sales__vCustomer__03F0984C");
            });

            modelBuilder.Entity<SalesDetail>(entity =>
            {
                entity.HasKey(e => e.VSalesDetailsId)
                    .HasName("PK__SalesDet__89D4A0BFF764CB78");

                entity.Property(e => e.VSalesDetailsId)
                    .IsUnicode(false)
                    .HasColumnName("vSalesDetailsId");

                entity.Property(e => e.IAutoId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("iAutoId");

                entity.Property(e => e.NProductPrice)
                    .HasColumnType("numeric(18, 2)")
                    .HasColumnName("nProductPrice");

                entity.Property(e => e.NQuantity)
                    .HasColumnType("numeric(18, 2)")
                    .HasColumnName("nQuantity");

                entity.Property(e => e.VProductId)
                    .IsRequired()
                    .HasMaxLength(900)
                    .IsUnicode(false)
                    .HasColumnName("vProductId");

                entity.Property(e => e.VSalesId)
                    .IsRequired()
                    .HasMaxLength(900)
                    .IsUnicode(false)
                    .HasColumnName("vSalesId");

                entity.HasOne(d => d.VProduct)
                    .WithMany(p => p.SalesDetails)
                    .HasForeignKey(d => d.VProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SalesDeta__vProd__7A672E12");
            });

            modelBuilder.HasSequence("seq_customer").HasMin(1);

            modelBuilder.HasSequence("seq_product").HasMin(1);

            modelBuilder.HasSequence("seq_sales").HasMin(1);

            modelBuilder.HasSequence("seq_salesdetails").HasMin(1);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
