<template>
  <div class="chat-page">
    <header class="chat-header">
      <div class="header-left">
        <h1><i class="fas fa-comments"></i> Poruke</h1>
      </div>
      <div class="header-right">
        <div class="user-info">
          <span>{{ currentUser?.ime }} {{ currentUser?.prezime }}</span>
        </div>
      </div>
    </header>


    <div class="chat-container">
      <div class="chat-sidebar">
        <div class="sidebar-header">
          <div class="search-box">
            <i class="fas fa-search"></i>
            <input 
              v-model="searchQuery" 
              type="text" 
              placeholder="Pretraži chatove..."
              @input="filterChats"
            />
          </div>
        </div>
        
        <div class="chats-list">
          <div 
            v-for="chat in filteredChats" 
            :key="chat.id"
            :class="['chat-item', { 'active': activeChat?.id === chat.id }]"
            @click="selectChat(chat)"
          >
            <div class="chat-avatar">
              <img
              :src="getChatAvatar(chat)"
              class="avatar-img"
              alt="Avatar"
            />
             
            </div>
            <div class="chat-info">
              <div class="chat-name">
                {{ getChatName(chat) }}
              </div>
              <div class="chat-last-message">
                {{ getLastMessagePreview(chat) }}
              </div>
            </div>
            <div class="chat-meta">
              <div class="chat-time">
                {{ formatTime(chat.poslednjaPorukaVreme || chat.kreiran) }}
              </div>
              <div v-if="hasUnreadMessages(chat)" class="unread-badge"></div>
            </div>
          </div>
          
          <div v-if="loadingChats" class="loading-chats">
            <i class="fas fa-spinner fa-spin"></i> Učitavanje chatova...
          </div>
          
          <div v-if="!loadingChats && chats.length === 0" class="no-chats">
            <i class="fas fa-comments"></i>
            <p>Nemate aktivnih razgovora</p>
          </div>
        </div>
      </div>

      <div class="chat-area" v-if="activeChat">
        <div class="chat-area-header">
          <div class="chat-partner-info">
            <div class="partner-avatar">
              <img
                :src="getChatAvatar(activeChat)"
                class="avatar-img"
                alt="Avatar"
              />
            </div>
            <div>
                <router-link :to="{ name: 'Profile', params: { username: getChatName(activeChat) } }">
                    <h3>{{ getChatName(activeChat) }}</h3>
                </router-link>
              <p class="oglas-info">Oglas: {{ activeChat.oglasNaziv }}</p>
            </div>
          </div>
          <button class="close-chat" @click="activeChat = null" title="Zatvori chat">
            <i class="fas fa-times"></i>
          </button>
        </div>

        <div class="messages-container" ref="messagesContainer">
          <div v-if="loadingMessages" class="loading-messages">
            <i class="fas fa-spinner fa-spin"></i> Učitavanje poruka...
          </div>
          
          <div v-else-if="messages.length === 0" class="no-messages">
            <p>Pošaljite prvu poruku</p>
          </div>
          
          <div v-else class="messages-list">
            <div 
              v-for="message in messages" 
              :key="message.id"
              :class="['message', { 'sent': message.posiljalacId === currentUser?.id }]"
            >
              <div class="message-content">
                <div class="message-sender">{{ message.posiljalacUsername }}</div>
                <div class="message-text">{{ message.tekst }}</div>
                <div class="message-time">{{ formatMessageTime(message.vremeSlanja) }}</div>
              </div>
            </div>
          </div>
        </div>

        <div class="message-input-container">
          <div class="input-wrapper">
            <input 
              v-model="newMessage" 
              @keyup.enter="sendMessage"
              :placeholder="`Pišite poruku ${getOtherUserName(activeChat)}...`"
              :disabled="!activeChat"
            />
            <button 
              @click="sendMessage" 
              :disabled="!newMessage.trim() || !activeChat"
              class="send-button"
            >
              <i class="fas fa-paper-plane"></i>
            </button>
          </div>
        </div>
      </div>
      <div class="welcome-screen" v-else>
        <div class="welcome-content">
          <i class="fas fa-comments fa-4x"></i>
          <h2>Dobrodošli u Chat</h2>
          <p>Izaberite razgovor sa liste da biste počeli da pričate</p>
          <p class="hint">Chat se automatski kreira kada neko prihvati vašu prijavu</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted, nextTick } from 'vue'
import { useRouter } from 'vue-router'
import api, { BACKEND_URL } from "@/api";
import { getUser } from '@/auth'
import { HubConnectionBuilder, HttpTransportType } from '@microsoft/signalr'

const router = useRouter()
const currentUser = getUser()


const chats = ref([])
const activeChat = ref(null)
const messages = ref([])
const newMessage = ref('')
const searchQuery = ref('')
const loadingChats = ref(false)
const loadingMessages = ref(false)
const connection = ref(null)
const messagesContainer = ref(null)

