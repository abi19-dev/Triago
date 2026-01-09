import React, { useEffect, useState } from "react";
import Sidebar from "./components/Sidebar.jsx";
import Topbar from "./components/Topbar.jsx";
import SettingsPanel from "./components/SettingsPanel.jsx";
import ComposePanel from "./components/ComposePanel.jsx";
import EmailListPanel from "./components/EmailListPanel.jsx";
import EmailDetailPanel from "./components/EmailDetailPanel.jsx";
import {
  analyzeWithLlm,
  createEmail,
  deleteEmail,
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
  const [savedSettings, setSavedSettings] = useState(null);
  const [status, setStatus] = useState("");
  const [settingsStatus, setSettingsStatus] = useState("");
  const [activeTab, setActiveTab] = useState("inbox");
  const [selectedEmailId, setSelectedEmailId] = useState(null);
  const [isLoading, setIsLoading] = useState(false);
  const [detailLlmStatus, setDetailLlmStatus] = useState("");
  const [detailLlmResult, setDetailLlmResult] = useState(null);

  const load = async () => {
    setIsLoading(true);
    try {
      const [items, currentSettings] = await Promise.all([
        getEmails(),
        getSettings()
      ]);
      setEmails(items);
      setSettings(currentSettings);
      setSavedSettings(currentSettings);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  useEffect(() => {
    const applyRoute = () => {
      const hash = window.location.hash.replace("#", "");
      if (hash && hash !== "llm") {
        setActiveTab(hash);
      }
    };

    applyRoute();
    window.addEventListener("hashchange", applyRoute);
    return () => window.removeEventListener("hashchange", applyRoute);
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

  useEffect(() => {
    setSelectedEmailId(null);
    setDetailLlmStatus("");
    setDetailLlmResult(null);
  }, [activeTab]);

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
    const updated = await updateSettings(settings);
    setSavedSettings(updated);
    setSettingsStatus("Settings saved.");
  };

  const handleReview = async (emailId, label) => {
    setIsLoading(true);
    await submitReview({ emailId, label, reviewer: "student" });
    await load();
  };

  const handleSelectEmail = (email) => {
    setSelectedEmailId(email.id);
    setDetailLlmStatus("");
    setDetailLlmResult(null);
  };

  const handleDetailWhy = async () => {
    if (!selectedEmail) return;
    setDetailLlmStatus("Analyzing...");
    setIsLoading(true);
    try {
      const result = await analyzeWithLlm({
        subject: selectedEmail.subject,
        body: selectedEmail.body
      });
      setDetailLlmResult(result);
      if (result.status === "not_configured") {
        setDetailLlmStatus("Add OpenAI API key in appsettings.json.");
      } else if (result.status === "error") {
        setDetailLlmStatus("LLM error. Check API response.");
      } else {
        setDetailLlmStatus("Explanation ready.");
      }
    } finally {
      setIsLoading(false);
    }
  };

  const handleDeleteEmail = async () => {
    if (!selectedEmail) return;
    setIsLoading(true);
    try {
      await deleteEmail(selectedEmail.id);
      handleCloseEmail();
      await load();
    } finally {
      setIsLoading(false);
    }
  };

  const handleCloseEmail = () => {
    setSelectedEmailId(null);
    setDetailLlmStatus("");
    setDetailLlmResult(null);
  };

  const filteredEmails = () => {
    switch (activeTab) {
      case "pending":
        return emails.filter((email) => email.status === "PendingReview");
      case "blocked":
        return emails.filter(
          (email) =>
            email.status === "Blocked" || email.decision === "Block"
        );
      case "completed":
        return emails.filter((email) => email.status === "Completed");
      case "reviewed":
        return emails.filter((email) => email.status === "Reviewed");
      default:
        return emails;
    }
  };

  const listEmails = filteredEmails();
  const selectedEmail = selectedEmailId
    ? emails.find((email) => email.id === selectedEmailId) ?? null
    : null;

  return (
    <div className="app-shell">
      <Sidebar activeTab={activeTab} settings={settings} />

      <div className="content">
        <Topbar retrainEnabled={savedSettings?.retrainEnabled} />

        {activeTab === "settings" && (
          <SettingsPanel
            settings={settings}
            settingsStatus={settingsStatus}
            onSave={handleSettingsSave}
            onChange={setSettings}
          />
        )}

        <main className="grid">
          {activeTab === "compose" && (
            <ComposePanel
              draft={draft}
              status={status}
              onSubmit={handleSubmit}
              onChange={setDraft}
            />
          )}

          {activeTab !== "settings" && activeTab !== "compose" && (
            <EmailListPanel
              activeTab={activeTab}
              emails={listEmails}
              isLoading={isLoading}
              onSelectEmail={handleSelectEmail}
              onReview={handleReview}
            />
          )}

          <EmailDetailPanel
            email={selectedEmail}
            isLoading={isLoading}
            detailLlmStatus={detailLlmStatus}
            detailLlmResult={detailLlmResult}
            onClose={handleCloseEmail}
            onAskWhy={handleDetailWhy}
            onDelete={handleDeleteEmail}
          />
        </main>
      </div>
    </div>
  );
}
