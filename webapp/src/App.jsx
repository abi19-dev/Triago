import React, { useEffect, useState } from "react";
import triagoLogo from "./Images/TriagoLogo.png";
import triagoTextLogo from "./Images/TriagoTextLogo.png";
import {
  createEmail,
  getEmails,
  getSettings,
  submitReview,
  updateSettings
} from "./api.js";

const emptyEmail = { sender: "", subject: "", body: "" };

export default function App() {
  const [emails, setEmails] = useState([]);
  const [draft, setDraft] = useState(emptyEmail);
  const [settings, setSettings] = useState(null);
  const [status, setStatus] = useState("");
  const [settingsStatus, setSettingsStatus] = useState("");
  const [activeTab, setActiveTab] = useState("inbox");
  const [selectedEmail, setSelectedEmail] = useState(null);
  const [isLoading, setIsLoading] = useState(false);

  const load = async () => {
    setIsLoading(true);
    try {
      const [items, currentSettings] = await Promise.all([
        getEmails(),
        getSettings()
      ]);
      setEmails(items);
      setSettings(currentSettings);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  useEffect(() => {
    if (activeTab === "settings" || activeTab === "compose") {
      return;
    }

    const intervalId = setInterval(() => {
      if (!isLoading) {
        load();
      }
    }, 4000);

    return () => clearInterval(intervalId);
  }, [activeTab, isLoading]);

  const handleSubmit = async (event) => {
    event.preventDefault();
    setStatus("Queueing email...");
    await createEmail(draft);
    setDraft(emptyEmail);
    await load();
    setStatus("Queued.");
  };

  const handleSettingsSave = async () => {
    if (!settings) return;
    setSettingsStatus("Saving settings...");
    await updateSettings(settings);
    setSettingsStatus("Settings saved.");
  };

  const handleReview = async (emailId, label) => {
    setIsLoading(true);
    await submitReview({ emailId, label, reviewer: "student" });
    await load();
  };

  const filteredEmails = () => {
    switch (activeTab) {
      case "pending":
        return emails.filter((email) => email.status === "PendingReview");
      case "blocked":
        return emails.filter((email) => email.status === "Blocked");
      case "completed":
        return emails.filter((email) => email.status === "Completed");
      case "reviewed":
        return emails.filter((email) => email.status === "Reviewed");
      default:
        return emails;
    }
  };

  const listEmails = filteredEmails();

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="brand">
          <img className="brand-mark" src={triagoLogo} alt="Triago logo" />
          <div className="brand-text">
            <img src={triagoTextLogo} alt="Triago" />
            <span>Agent Console</span>
          </div>
        </div>
        <button className="compose" onClick={() => setActiveTab("compose")}>
          Compose
        </button>
        <nav className="nav">
          <button
            className={`nav-item ${activeTab === "inbox" ? "active" : ""}`}
            onClick={() => setActiveTab("inbox")}
          >
            Inbox
          </button>
          <button
            className={`nav-item ${activeTab === "pending" ? "active" : ""}`}
            onClick={() => setActiveTab("pending")}
          >
            Pending Review
          </button>
          <button
            className={`nav-item ${activeTab === "blocked" ? "active" : ""}`}
            onClick={() => setActiveTab("blocked")}
          >
            Blocked
          </button>
          <button
            className={`nav-item ${activeTab === "completed" ? "active" : ""}`}
            onClick={() => setActiveTab("completed")}
          >
            Completed
          </button>
          <button
            className={`nav-item ${activeTab === "reviewed" ? "active" : ""}`}
            onClick={() => setActiveTab("reviewed")}
          >
            Reviewed
          </button>
          <button
            className={`nav-item ${activeTab === "settings" ? "active" : ""}`}
            onClick={() => setActiveTab("settings")}
          >
            Settings
          </button>
        </nav>
        {settings && (
          <div className="sidebar-card">
            <p>New gold since train</p>
            <strong>{settings.newGoldSinceLastTrain}</strong>
            <span>Threshold: {settings.goldThreshold}</span>
          </div>
        )}
      </aside>

      <div className="content">
        <header className="topbar">
          <div>
            <h1>Sense. Think. Act. Learn.</h1>
            <p>
              A triage dashboard that scores, routes, and retrains from human
              reviews.
            </p>
          </div>
          <div className="topbar-status">
            <span>System</span>
            <strong>{settings?.retrainEnabled ? "Active" : "Paused"}</strong>
          </div>
        </header>

        {activeTab === "settings" && (
          <section className="settings-panel">
            <h2>System Settings</h2>
            {settings && (
              <div className="settings-grid">
                <label>
                  Review threshold
                  <input
                    type="number"
                    step="0.05"
                    value={settings.reviewThreshold}
                    onChange={(e) =>
                      setSettings({
                        ...settings,
                        reviewThreshold: Number(e.target.value)
                      })
                    }
                  />
                </label>
                <label>
                  Block threshold
                  <input
                    type="number"
                    step="0.05"
                    value={settings.blockThreshold}
                    onChange={(e) =>
                      setSettings({
                        ...settings,
                        blockThreshold: Number(e.target.value)
                      })
                    }
                  />
                </label>
                <label>
                  Retrain enabled
                  <select
                    value={settings.retrainEnabled ? "true" : "false"}
                    onChange={(e) =>
                      setSettings({
                        ...settings,
                        retrainEnabled: e.target.value === "true"
                      })
                    }
                  >
                    <option value="true">true</option>
                    <option value="false">false</option>
                  </select>
                </label>
                <label>
                  Gold threshold
                  <input
                    type="number"
                    value={settings.goldThreshold}
                    onChange={(e) =>
                      setSettings({
                        ...settings,
                        goldThreshold: Number(e.target.value)
                      })
                    }
                  />
                </label>
                <label className="span-2">
                  Spam keywords (CSV)
                  <input
                    type="text"
                    value={settings.spamKeywordsCsv}
                    onChange={(e) =>
                      setSettings({
                        ...settings,
                        spamKeywordsCsv: e.target.value
                      })
                    }
                  />
                </label>
                <button onClick={handleSettingsSave}>Save settings</button>
                {settingsStatus && (
                  <span className="status settings-status">
                    {settingsStatus}
                  </span>
                )}
              </div>
            )}
          </section>
        )}

        <main className="grid">
          {activeTab === "compose" && (
            <section className="panel">
              <h2>Queue a new email</h2>
              <form onSubmit={handleSubmit} className="form compose-form">
                <div className="field">
                  <label htmlFor="compose-sender">Sender</label>
                  <input
                    id="compose-sender"
                    value={draft.sender}
                    onChange={(e) =>
                      setDraft({ ...draft, sender: e.target.value })
                    }
                    placeholder="student@fit.ba"
                    required
                  />
                </div>
                <div className="field">
                  <label htmlFor="compose-subject">Subject</label>
                  <input
                    id="compose-subject"
                    value={draft.subject}
                    onChange={(e) =>
                      setDraft({ ...draft, subject: e.target.value })
                    }
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
                    onChange={(e) =>
                      setDraft({ ...draft, body: e.target.value })
                    }
                    placeholder="Write the email content here..."
                    required
                  />
                </div>
                <button type="submit">Send to agent</button>
                <p className="status">{status}</p>
              </form>
            </section>
          )}

          {activeTab !== "settings" && activeTab !== "compose" && (
            <section className="panel">
              <div className="panel-header">
                <h2>
                  {activeTab === "pending"
                    ? "Pending Review"
                    : activeTab === "blocked"
                    ? "Blocked"
                    : activeTab === "completed"
                    ? "Completed"
                    : activeTab === "reviewed"
                    ? "Reviewed"
                    : "Inbox"}
                </h2>
                {isLoading && <span className="loading-pill">Loading...</span>}
              </div>
              <div className="email-list">
                {!isLoading && listEmails.length === 0 && (
                  <p className="status">No emails in this view yet.</p>
                )}
                {listEmails.map((email) => (
                  <article
                    className="email-card clickable"
                    key={email.id}
                    onClick={() => setSelectedEmail(email)}
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
                          <button
                            onClick={() => handleReview(email.id, "NotSpam")}
                            disabled={isLoading}
                          >
                            Mark not spam
                          </button>
                          <button
                            onClick={() => handleReview(email.id, "Spam")}
                            disabled={isLoading}
                          >
                            Mark spam
                          </button>
                        </div>
                      )}
                    </div>
                  </article>
                ))}
              </div>
            </section>
          )}

          {selectedEmail && activeTab !== "compose" && (
            <section className="panel email-detail">
              <div className="panel-header">
                <div>
                  <p className="meta">{selectedEmail.sender}</p>
                  <h2>{selectedEmail.subject}</h2>
                </div>
                <button onClick={() => setSelectedEmail(null)}>Back</button>
              </div>
              <div className="email-meta">
                <span>Status: {selectedEmail.status}</span>
                <span>
                  Decision: {selectedEmail.decision ?? "pending"}
                </span>
                <span>
                  Score: {selectedEmail.spamScore?.toFixed(2) ?? "n/a"}
                </span>
              </div>
              <div className="email-body">
                {selectedEmail.body}
              </div>
            </section>
          )}
        </main>
      </div>
    </div>
  );
}
