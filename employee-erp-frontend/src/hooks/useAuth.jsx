import { createContext, useContext, useMemo, useState } from 'react'
import authService from '../services/authService'
import { storage } from '../utils/storage'

const AuthContext = createContext(null)

export function AuthProvider({ children }) {
  const [user, setUser] = useState(storage.getUser())
  const [token, setToken] = useState(storage.getToken())

  const login = async (credentials) => {
    const data = await authService.login(credentials)
    setUser(data.user)
    setToken(data.token)
    return data
  }

  const logout = () => {
    authService.logout()
    setUser(null)
    setToken(null)
  }

  const value = useMemo(
    () => ({
      user,
      token,
      isAuthenticated: Boolean(token),
      login,
      logout,
    }),
    [user, token],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth() {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error('useAuth phai duoc su dung ben trong AuthProvider')
  }

  return context
}
