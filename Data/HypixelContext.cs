using dev;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static hypixel.ItemDetails;

namespace hypixel
{
    public abstract class MyDbSet<TEntity> : DbSet<TEntity>  where TEntity : class
    {
        public TEntity GetOrCreateAndAdd(TEntity entity)
        {
            var value = this.Find(entity);
            if(value != null)
            {
                return value;
            }
            Add(entity);
            return entity;
        }

        public void AddOrUpdate(TEntity entity)
        {
            if(this.Contains(entity))
            {
                this.Update(entity);
            } else {
                this.Add(entity);
            }
        }
    }


    public class HypixelContext : DbContext {
        public DbSet<SaveAuction> Auctions { get; set; }

        public DbSet<SaveBids> Bids { get; set; }

        public DbSet<Player> Players {get;set;}

        public DbSet<ProductInfo> BazaarPrices {get;set;}
        public DbSet<BazaarPull> BazaarPull {get;set;}
        public DbSet<SubscribeItem> SubscribeItem {get;set;}
        public DbSet<DBItem> Items {get;set;}
        public DbSet<AlternativeName> AltItemNames {get;set;}

        public DbSet<AveragePrice> Prices {get;set;}
        public DbSet<Enchantment> Enchantment {get;set;}

        protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseMySQL ("server=mariadb;database=test;user=root;password=takenfrombitnami; convert zero datetime=True;Charset=utf8; Connect Timeout=3600",
            opts=>opts.CommandTimeout(3600));
            
        }

     

        protected override void OnModelCreating (ModelBuilder modelBuilder) {
            base.OnModelCreating (modelBuilder);
          

            modelBuilder.Entity<SaveAuction> (entity => {
                entity.HasIndex (e => e.Uuid).IsUnique();
                entity.HasIndex(e=>e.End);
                entity.HasIndex(e=>e.SellerId);
                entity.HasIndex(e=>new {e.ItemId,e.End});
                //entity.HasOne<NbtData>(d=>d.NbtData);
                //entity.HasMany<Enchantment>(e=>e.Enchantments);
            });

            modelBuilder.Entity<SaveBids> (entity => {
                entity.HasIndex (e => e.BidderId);
            });

            modelBuilder.Entity<NbtData> (entity => {
                entity.HasKey (e=>e.Id);
            });


            modelBuilder.Entity<ProductInfo>(entity=> {
                entity.HasKey(e=>e.Id);
                entity.HasIndex(e=>e.ProductId);
            });

            modelBuilder.Entity<BazaarPull>(entity=> {
                entity.HasKey(e=>e.Id);
                entity.HasIndex(e=>e.Timestamp);
            });


            modelBuilder.Entity<Player>(entity=> {
                entity.HasKey(e=>e.UuId);
                entity.HasIndex(e=>e.Name);
                entity.HasIndex(e=>e.Id);
                //entity.Property(e=>e.Id).ValueGeneratedOnAdd();
                //entity.HasMany(p=>p.Auctions).WithOne().HasForeignKey(a=>a.SellerId).HasPrincipalKey(p=>p.Id);
                //entity.HasMany(p=>p.Bids).WithOne().HasForeignKey(a=>a.BidderId).HasPrincipalKey(p=>p.Id);
            });


            modelBuilder.Entity<DBItem>(entity => {
                entity.HasKey(e=>e.Id);
                entity.HasIndex(e=>e.Tag).IsUnique();
            });

            modelBuilder.Entity<AlternativeName>(entity => {
                entity.HasIndex(e=>e.Name);
            });


            modelBuilder.Entity<AveragePrice>(entity => {
                entity.HasIndex(e=>new {e.ItemId,e.Date}).IsUnique();
            });
          
            modelBuilder.Entity<Enchantment>(entity => {
                entity.HasIndex(e=>new {e.ItemType,e.Type,e.Level});
            });
        }
    }
}