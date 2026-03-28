<template>
  <div class="first-access-container">
    <img :src="logo" class="logo" alt="Logo" />

    <h2>Ativação de Conta</h2>
    <p>Insira o código de ativação fornecido pelo administrador para liberar sua conta.</p>

    <input v-model="activationCode" type="text" placeholder="Código de ativação" @keyup.enter="ativarConta" />

    <button @click="ativarConta" >Ativar</button>

    <p class="support">
      Não recebeu o código? <a href="#">Contate o suporte</a>
    </p>
  </div>
</template>

<script setup>
import { ref } from "vue"
import logo from "../assets/logo.png"

const emit = defineEmits(["activation-success"])

const activationCode = ref("")

function ativarConta() {
  if(activationCode.value === "1234") { // Exemplo de código válido
    alert("Conta ativada com sucesso!")
    emit("activation-success") // App.vue pode mudar para tela User/Admin
  } else {
    alert("Código inválido")
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
  width:200px;
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
  .logo{
    width:250px;
  }

  input, button{
    padding:14px;
    font-size:18px;
  }
}
</style>