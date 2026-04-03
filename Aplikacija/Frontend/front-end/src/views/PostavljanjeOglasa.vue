<template>
  <div class="page">
    <div class="card">
      <h1>Postavi novi oglas</h1>
      <p class="subtitle">
        Popuni informacije kako bi oglas bio vidljiv drugim korisnicima
      </p>

      <!-- BITNO: submit.prevent -->
      <form @keydown.enter.prevent @submit.prevent="submitOglas">

        <div class="field full">
          <label>Naziv oglasa *</label>
          <input
            v-model="oglas.naziv"
            type="text"
            required
            placeholder="npr. Električar – hitne intervencije"
          />
        </div>

        <div class="field full">
          <label>Opis</label>
          <textarea
            v-model="oglas.opis"
            rows="4"
            placeholder="Detaljan opis oglasa"
          ></textarea>
        </div>

        <div class="field">
          <label>Grad</label>
          <input v-model="oglas.grad" type="text" />
        </div>

        <div class="field">
          <label>Adresa</label>
          <input v-model="oglas.adresa" type="text" />
        </div>

        <div class="field full">
        <label>Izaberi lokaciju na mapi *</label>

        <div class="map-picker">
          <l-map
          :zoom="mapZoom"
          :center="mapCenter"
          :use-global-leaflet="false"
          style="height: 350px; width: 100%;"
          @click="onMapClick"
        >
            <l-tile-layer
              url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
            />

            <l-marker
              v-if="oglas.latitude && oglas.longitude"
              :lat-lng="[oglas.latitude, oglas.longitude]"
            />
          </l-map>
        </div>

        <small v-if="oglas.latitude">
          📍 Izabrano: {{ oglas.latitude.toFixed(5) }}, {{ oglas.longitude.toFixed(5) }}
        </small>
      </div>


        <div class="field">
          <label>Tip cene</label>
          <select v-model="oglas.tipCene">
            <option value="Fiksno">Fiksno</option>
            <option value="Satnica">Satnica</option>
            <option value="Dogovor">Dogovor</option>
            <option value="Besplatno">Besplatno</option>
          </select>
        </div>

        <div class="field">
          <label>Cena</label>
          <input
            v-model.number="oglas.cena"
            type="number"
            :disabled="oglas.tipCene === 'Dogovor' || oglas.TipCene === 'Besplatno'"
          />
        </div>

        <div class="field full">
          <label>Kategorija *</label>
          <select v-model="oglas.kategorijaId" required>
            <option disabled value="">Izaberi kategoriju</option>
            <option
              v-for="k in kategorije"
              :key="k.id"
              :value="k.id"
            >
              {{ k.naziv }}
            </option>
          </select>
        </div>

        <div class="field">
          <label>Slika oglasa</label>
          <img v-if="previewUrl" :src="previewUrl" class="preview"/>

          <button
  type="button"
  class="upload-btn"
  @click.prevent.stop="openFilePicker"
>
  📸 Izaberi sliku
</button>

          <input
          ref="fileInput"
          type="file"
          accept="image/*"
          hidden
          @change="onFileSelected"
          @keydown.enter.prevent
        />

          
        </div>
      

        <div class="actions">
          <button type="submit">Objavi oglas</button>
        </div>
    
        

      </form>

    </div>
  </div>
</template>

<script setup>
import { reactive, onMounted } from 'vue'
import api from '@/api'
import "leaflet/dist/leaflet.css";
import { LMap, LTileLayer, LMarker } from "@vue-leaflet/vue-leaflet";
import { ref } from "vue";
import {  useRouter } from 'vue-router';

const router = useRouter();
const mapCenter = ref([43.3209, 21.8958]); // Niš (default)
const mapZoom = ref(13);
const fileInput = ref(null)
const selectedFile = ref(null)
const previewUrl = ref(null)

