import { useState, useEffect } from 'react';
import employeeService from '../services/employeeService';
import Table from '../components/Table';

const columns = [
  { key: 'maNhanVien', label: 'Mã NV' },
  { key: 'tenNhanVien', label: 'Tên nhân viên' },
  { key: 'ngaySinh', label: 'Ngày sinh' },
  { key: 'gioiTinh', label: 'Giới tính' },
  { key: 'boPhan', label: 'Bộ phận' },
  { key: 'mucLuong', label: 'Mức lương', sortable: true },
];

export default function EmployeeList() {
  const [employees, setEmployees] = useState([]);
  const [loading, setLoading] = useState(false);
  const [pagination, setPagination] = useState({ page: 1, pageSize: 10, totalItems: 0, totalPages: 0 });
  
  const [params, setParams] = useState({
    page: 1,
    pageSize: 10,
    keyword: '',
    gioiTinh: '',
    boPhan: '', // State cho bộ phận
    sortBy: '',
    descending: true
  });

  const fetchEmployees = async () => {
    setLoading(true);
    try {
      const res = await employeeService.getEmployees(params);
      if (res.success) {
        setEmployees(res.data);
        setPagination(res.pagination);
      }
    } catch (error) {
      console.error("Lỗi:", error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchEmployees();
  }, [params]);

  // Xử lý Sort mức lương
  const handleSortSalary = () => {
    setParams(prev => ({
      ...prev,
      sortBy: 'mucLuong',
      descending: prev.sortBy === 'mucLuong' ? !prev.descending : true,
      page: 1
    }));
  };

  const tableRows = employees.map(emp => ({
    ...emp,
    ngaySinh: emp.ngaySinh ? new Date(emp.ngaySinh).toLocaleDateString('vi-VN') : '',
    mucLuong: new Intl.NumberFormat('vi-VN').format(emp.mucLuong) + ' ₫'
  }));

  return (
    <div className="card border-0 shadow-sm">
      <div className="card-body">
        
        {/* THANH FILTER */}
        <div className="row g-2 mb-4">
          <div className="col-md-4">
            <input 
              className="form-control form-control-sm" 
              placeholder="Tìm theo tên, mã..." 
              value={params.keyword}
              onChange={(e) => setParams(p => ({ ...p, keyword: e.target.value, page: 1 }))}
            />
          </div>
          
          <div className="col-md-3">
            <select 
              className="form-select form-select-sm"
              value={params.boPhan}
              onChange={(e) => setParams(p => ({ ...p, boPhan: e.target.value, page: 1 }))}
            >
              <option value="">-- Tất cả bộ phận --</option>
              <option value="HR">Phòng Nhân sự (HR)</option>
              <option value="IT">Phòng Kỹ thuật (IT)</option>
              <option value="Finance">Phòng Tài chính</option>
              <option value="Sales">Phòng Kinh doanh</option>
            </select>
          </div>
        </div>

        {/* BẢNG DỮ LIỆU */}
        <div className="table-responsive">
          <table className="table table-hover align-middle shadow-sm border">
            <thead className="table-light">
              <tr>
                {columns.map(col => (
                  <th 
                    key={col.key} 
                    onClick={col.sortable ? handleSortSalary : null}
                    className={col.sortable ? 'user-select-none' : ''}
                    style={{ cursor: col.sortable ? 'pointer' : 'default', whiteSpace: 'nowrap' }}
                  >
                    <div className="d-flex align-items-center">
                      {col.label}
                      {col.sortable && (
                        <span className="ms-2 d-flex flex-column" style={{ fontSize: '10px', lineHeight: '1' }}>
                          <i className={`bi bi-caret-up-fill ${params.sortBy === 'mucLuong' && !params.descending ? 'text-primary' : 'text-muted'}`}></i>
                          <i className={`bi bi-caret-down-fill ${params.sortBy === 'mucLuong' && params.descending ? 'text-primary' : 'text-muted'}`}></i>
                        </span>
                      )}
                    </div>
                  </th>
                ))}
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr><td colSpan={columns.length} className="text-center py-5">Đang tải dữ liệu...</td></tr>
              ) : tableRows.length > 0 ? (
                tableRows.map((row, idx) => (
                  <tr key={idx}>
                    {columns.map(col => <td key={col.key}>{row[col.key]}</td>)}
                  </tr>
                ))
              ) : (
                <tr><td colSpan={columns.length} className="text-center py-4">Không tìm thấy nhân viên nào</td></tr>
              )}
            </tbody>
          </table>
        </div>

        {/* PHÂN TRANG */}
        <div className="d-flex justify-content-between align-items-center mt-3">
          <p className="small text-muted mb-0">
            Hiển thị <b>{tableRows.length}</b> trên <b>{pagination.totalItems}</b> nhân viên
          </p>
          
          <nav>
            <ul className="pagination pagination-sm mb-0">
              <li className={`page-item ${params.page === 1 ? 'disabled' : ''}`}>
                <button className="page-link" onClick={() => setParams(p => ({ ...p, page: p.page - 1 }))}>Trước</button>
              </li>
              
              {[...Array(pagination.totalPages)].map((_, i) => (
                <li key={i} className={`page-item ${params.page === i + 1 ? 'active' : ''}`}>
                  <button className="page-link" onClick={() => setParams(p => ({ ...p, page: i + 1 }))}>{i + 1}</button>
                </li>
              ))}

              <li className={`page-item ${params.page === pagination.totalPages ? 'disabled' : ''}`}>
                <button className="page-link" onClick={() => setParams(p => ({ ...p, page: p.page + 1 }))}>Sau</button>
              </li>
            </ul>
          </nav>
        </div>
      </div>
    </div>
  );
}