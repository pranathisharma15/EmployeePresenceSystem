import React, { useState } from "react";
import "./FilterPanel.css";

const FiltersPanel = ({
  filters,
  setFilters,
  departments = [],
  teams = [],
}) => {
  const [showDateRange, setShowDateRange] = useState(false);

  const handleChange = (key, value) => {
    setFilters((prev) => ({
      ...prev,
      [key]: value,
    }));
  };

  return (
    <div className="filters-panel">
      <input
        type="text"
        placeholder="Search employee..."
        value={filters.search}
        onChange={(e) => handleChange("search", e.target.value)}
        className="filter-input"
      />

      <select
        value={filters.department}
        onChange={(e) => handleChange("department", e.target.value)}
        className="filter-input"
      >
        <option value="">All Departments</option>
        {departments.map((dept, index) => (
          <option key={index} value={dept}>
            {dept}
          </option>
        ))}
      </select>

      <select
        value={filters.team}
        onChange={(e) => handleChange("team", e.target.value)}
        className="filter-input"
      >
        <option value="">All Teams</option>
        {teams.map((team, index) => (
          <option key={index} value={team}>
            {team}
          </option>
        ))}
      </select>

      {/* Single expandable date range filter */}
      <div className="date-range-wrapper">
        <input
          type="text"
          readOnly
          value={
            filters.fromDate && filters.toDate
              ? `${filters.fromDate} → ${filters.toDate}`
              : "Select Date Range"
          }
          onClick={() => setShowDateRange(!showDateRange)}
          className="filter-input"
        />

        {showDateRange && (
          <div className="date-range-popup">
            <input
              type="date"
              value={filters.fromDate}
              onChange={(e) => handleChange("fromDate", e.target.value)}
              className="filter-input"
            />

            <input
              type="date"
              value={filters.toDate}
              onChange={(e) => handleChange("toDate", e.target.value)}
              className="filter-input"
            />
          </div>
        )}
      </div>

      <select
        value={filters.status}
        onChange={(e) => handleChange("status", e.target.value)}
        className="filter-input"
      >
        <option value="">All Status</option>
        <option value="Office">Office</option>
        <option value="Remote">Remote</option>
      </select>
    </div>
  );
};

export default FiltersPanel;