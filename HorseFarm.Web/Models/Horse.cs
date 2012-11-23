using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HorseFarm.Web.Models
{
    public class Horse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DOB { get; set; }
        //public virtual Address Address { get; set; }
        //public int AddressId { get; set; }
    }

    public class Address
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
    }
}