<script setup>
import { computed, onMounted, onUnmounted, ref } from "vue"
import Loader from "./components/loading.vue"
import Login from "./components/login.vue"
import User from "./components/user.vue"
import Admin from "./components/admin.vue"
import { getAuthSession } from "./services/authApi"
import { clearAuthSession } from "./services/authApi"
import {
  getPendingInspecaoCount,
  listenPendingInspecoesChange,
} from "./services/inspecaoApi"

const loading = ref(true)
const currentScreen = ref("login") // login | user | admin
const isOnline = ref(navigator.onLine)
const pendingInspecoes = ref(getPendingInspecaoCount())
let stopListeningPending = null

function updateOnlineStatus() {
  isOnline.value = navigator.onLine
}

// Loader inicial 3 segundos
onMounted(() => {
  const session = getAuthSession()
  if (session?.role === "admin") {
    currentScreen.value = "admin"
  } else if (session?.role === "user") {
    currentScreen.value = "user"
  }

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

function onLogout(){
  clearAuthSession()
  currentScreen.value = "login"
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
  />

  <!-- Telas do usuário e admin -->
  <User v-else-if="currentScreen === 'user'" @logout="onLogout" />
  <Admin v-else-if="currentScreen === 'admin'" @logout="onLogout" />
</template>