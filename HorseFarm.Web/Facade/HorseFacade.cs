using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HorseFarm.Web.Models.HorseData;
using HorseFarm.Web.Models;

namespace HorseFarm.Web.Facade
{
    public class HorseFacade
    {
        private IHorseRepository repository = new HorseRepository();
        private IHorseNotification notification = HorseNotification.GetHorseNotification();

        public void Delete(int id)
        {
            repository.Delete(id);
            notification.SendHorseAddedMessage(id);
        }

        public Models.Horse Get(int id)
        {
            return repository.Get(id);
        }

        public IEnumerable<Models.Horse> GetAll(params System.Linq.Expressions.Expression<Func<Models.Horse, object>>[] includeProperties)
        {
            return repository.GetAll();
        }

        public void Post(Models.Horse horse)
        {
            repository.Post(horse);
            notification.SendHorseChangedMessage(horse);
        }

        public void Update(int id, Models.Horse horse)
        {
            repository.Update(id, horse);
            notification.SendHorseChangedMessage(horse);
        }

        public void SaveChanges()
        {
            repository.SaveChanges();
        }
    }
}