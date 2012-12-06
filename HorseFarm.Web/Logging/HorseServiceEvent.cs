using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Geico.Applications.Foundation.EventProcessing;

namespace HorseFarm.Web.Logging
{

    public class HorseServiceEvent : BusinessEvent<EventKeys>
    {
        public HorseServiceEvent(string eventClassification)
            : base(eventClassification)
        {
        }


        public override string ApplicationId
        {
            get { return "HorseFarm"; }
        }
    }
}