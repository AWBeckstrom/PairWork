using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using Models.Enums;
using Models.Domain.Users;
using Models.Requests.Email;
using Models.Requests.Users;
using Services;
using Services.Interfaces;
using Web.Controllers;
using Web.Models.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SendGrid;
using Microsoft.AspNetCore.Identity;
using Models.Domain;

namespace Web.Api.Controllers.Users
{
    [Route("api/users")]
    [ApiController]
    public class UsersApiController : BaseApiController
    {
        private IUserService _service = null;
        private IAuthenticationService<int> _authService = null;
        private IEmailService _emailService = null;
        public UsersApiController(IUserService service, ILogger<UsersApiController> logger, IAuthenticationService<int> authService, IEmailService emailService) : base(logger)
        {
            _service = service;
            _authService = authService;
            _emailService = emailService;
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult<ItemResponse<int>> Add(UserAddRequest model)
        {
            ObjectResult result = null;

            try
            {
                int id = _service.Create(model);
                string token = Guid.NewGuid().ToString();
                TokenAddRequest tokenModel = new TokenAddRequest
                {
                    UserId = id,
                    TokenType = (int)TokenTypes.ConfirmEmail,
                    Token = token
                };
                _service.AddToken(tokenModel);
                ConfirmEmailRequest emailConModel = new ConfirmEmailRequest
                {
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };
                _emailService.ConfirmEmail(emailConModel, token);

                ItemResponse<int> response = new ItemResponse<int> { Item = id };
                result = Created201(response);
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);

                result = StatusCode(500, response);
            }
            return result;
        }
        [HttpPut("updatestatus/{id:int}")]
        public ActionResult<ItemResponse<int>> UpdateStatus(UserUpdateStatus model)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                _service.UpdateStatus(model);

                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpGet("paginate")]
        public ActionResult<ItemResponse<Paged<User>>> Pagination(int pageIndex, int PageSize)
        {
            ActionResult result = null;

            try
            {
                Paged<User> paged = _service.Pagination(pageIndex, PageSize);

                if (paged == null)
                {
                    result = NotFound404(new ErrorResponse("Application Resource Not Found"));
                }
                else
                {
                    ItemResponse<Paged<User>> response = new ItemResponse<Paged<User>> { Item = paged };
                    result = Ok200(response);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                result = StatusCode(500, new ErrorResponse(ex.ToString()));
            }
            return result;
        }
        [HttpPost("tokens")]
        public ActionResult<SuccessResponse> AddToken(TokenAddRequest model)
        {
            ObjectResult result = null;

            try
            {
                _service.AddToken(model);
                SuccessResponse response = new SuccessResponse();
                result = StatusCode(201, response);
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);

                result = StatusCode(500, response);
            }
            return result;
        }
        
        [HttpGet("tokentypes/{id:int}")]
        public ActionResult<ItemsResponse<UserTokens>> GetToken(int id)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                List<UserTokens> list = _service.GetToken(id);