const filteredChats = computed(() => {
  if (!searchQuery.value.trim()) return chats.value
  
  return chats.value.filter(chat => {
    const chatName = getChatName(chat).toLowerCase()
    const lastMessage = (chat.poslednjaPoruka || '').toLowerCase()
    const search = searchQuery.value.toLowerCase()
    
    return chatName.includes(search) || lastMessage.includes(search)
  })
})

onMounted(async () => {
  if (!currentUser) {
    router.push('/login')
    return
  }
  
  await loadChats()
  await connectToSignalR()
})

onUnmounted(() => {
  disconnectSignalR()
})

function getChatAvatar(chat) {
  let imageUrl = null

  if (chat.klijentId === currentUser.id) {
    // KLIJENT → druga strana je OGLAŠIVAČ
    imageUrl = chat.oglasivacSlikaUrl
  } else {
    // OGLAŠIVAČ → druga strana je KLIJENT
    imageUrl = chat.klijentSlikaUrl
  }

  if (!imageUrl) {
    return '/ProfilPlaceholder.jpg'
  }

  return `${BACKEND_URL}${imageUrl}`
}
async function loadChats() {
  loadingChats.value = true
  try {
    const response = await api.get('/chat/my')
    chats.value = response.data
    console.log('CHAT:', chats.value[0])
    chats.value.sort((a, b) => {
      const timeA = a.poslednjaPorukaVreme ? new Date(a.poslednjaPorukaVreme).getTime() : 0
      const timeB = b.poslednjaPorukaVreme ? new Date(b.poslednjaPorukaVreme).getTime() : 0
      return timeB - timeA
    })
  } catch (error) {
    console.error('Error loading chats:', error)
    showError('Greška pri učitavanju chatova')
  } finally {
    loadingChats.value = false
  }
}

async function loadMessages(chatId) {
  loadingMessages.value = true
  messages.value = []
  
  try {
    const response = await api.get(`/chat/${chatId}/messages`)
    messages.value = response.data.poruke
    scrollToBottom()
  } catch (error) {
    console.error('Error loading messages:', error)
    showError('Greška pri učitavanju poruka')
  } finally {
    loadingMessages.value = false
  }
}

async function selectChat(chat) {
  activeChat.value = chat
  await loadMessages(chat.id)
}

async function sendMessage() {
  if (!newMessage.value.trim() || !activeChat.value) return
  
  const messageText = newMessage.value.trim()
  const chatId = activeChat.value.id
  
  try {

    if (connection.value && connection.value.state === 'Connected') {
      await connection.value.invoke('SendMessage', chatId, messageText)
    } else {

      const response = await api.post(`/chat/${chatId}/message`, {
        message: messageText
      })
      

      const newMsg = response.data
      messages.value.push(newMsg)
      
      updateChatLastMessage(chatId, messageText, new Date().toISOString())
    }
    
    newMessage.value = ''
    scrollToBottom()
  } catch (error) {
    console.error('Error sending message:', error)
    showError('Greška pri slanju poruke')
  }
}


async function connectToSignalR() {
  const token = localStorage.getItem('token')
  
  if (!token) {
    console.warn('No token found for SignalR connection')
    return
  }
  
  try {
    connection.value = new HubConnectionBuilder()
      .withUrl('https://localhost:7080/chatHub', {
        accessTokenFactory: () => token,
        transport: HttpTransportType.WebSockets | HttpTransportType.ServerSentEvents | HttpTransportType.LongPolling
      })
      .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
      .build()
    

    connection.value.on('ReceiveMessage', (message) => {
      console.log('New message received:', message)
      if (activeChat.value && activeChat.value.id === message.chatId) {
        messages.value.push(message)
        scrollToBottom()
      }
      updateChatLastMessage(message.chatId, message.tekst, message.vremeSlanja)
    })
    
    connection.value.on('ChatCreated', (chat) => {
      console.log('New chat created:', chat)
      chats.value.unshift(chat)
      showNotification(`Novi chat kreiran: ${getChatName(chat)}`)
    })
    
    connection.value.on('Error', (error) => {
      console.error('SignalR error:', error)
    })
    
    await connection.value.start()
    console.log('SignalR connected successfully')
    
  } catch (err) {
    console.error('SignalR connection failed:', err)

  }
}

function disconnectSignalR() {
  if (connection.value) {
    connection.value.stop()
  }
}


function getChatName(chat) {
  if (chat.klijentId === currentUser.id) {
    return chat.oglasivacUsername
  } else {
    return chat.klijentUsername
  }
}

function getOtherUserName(chat) {
  if (!chat) return ''
  if (chat.klijentId === currentUser.id) {
    return chat.oglasivacUsername
  } else {
    return chat.klijentUsername
  }
}

