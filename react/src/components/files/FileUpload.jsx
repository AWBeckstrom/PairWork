import React, { Fragment, useCallback, useState } from "react";
import { Button, ListGroup, ListGroupItem } from "react-bootstrap";
import { useDropzone } from "react-dropzone";
import debug from "debug";
import filesService from "services/filesService";
import PropTypes from "prop-types";
import "./files.css";
import toastr from "toastr";

function FileUpload({ handleUploadSuccess, isMultiple }) {
  const _logger = debug.extend("fileUpload");

  const [fileAmount, setFileAmount] = useState(0);

  const [uploadedFiles, setUploadedFiles] = useState([]);

  const onDrop = useCallback(
    (acceptedFiles) => {
      handleUpload(acceptedFiles);
      _logger("file to be uploaded:", acceptedFiles);

      setFileAmount(acceptedFiles.length);
      setUploadedFiles([...uploadedFiles, ...acceptedFiles]);
    },
    [uploadedFiles]
  );

  const { getRootProps, getInputProps, isDragActive } = useDropzone({
    onDrop,
    accept: ".jpeg, .pdf, .txt, .docx, .png, .mp4, .jpg,",
    multiple: isMultiple,
  });

  const handleUpload = async () => {
    if (uploadedFiles.length > 0) {
      const data = new FormData();
      uploadedFiles.forEach((file) => {
        data.append(`file`, file);
      });

      _logger("call upload service fx!", data);
      await onUploadClicked(data);
    }
  };

  const onUploadClicked = (formData) => {
    _logger("upload to aws", formData);
    filesService
      .upload(formData)
      .then(onUploadFileSuccess)
      .catch(onUploadFileError);
  };

  const onUploadFileSuccess = (response) => {
    if (response && response.Item !== null) {
      _logger("file uploaded success handler", response);
      toastr.success("File uploaded successfully");
      handleUploadSuccess(response);
      setUploadedFiles([]);
    } else {
      _logger("can't send formData to .net");
    }
  };

  const onUploadFileError = (error) => {
    _logger("file upload error handler", error);
    toastr.error("Error: file did not upload successfully, try again");
  };

  return (
    <Fragment>
      <div
        className="file-upload-container"
        {...getRootProps({ className: "dropzone file-upload-container" })}
      >
        <input className="input-zone" {...getInputProps()} />
        {isDragActive ? (
          <p>Drop Files here...</p>
        ) : (
          <p>
            Drag and Drop the File You Would Like to Upload Here
            <span className="drag-box-number">
              {fileAmount > 0 &&
                `(${fileAmount} file${fileAmount > 1 ? "s" : ""} added)`}
            </span>
          </p>
        )}
      </div>
      <span className="placeholder col-12 bg-light align-center" />
      <div>
        <Button onClick={handleUpload}>
          Click to Upload After File is Dropped
        </Button>
      </div>

      {uploadedFiles.length > 0 && (
        <div className="uploaded-files-list">
          <h4>Uploaded Files:</h4>
          <ListGroup>
            {uploadedFiles.map((file, index) => (
              <ListGroupItem key={index}>{file.name}</ListGroupItem>
            ))}
          </ListGroup>
        </div>
      )}
    </Fragment>
  );
}

FileUpload.propTypes = {
  handleUploadSuccess: PropTypes.func.isRequired,
  isMultiple: PropTypes.bool.isRequired,
};

export default FileUpload;
