<template>
  <div class="historico-container">

    <!-- Conteúdo -->
    <main class="historico-content">

      <!-- Filtros -->
      <div class="filtros">
        <label>
          Data (início):
          <input type="date" v-model="filtroDataInicio" />
        </label>

        <label>
          Data (fim):
          <input type="date" v-model="filtroDataFim" />
        </label>

        <label>
          Título:
          <input type="text" v-model="filtroTitulo" placeholder="Buscar título..." />
        </label>
      </div>

      <!-- Lista de inspeções filtradas -->
      <InspectionList
        :inspections="inspecoesFiltradas"
        :show-user="false"
        @create="$emit('fechar')"
        @edit="editarInspecao"
        @delete="apagarInspecao"
      />
    </main>
  </div>
</template>

<script setup>
import { ref, computed } from "vue"
import InspectionList from "../components/InspectionList.vue"

const props = defineProps({
  inspections: {
    type: Array,
    default: () => [],
  },
})

const emit = defineEmits(["fechar"])

const filtroDataInicio = ref("")
const filtroDataFim = ref("")
const filtroTitulo = ref("")

const inspecoesFiltradas = computed(() => {
  let lista = [...props.inspections]

  if(filtroDataInicio.value) {
    const dataInicio = new Date(`${filtroDataInicio.value}T00:00:00`)
    lista = lista.filter(i => new Date(i.data) >= dataInicio)
  }

  if(filtroDataFim.value) {
    const dataFim = new Date(`${filtroDataFim.value}T23:59:59`)
    lista = lista.filter(i => new Date(i.data) <= dataFim)
  }

  if(filtroTitulo.value.trim() !== "") {
    lista = lista.filter(i => (i.titulo ?? "").toLowerCase().includes(filtroTitulo.value.toLowerCase()))
  }

  lista.sort((a,b) => {
    const dateA = new Date(a.data)
    const dateB = new Date(b.data)
    return dateB - dateA
  })

  return lista
})

function editarInspecao() { emit("fechar") }
function apagarInspecao() { emit("fechar") }
</script>

<style scoped>
.historico-container{
  height:100vh;
  display:flex;
  flex-direction:column;
  background:#f5f5f5;
}

.historico-content{
  flex:1;
  padding:20px;
}

.filtros{
  display:flex;
  flex-wrap:wrap;
  gap:15px;
  margin-bottom:20px;
  justify-content:center;
}

.filtros label{
  display:flex;
  flex-direction:column;
  font-size:14px;
  color:#333;
}

.filtros input, .filtros select{
  margin-top:5px;
  padding:8px 10px;
  border-radius:6px;
  border:1px solid #ccc;
  font-size:14px;
}

@media(min-width:768px){
  .historico-content{
    padding:30px;
  }

  .filtros label{
    font-size:16px;
  }

  .filtros input, .filtros select{
    font-size:16px;
    padding:10px 12px;
  }
}
</style>