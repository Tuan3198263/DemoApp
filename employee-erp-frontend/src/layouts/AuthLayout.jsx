import React from 'react'
import { Outlet } from 'react-router-dom'

const AuthLayout = () => {
  return (
    <div className="auth-container bg-light min-vh-100 d-flex align-items-center justify-content-center">
      <div className="auth-card shadow-lg bg-white rounded-4" style={{ width: '100%', maxWidth: '450px' }}>
        <div className="text-center mb-5 pt-5">
          <div className="mb-3">
            <i className="bi bi-building text-primary" style={{ fontSize: '3rem' }}></i>
          </div>
          <h2 className="fw-bold text-dark mb-2">Đăng nhập</h2>
         
        </div>

        {/* Nơi render trang Login.jsx */}
        <div className="px-4 px-md-5 pb-5">
          <Outlet />
        </div>

        <div className="text-center py-3 border-top">
          <small className="text-muted">&copy; 2026 ERP Management System</small>
        </div>
      </div>
    </div>
  )
}

export default AuthLayout