 using Newtonsoft.Json;
using OraRegaAV.Controllers.API;
using OraRegaAV.DBEntity;
using OraRegaAV.Helpers;
using OraRegaAV.Models;
using OraRegaAV.Models.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace OraRegaAV.Controllers
{
    public class WebsiteAPIController : ApiCustomBaseController
    {
        private readonly dbOraRegaEntities db = new dbOraRegaEntities();
        private Response _response = new Response();
        public WebsiteAPIController()
        {
            _response.IsSuccess = true;
        }

        #region Banner API

        [HttpPost]
        [Route("api/WebsiteAPI/SaveBanner")]
        public async Task<Response> SaveBanner()
        {
            string jsonParameter;
            tblBanner parameters, tbl;
            HttpFileCollection postedFiles;
            FileManager fileManager;

            try
            {
                fileManager = new FileManager();
                postedFiles = HttpContext.Current.Request.Files;

                #region Parameters Initialization
                jsonParameter = HttpContext.Current.Request.Form["Parameters"];

                if (string.IsNullOrEmpty(jsonParameter))
                {
                    _response.IsSuccess = false;
                    _response.Message = "Please provide parameters for this request";
                    return _response;
                }

                parameters = JsonConvert.DeserializeObject<tblBanner>(jsonParameter);

                if (postedFiles.Count > 0)
                {
                    parameters.ImageFile = postedFiles["ImageFile"].FileName;
                }
                #endregion

                #region Validation Check
                TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(tblBanner), typeof(TblBannerMetadata)), typeof(tblBanner));
                _response = ValueSanitizerHelper.GetValidationErrorsList(parameters);

                if (!_response.IsSuccess)
                {
                    return _response;
                }
                #endregion

                #region Banner Record Saving
                tbl = await db.tblBanners.Where(c => c.Id == parameters.Id).FirstOrDefaultAsync();

                if (tbl == null)
                {
                    tbl = new tblBanner();
                    tbl.LinkName = parameters.LinkName;
                    tbl.Position = parameters.Position;
                    tbl.AppType = parameters.AppType;
                    tbl.ImageFile = parameters.ImageFile;
                    tbl.IsActive = parameters.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;

                    if (postedFiles.Count > 0)
                    {
                        fileManager = new FileManager();

                        if (postedFiles["ImageFile"] != null)
                        {
                            tbl.ImageFilePath = fileManager.UploadBanner(postedFiles["ImageFile"], HttpContext.Current);
                        }
                    }

                    db.tblBanners.Add(tbl);
                    await db.SaveChangesAsync();

                    _response.IsSuccess = true;
                    _response.Message = "Banner details saved successfully";
                }
                else
                {
                    tbl.LinkName = parameters.LinkName;
                    tbl.Position = parameters.Position;
                    tbl.AppType = parameters.AppType;
                    if (!string.IsNullOrEmpty(parameters.ImageFile))
                    {
                        tbl.ImageFile = parameters.ImageFile;
                    }
                    tbl.IsActive = parameters.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    if (postedFiles.Count > 0)
                    {
                        fileManager = new FileManager();

                        if (postedFiles["ImageFile"] != null)
                        {
                            tbl.ImageFilePath = fileManager.UploadBanner(postedFiles["ImageFile"], HttpContext.Current);
                        }
                    }

                    await db.SaveChangesAsync();

                    _response.IsSuccess = true;
                    _response.Message = "Banner details updated successfully";
                }
                #endregion
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
        [Route("api/WebsiteAPI/GetBannerById")]
        public async Task<Response> GetBannerById([FromBody] int Id)
        {
            var host = Url.Content("~/");
            GetBannerList_Result bannerList_Result;
            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                bannerList_Result = await Task.Run(() => db.GetBannerList("", null,"",0,0, vTotal).ToList().Where(x => x.Id == Id).FirstOrDefault());

                if (bannerList_Result != null)
                {
                    if (!string.IsNullOrEmpty(bannerList_Result.ImageFilePath))
                    {
                        var path = host + "Uploads/Banner/" + bannerList_Result.ImageFilePath;
                        bannerList_Result.ImageFileUrl = path;
                    }
                }

                _response.Data = bannerList_Result;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/WebsiteAPI/GetBannerList")]
        public async Task<Response> GetBannerList(WebsiteSearchParameter request)
        {
            var host = Url.Content("~/");
            List<GetBannerList_Result> bannerList;

            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                bannerList = await Task.Run(() => db.GetBannerList(request.AppType, request.IsActive, request.SearchValue, request.PageSize, request.PageNo, vTotal).ToList());
                foreach (var obj in bannerList)
                {
                    if (!string.IsNullOrEmpty(obj.ImageFilePath))
                    {
                        var path = host + "Uploads/Banner/" + obj.ImageFilePath;
                        obj.ImageFileUrl = path;
                    }
                }

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = bannerList;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        #endregion

        #region OfferAds API

        [HttpPost]
        [Route("api/WebsiteAPI/SaveOfferAds")]
        public async Task<Response> SaveOfferAds()
        {
            string jsonParameter;
            tblOfferAd parameters, tbl;
            HttpFileCollection postedFiles;
            FileManager fileManager;

            try
            {
                fileManager = new FileManager();
                postedFiles = HttpContext.Current.Request.Files;

                #region Parameters Initialization
                jsonParameter = HttpContext.Current.Request.Form["Parameters"];

                if (string.IsNullOrEmpty(jsonParameter))
                {
                    _response.IsSuccess = false;
                    _response.Message = "Please provide parameters for this request";
                    return _response;
                }

                parameters = JsonConvert.DeserializeObject<tblOfferAd>(jsonParameter);

                if (postedFiles.Count > 0)
                {
                    parameters.ImageFile = postedFiles["ImageFile"].FileName;
                }
                #endregion

                #region Validation Check
                TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(tblBanner), typeof(TblBannerMetadata)), typeof(tblBanner));
                _response = ValueSanitizerHelper.GetValidationErrorsList(parameters);

                if (!_response.IsSuccess)
                {
                    return _response;
                }
                #endregion

                #region OfferAds Record Saving
                tbl = await db.tblOfferAds.Where(c => c.Id == parameters.Id).FirstOrDefaultAsync();

                if (tbl == null)
                {
                    tbl = new tblOfferAd();
                    tbl.LinkName = parameters.LinkName;
                    tbl.Position = parameters.Position;
                    tbl.AppType = parameters.AppType;
                    tbl.ImageFile = parameters.ImageFile;
                    tbl.IsActive = parameters.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;

                    if (postedFiles.Count > 0)
                    {
                        fileManager = new FileManager();

                        if (postedFiles["ImageFile"] != null)
                        {
                            tbl.ImageFilePath = fileManager.UploadOfferAds(postedFiles["ImageFile"], HttpContext.Current);
                        }
                    }

                    db.tblOfferAds.Add(tbl);
                    await db.SaveChangesAsync();

                    _response.IsSuccess = true;
                    _response.Message = "Offer Ads details saved successfully";
                }
                else
                {
                    tbl.LinkName = parameters.LinkName;
                    tbl.Position = parameters.Position;
                    tbl.AppType = parameters.AppType;
                    if (!string.IsNullOrEmpty(parameters.ImageFile))
                    {
                        tbl.ImageFile = parameters.ImageFile;
                    }
                    tbl.IsActive = parameters.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    if (postedFiles.Count > 0)
                    {
                        fileManager = new FileManager();

                        if (postedFiles["ImageFile"] != null)
                        {
                            tbl.ImageFilePath = fileManager.UploadOfferAds(postedFiles["ImageFile"], HttpContext.Current);
                        }
                    }

                    await db.SaveChangesAsync();

                    _response.IsSuccess = true;
                    _response.Message = "Offer Ads details updated successfully";
                }
                #endregion
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
        [Route("api/WebsiteAPI/GetOfferAdsById")]
        public async Task<Response> GetOfferAdsById([FromBody] int Id)
        {
            var host = Url.Content("~/");
            GetOfferAdsList_Result offerAdsList_Result;
            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                offerAdsList_Result = await Task.Run(() => db.GetOfferAdsList("", null,"",0,0, vTotal).ToList().Where(x => x.Id == Id).FirstOrDefault());

                if (offerAdsList_Result != null)
                {
                    if (!string.IsNullOrEmpty(offerAdsList_Result.ImageFilePath))
                    {
                        var path = host + "Uploads/OfferAds/" + offerAdsList_Result.ImageFilePath;
                        offerAdsList_Result.ImageFileUrl = path;
                    }
                }

                _response.Data = offerAdsList_Result;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/WebsiteAPI/GetOfferAdsList")]
        public async Task<Response> GetOfferAdsList(WebsiteSearchParameter request)
        {
            var host = Url.Content("~/");
            List<GetOfferAdsList_Result> offerAdsList;

            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                offerAdsList = await Task.Run(() => db.GetOfferAdsList(request.AppType, request.IsActive, request.SearchValue, request.PageSize, request.PageNo, vTotal).ToList());
                foreach (var obj in offerAdsList)
                {
                    if (!string.IsNullOrEmpty(obj.ImageFilePath))
                    {
                        var path = host + "Uploads/OfferAds/" + obj.ImageFilePath;
                        obj.ImageFileUrl = path;
                    }
                }

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = offerAdsList;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        #endregion

        #region Our Service API

        [HttpPost]
        [Route("api/WebsiteAPI/SaveOurService")]
        public async Task<Response> SaveOurService(OurService_Request parameters)
        {
            tblOurService tbl;
            FileManager fileManager;

            try
            {
                fileManager = new FileManager();

                #region Our Service Record Saving
                tbl = await db.tblOurServices.Where(c => c.Id == parameters.Id).FirstOrDefaultAsync();

                if (tbl == null)
                {
                    tbl = new tblOurService();
                    tbl.Name = parameters.Name;
                    tbl.Link = parameters.Link;
                    tbl.ContentName = parameters.ContentName;
                    tbl.Position = parameters.Position;
                    tbl.AppType = parameters.AppType;
                    tbl.IsActive = parameters.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;

                    if (!string.IsNullOrWhiteSpace(parameters.Files))
                    {
                        var imageName = Guid.NewGuid().ToString() + "_" + parameters.FilesName;
                        fileManager.UploadOurService(parameters.Files, imageName, HttpContext.Current);

                        tbl.ImageFile = imageName;
                    }

                    db.tblOurServices.Add(tbl);
                    await db.SaveChangesAsync();

                    _response.IsSuccess = true;
                    _response.Message = "Our Service details saved successfully";
                }
                else
                {
                    tbl.Name = parameters.Name;
                    tbl.Link = parameters.Link;
                    tbl.ContentName = parameters.ContentName;
                    tbl.Position = parameters.Position;
                    tbl.AppType = parameters.AppType;
                    tbl.IsActive = parameters.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    if (!string.IsNullOrWhiteSpace(parameters.Files))
                    {
                        var imageName = Guid.NewGuid().ToString() + "_" + parameters.FilesName;
                        fileManager.UploadOurService(parameters.Files, imageName, HttpContext.Current);

                        tbl.ImageFile = imageName;
                    }

                    await db.SaveChangesAsync();

                    _response.IsSuccess = true;
                    _response.Message = "Our Service details updated successfully";
                }
                #endregion
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
        [Route("api/WebsiteAPI/GetOurServiceById")]
        public async Task<Response> GetOurServiceById([FromBody] int Id)
        {
            var host = Url.Content("~/");
            GetOurServiceList_Result ourServiceList_Result;
            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                ourServiceList_Result = await Task.Run(() => db.GetOurServiceList("", null,"",0,0,vTotal).ToList().Where(x => x.Id == Id).FirstOrDefault());

                if (ourServiceList_Result != null)
                {
                    if (!string.IsNullOrEmpty(ourServiceList_Result.ImageFile))
                    {
                        var path = host + "Uploads/OurService/" + ourServiceList_Result.ImageFile;
                        ourServiceList_Result.ImageFileUrl = path;
                    }
                }

                _response.Data = ourServiceList_Result;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/WebsiteAPI/GetOurServiceList")]
        public async Task<Response> GetOurServiceList(WebsiteSearchParameter request)
        {
            var host = Url.Content("~/");
            List<GetOurServiceList_Result> ourServiceList;

            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                ourServiceList = await Task.Run(() => db.GetOurServiceList(request.AppType, request.IsActive, request.SearchValue, request.PageSize, request.PageNo, vTotal).ToList());
                foreach (var obj in ourServiceList)
                {
                    if (!string.IsNullOrEmpty(obj.ImageFile))
                    {
                        var path = host + "Uploads/OurService/" + obj.ImageFile;
                        obj.ImageFileUrl = path;
                    }
                }

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = ourServiceList;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        #endregion

        #region Our Product API

        [HttpPost]
        [Route("api/WebsiteAPI/SaveOurProduct")]
        public async Task<Response> SaveOurProduct(OurProduct_Request parameters)
        {
            tblOurProduct tbl;
            FileManager fileManager;

            try
            {
                fileManager = new FileManager();

                #region Our Product Record Saving
                tbl = await db.tblOurProducts.Where(c => c.Id == parameters.Id).FirstOrDefaultAsync();

                if (tbl == null)
                {
                    tbl = new tblOurProduct();
                    tbl.Name = parameters.Name;
                    tbl.Link = parameters.Link;
                    tbl.ContentName = parameters.ContentName;
                    tbl.Position = parameters.Position;
                    tbl.AppType = parameters.AppType;
                    tbl.IsActive = parameters.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;

                    if (!string.IsNullOrWhiteSpace(parameters.Files))
                    {
                        var imageName = Guid.NewGuid().ToString() + "_" + parameters.FilesName;
                        fileManager.UploadWebsiteOurProduct(parameters.Files, imageName, HttpContext.Current);

                        tbl.ImageFile = imageName;
                    }

                    db.tblOurProducts.Add(tbl);
                    await db.SaveChangesAsync();

                    _response.IsSuccess = true;
                    _response.Message = "Our Product details saved successfully";
                }
                else
                {
                    tbl.Name = parameters.Name;
                    tbl.Link = parameters.Link;
                    tbl.ContentName = parameters.ContentName;
                    tbl.Position = parameters.Position;
                    tbl.AppType = parameters.AppType;
                    tbl.IsActive = parameters.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    if (!string.IsNullOrWhiteSpace(parameters.Files))
                    {
                        var imageName = Guid.NewGuid().ToString() + "_" + parameters.FilesName;
                        fileManager.UploadWebsiteOurProduct(parameters.Files, imageName, HttpContext.Current);

                        tbl.ImageFile = imageName;
                    }

                    await db.SaveChangesAsync();

                    _response.IsSuccess = true;
                    _response.Message = "Our Product details updated successfully";
                }
                #endregion
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
        [Route("api/WebsiteAPI/GetOurProductById")]
        public async Task<Response> GetOurProductById([FromBody] int Id)
        {
            var host = Url.Content("~/");
            GetOurProductList_Result ourProductList_Result;
            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                ourProductList_Result = await Task.Run(() => db.GetOurProductList("", null,"",0,0, vTotal).ToList().Where(x => x.Id == Id).FirstOrDefault());

                if (ourProductList_Result != null)
                {
                    if (!string.IsNullOrEmpty(ourProductList_Result.ImageFile))
                    {
                        var path = host + "Uploads/OurProduct/" + ourProductList_Result.ImageFile;
                        ourProductList_Result.ImageFileUrl = path;
                    }
                }

                _response.Data = ourProductList_Result;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/WebsiteAPI/GetOurProductList")]
        public async Task<Response> GetOurProductList(WebsiteSearchParameter request)
        {
            var host = Url.Content("~/");
            List<GetOurProductList_Result> ourProductList;

            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                ourProductList = await Task.Run(() => db.GetOurProductList(request.AppType, request.IsActive, request.SearchValue, request.PageSize, request.PageNo, vTotal).ToList());
                foreach (var obj in ourProductList)
                {
                    if (!string.IsNullOrEmpty(obj.ImageFile))
                    {
                        var path = host + "Uploads/OurProduct/" + obj.ImageFile;
                        obj.ImageFileUrl = path;
                    }
                }

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = ourProductList;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        #endregion

        #region Testimonial API

        [HttpPost]
        [Route("api/WebsiteAPI/SaveTestimonial")]
        public async Task<Response> SaveTestimonial()
        {
            string jsonParameter;
            tblTestimonial parameters;
            HttpFileCollection postedFiles = HttpContext.Current.Request.Files;
            FileManager fileManager = new FileManager();

            try
            {
                #region Parameters Initialization
                if (postedFiles["TestimonialLogo"] != null && (postedFiles["TestimonialLogo"].ContentLength / 1024 / 1024) > 4)
                {
                    _response.IsSuccess = false;
                    _response.Message = "logo image cannot be more than 4 MB";
                    return _response;
                }

                jsonParameter = HttpContext.Current.Request.Form["Parameters"];

                if (string.IsNullOrEmpty(jsonParameter))
                {
                    _response.IsSuccess = false;
                    _response.Message = "Please provide parameters for this request";
                    return _response;
                }

                parameters = JsonConvert.DeserializeObject<tblTestimonial>(jsonParameter);

                #endregion

                bool isStatusNameExists;
                var tbl = db.tblTestimonials.Where(x => x.Id == parameters.Id).FirstOrDefault();
                string Msg = string.Empty;
                if (tbl == null)
                {
                    tbl = new tblTestimonial();
                    tbl.Name = parameters.Name;
                    tbl.Content = parameters.Content;
                    tbl.Position = parameters.Position;
                    tbl.IsActive = parameters.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;

                    if (postedFiles.Count > 0)
                    {
                        fileManager = new FileManager();

                        if (postedFiles["TestimonialLogo"] != null)
                        {
                            tbl.FileName = fileManager.UploadCompanyLogo(postedFiles["TestimonialLogo"], HttpContext.Current);
                        }
                    }

                    db.tblTestimonials.Add(tbl);

                    _response.IsSuccess = true;
                    _response.Message = "Testimonial saved successfully";

                    // Msg = "Testimonial saved successfully";
                }
                else
                {
                    tbl.Name = parameters.Name;
                    tbl.Content = parameters.Content;
                    tbl.Position = parameters.Position;
                    tbl.IsActive = parameters.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    if (postedFiles.Count > 0)
                    {
                        fileManager = new FileManager();

                        if (postedFiles["TestimonialLogo"] != null)
                        {
                            tbl.FileName = fileManager.UploadCompanyLogo(postedFiles["TestimonialLogo"], HttpContext.Current);
                        }
                    }

                    _response.IsSuccess = true;
                    _response.Message = "Testimonial updated successfully";

                    // Msg = "Testimonial updated successfully";
                }

                await db.SaveChangesAsync();
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
        [Route("api/WebsiteAPI/GetTestimonialId")]
        public async Task<Response> GetTestimonialId([FromBody] int Id)
        {
            var host = Url.Content("~/");
            GetTestimonialList_Result caseStatusList_Result;

            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                caseStatusList_Result = await Task.Run(() => db.GetTestimonialList(null,"",0,0, vTotal).ToList().Where(x => x.Id == Id).FirstOrDefault());

                if (caseStatusList_Result != null && !string.IsNullOrEmpty(caseStatusList_Result.FileName))
                {
                    var path = host + "Uploads/CompanyLogo/" + caseStatusList_Result.FileName;
                    caseStatusList_Result.FileUrl = path;
                }

                _response.Data = caseStatusList_Result;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/WebsiteAPI/GetTestimonialList")]
        public async Task<Response> GetTestimonialList(WebsiteSerachParameter parameter)
        {
            var host = Url.Content("~/");
            List<GetTestimonialList_Result> caseStatusList;
            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                caseStatusList = await Task.Run(() => db.GetTestimonialList(parameter.IsActive, parameter.SearchValue, parameter.PageSize, parameter.PageNo, vTotal).ToList());

                foreach (var item in caseStatusList)
                {
                    if (!string.IsNullOrEmpty(item.FileName))
                    {
                        var path = host + "Uploads/CompanyLogo/" + item.FileName;
                        item.FileUrl = path;
                    }
                }

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = caseStatusList;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        #endregion


        #region Refund and Cancellation Policy

        [HttpPost]
        [Route("api/WebsiteAPI/SaveRefundAndCancellationPolicy")]
        public async Task<Response> SaveRefundAndCancellationPolicy(RefundAndCancellationPolicyRequest parameters)
        {
            try
            {
                var tbl = db.tblRefundAndCancellationPolicies.Where(x => x.Id == parameters.Id).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblRefundAndCancellationPolicy();

                    tbl.RefundAndCancellationPolicy = parameters.RefundAndCancellationPolicy;
                    tbl.IsActive = parameters.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;

                    db.tblRefundAndCancellationPolicies.Add(tbl);

                    _response.Message = "Refund And Cancellation Policies saved successfully";
                }
                else
                {
                    tbl.RefundAndCancellationPolicy = parameters.RefundAndCancellationPolicy;
                    tbl.IsActive = parameters.IsActive;
                    tbl.IsActive = parameters.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    _response.Message = "Refund And Cancellation Policies updated successfully";
                }

                await db.SaveChangesAsync();
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
        [Route("api/WebsiteAPI/GetRefundAndCancellationPolicyId")]
        public async Task<Response> GetRefundAndCancellationPolicyId([FromBody] int Id)
        {
            GetRefundAndCancellationPolicyList_Result caseStatusList_Result;
            try
            {
                caseStatusList_Result = await Task.Run(() => db.GetRefundAndCancellationPolicyList(null).ToList().Where(x => x.Id == Id).FirstOrDefault());

                _response.Data = caseStatusList_Result;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/WebsiteAPI/GetRefundAndCancellationPolicyList")]
        public async Task<Response> GetRefundAndCancellationPolicyList(WebsiteSerachParameter parameter)
        {
            var host = Url.Content("~/");
            List<GetRefundAndCancellationPolicyList_Result> caseStatusList;
            try
            {
                caseStatusList = await Task.Run(() => db.GetRefundAndCancellationPolicyList(parameter.IsActive).ToList());

                _response.Data = caseStatusList;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        #endregion

        #region Payment Policy

        [HttpPost]
        [Route("api/WebsiteAPI/SavePaymentPolicy")]
        public async Task<Response> SavePaymentPolicy(PaymentPolicyRequest parameters)
        {
            try
            {
                var tbl = db.tblPaymentPolicies.Where(x => x.Id == parameters.Id).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblPaymentPolicy();

                    tbl.PaymentPolicy = parameters.PaymentPolicy;
                    tbl.IsActive = parameters.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;

                    db.tblPaymentPolicies.Add(tbl);

                    _response.Message = "Refund And Cancellation Policies saved successfully";
                }
                else
                {
                    tbl.PaymentPolicy = parameters.PaymentPolicy;
                    tbl.IsActive = parameters.IsActive;
                    tbl.IsActive = parameters.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    _response.Message = "Refund And Cancellation Policies updated successfully";
                }

                await db.SaveChangesAsync();
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
        [Route("api/WebsiteAPI/GetPaymentPolicyId")]
        public async Task<Response> GetPaymentPolicyId([FromBody] int Id)
        {
            GetPaymentPolicyList_Result caseStatusList_Result;
            try
            {
                caseStatusList_Result = await Task.Run(() => db.GetPaymentPolicyList(null).ToList().Where(x => x.Id == Id).FirstOrDefault());

                _response.Data = caseStatusList_Result;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/WebsiteAPI/GetPaymentPolicyList")]
        public async Task<Response> GetPaymentPolicyList(WebsiteSerachParameter parameter)
        {
            List<GetPaymentPolicyList_Result> caseStatusList;
            try
            {
                caseStatusList = await Task.Run(() => db.GetPaymentPolicyList(parameter.IsActive).ToList());

                _response.Data = caseStatusList;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        #endregion

        #region Privacy And Policy

        [HttpPost]
        [Route("api/WebsiteAPI/SavePrivacyAndPolicy")]
        public async Task<Response> SavePrivacyAndPolicy(PrivacyAndPolicyRequest parameters)
        {
            try
            {
                var tbl = db.tblPrivacyAndPolicies.Where(x => x.Id == parameters.Id).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblPrivacyAndPolicy();

                    tbl.PrivacyAndPolicy = parameters.PrivacyAndPolicy;
                    tbl.IsActive = parameters.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;

                    db.tblPrivacyAndPolicies.Add(tbl);

                    _response.Message = "Privacy and policy saved successfully";
                }
                else
                {
                    tbl.PrivacyAndPolicy = parameters.PrivacyAndPolicy;
                    tbl.IsActive = parameters.IsActive;
                    tbl.IsActive = parameters.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    _response.Message = "Privacy and policy updated successfully";
                }

                await db.SaveChangesAsync();
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
        [Route("api/WebsiteAPI/GetPrivacyAndPolicy")]
        public async Task<Response> GetPrivacyAndPolicy([FromBody] int Id)
        {
            GetPrivacyAndPolicyList_Result caseStatusList_Result;
            try
            {
                caseStatusList_Result = await Task.Run(() => db.GetPrivacyAndPolicyList(null).ToList().Where(x => x.Id == Id).FirstOrDefault());

                _response.Data = caseStatusList_Result;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/WebsiteAPI/GetPrivacyAndPolicyList")]
        public async Task<Response> GetPrivacyAndPolicyList(WebsiteSerachParameter parameter)
        {
            List<GetPrivacyAndPolicyList_Result> caseStatusList;
            try
            {
                caseStatusList = await Task.Run(() => db.GetPrivacyAndPolicyList(parameter.IsActive).ToList());

                _response.Data = caseStatusList;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        #endregion

        #region Privacy And Policy

        [HttpPost]
        [Route("api/WebsiteAPI/SaveTermsAndCondition")]
        public async Task<Response> SaveTermsAndCondition(TermsAndConditionRequest parameters)
        {
            try
            {
                var tbl = db.tblTermsAndConditions.Where(x => x.Id == parameters.Id).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblTermsAndCondition();

                    tbl.TermsAndCondition = parameters.TermsAndCondition;
                    tbl.IsActive = parameters.IsActive;
                    tbl.CreatedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.CreatedDate = DateTime.Now;

                    db.tblTermsAndConditions.Add(tbl);

                    _response.Message = "Privacy and policy saved successfully";
                }
                else
                {
                    tbl.TermsAndCondition = parameters.TermsAndCondition;
                    tbl.IsActive = parameters.IsActive;
                    tbl.IsActive = parameters.IsActive;
                    tbl.ModifiedBy = Utilities.GetUserID(ActionContext.Request);
                    tbl.ModifiedDate = DateTime.Now;

                    _response.Message = "Privacy and policy updated successfully";
                }

                await db.SaveChangesAsync();
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
        [Route("api/WebsiteAPI/GetTermsAndCondition")]
        public async Task<Response> GetTermsAndCondition([FromBody] int Id)
        {
            GetTermsAndConditionList_Result caseStatusList_Result;
            try
            {
                caseStatusList_Result = await Task.Run(() => db.GetTermsAndConditionList(null).ToList().Where(x => x.Id == Id).FirstOrDefault());

                _response.Data = caseStatusList_Result;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/WebsiteAPI/GetTermsAndConditionList")]
        public async Task<Response> GetTermsAndConditionList(WebsiteSerachParameter parameter)
        {
            List<GetTermsAndConditionList_Result> caseStatusList;
            try
            {
                caseStatusList = await Task.Run(() => db.GetTermsAndConditionList(parameter.IsActive).ToList());

                _response.Data = caseStatusList;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        #endregion

        #region Career Post
        [AllowAnonymous]
        [HttpPost]
        [Route("api/WebsiteAPI/SaveCareerPost")]
        public async Task<Response> SaveCareerPost(CareerPostRequest request)
        {
            try
            {
                var tbl = db.tblCareerPosts.Where(x => x.CareerPostId == request.CareerPostId).FirstOrDefault();
                if (tbl == null)
                {
                    tbl = new tblCareerPost();
                    tbl.JobTitle = request.JobTitle;
                    tbl.RequiredExp = request.RequiredExp;
                    tbl.Vacancies = request.Vacancies;
                    tbl.JobLocation = request.JobLocation;
                    tbl.JobDetails = request.JobDetails;
                    tbl.PostedDate = DateTime.Now;
                    tbl.IsActive = request.IsActive;

                    db.tblCareerPosts.Add(tbl);

                    _response.IsSuccess = true;
                    _response.Message = "Career Posted saved successfully";
                }
                else
                {
                    tbl.JobTitle = request.JobTitle;
                    tbl.RequiredExp = request.RequiredExp;
                    tbl.Vacancies = request.Vacancies;
                    tbl.JobLocation = request.JobLocation;
                    tbl.JobDetails = request.JobDetails;
                    //tbl.PostedDate = DateTime.Now;
                    tbl.IsActive = request.IsActive;

                    _response.IsSuccess = true;
                    _response.Message = "Career Posted updated successfully";
                }

                await db.SaveChangesAsync();


            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }
            return _response;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/WebsiteAPI/GetCareerPostById")]
        public async Task<Response> GetCareerPostById(int Id)
        {
            tblCareerPost careerPost;

            try
            {
                careerPost = await db.tblCareerPosts.Where(x => x.CareerPostId == Id).FirstOrDefaultAsync();

                _response.Data = careerPost;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/WebsiteAPI/GetCareerPostList")]
        public async Task<Response> GetCareerPostList(WebsiteSerachParameter parameter)
        {
            List<GetCareerPostList_Result> careerPost;

            try
            {
                var vTotal = new ObjectParameter("Total", typeof(int));
                careerPost = await Task.Run(() => db.GetCareerPostList(parameter.IsActive, parameter.SearchValue, parameter.PageSize, parameter.PageNo, vTotal).ToList());

                _response.TotalCount = Convert.ToInt32(vTotal.Value);
                _response.Data = careerPost;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ValidationConstant.InternalServerError;
                LogWriter.WriteLog(ex);
            }

            return _response;
        }
        #endregion
    }
}
