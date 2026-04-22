
import apiClient from './apiClient'
import { API_ENDPOINTS } from '../constants/api'
import { storage } from '../utils/storage'

const authService = {
  async login(credentials) {
    // data ở đây chính là object { success, data, message, errors } trả về từ server
    const result = await apiClient.post(API_ENDPOINTS.AUTH_LOGIN, {
      userName: credentials.username, // Chú ý: Curl dùng userName (N viết hoa)
      password: credentials.password
    });

    if (result.success && result.data) {
      const { accessToken, user } = result.data;
      
      storage.setToken(accessToken); // Lưu accessToken
      storage.setUser(user);         // Lưu thông tin user
      
      return result.data;
    } else {
      throw new Error(result.message || "Đăng nhập thất bại");
    }
  },

  logout() {
    storage.clearAuth();
  }
}

export default authService;