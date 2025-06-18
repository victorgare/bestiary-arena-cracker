"use client";
import React from "react";
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  BarElement,
  Tooltip,
  Legend,
} from "chart.js";
import { Bar } from "react-chartjs-2";

type Point = { date: string; compositions: number; results: number };

ChartJS.register(CategoryScale, LinearScale, BarElement, Tooltip, Legend);

export function ThroughputChart({ data }: { data: Point[] }) {
  if (!data.length) {
    return (
      <div className="text-center" style={{ color: "var(--color-muted)" }}>
        No data
      </div>
    );
  }

  const style = getComputedStyle(document.documentElement);
  const colorGold = style.getPropertyValue("--color-gold").trim();
  const colorGoldDark = style.getPropertyValue("--color-gold-dark").trim();
  const colorText = style.getPropertyValue("--color-text").trim();

  const chartData = {
    labels: data.map((d) => d.date.slice(5)),
    datasets: [
      {
        label: "Compositions",
        data: data.map((d) => d.compositions),
        backgroundColor: colorGold,
      },
      {
        label: "Results",
        data: data.map((d) => d.results),
        backgroundColor: colorGoldDark,
      },
    ],
  };

  const options = {
    responsive: true,
    maintainAspectRatio: false,
    scales: {
      x: {
        ticks: { color: colorText },
      },
      y: {
        beginAtZero: true,
        ticks: { color: colorText },
      },
    },
    plugins: {
      legend: {
        labels: { color: colorText },
      },
    },
  } as const;

  return (
    <div style={{ height: "10rem" }}>
      <Bar data={chartData} options={options} />
    </div>
  );
}
