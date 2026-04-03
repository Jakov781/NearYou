<template>
  <div class="main-container mt-3">
    
    <div class="filters-section mb-3">
      <div class="card filter-card shadow-md">
        <h2 class="mb-3 text-primary text-xl">Pronađi uslugu</h2>
        
        <div class="grid gap-3 mb-3"> 
          
          <div class="col-span-6">
            <label class="label">Ključne reči</label>
            <input 
              v-model="searchQuery" 
              @keyup.enter="fetchOglasi"
              type="text" 
              class="input std-height" 
              placeholder="Npr. 'telefon'" 
            />
          </div>

          <div class="col-span-4">
            <label class="label">Kategorija</label>
            <select v-model="selectedCategory" @change="fetchOglasi" class="select std-height">
              <option value="">Sve kategorije</option>
              
              <option 
                v-for="kategorija in kategorije" 
                :key="kategorija.id" 
                :value="kategorija.naziv"
              >
                {{ kategorija.naziv }}
              </option>
            </select>
          </div>

          <div class="col-span-2 flex items-end">
            <button @click="fetchOglasi" class="btn btn-primary w-full std-height shadow-sm flex-center">
              <span v-if="isLoading">⌛</span>
              <span v-else>🔍 Pretraži</span>
            </button>
          </div>

        </div>

        <div class="flex justify-between items-end border-t pt-3 border-gray-100">
           
           <div style="width: 20%;">
             <label class="label">Sortiranje</label>
             <select v-model="sortBy" @change="fetchOglasi" class="select std-height">
               <option :value="0">Najnovije</option>
               <option :value="1">Cena (Rastuća)</option>
               <option :value="2">Cena (Opadajuća)</option>
             </select>
           </div>

           <div style="width: 30%;" class="flex justify-end">
             <div class="flex gap-2 w-full">
               <button 
                 @click="viewMode = 'list'" 
                 class="btn w-full std-height" 
                 :class="viewMode === 'list' ? 'btn-primary' : 'btn-outline'"
               >
                 Lista
               </button>
               <button 
                 @click="handleMapToggle" 
                 class="btn w-full std-height" 
                 :class="viewMode === 'map' ? 'btn-primary' : 'btn-outline'"
               >
                 Mapa
               </button>
             </div>
           </div>

        </div>

      </div>
    </div>

    <div class="scrollable-content">
        
        <div v-if="isLoading" class="text-center p-5">
            <h3 class="text-muted">Učitavanje oglasa...</h3>
        </div>

        <div v-else-if="viewMode === 'list'">
          <div class="grid gap-4 pb-4 px-2"> 
            <div 
              v-for="oglas in oglasi" 
              :key="oglas.id" 
              class="col-span-4" 
              @click="openModal(oglas.id)"
            >
              <div class="card oglas-card h-full flex flex-col cursor-pointer hover-scale">
                
                <div class="card-image-wrapper mb-3">
                  <img 
                    v-if="oglas.slika"
                    :src="`${BACKEND_URL}${oglas.slika}`"
                    class="oglas-img"
                    @error="oglas.slika = null"
                  />

                  <div v-else class="fallback-icon flex-center">
                    <svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="#cbd5e1" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="3" width="18" height="18" rx="2" ry="2"></rect><circle cx="8.5" cy="8.5" r="1.5"></circle><polyline points="21 15 16 10 5 21"></polyline></svg>
                  </div>
                </div>

                <div class="flex-col justify-between flex-grow px-2">
                  <div class="mb-3">
                    <div class="flex items-center gap-2 mb-2 text-muted text-sm">
                   
                      <img
                        :src="getAvatarSrc(oglas)"
                        class="mini-avatar"
                        @error="oglas.postavljacSlika = null"
                      />

                       
                       <span>{{ oglas.postavljacIme }}</span>
                    </div>
                    <h3 class="text-base font-bold leading-tight" style="min-height: 2.5rem;">
                      {{ oglas.naziv }}
                    </h3>
                  </div>
                  
                  <div class="text-right border-t pt-2 border-gray-100">
                    <span class="price-tag">{{ oglas.cenaDisplay }}</span>
                  </div>
                </div>

              </div>
            </div>
          </div>

          <div v-if="oglasi.length === 0" class="text-center p-5">
            <h3 class="text-muted">Nema oglasa koji odgovaraju pretrazi.</h3>
          </div>
        </div>

        <div v-else class="map-view-container card p-0 overflow-hidden h-full">
          <l-map
            ref="mapRef"
            :zoom="zoom"
            :center="mapCenter"
            :use-global-leaflet="false"
            @moveend="handleMapMove"
            style="height: 100%; width: 100%;"
          >
            <l-tile-layer
              url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
              layer-type="base"
              name="OpenStreetMap"
            ></l-tile-layer>

            <l-marker 
              v-for="oglas in oglasi" 
              :key="oglas.id" 
              :lat-lng="[oglas.latitude, oglas.longitude]"
              @click="openModal(oglas.id)"
            >
              <l-icon class-name="custom-marker-wrapper">
                <div class="marker-container">
                  <div class="marker-tooltip">
                    {{ oglas.naziv }}
                  </div>
                  
                  <div class="marker-pill">
                    {{ oglas.cenaDisplay }}
                  </div>
                </div>
              </l-icon>
            </l-marker>
            
          </l-map>

          <div class="locate-me-btn">
             <button @click="requestLocation" class="btn btn-primary shadow-lg rounded-full w-12 h-12 flex items-center justify-center" title="Lociraj me">
                🎯
             </button>
        </div>
      </div>
    </div>
    
    <div v-if="selectedOglas" class="modal-overlay flex-center" @click.self="closeModal">
      <div class="modal-content card p-0 shadow-lg">
        
        <div class="modal-header">
            <img
            v-if="selectedOglas.slikaOglasa"
            :src="fullImageUrl"
            class="modal-cover-img"
            @error="selectedOglas.slikaOglasa = null"
          />
            <div v-else class="modal-fallback-header flex-center">
               <span>Pregled Usluge</span>
            </div>
            <button class="close-btn" @click="closeModal">&times;</button>
        </div>

        <div class="p-6"> 
          <div class="flex justify-between items-start mb-4">
            <h1 class="m-0 text-2xl col-span-8">{{ selectedOglas.naziv }}</h1>
            <span class="price-tag-lg">{{ selectedOglas.cenaDisplay }}</span>
          </div>
     
  

          <div class="user-section flex gap-4 items-center  mb-6 p-3 bg-blue-50 rounded-lg border border-blue-100">
            <div class="avatar-lg">
               <img
                  :src="avatarSrc"
                  alt="Avatar"
                  class="avatar-img-full"
                />
            </div>
            <div>
              <h4 class="m-0 text-lg">{{ selectedOglas.postavljac.ime }} {{ selectedOglas.postavljac.prezime }}</h4>
              <router-link :to="{ name: 'Profile', params: { username: selectedOglas.postavljac.username } }">
              <p class="text-sm text-muted m-0">
                @{{ selectedOglas.postavljac.username }} |
                Član od {{ selectedOglas.postavljac.godinaClanstva }}.
              </p>
            </router-link>
              <div class="mt-1">
                  <span class="badge bg-white">⭐ {{ selectedOglas.postavljac.prosecnaOcena }} ({{ selectedOglas.postavljac.brojRecenzija }} recenzija)</span>
              </div>
            </div>
            
             

          </div>
          
          <h4 class="mb-2 border-b pb-2">Opis oglasa</h4>
          <div class="description-content text-justify text-base mb-6 text-gray-700 leading-relaxed">
            <p style="white-space: pre-line;">{{ selectedOglas.opis }}</p>
          </div>

          <div class="alert alert-info text-sm mb-6 flex items-center gap-2">
            <span>📍</span> 
            <strong>Lokacija oglašivača:</strong> {{ selectedOglas.grad || 'Nepoznata' }} 
            (Lat: {{ selectedOglas.latitude }}, Lon: {{ selectedOglas.longitude }})
          </div>

          <div class="grid gap-3">
            <div :class="isAdmin ? 'col-span-6' : 'col-span-12'">
              <button v-if="loggedIn" class="btn btn-success w-full py-3 text-lg" @click="naruciPosao; showNaruciModal = true">✅ Naruči uslugu</button>
              <p v-else>Prijavite se da biste narucili oglas</p>
            </div>
            <div class= "col-span-6" >
              <button v-if="isAdmin"
              class="btn btn-danger w-full py-3 text-lg" @click="showDeleteConfirm = true"> Obriši oglas</button>
            </div>
          </div>

          <div v-if="showDeleteConfirm" class="">
                  <div class="bg-gray p-6 rounded-lg w-96 text-center">
                    <p class="mb-4 text-lg font-semibold">
                      Da li ste sigurni da želite da obrišete ovaj oglas?
                    </p>
                    <div class="flex justify-center gap-3">
                      <button class="btn btn-cancel px-4 py-2" @click="showDeleteConfirm = false">
                        Otkaži
                      </button>
                      <button class="btn btn-danger px-4 py-2" @click="obrisiOglas(selectedOglas.id)">
                       Da, obriši
                      </button>
                    </div>
                  </div>
            </div>
        </div>

      </div>
    </div>

  </div>



  <div
  v-if="showNaruciModal"
  class="modal-overlay flex-center"
  @click.self="showNaruciModal = false"
