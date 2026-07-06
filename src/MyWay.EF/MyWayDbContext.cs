using Microsoft.EntityFrameworkCore;
using MyWay.Core.CarrierListings;
using MyWay.Core.Companies;
using MyWay.Core.Profiles;
using MyWay.Core.Resources;
using MyWay.Core.Reviews;
using MyWay.Core.Shipments;
using MyWay.Core.Users;
using MyWay.Core.Vehicles;
using MyWay.Core.Warehouses;
using MyWay.Core.Waybills;

namespace MyWay.EF;

public sealed class MyWayDbContext : DbContext
{
    public MyWayDbContext(DbContextOptions<MyWayDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Company> Companies => Set<Company>();

    public DbSet<CompanyMember> CompanyMembers => Set<CompanyMember>();

    public DbSet<CustomerProfile> CustomerProfiles => Set<CustomerProfile>();

    public DbSet<CarrierProfile> CarrierProfiles => Set<CarrierProfile>();

    public DbSet<DriverProfile> DriverProfiles => Set<DriverProfile>();

    public DbSet<Vehicle> Vehicles => Set<Vehicle>();

    public DbSet<Warehouse> Warehouses => Set<Warehouse>();

    public DbSet<ShipmentRequest> ShipmentRequests => Set<ShipmentRequest>();

    public DbSet<ShipmentOffer> ShipmentOffers => Set<ShipmentOffer>();

    public DbSet<ShipmentOrder> ShipmentOrders => Set<ShipmentOrder>();

    public DbSet<CarrierListing> CarrierListings => Set<CarrierListing>();

    public DbSet<ResourceReservation> ResourceReservations => Set<ResourceReservation>();

    public DbSet<Waybill> Waybills => Set<Waybill>();

    public DbSet<Review> Reviews => Set<Review>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyWayDbContext).Assembly);
    }
}
