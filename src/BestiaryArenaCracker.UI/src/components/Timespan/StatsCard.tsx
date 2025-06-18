"use client";

export function StatsCard({
  label,
  value,
}: {
  label: string;
  value: string | number;
}) {
  return (
    <div
      className="flex flex-col items-center rounded-xl"
      style={{
        background: "var(--color-card)",
        border: "2px solid var(--color-border)",
        boxShadow: "0 2px 8px 0 #0006",
        padding: "1rem",
        flex: 1,
        minWidth: 80,
      }}
    >
      <span className="text-xs" style={{ color: "var(--color-muted)" }}>
        {label}
      </span>
      <span className="text-lg font-bold" style={{ color: "var(--color-gold)" }}>
        {value}
      </span>
    </div>
  );
}
