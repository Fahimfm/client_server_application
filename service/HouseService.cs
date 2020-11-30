using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace service
{
   [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class HouseService : IService, IAdmin
    {
        //Some hard-coded lines for communication
        List<IServiceCallback> clients = new List<IServiceCallback>();
        List<House> houses = new List<House>() {
            new House() { Address="Rachelsmolen 1", Price= 100000, Capacity=3, Offers=new List<int>(), SellerName="Bob" },
            new House() { Address="Rachelsmolen 3", Price= 300000, Capacity=5, Offers=new List<int>(), SellerName="Tom" },
            new House() { Address="Karel de Grotelaan 45", Price= 200000, Capacity=6, Offers=new List<int>(), SellerName="Sully" }
        };

        public void Connect()
        {
            clients.Add(OperationContext.Current.GetCallbackChannel<IServiceCallback>());
        }

        public void Disconnect()
        {
            clients.Remove(OperationContext.Current.GetCallbackChannel<IServiceCallback>());
        }

        public int GetAskingPrice(string address)
        {
            foreach (House house in houses)
            {
                if (house.Address == address)
                {
                    return house.Price;
                }
            }
            return 0;

            
        }

        public List<House> GetAvailableHouses()
        {
            List<House> housesToReturn = new List<House>();
            foreach (House h in houses)
            {
                if (h.Capacity>h.Offers.Count)
                {
                    housesToReturn.Add(h);
                }
            }
            return housesToReturn;

            
        }

        public List<int> GetOffers(string address)
        {
            foreach (var h in houses)
            {
                if (h.Address==address)
                {
                    return h.Offers;
                }
            }
            return new List<int>(); 
           
        }

        
        public OfferStatus MakeAnOffer(string address, int offer)
        {
            IServiceCallback client = OperationContext.Current.GetCallbackChannel<IServiceCallback>();

          
            foreach (House h in GetAvailableHouses())
            {
                if (h.Address == address)
                {
                    h.Offers.Add(offer);
                    if (h.Capacity > h.Offers.Count)
                    {
                        return OfferStatus.Succeeded;
                    }
                    else
                    {
                        return OfferStatus.SucceededAndUnavailable;
                    }
                } 
            }

           
            foreach (IServiceCallback c in clients)
            {
                if (c!= client)
                {
                    c.HouseIsNotAvailable(address);
                }
            }
            return OfferStatus.Unavailable;
        }
    }
}
