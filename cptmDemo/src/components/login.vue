<template>
  <div class="login-container">
    <!-- Logo -->
    <img :src="logo" class="logo" alt="Logo" />

    <!-- Box de login -->
    <div class="login-box">
      <!-- Usuário -->
      <input 
        v-model="user" 
        type="text" 
        placeholder="Usuário"
        @keyup.enter="focusPassword"
      />

      <!-- Senha -->
      <input 
        v-model="password" 
        type="password" 
        placeholder="Senha"
        ref="passwordInput"
        @keyup.enter="login"
      />

      <!-- Links de suporte -->
      <div class="login-links">
        <a href="#">Esqueci a senha</a>
      </div>

      <!-- Botão Entrar -->
      <button @click="login">Entrar</button>

      <!-- Botão Primeiro Acesso -->
      <button class="first-access" @click="primeiroAcesso">
        Primeiro Acesso
      </button>
    </div>
  </div>
</template>

<script setup>
import { ref } from "vue"
import logo from "../assets/logo.png"

// Emite eventos para App.vue controlar a tela
const emit = defineEmits(["login-success", "first-access"])

const user = ref("")
const password = ref("")
const passwordInput = ref(null)

// Foca no input de senha ao apertar Enter no usuário
function focusPassword() {
  passwordInput.value.focus()
}

// Função login
function login() {
  if(user.value === "admin" && password.value === "admin") {
    emit("login-success","admin")
  }
  else if(user.value === "user" && password.value === "user") {
    emit("login-success","user")
  }
  else{
    alert("Usuário ou senha incorretos")
  }
}

// Botão Primeiro Acesso
function primeiroAcesso() {
  emit("first-access")
}
</script>

<style scoped>
/* Container principal */
.login-container{
  height:100vh;
  display:flex;
  flex-direction:column;
  justify-content:center;
  align-items:center;
  background:#f5f5f5;
  padding:20px;
}

/* Logo */
.logo{
  width:250px;
  margin-bottom:30px;
}

/* Box de login */
.login-box{
  display:flex;
  flex-direction:column;
  width:100%;
  max-width:280px;
  gap:10px;
}

/* Inputs */
.login-box input{
  padding:12px;
  border:1px solid #ccc;
  border-radius:6px;
  font-size:16px;
}

/* Link "Esqueci a senha" */
.login-links{
  display:flex;
  justify-content:flex-end;
  margin-bottom:10px;
}

.login-links a{
  font-size:14px;
  color:#ea191f;
  text-decoration:none;
}

.login-links a:hover{
  text-decoration:underline;
}

/* Botão Entrar */
.login-box button{
  padding:12px;
  border:none;
  border-radius:6px;
  background:#ea191f;
  color:white;
  font-weight:bold;
  cursor:pointer;
  font-size:16px;
}

.login-box button:hover{
  opacity:0.9;
}

/* Botão Primeiro Acesso */
.login-box button.first-access{
  background:#1AEBC7;
  margin-top:5px;
  font-size:14px;
}

.login-box button.first-access:hover{
  opacity:0.8;
}

/* Responsivo para notebook / desktop */
@media (min-width: 768px){
  .logo{
    width:220px;
  }

  .login-box{
    max-width:400px;
  }

  .login-box input{
    padding:14px;
    font-size:18px;
  }

  .login-box button{
    padding:14px;
    font-size:18px;
  }

  .login-links a{
    font-size:16px;
  }

  .login-box button.first-access{
    font-size:16px;
  }
}
</style>