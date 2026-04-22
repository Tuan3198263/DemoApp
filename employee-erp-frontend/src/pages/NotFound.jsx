import React from 'react';
import { useNavigate } from 'react-router-dom';

const NotFound = () => {
  const navigate = useNavigate();

  return (
    <div className="d-flex align-items-center justify-content-center vh-100 bg-light">
      <div className="text-center">
        <h1 className="display-1 fw-bold text-primary">404</h1>
        <p className="fs-3"> <span className="text-danger">Rất tiếc!</span> Không tìm thấy trang.</p>
        <p className="lead">
          Đường dẫn bạn đang truy cập không tồn tại hoặc đã bị di chuyển.
        </p>
        <button 
          onClick={() => navigate('/dashboard')} 
          className="btn btn-primary shadow-sm"
        >
          <i className="bi bi-house-door me-2"></i> Quay lại trang chủ
        </button>
      </div>
    </div>
  );
};

export default NotFound;