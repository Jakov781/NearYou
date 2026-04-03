<template>
  <nav class="navbar">
    <div class="flex items-center justify-between h-full" style="width: 90%; max-width: 1600px; margin: 1rem auto;">
      
      <div class="flex items-center gap-6">
        <RouterLink to="/" class="logo-link">
          <span class="logo-text">Near<span class="text-primary">You</span></span>
        </RouterLink>
        
        <div class="desktop-links hidden-mobile ml-4 flex gap-4">
          <!-- <RouterLink 
            v-for="cat in kategorije.slice(0, 5)" 
            :key="cat.id"
            :to="{ path: '/', query: { kategorija: cat.naziv } }"
            class="nav-link"
          >
            {{ cat.naziv }}
          </RouterLink> -->
        </div>
      </div>

      <div v-if="loggedIn" class="flex items-center gap-3">
        
        <router-link to="/PostaviOglas" class="link"> 
        <button class="btn btn-outline text-sm font-bold mr-2">
          + Postavi oglas 
        </button>
      </router-link> 
      
      <router-link to="/chat">
        <div class="icon-btn" title="Poruke">
          <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z"></path></svg>
          <span class="notification-dot"></span>
        </div>
      </router-link>
      <router-link to="/mojeprijave">
  <div class="icon-btn" title="Moje prijave">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            width="20"
            height="20"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
          >
            <path d="M9 11l3 3L22 4" />
            <path d="M21 12v7a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11" />
          </svg>
        </div>
          </router-link>
            <router-link to="/MojiOglasi">
          <div class="icon-btn" title="Moji Oglasi">
            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-list-icon lucide-list"><path d="M3 5h.01"/><path d="M3 12h.01"/><path d="M3 19h.01"/><path d="M8 5h13"/><path d="M8 12h13"/><path d="M8 19h13"/></svg>
          </div>
        </router-link>
        <router-link to="/me">
          <div class="user-menu flex items-center gap-2 ml-2 cursor-pointer hover-bg p-1 rounded">
             <img class="nav-avatar" :src="avatarSrc" />
            <span class="font-bold text-sm hidden-mobile">Moj Profil</span>
          </div>
        </router-link>
        <router-link @click="Logout()" to="/">Odjavite se</router-link>
      </div>

      <div v-else class="flex items-center gap-3">
        <router-link to="/login">Prijavite se</router-link>
        <router-link to="/register">Registrujte se</router-link>
      </div>

    </div>
  </nav>
</template>

<script setup>
import { ref, onMounted,onUnmounted,computed } from 'vue';
import { RouterLink } from 'vue-router';
import api, { BACKEND_URL } from "@/api";
import { getUser, isLoggedIn } from "@/auth";
const kategorije = ref([]);

const loggedIn = isLoggedIn()
const user = ref(getUser());
const avatarVersion = ref(0);


// fallback ako se user promeni kasnije
window.addEventListener("storage", () => {
  user.value = JSON.parse(localStorage.getItem("user") || "null");
});

const avatarSrc = computed(() => {
  if (!user.value?.slikaUrl) return "/ProfilPlaceholder.jpg";
  return `${BACKEND_URL}${user.value.slikaUrl}?t=${avatarVersion.value}`;
});

const Logout = () => {
  localStorage.removeItem("token");
  localStorage.removeItem("user");
  window.location.reload() 
}

const fetchKategorije = async () => {
  try {
    const response = await api.get('/Kategorije/popular');
    kategorije.value = response.data;
  } catch (error) {
    console.error("Greška pri učitavanju kategorija u navbaru:", error);
  }
};

onMounted(() => {
  fetchKategorije();
  window.addEventListener("user-updated", onUserUpdated);
});
onUnmounted(() => {
  window.removeEventListener("user-updated", onUserUpdated);
});

function onUserUpdated(e) {
  user.value = e.detail;
  avatarVersion.value++; // cache bust
}
</script>

<style scoped>
.navbar {
  height: 5rem;
  background-color: var(--base-white);
  border-bottom: 1px solid var(--base-200);
  position: sticky;
  top: 0;
  z-index: 1000; 
  box-shadow: var(--shadow-sm);
  padding: 0 var(--spacing-3);
}

.logo-text {
  font-size: 1.5rem;
  font-weight: 800;
  letter-spacing: -0.5px;
  color: var(--base-900);
}
.text-primary { color: var(--primary); }

.nav-link {
  color: var(--base-500);
  font-weight: 600;
  margin-right: 2rem;
  font-size: 0.95rem;
  transition: color 0.2s;
  text-decoration: none;
}
.nav-link:hover { color: var(--primary); }

/* Buttons & Icons */
.icon-btn {
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--base-600);
  cursor: pointer;
  border-radius: 50%;
  transition: background 0.2s;
  position: relative;
}
.icon-btn:hover { background-color: var(--base-100); color: var(--primary); }

.notification-dot {
  position: absolute;
  top: 10px;
  right: 10px;
  width: 8px;
  height: 8px;
  background-color: var(--danger);
  border-radius: 50%;
  border: 1px solid white;
}

/* User Avatar */
.nav-avatar {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  object-fit: cover;
  border: 1px solid var(--base-300);
}

.hover-bg:hover {
  background-color: var(--base-100);
}

/* Responsive */
@media (max-width: 768px) {
  .hidden-mobile { display: none; }
  .logo-text { font-size: 1.2rem; }
}
.link{
  color: black;
}
</style>