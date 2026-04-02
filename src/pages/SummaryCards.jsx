import React from "react";

const SummaryCards = ({ employees = [] }) => {
  const total = employees.length;

  const office = employees.filter(
    (e) => e.status === "Office"
  ).length;

  const remote = employees.filter(
    (e) => e.status === "Remote"
  ).length;

  const avgDuration =
    employees.reduce(
      (sum, e) => sum + parseFloat(e.duration || 0),
      0
    ) / (total || 1);

  return (
    <div className="grid grid-cols-4 gap-4 mb-6">
      <div className="bg-white p-4 rounded shadow">
        <h2>Total Employees</h2>
        <p className="text-xl font-bold">{total}</p>
      </div>

      <div className="bg-green-100 p-4 rounded shadow">
        <h2>In Office</h2>
        <p className="text-xl font-bold">{office}</p>
      </div>

      <div className="bg-gray-200 p-4 rounded shadow">
        <h2>Remote</h2>
        <p className="text-xl font-bold">{remote}</p>
      </div>

      <div className="bg-blue-100 p-4 rounded shadow">
        <h2>Avg Duration</h2>
        <p className="text-xl font-bold">
          {avgDuration.toFixed(2)} hrs
        </p>
      </div>
    </div>
  );
};

export default SummaryCards;