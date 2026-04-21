# Hướng Dẫn Test API Bằng Swagger

## 1. Chạy Ứng Dụng

```bash
dotnet run
```

Mở Swagger UI tại: `https://localhost:5001` (hoặc `http://localhost:5000`)

## 2. Nhóm API Auth

### 2.1 Đăng Ký Tài Khoản

**Endpoint:** `POST /api/auth/register`

**Body:**

```json
{
  "userName": "admin01",
  "password": "123456",
  "fullName": "Admin Demo",
  "phone": "0900000000",
  "email": "admin01@example.com",
  "status": "admin"
}
```

**Response (201 Created):**

```json
{
  "success": true,
  "data": {
    "id": 1,
    "userName": "admin01",
    "fullName": "Admin Demo",
    "phone": "0900000000",
    "email": "admin01@example.com",
    "status": "admin",
    "createdAt": "2026-04-21T10:00:00Z",
    "updatedAt": "2026-04-21T10:00:00Z"
  },
  "message": "Register success"
}
```

### 2.2 Đăng Nhập

**Endpoint:** `POST /api/auth/login`

**Body:**

```json
{
  "userName": "admin01",
  "password": "123456"
}
```

**Response (200 OK):**

```json
{
  "success": true,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIs...",
    "expiresAtUtc": "2026-04-28T10:00:00Z",
    "user": {
      "id": 1,
      "userName": "admin01",
      "fullName": "Admin Demo",
      "phone": "0900000000",
      "email": "admin01@example.com",
      "status": "admin",
      "createdAt": "2026-04-21T10:00:00Z",
      "updatedAt": "2026-04-21T10:00:00Z"
    }
  },
  "message": "Login success"
}
```

**Lưu token:** Copy `accessToken` để dùng cho các request tiếp theo

### 2.3 Đăng Xuất

**Endpoint:** `POST /api/auth/logout`

**Authorization:** Gắn Bearer token trong header

**Response (200 OK):**

```json
{
  "success": true,
  "data": {},
  "message": "Logout success"
}
```

## 3. Authorize Trên Swagger

1. Bấm nút **Authorize** (góc trên bên phải)
2. Nhập: `Bearer <accessToken>` (thay `<accessToken>` bằng token từ login)
3. Bấm **Authorize** rồi **Close**
4. Giờ có thể test các API khác

## 4. Nhóm API User (Tài Khoản)

### 4.1 Lấy Danh Sách Người Dùng (Có Phân Trang)

**Endpoint:** `GET /api/users`

**Query Parameters:**

- `page`: Trang (mặc định: 1)
- `pageSize`: Số bản ghi mỗi trang, tối đa 100 (mặc định: 10)
- `keyword`: Từ khóa tìm kiếm (username/email/fullname)
- `status`: Lọc theo vai trò (admin/employee)

**Ví dụ:**

```
GET /api/users?page=1&pageSize=10&keyword=admin&status=admin
```

**Response (200 OK):**

```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "userName": "admin01",
      "fullName": "Admin Demo",
      "phone": "0900000000",
      "email": "admin01@example.com",
      "status": "admin",
      "createdAt": "2026-04-21T10:00:00Z",
      "updatedAt": "2026-04-21T10:00:00Z"
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 10,
    "totalItems": 1,
    "totalPages": 1
  },
  "message": "Success"
}
```

### 4.2 Lấy Chi Tiết Người Dùng

**Endpoint:** `GET /api/users/{id}`

**Ví dụ:**

```
GET /api/users/1
```

**Response (200 OK):**

```json
{
  "success": true,
  "data": {
    "id": 1,
    "userName": "admin01",
    "fullName": "Admin Demo",
    "phone": "0900000000",
    "email": "admin01@example.com",
    "status": "admin",
    "createdAt": "2026-04-21T10:00:00Z",
    "updatedAt": "2026-04-21T10:00:00Z"
  },
  "message": "Success"
}
```

### 4.3 Tạo Người Dùng Mới

**Endpoint:** `POST /api/users`

**Body:**

```json
{
  "userName": "employee01",
  "password": "123456",
  "fullName": "Nhân Viên 01",
  "phone": "0911111111",
  "email": "employee01@example.com",
  "status": "employee"
}
```

**Response (201 Created):** Tương tự GetDetail

**Lỗi (409 Conflict):** Nếu username hoặc email đã tồn tại

### 4.4 Xóa Người Dùng

**Endpoint:** `DELETE /api/users/{id}`

**Ví dụ:**

```
DELETE /api/users/2
```

**Response (204 No Content):** Không có body

### 4.5 Xóa Nhiều Người Dùng

**Endpoint:** `DELETE /api/users`

**Body:**

```json
{
  "userIds": [2, 3, 4]
}
```

**Response (200 OK):**

```json
{
  "success": true,
  "data": {
    "deletedCount": 3
  },
  "message": "Success"
}
```

## 5. Nhóm API Employee (Nhân Viên)

### 5.1 Lấy Danh Sách Nhân Viên (Có Phân Trang & Sắp Xếp)

**Endpoint:** `GET /api/employees`

**Query Parameters:**

- `page`: Trang (mặc định: 1)
- `pageSize`: Số bản ghi mỗi trang, tối đa 100 (mặc định: 10)
- `keyword`: Tìm kiếm theo mã nhân viên hoặc tên
- `gioiTinh`: Lọc theo giới tính (Nam/Nữ/Khác)
- `sortBy`: Sắp xếp theo trường (maNhanVien, tenNhanVien, mucLuong, v.v.)
- `descending`: Sắp xếp giảm dần (true/false, mặc định: true)

**Ví dụ:**

```
GET /api/employees?page=1&pageSize=10&keyword=NV&gioiTinh=Nam&sortBy=MucLuong&descending=false
```

