import { createContext, useContext, useMemo, useState } from 'react'
import authService from '../services/authService'
import { storage } from '../utils/storage'

const AuthContext = createContext(null)

export function AuthProvider({ children }) {
  const [user, setUser] = useState(storage.getUser());
  const [token, setToken] = useState(storage.getToken());

  const login = async (credentials) => {
    // 1. Chỉ lấy dữ liệu từ service, không để service tự setStorage
    const data = await authService.login(credentials);
    
    // 2. Cập nhật đồng thời cả Storage và State tại đây
    storage.setToken(data.accessToken); // Lưu ý: Backend bạn trả về accessToken
    storage.setUser(data.user);
    
    setToken(data.accessToken);
    setUser(data.user);
    
    return data;
  };

  const logout = () => {
    authService.logout();
    storage.clearAuth(); // Đảm bảo dọn sạch storage
    setUser(null);
    setToken(null);
  };

  const value = useMemo(
    () => ({
      user,
      token,
      isAuthenticated: !!token, // Check trực tiếp dựa trên state token
      login,
      logout,
    }),
    [user, token]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error('useAuth phai duoc su dung ben trong AuthProvider')
  }

  return context
}
