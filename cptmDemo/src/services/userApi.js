import { getAccessToken } from './authApi'

const configuredApiBaseUrl = import.meta.env.VITE_API_BASE_URL?.replace(/\/$/, "")
const apiBaseUrl = configuredApiBaseUrl ?? (import.meta.env.DEV ? "" : "http://localhost:5085")
const USERS_URL = `${apiBaseUrl}/api/Users`

function authHeaders() {
  const token = getAccessToken()
  return {
    'Content-Type': 'application/json',
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
  }
}

export async function getUsers(filters = {}) {
  const params = new URLSearchParams()
  if (filters.role) params.append('role', filters.role)
  if (filters.search) params.append('search', filters.search)

  const url = params.toString() ? `${USERS_URL}?${params.toString()}` : USERS_URL
  const res = await fetch(url, { headers: authHeaders() })
  if (!res.ok) throw new Error(await res.text())
  return res.json()
}

export async function createUser(username, password, role = 'user') {
  const res = await fetch(USERS_URL, {
    method: 'POST',
    headers: authHeaders(),
    body: JSON.stringify({ username, password, role }),
  })

  if (!res.ok) {
    const txt = await res.text()
    throw new Error(txt || 'Falha ao criar usuário')
  }

  return res.json()
}

export async function updateUserRole(id, role) {
  const res = await fetch(`${USERS_URL}/${id}/role`, {
    method: 'PUT',
    headers: authHeaders(),
    body: JSON.stringify({ role }),
  })

  if (!res.ok) {
    const txt = await res.text()
    throw new Error(txt || 'Falha ao atualizar role')
  }
}

export async function deleteUser(id) {
  const res = await fetch(`${USERS_URL}/${id}`, {
    method: 'DELETE',
    headers: authHeaders(),
  })

  if (!res.ok) {
    const txt = await res.text()
    throw new Error(txt || 'Falha ao remover usuário')
  }
}

export default { getUsers, createUser, updateUserRole, deleteUser }
