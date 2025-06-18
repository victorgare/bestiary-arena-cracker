"use client";
import React from "react";

type Point = { date: string; compositions: number; results: number };

export function ThroughputChart({ data }: { data: Point[] }) {
  if (!data.length) {
    return (
      <div className="text-center" style={{ color: "var(--color-muted)" }}>
        No data
      </div>
    );
  }

  const max = Math.max(...data.map((d) => Math.max(d.compositions, d.results)));

  return (
    <div className="flex items-end gap-2 h-40 w-full">
      {data.map((p) => (
        <div key={p.date} className="flex flex-col items-center flex-1">
          <div className="flex w-full items-end gap-1">
            <div
              className="flex-1"
              style={{
                height: `${(p.compositions / max) * 100}%`,
                background: "var(--color-gold)",
              }}
            />
            <div
              className="flex-1"
              style={{
                height: `${(p.results / max) * 100}%`,
                background: "var(--color-gold-dark)",
              }}
            />
          </div>
          <span className="text-xs mt-1" style={{ color: "var(--color-muted)" }}>
            {p.date.slice(5)}
          </span>
        </div>
      ))}
    </div>
  );
}
