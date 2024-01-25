import React, { useState, useEffect } from "react";
import StoryTable from "./StoryTable";
import storyApprovalService from "../../services/storyApprovalService";
import debug from "debug";
import Pagination from "rc-pagination";
import "rc-pagination/assets/index.css";

const _logger = debug.extend("storyApproval");

function StoryApproval() {
  const [sharedStories, setSharedStories] = useState({
    pagedItems: [],
    mappedItems: [],
    totalResults: 0,
    pageIndex: 0,
    pageSize: 10,
  });
  const [showDeleted, setShowDeleted] = useState(true); //state for dismissed stories

  useEffect(() => {
    _logger("Shared Stories useEffect");
    updateSharedStories();
  }, [showDeleted, sharedStories.pageIndex]);

  const updateSharedStories = () => {
    if (!showDeleted) {
      storyApprovalService
        .selectByNonApproval(sharedStories.pageIndex, sharedStories.pageSize)
        .then(updateSharedStoriesSuccess)
        .catch(updateSharedStoriesError);
    } else {
      storyApprovalService
        .recoverStoriesDismissed(
          sharedStories.pageIndex,
          sharedStories.pageSize
        )
        .then(updateSharedStoriesSuccess)
        .catch(updateSharedStoriesError);
    }
  };

  const updateSharedStoriesSuccess = (res) => {
    _logger(res);
    setSharedStories((prevState) => ({
      ...prevState,
      pagedItems: res.item.pagedItems,
      totalResults: res.item.totalCount,
      pageIndex: res.item.pageIndex,
      pageSize: res.item.pageSize,
    }));
  };

  const updateSharedStoriesError = (err) => {
    _logger(err);
  };

  const toggleShowShowDeleted = (e) => {
    _logger(e);
    setShowDeleted(!showDeleted);
  };

  const handleUpdateApprovalSuccess = (storyId) => {
    const updatedStories = sharedStories.pagedItems.filter(
      (story) => story.id !== storyId
    );
    setSharedStories((prevState) => ({
      ...prevState,
      pagedItems: updatedStories,
      totalResults: response.item.totalCount,
    }));
  };

  const mappingOfStories = (story) => {
    return (
      <StoryTable
        key={story.id}
        row={story}
        deleteHandler={onDeleteStory}
        recoverDismissed={onDeleteStory}
        isDeleted={showDeleted}
      />
    );
  };

  const handleDeleteStory = (idToBeDeleted, storyId) => {
    _logger("handleDeleteStory is Dismissed", idToBeDeleted, storyId);

    setSharedStories((prevState) => {
      const newState = { ...prevState };
      const indexOfDeactivated = newState.pagedItems.findIndex(
        (story) => story.id === idToBeDeleted
      );
      if (indexOfDeactivated >= 0) {
        newState.pagedItems.splice(indexOfDeactivated, 1);
        newState.sharedStories = newState.pagedItems.map(mappingOfStories);
      }
      return newState;
    });
  };

  const onChangePage = (e) => {
    setSharedStories((prevState) => ({
      ...prevState,
      pageIndex: e - 1,
    }));
  };

  return (
    <React.Fragment>
      <div className="main-landing">
        <div className="card-header">
          <div className="card text-md-primary">
            <div className="card-texts">
              <div className="card-body vertical-align-bottom display-4 fw-bold center">
                <h4 className="card-title"></h4>
                <p type="text" className="storyapprovalBannerText">
                  Member Stories for Approval
                </p>
              </div>
            </div>
          </div>
        </div>
        <hr />
        <div className="text-primary">
          <h3 className="text" type="text" id="approval-page-text1">
            {" "}
            Please take a moment to review the stories uploaded here by the
            members of WePairHealth.
          </h3>
          <div className="text-black">
            <h5 className="text" type="text" id="approval-page-text2">
              <em> </em>
              NOTE: Approved Stories Will List on the Public Facing Share Your
              Story Page for WePairHealth
            </h5>
          </div>
        </div>
        <hr />
        <div>
          <div className="d-flex justify-content-between">
            <Pagination
              className="pagination-item pagination-item-active"
              onChange={onChangePage}
              current={sharedStories.pageIndex + 1}
              total={sharedStories.totalResults}
              pagesize={sharedStories.pageSize}
            ></Pagination>
            <button
              className="btn btn-outline-dark"
              onClick={toggleShowShowDeleted}
            >
              {showDeleted
                ? "Hide Dismissed Stories"
                : "Show Dismissed Stories"}
            </button>
          </div>

          <StoryTable
            stories={sharedStories.pagedItems}
            onDeleteStory={handleDeleteStory}
            onUpdateApprovalSuccess={handleUpdateApprovalSuccess}
          ></StoryTable>
        </div>
      </div>
    </React.Fragment>
  );
}

export default StoryApproval;