>
  <div class="modal-content card p-6 shadow-lg" style="max-width: 500px">

    <h2 class="text-xl font-bold mb-4">
      📝 Poruka za oglašivača
    </h2>

    <p class="text-sm text-muted mb-3">
      Pošaljite kratku poruku vezanu za uslugu:
    </p>

    <textarea
      v-model="porukaZaOglas"
      class="input w-full"
      rows="5"
      placeholder="Npr. Zdravo, zanima me dostupnost za sledeću nedelju..."
    ></textarea>

    <div class="flex justify-end gap-3 mt-4">
      <button
        class="btn btn-outline"
        @click="showNaruciModal = false"
      >
        Otkaži
      </button>

      <button
        class="btn btn-success"
        @click="naruciPosao"
      >
        📩 Pošalji
      </button>
    </div>

  </div>
</div>
</template>

<script setup>
import { ref, onMounted, watch,computed } from 'vue';
import { useRoute } from 'vue-router';
import "leaflet/dist/leaflet.css";
import { LMap, LTileLayer, LMarker, LPopup, LCircle, LIcon } from "@vue-leaflet/vue-leaflet";
import api, { BACKEND_URL } from "@/api";
import { getUser, isLoggedIn } from "@/auth";

const loggedIn = isLoggedIn()
const route = useRoute();

