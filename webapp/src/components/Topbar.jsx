import React from "react";

export default function Topbar({ retrainEnabled }) {
  return (
    <header className="topbar">
      <div>
        <h1>Sense. Think. Act. Learn.</h1>
        <p>A triage dashboard that scores, routes, and retrains from human reviews.</p>
      </div>
      <div className="topbar-status">
        <span>System</span>
        <strong>{retrainEnabled ? "Active" : "Paused"}</strong>
      </div>
    </header>
  );
}
