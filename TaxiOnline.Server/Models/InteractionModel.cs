using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Data.Entity;
using TaxiOnline.Server.DataAccess;

namespace TaxiOnline.Server.Models
{
    public class InteractionModel
    {
        private static readonly Lazy<InteractionModel> _instance = new Lazy<InteractionModel>(() => new InteractionModel(), true);
        private readonly ObservableCollection<CityModel> _cities = new ObservableCollection<CityModel>();

        public IEnumerable<CityModel> Cities
        {
            get { return _cities; }
        }

        public static InteractionModel Instance
        {
            get { return InteractionModel._instance.Value; }
        }

        public void LoadDb()
        {
            City[] cities;
            using (DatabaseModel dbContext = new DatabaseModel())
            {
                dbContext.Set<City>().Load();
                cities = dbContext.Set<City>().ToArray();
            }
            _cities.Clear();
            foreach (City cityDA in cities)
            {
                CityModel city = new CityModel(cityDA);
                city.LoadDb();
                _cities.Add(city);
            }
        }
    }
}