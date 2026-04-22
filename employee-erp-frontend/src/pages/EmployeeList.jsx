import { Link } from 'react-router-dom'
import Table from '../components/Table'

const columns = [
  { key: 'fullName', label: 'Ho va ten' },
  { key: 'email', label: 'Email' },
  { key: 'department', label: 'Phong ban' },
  { key: 'status', label: 'Trang thai' },
]

const rows = [
  { id: 1, fullName: 'Nguyen Van A', email: 'a@company.com', department: 'HR', status: 'Active' },
  { id: 2, fullName: 'Tran Thi B', email: 'b@company.com', department: 'IT', status: 'Active' },
  { id: 3, fullName: 'Le Van C', email: 'c@company.com', department: 'Finance', status: 'Leave' },
]

export default function EmployeeList() {
  return (
    <div className="card border-0 shadow-sm">
      <div className="card-body">
        <div className="d-flex justify-content-between align-items-center mb-3">
          <h2 className="h5 mb-0">Danh sach nhan vien</h2>
          <Link to="/employees/1" className="btn btn-primary btn-sm">
            Xem chi tiet mau
          </Link>
        </div>
        <Table columns={columns} rows={rows} />
      </div>
    </div>
  )
}
