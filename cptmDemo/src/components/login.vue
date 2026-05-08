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
      <button :disabled="loggingIn" @click="login">{{ loggingIn ? "Entrando..." : "Entrar" }}</button>
    </div>
  </div>
</template>

<script setup>
import { ref } from "vue"
import logo from "../assets/logo.png"
import { login as loginApi } from "../services/authApi"

// Emite eventos para App.vue controlar a tela
const emit = defineEmits(["login-success"])

const user = ref("")
const password = ref("")
const passwordInput = ref(null)
const loggingIn = ref(false)

// Foca no input de senha ao apertar Enter no usuário
function focusPassword() {
  passwordInput.value.focus()
}

// Função login
async function login() {
  if (!user.value.trim() || !password.value.trim()) {
    alert("Informe usuário e senha")
    return
  }

  loggingIn.value = true
  try {
    const session = await loginApi(user.value.trim(), password.value)
    emit("login-success", session.role)
  } catch (error) {
    alert(error.message || "Usuário ou senha incorretos")
  } finally {
    loggingIn.value = false
  }
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
  width:var(--brand-logo-size);
  max-width:100%;
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

/* Responsivo para notebook / desktop */
@media (min-width: 768px){
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
}
</style>
