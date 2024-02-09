using System;
using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.Entity.Core.Objects;

namespace OraRegaAV.Controllers.API
{
    public class BranchAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public BranchAPIController()
        {
            _response.IsSuccess = true;
        }

        [HttpPost]
        [Route("api/BranchAPI/SaveBranch")]
        public async Task<Response> SaveBranch(tblBranch objtblBranch)
        {
            try
            {
                var tbl = db.tblBranches.Where(x => x.Id == objtblBranch.Id).FirstOrDefault();

                if (tbl == null)
                {
                    tbl = new tblBranch();
                    tbl.BranchName = objtblBranch.BranchName;
                    tbl.CompanyId = objtblBranch.CompanyId;
                    tbl.AddressLine1 = objtblBranch.AddressLine1;
                    tbl.AddressLine2 = objtblBranch.AddressLine2;
                    tbl.StateId = objtblBranch.StateId;
                    tbl.CityId = objtblBranch.CityId;
                    tbl.AreaId = objtblBranch.AreaId;
                    tbl.PincodeId = objtblBranch.PincodeId;
                    tbl.DepartmentHead = objtblBranch.DepartmentHead;
                    tbl.MobileNo = objtblBranch.MobileNo;
                    tbl.EmailId = objtblBranch.EmailId;
                    tbl.IsActive = objtblBranch.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;
                    db.tblBranches.Add(tbl);

                    _response.Message = "Branch details saved successfully";
                }
                else
                {
                    tbl.BranchName = objtblBranch.BranchName;
                    tbl.CompanyId = objtblBranch.CompanyId;
                    tbl.AddressLine1 = objtblBranch.AddressLine1;
                    tbl.AddressLine2 = objtblBranch.AddressLine2;
                    tbl.StateId = objtblBranch.StateId;
                    tbl.CityId = objtblBranch.CityId;
                    tbl.AreaId = objtblBranch.AreaId;
                    tbl.PincodeId = objtblBranch.PincodeId;
                    tbl.DepartmentHead = objtblBranch.DepartmentHead;
                    tbl.MobileNo = objtblBranch.MobileNo;
                    tbl.EmailId = objtblBranch.EmailId;
                    tbl.IsActive = objtblBranch.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    _response.Message = "Branch details updated successfully";
                }

                await db.SaveChangesAsync();

                _response.IsSuccess = true;

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        [Route("api/BranchAPI/GetById")]
        public async Task<Response> GetById([FromBody] int Id)
        {
            GetBranchList_Result objtblBranch;

            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                objtblBranch = await Task.Run(() => db.GetBranchList(0, 0, "",0,0,vTotal,0).ToList().Where(x => x.Id == Id).FirstOrDefault());
                _response.Data = objtblBranch;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        [Route("api/BranchAPI/GetBranchList")]
        public async Task<Response> GetBranchList(BranchModel parameters)
        {
            List<GetBranchList_Result> branchList;
            try
            {
                var userId = Utilities.GetUserID(ActionContext.Request);
                var vTotal = new ObjectParameter("Total", typeof(int));
                branchList = await Task.Run(() => db.GetBranchList(parameters.CompanyId, parameters.BranchId, parameters.SearchValue, parameters.PageSize, parameters.PageNo,vTotal,userId).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = branchList;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        [Route("api/BranchAPI/GetBranchListByCompany")]
        public async Task<Response> GetBranchListByCompany([FromBody] int companyId)
        {
            List<GetBranchListByCompany_Result> branchList;
            try
            {
                branchList = await Task.Run(() => db.GetBranchListByCompany(companyId).ToList());

                _response.Data = branchList;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        [Route("api/BranchAPI/GetBranchListByState")]
        public async Task<Response> GetBranchListByState([FromBody] int stateId)
        {
            List<GetBranchListByState_Result> branchList;
            try
            {
                branchList = await Task.Run(() => db.GetBranchListByState(stateId).ToList());

                _response.Data = branchList;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [HttpPost]
        [Route("api/BranchAPI/GetGSTNStateCodeByCompanyNState")]
        public async Task<Response> GetGSTNStateCodeByCompanyNState([FromBody] int companyId, int stateId)
        {
            List<GetGST_N_StateCode_ByCompanyNState_Result> ObjList;
            try
            {
                ObjList = await Task.Run(() => db.GetGST_N_StateCode_ByCompanyNState(companyId, stateId).ToList());

                _response.Data = ObjList;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }
    }
}
