<template>
    <div class="main-container">

        <div class="login-container">
            <form id="login-form" @submit.prevent="Login">
                <h2 class="">Login</h2>
                <div class="input-group">
                    <label for="username">Username</label>
                    <input type="text" id="username" placeholder="Enter username">
                </div>
                <div class="input-group">
                    <label for="password">Password</label>
                    <input type="password" id="password" placeholder="Enter password">
                </div>
                <div class="error" v-if="error">{{ error }}</div>
                <button type="submit" @click="Login()">Login</button>
                
                <p class="register-text">
                    Nemate nalog? <router-link to="/register">Registrujte se</router-link>
                </p>
            </form>
        </div>
    </div>
</template>
<script setup>
    import { ref } from 'vue';
    import { useRoute, useRouter } from 'vue-router';
    import "leaflet/dist/leaflet.css";
    import api from '@/api';
    const error = ref("");
const router = useRouter();
const Login = async () =>{
    const username = document.getElementById("username").value
    const password = document.getElementById("password").value
  try {
    const res = await api.post("/auth/login", {
      "username" : username,
      "password" : password
    });

    localStorage.setItem("token", res.data.token);

    localStorage.setItem("user", JSON.stringify(res.data.user));

    router.push("/");
    window.location.reload() 
    router.push("/");

    //console.log(res.data.token)

  } catch (err) {
    error.value =
      err.response?.data?.message || "Unesite korisničko ime i lozinku";
  }
}

</script>
<style scoped>
* {
    box-sizing: border-box;
    font-family: Arial, sans-serif;
}

.main-container {
    height: 100vh;
    margin: auto;
    background: linear-gradient(135deg, var(--primary),  var(--secondary));
    display: flex;
    justify-content: center;
    align-items: center;
}

.login-container {
    background: white;
    padding: 30px;
    width: 520px;
    border-radius: 10px;
    box-shadow: 0 10px 25px rgba(0,0,0,0.2);
}

.login-container h2 {
    text-align: center;
    margin-bottom: 20px;
}

.input-group {
    margin-bottom: 15px;
}

.input-group label {
    display: block;
    margin-bottom: 5px;
    font-size: 14px;
}

.input-group input {
    width: 100%;
    padding: 10px;
    border-radius: 6px;
    border: 1px solid #ccc;
}

button {
    width: 100%;
    padding: 10px;
    background: #667eea;
    color: white;
    border: none;
    border-radius: 6px;
    cursor: pointer;
    font-size: 16px;
}

button:hover {
    background: #5a67d8;
}

.register-text {
    margin-top: 15px;
    text-align: center;
    font-size: 14px;
}

.display-none{
    display: none;
}
.display-block{
    display: block;
}
.error{
  color: var(--danger);
  background-color: #fee2e2; 
  border: 1px solid var(--danger);
  padding: 0.5rem;
  border-radius: 0.375rem; 
  margin-top: 0.5rem;
  margin-bottom: 0.5rem;
}
</style>