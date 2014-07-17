using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using TaxiOnline.Server.DataAccess;
using TaxiOnline.Toolkit.Threading.CollectionsDecorators;
using TaxiOnline.Server.DataObjects;

namespace TaxiOnline.Server.Models
{
    public class CityModel
    {
        private readonly City _cityDA;
        private readonly ReadonlyCollectionDecorator<PedestrianModel> _pedestrians = new ReadonlyCollectionDecorator<PedestrianModel>();

        public Guid Id
        {
            get { return _cityDA.Id; }
        }

        public double InitialLatitude
        {
            get { return _cityDA.InitialLatitude; }
        }

        public double InitialLongitude
        {
            get { return _cityDA.InitialLongitude; }
        }

        public double InitialZoom
        {
            get { return _cityDA.InitialZoom; }
        }

        public string PhoneConstraintPattern
        {
            get { return _cityDA.PhoneConstraintPattern; }
        }

        public string PhoneCorrectionPattern
        {
            get { return _cityDA.PhoneCorrectionPattern; }
        }

        public CityModel(City cityDA)
        {
            _cityDA = cityDA;
        }

        public void LoadDb()
        {
            PedestriansInfo[] cities;
            using (DatabaseModel dbContext = new DatabaseModel())
            {
                dbContext.Set<PedestriansInfo>().Load();
                cities = dbContext.Set<PedestriansInfo>().ToArray();
            }
            _pedestrians.ModifyCollection(col =>
            {
                col.Clear();
                foreach (PedestriansInfo pedestrianDA in cities)
                {
                    PedestrianModel pedestrian = new PedestrianModel(this, pedestrianDA);
                    pedestrian.LoadDb();
                    col.Add(pedestrian);
                }
            });
        }

        public void AddPedestrian(PedestrianDTO pedestrianDTO)
        {
            PedestrianModel pedestrian = new PedestrianModel(this, pedestrianDTO);
            pedestrian.SaveDb();
            _pedestrians.ModifyCollection(col => col.Add(pedestrian));
        }
    }
}