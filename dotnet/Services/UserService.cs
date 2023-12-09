using Data.Providers;
using Models;
using Models.Domain;
using Models.Domain.Users;
using Models.Requests.Users;
using Services.Interfaces;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Data;
using System;
using System.Reflection;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Models.Enums;
using Models.Requests.Email;
using Models.Requests;
using Models.Domain.Blogs;
using System.Runtime;

namespace Services
{
    public class UserService : IUserService
    {
        private IAuthenticationService<int> _authenticationService;
        private IDataProvider _dataProvider;
        ILookUpService _lookUpService = null;
        private IEmailService _emailService = null;

        public UserService(IAuthenticationService<int> authSerice, IDataProvider dataProvider, ILookUpService lookUpService, IEmailService emailService)
        {
            _authenticationService = authSerice;
            _dataProvider = dataProvider;
            _lookUpService = lookUpService;
            _emailService = emailService;
        }

        public async Task<bool> LogInAsync(string email, string password)
        {
            bool isSuccessful = false;

            IUserAuthData response = Get(email, password);

            if (response != null)
            {
                await _authenticationService.LogInAsync(response);
                isSuccessful = true;
            }

            return isSuccessful;
        }
        public async Task<bool> LogInTest(string email, string password, int id, string[] roles = null)
        {
            bool isSuccessful = false;
            var testRoles = new[] { "User", "Super", "Content Manager" };

            var allRoles = roles == null ? testRoles : testRoles.Concat(roles);

            IUserAuthData response = new UserBase
            {
                Id = id
                ,
                Email = email
                ,
                Roles = allRoles
                ,
                TenantId = "Acme Corp UId"
            };

            Claim fullName = new Claim("CustomClaim", "Sabio Bootcamp");
            await _authenticationService.LogInAsync(response, new Claim[] { fullName });

            return isSuccessful;
        }

        public int Create(UserAddRequest model)
        {
            int id = 0;

            _dataProvider.ExecuteNonQuery("[dbo].[Users_Insert]", inputParamMapper: delegate (SqlParameterCollection col)
            {
                AddCommonParams(model, col);

                SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                idOut.Direction = ParameterDirection.Output;

                col.Add(idOut);
            },
            returnParameters: delegate (SqlParameterCollection returnCollection)
            {
                object oId = returnCollection["@Id"].Value;

                int.TryParse(oId.ToString(), out id);

            });
            return id;
        }
        /// <summary>
        /// Gets the Data call to get a give user
        /// </summary>
        /// <param name="email"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        private IUserAuthData Get(string email, string password)
        {
            UserBase user = null;
            string passwordFromDb = "";
            bool userConfirmed = false;

            _dataProvider.ExecuteCmd("[dbo].[Users_Select_AuthData]", delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Email", email);
            },
            delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                user = new UserBase();
                user.Id = reader.GetSafeInt32(startingIndex++);
                user.FirstName = reader.GetString(startingIndex++);
                user.LastName = reader.GetString(startingIndex++);
                user.AvatarUrl = reader.GetSafeString(startingIndex++);
                user.Email = reader.GetSafeString(startingIndex++);
                passwordFromDb = reader.GetSafeString(startingIndex++);
                user.TenantId = new object();
                user.TenantId = "WePairhealth";
                userConfirmed = reader.GetSafeBool(startingIndex++);
                List<LookUp> roles = reader.DeserializeObject<List<LookUp>>(startingIndex++);
                user.Roles = roles.Select(x => x.Name).ToList();

            }
            );

            if (user != null)
            {
                bool isValidCredentials = BCrypt.BCryptHelper.CheckPassword(password, passwordFromDb);
                if (!isValidCredentials || !userConfirmed)
                {
                    throw new Exception("Invalid login credentials.");
                }
            }
            else if (user == null)
            {
                throw new Exception("An account with that Email does not exist.");
            }

