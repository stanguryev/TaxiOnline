using NHibernate;
using NHibernate.Cfg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiOnline.Server.Data
{
    public class DataProxy
    {
        private ISession _session;

        public ISession Session
        {
            get { return _session; }
        }

        public DataProxy()
        {
            Configuration configuration = new Configuration();
            configuration.Configure(GetType().Assembly, "TaxiOnline.Server.Data.hibernate.cfg.xml");
            configuration.AddAssembly(GetType().Assembly);
            ISessionFactory sessionFactory = configuration.BuildSessionFactory();
            _session = sessionFactory.OpenSession();
        }
    }
}
