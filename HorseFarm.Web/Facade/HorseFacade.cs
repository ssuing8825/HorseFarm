using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HorseFarm.Web.Logging;
using HorseFarm.Web.Models.HorseData;
using HorseFarm.Web.Models;

namespace HorseFarm.Web.Facade
{
    public class HorseFacade
    {
        private IHorseRepository repository = new HorseRepository();


        public void Delete(int id)
        {
            repository.Delete(id);
        }

        public Models.Horse Get(int id)
        {
            new HorseServiceEvent("MyActivity").WithKeys(new EventKeys { ExceedClientId = id.ToString(), PolicyNumber = "123" }).Raise(
                "BouncedEmail",
                new
                {
                    Template = "B2BEstimatedPre...",
                    DateSent = "10/4/2012 1:28PM",
                    DateBounced = "10/4/2012 1:28PM"
                });


            return repository.Get(id);
        }

        public IEnumerable<Models.Horse> GetAll(params System.Linq.Expressions.Expression<Func<Models.Horse, object>>[] includeProperties)
        {
            return repository.GetAll();
        }

        public void Post(Models.Horse horse)
        {
            repository.Post(horse);
        }

        public void Update(int id, Models.Horse horse)
        {
            repository.Update(id, horse);
        }

        public void SaveChanges()
        {
            repository.SaveChanges();
        }
    }
}