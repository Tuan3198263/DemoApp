import { NavLink } from 'react-router-dom'
import { ROUTES } from '../constants/routes'

const menuItems = [
  { to: ROUTES.DASHBOARD, icon: 'bi-speedometer2', label: 'Trang chủ' },
  { to: ROUTES.EMPLOYEES, icon: 'bi-people', label: 'Nhân viên' },
]

export default function Sidebar({ collapsed, onToggle }) {
  return (
    <aside className={`sidebar border-end bg-white ${collapsed ? 'collapsed' : ''}`}>
      <div className="p-3 border-bottom d-flex align-items-center justify-content-between">
        <strong className="brand-name">Quản lý nhân viên</strong>
        <button className="btn btn-sm btn-outline-secondary d-none d-lg-inline" onClick={onToggle}>
          <i className="bi bi-layout-sidebar"></i>
        </button>
      </div>

      <nav className="nav flex-column p-2 gap-1">
        {menuItems.map((item) => (
          <NavLink
            key={item.to}
            to={item.to}
            className={({ isActive }) =>
              `nav-link rounded-3 d-flex align-items-center gap-2 ${isActive ? 'active bg-primary text-white' : 'text-dark'}`
            }
          >
            <i className={`bi ${item.icon}`}></i>
            <span>{item.label}</span>
          </NavLink>
        ))}
      </nav>
    </aside>
  )
}
