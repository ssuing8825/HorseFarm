using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;


namespace HorseFarm.Web.Models.HorseData
{
    public class HorseRepository : HorseFarm.Web.Models.HorseData.IHorseRepository
    {
        private IList<Horse> horses;
        private int nextID;

        private HorseContext context = new HorseContext();


        public HorseRepository()
        {


        }



        public IEnumerable<Horse> GetAll(params Expression<Func<Horse, object>>[] includeProperties)
        {
            IQueryable<Horse> query = context.Horses;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query.ToList().OrderBy(c => c.Name);
        }

        public Horse Get(int id)
        {
            return context.Horses.Find(id);
        }
        public void Update(int id, Horse horse)
        {
            context.Horses.Attach(horse);
            context.Entry(horse).State = EntityState.Modified;

        }
        public void SaveChanges()
        {
            context.SaveChanges();
        }
        public void Post(Horse horse)
        {
            context.Horses.Add(horse);
        }

        public void Delete(int id)
        {
            var horse = context.Horses.Find(id);
            context.Horses.Remove(horse);
        }

    }
}