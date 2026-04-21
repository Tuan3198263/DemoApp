# Huong Dan Test API Bang Swagger

## 1. Chay ung dung

- `dotnet run`
- Mo Swagger: `https://localhost:5001` (hoac `http://localhost:5000`).

## 2. Nhom API Auth

### 2.1 Dang ky tai khoan

- API: `POST /api/auth/register`
- Body mau:

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

### 2.2 Dang nhap

- API: `POST /api/auth/login`
- Body mau:

```json
{
  "userName": "admin01",
  "password": "123456"
}
```

- Ket qua: tra ve `accessToken`.
- Luu y: khi login thanh cong se co `Console.WriteLine` trong terminal.

### 2.3 Dang xuat

- API: `POST /api/auth/logout`
- JWT la stateless, logout chu yeu phia client xoa token.

## 3. Authorize tren Swagger

- Bam nut `Authorize`.
- Nhap: `Bearer <accessToken>`.
- Sau do test cac API User.

## 4. Nhom API User

### 4.1 Lay danh sach (co phan trang/filter)

- API: `GET /api/users?page=1&pageSize=10&keyword=adm&status=admin`
- Gioi han `pageSize` toi da 100.

### 4.2 Lay chi tiet

- API: `GET /api/users/{id}`

### 4.3 Tao user

- API: `POST /api/users`
- Body mau:

```json
{
  "userName": "employee01",
  "password": "123456",
  "fullName": "Nhan vien 01",
  "phone": "0911111111",
  "email": "employee01@example.com",
  "status": "employee"
}
```

### 4.4 Xoa 1 user

- API: `DELETE /api/users/{id}`

### 4.5 Xoa nhieu user

- API: `DELETE /api/users`
- Body mau:

```json
{
  "userIds": [2, 3, 4]
}
```

## 5. Loi thuong gap

- `401 Unauthorized`: chua Authorize token hoac token het han.
- `409 Conflict`: username/email da ton tai.
- `400 Bad Request`: du lieu khong hop le.
