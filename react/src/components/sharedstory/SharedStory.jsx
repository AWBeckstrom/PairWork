import { React, useState } from "react";
import { Formik, Form, Field, ErrorMessage } from "formik";
import { CKEditor } from "@ckeditor/ckeditor5-react";
import ClassicEditor from "@ckeditor/ckeditor5-build-classic";
import "./SharedStory.css";
import validationSchema from "schema/shareStorySchema";
import toastr from "toastr";
import sharedStoryService from "services/sharedStoryService";
import FileUpload from "components/files/FileUpload";
import { Col, Row } from "react-bootstrap";
import { trackGoogleAnalyticsEvent } from "helper/analyticsInitialize";

function SharedStory() {
  const [storyCharCount, setStoryCharCount] = useState(0);

  const [fileUploadKey, setFileUploadKey] = useState(0);
  const [formValues, setFormValues] = useState({
    name: "",
    email: "",
    story: "",
    fileIds: [],
  });

  const [editor, setEditor] = useState(null);

  const updateCharCount = (data) => {
    const tempDiv = document.createElement("div");
    tempDiv.innerHTML = data;
    const textContent = tempDiv.textContent || tempDiv.innerText || "";
    setStoryCharCount(textContent.length);
    tempDiv.innerHTML = "";
  };

  const renderStoryCharCount = () => {
    return storyCharCount > 3000 ? (
      <p className="red-text">Story must be at less than 3000 characters!</p>
    ) : null;
  };

  const handleSuccess = (result, { resetForm }) => {
    trackGoogleAnalyticsEvent(
      "StorySubmit",
      "Story Submitted",
      window.location.pathname
    );
    toastr.success("Form Submitted Successfully");
    resetHandler();
    resetForm();
    if (editor) {
      editor.setData("");
    }
  };

  const handleError = (error, { setSubmitting }) => {
    toastr.error("Please Try again", error);
    setSubmitting(false);
  };

  const handleSubmit = (values, formikProps) => {
    if (values.fileIds.length === 0) {
      values.fileIds = null;
    }

    sharedStoryService
      .add(values)
      .then((result) => handleSuccess(result, formikProps))
      .catch((error) => handleError(error, formikProps));
  };

  const handleUploadSuccess = (response, setFieldValue) => {
    const newFileIds = response.item.map((item) => item.id);
    setFieldValue(
      "fileIds",
      formValues.fileIds?.length
        ? [...formValues.fileIds, ...newFileIds]
        : newFileIds
    );
    //toastr.success("File(s) Uploaded Successfully");
  };

  const resetHandler = () => {
    setFormValues({ name: "", email: "", story: "", fileIds: [] });
    setFileUploadKey((prevKey) => prevKey + 1);
  };

  return (
    <div className="sharestory-app">
      <Row>
        <div className="card text-md-primary bg-primary">
          <div
            name="card-body"
            className="vertical-align-bottom display-4 fw-bold MX-auto"
          >
            <h3 className="card-title" />
            <p
              type="text"
              className="StoryCreateBannerText d-flex text-white justify-content-between"
            >
              Upload & Submit Your Story to Share
            </p>
          </div>
        </div>
      </Row>
      <hr />
      <Row>
        <Col lg={12} md={10} className="py-10 py-xl-10">
          <div className="card-body vertical-align center">
            <Formik
              enableReinitialize={true}
              initialValues={formValues}
              validationSchema={validationSchema}
              onSubmit={handleSubmit}
            >
              {({ isSubmitting, isValid, setFieldValue }) => (
                <Form className="form-container sharestory">
                  <div className="hr-container sharestory"></div>
                  <div className="form-group sharestory flex-container">
                    <div>
                      <label htmlFor="name">Story Name</label>
                      <Field
                        type="text"
                        name="name"
                        className="form-control sharestory"
                        id="name"
                        placeholder="A Title Name For Your Story"
                      />

                      <div className="description">
                        <p>Please enter the name of the story</p>
                        <ErrorMessage
                          name="name"
                          component="div"
                          className="error red-text"
                        />
                      </div>
                    </div>
                  </div>
                  <div className="form-group sharestory flex-container">
                    <div>
                      <label htmlFor="email">Email</label>
                      <Field
                        type="email"
                        name="email"
                        className="form-control sharestory"
                        id="email"
                        placeholder="WePairHealth Member Email"
                      />

                      <div className="description">
                        <p>Please enter the email of the story owner</p>
                        <ErrorMessage
                          name="email"
                          component="div"
                          className="error red-text"
                        />
                      </div>
                    </div>
                  </div>
                  <div className="form-group sharestory ckeditor-container">
                    <label htmlFor="story">Story</label>
                    <CKEditor
                      editor={ClassicEditor}
                      name="story"
                      config={{
                        placeholder: "Write the experience here!",
                      }}
                      data={formValues.story}
                      onReady={(editor) => {
                        setEditor(editor);
                      }}
                      onChange={(event, editor) => {
                        const data = editor.getData();
                        setFieldValue("story", data);
                        updateCharCount(data);
                      }}
                    />
                  </div>
                  <div className="description">
                    <p className="right-aligned-text">
                      <span
                        className={`${storyCharCount > 3000 ? "red-text" : ""}`}
                      >
                        {storyCharCount}
                      </span>
                      /3000
                    </p>
                    {renderStoryCharCount()}
                  </div>
                  <hr className="sharestory-separator" />
                  <div className="header-container-sharestory">
                    <h5 className="card-title sharestory">Upload Files</h5>
                  </div>
                  <div className="hr-container sharestory"></div>
                  <div className="form-group sharestory flex-container">
                    <div className="description">
                      <span>
                        Related files can be uploaded here. Accepted formats are
                        .jpeg, .pdf, .txt, .docx, .png, and .mp4.
                      </span>
                    </div>
                    <FileUpload
                      key={fileUploadKey}
                      isMultiple={true}
                      handleUploadSuccess={(response) =>
                        handleUploadSuccess(response, setFieldValue)
                      }
                      className="my-file-upload-sharestory"
                    />
                  </div>{" "}
                  <br />
                  <hr className="sharestory-separator" />
                  <button
                    type="submit"
                    className="btn btn-primary sharestory"
                    disabled={isSubmitting || !isValid}
                  >
                    Submit
                  </button>
                </Form>
              )}
            </Formik>
          </div>
        </Col>
      </Row>
    </div>
  );
}
export default SharedStory;
