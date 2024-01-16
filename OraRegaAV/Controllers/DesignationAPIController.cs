using OraRegaAV.DBEntity;
using OraRegaAV.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace OraRegaAV.Controllers.API
{
    public class DesignationAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public DesignationAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpGet]
        [Route("api/DesignationAPI/Get")]
        public async Task<string> Get()
        {
            return await Task.Run(() => "Hello From Designation");
        }

        //[HttpPost]
        //[Route("api/DesignationAPI/SaveDesignation")]
        //public async Task<Response> SaveDesignation(tblDesignation objtblDesignation)
        //{
        //    //try
        //    //{
        //    //    var tbl = db.tblDesignations.Where(x => x.Id == objtblDesignation.Id).FirstOrDefault();
        //    //    if (tbl == null)
        //    //    {
        //    //        tbl = new tblDesignation();
        //    //        tbl.DesignationName = objtblDesignation.DesignationName;
        //    //        tbl.DepartmentId = objtblDesignation.DepartmentId;
        //    //        tbl.IsActive = objtblDesignation.IsActive;
        //    //        db.tblDesignations.Add(tbl);
        //    //        await db.SaveChangesAsync();
        //    //    }
        //    //    else
        //    //    {
        //    //        tbl.DesignationName = objtblDesignation.DesignationName;
        //    //        tbl.DepartmentId = objtblDesignation.DepartmentId;
        //    //        tbl.IsActive = objtblDesignation.IsActive;
        //    //        await db.SaveChangesAsync();
        //    //    }
        //    //    _response.IsSuccess = true;

        //    //}
        //    //catch
        //    //{
        //    //    _response.IsSuccess = false;
        //    //    throw;
        //    //}
        //    return _response;
        //}
        
        [HttpGet]
        [Route("api/DesignationAPI/GetById")]
        public Response GetById(int Id = 0)
        {
            //tblDesignation objtblDesignation = db.tblDesignations.Where( x => x.Id == Id).FirstOrDefault();

            //_response.Data = objtblDesignation;

            return _response;
        }

        [HttpGet]
        [Route("api/DesignationAPI/GetDesignationList")]
        public Response GetDesignationList()
        {
            //_response.Data = db.tblDesignations.ToList();

            return _response;
        }

    }
}