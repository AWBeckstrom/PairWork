import React, { Fragment } from "react";
import FileUpload from "./FileUpload";
import debug from "debug";

function ExampleFileUpload() {
  const _logger = debug.extend("exampleFile");

  const handleUploadSuccess = (files) => {
    _logger("Sample files to test upload:", files);
  };

  return (
    <Fragment>
      <h1>Example File Upload</h1>
      <FileUpload handleUploadSuccess={handleUploadSuccess} isMultiple={true} />
    </Fragment>
  );
}

export default ExampleFileUpload;
