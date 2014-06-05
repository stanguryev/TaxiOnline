using Microsoft.WindowsAzure.Mobile.Service;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Web;
using TaxiOnline.Server.DataAccess;
using TaxiOnline.Server.DataObjects;

namespace TaxiOnline.Server.DomainManagers
{
    public class DriversDomainManager : MappedEntityDomainManager<DriverDTO, DriversInfo>
    {
        public DriversDomainManager(DbContext context, HttpRequestMessage request, ApiServices services)
            : base(context, request, services)
        {

        }

        public override System.Threading.Tasks.Task<bool> DeleteAsync(string id)
        {
            return DeleteItemAsync(id);
        }

        public override System.Web.Http.SingleResult<DriverDTO> Lookup(string id)
        {
            return LookupEntity(c => c.PersonsInfo.PersonId.ToString() == id);
        }

        public override System.Threading.Tasks.Task<DriverDTO> UpdateAsync(string id, System.Web.Http.OData.Delta<DriverDTO> patch)
        {
            return UpdateEntityAsync(patch, id);
        }
    }
}