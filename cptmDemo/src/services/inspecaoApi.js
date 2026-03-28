const BASE_URL = "/api/Inspecao";

async function parseResponse(response) {
  if (!response.ok) {
    const body = await response.text();
    throw new Error(body || "Falha ao comunicar com a API de inspeções.");
  }

  if (response.status === 204) {
    return null;
  }

  return response.json();
}

function toFormData(payload) {
  const formData = new FormData();
  formData.append("titulo", payload.titulo);
  formData.append("descricao", payload.descricao);
  formData.append("data", payload.data);
  formData.append("Photo", payload.photo);
  return formData;
}

export async function listInspecoes() {
  const response = await fetch(BASE_URL);
  return parseResponse(response);
}

export async function createInspecao(payload) {
  const response = await fetch(BASE_URL, {
    method: "POST",
    body: toFormData(payload),
  });

  return parseResponse(response);
}

export async function updateInspecao(id, payload) {
  const response = await fetch(`${BASE_URL}/${id}`, {
    method: "PUT",
    body: toFormData(payload),
  });

  return parseResponse(response);
}

export async function deleteInspecao(id) {
  const response = await fetch(`${BASE_URL}/${id}`, {
    method: "DELETE",
  });

  return parseResponse(response);
}
