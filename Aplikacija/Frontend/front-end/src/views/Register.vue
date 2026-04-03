<template>
        <div class="main-container">
                <div class="login-container">
                <form @submit.prevent="Register" >
                    <h2 class="">Register</h2>
                    
                    <div class="input-group">
                        <label for="name">Name</label>
                        <input type="text" id="name" placeholder="Enter name">
                    </div>
                    <div class="input-group">
                        <label for="surname">Surname</label>
                        <input type="text" id="surname" placeholder="Enter surname">
                    </div>
                    <div class="input-group">
                        <label for="username">Username</label>
                        <input type="text" id="username" placeholder="Enter username">
                    </div>
                    
                    <div class="input-group">
                        <label for="email">Email</label>
                        <input type="text" id="email" placeholder="Enter email">
                    </div>
                    <div class="input-group">
                        <label for="telefon">Telephone</label>
                        <input type="text" id="telefon" placeholder="Enter telephone">
                    </div>
                    <div class="input-group">
                        <label for="telefon">Skills (Odvojiti zarezom)</label>
                        <input type="text" id="skills" placeholder="Enter skill1, skill2, skill3...">
                    </div>
                    <div class="input-group">
                        <label for="password">Password</label>
                <input type="password" id="password" placeholder="Enter password">
            </div>
            
            <div class="input-group">
                <label for="confirm">Confirm Password</label>
                <input type="password" id="confirm" placeholder="Confirm password">
            </div>
            
            <button type="submit">Register</button>
            
            <p class="register-text">
                Već imate nalog? <router-link to="/login">Ulogujte se</router-link>
            </p>
        </form>
    </div>
    </div>
</template>
<script setup>
    import { ref, onMounted, watch } from 'vue';
    import { useRoute, useRouter } from 'vue-router';
    import "leaflet/dist/leaflet.css";
    import api from '@/api';
    import router from '@/router';
    
const route = useRouter();

const Register = async () => {
    const name = document.getElementById("name").value
    const surname = document.getElementById("surname").value
    const usernamee = document.getElementById("username").value
    const emaill = document.getElementById("email").value
    const passwordd = document.getElementById("password").value
    const confirmPassword = document.getElementById("confirm").value
    const telefon = document.getElementById("telefon").value
    const skills = document.getElementById("skills").value
    if(passwordd != confirmPassword){
        alert("Pogresno uneta sifra")
        return
    }

    try {
        const res = await api.post(
            "/auth/register",
            {
                "ime": name,
                "prezime": surname,
                "username": usernamee,
                "email": emaill,
                "password": passwordd,
                "biografija": "prazno",
                "slikaurl" : "C:\Users\jakov\Pictures\Screenshots\Screenshot 2025-11-30 174334.png",//TODO slika da se posalje na server
                "telefon" : telefon,
                "vestinejson" : JSON.stringify(skills.split(","))
            }
        );
        //localStorage.setItem("token", res.data.token);
        //console.log("Token:", res.data.token);
        route.push("/login")
    } catch (err) {
        console.log("Backend error:", err.response?.data);
    alert(
        err.response?.data?.message ||
        JSON.stringify(err.response?.data) ||
        err.message
    );
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
</style>