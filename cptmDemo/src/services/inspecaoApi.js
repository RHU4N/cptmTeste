const configuredApiBaseUrl = import.meta.env.VITE_API_BASE_URL?.replace(/\/$/, "");
const apiBaseUrl = configuredApiBaseUrl ?? (import.meta.env.DEV ? "" : "http://localhost:5085");
const BASE_URL = `${apiBaseUrl}/api/Inspecao`;

const CACHE_KEY = "cptm.inspecoes.cache.v1";
const QUEUE_KEY = "cptm.inspecoes.queue.v1";
const TEMP_ID_PREFIX = "offline-";
const QUEUE_CHANGE_EVENT = "cptm-inspecoes-queue-change";

let syncInProgress = false;

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

function readJson(key, fallback) {
  try {
    const raw = localStorage.getItem(key);
    return raw ? JSON.parse(raw) : fallback;
  } catch {
    return fallback;
  }
}

function writeJson(key, value) {
  localStorage.setItem(key, JSON.stringify(value));
}

function notifyQueueChange() {
  window.dispatchEvent(new CustomEvent(QUEUE_CHANGE_EVENT));
}

function getCachedInspecoes() {
  return readJson(CACHE_KEY, []);
}

function setCachedInspecoes(inspecoes) {
  writeJson(CACHE_KEY, inspecoes);
}

export function getPendingInspecaoCount() {
  return getQueue().length;
}

export function listenPendingInspecoesChange(handler) {
  window.addEventListener(QUEUE_CHANGE_EVENT, handler);
  return () => window.removeEventListener(QUEUE_CHANGE_EVENT, handler);
}

function getQueue() {
  return readJson(QUEUE_KEY, []);
}

function setQueue(queue) {
  writeJson(QUEUE_KEY, queue);
  notifyQueueChange();
}

function isTempId(id) {
  return String(id).startsWith(TEMP_ID_PREFIX);
}

function generateTempId() {
  return `${TEMP_ID_PREFIX}${Date.now()}-${Math.random().toString(36).slice(2, 8)}`;
}

function buildLocalItemFromPayload(id, payload) {
  const dateIso = payload.data || new Date().toISOString();

  return {
    id,
    titulo: payload.titulo,
    descricao: payload.descricao,
    data: dateIso,
    usuario: "offline",
    photoBase64: payload.photoDataUrl ? payload.photoDataUrl.split(",")[1] ?? null : null,
  };
}

function applyOperationToList(list, operation) {
  if (operation.type === "create") {
    return [
      buildLocalItemFromPayload(operation.tempId, operation.payload),
      ...list,
    ];
  }

  if (operation.type === "update") {
    return list.map((item) => {
      if (String(item.id) !== String(operation.id)) {
        return item;
      }

      return {
        ...item,
        titulo: operation.payload.titulo,
        descricao: operation.payload.descricao,
        data: operation.payload.data,
        photoBase64: operation.payload.photoDataUrl
          ? operation.payload.photoDataUrl.split(",")[1] ?? item.photoBase64
          : item.photoBase64,
      };
    });
  }

  if (operation.type === "delete") {
    return list.filter((item) => String(item.id) !== String(operation.id));
  }

  return list;
}

function projectInspecoes(baseList, queue) {
  return queue.reduce((acc, operation) => applyOperationToList(acc, operation), [...baseList]);
}

function fileToDataUrl(file) {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onload = () => resolve(reader.result);
    reader.onerror = () => reject(new Error("Não foi possível ler a imagem para modo offline."));
    reader.readAsDataURL(file);
  });
}

async function serializePayload(payload) {
  const photoDataUrl = payload.photo ? await fileToDataUrl(payload.photo) : null;

  return {
    titulo: payload.titulo,
    descricao: payload.descricao,
    data: payload.data,
    photoDataUrl,
  };
}

async function dataUrlToFile(dataUrl, filename) {
  const response = await fetch(dataUrl);
  const blob = await response.blob();
  const extension = blob.type.split("/")[1] || "jpg";

  return new File([blob], `${filename}.${extension}`, { type: blob.type || "image/jpeg" });
}

async function deserializePayload(payload, filenamePrefix) {
  return {
    titulo: payload.titulo,
    descricao: payload.descricao,
    data: payload.data,
    photo: payload.photoDataUrl ? await dataUrlToFile(payload.photoDataUrl, filenamePrefix) : null,
  };
}

