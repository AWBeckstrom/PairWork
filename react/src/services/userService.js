import axios from "axios";
import debug from "sabio-debug";
import {
  onGlobalError,
  onGlobalSuccess,
  API_HOST_PREFIX,
} from "./serviceHelpers";

const _logger = debug.extend("userService");

const users = { userUrl: `${API_HOST_PREFIX}/api/users` };

let register = (payload) => {
  _logger("register middle");
  const config = {
    method: "POST",
    url: users.userUrl,
    data: payload,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

let login = (payload) => {
  _logger("login middle");
  const config = {
    method: "POST",
    url: `${users.userUrl}/login`,
    data: payload,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

let logout = () => {
  _logger("logout middle");
  const config = {
    method: "GET",
    url: `${users.userUrl}/logout`,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

let confirm = (token) => {
  _logger("confirm middle");
  const config = {
    method: "GET",
    url: `${users.userUrl}/confirm?token=${token}`,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

let userInfo = () => {
  const config = {
    method: "GET",
    url: `${users.userUrl}`,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

let current = () => {
  const config = {
    method: "GET",
    url: `${users.userUrl}/current`,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

let forgotPassword = (payload) => {
  const config = {
    method: "PUT",
    url: `${users.userUrl}/forgotpassword?email=${payload.email}`,
    withCredentials: true,
    data: null,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

let changePassword = (payload) => {
  const config = {
    method: "PUT",
    url: `${users.userUrl}/changepassword`,
    withCredentials: true,
    data: payload,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

let update = (payload) => {
  const config = {
    method: "PUT",
    url: `${users.userUrl}/update/${payload.id}`,
    withCredentials: true,
    data: payload,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

let paginate = (pageIndex, pageSize) => {
  const config = {
    method: "GET",
    url: `${users.userUrl}/status/paginate?PageIndex=${pageIndex}&PageSize=${pageSize}`,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

let searchPaginate = (pageIndex, pageSize, query) => {
  const config = {
    method: "GET",
    url: `${users.userUrl}/status/search?PageIndex=${pageIndex}&PageSize=${pageSize}&Query=${query}`,
    data: null,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

let updateStatus = (payload) => {
  const config = {
    method: "PUT",
    url: `${users.userUrl}/updatestatus/${payload.id}`,
    withCredentials: true,
    data: payload,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return axios(config)
    .then(onGlobalSuccess)
    .then(() => payload)
    .catch(onGlobalError);
};
let getUser = (payload) => {
  const config = {
    method: "GET",
    url: `${users.userUrl}/${payload.id}`,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};
let getByStatus = (id, pageIndex, pageSize) => {
  const config = {
    method: "GET",
    url: `${users.userUrl}/status/${id}/paginate?pageIndex=${pageIndex}&pageSize=${pageSize}`,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};
let getDashInfo = () => {
  const config = {
    method: "GET",
    url: `${users.userUrl}/dashinfo`,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

export {
  register,
  login,
  logout,
  confirm,
  current,
  userInfo,
  forgotPassword,
  changePassword,
  update,
  paginate,
  searchPaginate,
  updateStatus,
  getUser,
  getByStatus,
  getDashInfo,
};
