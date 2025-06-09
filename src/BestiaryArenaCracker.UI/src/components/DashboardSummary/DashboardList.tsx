import React from "react";
import { DashboardSummary } from "./Model/DashboardSummary";
import { DashboardCard } from "./DashboardCard";

// Dummy image mapping, replace with your own logic later
function getRoomImage(roomId: string) {
  return `/rooms/${roomId}.png`;
}

export function DashboardList({
  summaries,
}: {
  summaries: DashboardSummary[];
}) {
  if (!summaries.length) {
    return (
      <div
        className="w-full text-center py-8"
        style={{ color: "var(--color-muted)" }}
      >
        No data available.
      </div>
    );
  }

  // Find max for progress bars
  const maxResults = Math.max(...summaries.map((s) => s.totalResults), 1);
  const maxCompositions = Math.max(
    ...summaries.map((s) => s.totalCompositions),
    1
  );

  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4 w-full">
      {summaries.map((summary) => (
        <DashboardCard
          key={summary.roomId}
          summary={summary}
          maxResults={maxResults}
          maxCompositions={maxCompositions}
          imageSrc={getRoomImage(summary.roomId)}
        />
      ))}
    </div>
  );
}
