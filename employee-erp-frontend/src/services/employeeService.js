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
  async addEmployees(data) {
    // [cite: 32, 294]
    // BE yêu cầu format: { employees: [ { maNhanVien, ... }, ... ] }
    const payload = {
      employees: data
    };
    return await apiClient.post(`${API_ENDPOINTS.EMPLOYEES}/batch`, payload);
  },

  /**
   * Cập nhật thông tin nhân viên
   * id: ID của nhân viên (path parameter)
   * data: Object thông tin (tenNhanVien, ngaySinh, gioiTinh, boPhan, mucLuong)
   */
  async updateEmployee(id, data) {
    // [cite: 234, 295]
    // Swagger: PUT /api/employees/{id}
    return await apiClient.put(`${API_ENDPOINTS.EMPLOYEES}/${id}`, data);
  },

  /**
   * Xóa nhân viên (Xóa một hoặc nhiều)
   * ids: mảng các ID [id1, id2, ...]
   */
  async deleteEmployees(ids) {
    // [cite: 28, 79, 296]
    // Swagger: DELETE /api/employees nhận Request Body { employeeIds: [0] }
    return await apiClient.delete(API_ENDPOINTS.EMPLOYEES, {
      data: { employeeIds: ids }
    });
  }
}

export default employeeService