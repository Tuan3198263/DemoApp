const TOKEN_KEY = 'erp_token'
const USER_KEY = 'erp_user'

export const storage = {
  // Lấy token
  getToken: () => localStorage.getItem('accessToken'),
  
  // Lưu token (Hàm này đang bị báo lỗi nếu bạn đặt tên khác)
  setToken: (token) => localStorage.setItem('accessToken', token),
  
  // Xóa token
  removeToken: () => localStorage.removeItem('accessToken'),

  // Quản lý User
  getUser: () => JSON.parse(localStorage.getItem('user') || '{}'),
  setUser: (user) => localStorage.setItem('user', JSON.stringify(user)),
  removeUser: () => localStorage.removeItem('user'),

  // Xóa sạch khi Logout
  clearAuth: () => {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('user');
  }
};
