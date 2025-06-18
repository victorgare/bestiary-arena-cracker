"use client";
import { TimeRangePicker } from "@/components/atom/TimeRangePicker.tsx";
import { ThroughputChart } from "@/components/Timespan/ThroughputChart";
import { StatsCard } from "@/components/Timespan/StatsCard";
import { useEffect, useState } from "react";

type TimespanData = {
  totalCompositions: number;
  totalResults: number;
  averageCompositionsPerHour: number;
  averageResultsPerHour: number;
  points: { date: string; compositions: number; results: number }[];
};

export default function TimespanPage() {
  const now = new Date();
  const toIso = (d: Date) => d.toISOString().slice(0, 16);
  const [range, setRange] = useState({
    start: toIso(new Date(now.getTime() - 24 * 60 * 60 * 1000)),
    end: toIso(now),
  });
  const [data, setData] = useState<TimespanData | null>(null);
  const [loading, setLoading] = useState(false);

  const baseUrl = process.env.NEXT_PUBLIC_API_BASEURL;

  useEffect(() => {
    if (!range.start || !range.end) return;
    setLoading(true);
    fetch(
      `${baseUrl}/dashboards/timespan?start=${range.start}&end=${range.end}`
    )
      .then((res) => res.json())
      .then(setData)
      .catch(() => setData(null))
      .finally(() => setLoading(false));
  }, [range, baseUrl]);

  return (
    <div className="p-8">
      <h1 className="text-3xl font-bold mb-4" style={{ color: "var(--color-gold)" }}>
        Timespan Dashboard
      </h1>
      <TimeRangePicker value={range} onChange={setRange} />
      {loading && (
        <div className="text-center" style={{ color: "var(--color-muted)" }}>
          Loading...
        </div>
      )}
      {data && (
        <div className="flex flex-col gap-24 mt-12">
          <div className="grid grid-cols-2 sm:grid-cols-4 gap-4">
            <StatsCard label="Compositions" value={data.totalCompositions} />
            <StatsCard label="Results" value={data.totalResults} />
            <StatsCard
              label="Avg Comp/h"
              value={data.averageCompositionsPerHour.toFixed(2)}
            />
            <StatsCard
              label="Avg Res/h"
              value={data.averageResultsPerHour.toFixed(2)}
            />
          </div>
          <ThroughputChart data={data.points} />
        </div>
      )}
    </div>
  );
}
