<script setup>
import { reactive, watch, computed } from "vue"

const props = defineProps({
  initialInspecao: {
    type: Object,
    default: null,
  },
  submitting: {
    type: Boolean,
    default: false,
  },
})

const emit = defineEmits(["finalizar", "fechar"])

const form = reactive({
  titulo: "",
  descricao: "",
  data: "",
  photo: null,
})

const isEditMode = computed(() => Boolean(props.initialInspecao?.id))

watch(
  () => props.initialInspecao,
  (value) => {
    form.titulo = value?.titulo ?? ""
    form.descricao = value?.descricao ?? ""
    form.data = value?.data ? new Date(value.data).toISOString().split("T")[0] : ""
    form.photo = null
  },
  { immediate: true }
)

function onFileChange(event) {
  const [file] = event.target.files
  form.photo = file ?? null
}

function onSubmit() {
  emit("finalizar", {
    id: props.initialInspecao?.id ?? null,
    titulo: form.titulo,
    descricao: form.descricao,
    data: form.data,
    photo: form.photo,
  })
}
</script>

<template>
  <div class="inspection-form">
    <h2>{{ isEditMode ? "Editar inspeção" : "Nova inspeção" }}</h2>

    <form class="form-page" @submit.prevent="onSubmit">
      <div class="form-item">
        <label for="titulo">Título</label>
        <input id="titulo" type="text" v-model="form.titulo" required />
      </div>

      <div class="form-item">
        <label for="descricao">Descrição</label>
        <textarea id="descricao" v-model="form.descricao" rows="4" required></textarea>
      </div>

      <div class="form-item">
        <label for="data">Data</label>
        <input id="data" type="date" v-model="form.data" required />
      </div>

      <div class="form-item">
        <label for="photo">Foto</label>
        <input id="photo" type="file" accept="image/*" @change="onFileChange" required />
      </div>

      <div class="navigation">
        <button type="button" class="secondary" @click="$emit('fechar')">Cancelar</button>
        <button type="submit" :disabled="submitting">
          {{ submitting ? "Salvando..." : "Salvar" }}
        </button>
      </div>
    </form>
  </div>
</template>

<style scoped>
.inspection-form {
  max-width: 600px;
  margin: 20px auto;
  background: #fff;
  padding: 20px;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.15);
}

.form-item { margin-bottom: 15px; }
.form-item label { font-weight: bold; display:block; margin-bottom:5px; }
.form-item input[type="text"],
.form-item input[type="date"],
.form-item textarea,
.form-item input[type="file"] {
  width:100%;
  padding:10px;
  border-radius:6px;
  border:1px solid #ccc;
  box-sizing: border-box;
}

.navigation {
  display:flex;
  justify-content:flex-end;
  gap: 10px;
  align-items:center;
  margin-top:20px;
}

.navigation button {
  padding:10px 15px;
  border:none;
  border-radius:6px;
  background:#ea191f;
  color:white;
  font-weight:bold;
  cursor:pointer;
}

.navigation button.secondary {
  background: #666;
}

.navigation button:disabled {
  opacity:0.5;
  cursor:not-allowed;
}
</style>