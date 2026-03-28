<template>
  <div class="admin-users-container">

    <main class="admin-users-content">
      <!-- Formulário para criar usuário -->
      <div class="create-user-box">
        <h3>Criar Novo Usuário</h3>
        <input v-model="novoUsuarioNome" type="text" placeholder="Nome do usuário" />
        <select v-model="novoUsuarioStatus">
          <option value="user">User</option>
          <option value="admin">Admin</option>
        </select>
        <button @click="criarUsuario">Criar Usuário</button>
      </div>

      <!-- Lista de usuários existentes -->
      <div class="users-list-box">
        <h3>Usuários Cadastrados</h3>
        <table>
          <thead>
            <tr>
              <th>Nome</th>
              <th>Status</th>
              <th>Ações</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="u in usuarios" :key="u.id">
              <td>{{ u.nome }}</td>
              <td>
                <select v-model="u.permissao">
                  <option value="user">User</option>
                  <option value="admin">Admin</option>
                </select>
              </td>
              <td>
                <button @click="removerUsuario(u)">Remover</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </main>
  </div>
</template>

<script setup>
import { reactive, ref } from 'vue'
import AppHeader from "../components/AppHeader.vue"

// Lista de usuários (poderíamos receber do Admin.vue)
const usuarios = reactive([
  { id:1, nome:'user1', permissao:'user' },
  { id:2, nome:'user2', permissao:'user' },
  { id:3, nome:'user3', permissao:'admin' },
])

// Campos do formulário
const novoUsuarioNome = ref('')
const novoUsuarioPer = ref('user')

// Criar usuário
function criarUsuario(){
  if(!novoUsuarioNome.value.trim()){
    alert("Digite um nome válido")
    return
  }
  const id = usuarios.length ? Math.max(...usuarios.map(u=>u.id))+1 : 1
  usuarios.push({
    id,
    nome: novoUsuarioNome.value,
    status: novoUsuarioPer.value
  })
  novoUsuarioNome.value = ''
  novoUsuarioPer.value = 'user'
}

// Remover usuário
function removerUsuario(usuario){
  if(confirm(`Deseja remover ${usuario.nome}?`)){
    const index = usuarios.findIndex(u => u.id === usuario.id)
    if(index !== -1) usuarios.splice(index,1)
  }
}

// Voltar para a tela inicial do admin
function voltarTelaInicial(){
  // aqui você pode emitir evento ou alterar a flag no Admin.vue
  alert("Voltar à tela inicial do Admin")
}
</script>

<style scoped>
.admin-users-container{
  display:flex;
  flex-direction:column;
  height:100vh;
  background:#f5f5f5;
}

.admin-users-content{
  flex:1;
  padding:20px;
  display:flex;
  flex-direction:column;
  gap:20px;
}

.create-user-box{
  display:flex;
  flex-wrap:wrap;
  gap:10px;
  align-items:center;
  background:#fff;
  padding:15px;
  border-radius:8px;
  box-shadow:0 1px 4px rgba(0,0,0,0.1);
}

.create-user-box input,
.create-user-box select{
  padding:10px;
  font-size:16px;
  border-radius:6px;
  border:1px solid #ccc;
}

.create-user-box button{
  padding:10px 15px;
  border:none;
  border-radius:6px;
  background:#ea191f;
  color:white;
  cursor:pointer;
  font-weight:bold;
}

.create-user-box button:hover{
  opacity:0.9;
}

.users-list-box{
  background:#fff;
  padding:15px;
  border-radius:8px;
  box-shadow:0 1px 4px rgba(0,0,0,0.1);
  overflow-x:auto;
}

.users-list-box table{
  width:100%;
  border-collapse:collapse;
}

.users-list-box th,
.users-list-box td{
  padding:10px;
  text-align:left;
  border-bottom:1px solid #ddd;
}

.users-list-box select{
  padding:5px;
  border-radius:6px;
  border:1px solid #ccc;
}

.users-list-box button{
  padding:5px 10px;
  border:none;
  border-radius:6px;
  background:#ea191f;
  color:white;
  cursor:pointer;
  font-weight:bold;
}

.users-list-box button:hover{
  opacity:0.9;
}

@media(min-width:768px){
  .create-user-box input,
  .create-user-box select{
    font-size:18px;
    padding:12px;
  }

  .create-user-box button{
    padding:12px 18px;
  }
}
</style>