<template>
  <header class="app-header">
    <img src="../assets/logoH.png" class="logo" alt="Logo" @click="$emit('click-logo')" />
    <h1 class="header-title">{{ title }}</h1>
    <!-- botões de ação (config removido) -->
    <button class="logout-btn" title="Sair" @click="$emit('logout')">
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor"
           stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
        <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4" />
        <polyline points="16 17 21 12 16 7" />
        <line x1="21" y1="12" x2="9" y2="12" />
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
  height:var(--header-logo-height);
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

.logout-btn{
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
  margin-left:8px;
}

.logout-btn:hover{ background: rgba(255,255,255,0.2); }

.logout-btn:hover{ background: rgba(255,255,255,0.2); }