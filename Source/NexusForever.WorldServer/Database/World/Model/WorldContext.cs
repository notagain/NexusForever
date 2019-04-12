using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using NexusForever.Shared;
using NexusForever.Shared.Configuration;
using NexusForever.Shared.Database;

namespace NexusForever.WorldServer.Database.World.Model
{
    public partial class WorldContext : DbContext
    {
        public WorldContext()
        {
        }

        public WorldContext(DbContextOptions<WorldContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Entity> Entity { get; set; }
        public virtual DbSet<EntityVendor> EntityVendor { get; set; }
        public virtual DbSet<EntityVendorCategory> EntityVendorCategory { get; set; }
        public virtual DbSet<EntityVendorItem> EntityVendorItem { get; set; }

        public virtual DbSet<StoreCategory> StoreCategory { get; set; }
        public virtual DbSet<StoreOfferGroup> StoreOfferGroup { get; set; }
        public virtual DbSet<StoreOfferGroupCategory> StoreOfferGroupCategory { get; set; }
        public virtual DbSet<StoreOfferItem> StoreOfferItem { get; set; }
        public virtual DbSet<StoreOfferItemData> StoreOfferItemData { get; set; }
        public virtual DbSet<StoreOfferItemPrice> StoreOfferItemPrice { get; set; }
        public virtual DbSet<StoreOfferItemCurrency> StoreOfferItemCurrency { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseConfiguration(DatabaseManager.Config, DatabaseType.World);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entity>(entity =>
            {
                entity.ToTable("entity");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Area)
                    .HasColumnName("area")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Creature)
                    .HasColumnName("creature")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.DisplayInfo)
                    .HasColumnName("displayInfo")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Faction1)
                    .HasColumnName("faction1")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Faction2)
                    .HasColumnName("faction2")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.OutfitInfo)
                    .HasColumnName("outfitInfo")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Rx)
                    .HasColumnName("rx")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Ry)
                    .HasColumnName("ry")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Rz)
                    .HasColumnName("rz")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.World)
                    .HasColumnName("world")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.X)
                    .HasColumnName("x")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Y)
                    .HasColumnName("y")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Z)
                    .HasColumnName("z")
                    .HasDefaultValueSql("'0'");
            });

            modelBuilder.Entity<EntityVendor>(entity =>
            {
                entity.ToTable("entity_vendor");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.BuyPriceMultiplier)
                    .HasColumnName("buyPriceMultiplier")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.SellPriceMultiplier)
                    .HasColumnName("sellPriceMultiplier")
                    .HasDefaultValueSql("'1'");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.EntityVendor)
                    .HasForeignKey<EntityVendor>(d => d.Id)
                    .HasConstraintName("FK__entity_vendor_id__entity_id");
            });

            modelBuilder.Entity<EntityVendorCategory>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Index });

                entity.ToTable("entity_vendor_category");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Index)
                    .HasColumnName("index")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.LocalisedTextId)
                    .HasColumnName("localisedTextId")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.EntityVendorCategory)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__entity_vendor_category_id__entity_id");
            });

            modelBuilder.Entity<EntityVendorItem>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.Index });

                entity.ToTable("entity_vendor_item");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Index)
                    .HasColumnName("index")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.CategoryIndex)
                    .HasColumnName("categoryIndex")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ItemId)
                    .HasColumnName("itemId")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.IdNavigation)
                    .WithMany(p => p.EntityVendorItem)
                    .HasForeignKey(d => d.Id)
                    .HasConstraintName("FK__entity_vendor_item_id__entity_id");
            });

            modelBuilder.Entity<StoreCategory>(entity =>
            {
                entity.HasKey(e => new { e.Id });

                entity.ToTable("store_category");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ParentCategoryId)
                    .HasColumnName("parentId")
                    .HasDefaultValueSql("'26'");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType("varchar(150)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Index)
                    .HasColumnName("index")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Visible)
                    .HasColumnName("visible")
                    .HasDefaultValueSql("'0'");
            });

            modelBuilder.Entity<StoreOfferGroup>(entity =>
            {
                entity.HasKey(e => new { e.Id });

                entity.ToTable("store_offer_group");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.DisplayFlags)
                    .HasColumnName("displayFlags")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType("varchar(500)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Field2)
                    .HasColumnName("field_2")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Visible)
                    .HasColumnName("visible")
                    .HasDefaultValueSql("'0'");
            });

            modelBuilder.Entity<StoreOfferGroupCategory>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.CategoryId });

                entity.ToTable("store_offer_group_category");

                entity.Property(e => e.Id)
                    .HasColumnName("id");

                entity.Property(e => e.CategoryId)
                    .HasColumnName("categoryId");

                entity.Property(e => e.Index)
                    .HasColumnName("index");

                entity.Property(e => e.Visible)
                    .HasColumnName("visible")
                    .HasDefaultValueSql("'1'");
            });

            modelBuilder.Entity<StoreOfferItem>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.GroupId });

                entity.ToTable("store_offer_item");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.GroupId)
                    .HasColumnName("groupId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType("varchar(500)")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.DisplayFlags)
                    .HasColumnName("displayFlags")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Field6)
                    .HasColumnName("field_6")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Field7)
                    .HasColumnName("field_7")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Visible)
                    .HasColumnName("visible")
                    .HasDefaultValueSql("'0'");
            });

            modelBuilder.Entity<StoreOfferItemData>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.OfferId });

                entity.ToTable("store_offer_item_data");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("AUTO_INCREMENT");

                entity.Property(e => e.OfferId)
                    .HasColumnName("offerId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ItemId)
                    .HasColumnName("itemId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Amount)
                    .HasDefaultValueSql("'1'");
            });

            modelBuilder.Entity<StoreOfferItemPrice>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.OfferId });

                entity.ToTable("store_offer_item_price");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("AUTO_INCREMENT");

                entity.Property(e => e.OfferId)
                    .HasColumnName("offerId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Field1)
                    .HasColumnName("field_1")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Field2)
                    .HasColumnName("field_2")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Field3)
                    .HasColumnName("field_3")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Field4)
                    .HasColumnName("field_4")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Field5)
                    .HasColumnName("field_5")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Field6)
                    .HasColumnName("field_6")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Omnibits)
                    .HasColumnName("omnibits")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Field8)
                    .HasColumnName("field_8")
                    .HasDefaultValueSql("'0'");
            });

            modelBuilder.Entity<StoreOfferItemCurrency>(entity =>
            {
                entity.HasKey(e => new { e.OfferId, e.CurrencyId });

                entity.ToTable("store_offer_item_currency");

                entity.Property(e => e.OfferId)
                    .HasColumnName("offerId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.CurrencyId)
                    .HasColumnName("currencyId")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Field11)
                    .HasColumnName("field_11")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Field12)
                    .HasColumnName("field_12")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Field13)
                    .HasColumnName("field_13")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Field14)
                    .HasColumnName("field_14")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Expiry)
                    .HasColumnName("expiry")
                    .HasDefaultValueSql("'0'");
            });
        }
    }
}
