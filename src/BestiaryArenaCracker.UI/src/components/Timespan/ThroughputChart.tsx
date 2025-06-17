"use client";
import React from "react";

type Point = { date: string; count: number };

export function ThroughputChart({ data }: { data: Point[] }) {
  if (!data.length) {
    return (
      <div className="text-center" style={{ color: "var(--color-muted)" }}>
        No data
      </div>
    );
  }

  const max = Math.max(...data.map((d) => d.count));

  return (
    <div className="flex items-end gap-2 h-40 w-full">
      {data.map((p) => (
        <div key={p.date} className="flex flex-col items-center flex-1">
          <div
            className="w-full"
            style={{
              height: `${(p.count / max) * 100}%`,
              background: "var(--color-gold)",
            }}
          />
          <span
            className="text-xs mt-1"
            style={{ color: "var(--color-muted)" }}
          >
            {p.date.slice(5)}
          </span>
        </div>
      ))}
    </div>
  );
}
