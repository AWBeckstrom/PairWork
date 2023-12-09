
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using Models.Domain.ShareStories;
using Models.Requests.ShareStories;
using Services;
using Services.Interfaces.ShareStories;
using Web.Controllers;
using Web.Models.Responses;
using System;

namespace Web.Api.Controllers.ShareStories
{
    [Route("api/shareStory")]
    [ApiController]
    public class ShareStoryApiController : BaseApiController
    {
        private IShareStoryService _service = null;
        private IAuthenticationService<int> _authService = null;
        public ShareStoryApiController(IShareStoryService service
            , ILogger<ShareStoryApiController> logger
            , IAuthenticationService<int> authService) : base(logger)
        {
            _service = service;
            _authService = authService;
        }

        [HttpPost]
        public ActionResult<ItemResponse<int>> Create(ShareStoryAddRequest model)
        {
            ObjectResult result = null;

            try
            {
                int userId = _authService.GetCurrentUserId();

                int id = _service.Insert(model, userId);

                ItemResponse<int> response = new ItemResponse<int>();

                response.Item = id;

                result = Created201(response);
            }
            catch (Exception ex)
            {

                Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);

                result = StatusCode(500, response);
            }
            return result;

        }


        [HttpPut("{id:int}")]
        public ActionResult<ItemResponse<int>> Update(ShareStoryUpdateRequest model)
        {
            ObjectResult result = null;

            try
            {
                int userId = _authService.GetCurrentUserId();
                _service.Update(model, userId);
                SuccessResponse response = new SuccessResponse();

                result = Ok(response);
            }
            catch (Exception ex)
            {

                Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);

                result = StatusCode(500, response);
            }
            return result;

        }

        [HttpPut("approval/{id:int}")]
        public ActionResult<ItemResponse<int>> UpdateApproval(int id)
        {
            ObjectResult result = null;

            try
            {
                int userId = _authService.GetCurrentUserId();
                _service.UpdateApproval(id, userId);
                SuccessResponse response = new SuccessResponse();

                result = Ok(response);
            }
            catch (Exception ex)
            {

                Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);

                result = StatusCode(500, response);
            }
            return result;

        }

        [HttpPut("isDeleted/{id:int}")]
        public ActionResult<ItemResponse<int>> UpdateIsDeleted(int id)
        {
            ObjectResult result = null;

            try
            {
       
                _service.UpdateIsDeleted(id );
                SuccessResponse response = new SuccessResponse();

                result = Ok(response);
            }
            catch (Exception ex)
            {

                Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);

                result = StatusCode(500, response);
            }
            return result;

        }

        [HttpGet("isapproved")]
        public ActionResult<ItemResponse<Paged<ShareStory>>> SelectByApproval(int pageIndex, int pageSize)
        {
            int iCode = 200;
            BaseResponse response = null;
            Paged<ShareStory> list = _service.SelectByApproval(pageIndex, pageSize);

            try
            {
                if (list.PagedItems == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Resources not Found");
                }
                else
                {
                    response = new ItemResponse<Paged<ShareStory>> { Item = list };
                }
            }
            catch (Exception ex)
            {
                iCode = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(iCode, response);


        }


        [HttpGet("nonapproved")]
        public ActionResult<ItemResponse<Paged<ShareStory>>>SelectByNonApproval(int pageIndex, int pageSize)
        {
            int iCode = 200;
            BaseResponse response = null;
            Paged<ShareStory> list = _service.SelectByNonApproval(pageIndex, pageSize);

            try
            {
                if (list == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Resources not Found");
                }
                else
                {
                    response = new ItemResponse<Paged<ShareStory>> { Item = list };
                }
            }
            catch (Exception ex)
            {
                iCode = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(iCode, response);


        }

        [HttpGet("recoverDismissed")]
        public ActionResult<ItemResponse<Paged<ShareStory>>> SelectByIsDeleted(int pageIndex, int pageSize)
        {
            int iCode = 200;
            BaseResponse response = null;
            Paged<ShareStory> list = _service.SelectByIsDeleted(pageIndex, pageSize);

            try
            {
                if (list.PagedItems == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Resources not Found");
                }
                else
                {
                    response = new ItemResponse<Paged<ShareStory>> { Item = list };
                }
            }
            catch (Exception ex)
            {
                iCode = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(iCode, response);


        }


        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<ShareStory>> Get(int id)
        {
            int iCode = 200;
            BaseResponse response = null;
            try
            {
                ShareStory shareStory = _service.Get(id);

                if (shareStory == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Resource Not Found");
                }
                else
                {
                    response = new ItemResponse<ShareStory> { Item = shareStory };
                }
            }
            catch (Exception ex)
            {
                iCode = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(iCode, response);
        }

        [HttpGet("paginate")]
        public ActionResult<ItemResponse<Paged<ShareStory>>> SelectAll(int pageIndex, int pageSize)
        {
            int iCode = 200;
            BaseResponse response = null;
            Paged<ShareStory> list = _service.SelectAll(pageIndex, pageSize);

            try
            {
                if (list.PagedItems == null)
                {
                    iCode = 404;
                    response = new ErrorResponse("Resources not Found");
                }
                else
                {
                    response = new ItemResponse<Paged<ShareStory>> { Item = list };
                }
            }
            catch (Exception ex)
            {
                iCode = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(iCode, response);


        }


        [HttpDelete("{id:int}")]
        public ActionResult<SuccessResponse> Delete(int id)
        {
            int iCode = 200;
            BaseResponse response = null;

            try
            {
                _service.Delete(id);                                
                response = new SuccessResponse();
 
            }
            catch (Exception ex)
            {
                iCode = 500;
                Logger.LogError(ex.ToString());
                response = new ErrorResponse(ex.Message);

            }

            return StatusCode(iCode, response);
        }

    }
}

