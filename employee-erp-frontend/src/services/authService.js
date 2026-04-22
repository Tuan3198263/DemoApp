import apiClient from './apiClient'
import { API_ENDPOINTS } from '../constants/api'
import { storage } from '../utils/storage'

const authService = {
  // Tam thoi fallback local neu backend chua san sang.
  async login(credentials) {
    try {
      const response = await apiClient.post(API_ENDPOINTS.AUTH_LOGIN, credentials)
      const token = response?.data?.token || 'demo-token'
      const user = response?.data?.user || {
        fullName: credentials.username,
        role: 'Admin',
      }

      storage.setToken(token)
      storage.setUser(user)

      return { token, user }
    } catch (error) {
      const token = 'demo-token'
      const user = { fullName: credentials.username, role: 'Admin' }
      storage.setToken(token)
      storage.setUser(user)
      return { token, user }
    }
  },

  logout() {
    storage.clearAuth()
  },

  getCurrentUser() {
    return storage.getUser()
  },
}

export default authService
