<template>
  <div class="loader-screen">
    <img class="loader-logo" :src="logo" alt="Logo" />
    <h1 class="loader-title">CPTM X FATEC</h1>

    <!-- container para spinner + GIF -->
    <div class="loader-animation">
      <div class="spinner"></div>
      <img 
        class="loader-gif" 
        src="https://media0.giphy.com/media/v1.Y2lkPTc5MGI3NjExd3o1MW82M2F4Y2xmZm90eHhlZDlycmpuOWxoZzN5N3IwYTEyamhyOSZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9cw/Uei7b3jIpgKbfV7PMp/giphy.gif" 
        alt="Animação Loading"
      />
    </div>
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
  width: 200px;
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

/* GIF animado por cima do spinner */
.loader-gif {
  width: 60px;        /* metade do spinner para ficar centralizado */
  height: auto;
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
}

/* animação do spinner */
@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

/* Responsividade */
@media (min-width: 768px) {
  .loader-logo {
    width: 250px;
  }

  .loader-title {
    font-size: 2.2rem;
  }

  .loader-animation {
    width: 150px;
    height: 150px;
  }

  .loader-gif {
    width: 80px;
  }
}
</style>