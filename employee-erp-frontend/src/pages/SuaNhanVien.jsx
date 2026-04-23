import React, { useState, useEffect } from 'react';
import EmployeeForm from './EmployeeForm';
import employeeService from '../services/employeeService'; // Import service

export default function SuaNhanVien({ show, employeeData, onClose, onRefresh }) {
  const [formData, setFormData] = useState({});
  const [loading, setLoading] = useState(false); // Thêm loading

  useEffect(() => {
    // Khi mở modal hoặc dữ liệu thay đổi, reset form và loading
    if (show && employeeData) {
      setFormData({ ...employeeData });
      setLoading(false);
    }
  }, [employeeData, show]);

  const handleSave = async (e) => {
    if (e) e.preventDefault(); // Chặn reload trang nếu dùng thẻ form

    setLoading(true);
    try {
      // Gọi service: PUT /api/employees/{id}
      // formData.id là khóa chính, phần còn lại là body
      const result = await employeeService.updateEmployee(formData.id, formData);

      if (result.success) {
        alert("Cập nhật nhân viên thành công!");
        onClose(); // Đóng modal
        if (onRefresh) onRefresh(); // Tải lại danh sách ở Table
      }
    } catch (error) {
      console.error("Lỗi cập nhật:", error);
      alert(error.response?.data?.message || "Không thể cập nhật nhân viên");
    } finally {
      setLoading(false);
    }
  };

  if (!show) return null;

  return (
    <div className="modal d-block" style={{ background: 'rgba(0,0,0,0.5)', zIndex: 1050 }}>
      <div className="modal-dialog modal-lg shadow-lg">
        <div className="modal-content border-0">
          {/* Bao bọc bằng form để trigger 'required' validation của EmployeeForm */}
          <form onSubmit={handleSave}>
            <div className="modal-header bg-primary text-white py-2">
              <h5 className="modal-title h6 text-uppercase">Cập nhật thông tin nhân viên</h5>
              <button 
                type="button" 
                className="btn-close btn-close-white" 
                onClick={onClose}
                disabled={loading}
              ></button>
            </div>

            <div className="modal-body p-0">
              <EmployeeForm 
                data={formData} 
                onChange={(data) => setFormData(data)} 
                disableMaNV={true} // Sửa thì không cho sửa mã NV
              />
            </div>

            <div className="modal-footer bg-light border-0">
              <button 
                type="button" 
                className="btn btn-secondary btn-sm px-3" 
                onClick={onClose}
                disabled={loading}
              >
                Hủy
              </button>
              <button 
                type="submit" 
                className="btn btn-primary btn-sm px-4 shadow-sm"
                disabled={loading}
              >
                {loading ? (
                  <>
                    <span className="spinner-border spinner-border-sm me-2"></span>
                    Đang lưu...
                  </>
                ) : "Cập nhật"}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}