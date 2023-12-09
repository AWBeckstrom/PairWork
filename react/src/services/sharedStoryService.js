import axios from "axios";
import * as helper from "./serviceHelpers";

var sharedStoryService = {
  endpoint: `${helper.API_HOST_PREFIX}/api/shareStory`,
};

sharedStoryService.add = (payload) => {
  const config = {
    method: "POST",
    url: sharedStoryService.endpoint,
    data: payload,
    withCredentials: true,
    crossdomain: true,
    headers: {"Content-Type": "application/json"},
  };
  return axios(config).then(helper.onGlobalSuccess).catch(helper.onGlobalError);
};
export default sharedStoryService;
