using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using TaxiOnline.Server.DataAccess;
using TaxiOnline.Server.Models;
using TaxiOnline.Server.DomainManagers;
using TaxiOnline.Server.DataObjects;
using TaxiOnline.Server.Authentication;
using Microsoft.WindowsAzure.Mobile.Service.Security;

namespace TaxiOnline.Server.Controllers
{

    public class PedestriansController : TableController<PedestrianDTO>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            DatabaseModel context = new DatabaseModel();
            DomainManager = new PedestriansDomainManager(context, Request, Services);
        }

        [TaxiOnlineAuthorize]
        // GET tables/Pedestrians
        public IQueryable<PedestrianDTO> GetAllPedestrianModel()
        {
            return Query();
        }

        [TaxiOnlineAuthorize]
        // GET tables/Pedestrians/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<PedestrianDTO> GetPedestrianModel(string id)
        {
            return Lookup(id);
        }

        [AuthorizeLevel(AuthorizationLevel.Admin)]
        // PATCH tables/Pedestrians/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<PedestrianDTO> PatchPedestrianModel(string id, Delta<PedestrianDTO> patch)
        {
            return UpdateAsync(id, patch);
        }

        [AuthorizeLevel(AuthorizationLevel.Admin)]
        // POST tables/Pedestrians/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task<IHttpActionResult> PostPedestrianModel(PedestrianDTO item)
        {
            PedestrianDTO current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        [AuthorizeLevel(AuthorizationLevel.Admin)]
        // DELETE tables/Pedestrians/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeletePedestrianModel(string id)
        {
            return DeleteAsync(id);
        }

    }
}