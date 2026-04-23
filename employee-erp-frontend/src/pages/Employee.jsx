import { useState } from 'react';
import EmployeeList from './EmployeeList';
import ThemNhanVien from './ThemNhanVien';

export default function Employee() {
  const [showAdd, setShowAdd] = useState(false);

  return (
    <div className="container-fluid py-3">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2 className="h4 mb-0 text-uppercase fw-bold text-primary">Nhân viên</h2>
        <div className="d-flex gap-2">
          {/* Cả 2 nút đều mở chung Modal thêm, logic thêm nhiều xử lý bên trong Modal */}
          <button className="btn btn-primary btn-sm" onClick={() => setShowAdd(true)}>
            Thêm nhân viên
          </button>
        </div>
      </div>

      {/* Danh sách hiển thị */}
      <EmployeeList />

      {/* Modal thêm nằm tại đây */}
      <ThemNhanVien show={showAdd} onClose={() => setShowAdd(false)} />
    </div>
  );
}