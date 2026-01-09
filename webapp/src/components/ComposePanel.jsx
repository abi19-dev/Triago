import React from "react";

export default function ComposePanel({ draft, status, onSubmit, onChange }) {
  return (
    <section className="panel">
      <h2>Queue a new email</h2>
      <form onSubmit={onSubmit} className="form compose-form">
        <div className="field">
          <label htmlFor="compose-sender">Sender</label>
          <input
            id="compose-sender"
            value={draft.sender}
            onChange={(e) => onChange({ ...draft, sender: e.target.value })}
            placeholder="student@fit.ba"
            required
          />
        </div>
        <div className="field">
          <label htmlFor="compose-subject">Subject</label>
          <input
            id="compose-subject"
            value={draft.subject}
            onChange={(e) => onChange({ ...draft, subject: e.target.value })}
            placeholder="Project status update"
            required
          />
        </div>
        <div className="field">
          <label htmlFor="compose-body">Body</label>
          <textarea
            id="compose-body"
            rows="8"
            value={draft.body}
            onChange={(e) => onChange({ ...draft, body: e.target.value })}
            placeholder="Write the email content here..."
            required
          />
        </div>
        <button type="submit">Send to agent</button>
        <p className="status">{status}</p>
      </form>
    </section>
  );
}