const searchQuery = ref("");
const selectedCategory = ref("");
const sortBy = ref(0); // 0 = DateDesc, to nam je default sort
const viewMode = ref("list");
const mapCenter = ref([43.3209, 21.8958]);
const zoom = ref(14);
const mapRef = ref(null);
const isLoading = ref(false);

const kategorije = ref([]); // kategorije dropdown
const oglasi = ref([]); // Lista iz pretrage
const selectedOglas = ref(null); // Detalji za modal
const isAdmin = JSON.parse(localStorage.getItem("user"))?.role === "Admin" // provera admina
const showDeleteConfirm = ref(false);
const selectedOglasId = ref(null);
const showNaruciModal = ref(false);
const porukaZaOglas = ref("");
const avatarVersion = ref(0);

// Lokacija korisnika
const userLocation = ref({ lat: null, lon: null });

const avatarSrc = computed(() => {
  if (!selectedOglas.value?.postavljac?.slikaUrl) return "/ProfilPlaceholder.jpg";
  return `${BACKEND_URL}${selectedOglas.value.postavljac.slikaUrl}?t=${avatarVersion.value}`;
});

const getAvatarSrc = (oglas) => {
  if (!oglas?.postavljacSlika) {
    return "/ProfilPlaceholder.jpg";
  }
  return `${BACKEND_URL}${oglas.postavljacSlika}`;
};

const fullImageUrl = computed(() => {
  if (!selectedOglas.value?.slikaOglasa) return null;
  return BACKEND_URL + selectedOglas.value.slikaOglasa;
});


const fetchKategorije = async () => {
  isLoading.value = true;
  try {
    const response = await api.get('/Kategorije/all');
    kategorije.value = response.data;
  } catch (error) {
    console.error("Greška pri učitavanju kategorija:", error);
    alert("Greška pri komunikaciji sa serverom.");
  } finally {
    isLoading.value = false;
  }
}

const fetchOglasi = async () => {
  isLoading.value = true;
  try {
    const params = {
      Q: searchQuery.value,
      Kategorija: selectedCategory.value,
      Sort: sortBy.value,
    };

    // Ako smo na mapi i mapa je učitana
    if (viewMode.value === 'map' && mapRef.value && mapRef.value.leafletObject) {
      const map = mapRef.value.leafletObject;
      const center = map.getCenter();
      const bounds = map.getBounds();
      const northEast = bounds.getNorthEast();

      mapCenter.value = [center.lat, center.lng];
      zoom.value = map._zoom;
      console.log(mapRef.value.leafletObject);

      // racunam distance od centra do ugla u metrima, pa pretvaramo u km
      const distanceMeters = map.distance(center, northEast);
      const radiusKm = distanceMeters / 1000;

      params.SearchLat = center.lat;
      params.SearchLon = center.lng;
      params.RadiusKm = radiusKm;
    }
    else if (viewMode.value === 'map' && userLocation.value.lat) {
       params.searchLat = userLocation.value.lat;
       params.searchLon = userLocation.value.lon;
       params.radiusKm = 20; 
    }

    const response = await api.get('/Oglasi/search', { params });
    oglasi.value = response.data;
  } catch (error) {
    console.error("Greška:", error);
  } finally {
    isLoading.value = false;
  }
};

