<template>
  <div class="admin-users-container">

    <main class="admin-users-content">
      <!-- Formulário para criar usuário -->
      <div class="create-user-box">
        <h3>Criar Novo Usuário</h3>
        <input v-model="novoUsuarioNome" type="text" placeholder="Nome do usuário" />
        <input v-model="novoUsuarioSenha" type="password" placeholder="Senha" />
        <select v-model="novoUsuarioRole">
          <option value="user">User</option>
          <option value="admin">Admin</option>
        </select>
        <button @click="criarUsuario">Criar Usuário</button>
      </div>

      <!-- Lista de usuários existentes -->
      <div class="users-list-box">
        <h3>Usuários Cadastrados</h3>
          <div class="users-filters">
            <input v-model="filtroSearch" type="text" placeholder="Pesquisar usuário" />
            <select v-model="filtroRole">
              <option value="">Todos</option>
              <option value="user">User</option>
              <option value="admin">Admin</option>
            </select>
            <button @click="carregarUsuarios">Filtrar</button>
            <button @click="limparFiltro">Limpar</button>
          </div>
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
                <select v-model="u.role" @change="atualizarRole(u)">
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
import { reactive, ref, onMounted } from 'vue'
import * as userApi from '../services/userApi'

const usuarios = reactive([])

const novoUsuarioNome = ref('')
const novoUsuarioSenha = ref('')
const novoUsuarioRole = ref('user')
const filtroRole = ref('')
const filtroSearch = ref('')

async function carregarUsuarios(){
  try{
    const data = await userApi.getUsers({ role: filtroRole.value, search: filtroSearch.value })
    usuarios.splice(0, usuarios.length, ...data.map(u => ({ id: u.id, nome: u.username, role: u.role })))
  }catch(err){
    console.error(err)
    alert('Falha ao buscar usuários: ' + (err.message || err))
  }
}

function limparFiltro(){
  filtroRole.value = ''
  filtroSearch.value = ''
  carregarUsuarios()
}

onMounted(() => {
  carregarUsuarios()
})

async function criarUsuario(){
  if(!novoUsuarioNome.value.trim() || !novoUsuarioSenha.value){
    alert('Nome e senha são obrigatórios')
    return
  }

  try{
    await userApi.createUser(novoUsuarioNome.value.trim(), novoUsuarioSenha.value, novoUsuarioRole.value)
    novoUsuarioNome.value = ''
    novoUsuarioSenha.value = ''
    novoUsuarioRole.value = 'user'
    await carregarUsuarios()
    alert('Usuário criado com sucesso')
  }catch(err){
    console.error(err)
    alert('Falha ao criar usuário: ' + (err.message || err))
  }
}

async function atualizarRole(usuario){
  try{
    await userApi.updateUserRole(usuario.id, usuario.role)
    alert('Permissão atualizada')
  }catch(err){
    console.error(err)
    alert('Falha ao atualizar permissão: ' + (err.message || err))
    await carregarUsuarios()
  }
}

async function removerUsuario(usuario){
  if(!confirm(`Deseja remover ${usuario.nome}?`)) return
  try{
    await userApi.deleteUser(usuario.id)
    await carregarUsuarios()
    alert('Usuário removido')
  }catch(err){
    console.error(err)
    alert('Falha ao remover usuário: ' + (err.message || err))
  }
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