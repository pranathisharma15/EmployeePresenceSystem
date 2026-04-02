import React, {
  useEffect,
  useMemo,
  useState,
  useCallback,
} from "react";
import API from "../../services/api";

import Navbar from "../../components/Navbar";
import SummaryCards from "../../components/SummaryCards";
import FiltersPanel from "../../components/FilterPanel";
import EmployeeTable from "../../components/EmployeeTable";
import Charts from "../../components/Charts";
import EmployeeDetailsModal from "../../components/EmployeeDetailsModal";

import "./Dashboard.css";

const Dashboard = ({ setIsAuthenticated }) => {
  const [employees, setEmployees] = useState([]);
  const [selectedEmployee, setSelectedEmployee] = useState(null);
  const [loading, setLoading] = useState(true);

  const [filters, setFilters] = useState({
  search: "",
  department: "",
  team: "",
  company: "",
  fromDate: "",
  toDate: "",
  status: ""
});

  const calculateDuration = (loginTime, logoutTime) => {
    if (!loginTime || !logoutTime) return null;

    const login = new Date(loginTime);
    const logout = new Date(logoutTime);

    const diffMs = logout.getTime() - login.getTime();

    if (isNaN(diffMs) || diffMs <= 0) return null;

    return (diffMs / (1000 * 60 * 60)).toFixed(2);
  };

  const formatDate = (value) => {
    if (!value) return null;
    return new Date(value).toLocaleString();
  };

  const fetchPresenceData = useCallback(async () => {
  try {
    setLoading(true);

    const url = filters.date
      ? `/presence?date=${filters.date}`
      : "/presence";

    const res = await API.get(url);

    const formatted = res.data.map((item, index) => ({
      id: item.id || index,
      name: item.employeeName || "N/A",
      department: item.department || "",
      team: item.team || "",
      designation: item.designation || "",
      status: item.status || "Remote",
      firstLogin: formatDate(item.loginTime),
      lastLogout: formatDate(item.logoutTime),
      duration: calculateDuration(
        item.loginTime,
        item.logoutTime
      ),
      date: item.loginTime || new Date().toISOString(),
      sessions: item.sessions || [],
    }));

    setEmployees(formatted);
  } catch (err) {
    console.error(err);
    alert("Failed to load presence data from backend");
  } finally {
    setLoading(false);
  }
}, [filters.date]);

  useEffect(() => {
    fetchPresenceData();
  }, [fetchPresenceData]);

  const departments = useMemo(
    () => [...new Set(employees.map((e) => e.department).filter(Boolean))],
    [employees]
  );

  const teams = useMemo(
    () => [...new Set(employees.map((e) => e.team).filter(Boolean))],
    [employees]
  );

  const filteredEmployees = useMemo(() => {
    return employees.filter((emp) => {
      const matchesSearch = emp.name
        .toLowerCase()
        .includes(filters.search.toLowerCase());

      const matchesDepartment =
        !filters.department || emp.department === filters.department;

      const matchesTeam =
        !filters.team || emp.team === filters.team;

      const matchesStatus =
        !filters.status || emp.status === filters.status;

      const matchesDate =
      !filters.date ||
      new Date(emp.date).toISOString().split("T")[0] === filters.date;

      return (
  matchesSearch &&
  matchesDepartment &&
  matchesTeam &&
  matchesStatus &&
  matchesDate
);
    });
  }, [employees, filters]);

  return (
    <div className="dashboard-page">
      <Navbar setIsAuthenticated={setIsAuthenticated} />

      <SummaryCards employees={filteredEmployees} />

      <FiltersPanel
        filters={filters}
        setFilters={setFilters}
        departments={departments}
        teams={teams}
      />

      {loading ? (
        <div className="loading-card">
          Loading employee presence data...
        </div>
      ) : (
        <>
          <EmployeeTable
            employees={filteredEmployees}
            onRowClick={setSelectedEmployee}
          />

          <Charts presenceRecords={filteredEmployees} />
        </>
      )}

      {selectedEmployee && (
        <EmployeeDetailsModal
          employee={selectedEmployee}
          onClose={() => setSelectedEmployee(null)}
        />
      )}
    </div>
  );
};

export default Dashboard;