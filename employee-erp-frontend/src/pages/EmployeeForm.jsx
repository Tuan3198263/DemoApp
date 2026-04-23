// components/EmployeeForm.jsx
import React from 'react';

export default function EmployeeForm({ data, onChange, onRemove, isMultiple = false, disableMaNV = false }) {
  const handleChange = (e) => {
    const { name, value } = e.target;
    onChange({ ...data, [name]: value });
  };

  return (
    <div className={`p-3 position-relative ${isMultiple ? 'border rounded mb-4 bg-light' : ''}`}>
      {/* Nút icon xóa xuất hiện ở góc trên bên phải khi thêm nhiều */}
      {isMultiple && (
        <button 
          type="button" 
          className="btn btn-outline-danger btn-sm border-0 position-absolute" 
          style={{ top: '5px', right: '5px' }}
          onClick={onRemove}
        >
          <i className="bi bi-trash"></i>
        </button>
      )}

      <div className="row g-3">
        {/* Dòng 1: Mã NV và Tên */}
        <div className="col-md-6">
          <label className="form-label small fw-bold">Mã nhân viên <span className="text-danger">*</span></label>
          <input 
            name="maNhanVien" 
            className="form-control" 
            value={data.maNhanVien || ''} 
            onChange={handleChange}
            disabled={disableMaNV}
            required
          />
        </div>
        <div className="col-md-6">
          <label className="form-label small fw-bold">Tên nhân viên <span className="text-danger">*</span></label>
          <input 
            name="tenNhanVien" 
            className="form-control" 
            value={data.tenNhanVien || ''} 
            onChange={handleChange}
            required
          />
        </div>

        {/* Dòng 2: Ngày sinh và Giới tính */}
        <div className="col-md-6">
          <label className="form-label small fw-bold">Ngày sinh <span className="text-danger">*</span></label>
          <input 
            name="ngaySinh" 
            type="date" 
            className="form-control" 
            value={data.ngaySinh || ''} 
            onChange={handleChange}
            required
          />
        </div>
        <div className="col-md-6">
          <label className="form-label small fw-bold">Giới tính <span className="text-danger">*</span></label>
          <select name="gioiTinh" className="form-select" value={data.gioiTinh || ''} onChange={handleChange} required>
            <option value="">-- Chọn giới tính --</option>
            <option value="Nam">Nam</option>
            <option value="Nữ">Nữ</option>
          </select>
        </div>

        {/* Dòng 3: Bộ phận và Mức lương */}
        <div className="col-md-6">
          <label className="form-label small fw-bold">Bộ phận <span className="text-danger">*</span></label>
          <select name="boPhan" className="form-select" value={data.boPhan || ''} onChange={handleChange} required>
            <option value="">-- Chọn bộ phận --</option>
            <option value="HR">HR</option>
            <option value="IT">IT</option>
            <option value="Finance">Finance</option>
            <option value="Marketing">Marketing</option>
          </select>
        </div>
        <div className="col-md-6">
          <label className="form-label small fw-bold">Mức lương <span className="text-danger">*</span></label>
          <div className="input-group">
            <input 
              name="mucLuong" 
              type="number" 
              className="form-control" 
              value={data.mucLuong || ''} 
              onChange={handleChange}
              min="0"
              required
            />
            <span className="input-group-text">₫</span>
          </div>
        </div>
      </div>
    </div>
  );
}