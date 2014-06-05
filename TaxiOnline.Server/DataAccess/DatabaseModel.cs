namespace TaxiOnline.Server.DataAccess
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DatabaseModel : DbContext
    {
        public DatabaseModel()
            : base("name=DatabaseModel")
        {
        }

        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<CityName> CityNames { get; set; }
        public virtual DbSet<DriverAccount> DriverAccounts { get; set; }
        public virtual DbSet<DriverRespons> DriverResponses { get; set; }
        public virtual DbSet<DriversInfo> DriversInfoes { get; set; }
        public virtual DbSet<PedestrianAccount> PedestrianAccounts { get; set; }
        public virtual DbSet<PedestrianRequest> PedestrianRequests { get; set; }
        public virtual DbSet<PedestriansInfo> PedestriansInfoes { get; set; }
        public virtual DbSet<PersonAccount> PersonAccounts { get; set; }
        public virtual DbSet<PersonsInfo> PersonsInfoes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>()
                .HasMany(e => e.CityNames)
                .WithRequired(e => e.City)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<City>()
                .HasMany(e => e.PersonsInfo)
                .WithRequired(e => e.City)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DriversInfo>()
                .HasMany(e => e.PedestrianRequests)
                .WithRequired(e => e.DriversInfo)
                .HasForeignKey(e => e.Target)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PedestrianRequest>()
                .HasMany(e => e.DriverResponses)
                .WithRequired(e => e.PedestrianRequest)
                .HasForeignKey(e => e.RequestId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PedestriansInfo>()
                .HasMany(e => e.PedestrianRequests)
                .WithRequired(e => e.PedestriansInfo)
                .HasForeignKey(e => e.Author)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PersonAccount>()
                .HasMany(e => e.DriverAccounts)
                .WithRequired(e => e.PersonAccount)
                .HasForeignKey(e => e.PersonId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PersonAccount>()
                .HasMany(e => e.PedestrianAccounts)
                .WithRequired(e => e.PersonAccount)
                .HasForeignKey(e => e.PersonId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PersonAccount>()
                .HasMany(e => e.PersonsInfoes)
                .WithRequired(e => e.PersonAccount)
                .HasForeignKey(e => e.PersonId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PersonsInfo>()
                .HasMany(e => e.DriversInfoes)
                .WithRequired(e => e.PersonsInfo)
                .HasForeignKey(e => e.PersonInfo)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PersonsInfo>()
                .HasMany(e => e.PedestriansInfoes)
                .WithRequired(e => e.PersonsInfo)
                .HasForeignKey(e => e.PersonInfo)
                .WillCascadeOnDelete(false);
        }
    }
}
