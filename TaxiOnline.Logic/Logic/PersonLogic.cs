using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.Logic.Common;
using TaxiOnline.Logic.Models;

namespace TaxiOnline.Logic.Logic
{
    internal abstract class PersonLogic
    {
        private readonly PersonModel _personModel;
        protected readonly AdaptersExtender _adaptersExtender;
        protected readonly CityLogic _city;

        public PersonModel PersonModel
        {
            get { return _personModel; }
        }

        public PersonLogic(PersonModel model, AdaptersExtender adaptersExtender, CityLogic city)
        {
            _personModel = model;
            _adaptersExtender = adaptersExtender;
            _city = city;
        }
    }
}