                if (list == null)
                {
                    code = 404;
                    response = new ErrorResponse("Application Resource Not Found");
                }
                else
                {
                    response = new ItemsResponse<UserTokens> { Items = list };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }
       
        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult<ItemResponse<bool>> Login(UserLoginRequest model)
        {
            ObjectResult result = null;

            try
            {
                bool isSuccessful = _service.LogInAsync(model.Email, model.Password).Result;
                ItemResponse<bool> response = new ItemResponse<bool> { Item = isSuccessful };
                result = Ok(response);
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);
                result = StatusCode(500, response);
            }
            return result;
        }
        
        [AllowAnonymous]
        [HttpGet("confirm")]
        public ActionResult<SuccessResponse> ConfirmEmail(string token)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                _service.ConfirmUserEmail(token);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }
            return StatusCode(code, response);
        }
        
        [HttpGet("current")]
        public ActionResult<ItemResponse<IUserAuthData>> GetCurrrent()
        {
            IUserAuthData user = _authService.GetCurrentUser();
            ItemResponse<IUserAuthData> response = new ItemResponse<IUserAuthData>();
            response.Item = user;

            return Ok200(response);
        }
        
        [HttpGet("logout")]
        public async Task<ActionResult<SuccessResponse>> LogoutAsync()
        {
            await _authService.LogOutAsync();
            SuccessResponse response = new SuccessResponse();
            return Ok200(response);
        }
        [HttpGet("status/paginate")]
        public ActionResult<ItemResponse<Paged<User>>> UserStatusPagination(int pageIndex, int pageSize)
        {
            ActionResult result = null;

            try
            {
                Paged<User> statusPaged = _service.UserStatusPagination(pageIndex, pageSize);

                if (statusPaged == null)
                {
                    result = NotFound404(new ErrorResponse("Application Resource Not Found"));
                }
                else
                {
                    ItemResponse<Paged<User>> response = new ItemResponse<Paged<User>> { Item = statusPaged };
                    result = Ok200(response);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                result = StatusCode(500, new ErrorResponse(ex.ToString()));
            }
            return result;
        }
        [HttpGet("status/search")]
        public ActionResult<ItemResponse<Paged<User>>> SearchUserPagination(int pageIndex, int pageSize, string query)
        {
            ActionResult result = null;

            try
            {
                if (query == null)
                {
                    return BadRequest(new ErrorResponse("Invalid search request."));
                }

                Paged<User> statusPaged = _service.SearchUserPagination(
                     pageIndex, pageSize, query
                );

                if (statusPaged == null)
                {
                    result = NotFound404(new ErrorResponse("No results found."));
                }
                else
                {
                    ItemResponse<Paged<User>> response = new ItemResponse<Paged<User>> { Item = statusPaged };
                    result = Ok200(response);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                result = StatusCode(500, new ErrorResponse(ex.ToString()));
            }

            return result;
        }

        [AllowAnonymous]
        [HttpPut("forgotPassword")]
        public ActionResult<ItemResponse<int>> ForgotPassword(string email)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {

                if (!String.IsNullOrEmpty(email))
                {
                    String newGuid = Guid.NewGuid().ToString();
                    _service.ForgotPassword(email, newGuid);
                    response = new SuccessResponse();
                    return StatusCode(code, response);
                }
                return StatusCode(404, new ErrorResponse("Application Resource Not Found"));
            }

            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                return StatusCode(code, response);
            } 
        }

        [AllowAnonymous]
        [HttpPut("changePassword")]
        public ActionResult<ItemResponse<int>> ChangePassword(UserUpdatePasswordRequest model)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                _service.ChangePassword(model);
                response = new SuccessResponse();
            }

            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(code, response);
        }

        [HttpPut("update/{id:int}")]
        public ActionResult<ItemResponse<int>> UpdateUserInfo(UserUpdateRequest model)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {

                _service.UpdateUserInfo(model);

                response = new SuccessResponse();

            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }
        [HttpGet("skills/{id:int}")]
        public ActionResult<ItemResponse<List<LookUp>>> GetUserSkills_ById(int id)
        {
            ActionResult result = null;
            try
            {
                List<LookUp> list = _service.GetUserSkillsById(id);

                if (list == null)
                {
                    result = NotFound404(new ErrorResponse("Application Resource Not Found"));
                }
                else
                {
                    ItemResponse<List<LookUp>> response = new ItemResponse<List<LookUp>> { Item = list };
                    result = Ok200(response);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                result = StatusCode(500, new ErrorResponse(ex.Message.ToString()));
            }
            return result;
        }
        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<User>> GetUserById(int id)
        {
            User user = _service.GetUser(id);
            ItemResponse<User> response = new ItemResponse<User>();
            response.Item = user;

            return Ok200(response);
        }
        [HttpGet("status/{id:int}/paginate")]
        public ActionResult<ItemResponse<Paged<User>>> GetByStatus(int id, int pageIndex, int pageSize)
        {
            ActionResult result = null;
            try
            {

                Paged<User> paged = _service.GetByStatus(id, pageIndex, pageSize);
                if (paged == null)
                {
                    result = NotFound404(new ErrorResponse("Records Not Found"));
                }
                else
                {
                    int code = 200;
                    ItemResponse<Paged<User>> response = new ItemResponse<Paged<User>> { Item = paged };
                    result = StatusCode(code, response);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                result = StatusCode(500, new ErrorResponse(ex.Message.ToString()));
            }
            return result;
        }
        [HttpGet("dashinfo")]
        public ActionResult<ItemResponse<UserDashInfo>> GetDashInfo()
        {
            ActionResult result = null;
            try
            {
                int userId = _authService.GetCurrentUserId();
                UserDashInfo dashInfo = _service.GetUserDashInfo(userId);
                if(dashInfo == null)
                {
                    result = NotFound404(new ErrorResponse("Records Not Found"));
                }
                else
                {
                    int code = 200;
                    ItemResponse<UserDashInfo> response = new ItemResponse<UserDashInfo> { Item = dashInfo };
                    result = StatusCode(code, response);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                result = StatusCode(500, new ErrorResponse(ex.Message.ToString()));
            }
            return result;
        }
    }
}
