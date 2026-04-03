<script setup>
import { RouterView } from 'vue-router'
import TheNavbar from './components/TheNavbar.vue' // Prilagodi putanju ako je drugacija
</script>

<template>
  <div class="app-layout">
    
    <TheNavbar />

    <main class="main-content">
      <RouterView />
    </main>

    </div>
</template>

<style>
/* Globalni reset ako nije vec u design.css */
html, body {
  margin: 0;
  padding: 0;
  background-color: var(--base-50);
}

.app-layout {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.main-content {
  flex: 1; /* Ovo gura footer na dno ako ga imas */
  /* Padding top nije potreban jer je navbar sticky, 
     ali ako promenis navbar u fixed, trebao bi ti padding-top: 70px */
}
</style>