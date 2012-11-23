using System;
namespace HorseFarm.Web.Models
{
    interface IHorseNotification
    {
        void SendHorseAddedMessage(int id);
        void SendHorseChangedMessage(Horse horse);
        void SendHorseDeletedMessage(int id);
    }
}
