import EmployeeList from './EmployeeList';

export default function Employee() {
  return (
    <div className="container-fluid py-3">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2 className="h4 mb-0 text-uppercase fw-bold text-primary">Nhân viên</h2>
        <div className="d-flex gap-2">
          <button className="btn btn-outline-primary btn-sm">Thêm nhiều</button>
          <button className="btn btn-primary btn-sm">Thêm nhân viên</button>
        </div>
      </div>
      <EmployeeList />
    </div>
  );
}