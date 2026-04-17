<script setup>
import { computed, onMounted, onUnmounted, ref } from "vue"
import Loader from "./components/loading.vue"
import Login from "./components/login.vue"
import User from "./components/user.vue"
import Admin from "./components/admin.vue"
import PrimeiroAcesso from "./components/firstAccess.vue"
import {
  getPendingInspecaoCount,
  listenPendingInspecoesChange,
} from "./services/inspecaoApi"

const loading = ref(true)
const currentScreen = ref("login") // login | user | admin | first-access
const isOnline = ref(navigator.onLine)
const pendingInspecoes = ref(getPendingInspecaoCount())
let stopListeningPending = null

function updateOnlineStatus() {
  isOnline.value = navigator.onLine
}

// Loader inicial 3 segundos
onMounted(() => {
  window.addEventListener("online", updateOnlineStatus)
  window.addEventListener("offline", updateOnlineStatus)
  stopListeningPending = listenPendingInspecoesChange(() => {
    pendingInspecoes.value = getPendingInspecaoCount()
  })

  setTimeout(() => {
    loading.value = false
  }, 3000)
})

onUnmounted(() => {
  window.removeEventListener("online", updateOnlineStatus)
  window.removeEventListener("offline", updateOnlineStatus)

  if (stopListeningPending) {
    stopListeningPending()
  }
})

// Função que muda de tela
function changeScreen(screen) {
  currentScreen.value = screen
}

const offlineMessage = computed(() => {
  const baseMessage = isOnline.value
    ? ""
    : "Você está offline. O app continua aberto, mas ações que dependem da API podem usar dados em cache ou falhar até a conexão voltar."

  if (!pendingInspecoes.value) {
    return baseMessage
  }

  const pendingMessage = `${pendingInspecoes.value} alteração(ões) aguardando sincronização.`

  return baseMessage ? `${baseMessage} ${pendingMessage}` : pendingMessage
})
</script>

<template>
  <div v-if="!loading && !isOnline" class="offline-banner" role="status" aria-live="polite">
    {{ offlineMessage }}
  </div>

  <!-- Loader -->
  <Loader v-if="loading" />

  <!-- Login -->
  <Login 
    v-else-if="currentScreen === 'login'" 
    @login-success="changeScreen" 
    @first-access="() => changeScreen('first-access')"
  />

  <!-- Primeiro Acesso -->
  <PrimeiroAcesso 
    v-else-if="currentScreen === 'first-access'" 
    @activation-success="() => changeScreen('user')"
  />

  <!-- Telas do usuário e admin -->
  <User v-else-if="currentScreen === 'user'" />
  <Admin v-else-if="currentScreen === 'admin'" />
</template>