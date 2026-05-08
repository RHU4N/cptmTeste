<template>
  <div class="admin-container">
    <!-- Header -->
    <AppHeader 
      :title="tituloHeader" 
      @click-logo="voltarTelaInicial"
      @logout="handleLogout"
    />

    <main class="admin-content">
      <p v-if="erro" class="erro-api">{{ erro }}</p>

      <!-- Tela principal -->
      <div v-if="!abrindoFormulario && !abrindoHistorico && !abrindoAdminUsers">
        <p class="intro-text">Escolha uma das funcionalidades abaixo ou veja as inspeções recentes:</p>

        <!-- Botões -->
        <div class="button-group">
          <AppButton :icon="inspecaoIcon" @click="abrirFormularioNovaInspecao">Inspeção</AppButton>
          <AppButton :icon="historicoIcon" @click="abrindoHistorico = true">Histórico</AppButton>
          <AppButton :icon="adminIcon" @click="abrindoAdminUsers = true">Admin</AppButton>
        </div>

        <!-- Lista de inspeções -->
        <InspectionList
          :inspections="inspecoes"
          :loading="carregando"
          :show-user="true"
          @create="abrirFormularioNovaInspecao"
          @edit="editarInspecao"
          @delete="apagarInspecao"
        />
      </div>

      <!-- Formulário de inspeção -->
      <InspectionForm
        v-if="abrindoFormulario"
        :initial-inspecao="inspecaoEmEdicao"
        :submitting="salvando"
        @finalizar="finalizarFormulario"
        @fechar="fecharFormulario"
      />

      <!-- Tela de histórico -->
      <Historico
        v-if="abrindoHistorico"
        :inspections="inspecoes"
        @fechar="abrindoHistorico = false"
      />

      <!-- Tela de AdminUsers -->
      <AdminUsers
        v-if="abrindoAdminUsers"
        :usuarios="usuarios"
        @fechar="abrindoAdminUsers = false"
      />
    </main>
  </div>
</template>

<script setup>
import { reactive, ref, computed, onMounted, onUnmounted } from "vue"
import AppHeader from "../components/AppHeader.vue"
import AppButton from "../components/AppButton.vue"
import InspectionList from "../components/InspectionList.vue"
import InspectionForm from "../components/InspectionForm.vue"
import Historico from "./Historico.vue"
import AdminUsers from "./AdminUsers.vue"
import { clearAuthSession } from "../services/authApi"

const emit = defineEmits(["logout"])

function handleLogout(){
  clearAuthSession()
  emit("logout")
}
import {
  listInspecoes,
  createInspecao,
  updateInspecao,
  deleteInspecao,
} from "../services/inspecaoApi"

// Ícones
import inspecaoIcon from "../assets/inspecao.png"
import historicoIcon from "../assets/historico.png"
import adminIcon from "../assets/admin.jpg"

// Flags
const abrindoFormulario = ref(false)
const abrindoHistorico = ref(false)
const abrindoAdminUsers = ref(false)
const carregando = ref(false)
const salvando = ref(false)
const erro = ref("")
const inspecaoEmEdicao = ref(null)

const inspecoes = ref([])

function mapInspecaoApi(item) {
  return {
    ...item,
    usuario: item.usuarioUsername || item.usuario || "-",
  }
}

async function carregarInspecoes() {
  carregando.value = true
  erro.value = ""

  try {
    const data = await listInspecoes()
    inspecoes.value = data.map(mapInspecaoApi)
  } catch (error) {
    erro.value = error.message
  } finally {
    carregando.value = false
  }
}

onMounted(() => {
  carregarInspecoes()
  window.addEventListener("online", carregarInspecoes)
})

onUnmounted(() => {
  window.removeEventListener("online", carregarInspecoes)
})

// Lista de usuários
const usuarios = reactive([
  { id:1, nome:'admin', status:'online', coords:[-23.55052, -46.633308] },
  { id:2, nome:'user1', status:'online', coords:[-23.55152, -46.634308] },
  { id:3, nome:'user2', status:'offline', coords:[-23.55252, -46.635308] },
  { id:4, nome:'user3', status:'online', coords:[-23.55352, -46.636308] },
])

// ---------------- Funções ----------------
function abrirConfigs() { alert("Abrir configurações") }

function abrirFormularioNovaInspecao() {
  inspecaoEmEdicao.value = null
  abrindoFormulario.value = true
}

function editarInspecao(inspecao) {
  inspecaoEmEdicao.value = inspecao
  abrindoFormulario.value = true
}

async function apagarInspecao(inspecao){
  if(!confirm("Tem certeza que deseja apagar esta inspeção?")){
    return
  }

  erro.value = ""
  try {
    await deleteInspecao(inspecao.id)
    await carregarInspecoes()
  } catch (error) {
    erro.value = error.message
  }
}

async function finalizarFormulario(payload){
  salvando.value = true
  erro.value = ""

  try {
    if (payload.id) {
      await updateInspecao(payload.id, payload)
    } else {
      await createInspecao(payload)
    }

    await carregarInspecoes()
    fecharFormulario()
  } catch (error) {
    erro.value = error.message
  } finally {
    salvando.value = false
  }
}

function fecharFormulario() {
  abrindoFormulario.value = false
  inspecaoEmEdicao.value = null
}

// Computed
const tituloHeader = computed(()=>{
  if(abrindoFormulario.value) return "Formulário de Inspeção"
  if(abrindoHistorico.value) return "Histórico de Inspeções"
  if(abrindoAdminUsers.value) return "Gerenciamento de Usuários"
  return "Bem-vindo, Admin"
})

// Voltar à tela principal
function voltarTelaInicial(){
  fecharFormulario()
  abrindoHistorico.value=false
  abrindoAdminUsers.value=false
}
</script>

<style scoped>
.admin-container{ height:100vh; display:flex; flex-direction:column; background:#f5f5f5; }
.admin-content{ flex:1; padding:20px; }
.erro-api{
  color:#b00020;
  background:#ffe7e8;
  border:1px solid #ffb2b5;
  border-radius:8px;
  padding:10px;
  margin-bottom:12px;
}
.intro-text{ font-size:1rem; color:#333; margin-bottom:15px; text-align:center; }
.button-group{ display:flex; flex-wrap:wrap; gap:15px; margin-bottom:20px; justify-content:center; }

@media(min-width:768px){
  .admin-content{ padding:30px; }
  .button-group{ gap:25px; }
  .intro-text{ font-size:1.2rem; }
}
</style>