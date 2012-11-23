using System;
namespace HorseFarm.Web.Models.HorseData
{
    interface IHorseRepository
    {
        void Delete(int id);
        HorseFarm.Web.Models.Horse Get(int id);
        System.Collections.Generic.IEnumerable<HorseFarm.Web.Models.Horse> GetAll(params System.Linq.Expressions.Expression<Func<HorseFarm.Web.Models.Horse, object>>[] includeProperties);
        void Post(HorseFarm.Web.Models.Horse horse);
        void Update(int id, HorseFarm.Web.Models.Horse horse);
        void SaveChanges();
    }
}