**Response (200 OK):**

```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "maNhanVien": "NV001",
      "tenNhanVien": "Nguyễn Văn A",
      "gioiTinh": "Nam",
      "boPhan": "IT",
      "mucLuong": 5000000.0,
      "updatedBy": 1,
      "createdAt": "2026-04-21T10:00:00Z",
      "updatedAt": "2026-04-21T10:00:00Z"
    }
  ],
  "pagination": {
    "page": 1,
    "pageSize": 10,
    "totalItems": 1,
    "totalPages": 1
  },
  "message": "Success"
}
```

### 5.2 Lấy Chi Tiết Nhân Viên

**Endpoint:** `GET /api/employees/{id}`

**Ví dụ:**

```
GET /api/employees/1
```

**Response (200 OK):**

```json
{
  "success": true,
  "data": {
    "id": 1,
    "maNhanVien": "NV001",
    "tenNhanVien": "Nguyễn Văn A",
    "gioiTinh": "Nam",
    "boPhan": "IT",
    "mucLuong": 5000000.0,
    "updatedBy": 1,
    "createdAt": "2026-04-21T10:00:00Z",
    "updatedAt": "2026-04-21T10:00:00Z"
  },
  "message": "Success"
}
```

### 5.3 Tạo Nhân Viên Mới

**Endpoint:** `POST /api/employees`

**Body:**

```json
{
  "maNhanVien": "NV001",
  "tenNhanVien": "Nguyễn Văn A",
  "gioiTinh": "Nam",
  "boPhan": "IT",
  "mucLuong": 5000000.0
}
```

**Response (201 Created):**

```json
{
  "success": true,
  "data": {
    "id": 1,
    "maNhanVien": "NV001",
    "tenNhanVien": "Nguyễn Văn A",
    "gioiTinh": "Nam",
    "boPhan": "IT",
    "mucLuong": 5000000.0,
    "updatedBy": 1,
    "createdAt": "2026-04-21T10:00:00Z",
    "updatedAt": "2026-04-21T10:00:00Z"
  },
  "message": "Tạo nhân viên thành công"
}
```

**Lỗi (409 Conflict):** Nếu mã nhân viên đã tồn tại
**Lỗi (400 Bad Request):** Nếu dữ liệu không hợp lệ

### 5.4 Cập Nhật Thông Tin Nhân Viên

**Endpoint:** `PUT /api/employees/{id}`

**Body (Tất cả các trường tùy chọn):**

```json
{
  "tenNhanVien": "Nguyễn Văn A Updated",
  "gioiTinh": "Nam",
  "boPhan": "HR",
  "mucLuong": 6000000.0
}
```

**Response (200 OK):** Trả về thông tin nhân viên sau cập nhật

**Lỗi (404 Not Found):** Nếu nhân viên không tồn tại

### 5.5 Xóa Nhân Viên

**Endpoint:** `DELETE /api/employees/{id}`

**Ví dụ:**

```
DELETE /api/employees/1
```

**Response (204 No Content):** Không có body

### 5.6 Xóa Nhiều Nhân Viên

**Endpoint:** `DELETE /api/employees`

**Body:**

```json
{
  "employeeIds": [1, 2, 3]
}
```

**Response (200 OK):**

```json
{
  "success": true,
  "data": {
    "deletedCount": 3
  },
  "message": "Success"
}
```

## 6. Các Lỗi Thường Gặp

| Mã Lỗi                    | Nguyên Nhân                       | Giải Pháp                      |
| ------------------------- | --------------------------------- | ------------------------------ |
| 401 Unauthorized          | Chưa đăng nhập hoặc token hết hạn | Đăng nhập lại và lấy token mới |
| 403 Forbidden             | Không có quyền truy cập           | Kiểm tra quyền tài khoản       |
| 404 Not Found             | Bản ghi không tồn tại             | Kiểm tra lại ID                |
| 409 Conflict              | Dữ liệu trùng lặp (mã/email)      | Sử dụng mã/email khác          |
| 400 Bad Request           | Dữ liệu không hợp lệ              | Kiểm tra lại format dữ liệu    |
| 500 Internal Server Error | Lỗi server                        | Kiểm tra log server            |

## 7. Ghi Chú Quan Trọng

- **UpdatedBy**: Tất cả các API POST/PUT/DELETE sẽ tự động gắn `UpdatedBy` bằng ID của user hiện tại (lấy từ JWT token)
- **Soft Delete**: Khi xóa, dữ liệu không bị xóa hẳn mà chỉ đánh dấu `DeletedAt` = thời gian hiện tại
- **Pagination**:
  - Trang bắt đầu từ 1
  - PageSize tối đa là 100
  - Nếu PageSize > 100, sẽ bị giới hạn lại 100
- **Sorting**:
  - SortBy hỗ trợ các trường như: `maNhanVien`, `tenNhanVien`, `mucLuong`, `createdAt`, `updatedAt`
  - Mặc định sắp xếp giảm dần (`descending = true`)
  - Nếu SortBy không hợp lệ, sẽ sắp xếp theo `CreatedAt`

## 8. Flow Thử Nghiệm Hoàn Chỉnh

1. **Đăng ký tài khoản admin**
2. **Đăng nhập** → Lấy token
3. **Authorize** trên Swagger với token
4. **Tạo nhân viên** (GET employee list rỗng trước, sau đó create)
5. **Lấy danh sách nhân viên** (với các filter/sort khác nhau)
6. **Lấy chi tiết nhân viên**
7. **Cập nhật nhân viên**
8. **Xóa nhân viên** (xóa 1 hoặc xóa nhiều)
9. **Xác nhận** nhân viên đã bị xóa (soft delete)
