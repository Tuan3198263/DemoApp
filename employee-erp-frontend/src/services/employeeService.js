import apiClient from './apiClient'
import { API_ENDPOINTS } from '../constants/api'

const employeeService = {
  async getEmployees() {
    const response = await apiClient.get(API_ENDPOINTS.EMPLOYEES)
    return response?.data || []
  },
}

export default employeeService
