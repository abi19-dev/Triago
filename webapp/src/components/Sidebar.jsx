import React from "react";
import triagoLogo from "../Images/TriagoLogo.png";
import triagoTextLogo from "../Images/TriagoTextLogo.png";

export default function Sidebar({ activeTab, settings }) {
  const goTo = (tab) => {
    window.location.hash = tab;
  };

  return (
    <aside className="sidebar">
      <div className="brand">
        <img className="brand-mark" src={triagoLogo} alt="Triago logo" />
        <div className="brand-text">
          <img src={triagoTextLogo} alt="Triago" />
          <span>Agent Console</span>
        </div>
      </div>
      <button className="compose" onClick={() => goTo("compose")}>
        Compose
      </button>
      <nav className="nav">
        <button
          className={`nav-item ${activeTab === "inbox" ? "active" : ""}`}
          onClick={() => goTo("inbox")}
        >
          Inbox
        </button>
        <button
          className={`nav-item ${activeTab === "pending" ? "active" : ""}`}
          onClick={() => goTo("pending")}
        >
          Pending Review
        </button>
        <button
          className={`nav-item ${activeTab === "blocked" ? "active" : ""}`}
          onClick={() => goTo("blocked")}
        >
          Blocked
        </button>
        <button
          className={`nav-item ${activeTab === "completed" ? "active" : ""}`}
          onClick={() => goTo("completed")}
        >
          Completed
        </button>
        <button
          className={`nav-item ${activeTab === "reviewed" ? "active" : ""}`}
          onClick={() => goTo("reviewed")}
        >
          Reviewed
        </button>
        <button
          className={`nav-item ${activeTab === "settings" ? "active" : ""}`}
          onClick={() => goTo("settings")}
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
  );
}
