#nullable disable

using GridPromocional.Areas.Identity.Data;
using GridPromocional.Models;
using GridPromocional.Models.Views;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace GridPromocional.Data;

public class GridContext : IdentityDbContext<GridUser>
{
    public GridContext(DbContextOptions<GridContext> options)
        : base(options)
    {
    }

    public virtual DbSet<PgCatFamily> PgCatFamily { get; set; }
    public virtual DbSet<PgCatManagersOthers> PgCatManagersOthers { get; set; }
    public virtual DbSet<PgCatMaterialCategory> PgCatMaterialCategory { get; set; }
    public virtual DbSet<PgCatMaterialType> PgCatMaterialType { get; set; }
    public virtual DbSet<PgCatMenuActions> PgCatMenuActions { get; set; }
    public virtual DbSet<PgCatProducts> PgCatProducts { get; set; }
    public virtual DbSet<PgCatSalesForce> PgCatSalesForce { get; set; }
    public virtual DbSet<PgCatStatusDo> PgCatStatusDo { get; set; }
    public virtual DbSet<PgCatStatusEmployee> PgCatStatusEmployee { get; set; }
    public virtual DbSet<PgCatStatusProducts> PgCatStatusProducts { get; set; }
    public virtual DbSet<PgFactDistributionOrders> PgFactDistributionOrders { get; set; }
    public virtual DbSet<PgFactInventories> PgFactInventories { get; set; }
    public virtual DbSet<PgFactPurchaseOrder> PgFactPurchaseOrder { get; set; }
    public virtual DbSet<PgLogActivity> PgLogActivity { get; set; }
    public virtual DbSet<PgLogUploadHistory> PgLogUploadHistory { get; set; }
    public virtual DbSet<PgRoleActions> PgRoleActions { get; set; }
    public virtual DbSet<PgSelFamily> PgSelFamily { get; set; }
    public virtual DbSet<PgSelSalesForce> PgSelSalesForce { get; set; }
    public virtual DbSet<PgStgCatManagersOthers> PgStgCatManagersOthers { get; set; }
    public virtual DbSet<PgStgCatProducts> PgStgCatProducts { get; set; }
    public virtual DbSet<PgStgCatSalesForce> PgStgCatSalesForce { get; set; }
    public virtual DbSet<PgStgFactInventories> PgStgFactInventories { get; set; }
    public virtual DbSet<PgStgFactPurchaseOrder> PgStgFactPurchaseOrder { get; set; }
    public virtual DbSet<PgUserFamily> PgUserFamilies { get; set; } = null!;
    public virtual DbSet<UserRecord> UserRecord { get; set; }
    public virtual DbSet<UserFamiliesReportRecord> UserFamiliesReportRecord { get; set; }
    public virtual DbSet<PgSKUMaster> PgSKUMaster { get; set; }
    public virtual DbSet<ViewInventories> ViewInventories { get; set; } = null!;
    public virtual DbSet<SKUMasterView> SKUMasterView { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);

        builder.Entity<PgCatFamily>(entity =>
        {
            entity.HasKey(e => e.IdFam)
                .HasName("PK__PG_cat_f__2B3AB9A2BB0A884B");

            entity.Property(e => e.IdFam).IsFixedLength();
        });

