import { useParams } from 'react-router-dom'

export default function EmployeeDetail() {
  const { id } = useParams()

  return (
    <div className="card border-0 shadow-sm">
      <div className="card-body">
        <h2 className="h5">Thong tin nhan vien #{id}</h2>
        <p className="mb-1">Ho ten: Nguyen Van A</p>
        <p className="mb-1">Email: a@company.com</p>
        <p className="mb-1">Phong ban: HR</p>
        <p className="mb-0">Trang thai: Active</p>
      </div>
    </div>
  )
}
