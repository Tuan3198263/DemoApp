export function validateLogin(values) {
  const errors = {}

  if (!values.username?.trim()) {
    errors.username = 'Username la bat buoc.'
  }

  if (!values.password?.trim()) {
    errors.password = 'Password la bat buoc.'
  } else if (values.password.length < 6) {
    errors.password = 'Password toi thieu 6 ky tu.'
  }

  return errors
}
