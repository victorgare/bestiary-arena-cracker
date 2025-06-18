"use client";
import React from "react";

type Range = { start: string; end: string };

export function TimeRangePicker({
  value,
  onChange,
}: {
  value: Range;
  onChange: (r: Range) => void;
}) {
  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    onChange({ ...value, [e.target.name]: e.target.value });
  };

  return (
    <div className="flex items-center gap-2 mb-4">
      <input
        type="datetime-local"
        name="start"
        value={value.start}
        onChange={handleChange}
        className="px-2 py-1 rounded bg-[var(--color-input)]"
      />
      <span className="mx-1" style={{ color: "var(--color-muted)" }}>
        to
      </span>
      <input
        type="datetime-local"
        name="end"
        value={value.end}
        onChange={handleChange}
        className="px-2 py-1 rounded bg-[var(--color-input)]"
      />
    </div>
  );
}
