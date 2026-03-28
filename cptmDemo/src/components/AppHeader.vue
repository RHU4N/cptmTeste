<template>
  <header class="app-header">
    <img src="../assets/logoH.png" class="logo" alt="Logo" @click="$emit('click-logo')" />
    <h1 class="header-title">{{ title }}</h1>
    <button class="config-btn" @click="$emit('open-configs')">
      <!-- SVG minimalista -->
      <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor"
           stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
        <circle cx="12" cy="12" r="3"></circle>
        <path d="M19.4 15a1.65 1.65 0 0 0 .33 1.82l0 0a2 2 0 1 1-2.83 2.83l0 0a1.65 1.65 0 0 0-1.82-.33 1.65 1.65 0 0 0-1 1.51V21a2 2 0 1 1-4 0v0a1.65 1.65 0 0 0-1-1.51 1.65 1.65 0 0 0-1.82.33l0 0a2 2 0 1 1-2.83-2.83l0 0a1.65 1.65 0 0 0 .33-1.82 1.65 1.65 0 0 0-1.51-1H3a2 2 0 1 1 0-4h0a1.65 1.65 0 0 0 1.51-1 1.65 1.65 0 0 0-.33-1.82l0 0a2 2 0 1 1 2.83-2.83l0 0a1.65 1.65 0 0 0 1.82.33H9a1.65 1.65 0 0 0 1-1.51V3a2 2 0 1 1 4 0v0a1.65 1.65 0 0 0 1 1.51 1.65 1.65 0 0 0 1.82-.33l0 0a2 2 0 1 1 2.83 2.83l0 0a1.65 1.65 0 0 0-.33 1.82V9a1.65 1.65 0 0 0 1.51 1H21a2 2 0 1 1 0 4h0a1.65 1.65 0 0 0-1.51 1z"/>
      </svg>
    </button>
  </header>
</template>

<script setup>
import logo from "../assets/logoH.png"
const props = defineProps({ title: { type: String, default: "Bem-vindo" } })
</script>

<style scoped>
.app-header{
  display:flex;
  align-items:center;
  justify-content:space-between;
  position:relative;
  padding:15px 20px;
  background:#ea191f;
  color:white;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.app-header .logo{
  height:40px;
  cursor:pointer;
}

.app-header .header-title{
  position:absolute;
  left:50%;
  transform:translateX(-50%);
  font-size:18px;
  font-weight:bold;
}

.config-btn{
  background:none;
  border:none;
  color:white;
  cursor:pointer;
  display:flex;
  align-items:center;
  justify-content:center;
  padding:5px;
  border-radius:4px;
  transition: background 0.2s;
}

.config-btn:hover{
  background: rgba(255,255,255,0.2);
}

@media (min-width:768px){
  .app-header{
    padding:25px 40px;
  }
  .app-header .logo{ height:60px; }
  .app-header .header-title{ font-size:24px; }
}
</style>