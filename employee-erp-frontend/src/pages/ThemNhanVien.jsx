// components/ThemNhanVien.jsx
import React, { useState } from 'react';
import EmployeeForm from './EmployeeForm';

export default function ThemNhanVien({ show, onClose }) {
  const [listAdd, setListAdd] = useState([{}]);

  const handleRowChange = (index, newData) => {
    const newList = [...listAdd];
    newList[index] = newData;
    setListAdd(newList);
  };

  const addRow = () => setListAdd([...listAdd, {}]);
  const removeRow = (index) => {
    if (listAdd.length > 1) setListAdd(listAdd.filter((_, i) => i !== index));
  };

  const handleSave = () => {
    console.log("Dữ liệu gửi lên API (Thêm mới):", listAdd);
    onClose();
    setListAdd([{}]); // Reset form sau khi đóng
  };

  if (!show) return null;

  return (
    <div className="modal d-block" style={{ background: 'rgba(0,0,0,0.5)', zIndex: 1050 }}>
      <div className="modal-dialog modal-lg shadow-lg">
        <div className="modal-content">
          <div className="modal-header bg-success text-white">
            <h5 className="modal-title">Thêm nhân viên mới</h5>
            <button className="btn-close btn-close-white" onClick={onClose}></button>
          </div>
          <div className="modal-body" style={{ maxHeight: '65vh', overflowY: 'auto' }}>
            {listAdd.map((item, idx) => (
              <EmployeeForm 
                key={idx} 
                data={item} 
                isMultiple={listAdd.length > 1}
                onChange={(data) => handleRowChange(idx, data)}
                onRemove={() => removeRow(idx)}
              />
            ))}
            <button className="btn btn-link btn-sm text-decoration-none" onClick={addRow}>
              <i className="bi bi-plus-circle"></i> Thêm dòng nhập liệu
            </button>
          </div>
          <div className="modal-footer bg-light">
            <button className="btn btn-secondary btn-sm" onClick={onClose}>Đóng</button>
            <button className="btn btn-success btn-sm px-4" onClick={handleSave}>Lưu tất cả</button>
          </div>
        </div>
      </div>
    </div>
  );
}