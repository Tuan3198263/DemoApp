// components/ThemNhanVien.jsx
import React, { useState } from 'react';
import EmployeeForm from './EmployeeForm';
import employeeService from '../services/employeeService'; // Import service

export default function ThemNhanVien({ show, onClose, onRefresh }) {
  const [listAdd, setListAdd] = useState([{}]);
  const [loading, setLoading] = useState(false);

  const handleRowChange = (index, newData) => {
    const newList = [...listAdd];
    newList[index] = newData;
    setListAdd(newList);
  };

  const addRow = () => setListAdd([...listAdd, {}]);

  const removeRow = (index) => {
    if (listAdd.length > 1) setListAdd(listAdd.filter((_, i) => i !== index));
  };

// Hàm kiểm tra logic trước khi gọi API
  const validateData = () => {
    const codes = listAdd.map(item => item.maNhanVien?.trim().toLowerCase());
    
    // 1. Kiểm tra mã nhân viên bị trùng ngay trong danh sách thêm mới
    const hasDuplicate = codes.some((code, index) => codes.indexOf(code) !== index);
    if (hasDuplicate) {
      alert("Lỗi: Các mã nhân viên trong danh sách thêm mới không được trùng nhau!");
      return false;
    }

    // 2. Kiểm tra các trường bắt buộc (bổ sung thêm nếu cần ngoài thuộc tính required của HTML5)
    for (const item of listAdd) {
      if (!item.maNhanVien || !item.tenNhanVien || !item.boPhan) {
        alert("Lỗi: Vui lòng nhập đầy đủ các trường bắt buộc cho tất cả nhân viên!");
        return false;
      }
    }

    return true;
  };

  const handleSave = async (e) => {
    if (e) e.preventDefault(); // Chặn load lại trang

    // Chạy validation frontend
    if (!validateData()) return;

    setLoading(true);
    try {
      // Gọi API: POST /api/employees/batch
      // Dữ liệu truyền vào là mảng listAdd
      const result = await employeeService.addEmployees(listAdd);

      if (result.success) {
        alert("Thêm danh sách nhân viên thành công!");
        setListAdd([{}]); // Reset form
        onClose(); // Đóng modal
      if (typeof onRefresh === 'function') {
           await onRefresh(); 
        }
      } else {
        alert(result.message || "Có lỗi xảy ra!");
      }
    } catch (error) {
      // Bắt lỗi từ BE (Ví dụ: BE báo mã NV đã tồn tại trong DB)
      const errorMsg = error.response?.data?.message || "Mã nhân viên đã tồn tại hoặc dữ liệu không hợp lệ!";
      alert("Lỗi: " + errorMsg);
      console.error("API Error:", error);
    } finally {
      setLoading(false);
    }
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