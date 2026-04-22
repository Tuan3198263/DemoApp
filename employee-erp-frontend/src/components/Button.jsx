export default function Button({ children, className = '', ...rest }) {
  return (
    <button type="button" className={`btn ${className}`.trim()} {...rest}>
      {children}
    </button>
  )
}
