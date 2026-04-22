import axios from 'axios';
import { API_BASE_URL } from '../constants/api';
import { storage } from '../utils/storage';

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: 15000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// 1. Request Interceptor: Tự động đính kèm Token
apiClient.interceptors.request.use(
  (config) => {
    const token = storage.getToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// 2. Response Interceptor: Xử lý dữ liệu và lỗi tập trung
apiClient.interceptors.response.use(
  (response) => {
    // Trả về data trực tiếp giúp code ở Service ngắn gọn hơn
    // Ví dụ: thay vì response.data.user, bạn chỉ cần dùng data.user
    return response.data;
  },
  (error) => {
    // Xử lý lỗi HTTP
    if (error.response) {
      const { status } = error.response;

      // Token hết hạn hoặc không hợp lệ
      if (status === 401) {
        console.error('Phiên đăng nhập hết hạn!');
        storage.removeToken();
        storage.removeUser();
        // Redirect về login nếu không phải đang ở trang login
        if (window.location.pathname !== '/login') {
          window.location.href = '/login';
        }
      }

      // Không có quyền truy cập
      if (status === 403) {
        console.error('Bạn không có quyền thực hiện hành động này');
      }

      // Lỗi hệ thống (BE crash)
      if (status >= 500) {
        console.error('Lỗi hệ thống, vui lòng thử lại sau');
      }
    } else if (error.request) {
      // Lỗi không nhận được phản hồi (Server chết hoặc sai cổng 5001)
      console.error('Không thể kết nối đến máy chủ. Kiểm tra lại Backend!');
    }

    return Promise.reject(error);
  }
);

export default apiClient;