using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using TaxiOnline.Server.DataObjects;
using TaxiOnline.Server.Models;
using TaxiOnline.Server.DataAccess;
using TaxiOnline.Server.DomainManagers;
using TaxiOnline.Server.Authentication;
using Microsoft.WindowsAzure.Mobile.Service.Security;

namespace TaxiOnline.Server.Controllers
{
    public class DriversController : TableController<DriverDTO>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            DatabaseModel context = new DatabaseModel();
            DomainManager = new DriversDomainManager(context, Request, Services);
        }

        [TaxiOnlineAuthorize]
        // GET tables/Drivers
        public IQueryable<DriverDTO> GetAllDriverDTO()
        {
            InteractionModel.Instance.Cities.ToArray();
            return Query();
        }

        [TaxiOnlineAuthorize]
        // GET tables/Drivers/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<DriverDTO> GetDriverDTO(string id)
        {
            return Lookup(id);
        }

        [AuthorizeLevel(AuthorizationLevel.Admin)]
        // PATCH tables/Drivers/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<DriverDTO> PatchDriverDTO(string id, Delta<DriverDTO> patch)
        {
            return UpdateAsync(id, patch);
        }

        [AuthorizeLevel(AuthorizationLevel.Admin)]
        // POST tables/Drivers/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task<IHttpActionResult> PostDriverDTO(DriverDTO item)
        {
            DriverDTO current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        [AuthorizeLevel(AuthorizationLevel.Admin)]
        // DELETE tables/Drivers/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteDriverDTO(string id)
        {
            return DeleteAsync(id);
        }

    }
}