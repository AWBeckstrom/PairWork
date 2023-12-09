import axios from "axios";
import {
  onGlobalError,
  onGlobalSuccess,
  API_HOST_PREFIX,
} from "./serviceHelpers";

const storyApprovalService = { endPoint: `${API_HOST_PREFIX}/api/shareStory/` };

storyApprovalService.selectByNonApproval = (pageIndex, pageSize) => {
  const config = {
    method: "GET",
    url: `${storyApprovalService.endPoint}nonapproved?pageIndex=${pageIndex}&pageSize=${pageSize}`,
    crossdomain: true,
    withCredentials: true,
    headers: { "Content-Type": "application/json" },
  };
  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

storyApprovalService.recoverStoriesDismissed = (pageIndex, pageSize) => {
  const config = {
    method: "GET",
    url: `${storyApprovalService.endPoint}recoverDismissed?pageIndex=${pageIndex}&pageSize=${pageSize}`,
    withCredentials: true,
    crossDomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

storyApprovalService.updateIsDeleted = (id) => {
  const config = {
    method: "PUT",
    url: `${storyApprovalService.endpoint}/isDeleted/${id}`,
    data: payload,
    headers: { "Content-Type": "application/json" },
    withCredentials: true,
    crossdomain: true
  };

  return axios(config).then(onGlobalSuccess).catch(onGlobalError);
};

export default storyApprovalService;
