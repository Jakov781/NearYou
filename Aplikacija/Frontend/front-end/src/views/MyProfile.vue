<template>
  <div v-if="loggedIn" class="page">
    <div class="profile-card">

      <!-- HEADER -->
      <div class="header">
 
        <img class="avatar" :src="avatarSrc" @click="openAvatarModal"/>

        <div class="header-info">
          <h1>{{ user.firstName}} {{ user.lastName }}</h1>
          <p class="username">@{{ user.username }}</p>
        </div>
      </div>

      <!-- BIO -->
      <section class="section">
        <div class="section-header">
          <h2>Biografija</h2>
          <button class="btn" @click="editing.bio = !editing.bio; prikaziSacuvajDugme()">Izmeni</button>
        </div>

        <p v-if="!editing.bio">{{ user.bio }}</p>
        <textarea
          v-else
          v-model="user.bio"
          class="input textarea"
        ></textarea>
      </section>

      <!-- CONTACT -->
      <section class="section">
        <h2>Kontakt informacije</h2>

        <div class="field">
          <label>Email</label>
          <input
            class="input"
            v-model="user.email"
            :disabled="!editing.email"
          />
          <button class="btn small" @click="editing.email = !editing.email; prikaziSacuvajDugme()">
            {{ editing.email ? 'Sačuvaj' : 'Izmeni' }}
          </button>
        </div>

        <div class="field">
          <label>Telefon</label>
          <input
            class="input"
            v-model="user.phone"
            :disabled="!editing.phone"
          />
          <button class="btn small" @click="editing.phone = !editing.phone; prikaziSacuvajDugme()">
            {{ editing.phone ? 'Sačuvaj' : 'Izmeni' }}
          </button>
        </div>
      </section>

      <!-- SKILLS -->
      <section class="section">
        <div class="section-header">
          <h2>Veštine</h2>
          <button class="btn" @click="editing.skills = !editing.skills; prikaziSacuvajDugme()">
            {{ editing.skills ? 'Sačuvaj' : 'Izmeni' }}
          </button>
        </div>

        <div class="skills">
          <span
            class="skill"
            v-for="(skill, index) in user.skills"
            :key="index"
          >
            {{ skill.replace(/[\[\]"]/g, '') }}
            <button
              v-if="editing.skills"
              class="remove"
              @click="removeSkill(index)"
            >
              ✕
            </button>
          </span>
        </div>

        <div v-if="editing.skills" class="add-skill">
          <input
            class="input"
            v-model="newSkill"
            placeholder="Nova veština"
          />
          <button class="btn small" @click="addSkill">Dodaj</button>
        </div>
      </section>

          <section class="section">
      <div class="section-header">
        <h2>Promena šifre</h2>
      </div>

      <div class="field">
        <label>Trenutna šifra</label>
        <input
          type="password"
          class="input"
          v-model="passwords.oldPassword"
        />
      </div>

      <div class="field">
        <label>Nova šifra</label>
        <input
          type="password"
          class="input"
          v-model="passwords.newPassword"
        />
      </div>

      <div class="field">
        <label>Potvrda nove šifre</label>
        <input
          type="password"
          class="input"
          v-model="passwords.confirmPassword"
        />
      </div>

      <button class="btn small" @click="promeniSifru">
        Promeni šifru
      </button>
    </section>

       <button v-if="izmenaDugme" class="btn small" @click="sacuvajPromene()">
            Sacuvaj izmene
          </button>
    </div>
  </div>

  <div v-if="avatarModal" class="avatar-modal" @click.self="closeAvatarModal">
    <div class="avatar-modal-content">
      <img class="avatar-large" :src="avatarSrc"/>

      <button class="btn" @click="triggerFilePicker">
        Promeni sliku
      </button>

      <input type="file" accept="image/*" ref="fileInput" hidden @change="onAvatarSelected"/>

      
    </div>
  </div>
</template>

<script setup>
import { reactive, ref } from 'vue'
import { computed } from "vue";
import { isLoggedIn, getUser } from "@/auth";
import api, { BACKEND_URL } from "@/api";

const loggedIn = isLoggedIn()
let izmenaDugme = false;
let count = false
const prikaziSacuvajDugme = async () => {
  if(!count)
    count = true
  else
    izmenaDugme = true;
}
const userdo = getUser();
console.log(userdo)
const user = reactive({
  firstName: userdo.ime,
  lastName: userdo.prezime,
  username: userdo.username,
  bio: userdo.biografija,
  email: userdo.email,
  phone: userdo.brojTelefona,
  skills: userdo.vestine?.split(","),
  slikaUrl: userdo.slikaUrl
})

const editing = reactive({
  bio: false,
  email: false,
  phone: false,
  skills: false
})
const passwords = reactive({
  oldPassword: "",
  newPassword: "",
  confirmPassword: ""
});

const promeniSifru = async () => {
  if (!passwords.oldPassword || !passwords.newPassword) {
    alert("Popuni sva polja");
    return;
  }

  if (passwords.newPassword !== passwords.confirmPassword) {
    alert("Nova šifra i potvrda se ne poklapaju");
    return;
  }

  try {
    await api.put(`/korisnici/${userdo.id}/promeni-sifru`, {
      oldPassword: passwords.oldPassword,
      newPassword: passwords.newPassword
    });

    alert("Šifra je uspešno promenjena");

    passwords.oldPassword = "";
    passwords.newPassword = "";
    passwords.confirmPassword = "";
  } catch (err) {
    alert(err.response?.data?.message || "Greška pri promeni šifre");
  }
};
const newSkill = ref('')

function addSkill() {
  if (newSkill.value.trim()) {
    user.skills.push(newSkill.value)
    newSkill.value = ''
  }
}

function removeSkill(index) {
  user.skills.splice(index, 1)
}

const error = ref("")
const sacuvajPromene = async() =>{
  try{
    const res = await api.put("/korisnici/" + userdo.id, {
    "bio" : user.bio,
    "email" : user.email,
        "telefon" : user.phone,
        "vestine" : user.skills.join(", ")
      });
    izmenaDugme = false;
    count = false;

    localStorage.setItem("user", JSON.stringify(res.data.user));
    window.location.reload()
  }
    catch (err) {
    error.value =
      err.response?.data?.message || "Greska";
      alert(error.value)
  }
}
const avatarModal = ref(false)
const fileInput = ref(null)
const selectedFile = ref(null)
const avatarVersion = ref(0);

const openAvatarModal = () => {
  avatarModal.value = true
}

const closeAvatarModal = () => {
  avatarModal.value = false
}

const triggerFilePicker = () => {
  fileInput.value.click()
}
const avatarSrc = computed(() => {
  if (!user.slikaUrl) return "/ProfilPlaceholder.jpg";
  return `${BACKEND_URL}${user.slikaUrl}?t=${avatarVersion.value}`;
});

const onAvatarSelected = async (e) => {
  const file = e.target.files[0]
  if (!file) return

  selectedFile.value = file

  const formData = new FormData()
  formData.append("image", file)

  try {
    const res = await api.post("/Korisnici/profile-image",formData);

    // backend vrati SlikaUrl
    user.slikaUrl = res.data.slikaUrl
   // const storedUser = JSON.parse(localStorage.getItem("user") || "{}");
    //storedUser.slikaUrl = newUrl;
    const storedUser = JSON.parse(localStorage.getItem("user") || "{}");
    storedUser.slikaUrl = res.data.slikaUrl;
    localStorage.setItem("user", JSON.stringify(storedUser));
    
    avatarVersion.value++;
    window.dispatchEvent(new CustomEvent("user-updated", {detail: storedUser}));
    
    closeAvatarModal();

  } catch (err) {
    alert("Greška pri upload-u slike " +  {err})
  }

}
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
