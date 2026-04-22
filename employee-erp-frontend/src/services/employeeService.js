import apiClient from './apiClient'
import { API_ENDPOINTS } from '../constants/api'

const employeeService = {
  /**
   * Lấy danh sách nhân viên có phân trang và lọc
   * @param {Object} params - Các tham số từ Swagger (page, pageSize, keyword, gioiTinh, boPhan, sortBy, descending)
   */
  async getEmployees(params) {
    // apiClient đã có interceptor tự gắn Token
    // và đã bóc tách response.data ở bản nâng cấp trước đó
    const result = await apiClient.get(API_ENDPOINTS.EMPLOYEES, { params });
    
    // Theo cấu trúc JSON BE trả về: { success, data: [], pagination: {}, ... }
    return result; 
  },
}

export default employeeService