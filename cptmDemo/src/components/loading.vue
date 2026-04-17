<template>
  <div class="loader-screen">
    <img class="loader-logo" :src="logo" alt="Logo" />
    <h1 class="loader-title">CPTM X FATEC</h1>

    <div class="loader-animation" aria-label="Carregando">
      <div class="spinner"></div>
      <img class="loader-gif" :src="trainLoad" alt="Animação de trem carregando" />
    </div>

    <p class="loader-subtitle">Preparando o aplicativo...</p>
  </div>
</template>

<script>
import { ref, onMounted } from "vue"
import logo from "../assets/logo.png"
import trainLoad from "../assets/trainLoad.gif"

export default {
  setup() {
    const loading = ref(true)

    const img = new Image()
    img.src = logo

    onMounted(() => {
      setTimeout(() => {
        loading.value = false
      }, 3000)
    })

    return { loading, logo, trainLoad }
  },
}
</script>

<style scoped>
.loader-screen {
  position: fixed;
  inset: 0;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  background-color: #fff;
  z-index: 9999;
  padding: 20px;
}

.loader-logo {
  width: var(--brand-logo-size);
  max-width: 100%;
  height: auto;
  margin-bottom: 15px;
}

.loader-title {
  font-size: 2rem;
  margin-bottom: 20px;
  color: #333;
  text-align: center;
}

.loader-animation {
  position: relative;
  width: 160px;
  height: 160px;
  display: grid;
  place-items: center;
}

.spinner {
  position: absolute;
  inset: 0;
  width: 100%;
  height: 100%;
  border: 4px solid #e3e3e3;
  border-top-color: #ea191f;
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

.loader-gif {
  width: 92px;
  height: 92px;
  object-fit: contain;
  position: relative;
  z-index: 1;
}

.loader-subtitle {
  margin-top: 18px;
  color: #666;
  font-size: 0.95rem;
  text-align: center;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

@media (min-width: 768px) {
  .loader-title {
    font-size: 2.2rem;
  }

  .loader-animation {
    width: 190px;
    height: 190px;
  }

  .loader-gif {
    width: 110px;
    height: 110px;
  }

  .loader-subtitle {
    font-size: 1rem;
  }
}
</style>