import React, { useMemo } from "react";
import "./SummaryCards.css";

const SummaryCards = ({ employees = [] }) => {
  const stats = useMemo(() => {
    const total = employees.length;

    const office = employees.filter(
      (emp) => emp.status === "Office"
    ).length;

    const remote = employees.filter(
      (emp) => emp.status === "Remote"
    ).length;

    const totalDuration = employees.reduce((sum, emp) => {
      const duration = parseFloat(emp.duration || 0);
      return sum + (isNaN(duration) ? 0 : duration);
    }, 0);

    const avg = total > 0
      ? (totalDuration / total).toFixed(2)
      : 0;

    return {
      total,
      office,
      remote,
      avg,
    };
  }, [employees]);

  const cards = [
    {
      title: "Total Employees",
      value: stats.total,
      className: "card total",
    },
    {
      title: "In Office",
      value: stats.office,
      className: "card online",
    },
    {
      title: "Remote",
      value: stats.remote,
      className: "card offline",
    },
    {
      title: "Avg Duration",
      value: `${stats.avg} hrs`,
      className: "card avg",
    },
  ];

  return (
    <div className="summary-grid">
      {cards.map((card, index) => (
        <div key={index} className={card.className}>
          <h3>{card.title}</h3>
          <p>{card.value}</p>
        </div>
      ))}
    </div>
  );
};

export default SummaryCards;