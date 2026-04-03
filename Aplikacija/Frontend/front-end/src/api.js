import axios from 'axios';
export const BACKEND_URL = "https://localhost:7080";

const api = axios.create({
  baseURL: 'https://localhost:7080/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) {
    config.headers.Authorization = `Bearer ${token.trim()}`;
  }


   if (config.data instanceof FormData) {
    delete config.headers["Content-Type"];
  }

  return config;
});


export default api;