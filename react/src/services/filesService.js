import axios from "axios";
import * as helper from "./serviceHelpers"

const filesService = {
    endpoint: `${helper.API_HOST_PREFIX}/api/files`
};

filesService.upload = (data) => {
    const config = {
        method: "POST",
        url: `${filesService.endpoint}/upload`,
        data: data,
        withCredentials: true,
        crossdomain: true,
        headers: { "Content-Type": "multipart/form-data" } 
      };
      return axios(config).then(helper.onGlobalSuccess).catch(helper.onGlobalError);
}

export default filesService;

