
import axios from "axios";
import debug from "sabio-debug";
import {
    onGlobalError,
    onGlobalSuccess,
  API_HOST_PREFIX,
} 
    from "./serviceHelpers";

const _logger = debug.extend("FileService")

const fileManagerService = { endPoint: `${API_HOST_PREFIX}/api/files` };

_logger("EndPoint",fileManagerService.endPoint)

// Adding showDeleted to the the params
fileManagerService.getFiles = (pageIndex, pageSize,showDeleted ) => {
    const config = {
        method: "GET",
        url: `${fileManagerService.endPoint}/paginate/?pageIndex=${pageIndex}&pageSize=${pageSize}&showDeleted=${showDeleted}`,
        withCredentials: true,
        crossDomain: true,
        headers: {"Content-Type": "application/json"}
    };
    return axios(config).then(onGlobalSuccess).catch(onGlobalError)
};

fileManagerService.searchPagination  = (pageIndex, pageSize, query, showDeleted) => {
    const config = {
        method: "GET",
        url: `${fileManagerService.endPoint}/search?pageIndex=${pageIndex}&pageSize=${pageSize}&query=${query}&showDeleted=${showDeleted}`,
        
        withCredentials: true,
        crossDomain: true,
        headers: {"Content-Type": "application/json"}
    };
    return axios(config).then(onGlobalSuccess).catch(onGlobalError)
};

fileManagerService.downloadFile = (fileId) => {
    const config = {
      method: "GET",
      url: `${fileManagerService.endPoint}/download/${fileId}`, 
      responseType: "Working", 
      withCredentials: true,
      crossDomain: true,
      headers: { "Content-Type": "application/json" },
    };
  
    return axios(config).then(onGlobalSuccess).catch(onGlobalError)

  };
  
  fileManagerService.deleteFileById = (Id, status) => {
    const config = {
      method: "DELETE",
      url: `${fileManagerService.endPoint}/${Id}/${status}`, 
      withCredentials: true,
      crossDomain: true,
      headers: { "Content-Type": "application/json" },
    };
  
    return axios(config).then(onGlobalSuccess).then(() =>Id ).catch(onGlobalError)

  };

  fileManagerService.recoverFileById = (Id, status) => {
    const config = {
      method: "DELETE",
      url: `${fileManagerService.endPoint}/${Id}/${status}`, 
      withCredentials: true,
      crossDomain: true,
      headers: { "Content-Type": "application/json" },
    };
  
    return axios(config).then(onGlobalSuccess).then(() =>Id ).catch(onGlobalError)

  };

export default fileManagerService

