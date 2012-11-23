using System.Data.Entity;
using System.Data;
using System.Data.Entity.Infrastructure;

namespace HorseFarm.Web.Models.HorseData
{
    public class HorseContext : DbContext
    {
        public HorseContext()
            : base("HorseFarm")
        {
        }

        public void ChangeObjectState<T>(T entity, EntityState entityState)
        {

            this.Configuration.ValidateOnSaveEnabled = false;
            this.Configuration.LazyLoadingEnabled = true;
            ((IObjectContextAdapter)this).ObjectContext.ObjectStateManager.ChangeObjectState(entity, entityState);

        }

        public DbSet<Horse> Horses { get; set; }

        public DbSet<Address> Addresses { get; set; }
    }

    public class Initializer : DropCreateDatabaseIfModelChanges<HorseContext>
    {
        public void InitializeDatabase(HorseContext context)
        {
            if (!context.Database.Exists() || !context.Database.CompatibleWithModel(true))
            {
                context.Database.Delete();
                context.Database.Create();
            }
        }
        protected override void Seed(HorseContext context)
        {

        //    var address = new Address { Id = 1, Street = "asdfasdf" };

            //var horse1 = new Horse { Id = 1, Name = "Sparkle", DOB = new System.DateTime(2012, 01, 01), Address = address, AddressId = 1 };
            //var horse2 = new Horse { Id = 2, Name = "Trying To Hard", DOB = new System.DateTime(2012, 01, 01), Address = address, AddressId = 1 };
            //var horse3 = new Horse { Id = 3, Name = "Sloppy Muck", DOB = new System.DateTime(2012, 01, 01), Address = address, AddressId = 1 };
            //var horse4 = new Horse { Id = 4, Name = "Birght Day", DOB = new System.DateTime(2012, 01, 01), Address = address, AddressId = 1 };

            var horse1 = new Horse { Id = 1, Name = "Sparkle", DOB = new System.DateTime(2012, 01, 01)};
            var horse2 = new Horse { Id = 2, Name = "Trying To Hard", DOB = new System.DateTime(2012, 01, 01) };
            var horse3 = new Horse { Id = 3, Name = "Sloppy Muck", DOB = new System.DateTime(2012, 01, 01) };
            var horse4 = new Horse { Id = 4, Name = "Birght Day", DOB = new System.DateTime(2012, 01, 01) };

            context.Horses.Add(horse1);
            context.Horses.Add(horse2);
            context.Horses.Add(horse3);
            context.Horses.Add(horse4);

            context.SaveChanges();
            base.Seed(context);

        }
    }
}
