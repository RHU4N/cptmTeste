<script setup>
import { reactive, watch, computed, ref, onBeforeUnmount, onMounted } from "vue"
import L from "leaflet"
import "leaflet/dist/leaflet.css"

const props = defineProps({
  initialInspecao: {
    type: Object,
    default: null,
  },
  submitting: {
    type: Boolean,
    default: false,
  },
})

const emit = defineEmits(["finalizar", "fechar"])

const form = reactive({
  titulo: "",
  descricao: "",
  data: "",
  photo: null,
  localizacao: "",
  latitude: null,
  longitude: null,
})

const photoInputKey = ref(0)
const photoPreviewUrl = ref("")
const locationMessage = ref("")
const mapContainer = ref(null)
let mapInstance = null
let marker = null
const GEO_ADDRESS_CACHE_KEY = "cptm.geo.reverse.v1"

const isEditMode = computed(() => Boolean(props.initialInspecao?.id))
const initialPhotoPreview = computed(() => {
  if (props.initialInspecao?.photoUrl) {
    return props.initialInspecao.photoUrl
  }

  if (props.initialInspecao?.photoBase64) {
    return `data:image/jpeg;base64,${props.initialInspecao.photoBase64}`
  }

  return ""
})

const photoPreview = computed(() => photoPreviewUrl.value || initialPhotoPreview.value)
const isPhotoRequired = computed(() => !photoPreview.value)
const hasCoordinates = computed(() => Number.isFinite(form.latitude) && Number.isFinite(form.longitude))
const googleMapsUrl = computed(() => {
  if (!hasCoordinates.value) {
    return ""
  }

  return `https://www.google.com/maps?q=${form.latitude},${form.longitude}`
})

function toDatetimeLocalValue(value) {
  if (!value) {
    return ""
  }

  const date = new Date(value)

  if (Number.isNaN(date.getTime())) {
    return ""
  }

  const pad = (number) => String(number).padStart(2, "0")

  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`
}

function revokePreviewUrl() {
  if (photoPreviewUrl.value.startsWith("blob:")) {
    URL.revokeObjectURL(photoPreviewUrl.value)
  }
}

watch(
  () => props.initialInspecao,
  async (value) => {
    revokePreviewUrl()
    form.titulo = value?.titulo ?? ""
    form.descricao = value?.descricao ?? ""
    form.data = toDatetimeLocalValue(value?.data)
    form.localizacao = value?.localizacao ?? ""
    form.latitude = toNumberOrNull(value?.latitude)
    form.longitude = toNumberOrNull(value?.longitude)
    form.photo = null
    photoPreviewUrl.value = ""
    photoInputKey.value += 1

    initOrUpdateMap()

    if (!isEditMode.value && !hasCoordinates.value) {
      await usarLocalizacaoAtual()
    }
  },
  { immediate: true }
)

onMounted(() => {
  initOrUpdateMap()
})

onBeforeUnmount(() => {
  revokePreviewUrl()

  if (mapInstance) {
    mapInstance.remove()
    mapInstance = null
    marker = null
  }
})

function toNumberOrNull(value) {
  if (value === null || value === undefined || value === "") {
    return null
  }

  const parsed = Number(value)
  return Number.isFinite(parsed) ? parsed : null
}

function updateMarkerAndView(latitude, longitude, zoom = 16) {
  if (!mapInstance) {
    return
  }

  const coordinates = [latitude, longitude]

  if (!marker) {
    marker = L.marker(coordinates).addTo(mapInstance)
  } else {
    marker.setLatLng(coordinates)
  }

  mapInstance.setView(coordinates, zoom)
}

function getCoordinateCacheKey(latitude, longitude) {
  return `${latitude.toFixed(4)},${longitude.toFixed(4)}`
}

function readGeoAddressCache() {
  try {
    const raw = localStorage.getItem(GEO_ADDRESS_CACHE_KEY)
    return raw ? JSON.parse(raw) : {}
  } catch {
    return {}
  }
}

function writeGeoAddressCache(cache) {
  localStorage.setItem(GEO_ADDRESS_CACHE_KEY, JSON.stringify(cache))
}

function saveAddressInCache(latitude, longitude, address) {
  if (!address) {
    return
  }

  const cache = readGeoAddressCache()
  cache[getCoordinateCacheKey(latitude, longitude)] = address
  writeGeoAddressCache(cache)
}

function getAddressFromCache(latitude, longitude) {
  const cache = readGeoAddressCache()
  return cache[getCoordinateCacheKey(latitude, longitude)] || ""
}

function latLngToTile(latitude, longitude, zoom) {
  const latRad = (latitude * Math.PI) / 180
  const n = 2 ** zoom
  const x = Math.floor(((longitude + 180) / 360) * n)
  const y = Math.floor(
    ((1 - Math.log(Math.tan(latRad) + 1 / Math.cos(latRad)) / Math.PI) / 2) * n,
  )

  return { x, y }
}

async function cacheMapTilesAroundLocation(latitude, longitude, zoom = 16, radius = 1) {
  if (!navigator.onLine) {
    return
  }

  const center = latLngToTile(latitude, longitude, zoom)
  const requests = []

  for (let dx = -radius; dx <= radius; dx += 1) {
    for (let dy = -radius; dy <= radius; dy += 1) {
      const x = center.x + dx
      const y = center.y + dy
      if (x < 0 || y < 0) {
        continue
      }

      requests.push(fetch(`https://a.tile.openstreetmap.org/${zoom}/${x}/${y}.png`))
    }
  }

  try {
    await Promise.allSettled(requests)
  } catch {
    // Best effort cache warmup; failures are non-blocking.
  }
}

