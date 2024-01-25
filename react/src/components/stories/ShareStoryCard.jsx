import React, { useEffect, useState } from "react";
import "./ShareStoryCard.scss";
import PropTypes from "prop-types";
import debug from "debug";

const _logger = debug.extend("sharedStory");

const ShareStoryCard = (props) => {
  _logger(props);

  const [lastUpdate, setLastUpdate] = useState("");

  const findLastUpdated = () => {
    const createdDate = new Date(props.story.dateCreated);
    const now = new Date();

    const timeDifference = now - createdDate;
    const minutesDifference = Math.floor(timeDifference / (1000 * 60));
    const hoursDifference = Math.floor(timeDifference / (1000 * 60 * 60));
    const daysDifference = Math.floor(timeDifference / (1000 * 60 * 60 * 24));
    if (daysDifference >= 1) {
      return `Last updated ${daysDifference} days ago `;
    } else if (hoursDifference >= 1) {
      return `Last update ${hoursDifference} hours ago`;
    } else {
      return `Last updated ${minutesDifference} minutes ago`;
    }
  };

  useEffect(() => {
    setLastUpdate(findLastUpdated());
  });

  return (
    <React.Fragment>
      <div className="text-center">
        <h5 className="cardTitle display-6 fw-bold  align-center">
          {props.story.name}
        </h5>
        <h4 className="story fw-bold">{props.story.createdBy.firstName}</h4>
      </div>

      <div id="cardstory" className="container-fluid">
        <div className="card mb-8">
          <div className="row g-0">
            <div className="col-md-5">
              <img
                src={props.story.storyFileUrl}
                alt="SharedStorySubmissionImage"
                width="100%"
                height="500px"
                className="image-fluid mx-5 rounded-start object-fit-cover gx-5"
              />
            </div>

            <div className="col-md-7 my-auto align-items-right">
              <div className="card-body mx-5">
                <p className="card-text mx-5">
                  <div
                    className="story-text"
                    dangerouslySetInnerHTML={{ __html: props.story.story }}
                  />
                </p>

                <p className="card-timer-text text-body-secondary fs-25px float-end">
                  <em>{lastUpdate}</em>
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
      <span className="placeholder col-12 bg-light align-center" />
      <span className="placeholder col-12 bg-light align-center" />
      <div className="line"></div>
    </React.Fragment>
  );
};

ShareStoryCard.propTypes = {
  story: PropTypes.shape({
    id: PropTypes.number.isRequired,
    name: PropTypes.string.isRequired,
    story: PropTypes.string.isRequired,
    storyFileUrl: PropTypes.string.isRequired,
    createdBy: PropTypes.shape({
      firstName: PropTypes.string.isRequired,
      lastName: PropTypes.string.isRequired,
    }).isRequired,
    dateCreated: PropTypes.string.isRequired,
  }),
};

export default ShareStoryCard;
