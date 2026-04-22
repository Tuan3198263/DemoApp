export function validateEmployee(values) {
  const errors = {}

  if (!values.fullName?.trim()) {
    errors.fullName = 'Ho ten nhan vien la bat buoc.'
  }

  if (!values.email?.trim()) {
    errors.email = 'Email la bat buoc.'
  } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(values.email)) {
    errors.email = 'Email khong dung dinh dang.'
  }

  return errors
}
