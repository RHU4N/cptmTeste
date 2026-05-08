<template>
  <div class="first-access-container">
    <img :src="logo" class="logo" alt="Logo" />

    <h2>Primeiro Acesso</h2>
    <p>Crie seu usuário para acessar o sistema.</p>

    <input v-model="username" type="text" placeholder="Usuário" />
    <input v-model="password" type="password" placeholder="Senha" @keyup.enter="registrar" />

    <input v-model="activationCode" type="text" placeholder="Código de ativação" @keyup.enter="registrar" />

    <button :disabled="loading" @click="registrar">{{ loading ? "Ativando..." : "Ativar" }}</button>

    <p class="support">
      Não recebeu o código? <a href="#">Contate o suporte</a>
    </p>
  </div>
</template>

<script setup>
import { ref } from "vue"
import logo from "../assets/logo.png"
import { register as registerApi } from "../services/authApi"

const emit = defineEmits(["activation-success"])

const username = ref("")
const password = ref("")
const activationCode = ref("")
const loading = ref(false)

async function registrar() {
  if (!username.value.trim() || !password.value.trim() || !activationCode.value.trim()) {
    alert("Preencha usuário, senha e código de ativação")
    return
  }

  loading.value = true
  try {
    const session = await registerApi(username.value.trim(), password.value, activationCode.value.trim())
    alert("Conta ativada com sucesso!")
    emit("activation-success", session.role)
  } catch (error) {
    alert(error.message || "Não foi possível ativar conta")
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.first-access-container{
  height:100vh;
  display:flex;
  flex-direction:column;
  justify-content:center;
  align-items:center;
  background:#f5f5f5;
  padding:20px;
}

.logo{
  width:var(--brand-logo-size);
  max-width:100%;
  margin-bottom:20px;
}

input{
  padding:12px;
  border:1px solid #ccc;
  border-radius:6px;
  width:100%;
  max-width:280px;
  margin-bottom:10px;
}

button{
  padding:12px;
  border:none;
  border-radius:6px;
  background:#ea191f;
  color:white;
  font-weight:bold;
  cursor:pointer;
  width:100%;
  max-width:280px;
  margin-bottom:10px;
}

button:hover{
  opacity:0.9;
}

.support a{
  color:#ea191f;
  text-decoration:none;
}

.support a:hover{
  text-decoration:underline;
}

h2{
  margin-bottom:10px;
  color:#333;
  text-align:center;
}

p{
  text-align:center;
  margin-bottom:15px;
}

@media (min-width:768px){
  input, button{
    padding:14px;
    font-size:18px;
  }
}
</style>