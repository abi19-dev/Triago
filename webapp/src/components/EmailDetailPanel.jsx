import React from "react";

export default function EmailDetailPanel({
  email,
  isLoading,
  detailLlmStatus,
  detailLlmResult,
  onClose,
  onAskWhy,
  onDelete
}) {
  if (!email) return null;

  return (
    <section className="panel email-detail" key={email.id}>
      <div className="panel-header">
        <div>
          <p className="meta">{email.sender}</p>
          <h2>{email.subject}</h2>
        </div>
        <button className="close-button" onClick={onClose}>
          ×
        </button>
      </div>
      <div className="email-meta">
        <span>Status: {email.status}</span>
        <span>Decision: {email.decision ?? "pending"}</span>
        <span>Score: {email.spamScore?.toFixed(2) ?? "n/a"}</span>
      </div>
      <div className="email-body">{email.body}</div>
      <div className="detail-footer">
        <button onClick={onAskWhy} disabled={isLoading}>
          Ask GPT Why?
        </button>
        <button onClick={onDelete} className="danger" disabled={isLoading}>
          Delete email
        </button>
      </div>
      {detailLlmStatus && (
        <p className="status llm-detail-status">{detailLlmStatus}</p>
      )}
      {detailLlmResult?.rationale && (
        <div className="llm-detail">
          <h3>Why this decision?</h3>
          <div className="llm-metrics">
            <span>
              Agent score:{" "}
              {detailLlmResult.spamScore != null
                ? detailLlmResult.spamScore.toFixed(2)
                : "n/a"}
            </span>
          </div>
          <p className="status">{detailLlmResult.rationale}</p>
          {detailLlmResult.redFlags?.length > 0 && (
            <ul className="llm-flags">
              {detailLlmResult.redFlags.map((flag, index) => (
                <li key={`${flag}-${index}`}>{flag}</li>
              ))}
            </ul>
          )}
        </div>
      )}
    </section>
  );
}
