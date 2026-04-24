<template>
  <div ref="container" class="inspection-location-map"></div>
</template>

<script setup>
import { onMounted, onBeforeUnmount, ref } from "vue"
import L from "leaflet"
import "leaflet/dist/leaflet.css"

const props = defineProps({
  latitude: { type: Number, required: true },
  longitude: { type: Number, required: true },
  zoom: { type: Number, default: 15 },
})

const container = ref(null)
let mapInstance = null
let marker = null

onMounted(() => {
  mapInstance = L.map(container.value, {
    zoomControl: false,
    attributionControl: true,
  }).setView([props.latitude, props.longitude], props.zoom)

  L.tileLayer("https://a.tile.openstreetmap.org/{z}/{x}/{y}.png", {
    attribution: "&copy; OpenStreetMap contributors",
  }).addTo(mapInstance)

  marker = L.marker([props.latitude, props.longitude]).addTo(mapInstance)
})

onBeforeUnmount(() => {
  if (mapInstance) {
    mapInstance.remove()
    mapInstance = null
    marker = null
  }
})
</script>

<style scoped>
.inspection-location-map {
  width: 100%;
  max-width: 420px;
  height: 220px;
  border-radius: 8px;
  border: 1px solid #ddd;
  overflow: hidden;
}
</style>
