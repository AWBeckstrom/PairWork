import React from "react";
import { Row } from "react-bootstrap";
import PropTypes from "prop-types";
import ApprovalToggle from "./ApprovalToggle";
import storyApprovalService from "services/storyApprovalService";
import { BsFillTrashFill } from "react-icons/bs";
import { FcDataRecovery } from "react-icons/fc";
import "./ApprovalStyle.css";

const StoryTable = (props) => {
  StoryTable.propTypes = {
    stories: PropTypes.arrayOf(
      PropTypes.shape({
        id: PropTypes.number.isRequired,
        name: PropTypes.string.isRequired,
        createdBy: PropTypes.shape({
          firstName: PropTypes.string.isRequired,
          lastName: PropTypes.string.isRequired,
        }).isRequired,
        isApproved: PropTypes.bool.isRequired,
        dateCreated: PropTypes.string.isRequired,
        storyFileUrl: PropTypes.string.isRequired,
        isDeleted: PropTypes.bool.isRequired,
      })
    ).isRequired,
    idToDelete: PropTypes.number,
    onDeleteStory: PropTypes.func,
    onUpdateApprovalSuccess: PropTypes.func,
    onUpdateDeleteSuccess: PropTypes.func,
    deleteHandler: PropTypes.func,
    recoverStory: PropTypes.func,
    isDeleted: PropTypes.bool,
  };

  const handleFileLinkClick = (e, story) => {
    e.preventDefault();
    const fileUrl = story.storyFileUrl;
    window.open(fileUrl, "_blank");
  };

  const recoverStory = (item) => {
    storyApprovalService
      .recoverStoriesDismissed(item, 0)
      .then(onRecoverStorySuccess)
      .catch(onRecoverFileError);
  };

  const onRecoverStorySuccess = () => {
    _logger("onRecoverStorySuccess");
    props.recoverStory(item.id);
  };

  const onRecoverFileError = () => {
    _logger("Error Dismissing Story");
  };

  const StoryTableRow = ({
    story,
    onDeleteStory,
    onUpdateApprovalSuccess,
    isDeleted,
  }) => (
    <tr key={story.id}>
      <td className="column1" name="Id Column1" scope="row">
        {story.id}
      </td>
      <td className="column2" name="Title Column2">
        {story.name}
      </td>
      <td className="column3" name="Author Column3">
        {story.createdBy.firstName} {story.createdBy.lastName}
      </td>
      <td
        className="column4"
        name="Files Column4"
        alt="storyfileurl-avatar"
        id="SharedStorySubmissionImage"
        type="Img"
        width="5%"
        height="25px"
      >
        {story.storyFileUrl ? (
          <a
            href={story.storyFileUrl}
            onClick={(e) => handleFileLinkClick(e, story)}
          >
            File
          </a>
        ) : (
          <p>None</p>
        )}
      </td>
      <td className="column5" name="Created Column5">
        {new Date(story.dateCreated).toLocaleDateString()}
      </td>
      <td className="column6" name="approval Column6">
        <ApprovalToggle
          storyId={story.id}
          onUpdateApprovalSuccess={onUpdateApprovalSuccess}
          onDeleteSuccess={onDeleteStory}
          isDeleted={isDeleted}
        >
          {isDeleted ? (
            <FcDataRecovery
              className="mx-4"
              title="Recover"
              cursor="pointer"
              onClick={() => recoverStory(story.id)}
              color="green"
            />
          ) : (
            <BsFillTrashFill
              className="mx-4"
              title="Delete"
              cursor="pointer"
              onClick={() => deleteStory(story.id)}
            />
          )}
        </ApprovalToggle>
      </td>
    </tr>
  );

  StoryTableRow.propTypes = {
    story: PropTypes.shape({
      id: PropTypes.number.isRequired,
      name: PropTypes.string.isRequired,
      createdBy: PropTypes.shape({
        firstName: PropTypes.string.isRequired,
        lastName: PropTypes.string.isRequired,
      }).isRequired,
      isApproved: PropTypes.bool.isRequired,
      dateCreated: PropTypes.string.isRequired,
      storyFileUrl: PropTypes.string,
      isDeleted: PropTypes.bool.isRequired,
    }).isRequired,
  };

  const renderTableRows = () =>
    props.stories.map((story) => (
      <StoryTableRow key={story.id} story={story} />
    ));

  return (
    <React.Fragment>
      <div className="container-fluid p-12">
        <Row>
          <div className="col-lg-12 col-lg-12">
            <div className="row">
              <div className="justify-content-lg-between mb-8 mb-xl-0 row">
                <div className="shadow-lg p-9 bg-white w-100 mx-auto my-5">
                  <div className="">
                    <table className="table text-center">
                      <thead>
                        <tr>
                          <th className="column1" name="Id Column1" scope="col">
                            <strong>Id</strong>
                          </th>
                          <th
                            className="column2"
                            name="Title Column2"
                            scope="col"
                          >
                            <strong>Story Name</strong>
                          </th>
                          <th
                            className="column3"
                            name="Author Column3"
                            scope="col"
                          >
                            <strong>Member Author</strong>
                          </th>
                          <th
                            className="column4"
                            name="Files Column4"
                            scope="col"
                          >
                            <strong>Story Files</strong>
                          </th>
                          <th
                            className="column5"
                            name="Created Column5"
                            scope="col"
                          >
                            <strong>Date Created</strong>
                          </th>
                          <th
                            className="column6"
                            name="approval Column6"
                            scope="col"
                          >
                            <strong>Post Approval</strong>
                          </th>
                        </tr>
                      </thead>
                      {props.stories ? (
                        <tbody>{renderTableRows()}</tbody>
                      ) : (
                        <p>No Results</p>
                      )}
                    </table>
                  </div>
                </div>
                <span className="placeholder col-12 bg-light align-center" />
              </div>
            </div>
          </div>
        </Row>
      </div>
    </React.Fragment>
  );
};

export default StoryTable;
