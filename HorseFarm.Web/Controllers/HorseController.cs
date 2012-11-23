using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using HorseFarm.Web.Models;
using HorseFarm.Web.Models.HorseData;
using HorseFarm.Web.Facade;

namespace HorseFarm.Web.Controllers
{
    public class HorseController : ApiController
    {
        private HorseFacade facade = new HorseFacade();

        // GET api/Horse
        public IEnumerable<Horse> GetHorses()
        {
            return facade.GetAll();
        }

        // GET api/Horse/5
        public Horse GetHorse(int id)
        {
            Horse horse = facade.Get(id);
            if (horse == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return horse;
        }

        // PUT api/Horse/5
        public HttpResponseMessage PutHorse(int id, Horse horse)
        {
            if (ModelState.IsValid && id == horse.Id)
            {
                facade.Update(id, horse);

                try
                {
                    facade.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // POST api/Horse
        public HttpResponseMessage PostHorse(Horse horse)
        {
            if (ModelState.IsValid)
            {
                facade.Post(horse);
                facade.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, horse);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = horse.Id }));
                return response;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // DELETE api/Horse/5
        public HttpResponseMessage DeleteHorse(int id)
        {
            Horse horse = facade.Get(id);
            if (horse == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            facade.Delete(id);

            try
            {
                facade.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, horse);
        }
    }
}