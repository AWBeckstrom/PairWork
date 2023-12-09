import axios from "axios";
import {
  onGlobalError,
  onGlobalSuccess,
  API_HOST_PREFIX,
} from "./serviceHelpers";

const shareStoryService = { endpoint: `${API_HOST_PREFIX}/api/shareStory` };

shareStoryService.getById = (id) => {
  const config = {
    method: "GET",
    url: `${shareStoryService.endpoint}/${id}`,
    headers: { "Content-Type": "application/json" },
  };

  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

shareStoryService.getAll = (pageIndex, pageSize) => {
  const config = {
    method: "GET",
    url: `${shareStoryService.endpoint}/paginate/?pageIndex=${pageIndex}&pageSize=${pageSize}`,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };

  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

shareStoryService.selectByApproval = (pageIndex, pageSize) => {
  const config = {
    method: "GET",
    url: `${shareStoryService.endpoint}/isapproved/?pageIndex=${pageIndex}&pageSize=${pageSize}`,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };

  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

shareStoryService.create = (payload) => {
  const config = {
    method: "POST",
    url: `${shareStoryService.endpoint}`,
    data: payload,
    headers: { "Content-Type": "application/json" },
  };

  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

shareStoryService.update = (id, payload) => {
  const config = {
    method: "PUT",
    url: `${shareStoryService.endpoint}/${payload.id}`,
    data: payload,
    headers: { "Content-Type": "application/json" },
  };

  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

shareStoryService.updateApproval = (id) => {
  const config = {
    method: "PUT",
    url: `${shareStoryService.endpoint}/approval/${id}`,
    headers: { "Content-Type": "application/json" },
    withCredentials: true,
    crossdomain: true
  };

  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

shareStoryService.remove = (id) => {
  const config = {
    method: "PUT",
    url: `${shareStoryService.endpoint}/isDeleted/${id}`,
    headers: { "Content-Type": "application/json" },
    withCredentials: true,
    crossdomain: true
  };

  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

export default shareStoryService;
