using System;
using System.Data.Entity;

namespace Sift.Server.Db
{
    [DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    internal class SettingContext : DbContext
    {
        public SettingContext() : base(Program.Database.CreateConnection(), true)
        {
            Configuration.ValidateOnSaveEnabled = false;
        }

        static SettingContext()
        {
            DbConfiguration.SetConfiguration(new MySql.Data.Entity.MySqlEFConfiguration());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Setting> Settings { get; set; }
    }
}
