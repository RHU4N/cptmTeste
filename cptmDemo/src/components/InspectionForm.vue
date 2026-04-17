<script setup>
import { reactive, watch, computed, ref, onBeforeUnmount } from "vue"

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

const photoInputKey = ref(0)
const photoPreviewUrl = ref("")

const isEditMode = computed(() => Boolean(props.initialInspecao?.id))
const initialPhotoPreview = computed(() => {
  if (props.initialInspecao?.photoUrl) {
    return props.initialInspecao.photoUrl
  }

  if (props.initialInspecao?.photoBase64) {
    return `data:image/jpeg;base64,${props.initialInspecao.photoBase64}`
  }

  return ""
})

const photoPreview = computed(() => photoPreviewUrl.value || initialPhotoPreview.value)
const isPhotoRequired = computed(() => !photoPreview.value)

function toDatetimeLocalValue(value) {
  if (!value) {
    return ""
  }

  const date = new Date(value)

  if (Number.isNaN(date.getTime())) {
    return ""
  }

  const pad = (number) => String(number).padStart(2, "0")

  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`
}

function revokePreviewUrl() {
  if (photoPreviewUrl.value.startsWith("blob:")) {
    URL.revokeObjectURL(photoPreviewUrl.value)
  }
}

watch(
  () => props.initialInspecao,
  (value) => {
    revokePreviewUrl()
    form.titulo = value?.titulo ?? ""
    form.descricao = value?.descricao ?? ""
    form.data = toDatetimeLocalValue(value?.data)
    form.photo = null
    photoPreviewUrl.value = ""
    photoInputKey.value += 1
  },
  { immediate: true }
)

onBeforeUnmount(() => {
  revokePreviewUrl()
})

function onFileChange(event) {
  const [file] = event.target.files
  revokePreviewUrl()
  form.photo = file ?? null

  if (file) {
    photoPreviewUrl.value = URL.createObjectURL(file)
  } else {
    photoPreviewUrl.value = ""
  }
}

function removerFoto() {
  revokePreviewUrl()
  form.photo = null
  photoPreviewUrl.value = ""
  photoInputKey.value += 1
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
        <label for="data">Data e hora</label>
        <input id="data" type="datetime-local" v-model="form.data" required />
      </div>

      <div class="form-item">
        <label for="photo">Foto</label>
        <input
          :key="photoInputKey"
          id="photo"
          type="file"
          accept="image/*"
          @change="onFileChange"
          :required="isPhotoRequired"
        />
      </div>

      <div v-if="photoPreview" class="photo-preview-box">
        <img :src="photoPreview" alt="Prévia da foto selecionada" class="photo-preview" />
        <button type="button" class="secondary remove-photo" @click="removerFoto">
          Remover foto
        </button>
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

.photo-preview-box {
  display: flex;
  flex-direction: column;
  gap: 10px;
  margin-bottom: 15px;
  align-items: center;
}

.photo-preview {
  width: 100%;
  max-width: 360px;
  height: 240px;
  object-fit: contain;
  padding: 10px;
  box-sizing: border-box;
  border-radius: 10px;
  border: 1px solid #ddd;
  background: #f8f8f8;
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

.remove-photo {
  align-self: center;
}

.navigation button:disabled {
  opacity:0.5;
  cursor:not-allowed;
}

@media (min-width: 768px) {
  .photo-preview {
    max-width: 420px;
    height: 280px;
    padding: 12px;
  }

  .photo-preview-box {
    align-items: center;
  }
}
</style>