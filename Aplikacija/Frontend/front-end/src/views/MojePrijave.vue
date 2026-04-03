<template>
  <div class="page">
    <h1>Moje prijave</h1>

    <div v-if="prijave.length === 0" class="empty">
      Nemate nijednu prijavu.
    </div>

    <div
      v-for="prijava in prijave"
      :key="prijava.id"
      class="prijava-card"
      :class="statusClass(prijava.status)"
    >
      <div class="header">
        <h3>{{ prijava.oglasNaziv }}</h3>

        <span class="status">
          {{ prijava.status }}
        </span>
      </div>

      <p class="time">
        {{ formatDate(prijava.vremePrijave) }}
      </p>

      <p class="poruka">
        {{ prijava.poruka }}
      </p>
    </div>
  </div>
</template>
<script setup>
import { ref, onMounted } from "vue";
import api from "@/api";

const prijave = ref([]);

onMounted(loadPrijave);

async function loadPrijave() {
  const res = await api.get("/prijave/oglas/moje");

  // ako backend vraća PascalCase
  prijave.value = res.data.map(p => ({
    id: p.id ?? p.ID,
    oglasNaziv: p.oglasNaziv ?? p.OglasNaziv,
    status: p.status ?? p.Status,
    poruka: p.poruka ?? p.Poruka,
    vremePrijave: p.vremePrijave ?? p.VremePrijave
  }));
}

function statusClass(status) {
  return {
    accepted: status === "Prihvacena",
    rejected: status === "Odbijena",
    pending: status !== "Prihvacena" && status !== "Odbijena"
  };
}

function formatDate(date) {
  return new Date(date).toLocaleString("sr-RS");
}
</script>


<style scoped>
.page {
  max-width: 900px;
  margin: auto;
}

h1 {
  margin-bottom: 20px;
}

/* CARD */
.prijava-card {
  background: #f8fafc;
  padding: 20px;
  border-radius: 16px;
  margin-bottom: 16px;
  box-shadow: 0 8px 18px rgba(0,0,0,.08);
  border-left: 6px solid transparent;
}

/* HEADER */
.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.status {
  font-weight: 600;
  font-size: 14px;
}

/* STATUS BOJE */
.prijava-card.accepted {
  background: #dcfce7;
  border-left-color: #22c55e;
}

.prijava-card.accepted .status {
  color: #166534;
}

.prijava-card.rejected {
  background: #fee2e2;
  border-left-color: #ef4444;
}

.prijava-card.rejected .status {
  color: #991b1b;
}

.prijava-card.pending {
  background: #f1f5f9;
  border-left-color: #64748b;
}

.prijava-card.pending .status {
  color: #475569;
}

/* TEXT */
.cena {
  color: #64748b;
  font-size: 14px;
  margin: 6px 0;
}

.poruka {
  margin-top: 10px;
}

.empty {
  color: #94a3b8;
}
</style>
