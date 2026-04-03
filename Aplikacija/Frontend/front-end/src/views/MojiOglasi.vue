<template>
  <div class="page">
    <h1>Moji oglasi</h1>

    <!-- OGLASI -->
    <div v-if="oglasi.length === 0" class="empty">
      Nemate postavljenih oglasa.
    </div>

    <div @click="selectOglas(oglas)" v-for="oglas in oglasi" :key="oglas.id" class="oglas-card">
      <div class="oglas-header">
        <h3>
          {{ oglas.naziv }}
        </h3>

        <button class="delete" @click.stop="obrisiOglas(oglas.id)">
          Obriši
        </button>
      </div>

      <p class="muted">{{ oglas.cenaDisplay }}</p>

      <!-- PRIJAVE -->
      <div v-if="selectedOglas?.id === oglas.id" class="prijave">
        <h4>Prijave</h4>

        <div v-if="prijave.length === 0" class="empty">
          Nema prijava za ovaj oglas.
        </div>

       <div
        v-for="prijava in prijave"
        :key="prijava.id"
        class="prijava"
        :class="{
          accepted: prijava.status === 'Prihvacena',
          disabled: prijava.status === 'Prihvacena'
        }"
        @click="prijava.status !== 'Prihvacena' && openModal(prijava)"
       >
          <strong>
            {{ prijava.korisnik.ime }} {{ prijava.korisnik.prezime }}
          </strong>
          <span class="muted">@{{ prijava.korisnik.username }}</span>

          <p>{{ prijava.poruka }}</p>
        </div>
      </div>
    </div>

    <!-- MODAL -->
    <div v-if="showModal" class="modal-overlay">
>
      <div class="modal">
        <h3>Prijava korisnika</h3>

        <p>
          <strong>
            {{ selectedPrijava.korisnik.ime }}
            {{ selectedPrijava.korisnik.prezime }}
          </strong>
        </p>

        <p class="muted">@{{ selectedPrijava.korisnik.username }}</p>

        <p class="poruka">
          {{ selectedPrijava.poruka }}
        </p>

        <div class="modal-actions">
          <button class="accept" @click="promeniStatus('Prihvacena')">
            Prihvati
          </button>

          <button class="reject" @click="promeniStatus('Odbijena')">
            Odbij
          </button>

          <button class="close" @click="closeModal">
            Zatvori
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import api from '@/api'

const oglasi = ref([])
const prijave = ref([])

const selectedOglas = ref(null)
const selectedPrijava = ref(null)
const showModal = ref(false)

onMounted(loadOglasi)

async function loadOglasi() {
  const res = await api.get('/oglasi/korisnik')
  oglasi.value = res.data
}

async function selectOglas(oglas) {
  selectedOglas.value = oglas

  const res = await api.get(`/prijave/oglas/${oglas.id}`)
  prijave.value = res.data
}

function openModal(prijava) {
  selectedPrijava.value = prijava
  showModal.value = true
}

function closeModal() {
  showModal.value = false
  selectedPrijava.value = null
}

async function promeniStatus(status) {
  await api.put(`/prijave/${selectedPrijava.value.id}/status`, {
    status
  })

  closeModal()
  selectOglas(selectedOglas.value)
}

async function obrisiOglas(oglasId) {
  if (!confirm('Da li ste sigurni da želite da obrišete oglas?')) return

  await api.put(`/oglasi/obrisi/${oglasId}`)
  oglasi.value = oglasi.value.filter(o => o.id !== oglasId)
}
</script>

<style scoped>
.page {
  max-width: 900px;
  margin: auto;
}

.oglas-card {
  background: white;
  padding: 20px;
  border-radius: 14px;
  margin-bottom: 16px;
  box-shadow: 0 10px 20px rgba(0,0,0,.08);
}

.oglas-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

h3 {
  cursor: pointer;
}

.delete {
  background: #ef4444;
  color: white;
  border: none;
  padding: 6px 12px;
  border-radius: 8px;
  cursor: pointer;
}

.prijava {
  padding: 12px;
  border-radius: 10px;
  background: #f1f5f9;
  margin-top: 10px;
  cursor: pointer;
}

.prijava:hover {
  background: #e2e8f0;
}

.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0,0,0,.6);
  display: flex;
  align-items: center;
  justify-content: center;
}

.modal {
  background: white;
  padding: 24px;
  width: 400px;
  border-radius: 16px;
}

.modal-actions {
  display: flex;
  gap: 10px;
  margin-top: 20px;
}

.accept {
  background: #22c55e;
}

.reject {
  background: #ef4444;
}

.close {
  background: #64748b;
}
.prijava.accepted {
  background: #dcfce7;        /* svetlo zelena */
  border: 1px solid #22c55e;
}

.prijava.accepted:hover {
  background: #bbf7d0;
}
button {
  color: white;
  border: none;
  padding: 10px;
  border-radius: 10px;
  cursor: pointer;
}

.muted {
  color: #64748b;
}

.empty {
  color: #94a3b8;
}
</style>