function initOrUpdateMap() {
  if (!mapContainer.value || mapInstance) {
    if (mapInstance && hasCoordinates.value) {
      updateMarkerAndView(form.latitude, form.longitude)
    }

    return
  }

  mapInstance = L.map(mapContainer.value).setView([-23.55052, -46.633308], 12)

  L.tileLayer("https://a.tile.openstreetmap.org/{z}/{x}/{y}.png", {
    attribution: "&copy; OpenStreetMap contributors",
  }).addTo(mapInstance)

  mapInstance.on("click", async (event) => {
    const { lat, lng } = event.latlng
    form.latitude = Number(lat.toFixed(6))
    form.longitude = Number(lng.toFixed(6))
    updateMarkerAndView(form.latitude, form.longitude)

    await preencherEnderecoPorCoordenada(form.latitude, form.longitude)
    await cacheMapTilesAroundLocation(form.latitude, form.longitude)
  })

  if (hasCoordinates.value) {
    updateMarkerAndView(form.latitude, form.longitude)
  }
}

async function preencherEnderecoPorCoordenada(latitude, longitude) {
  const cachedAddress = getAddressFromCache(latitude, longitude)

  if (!navigator.onLine && cachedAddress) {
    form.localizacao = cachedAddress
    locationMessage.value = "Localização recuperada do cache offline."
    return
  }

  if (!navigator.onLine && !cachedAddress) {
    form.localizacao = `${latitude}, ${longitude}`
    locationMessage.value = "Sem internet para endereço detalhado. Usando coordenadas."
    return
  }

  try {
    const response = await fetch(
      `https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat=${latitude}&lon=${longitude}`,
      {
        headers: {
          Accept: "application/json",
        },
      },
    )

    if (!response.ok) {
      throw new Error("Falha ao resolver endereço")
    }

    const payload = await response.json()
    form.localizacao = payload.display_name || `${latitude}, ${longitude}`
    saveAddressInCache(latitude, longitude, form.localizacao)
    locationMessage.value = "Localização preenchida automaticamente."
  } catch {
    if (cachedAddress) {
      form.localizacao = cachedAddress
      locationMessage.value = "Localização recuperada do cache local."
      return
    }

    form.localizacao = `${latitude}, ${longitude}`
    locationMessage.value = "Não foi possível obter endereço detalhado."
  }
}

async function usarLocalizacaoAtual() {
  locationMessage.value = "Capturando localização atual..."

  if (!navigator.geolocation) {
    locationMessage.value = "Geolocalização não suportada neste dispositivo."
    return
  }

  await new Promise((resolve) => {
    navigator.geolocation.getCurrentPosition(
      async (position) => {
        form.latitude = Number(position.coords.latitude.toFixed(6))
        form.longitude = Number(position.coords.longitude.toFixed(6))
        updateMarkerAndView(form.latitude, form.longitude)
        await preencherEnderecoPorCoordenada(form.latitude, form.longitude)
        await cacheMapTilesAroundLocation(form.latitude, form.longitude)
        resolve()
      },
      () => {
        locationMessage.value = "Não foi possível capturar sua localização atual."
        resolve()
      },
      { enableHighAccuracy: true, timeout: 10000 },
    )
  })
}

function onFileChange(event) {
  const [file] = event.target.files
  revokePreviewUrl()
  form.photo = file ?? null

  if (file) {
    photoPreviewUrl.value = URL.createObjectURL(file)
  } else {
    photoPreviewUrl.value = ""
  }
}