function upsertQueueOperation(newOperation) {
  let queue = getQueue();

  if (newOperation.type === "create") {
    queue.push(newOperation);
    setQueue(queue);
    return;
  }

  if (newOperation.type === "update") {
    if (isTempId(newOperation.id)) {
      queue = queue.map((operation) => {
        if (operation.type === "create" && String(operation.tempId) === String(newOperation.id)) {
          return {
            ...operation,
            payload: newOperation.payload,
          };
        }

        return operation;
      });

      setQueue(queue);
      return;
    }

    queue = queue.filter(
      (operation) =>
        !(
          String(operation.id) === String(newOperation.id) &&
          (operation.type === "update" || operation.type === "delete")
        ),
    );

    queue.push(newOperation);
    setQueue(queue);
    return;
  }

  if (newOperation.type === "delete") {
    if (isTempId(newOperation.id)) {
      queue = queue.filter(
        (operation) =>
          !(
            (operation.type === "create" && String(operation.tempId) === String(newOperation.id)) ||
            (String(operation.id) === String(newOperation.id) &&
              (operation.type === "update" || operation.type === "delete"))
          ),
      );

      setQueue(queue);
      return;
    }

    queue = queue.filter(
      (operation) =>
        !(
          String(operation.id) === String(newOperation.id) &&
          (operation.type === "update" || operation.type === "delete")
        ),
    );

    queue.push(newOperation);
    setQueue(queue);
  }
}

function getProjectedCache() {
  return projectInspecoes(getCachedInspecoes(), getQueue());
}

async function request(url, options) {
  try {
    const response = await fetch(url, options);
    return parseResponse(response);
  } catch (error) {
    if (!navigator.onLine) {
      throw new Error(
        "Sem conexão com a internet. Os dados podem estar indisponíveis até que o app tenha cache local ou a rede volte.",
      );
    }

    if (error instanceof TypeError) {
      throw new Error(
        "Não foi possível alcançar a API. No build/PWA, configure VITE_API_BASE_URL (ex.: http://localhost:5085).",
      );
    }

    throw error;
  }
}

export async function syncPendingInspecoes() {
  if (!navigator.onLine || syncInProgress) {
    return;
  }

  const queue = getQueue();

  if (!queue.length) {
    return;
  }

  syncInProgress = true;

  try {
    for (let index = 0; index < queue.length; index += 1) {
      const operation = queue[index];

      if (operation.type === "create") {
        const payload = await deserializePayload(operation.payload, `offline-create-${index}`);
        await request(BASE_URL, {
          method: "POST",
          body: toFormData(payload),
        });
      }

      if (operation.type === "update") {
        const payload = await deserializePayload(operation.payload, `offline-update-${operation.id}`);
        await request(`${BASE_URL}/${operation.id}`, {
          method: "PUT",
          body: toFormData(payload),
        });
      }

      if (operation.type === "delete") {
        await request(`${BASE_URL}/${operation.id}`, {
          method: "DELETE",
        });
      }

      setQueue(queue.slice(index + 1));
    }

    const freshData = await request(BASE_URL);
    setCachedInspecoes(freshData);
  } finally {
    syncInProgress = false;
  }
}

function toFormData(payload) {
  const formData = new FormData();
  formData.append("titulo", payload.titulo);
  formData.append("descricao", payload.descricao);
  formData.append("data", payload.data);

  if (payload.photo) {
    formData.append("Photo", payload.photo);
  }

  return formData;
}

export async function listInspecoes() {
  if (navigator.onLine) {
    try {
      await syncPendingInspecoes();
      const data = await request(BASE_URL);
      setCachedInspecoes(data);
      return data;
    } catch (error) {
      const fallback = getProjectedCache();

      if (fallback.length) {
        return fallback;
      }

      throw error;
    }
  }

  return getProjectedCache();
}

export async function createInspecao(payload) {
  if (navigator.onLine) {
    return request(BASE_URL, {
      method: "POST",
      body: toFormData(payload),
    });
  }

  const serializedPayload = await serializePayload(payload);
  const tempId = generateTempId();

  upsertQueueOperation({
    type: "create",
    tempId,
    payload: serializedPayload,
  });

  return {
    queued: true,
    id: tempId,
  };
}

export async function updateInspecao(id, payload) {
  if (navigator.onLine) {
    return request(`${BASE_URL}/${id}`, {
      method: "PUT",
      body: toFormData(payload),
    });
  }

  const serializedPayload = await serializePayload(payload);

  upsertQueueOperation({
    type: "update",
    id,
    payload: serializedPayload,
  });

  return {
    queued: true,
    id,
  };
}

export async function deleteInspecao(id) {
  if (navigator.onLine) {
    return request(`${BASE_URL}/${id}`, {
      method: "DELETE",
    });
  }

  upsertQueueOperation({
    type: "delete",
    id,
  });

  return {
    queued: true,
    id,
  };
}
