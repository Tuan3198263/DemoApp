# SYSTEM OVERVIEW - ERP MINI EMPLOYEE MANAGEMENT

## 1. Muc tieu

Ung dung frontend React cho he thong quan ly nhan vien quy mo nho (ERP mini), uu tien:

- Khoi tao nhanh.
- Cau truc ro rang de mo rong.
- Responsive day du tren desktop/mobile.

## 2. Tech Stack

- React (JavaScript, Functional Components, Hooks).
- React Router v6.
- Bootstrap 5.3 + Bootstrap Icons.
- Axios cho giao tiep API.
- LocalStorage cho auth state (token/user).

## 3. Kien truc thu muc src

- pages: Dashboard, EmployeeList, EmployeeDetail, Settings, Login.
- components: Button, Input, Table, Modal, PrivateRoute.
- layouts: Sidebar, Topbar, Footer, MainLayout.
- services: apiClient, authService, employeeService.
- hooks: useAuth, useLocalStorage.
- utils: storage.
- constants: api, actionTypes, messages, routes.
- validators: loginValidator, employeeValidator.

## 4. Luong xac thuc

1. User dang nhap tai trang /login.
2. Login goi authService -> luu token/user vao localStorage.
3. PrivateRoute kiem tra token.
4. Neu chua dang nhap -> redirect /login.
5. Neu dang nhap -> vao MainLayout va cac trang noi bo.

## 5. API Base

- Axios baseURL: http://localhost:5001

## 6. Trang thai hien tai

- Da co khung UI co ban cho Dashboard, Employee List, Employee Detail, Settings.
- Sidebar co che do collapse tren desktop.
- Mobile co drawer menu + backdrop.
- Chua day du nghiep vu CRUD nhan vien (se phat trien sau).
