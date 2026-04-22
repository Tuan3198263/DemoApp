import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import Button from '../components/Button'
import Input from '../components/Input'
import { MESSAGES } from '../constants/messages'
import { ROUTES } from '../constants/routes'
import { useAuth } from '../hooks/useAuth'
import { validateLogin } from '../validators/loginValidator'

const initialForm = {
  username: '',
  password: '',
}

export default function Login() {
  const navigate = useNavigate()
  const { login } = useAuth()
  const [form, setForm] = useState(initialForm)
  const [errors, setErrors] = useState({})
  const [submitting, setSubmitting] = useState(false)

  const onChange = (event) => {
    const { name, value } = event.target
    setForm((prev) => ({ ...prev, [name]: value }))
  }

  const onSubmit = async (event) => {
    event.preventDefault()

    const nextErrors = validateLogin(form)
    setErrors(nextErrors)

    if (Object.keys(nextErrors).length > 0) {
      return
    }

    setSubmitting(true)

    try {
      await login(form)
      navigate(ROUTES.DASHBOARD)
    } catch (error) {
      setErrors({ general: MESSAGES.LOGIN_FAILED })
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <div className="login-page d-flex align-items-center justify-content-center p-3">
      <div className="card border-0 shadow-lg login-card">
        <div className="card-body p-4 p-md-5">
          <h1 className="h3 mb-1">Dang nhap he thong</h1>
          <p className="text-body-secondary mb-4">ERP Employee Management</p>

          <form onSubmit={onSubmit} noValidate>
            <Input
              id="username"
              name="username"
              label="Username"
              value={form.username}
              onChange={onChange}
              error={errors.username}
              placeholder="admin"
            />
            <Input
              id="password"
              name="password"
              type="password"
              label="Password"
              value={form.password}
              onChange={onChange}
              error={errors.password}
              placeholder="******"
            />

            {errors.general ? <p className="text-danger small">{errors.general}</p> : null}

            <Button className="btn-primary w-100" type="submit" disabled={submitting}>
              {submitting ? 'Dang xu ly...' : 'Dang nhap'}
            </Button>
          </form>
        </div>
      </div>
    </div>
  )
}
