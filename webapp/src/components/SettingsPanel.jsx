import React from "react";

export default function SettingsPanel({ settings, settingsStatus, onSave, onChange }) {
  if (!settings) return null;

  return (
    <section className="settings-panel">
      <h2>System Settings</h2>
      <div className="settings-grid">
        <label>
          Review threshold
          <input
            type="number"
            step="0.05"
            value={settings.reviewThreshold}
            onChange={(e) =>
              onChange({ ...settings, reviewThreshold: Number(e.target.value) })
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
              onChange({ ...settings, blockThreshold: Number(e.target.value) })
            }
          />
        </label>
        <label>
          Retrain enabled
          <select
            value={settings.retrainEnabled ? "true" : "false"}
            onChange={(e) =>
              onChange({
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
              onChange({ ...settings, goldThreshold: Number(e.target.value) })
            }
          />
        </label>
        <label className="span-2">
          Spam keywords (CSV)
          <input
            type="text"
            value={settings.spamKeywordsCsv}
            onChange={(e) =>
              onChange({ ...settings, spamKeywordsCsv: e.target.value })
            }
          />
        </label>
        <button onClick={onSave}>Save settings</button>
        {settingsStatus && (
          <span className="status settings-status">{settingsStatus}</span>
        )}
      </div>
    </section>
  );
}
