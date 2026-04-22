import { useState } from 'react'
import { Outlet } from 'react-router-dom'
import Footer from './Footer'
import Sidebar from './Sidebar'
import Topbar from './Topbar'

export default function MainLayout() {
  const [collapsed, setCollapsed] = useState(false)
  const [showMobileMenu, setShowMobileMenu] = useState(false)

  return (
    <div className="app-shell">
      <div className={`mobile-backdrop ${showMobileMenu ? 'show' : ''}`} onClick={() => setShowMobileMenu(false)}></div>

      <Sidebar
        collapsed={collapsed}
        onToggle={() => setCollapsed((prev) => !prev)}
      />

      <aside className={`mobile-sidebar ${showMobileMenu ? 'show' : ''}`}>
        <Sidebar
          collapsed={false}
          onToggle={() => setShowMobileMenu(false)}
        />
      </aside>

      <div className="content-area d-flex flex-column min-vh-100">
        <Topbar onOpenMobileMenu={() => setShowMobileMenu(true)} />
        <main className="flex-grow-1 p-3 bg-body-tertiary">
          <Outlet />
        </main>
        <Footer />
      </div>
    </div>
  )
}
