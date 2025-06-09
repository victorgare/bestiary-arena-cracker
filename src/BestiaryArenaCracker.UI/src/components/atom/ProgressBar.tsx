import React from "react";

type ProgressBarProps = {
  label: string;
  value: number;
  max?: number;
  percent: number;
  color: string;
};

export function ProgressBar({ label, value, max, percent, color }: ProgressBarProps) {
  return (
    <div>
      <div className="flex justify-between text-xs mb-1" style={{ color: "var(--color-muted)" }}>
        <span>{label}</span>
        <span>
          {max !== undefined ? `${value} / ${max}` : value}
        </span>
      </div>
      <div className="w-full h-3 bg-[#232026] rounded">
        <div
          className="h-3 rounded"
          style={{
            width: `${percent}%`,
            background: color,
            transition: "width 0.4s",
          }}
        />
      </div>
    </div>
  );
}