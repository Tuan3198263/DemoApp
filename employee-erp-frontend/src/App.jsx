import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import PrivateRoute from './components/PrivateRoute'
import { ROUTES } from './constants/routes'
import { AuthProvider } from './hooks/useAuth'
import MainLayout from './layouts/MainLayout'
import Dashboard from './pages/Dashboard'
import EmployeeDetail from './pages/EmployeeDetail'
import EmployeeList from './pages/EmployeeList'
import Login from './pages/Login'
import Settings from './pages/Settings'

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path={ROUTES.LOGIN} element={<Login />} />

          {/* Bao ve toan bo route noi bo bang PrivateRoute. */}
          <Route
            element={
              <PrivateRoute>
                <MainLayout />
              </PrivateRoute>
            }
          >
            <Route path={ROUTES.DASHBOARD} element={<Dashboard />} />
            <Route path={ROUTES.EMPLOYEES} element={<EmployeeList />} />
            <Route path={ROUTES.EMPLOYEE_DETAIL} element={<EmployeeDetail />} />
            <Route path={ROUTES.SETTINGS} element={<Settings />} />
          </Route>

          <Route path="*" element={<Navigate to={ROUTES.DASHBOARD} replace />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  )
}

export default App
