export default function Input({ label, error, id, ...rest }) {
  return (
    <div className="mb-3">
      {label ? (
        <label htmlFor={id} className="form-label">
          {label}
        </label>
      ) : null}
      <input id={id} className={`form-control ${error ? 'is-invalid' : ''}`} {...rest} />
      {error ? <div className="invalid-feedback">{error}</div> : null}
    </div>
  )
}
