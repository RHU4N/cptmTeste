<template>
  <div ref="mapContainer" class="map-container"></div>
</template>

<script setup>
import { onMounted, ref, watch } from 'vue'
import L from 'leaflet'
import 'leaflet/dist/leaflet.css'

const props = defineProps({
  usuarios: { type: Array, required: true },
  adminLoc: { type: Array, default: () => null } // se quisermos setar manual
})

const mapContainer = ref(null)
const map = ref(null)

onMounted(() => {
  // Inicializa o mapa
  map.value = L.map(mapContainer.value).setView([-23.55052, -46.633308], 12)

  L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '&copy; OpenStreetMap contributors'
  }).addTo(map.value)

  // Função para adicionar os pins
  function adicionarPins() {
    if (!map.value) return
    // Primeiro limpa camadas existentes
    map.value.eachLayer(layer => {
      if (layer instanceof L.Marker) map.value.removeLayer(layer)
    })

    // Usuários
    props.usuarios.forEach(u => {
      const icon = L.icon({
        iconUrl: u.status === 'online'
          ? 'https://cdn-icons-png.flaticon.com/512/190/190411.png'
          : 'https://cdn-icons-png.flaticon.com/512/190/190406.png', // offline vermelho
        iconSize: [25, 25],
        iconAnchor: [12, 25],
      })
      L.marker(u.coords, { icon }).addTo(map.value).bindPopup(u.nome)
    })

    // Admin
    if (props.adminLoc) {
      const adminIcon = L.icon({
        iconUrl: 'https://cdn-icons-png.flaticon.com/512/149/149071.png', // admin azul padrão
        iconSize: [25, 25],
        iconAnchor: [12, 25],
      })
      L.marker(props.adminLoc, { icon: adminIcon }).addTo(map.value).bindPopup("Admin")
    }
  }

  // Se quisermos pegar geolocalização do admin
  if (navigator.geolocation) {
    navigator.geolocation.getCurrentPosition(pos => {
      props.adminLoc = [pos.coords.latitude, pos.coords.longitude]
      map.value.setView(props.adminLoc, 13)
      adicionarPins()
    }, err => {
      // se não aceitar geolocalização, usa a loc padrão
      adicionarPins()
    })
  } else {
    adicionarPins()
  }
})

// Atualiza pins se props mudarem
watch(() => props.usuarios, () => {
  if (map.value) map.value.eachLayer(layer => {
    if (layer instanceof L.Marker) map.value.removeLayer(layer)
  })
  // adiciona novamente
  const event = new Event('load')
  window.dispatchEvent(event)
})
</script>

<style scoped>
.map-container {
  width: 100%;
  height: 100%;
  border-radius: 8px;
}
</style>