# Hướng Dẫn Test API Employee Bằng Swagger

## 1. Chạy ứng dụng

```bash
dotnet run
```

Mở Swagger UI tại `https://localhost:5001` hoặc `http://localhost:5000`.

## 2. Chuẩn bị token

1. Gọi `POST /api/auth/login` để lấy `accessToken`.
2. Bấm nút `Authorize` trên Swagger.
3. Nhập `Bearer <accessToken>`.

## 3. Endpoint Employee

### 3.1 Lấy danh sách

`GET /api/employees?page=1&pageSize=10&keyword=NV&gioiTinh=Nam&boPhan=IT&sortBy=NgaySinh&descending=true`

Ghi chú:

- `pageSize` tối đa 100.
- Hỗ trợ sắp xếp qua `sortBy` và `descending`.

### 3.2 Lấy chi tiết

`GET /api/employees/{id}`

### 3.3 Tạo 1 nhân viên

`POST /api/employees`

Body mẫu:

```json
{
  "maNhanVien": "NV001",
  "tenNhanVien": "Nguyễn Văn A",
  "ngaySinh": "1995-03-21",
  "gioiTinh": "Nam",
  "boPhan": "IT",
  "mucLuong": 12000000
}
```

### 3.4 Tạo nhiều nhân viên

`POST /api/employees/batch`

Body mẫu:

```json
{
  "employees": [
    {
      "maNhanVien": "NV002",
      "tenNhanVien": "Trần Văn B",
      "ngaySinh": "1994-11-10",
      "gioiTinh": "Nam",
      "boPhan": "HR",
      "mucLuong": 10000000
    },
    {
      "maNhanVien": "NV003",
      "tenNhanVien": "Nguyễn Thị C",
      "ngaySinh": "1998-06-05",
      "gioiTinh": "Nữ",
      "boPhan": "Finance",
      "mucLuong": 11000000
    }
  ]
}
```

Lưu ý:

- Endpoint kiểm tra trùng mã trong danh sách gửi lên.
- Endpoint kiểm tra trùng mã với dữ liệu đã có trong DB.
- Khi lưu, `UpdatedBy` được gắn tự động từ user đang đăng nhập.

### 3.5 Sửa nhân viên

`PUT /api/employees/{id}`

Body mẫu:

```json
{
  "tenNhanVien": "Nguyễn Văn A Updated",
  "ngaySinh": "1995-03-22",
  "gioiTinh": "Nam",
  "boPhan": "IT",
  "mucLuong": 13000000
}
```

### 3.6 Xóa 1 nhân viên

`DELETE /api/employees/{id}`

### 3.7 Xóa nhiều nhân viên

`DELETE /api/employees`

Body mẫu:

```json
{
  "employeeIds": [1, 2, 3]
}
```

## 4. Dữ liệu trả về quan trọng

- `updatedBy`: tự động lấy từ user đang đăng nhập.
- `ngaySinh`: kiểu `date`, chỉ truyền phần ngày `yyyy-MM-dd`.
- `mucLuong`: số thập phân, có thể gửi dạng `12000000` hoặc `12000000.00`.

## 5. Các lỗi thường gặp

- `400 Bad Request`: dữ liệu không hợp lệ hoặc thiếu trường bắt buộc.
- `401 Unauthorized`: chưa gắn token.
- `409 Conflict`: mã nhân viên bị trùng.
- `404 Not Found`: nhân viên không tồn tại.