function removerFoto() {
  revokePreviewUrl()
  form.photo = null
  photoPreviewUrl.value = ""
  photoInputKey.value += 1
}

function onSubmit() {
  if (!hasCoordinates.value || !form.localizacao.trim()) {
    locationMessage.value = "Preencha a localização antes de salvar."
    return
  }

  emit("finalizar", {
    id: props.initialInspecao?.id ?? null,
    titulo: form.titulo,
    descricao: form.descricao,
    data: form.data,
    photo: form.photo,
    localizacao: form.localizacao,
    latitude: Number(form.latitude),
    longitude: Number(form.longitude),
  })
}
</script>

<template>
  <div class="inspection-form">
    <h2>{{ isEditMode ? "Editar inspeção" : "Nova inspeção" }}</h2>

    <form class="form-page" @submit.prevent="onSubmit">
      <div class="form-item">
        <label for="titulo">Título</label>
        <input id="titulo" type="text" v-model="form.titulo" required />
      </div>

      <div class="form-item">
        <label for="descricao">Descrição</label>
        <textarea id="descricao" v-model="form.descricao" rows="4" required></textarea>
      </div>

      <div class="form-item">
        <label for="data">Data e hora</label>
        <input id="data" type="datetime-local" v-model="form.data" required />
      </div>

      <div class="form-item">
        <label for="photo">Foto</label>
        <input
          :key="photoInputKey"
          id="photo"
          type="file"
          accept="image/*"
          @change="onFileChange"
          :required="isPhotoRequired"
        />
      </div>

      <div class="form-item">
        <label for="localizacao">Localização</label>
        <input id="localizacao" type="text" v-model="form.localizacao" required />
      </div>

      <div class="form-item location-actions">
        <button type="button" class="secondary" @click="usarLocalizacaoAtual">
          {{ isEditMode ? "Atualizar localização" : "Usar minha localização" }}
        </button>
        <a v-if="googleMapsUrl" :href="googleMapsUrl" target="_blank" rel="noreferrer">Abrir no Google Maps</a>
      </div>

      <p v-if="locationMessage" class="location-message">{{ locationMessage }}</p>

      <div class="form-item map-wrapper">
        <div ref="mapContainer" class="map-container"></div>
      </div>

      <div v-if="photoPreview" class="photo-preview-box">
        <img :src="photoPreview" alt="Prévia da foto selecionada" class="photo-preview" />
        <button type="button" class="secondary remove-photo" @click="removerFoto">
          Remover foto
        </button>
      </div>

      <div class="navigation">
        <button type="button" class="secondary" @click="$emit('fechar')">Cancelar</button>
        <button type="submit" :disabled="submitting">
          {{ submitting ? "Salvando..." : "Salvar" }}
        </button>
      </div>
    </form>
  </div>
</template>

<style scoped>
.inspection-form {
  max-width: 600px;
  margin: 20px auto;
  background: #fff;
  padding: 20px;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.15);
}

.form-item { margin-bottom: 15px; }
.form-item label { font-weight: bold; display:block; margin-bottom:5px; }
.form-item input[type="text"],
.form-item input[type="date"],
.form-item textarea,
.form-item input[type="file"] {
  width:100%;
  padding:10px;
  border-radius:6px;
  border:1px solid #ccc;
  box-sizing: border-box;
}

.location-actions {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
}

.location-message {
  color: #555;
  margin-bottom: 12px;
}

.map-wrapper {
  margin-top: 10px;
}

.map-container {
  width: 100%;
  height: 240px;
  border: 1px solid #ddd;
  border-radius: 8px;
}

.photo-preview-box {
  display: flex;
  flex-direction: column;
  gap: 10px;
  margin-bottom: 15px;
  align-items: center;
}

.photo-preview {
  width: 100%;
  max-width: 360px;
  height: 240px;
  object-fit: contain;
  padding: 10px;
  box-sizing: border-box;
  border-radius: 10px;
  border: 1px solid #ddd;
  background: #f8f8f8;
}

.navigation {
  display:flex;
  justify-content:flex-end;
  gap: 10px;
  align-items:center;
  margin-top:20px;
}

.navigation button {
  padding:10px 15px;
  border:none;
  border-radius:6px;
  background:#ea191f;
  color:white;
  font-weight:bold;
  cursor:pointer;
}

.navigation button.secondary {
  background: #666;
}

.remove-photo {
  align-self: center;
}

.navigation button:disabled {
  opacity:0.5;
  cursor:not-allowed;
}

@media (min-width: 768px) {
  .photo-preview {
    max-width: 420px;
    height: 280px;
    padding: 12px;
  }

  .photo-preview-box {
    align-items: center;
  }
}
</style>