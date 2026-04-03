<template>
  <div class="page"  v-if="user">
    <div class="profile-card">

      <!-- HEADER -->
      <div class="header">
        <img class="avatar" :src="user.slikaUrl
          ?  BACKEND_URL+ user.slikaUrl
          : '/ProfilPlaceholder.jpg'" @click="openAvatarModal"/>
        <div class="header-info">
          <h1>{{ user.ime }} {{ user.prezime }}</h1>
          <p class="username">@{{ user.username }}</p>
        </div>
      </div>

      <div v-if="isAdmin">
        <section class="section">
        <div class="section-header">
          <h2>Status</h2>
        </div>

        <p>{{ user.role }}</p>

     
       
      <button v-if="user.role!='Admin'" class="btn " @click="unapredi(user.id)"> Unapredi u admina</button>
       </section>
      </div>
      <!-- BIO -->
      <section class="section">
        <div class="section-header">
          <h2>Biografija</h2>
        </div>

        <p>{{ user.biografija }}</p>
      </section>

      <!-- CONTACT -->
      <section class="section">
        <h2>Kontakt informacije</h2>

        <div class="field">
          <label>Email</label>
          <p>{{ user.email }}</p>
        </div>
      </section>

      <!-- SKILLS -->
      <section class="section">
        <div class="section-header">
          <h2>Veštine</h2>
        </div>

        <div class="skills">
          <span
            class="skill"
            v-for="(skill, index) in vestine"
            :key="index"
          >
            {{ skill }}
          </span>
        </div>
      </section>

    </div>
  </div>
  <div v-else class="page">
    <p style="color:white">Učitavanje profila...</p>
  </div>

  <div v-if="avatarModal" class="avatar-modal" @click.self="closeAvatarModal">
    <div class="avatar-modal-content">
      <img class="avatar-large" :src="user.slikaUrl
            ? BACKEND_URL+ user.slikaUrl
            : '/ProfilPlaceholder.jpg'"/>

      
    </div>
  </div>
</template>

<script setup>
import { reactive, ref, onMounted } from 'vue'
import api, { BACKEND_URL } from "@/api";
import { useRoute, useRouter } from 'vue-router';

const route = useRoute()
const user = ref(null);
const error = ref("");
const vestine = ref([])
const avatarModal = ref(false)

const isAdmin = JSON.parse(localStorage.getItem("user"))?.role === "Admin" // provera admina
onMounted(async () => {
  try {
    const res = await api.get(`/korisnici/username/${route.params.username}`)
    user.value = res.data
    console.log(user.value)
    //vestine.value = JSON.parse(user.value.vestineJson || "[]") TODO: srediti vestine
  } catch (err) {
    error.value =
      err.response?.data?.message || "Greška pri učitavanju profila"
    console.log(error.value)
  }
})

const openAvatarModal = () => {
  avatarModal.value = true
}
const closeAvatarModal = () => {
  avatarModal.value = false
}
const unapredi = async (id) => {
  try {
    await api.put(`/Korisnici/PromoteToAdmin/${id}`);
    user.value.role = "Admin";
    alert("Korisnik je sada Admin!");

  } catch (error) {
    console.error("Greška pri unapredjenju u admina:", error);
  }
};

</script>

<style scoped>
.page {
  min-height: 100vh;
  background: linear-gradient(135deg, #4338ca, #2563eb);
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 40px;
}

.profile-card {
  width: 100%;
  max-width: 700px;
  background: #ffffff;
  border-radius: 20px;
  padding: 30px;
  box-shadow: 0 25px 50px rgba(0, 0, 0, 0.2);
}

/* HEADER */
.header {
  display: flex;
  align-items: center;
  gap: 25px;
  margin-bottom: 30px;
}

.avatar {
  width: 140px;
  height: 140px;
  border-radius: 50%;
  border: 5px solid #4338ca;
   cursor: pointer;
}

.header-info h1 {
  font-size: 28px;
}

.username {
  color: #6b7280;
}

/* SECTIONS */
.section {
  margin-bottom: 30px;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.section h2 {
  margin-bottom: 10px;
}

/* FIELDS */
.field {
  display: grid;
  grid-template-columns: 1fr auto;
  gap: 10px;
  align-items: center;
  margin-bottom: 15px;
}

.field label {
  grid-column: 1 / -1;
  font-weight: 600;
}

/* INPUTS */
.input {
  padding: 10px 14px;
  border-radius: 10px;
  border: 1px solid #d1d5db;
  font-size: 14px;
}

.textarea {
  width: 100%;
  min-height: 100px;
}

/* BUTTONS */
.btn {
  background-color: #4338ca;
  color: white;
  border: none;
  padding: 10px 18px;
  border-radius: 10px;
  cursor: pointer;
  font-weight: 500;
}

.btn.small {
  padding: 8px 12px;
  font-size: 13px;
}

.btn:hover {
  background-color: #3730a3;
}

/* SKILLS */
.skills {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
}

.skill {
  background: #eef2ff;
  color: #3730a3;
  padding: 8px 14px;
  border-radius: 999px;
  display: flex;
  align-items: center;
  gap: 6px;
}

.remove {
  background: none;
  border: none;
  cursor: pointer;
  color: #991b1b;
}

.add-skill {
  display: flex;
  gap: 10px;
  margin-top: 10px;
}
.avatar-modal {
  position: fixed;
  inset: 0;
  background: rgba(0,0,0,0.7);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.avatar-modal-content {
  background: #fff;
  padding: 20px;
  border-radius: 12px;
  text-align: center;
}

.avatar-large {
  width: 280px;
  height: 280px;
  border-radius: 50%;
  border: 5px solid #4338ca;
  object-fit: cover;
  margin-bottom: 15px;
}
</style>
