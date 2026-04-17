<template>
  <div class="loader-screen">
    <img class="loader-logo" :src="logo" alt="Logo" />
    <h1 class="loader-title">CPTM X FATEC</h1>

    <!-- loader local para funcionar também sem internet -->
    <div class="loader-animation">
      <div class="spinner"></div>
      <div class="loader-core"></div>
    </div>
    <p class="loader-subtitle">Preparando o aplicativo...</p>
  </div>
</template>

<script>
import { ref, onMounted } from "vue";
import logo from "../assets/logo.png";

export default {
  setup() {
    const loading = ref(true);

    // pré-carregar logo
    const img = new Image();
    img.src = logo;

    onMounted(() => {
      setTimeout(() => {
        loading.value = false;
      }, 3000);
    });

    return { loading, logo };
  },
};
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

/* Container do spinner + GIF */
.loader-animation {
  position: relative;
  width: 120px;
  height: 120px;
}

/* Spinner circular */
.spinner {
  width: 100%;
  height: 100%;
  border: 4px solid #ccc;
  border-top-color: #ea191f;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  position: absolute;
  top: 0;
  left: 0;
}

/* Núcleo visual no centro do loader */
.loader-core {
  width: 56px;
  height: 56px;
  border-radius: 50%;
  background: radial-gradient(circle at 35% 35%, #ffffff 0%, #f6f6f6 35%, #ea191f 100%);
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  box-shadow: 0 10px 20px rgba(234, 25, 31, 0.18);
}

.loader-subtitle {
  margin-top: 18px;
  color: #666;
  font-size: 0.95rem;
  text-align: center;
}

/* animação do spinner */
@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

/* Responsividade */
@media (min-width: 768px) {
  .loader-title {
    font-size: 2.2rem;
  }

  .loader-animation {
    width: 150px;
    height: 150px;
  }

  .loader-core {
    width: 70px;
    height: 70px;
  }

  .loader-subtitle {
    font-size: 1rem;
  }
}
</style>