function getLastMessagePreview(chat) {
  if (!chat.poslednjaPoruka) return 'Nema poruka'
  
  const prefix = chat.poslednjaPorukaPosiljalac === currentUser.username ? 'Ti: ' : ''
  const message = chat.poslednjaPoruka
  
  if (message.length > 30) {
    return prefix + message.substring(0, 30) + '...'
  }
  return prefix + message
}

function hasUnreadMessages(chat) {
  return false
}

function formatTime(dateTime) {
  if (!dateTime) return ''
  
  const date = new Date(dateTime)
  const now = new Date()
  const diffMs = now - date
  const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24))
  
  if (diffDays === 0) {
    return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
  } else if (diffDays === 1) {
    return 'Juče'
  } else if (diffDays < 7) {
    return date.toLocaleDateString([], { weekday: 'short' })
  } else {
    return date.toLocaleDateString([], { day: '2-digit', month: '2-digit' })
  }
}

function formatMessageTime(dateTime) {
  const date = new Date(dateTime)
  return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
}

function updateChatLastMessage(chatId, message, timestamp) {
  const chatIndex = chats.value.findIndex(c => c.id === chatId)
  if (chatIndex !== -1) {
    const chat = chats.value[chatIndex]
    chat.poslednjaPoruka = message
    chat.poslednjaPorukaVreme = timestamp
    chat.poslednjaPorukaPosiljalac = currentUser.username
    
    const updatedChat = chats.value.splice(chatIndex, 1)[0]
    chats.value.unshift(updatedChat)
  }
}

function scrollToBottom() {
  nextTick(() => {
    if (messagesContainer.value) {
      messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight
    }
  })
}

function showError(message) {
  alert(message)
}

function showNotification(message) {

  console.log('Notification:', message)
}

function filterChats() {

}
</script>

<style scoped>
* {
  box-sizing: border-box;
}


.chat-page {
  height: 100vh;
  display: flex;
  flex-direction: column;
  background: #f5f7fb;
}

.chat-container {
  display: flex;
  flex: 1;
  min-height: 0; 
  overflow: hidden;
}

.chat-sidebar {
  width: 350px;
  background: white;
  border-right: 1px solid #e0e0e0;
  display: flex;
  flex-direction: column;
  min-height: 0; 
}

.chats-list {
  flex: 1;
  overflow-y: auto;
  min-height: 0; 
}

.chat-area {
  flex: 1;
  display: flex;
  flex-direction: column;
  background: white;
  min-height: 0; 
}

.messages-container {
  flex: 1;
  padding: 1.5rem;
  overflow-y: auto;
  background: #f8f9fa;
  min-height: 0;
}

.debug-border {
  border: 2px solid red !important;
}
.chat-page {
  height: 100vh;
  display: flex;
  flex-direction: column;
  background: #f5f7fb;
}

.chat-header {
  background: white;
  padding: 1rem 2rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
  z-index: 10;
}

.header-left h1 {
  margin: 0;
  color: #333;
  font-size: 1.5rem;
}

.header-left i {
  margin-right: 10px;
  color: var(--primary);
}

.user-info {
  background: var(--primary);
  color: white;
  padding: 0.5rem 1rem;
  border-radius: 20px;
  font-weight: 500;
}

.chat-container {
  display: flex;
  flex: 1;
  overflow: hidden;
}

/* Sidebar */
.chat-sidebar {
  width: 350px;
  background: white;
  border-right: 1px solid #e0e0e0;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.sidebar-header {
  padding: 1rem;
  border-bottom: 1px solid #e0e0e0;
}

.search-box {
  position: relative;
  display: flex;
  align-items: center;
}

.search-box i {
  position: absolute;
  left: 12px;
  color: #999;
}

.search-box input {
  width: 100%;
  padding: 0.75rem 1rem 0.75rem 2.5rem;
  border: 1px solid #ddd;
  border-radius: 8px;
  font-size: 0.9rem;
  outline: none;
}

.search-box input:focus {
  border-color: var(--primary);
}

.chats-list {
  flex: 1;
  overflow-y: auto;
}

.chat-item {
  display: flex;
  align-items: center;
  padding: 1rem;
  border-bottom: 1px solid #f0f0f0;
  cursor: pointer;
  transition: background-color 0.2s;
}

.chat-item:hover {
  background-color: #f8f9fa;
}

.chat-item.active {
  background-color: #e8f4fd;
  border-left: 4px solid var(--primary);
}

.chat-avatar {
  width: 50px;
  height: 50px;
  background: #e9ecef;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-right: 1rem;
  font-size: 1.5rem;
  color: #6c757d;
  overflow: hidden;
}


.chat-info {
  flex: 1;
  min-width: 0;
}

.chat-name {
  font-weight: 600;
  margin-bottom: 0.25rem;
  color: #333;
}

