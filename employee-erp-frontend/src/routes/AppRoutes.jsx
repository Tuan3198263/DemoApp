import React from 'react'
import { Routes, Route, Navigate } from 'react-router-dom'
import { useAuth } from '../hooks/useAuth'

// Layouts
import MainLayout from '../layouts/MainLayout'
import AuthLayout from '../layouts/AuthLayout'
// Pages
import Dashboard from '../pages/Dashboard'
import EmployeeList from '../pages/Employee'
import Login from '../pages/Login'
import NotFound from '../pages/NotFound'

// Component bảo vệ Route - sử dụng useAuth từ AuthProvider
const PrivateRoute = ({ children }) => {
  const { isAuthenticated } = useAuth()
  return isAuthenticated ? children : <Navigate replace to="/login" />
}

const AppRoutes = () => {
  const { isAuthenticated } = useAuth()

  return (
    <Routes>
      {/* Group các trang yêu cầu login */}
      <Route
        element={
          <PrivateRoute>
            <MainLayout />
          </PrivateRoute>
        }
      >
        <Route path="/" element={<Navigate to="/dashboard" />} />
        <Route path="/dashboard" element={<Dashboard />} />
        <Route path="/employees" element={<EmployeeList />} />
      </Route>

      {/* Group các trang không yêu cầu login */}
      <Route element={<AuthLayout />}>
        <Route path="/login" element={isAuthenticated ? <Navigate to="/dashboard" replace /> : <Login />} />
      </Route>

      {/* Trang 404 */}
      <Route path="*" element={<NotFound />} />
    </Routes>
  )
}

export default AppRoutes