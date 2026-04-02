import React from "react";
import "./EmployeeTable.css";

const EmployeeTable = ({ employees = [], onRowClick }) => {
  return (
    <div className="employee-table-wrapper">
      <table className="employee-table">
        <thead>
          <tr>
            <th>Name</th>
            <th>Department</th>
            <th>Team</th>
            <th>Status</th>
            <th>First Login</th>
            <th>Last Logout</th>
            <th>Duration</th>
          </tr>
        </thead>

        <tbody>
          {employees.length > 0 ? (
            employees.map((emp, index) => (
              <tr
                key={emp.id || index}
                className="employee-row"
                onClick={() => onRowClick(emp)}
              >
                <td>{emp.name || "N/A"}</td>
                <td>{emp.department || "N/A"}</td>
                <td>{emp.team || "N/A"}</td>

                <td>
                  <span
                    className={`status-badge ${
                      emp.status === "Office"
                        ? "online"
                        : "offline"
                    }`}
                  >
                    {emp.status || "Remote"}
                  </span>
                </td>

                <td>{emp.firstLogin || "-"}</td>
                <td>{emp.lastLogout || "-"}</td>
                <td>
                  {emp.duration ? `${emp.duration} hrs` : "-"}
                </td>
              </tr>
            ))
          ) : (
            <tr>
              <td colSpan="7" className="empty-row">
                No employee presence data available
              </td>
            </tr>
          )}
        </tbody>
      </table>
    </div>
  );
};

export default EmployeeTable;