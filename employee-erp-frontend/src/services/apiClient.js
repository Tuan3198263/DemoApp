import axios from 'axios'
import { API_BASE_URL } from '../constants/api'
import { storage } from '../utils/storage'

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 15000,
})

// Tu dong gan token cho moi request neu da dang nhap.
apiClient.interceptors.request.use((config) => {
  const token = storage.getToken()
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }

  return config
})

export default apiClient