function onMapClick(e) {
  oglas.latitude = e.latlng.lat;
  oglas.longitude = e.latlng.lng;
}
onMounted(() => {
  if (navigator.geolocation) {
    navigator.geolocation.getCurrentPosition(pos => {
      mapCenter.value = [
        pos.coords.latitude,
        pos.coords.longitude
      ];
    });
  }
});
function openFilePicker() {
  fileInput.value.click()
}
function onFileSelected(e) {
  if (!e.target.files || e.target.files.length === 0) {
      return;
    }
  const file = e.target.files[0]
  if (!file) return

  selectedFile.value = file
  previewUrl.value = URL.createObjectURL(file)
}

// MODEL KOJI IDE NA BACKEND
const oglas = reactive({
  naziv: '',
  opis: '',
  adresa: '',
  grad: '',
  latitude: null,
  longitude: null,
  cena: 0,
  tipCene: 'Fiksno',
  slikaUrl: '',
  kategorijaId: ''
})

const kategorije = reactive([])

// UČITAVANJE KATEGORIJA
onMounted(async () => {
  const res = await api.get('/Kategorije/all')
  kategorije.push(...res.data)
})

// SLANJE NA SERVER
async function submitOglas() {
  try {
    const formData = new FormData()

    formData.append("naziv", oglas.naziv)
    formData.append("opis", oglas.opis)
    formData.append("adresa", oglas.adresa)
    formData.append("grad", oglas.grad)
    formData.append("latitude", oglas.latitude)
    formData.append("longitude", oglas.longitude)
    formData.append("cena", oglas.cena)
    formData.append("tipCene", oglas.tipCene)
    formData.append("kategorijaId", oglas.kategorijaId)

    if (selectedFile.value) {
      formData.append("slika", selectedFile.value)
    }

    await api.post("/oglasi", formData)

    router.push("/")
  } catch (error) {
    console.error(error)
  }
}

/*async function submitOglas() {
  try {
    console.log('Šaljem:', oglas)
    await api.post('/oglasi', {
        naziv: oglas.naziv,
        opis: oglas.opis,
        adresa: oglas.adresa,
        grad: oglas.grad,
        latitude: Number(oglas.latitude),
        longitude: Number(oglas.longitude),
        cena: Number(oglas.cena),
        tipCene: 'Fiksno',
        slikaUrl: '',
        kategorijaId: Number(oglas.kategorijaId)
    })
    window.location.reload() 
    router.push("/");
  } catch (error) {
    if (error.response) {
    console.log("STATUS:", error.response.status)
    console.log("DATA:", error.response.data)   //  NAJBITNIJE
    console.log("HEADERS:", error.response.headers)
  } else {
    console.log("ERROR:", error.message)
  }
  }
}
  */
</script>

<style scoped>
.page {
  display: flex;
  justify-content: center;
  padding: 40px;
}

.card {
  background: white;
  padding: 32px;
  max-width: 900px;
  width: 100%;
  border-radius: 18px;
  box-shadow: 0 20px 40px rgba(0,0,0,.08);
}

.subtitle {
  color: #64748b;
  margin-bottom: 30px;
}

form {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 20px;
}

.field {
  display: flex;
  flex-direction: column;
}

.field.full {
  grid-column: span 2;
}

label {
  font-weight: 600;
  margin-bottom: 6px;
}

input, textarea, select {
  padding: 12px;
  border-radius: 12px;
  border: 1px solid #cbd5f5;
}

.actions {
  grid-column: span 2;
  display: flex;
  justify-content: flex-end;
}

button {
  background: #2563eb;
  color: white;
  border: none;
  padding: 14px 28px;
  border-radius: 14px;
  font-weight: 600;
  cursor: pointer;
}
.map-picker {
  height: 350px;
  border-radius: 16px;
  overflow: hidden;
  border: 1px solid #cbd5f5;
}
:deep(.leaflet-container) {
  height: 100%;
  width: 100%;
}

.upload-btn {
  background: #f1f5f9;
  color: #1e293b;
  border: 2px dashed #cbd5f5;
  padding: 16px;
  border-radius: 14px;
  font-weight: 600;
  cursor: pointer;
}

.upload-btn:hover {
  background: #e2e8f0;
}

.preview {
  margin-top: 10px;
  margin-bottom: 10px;
  max-height: 200px;
  max-width: 200px;
  border-radius: 12px;
  border: 1px solid black;
  object-fit: cover;
}

</style>