.chat-last-message {
  font-size: 0.85rem;
  color: #666;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.chat-meta {
  text-align: right;
}

.chat-time {
  font-size: 0.75rem;
  color: #999;
  margin-bottom: 0.25rem;
}

.unread-badge {
  width: 10px;
  height: 10px;
  background-color: var(--primary);
  border-radius: 50%;
  display: inline-block;
}

.loading-chats, .no-chats {
  text-align: center;
  padding: 2rem;
  color: #666;
}

.no-chats i {
  font-size: 3rem;
  margin-bottom: 1rem;
  color: #ddd;
}

/* Chat Area */
.chat-area {
  flex: 1;
  display: flex;
  flex-direction: column;
  background: white;
}

.welcome-screen {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  background: white;
}

.welcome-content {
  text-align: center;
  color: #666;
}

.welcome-content i {
  color: #ddd;
  margin-bottom: 1rem;
}

.welcome-content h2 {
  color: #333;
  margin-bottom: 0.5rem;
}

.hint {
  font-size: 0.9rem;
  color: #999;
  margin-top: 1rem;
  font-style: italic;
}

.chat-area-header {
  padding: 1rem 1.5rem;
  border-bottom: 1px solid #e0e0e0;
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: white;
}

.chat-partner-info {
  display: flex;
  align-items: center;
}

.partner-avatar {
  width: 40px;
  height: 40px;
  background: #e9ecef;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-right: 1rem;
  font-size: 1.2rem;
  color: #6c757d;
  overflow: hidden;
}

.chat-partner-info h3 {
  margin: 0;
  font-size: 1.1rem;
}

.oglas-info {
  margin: 0;
  font-size: 0.85rem;
  color: #666;
}

.close-chat {
  background: none;
  border: none;
  font-size: 1.2rem;
  color: #999;
  cursor: pointer;
  padding: 0.5rem;
  border-radius: 50%;
}

.close-chat:hover {
  background: #f5f5f5;
  color: #333;
}

.messages-container {
  flex: 1;
  padding: 1.5rem;
  overflow-y: auto;
  background: #f8f9fa;
}

.loading-messages, .no-messages {
  text-align: center;
  padding: 2rem;
  color: #666;
}

.messages-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.message {
  display: flex;
  max-width: 70%;
}

.message.sent {
  align-self: flex-end;
}

.message:not(.sent) {
  align-self: flex-start;
}

.message-content {
  padding: 0.75rem 1rem;
  border-radius: 18px;
  position: relative;
}

.message.sent .message-content {
  background: var(--primary);
  color: white;
  border-bottom-right-radius: 4px;
}

.message:not(.sent) .message-content {
  background: white;
  color: #333;
  border-bottom-left-radius: 4px;
  box-shadow: 0 1px 2px rgba(0,0,0,0.1);
}

.message-sender {
  font-weight: 600;
  font-size: 0.85rem;
  margin-bottom: 0.25rem;
}

.message.sent .message-sender {
  color: rgba(255,255,255,0.9);
}

.message:not(.sent) .message-sender {
  color: #666;
}

.message-text {
  word-break: break-word;
  line-height: 1.4;
}

.message-time {
  font-size: 0.75rem;
  text-align: right;
  margin-top: 0.25rem;
  opacity: 0.8;
}

/* Message Input */
.message-input-container {
  padding: 1rem 1.5rem;
  border-top: 1px solid #e0e0e0;
  background: white;
}

.input-wrapper {
  display: flex;
  gap: 0.5rem;
}

.input-wrapper input {
  flex: 1;
  padding: 0.75rem 1rem;
  border: 1px solid #ddd;
  border-radius: 24px;
  font-size: 0.95rem;
  outline: none;
  transition: border-color 0.2s;
}

.input-wrapper input:focus {
  border-color: var(--primary);
}

.input-wrapper input:disabled {
  background: #f5f5f5;
  cursor: not-allowed;
}

.send-button {
  width: 48px;
  height: 48px;
  background: var(--primary);
  color: white;
  border: none;
  border-radius: 50%;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.1rem;
  transition: background-color 0.2s;
}

.send-button:hover:not(:disabled) {
  background: var(--primary-dark);
}

.send-button:disabled {
  background: #ccc;
  cursor: not-allowed;
}

/* Responsive */
@media (max-width: 768px) {
  .chat-sidebar {
    width: 100%;
  }
  
  .chat-container {
    flex-direction: column;
  }
  
  .chat-area {
    display: none;
  }
  
  .chat-area.active {
    display: flex;
  }
  
  .message {
    max-width: 85%;
  }
}

.avatar-img {
  width: 100%;
  height: 100%;
  border-radius: 50%;
  object-fit: cover;
  border: 1px solid var(--base-300);
}
</style>