const openModal = async (id) => {
  try {
  selectedOglasId.value = id
    // Prvo prikaži loading ili stari modal dok se ne učita
    const response = await api.get('/Oglasi/prikaz', { 
        params: { oglasId: id } 
    });
    selectedOglas.value = response.data; // OglasPrikazDto
    document.body.style.overflow = 'hidden';
  } catch (error) {
    console.error("Greška pri učitavanju detalja:", error);
  }
};

const closeModal = () => {
  selectedOglas.value = null;
  document.body.style.overflow = 'auto';
};

const requestLocation = () => {
  if (!navigator.geolocation) return;
  
  const options = { enableHighAccuracy: true, timeout: 5000 };
  
  navigator.geolocation.getCurrentPosition((position) => {
      userLocation.value = {
        lat: position.coords.latitude,
        lon: position.coords.longitude
      };
      mapCenter.value = [position.coords.latitude, position.coords.longitude];
      
      if(viewMode.value !== 'map') {
          viewMode.value = 'map';
          // Ovde moramo ručno fetch jer moveend možda ne okine ako se centar nije promenio a samo smo prebacili view
          setTimeout(() => fetchOglasi(), 100); 
      }
    }, 
    (err) => console.warn(err), 
    options
  );
};

const handleMapToggle = () => {
  viewMode.value = 'map';
  requestLocation();
};

const handleMapMove = () => {
  // Ovo se poziva svaki put kad korisnik pusti mapu nakon pomeranja/zumiranja
  fetchOglasi();
};

const naruciPosao = async() => {
  try{
    if (!selectedOglasId.value) return;
    await api.post("/prijave", {
      "oglasId" : Number(selectedOglasId.value),
      "poruka" : porukaZaOglas.value
    })
    window.location.reload() 
  }
  catch(err){
    console.log(err.value)
    alert("Vec ste prijavljeni na ovaj oglas")
  }
};

const obrisiOglas = async (id) => {
  try {
    await api.delete(`/Oglasi/${id}`);
    alert("Oglas obrisan");

    await fetchOglasi();
    closeModal();
  } catch (error) {
    console.error("Greška pri brisanju oglasa:", error);
  }
};


onMounted(() => {
  fetchKategorije();
  if (route.query.kategorija) {
    selectedCategory.value = route.query.kategorija; // za kad se klikne kategorija na navbar
  }
  fetchOglasi();
});



// za klik kategorije u navbar dok smo na mainpage
watch(() => route.query.kategorija, (novaKategorija) => {
  if (novaKategorija) {
    selectedCategory.value = novaKategorija;
    fetchOglasi();
  } else {
    selectedCategory.value = "";
    fetchOglasi();
  }
});

</script>

<style scoped>
.main-container {
  width: 85%;
  margin: 0 auto;
  max-width: 1600px;
  height: calc(100vh - 80px);
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.map-view-container {
  height: 95%;
  position: relative;
}
.oglas-map {
  height: 100%;
}
.locate-me-btn {
  position: absolute;
  bottom: 2rem;
  right: 2rem;
  z-index: 1000;
}
.locate-me-btn > .btn {
  background-color: #ffffff99;
}

.scrollable-content {
  flex-grow: 1;
  overflow-y: auto;
  padding-right: 5px;
  width: 100%;
}

.filters-section {
  width: 70%;
  min-width: 900px;
  align-self: center;
  flex-shrink: 0;
  margin-top: 1rem;
}

.filter-card {
  padding: 1rem 1.5rem;
}

.std-height {
  height: 40px !important;
  padding: 0.2rem 0.8rem;
  font-size: 0.95rem;
  display: flex;
  align-items: center;
}
.select.std-height {
  padding-right: 2rem;
}

.btn-primary.std-height, .btn-outline.std-height {
  justify-content: center;
}

.text-primary { color: var(--primary); }
.text-xl { font-size: 1.25rem; font-weight: 700; }

.card-image-wrapper {
  height: 200px; 
  background-color: var(--base-200);
  border-radius: var(--radius-sm);
  overflow: hidden;
  position: relative;
}
.oglas-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}
.fallback-icon {
  width: 100%;
  height: 100%;
  background-color: var(--base-100);
  display: flex;
  align-items: center;
  justify-content: center;
}

