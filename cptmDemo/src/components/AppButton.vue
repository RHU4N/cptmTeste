<template>
  <button class="app-button" @click="$emit('click')">
    <!-- ícone no topo, opcional -->
    <img v-if="icon" :src="icon" alt="ícone" class="button-icon" />

    <!-- conteúdo do slot (texto) abaixo do ícone -->
    <span class="button-text">
      <slot></slot>
    </span>
  </button>
</template>

<script setup>
const props = defineProps({
  icon: { type: String, default: null } // permite passar ícone opcional
})
</script>

<style scoped>
.app-button{
  display:flex;
  flex-direction:column;
  align-items:center;
  justify-content:center;
  gap:10px;
  padding:20px;
  background:#ea191f;
  color:white;
  border:none;
  border-radius:12px;
  font-weight:bold;
  font-size:16px;
  cursor:pointer;
  transition: transform 0.2s, background 0.2s;
  width:120px;
  height:140px;
}

.app-button:hover{
  background:#d0181c;
  transform: translateY(-3px);
}

.button-icon{
  width:50px;
  height:50px;
  object-fit:contain;
}

.button-text{
  text-align:center;
  font-size:14px;
}

/* Responsivo desktop */
@media (min-width:768px){
  .app-button{
    width:160px;
    height:180px;
    padding:25px;
    font-size:16px;
  }

  .button-icon{
    width:70px;
    height:70px;
  }

  .button-text{
    font-size:16px;
  }
}
</style>