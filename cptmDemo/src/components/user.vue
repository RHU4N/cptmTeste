<template>
  <div class="user-container">
<AppHeader 
  :title="tituloHeader" 
  @click-logo="voltarTelaInicial"
  @logout="handleLogout"
/> 

    <main class="user-content">
      <p v-if="erro" class="erro-api">{{ erro }}</p>

      <div v-if="!abrindoFormulario && !abrindoHistorico">
        <p class="intro-text">Escolha uma das funcionalidades abaixo ou veja suas inspeções recentes:</p>

        <div class="button-group">
          <AppButton :icon="inspecaoIcon" @click="abrirFormularioNovaInspecao">Inspeção</AppButton>
          <AppButton :icon="historicoIcon" @click="abrindoHistorico = true">Histórico</AppButton>
        </div>

        <InspectionList
          :inspections="inspecoes"
          :loading="carregando"
          :show-user="false"
          @create="abrirFormularioNovaInspecao"
          @edit="editarInspecao"
          @delete="apagarInspecao"
        />
      </div>

      <InspectionForm
        v-if="abrindoFormulario"
        :initial-inspecao="inspecaoEmEdicao"
        :submitting="salvando"
        @finalizar="finalizarFormulario"
        @fechar="abrindoFormulario = false"
      />

      <Historico
        v-if="abrindoHistorico"
        :inspections="inspecoes"
        @fechar="abrindoHistorico = false"
      />
    </main>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from "vue"
import AppHeader from "../components/AppHeader.vue"
import AppButton from "../components/AppButton.vue"
import InspectionList from "../components/InspectionList.vue"
import InspectionForm from "../components/InspectionForm.vue"
import Historico from "./Historico.vue"
import {
  listInspecoes,
  createInspecao,
  updateInspecao,
  deleteInspecao,
} from "../services/inspecaoApi"

import inspecaoIcon from "../assets/inspecao.png"
import historicoIcon from "../assets/historico.png"
import { clearAuthSession } from "../services/authApi"

const emit = defineEmits(["logout"])

function handleLogout(){
  clearAuthSession()
  emit("logout")
}

const abrindoFormulario = ref(false)
const abrindoHistorico = ref(false)
const inspecoes = ref([])
const carregando = ref(false)
const salvando = ref(false)
const erro = ref("")
const inspecaoEmEdicao = ref(null)

function mapInspecaoApi(item) {
  return {
    ...item,
    descricao: item.descricao || "Sem descrição",
    photoUrl: item.photoBase64 ? `data:image/jpeg;base64,${item.photoBase64}` : null,
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

function voltarTelaInicial() {
  abrindoFormulario.value = false
  abrindoHistorico.value = false
  inspecaoEmEdicao.value = null
}

function abrirConfigs() { alert("Abrir configurações") }

function abrirFormularioNovaInspecao() {
  inspecaoEmEdicao.value = null
  abrindoFormulario.value = true
}

function editarInspecao(inspecao) {
  inspecaoEmEdicao.value = inspecao
  abrindoFormulario.value = true
}

async function apagarInspecao(inspecao) {
  if (!confirm("Tem certeza que deseja apagar esta inspeção?")) {
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

async function finalizarFormulario(payload) {
  salvando.value = true
  erro.value = ""

  try {
    if (payload.id) {
      await updateInspecao(payload.id, payload)
    } else {
      await createInspecao(payload)
    }

    await carregarInspecoes()
    abrindoFormulario.value = false
    inspecaoEmEdicao.value = null
  } catch (error) {
    erro.value = error.message
  } finally {
    salvando.value = false
  }
}

const tituloHeader = computed(() => {
  if(abrindoFormulario.value) return "Formulário de Inspeção"
  if(abrindoHistorico.value) return "Histórico de Inspeções"
  return "Bem-vindo, Inspetor"
})
</script>

<style scoped>
.user-container{
  height:100vh;
  display:flex;
  flex-direction:column;
  background:#f5f5f5;
}

.user-content{
  flex:1;
  padding:20px;
}

.erro-api{
  color:#b00020;
  background:#ffe7e8;
  border:1px solid #ffb2b5;
  border-radius:8px;
  padding:10px;
  margin-bottom:12px;
}

.intro-text{
  font-size:1rem;
  color:#333;
  margin-bottom:15px;
  text-align:center;
}

.button-group{
  display:flex;
  flex-wrap:wrap;
  gap:15px;
  margin-bottom:20px;
  justify-content:center;
}

@media(min-width:768px){
  .user-content{
    padding:30px;
  }
  .button-group{
    gap:25px;
  }
  .intro-text{
    font-size:1.2rem;
  }
}
</style>