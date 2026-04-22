# CODING RULES & STYLE

## 1. Nguyen tac chung

- Uu tien code de doc, de maintain, de test.
- Tach biet ro UI, business logic, service call, validate.
- Tranh hard-code lap lai, dua vao constants.

## 2. React Convention

- Su dung Functional Components.
- Uu tien Hooks (`useState`, `useEffect`, custom hooks).
- Props dat ten ro nghia, han che truyen qua nhieu tang.
- Component tai su dung dat trong `components/`.

## 3. Routing va Auth

- Route private phai di qua `PrivateRoute`.
- Trang login la route public.
- Redirect ro rang khi user chua co token.

## 4. API & Data

- Tat ca HTTP request di qua `services/apiClient.js`.
- Khong goi axios truc tiep trong page component.
- Interceptor phai gan token Bearer neu co.

## 5. LocalStorage

- Chi luu thong tin can thiet: token, user.
- Khong luu du lieu nhay cam khong can thiet.
- Utility thao tac localStorage tap trung tai `utils/storage.js`.

## 6. Validation

- Validation tach rieng trong `validators/`.
- Form khong submit neu validation fail.
- Message loi ngan gon, de hieu.

## 7. UI/UX

- Uu tien Bootstrap utility class va layout grid.
- Dam bao responsive tren mobile/tablet/desktop.
- Giao dien nhat quan qua layout chung (Sidebar/Topbar/Footer).

## 8. Comment va Dat ten

- Comment ngan gon bang tieng Viet cho doan logic kho.
- Dat ten bien/ham ro nghia, theo camelCase.
- Component dat ten PascalCase.

## 9. Mo rong tiep theo

- Tich hop API that cho employeeService va authService.
- Bo sung trang tao/sua/xoa nhan vien.
- Them phan quyen theo role.