            return user;
        }
        public Paged<User> Pagination(int pageIndex, int pageSize)
        {
            Paged<User> pagedList = null;
            List<User> list = null;
            int totalCount = 0;

            _dataProvider.ExecuteCmd("[dbo].[Users_SelectAll]", inputParamMapper: delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@PageIndex", pageIndex);
                    col.AddWithValue("@PageSize", pageSize);
                },
                (reader, recordSetIndex) =>
                {
                    int startingIndex = 0;
                    User user = MapSingleUser(reader, ref startingIndex);
                    if (totalCount == 0)
                    {
                        totalCount = reader.GetSafeInt32(startingIndex++);
                    }
                    if (list == null)
                    {
                        list = new List<User>();
                    }
                    list.Add(user);
                });
            if (list != null)
            {
                pagedList = new Paged<User>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;
        }
        public User GetUser(int id)
        {
            User user = null;

            _dataProvider.ExecuteCmd("[dbo].[Users_Select_ById]", delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Id", id);
            },
            delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                user = MapSingleUser(reader, ref startingIndex);
            }
            );
            return user;
        }
        public void UpdateConfirm(string token)
        {
            _dataProvider.ExecuteNonQuery("[dbo].[Users_Confirm]", inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Token", token);
            },
            returnParameters: null);
        }
        public void UpdateStatus(UserUpdateStatus model)
        {
            _dataProvider.ExecuteNonQuery("[dbo].[Users_UpdateStatus]", inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Id", model.Id);
                col.AddWithValue("@StatusId", model.StatusId);
            },
            returnParameters: null);
        }
        public UserAuth GetAuthData(string email)
        {
            UserAuth user = null;

            _dataProvider.ExecuteCmd("[dbo].[Users_Select_AuthData]", delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Email", email);
            },
            delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                user = new UserAuth();

                user.Id = reader.GetSafeInt32(startingIndex++);
                user.Email = reader.GetSafeString(startingIndex++);
                user.Password = reader.GetSafeString(startingIndex++);
                user.Roles = reader.DeserializeObject<List<LookUp>>(startingIndex++);
            }
            );
            return user;
        }
        public BaseUserAuth GetPassword(string email)
        {
            BaseUserAuth user = null;

            _dataProvider.ExecuteCmd("[dbo].[Users_SelectPass_ByEmail]", delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Email", email);
            },
            delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                user = new BaseUserAuth();
                user.Password = reader.GetSafeString(startingIndex++);
            }
            );
            return user;
        }
        public List<LookUp> GetUserSkillsById(int userId)
        {
            string procName = "[dbo].[UserSkills_SelectByUserId]";
            List<LookUp> list = null;
            _dataProvider.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@UserId", userId);
            }, delegate (IDataReader reader, short set)
            {
                int startingIdx = 0;
                LookUp skill = _lookUpService.MapSingleLookUp(reader, ref startingIdx);

                if (list == null)
                {
                    list = new List<LookUp>();
                }

                list.Add(skill);
            });
            return list;
        }

        public UserDashInfo GetUserDashInfo(int userId)
        {
            string procName = "[dbo].[UserDashInfo_SelectById]";
            UserDashInfo info = null;
            _dataProvider.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Id", userId);
            }, delegate (IDataReader reader, short set)
            {
                int startingIdx = 0;
                if (info == null)
                {
                    info = new UserDashInfo();
                }
                if (set == 0)
                {
                    UserDashAppointmentInfo appointmentInfo = MapSingleAppointmentInfo(reader);
                    if (info.AppointmentInfo == null)
                    {
                        info.AppointmentInfo = new List<UserDashAppointmentInfo>();
                    }
                    info.AppointmentInfo.Add(appointmentInfo);
                }
                else if (set == 1)
                {
                    UserDashJobInfo jobInfo = MapSingleJobInfo(reader);
                    if (info.JobInfo == null)
                    {
                        info.JobInfo = new List<UserDashJobInfo>();
                    }
                    info.JobInfo.Add(jobInfo);
                }
                else if (set == 2)
                {
                    LookUp orgInfo = _lookUpService.MapSingleLookUp(reader, ref startingIdx);
                    if (info.OrgInfo == null)
                    {
                        info.OrgInfo = new List<LookUp>();
                    }
                    info.OrgInfo.Add(orgInfo);
                }
            });
            return info;
        }
        private static void AddCommonParams(UserAddRequest model, SqlParameterCollection col)
        {

            col.AddWithValue("@Email", model.Email);
            col.AddWithValue("@FirstName", model.FirstName);
            col.AddWithValue("@LastName", model.LastName);
            col.AddWithValue("@Mi", model.Mi);
            col.AddWithValue("@AvatarUrl", model.AvatarUrl);
            col.AddWithValue("@Password", GenerateSalt(model.Password));
            col.AddWithValue("@IsConfirmed", model.IsConfirmed);
            col.AddWithValue("@StatusId", model.StatusId);
            col.AddWithValue("@RoleId", model.RoleId);
        }
        private static string GenerateSalt(string password)
        {
            string salt = BCrypt.BCryptHelper.GenerateSalt();
            return BCrypt.BCryptHelper.HashPassword(password, salt);
        }
        private User MapSingleUser(IDataReader reader, ref int startingIndex)
        {
            User aUser = new User();

            aUser.Id = reader.GetSafeInt32(startingIndex++);
            aUser.Email = reader.GetSafeString(startingIndex++);
            aUser.FirstName = reader.GetSafeString(startingIndex++);
            aUser.LastName = reader.GetSafeString(startingIndex++);
            aUser.Mi = reader.GetSafeString(startingIndex++);
            aUser.AvatarUrl = reader.GetSafeString(startingIndex++);
            aUser.IsConfirmed = reader.GetSafeBool(startingIndex++);
            aUser.Status = _lookUpService.MapSingleLookUp(reader, ref startingIndex);
            aUser.DateCreated = reader.GetSafeDateTime(startingIndex++);
            aUser.DateModified = reader.GetSafeDateTime(startingIndex++);

            return aUser;
        }
        public void AddToken(TokenAddRequest model)
        {
            _dataProvider.ExecuteNonQuery("[dbo].[UserTokens_Insert]", inputParamMapper: delegate (SqlParameterCollection col)
            {
                TokenCommonParams(model, col);
            },
            returnParameters: null);

        }
        public void Delete(string Token)
        {
            _dataProvider.ExecuteNonQuery("[dbo].[UserTokens_Delete_ByToken]", inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Token", Token);
            },
                returnParameters: null);
        }
        public List<UserTokens> GetToken(int id)
        {
            List<UserTokens> list = null;

            _dataProvider.ExecuteCmd("[dbo].[UserTokens_Select_ByTokenType]", delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@TokenType", id);
            },
            delegate (IDataReader reader, short set)
            {
                int startingIndex = 0;
                UserTokens tokenType = MapTokenByType(reader, ref startingIndex);

                if (list == null)
                {
                    list = new List<UserTokens>();
                }

                list.Add(tokenType);
            }
            );

            return list;
        }
        private static void TokenCommonParams(TokenAddRequest model, SqlParameterCollection col)
        {
            col.AddWithValue("@Token", model.Token);
            col.AddWithValue("@UserId", model.UserId);
            col.AddWithValue("@TokenType", model.TokenType);
        }
        private static UserTokens MapTokenByType(IDataReader reader, ref int startingIndex)
        {
            UserTokens aToken = new UserTokens();

            aToken.Token = reader.GetSafeString(startingIndex++);
            aToken.UserId = reader.GetSafeInt32(startingIndex++);

            return aToken;
        }
        public void ConfirmUserEmail(string token)
        {
            UpdateConfirm(token);
        }

        public Paged<User> UserStatusPagination(int pageIndex, int pageSize)
        {
            Paged<User> paginatedUsers = null;
            List<User> list = null;
            int totalCount = 0;

            _dataProvider.ExecuteCmd("[dbo].[Users_Admin_Pagination]", inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@PageIndex", pageIndex);
                col.AddWithValue("@PageSize", pageSize);
            }, (reader, recordSetIndex) =>
            {
                int startingIndex = 0;
                User user = MapSingleUserStatus(reader, ref startingIndex);
                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(startingIndex++);
                }
                if (list == null)
                {
                    list = new List<User>();
                }
                list.Add(user);
            });
            if (list != null)
            {
                paginatedUsers = new Paged<User>(list, pageIndex, pageSize, totalCount);
            }
            return paginatedUsers;
        }
        private User MapSingleUserStatus(IDataReader reader, ref int startingIndex)
        {
            User aUser = new User();
            aUser.Id = reader.GetSafeInt32(startingIndex++);
            aUser.FirstName = reader.GetSafeString(startingIndex++);
            aUser.Mi = reader.GetSafeString(startingIndex++);
            aUser.LastName = reader.GetSafeString(startingIndex++);
            aUser.AvatarUrl = reader.GetSafeString(startingIndex++);
            aUser.Email = reader.GetSafeString(startingIndex++);
            aUser.Status = _lookUpService.MapSingleLookUp(reader, ref startingIndex);
            return aUser;
        }

        public Paged<User> SearchUserPagination(int pageIndex, int pageSize, string query)
        {
            Paged<User> paginatedUsers = null;
            List<User> list = null;
            int totalCount = 0;

            _dataProvider.ExecuteCmd("[dbo].[Users_Admin_Search_Pagination]", inputParamMapper: delegate (SqlParameterCollection col)
            {

                col.AddWithValue("@PageIndex", pageIndex);
                col.AddWithValue("@PageSize", pageSize);
                col.AddWithValue("@Query", query);

            }, (reader, recordSetIndex) =>
            {
                int startingIndex = 0;
                User user = MapSingleUserStatus(reader, ref startingIndex);
                if (totalCount == 0)
                {
                    totalCount = reader.GetSafeInt32(startingIndex++);
                }
                if (list == null)
                {
                    list = new List<User>();
                }
                list.Add(user);
            });

            if (list != null)
            {
                paginatedUsers = new Paged<User>(list, pageIndex, pageSize, totalCount);
            }
            return paginatedUsers;
        }


        private BaseUser DoesUserExist(string email)
        {
            BaseUser baseUser = null;

            _dataProvider.ExecuteCmd("[dbo].[Users_Select_Email]", inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Email", email);
            },
            singleRecordMapper: delegate (IDataReader reader, short set)
            {
                baseUser = new BaseUser();
                int startingIndex = 0;

                baseUser.Id = reader.GetSafeInt32(startingIndex++);
                baseUser.Email = reader.GetSafeString(startingIndex++);
                baseUser.FirstName = reader.GetSafeString(startingIndex++);
                baseUser.LastName = reader.GetSafeString(startingIndex++);
                baseUser.Mi = reader.GetSafeString(startingIndex++);
                baseUser.AvatarUrl = reader.GetSafeString(startingIndex++);
            });

            return baseUser;
        }

        public void ForgotPassword(string email, string guid)
        {

            BaseUser user = DoesUserExist(email);

            if (user != null)
            {
                TokenAddRequest tokenModel = new TokenAddRequest
                {
                    UserId = user.Id,
                    TokenType = (int)TokenTypes.ResetPassword,
                    Token = guid
                };

                AddToken(tokenModel);
                _emailService.ResetPassword(guid, email);
            }
        }
        public void UpdateUserInfo(UserUpdateRequest model)
        {
            _dataProvider.ExecuteNonQuery("[dbo].[Users_Update]", inputParamMapper: delegate (SqlParameterCollection col)
            {
                col.AddWithValue("@Id", model.Id);
                col.AddWithValue("@Email", model.Email);
                col.AddWithValue("@FirstName", model.FirstName);
                col.AddWithValue("@LastName", model.LastName);
                col.AddWithValue("@AvatarUrl", model.AvatarUrl);

            }, returnParameters: null);
        }
        public void ChangePassword(UserUpdatePasswordRequest model)
        {
            _dataProvider.ExecuteNonQuery("[dbo].[Users_UpdatePassword]", inputParamMapper: delegate (SqlParameterCollection col)
            {

                AddCommonParams(model, col);


            },
            returnParameters: null);
        }
        private static void AddCommonParams(UserUpdatePasswordRequest model, SqlParameterCollection col)
        {

            col.AddWithValue("@Password", GenerateSalt(model.Password));
            col.AddWithValue("@token", model.Token);

        }

        public Paged<User> GetByStatus(int id, int pageIndex, int pageSize)
        {
            Paged<User> pagedList = null;
            List<User> list = null;
            int totalCount = 0;

            _dataProvider.ExecuteCmd("[dbo].[Users_Admin_SelectByStatus]",
                inputParamMapper: delegate (SqlParameterCollection parameterCollection)
                {
                    parameterCollection.AddWithValue("@Id", id);
                    parameterCollection.AddWithValue("@PageIndex", pageIndex);
                    parameterCollection.AddWithValue("@PageSize", pageSize);
                },
                singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    int startingIndex = 0;
                    User user = MapSingleUserStatus(reader, ref startingIndex);

                    if (totalCount == 0)
                    {
                        totalCount = reader.GetSafeInt32(reader.FieldCount - 1);
                    }
                    if (list == null)
                    {
                        list = new List<User>();
                    }
                    list.Add(user);
                });

            if (list != null)
            {
                pagedList = new Paged<User>(list, pageIndex, pageSize, totalCount);
            }
            return pagedList;
        }

        private static UserDashAppointmentInfo MapSingleAppointmentInfo(IDataReader reader)
        {
            UserDashAppointmentInfo info = new UserDashAppointmentInfo();
            int startingIdx = 0;

            info.OrganizationName = reader.GetSafeString(startingIdx++);
            info.IsConfirmed = reader.GetBoolean(startingIdx++);
            info.AppointmentDate = reader.GetSafeDateTime(startingIdx++);
            return info;
        }

        private static UserDashJobInfo MapSingleJobInfo(IDataReader reader)
        {
            UserDashJobInfo info = new UserDashJobInfo();
            int startingIdx = 0;

            info.JobName = reader.GetSafeString(startingIdx++);
            info.ApplyDate = reader.GetSafeDateTime(startingIdx++);
            return info;
        }
    }
}