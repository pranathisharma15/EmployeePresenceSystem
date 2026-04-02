import React, { useMemo } from "react";
import "./Charts.css";
import {
  AreaChart,
  Area,
  PieChart,
  Pie,
  Cell,
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  Legend,
} from "recharts";

const Charts = ({ presenceRecords = [] }) => {
  const officeVsRemote = useMemo(() => {
    const office = presenceRecords.filter(
      (e) => e.status === "Office"
    ).length;

    const remote = presenceRecords.filter(
      (e) => e.status === "Remote"
    ).length;

    return [
      { name: "Office", value: office },
      { name: "Remote", value: remote },
    ];
  }, [presenceRecords]);

  const departmentData = useMemo(() => {
    const grouped = {};

    presenceRecords.forEach((record) => {
      const dept = record.department || "Unknown";
      grouped[dept] = (grouped[dept] || 0) + 1;
    });

    return Object.keys(grouped).map((dept) => ({
      department: dept,
      count: grouped[dept],
    }));
  }, [presenceRecords]);

  const weeklyData = useMemo(() => {
    const grouped = {
      Mon: 0,
      Tue: 0,
      Wed: 0,
      Thu: 0,
      Fri: 0,
    };

    presenceRecords.forEach((record) => {
      const date = new Date(record.date);
      const day = date.toLocaleDateString("en-US", {
        weekday: "short",
      });

      if (grouped[day] !== undefined) {
        grouped[day] += 1;
      }
    });

    return Object.keys(grouped).map((day) => ({
      day,
      count: grouped[day],
    }));
  }, [presenceRecords]);

  const teamAttendanceData = useMemo(() => {
  const teamMap = {};

  presenceRecords.forEach((record) => {
    const team = record.team || "Unknown";

    if (!teamMap[team]) {
      teamMap[team] = 0;
    }

    if (record.status === "Office") {
      teamMap[team] += 1;
    }
  });

  return Object.keys(teamMap).map((team) => ({
    team,
    attendance: teamMap[team],
  }));
}, [presenceRecords]);

  return (
  <div className="charts-grid">
    <div className="chart-card">
      <h3>Office vs Remote</h3>
      <ResponsiveContainer width="100%" height={280}>
        <PieChart>
          <Pie
            data={officeVsRemote}
            dataKey="value"
            nameKey="name"
            outerRadius={90}
            label
          >
            <Cell fill="#22c55e" />
            <Cell fill="#ef4444" />
          </Pie>
          <Tooltip />
          <Legend />
        </PieChart>
      </ResponsiveContainer>
    </div>

    <div className="chart-card">
      <h3>Attendance by Department</h3>
      <ResponsiveContainer width="100%" height={280}>
        <BarChart data={departmentData}>
          <CartesianGrid strokeDasharray="3 3" />
          <XAxis dataKey="department" />
          <YAxis />
          <Tooltip />
          <Bar dataKey="count" radius={[8, 8, 0, 0]} fill="#2563eb" />
        </BarChart>
      </ResponsiveContainer>
    </div>

    <div className="chart-card">
  <h3>Attendance by Team</h3>
  <ResponsiveContainer width="100%" height={300}>
    <BarChart data={teamAttendanceData}>
      <CartesianGrid strokeDasharray="3 3" />
      <XAxis dataKey="team" interval={0}
  angle={-25}
  textAnchor="end"
  height={80}/>
      <YAxis />
      <Tooltip />
      <Bar dataKey="attendance" />
    </BarChart>
  </ResponsiveContainer>
</div>

    <div className="chart-card full-width">
      <h3>Weekly Attendance Trend</h3>
      <p>Employee office presence count for the current week</p>

      <ResponsiveContainer width="100%" height={300}>
        <AreaChart data={weeklyData}>
          <defs>
            <linearGradient
              id="attendanceGradient"
              x1="0"
              y1="0"
              x2="0"
              y2="1"
            >
              <stop offset="5%" stopColor="#2563eb" stopOpacity={0.3} />
              <stop offset="95%" stopColor="#2563eb" stopOpacity={0.02} />
            </linearGradient>
          </defs>

          <CartesianGrid strokeDasharray="3 3" />
          <XAxis dataKey="day" />
          <YAxis />
          <Tooltip />
          <Area
            type="monotone"
            dataKey="count"
            stroke="#2563eb"
            fill="url(#attendanceGradient)"
            strokeWidth={3}
          />
        </AreaChart>
      </ResponsiveContainer>
    </div>

    
  </div>
);
};

export default Charts;