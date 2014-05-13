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
            System.Configuration.Configuration exeConfiguration = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Reflection.Assembly.GetEntryAssembly().Location);
            System.Configuration.ConnectionStringsSection connectionStringsSection = exeConfiguration.Sections.OfType<System.Configuration.ConnectionStringsSection>().Single();
            Configuration configuration = new Configuration();
            configuration.Configure(GetType().Assembly, "TaxiOnline.Server.Data.hibernate.cfg.xml");
            configuration.Properties.Add("connection.connection_string", connectionStringsSection.ConnectionStrings["DefaultConnectionString"].ConnectionString);
            configuration.AddAssembly(GetType().Assembly);
            ISessionFactory sessionFactory = configuration.BuildSessionFactory();
            _session = sessionFactory.OpenSession();
        }
    }
}
