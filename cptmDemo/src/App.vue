<script setup>
import { ref, onMounted } from "vue"
import Loader from "./components/loading.vue"
import Login from "./components/login.vue"
import User from "./components/user.vue"
import Admin from "./components/admin.vue"
import PrimeiroAcesso from "./components/firstAccess.vue"

const loading = ref(true)
const currentScreen = ref("login") // login | user | admin | first-access

// Loader inicial 3 segundos
onMounted(() => {
  setTimeout(() => {
    loading.value = false
  }, 3000)
})

// Função que muda de tela
function changeScreen(screen) {
  currentScreen.value = screen
}
</script>

<template>
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