import React, { useState } from "react";
import shareStoryService from "services/shareStoryService";
import PropTypes from "prop-types";
import debug from "debug";

const _logger = debug.extend("storyApproval");

function ApprovalToggle(props) {
  const [activeButton, setActiveButton] = useState("approve-btn");

  const handleApprovalButtonClick = (e) => {
    const buttonId = e.currentTarget.id;

    if (buttonId === "approve") {
      shareStoryService
        .updateApproval(props.storyId)
        .then(onUpdateApprovalSuccess)
        .catch(onUpdateApprovalError);
      setActiveButton("approve-btn");
    } else {
      shareStoryService
        .remove(props.storyId)
        .then(onDeleteSuccess)
        .catch(onDeleteError);
      setActiveButton("delete-btn");
    }
  };

  const onUpdateApprovalSuccess = (response) => {
    _logger(response);
    props.onUpdateApprovalSuccess(props.storyId);
  };

  const onUpdateApprovalError = (err) => {
    _logger(err);
  };

  const onDeleteSuccess = (response) => {
    _logger(response);
    props.onDeleteSuccess(props.storyId);
  };

  const onDeleteError = (err) => {
    _logger(err);
  };

  const getButtonClassName = (buttonId) => {
    const baseClassName = "btn";
    const isActive = activeButton === buttonId;

    return `${baseClassName} ${
      isActive ? "btn-primary" : "btn-outline-primary"
    }`;
  };

  return (
    <div className="btn-group" role="group" aria-label="approval-buttons">
      <button
        type="button"
        id="approve"
        className={getButtonClassName("approve-btn")}
        onClick={handleApprovalButtonClick}
      >
        Approve
      </button>
      <button
        type="button"
        id="delete"
        className={getButtonClassName("delete-btn")}
        onClick={handleApprovalButtonClick}
      >
        Dismiss
      </button>
    </div>
  );
}

ApprovalToggle.propTypes = {
  storyId: PropTypes.number.isRequired,
  onUpdateApprovalSuccess: PropTypes.func.isRequired,
  onDeleteSuccess: PropTypes.func,
};

export default ApprovalToggle;
