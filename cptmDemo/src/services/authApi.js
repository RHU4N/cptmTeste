const configuredApiBaseUrl = import.meta.env.VITE_API_BASE_URL?.replace(/\/$/, "")
const apiBaseUrl = configuredApiBaseUrl ?? (import.meta.env.DEV ? "" : "http://localhost:5085")
const LOGIN_URL = `${apiBaseUrl}/api/Auth/login`
const REGISTER_URL = `${apiBaseUrl}/api/Auth/register`
const AUTH_SESSION_KEY = "cptm.auth.session.v1"

function readSession() {
  try {
    const raw = localStorage.getItem(AUTH_SESSION_KEY)
    return raw ? JSON.parse(raw) : null
  } catch {
    return null
  }
}

function writeSession(session) {
  localStorage.setItem(AUTH_SESSION_KEY, JSON.stringify(session))
}

export function clearAuthSession() {
  localStorage.removeItem(AUTH_SESSION_KEY)
}

export function getAuthSession() {
  const session = readSession()
  if (!session?.token || !session?.expiresAtUtc) {
    return null
  }

  if (new Date(session.expiresAtUtc).getTime() <= Date.now()) {
    clearAuthSession()
    return null
  }

  return session
}

export function getAccessToken() {
  return getAuthSession()?.token || ""
}

export async function login(username, password) {
  const response = await fetch(LOGIN_URL, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({ username, password }),
  })

  if (!response.ok) {
    const message = await response.text()
    throw new Error(message || "Falha no login")
  }

  const payload = await response.json()
  writeSession(payload)
  return payload
}

export async function register(username, password, activationCode) {
  const response = await fetch(REGISTER_URL, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({ username, password, activationCode }),
  })

  if (!response.ok) {
    const message = await response.text()
    throw new Error(message || "Falha no cadastro")
  }

  const payload = await response.json()
  writeSession(payload)
  return payload
}