import React from "react";

const titleMap = {
  pending: "Pending Review",
  blocked: "Blocked",
  completed: "Completed",
  reviewed: "Reviewed",
  inbox: "Inbox"
};

export default function EmailListPanel({
  activeTab,
  emails,
  isLoading,
  onSelectEmail,
  onReview
}) {
  return (
    <section className="panel">
      <div className="panel-header">
        <h2>{titleMap[activeTab] ?? "Inbox"}</h2>
      </div>
      <div className="email-list">
        {!isLoading && emails.length === 0 && (
          <p className="status">No emails in this view yet.</p>
        )}
        {emails.map((email) => (
          <article
            className="email-card clickable"
            key={email.id}
            onClick={() => onSelectEmail(email)}
          >
            <div>
              <p className="meta">
                {email.sender} · {email.status}
              </p>
              <h3>{email.subject}</h3>
              <p className="body">{email.body}</p>
            </div>
            <div className="email-actions">
              <span className="score">
                Score: {email.spamScore?.toFixed(2) ?? "n/a"}
              </span>
              <span className="decision">
                Decision: {email.decision ?? "pending"}
              </span>
              {email.status === "PendingReview" && (
                <div className="review-actions">
                  <button onClick={() => onReview(email.id, "NotSpam")} disabled={isLoading}>
                    Mark not spam
                  </button>
                  <button onClick={() => onReview(email.id, "Spam")} disabled={isLoading}>
                    Mark spam
                  </button>
                </div>
              )}
            </div>
          </article>
        ))}
      </div>
    </section>
  );
}
