import React, { useEffect, useState } from "react";
import "./StoriesStyles.scss";
import shareStoryService from "../../services/shareStoryService";
import Pagination from "rc-pagination";
import "rc-pagination/assets/index.css";
import ShareStoryCard from "./ShareStoryCard";
import debug from "debug";
import locale from "rc-pagination/lib/locale/en_US";

const _logger = debug.extend("sharedStory");

function ShareStory() {
  const [sharedStories, setSharedStories] = useState({
    pagedItems: [],
    mappedItems: [],
    totalResults: 0,
    pageIndex: 1,
    pageSize: 5,
  });

  useEffect(() => {
    shareStoryService
      .selectByApproval(sharedStories.pageIndex - 1, sharedStories.pageSize)
      .then(onGetSuccess)
      .catch(onGetErr);
  }, [sharedStories.pageIndex]);

  const onCurrentPage = (page) => {
    setSharedStories((prevState) => {
      let newState = { ...prevState };
      newState.pageIndex = page;
      return newState;
    });
  };

  function onGetErr(err) {
    _logger(err);
  }

  const onGetSuccess = (response) => {
    _logger(response);
    const storiesArray = response.item.pagedItems;
    setSharedStories((prevState) => {
      const newState = { ...prevState };
      newState.pagedItems = storiesArray;
      newState.mappedItems = storiesArray.map(mapSharedStories);
      newState.totalResults = response.item.totalCount;
      return newState;
    });
  };

  const mapSharedStories = (story) => {
    return <ShareStoryCard key={story.id} story={story} />;
  };

  return (
    <>
      <div className="main-landing">
        <div className="card-header">
          <div className="card text-bg-dark">
            <img
              src="https://imgur.com/1TRu33z.jpg"
              className="storyBannerImg"
              alt="story-banner-image-card"
            />
            <div className="card-img-overlay">
              <div className="card-texts">
                <div className="card-body vertical-align-bottom display-4 fw-bold center">
                  <h1 className="card-title"></h1>
                  <p type="text" className="storyBannerText">
                    <small>Stories of WePairHealth</small>
                  </p>
                </div>
              </div>
            </div>
          </div>
          <hr />
          <span className="placeholder col-12 bg-light align-center" />
          <div className="describingText">
            <h4 className="text-center">
              <strong>
                {" "}
                Though each story is unique, they illuminate the many ways
                WePairHealth does exactly that.
              </strong>
            </h4>
          </div>
        </div>
        <span className="placeholder col-12 bg-light align-center" />
        <hr className="mb-5" />
        <Pagination
          className="pagination-item pagination-item-active text-center mb-4"
          onChange={onCurrentPage}
          current={sharedStories.pageIndex}
          pageSize={sharedStories.pageSize}
          total={sharedStories.totalResults}
          locale={locale}
        ></Pagination>
        {sharedStories.mappedItems}
        <Pagination
          className="pagination-item pagination-item-active text-center mb-4"
          onChange={onCurrentPage}
          current={sharedStories.pageIndex}
          pageSize={sharedStories.pageSize}
          total={sharedStories.totalResults}
          locale={locale}
        ></Pagination>
      </div>
    </>
  );
}
export default ShareStory;
