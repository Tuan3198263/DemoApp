import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { ROUTES } from '../constants/routes';
import { useAuth } from '../hooks/useAuth';

export default function Login() {
  const navigate = useNavigate();
  const { login } = useAuth();
  
  const [form, setForm] = useState({ username: '', password: '' });
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');

  const onChange = (e) => {
    const { name, value } = e.target;
    setForm(prev => ({ ...prev, [name]: value }));
    if (error) setError(''); // Xóa lỗi cũ khi user nhập lại
  };

  const onSubmit = async (e) => {
    e.preventDefault();
    setSubmitting(true);

    try {
      // Gọi trực tiếp authService thông qua hook useAuth
      await login(form);
      navigate(ROUTES.DASHBOARD, { replace: true });
    } catch (err) {
      // Hiển thị lỗi thật từ Backend (400, 401, 500...)
      setError(err?.message || "Sai tài khoản hoặc mật khẩu");
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="login-form">
      {error && (
        <div className="alert alert-danger py-2 small" role="alert">
          {error}
        </div>
      )}

      <form onSubmit={onSubmit}>
        <div className="mb-3">
          <label className="form-label small fw-bold">Tên đăng nhập</label>
          <input
            name="username"
            type="text"
            className="form-control form-control-sm"
            value={form.username}
            onChange={onChange}
            disabled={submitting}
            required
          />
        </div>

        <div className="mb-3">
          <label className="form-label small fw-bold">Mật khẩu</label>
          <input
            name="password"
            type="password"
            className="form-control form-control-sm"
            value={form.password}
            onChange={onChange}
            disabled={submitting}
            required
          />
        </div>

        <button
          type="submit"
          className="btn btn-primary w-100 btn-sm fw-bold"
          disabled={submitting}
        >
          {submitting ? 'Đang xử lý...' : 'Đăng nhập'}
        </button>
      </form>
    </div>
  );
}