        builder.Entity<PgCatManagersOthers>(entity =>
        {
            entity.HasKey(e => e.Colemp)
                .HasName("PK__PG_cat_m__79774785FE7FC7E8");

            entity.Property(e => e.Cdgcmp).IsFixedLength();

            entity.Property(e => e.Cdpemp).IsFixedLength();

            entity.Property(e => e.Diaing).IsFixedLength();

            entity.Property(e => e.Estemp).IsFixedLength();

            entity.Property(e => e.Rdtf).IsFixedLength();

            entity.HasOne(d => d.EstempNavigation)
                .WithMany(p => p.PgCatManagersOthers)
                .HasForeignKey(d => d.Estemp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PG_cat_managers_others_PG_cat_status_employee");
        });

        builder.Entity<PgCatMaterialCategory>(entity =>
        {
            entity.Property(e => e.IdCategory).IsFixedLength();
        });

        builder.Entity<PgCatMaterialType>(entity =>
        {
            entity.HasKey(e => e.IdType)
                .HasName("PK__PG_cat_m__274CEC8213116394");

            entity.Property(e => e.IdType).IsFixedLength();

            entity.Property(e => e.IdCategory).IsFixedLength();

            entity.HasOne(d => d.IdCategoryNavigation)
                .WithMany(p => p.PgCatMaterialType)
                .HasForeignKey(d => d.IdCategory)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PG_cat_material_type_PG_cat_material_type");
        });

        builder.Entity<PgCatMenuActions>(entity =>
        {
            entity.HasKey(e => e.IdMa)
                .HasName("PK_PG_cat_menu");
        });

        builder.Entity<PgCatProducts>(entity =>
        {
            entity.HasKey(e => e.Code)
                .HasName("PK__PG_cat_p__17597AF21BC467EE");

            entity.Property(e => e.IdFam).IsFixedLength();

            entity.Property(e => e.IdSt).IsFixedLength();

            entity.Property(e => e.IdType).IsFixedLength();

            entity.Property(e => e.Status).HasDefaultValueSql("((1))");

            entity.HasOne(d => d.IdFamNavigation)
                .WithMany(p => p.PgCatProducts)
                .HasForeignKey(d => d.IdFam)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PG_cat_products_PG_cat_family");

            entity.HasOne(d => d.IdStNavigation)
                .WithMany(p => p.PgCatProducts)
                .HasForeignKey(d => d.IdSt)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PG_cat_products_PG_cat_status_products");

            entity.HasOne(d => d.IdTypeNavigation)
                .WithMany(p => p.PgCatProducts)
                .HasForeignKey(d => d.IdType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PG_cat_products_PG_cat_material_type");
        });

        builder.Entity<PgCatSalesForce>(entity =>
        {
            entity.HasKey(e => e.SearchTerm2)
                .HasName("PK__PG_cat_s__844DE3ED43715AB4");

            entity.Property(e => e.CompanyCode).IsFixedLength();

            entity.Property(e => e.CreatedBy).IsFixedLength();

            entity.Property(e => e.PostalCode).IsFixedLength();

            entity.Property(e => e.SalesOrganization).IsFixedLength();

            entity.Property(e => e.SearchTerm1).IsFixedLength();
        });

        builder.Entity<PgCatStatusDo>(entity =>
        {
            entity.HasKey(e => e.IdSt)
                .HasName("PK__PG_cat_s__8B63A9834110958B");

            entity.Property(e => e.IdSt).ValueGeneratedNever();
        });

        builder.Entity<PgCatStatusEmployee>(entity =>
        {
            entity.Property(e => e.Estemp).IsFixedLength();
        });

        builder.Entity<PgCatStatusProducts>(entity =>
        {
            entity.HasKey(e => e.IdSt)
                .HasName("PK__PG_cat_s__8B63A98366B13201");

            entity.Property(e => e.IdSt).IsFixedLength();
        });

        builder.Entity<PgFactDistributionOrders>(entity =>
        {
            entity.HasKey(e => new { e.Date, e.Username, e.Sku })
                .HasName("PK__PG_fact___F9A3D01905D2A005");

            entity.Property(e => e.IdFam).IsFixedLength();

            entity.Property(e => e.IdType).IsFixedLength();

            entity.Property(e => e.Ordnum).IsFixedLength();

            entity.HasOne(d => d.IdFamNavigation)
                .WithMany(p => p.PgFactDistributionOrders)
                .HasForeignKey(d => d.IdFam)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PG_fact_distribution_orders_PG_cat_family");

            entity.HasOne(d => d.IdStNavigation)
                .WithMany(p => p.PgFactDistributionOrders)
                .HasForeignKey(d => d.IdSt)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PG_fact_distribution_orders_PG_cat_status_do");

            entity.HasOne(d => d.IdTypeNavigation)
                .WithMany(p => p.PgFactDistributionOrders)
                .HasForeignKey(d => d.IdType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PG_fact_distribution_orders_PG_cat_material_type");
        });

        builder.Entity<PgFactInventories>(entity =>
        {
            entity.HasKey(e => e.Code)
                    .HasName("PK__PG_fact___AA1D43782864B4F3");

            entity.Property(e => e.IdFam).IsFixedLength();

            entity.Property(e => e.IdType).IsFixedLength();

            entity.Property(e => e.Status).IsFixedLength();

            entity.Property(e => e.Uom).IsFixedLength();

            entity.HasOne(d => d.CodeNavigation)
                .WithOne(p => p.PgFactInventories)
                .HasForeignKey<PgFactInventories>(d => d.Code)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PG_fact_inventories_PG_cat_products");

            entity.HasOne(d => d.IdFamNavigation)
                .WithMany(p => p.PgFactInventories)
                .HasForeignKey(d => d.IdFam)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PG_fact_inventories_PG_cat_family");

            entity.HasOne(d => d.IdTypeNavigation)
                .WithMany(p => p.PgFactInventories)
                .HasForeignKey(d => d.IdType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PG_fact_inventories_PG_cat_material_type");
        });

        builder.Entity<PgFactPurchaseOrder>(entity =>
        {
            entity.HasKey(e => new { e.PurchasingDocument, e.Code })
                .HasName("PK__PG_fact___4FDF14CA1D39323C");

            entity.Property(e => e.Vendor).IsFixedLength();

            entity.HasOne(d => d.CodeNavigation)
                .WithMany(p => p.PgFactPurchaseOrder)
                .HasForeignKey(d => d.Code)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PG_fact_purchase_order_PG_cat_products");
        });

        builder.Entity<PgLogActivity>(entity =>
        {
            entity.HasKey(e => e.IdLa)
                .HasName("PK__PG_log_a__8B62F4A1BEAE4730");
        });

        builder.Entity<PgLogUploadHistory>(entity =>
        {
            entity.HasKey(e => e.IdUh)
                .HasName("PK__PG_fact___8B62595A25B613F0");
        });

        builder.Entity<PgRoleActions>(entity =>
        {
            entity.HasKey(e => new { e.RoleName, e.IdMa });

            entity.HasOne(d => d.IdMaNavigation)
                .WithMany(p => p.PgRoleActions)
                .HasForeignKey(d => d.IdMa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PG_role_actions_PG_cat_menu_actions");
        });

    builder.Entity<PgSelFamily>(entity =>
    {
        entity.HasKey(e => new { e.IdCategory, e.IdFam });

        entity.Property(e => e.IdCategory).IsFixedLength();

        entity.Property(e => e.IdFam).IsFixedLength();

        entity.HasOne(d => d.IdCategoryNavigation)
            .WithMany(p => p.PgSelFamily)
            .HasForeignKey(d => d.IdCategory)
            .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PG_sel_family_PG_cat_material_category");

            entity.HasOne(d => d.IdFamNavigation)
                .WithMany(p => p.PgSelFamily)
                .HasForeignKey(d => d.IdFam)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PG_sel_family_PG_cat_family");
        });

        builder.Entity<PgSelSalesForce>(entity =>
        {
            entity.HasKey(e => new { e.IdCategory, e.Username, e.SalesForce });

            entity.Property(e => e.IdCategory).IsFixedLength();

            entity.HasOne(d => d.IdCategoryNavigation)
                .WithMany(p => p.PgSelSalesForce)
                .HasForeignKey(d => d.IdCategory)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PG_sel_sales_force_PG_cat_material_category");
        });

        builder.Entity<PgStgCatManagersOthers>(entity =>
        {
            entity.HasKey(e => e.IdGa)
                .HasName("PK__PG_stg_c__8B62CF078A65394A");
        });

        builder.Entity<PgStgCatProducts>(entity =>
        {
            entity.HasKey(e => e.IdAp)
                .HasName("PK__PG_stg_c__8B623FD9F6C8B6B6");
        });

        builder.Entity<PgStgCatSalesForce>(entity =>
        {
            entity.HasKey(e => e.IdSf)
                .HasName("PK__PG_stg_c__8B63A995BB4967FC");
        });

        builder.Entity<PgStgFactInventories>(entity =>
        {
            entity.HasKey(e => e.IdIp)
                    .HasName("PK__PG_stg_f__8B62FCD328ED76FF");
        });

        builder.Entity<PgStgFactPurchaseOrder>(entity =>
        {
            entity.HasKey(e => e.IdPo )
                .HasName("PK__PG_stg_f__8B63902A0CCE9782");
        });

        builder.Entity<UserRecord>(entity =>
        {
            entity.HasNoKey();
        });

        builder.Entity<UserFamiliesReportRecord>(entity =>
        {
            entity.HasNoKey();
        });

        builder.Entity<PgUserFamily>(entity =>
        {
            entity.HasKey(e => new { e.IdFam, e.Colemp })
                .HasName("PK__PG_user___B5C34F098701EB33");

            entity.ToTable("PG_user_family");

            entity.Property(e => e.IdFam)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("ID_FAM")
                .IsFixedLength();

            entity.Property(e => e.Colemp)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("COLEMP");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
        });

        builder.Entity<PgSKUMaster>(entity =>
        {
            entity.HasKey(e => e.Code);
        });

        builder.Entity<SKUMasterView>(entity =>
        {
            entity.HasNoKey();
        });
    }
}
