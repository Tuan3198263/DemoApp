// components/SuaNhanVien.jsx
import React, { useState, useEffect } from 'react';
import EmployeeForm from './EmployeeForm';

export default function SuaNhanVien({ show, employeeData, onClose }) {
  const [formData, setFormData] = useState({});

  useEffect(() => {
    if (employeeData) setFormData({ ...employeeData });
  }, [employeeData, show]);

  const handleSave = () => {
    console.log("Dữ liệu gửi lên API (Cập nhật):", [formData]);
    onClose();
  };

  if (!show) return null;

  return (
    <div className="modal d-block" style={{ background: 'rgba(0,0,0,0.5)', zIndex: 1050 }}>
      <div className="modal-dialog modal-lg shadow-lg">
        <div className="modal-content">
          <div className="modal-header bg-success text-white">
            <h5 className="modal-title">Cập nhật thông tin nhân viên</h5>
            <button className="btn-close btn-close-white" onClick={onClose}></button>
          </div>
          <div className="modal-body">
            <EmployeeForm 
              data={formData} 
              onChange={(data) => setFormData(data)} 
              disableMaNV={true} 
            />
          </div>
          <div className="modal-footer bg-light">
            <button className="btn btn-secondary btn-sm" onClick={onClose}>Hủy</button>
            <button className="btn btn-primary btn-sm px-4" onClick={handleSave}>Cập nhật</button>
          </div>
        </div>
      </div>
    </div>
  );
}