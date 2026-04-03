import { createRouter, createWebHistory } from 'vue-router'
import MainPage from '../views/MainPage.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: MainPage,
    },
    {
    path: '/Login',
    name: 'Login',
    component: () => import('../views/Login.vue'),
    },
    {
    path: "/profile/:username",
    name: "Profile",
    component: () => import('../views/Profile.vue'),
    meta: { requiresAuth: true }
    },
    {
    path: "/MojiOglasi",
    name: "MojiOglasi",
    component: () => import('../views/MojiOglasi.vue'),
    meta: { requiresAuth: true }
    },
    {
    path: '/Me',
    name: 'Me',
    component: () => import('../views/MyProfile.vue'),
    meta: { requiresAuth: true } 
    },
    {
    path: '/Chat',
    name: 'Chat',
    component: () => import('../views/Chat.vue'),
    meta: { requiresAuth: true } 
    },
    {
    path: '/Register',
    name: 'Register',
    component: () => import('../views/Register.vue'),
    },
        {
    path: '/PostaviOglas',
    name: 'PostavljanjeOglasa',
    component: () => import('../views/PostavljanjeOglasa.vue'),
    meta: { requiresAuth: true } 
    },
    {
    path: '/MojePrijave',
    name: 'MojePrijave',
    component: () => import('../views/MojePrijave.vue'),
    meta: { requiresAuth: true } 
    },
    {
      path: '/mainPage',
      name: 'Main Page',
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import('../views/MainPage.vue'),
    },
  ],
})

router.beforeEach((to, from, next) => {
  const token = localStorage.getItem("token");

  if (to.meta.requiresAuth && !token) {
    next("/login"); 
  } else {
    next(); 
  }
});
router.beforeEach((to, from, next) => {
  const token = localStorage.getItem("token");
  
  if ((to.path === "/login" || to.path === "/register") && token) {
    next("/");
  } 
  else if (to.meta.requiresAuth && !token) {
    next("/login");
  } 
  else {
    next();
  }
});


export default router
