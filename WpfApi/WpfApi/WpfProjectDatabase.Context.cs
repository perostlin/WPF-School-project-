﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WpfApi
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class WpfProjectDatabaseEntities : DbContext
    {
        public WpfProjectDatabaseEntities()
            : base("name=WpfProjectDatabaseEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Color> Color { get; set; }
        public virtual DbSet<FuelType> FuelType { get; set; }
        public virtual DbSet<ModelYear> ModelYear { get; set; }
        public virtual DbSet<RefuelingDriverJournal> RefuelingDriverJournal { get; set; }
        public virtual DbSet<ReportDriverJournal> ReportDriverJournal { get; set; }
        public virtual DbSet<TypeOfCost> TypeOfCost { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Vehicle> Vehicle { get; set; }
        public virtual DbSet<VehicleType> VehicleType { get; set; }
    }
}
