const defaultHeaders = {
  "Content-Type": "application/json"
};

export async function getEmails(take = 25) {
  const response = await fetch(`/api/emails?take=${take}`);
  return response.json();
}

export async function createEmail(payload) {
  const response = await fetch("/api/emails", {
    method: "POST",
    headers: defaultHeaders,
    body: JSON.stringify(payload)
  });
  return response.json();
}

export async function getSettings() {
  const response = await fetch("/api/settings");
  return response.json();
}

export async function updateSettings(payload) {
  const response = await fetch("/api/settings", {
    method: "PUT",
    headers: defaultHeaders,
    body: JSON.stringify(payload)
  });
  return response.json();
}

export async function submitReview(payload) {
  await fetch("/api/reviews", {
    method: "POST",
    headers: defaultHeaders,
    body: JSON.stringify(payload)
  });
}
