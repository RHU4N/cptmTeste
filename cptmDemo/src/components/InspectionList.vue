<template>
  <div class="inspection-list">
    <button class="create-btn" @click="criarInspecao">+ Nova Inspeção</button>

    <p v-if="loading" class="message">Carregando inspeções...</p>
    <p v-else-if="!localInspections.length" class="message">Nenhuma inspeção cadastrada.</p>

    <ul>
      <li
        v-for="inspecao in localInspections"
        :key="inspecao.id"
      >
        <div class="info">
          <span class="linha">{{ inspecao.titulo || "Sem título" }}</span>
          <span class="data-hora">{{ formatDate(inspecao.data) }}</span>
          <span v-if="showUser" class="user">{{ inspecao.usuario }}</span>
          <span class="descricao">{{ inspecao.descricao }}</span>
        </div>

        <div class="actions">
          <button @click="$emit('edit', inspecao)">Editar</button>
          <button class="danger" @click="$emit('delete', inspecao)">Apagar</button>
        </div>
      </li>
    </ul>
  </div>
</template>

<script setup>
import { computed } from "vue";

const props = defineProps({
  inspections: { type: Array, required: true },
  loading: { type: Boolean, default: false },
  showUser: { type: Boolean, default: false },
});

const emit = defineEmits(["create", "edit", "delete"]);

const localInspections = computed(() => props.inspections ?? []);

function criarInspecao() {
  emit("create");
}

function formatDate(value) {
  if (!value) return "-";
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return value;
  return date.toLocaleString("pt-BR");
}
</script>

<style scoped>
.inspection-list {
  margin-top: 20px;
}

.create-btn {
  padding: 10px 15px;
  border: none;
  border-radius: 8px;
  background: #ea191f;
  color: white;
  font-weight: bold;
  cursor: pointer;
  margin-bottom: 15px;
}

.create-btn:hover {
  opacity: 0.9;
}

ul {
  list-style: none;
  padding: 0;
  margin: 0;
}

li {
  background: #fff;
  border-radius: 8px;
  padding: 10px;
  margin-bottom: 10px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  box-shadow: 0 1px 4px rgba(0, 0, 0, 0.1);
}

.info span {
  display: block;
  margin-right: 10px;
}

.linha {
  font-size: 1rem;
  font-weight: 700;
}

.data-hora {
  color: #777;
  margin-top: 2px;
}

.descricao {
  margin-top: 5px;
}

.actions button {
  margin-left: 5px;
  padding: 5px 10px;
  border: none;
  border-radius: 6px;
  background: #ea191f;
  color: white;
  cursor: pointer;
}

.actions button:hover {
  opacity: 0.9;
}

.actions button.danger {
  background: #444;
}

.message {
  color: #555;
  margin: 8px 0 14px;
}

@media (min-width: 768px) {
  li {
    padding: 15px;
  }
  .actions button {
    padding: 6px 12px;
  }
  .create-btn {
    padding: 12px 18px;
  }
}
</style>
