import { useAuth } from '../hooks/useAuth'

export default function Topbar({ onOpenMobileMenu }) {
  const { user, logout } = useAuth()

  return (
    <header className="topbar border-bottom bg-white px-3 py-2 d-flex align-items-center justify-content-between">
      <div className="d-flex align-items-center gap-2">
        <button className="btn btn-outline-secondary d-lg-none" onClick={onOpenMobileMenu}>
          <i className="bi bi-list"></i>
        </button>
        
      </div>

      <div className="d-flex align-items-center gap-3">
        <button className="btn btn-light position-relative">
          <i className="bi bi-envelope"></i>
          <span className="badge text-bg-danger rounded-pill icon-badge">3</span>
        </button>

        <button className="btn btn-light position-relative">
          <i className="bi bi-bell"></i>
          <span className="badge text-bg-warning rounded-pill icon-badge">7</span>
        </button>

        <div className="dropdown">
          <button className="btn btn-light dropdown-toggle" data-bs-toggle="dropdown">
            <i className="bi bi-person-circle me-1"></i>
            {user?.fullName || 'User'}
          </button>
          <ul className="dropdown-menu dropdown-menu-end">
            <li>
              <span className="dropdown-item-text small text-body-secondary">{user?.role || 'Guest'}</span>
            </li>
            <li>
              <hr className="dropdown-divider" />
            </li>
            <li>
              <button className="dropdown-item text-danger" onClick={logout}>
                Đăng xuất
              </button>
            </li>
          </ul>
        </div>
      </div>
    </header>
  )
}
