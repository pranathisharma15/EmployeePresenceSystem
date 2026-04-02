import React from "react";
import "./EmployeeDetailsModal.css";

const EmployeeDetailsModal = ({ employee, onClose }) => {
  if (!employee) return null;

  const sessions = employee.sessions || [];

  return (
    <div className="employee-modal-overlay" onClick={onClose}>
      <div
        className="employee-modal"
        onClick={(e) => e.stopPropagation()}
      >
        <div className="employee-modal-header">
          <div>
            <h2>{employee.name || "Employee Details"}</h2>
            <p>
              {employee.department || "N/A"} • {employee.team || "N/A"}
            </p>
            {employee.designation && (
              <small className="designation-text">
                {employee.designation}
              </small>
            )}
          </div>

          <button className="close-btn" onClick={onClose}>
            Close
          </button>
        </div>

        <div className="employee-info-grid">
          <div className="info-card">
            <span>First Login</span>
            <strong>{employee.firstLogin || "N/A"}</strong>
          </div>

          <div className="info-card">
            <span>Last Logout</span>
            <strong>{employee.lastLogout || "Active"}</strong>
          </div>

          <div className="info-card">
            <span>Total Duration</span>
            <strong>{employee.duration || 0} hrs</strong>
          </div>

          <div className="info-card">
            <span>Status</span>
            <strong
              className={
                employee.status === "Office"
                  ? "status-office"
                  : "status-remote"
              }
            >
              {employee.status || "Remote"}
            </strong>
          </div>
        </div>

        <div className="sessions-section">
          <h3>Login Sessions</h3>

          {sessions.length > 0 ? (
            <div className="session-list">
              {sessions.map((session, index) => (
                <div key={index} className="session-card">
                  <span>{session.start || "N/A"}</span>
                  <span>{session.end || "Active"}</span>
                </div>
              ))}
            </div>
          ) : (
            <div className="session-card">
              <span>{employee.firstLogin || "N/A"}</span>
              <span>{employee.lastLogout || "Active"}</span>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default EmployeeDetailsModal;