.oglas-card {
  transition: all 0.2s ease;
  border: 1px solid transparent;
}
.hover-scale:hover {
  transform: translateY(-5px);
  box-shadow: var(--shadow-lg);
  border-color: var(--primary-lighter);
}

.mini-avatar {
  width: 24px;
  height: 24px;
  border-radius: 50%;
  object-fit: cover;
  border: 1px solid var(--base-300);
}

.price-tag {
  font-weight: 800;
  color: var(--base-900);
  font-size: 1.1rem;
}
.price-tag-lg {
  font-weight: 800;
  color: var(--success);
  font-size: 1.8rem;
}

.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  background-color: rgba(0, 0, 0, 0.6);
  z-index: 2000;
  display: flex;
  justify-content: center;
  align-items: center;
  backdrop-filter: blur(3px);
  padding: 20px;
}
.modal-content {
  background: white;
  width: 100%;
  max-width: 800px;
  max-height: 90vh;
  overflow-y: auto;
  border-radius: var(--radius-lg);
  animation: slideUp 0.3s ease-out;
}
.modal-header {
  height: 250px;
  background: var(--base-200);
  position: relative;
}
.modal-cover-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}
.modal-fallback-header {
  width: 100%;
  height: 100%;
  background: var(--base-300);
  color: var(--base-600);
  font-weight: bold;
  font-size: 1.5rem;
}
.close-btn {
  position: absolute;
  top: 15px;
  right: 15px;
  background: white;
  color: black;
  border: none;
  border-radius: 50%;
  width: 40px;
  height: 40px;
  cursor: pointer;
  font-size: 1.5rem;
  box-shadow: var(--shadow-md);
  transition: transform 0.2s;
}
.close-btn:hover { transform: rotate(90deg); }

.avatar-lg img {
  width: 70px;
  height: 70px;
  border-radius: 50%;
  border: 3px solid white;
  box-shadow: var(--shadow-sm);
}

@keyframes slideUp {
  from { transform: translateY(30px); opacity: 0; }
  to { transform: translateY(0); opacity: 1; }
}

/* --- LEAFLET CUSTOM MARKER STILOVI UMRO SAM NA OVO --- */
:deep(.custom-marker-wrapper) {
  background: none;
  border: none;
}

.marker-container {
  position: relative;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  /* Centriramo ga da ne beži (Leaflet defaultno kači gornji levi ugao) */
  transform: translate(-50%, -50%);
  cursor: pointer;
}

/* cena (uvek vidljiva pilula) */
.marker-pill {
  background-color: white;
  color: var(--base-900);
  font-weight: 800;
  padding: 6px 10px;
  border-radius: 20px;
  box-shadow: 0 2px 5px rgba(0,0,0,0.3);
  font-size: 0.9rem;
  transition: transform 0.2s ease, background-color 0.2s;
  white-space: nowrap;
}

/* hover efekat na cenu */
.marker-container:hover .marker-pill {
  transform: scale(1.1);
  background-color: var(--base-900);
  color: white;
  z-index: 1000; /* Da iskoči ispred drugih */
}

/* aslov (tooltip iznad) - default sakrivenn */
.marker-tooltip {
  position: absolute;
  bottom: 35px; /* Iznad cene */
  background-color: var(--base-900);
  color: white;
  padding: 5px 10px;
  border-radius: 6px;
  font-size: 0.8rem;
  white-space: nowrap;
  opacity: 0;
  visibility: hidden;
  transition: all 0.2s ease;
  transform: translateY(10px); /* Mali slide-up efekat */
  box-shadow: 0 4px 6px rgba(0,0,0,0.2);
  pointer-events: none; /* Da ne smeta pri kliku */
}

/* Trouglić ispod tooltipa (opciono, za lepši izgled) */
.marker-tooltip::after {
  content: '';
  position: absolute;
  top: 100%;
  left: 50%;
  margin-left: -5px;
  border-width: 5px;
  border-style: solid;
  border-color: var(--base-900) transparent transparent transparent;
}

/* Prikazivanje naslova na hover */
.marker-container:hover .marker-tooltip {
  opacity: 1;
  visibility: visible;
  transform: translateY(0);
  z-index: 1001;
}
</style>