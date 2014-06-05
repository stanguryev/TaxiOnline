using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using TaxiOnline.Server.DataAccess;

namespace TaxiOnline.Server.App_Start
{
    public class DatabaseModelInitializer : IDatabaseInitializer<DatabaseModel>
    {
        public void InitializeDatabase(DatabaseModel context)
        {
            context.Set<City>().Load();
            context.Set<PersonAccount>().Load();
            context.Set<PersonsInfo>().Load();
            context.Set<PedestrianAccount>().Load();
            context.Set<PedestriansInfo>().Load();
        }
    }
}
