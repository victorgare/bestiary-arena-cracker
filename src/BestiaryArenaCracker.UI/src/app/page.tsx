"use client";
import { CrackerIcon } from "@/components/atom/Icons/CrackerIcon ";
import { ProgressBar } from "@/components/atom/ProgressBar";
import { DashboardList } from "@/components/DashboardSummary/DashboardList";
import { DashboardSummary } from "@/components/DashboardSummary/Model/DashboardSummary";
import { useEffect, useState, useRef } from "react";

const REFRESH_INTERVAL = 60; // seconds

export default function Home() {
  const [summaries, setSummaries] = useState<DashboardSummary[]>([]);
  const [loading, setLoading] = useState(true);
  const [secondsLeft, setSecondsLeft] = useState(REFRESH_INTERVAL);
  const timerRef = useRef<NodeJS.Timeout | null>(null);

  const baseUrl = process.env.NEXT_PUBLIC_API_BASEURL;

  // Fetch data function
  const fetchData = () => {
    setLoading(true);
    fetch(`${baseUrl}/dashboards/summary`)
      .then((res) => res.json())
      .then((data) => setSummaries(data))
      .catch(() => setSummaries([]))
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    fetchData();
    setSecondsLeft(REFRESH_INTERVAL);

    timerRef.current = setInterval(() => {
      setSecondsLeft((prev) => {
        if (prev <= 1) {
          fetchData();
          return REFRESH_INTERVAL;
        }
        return prev - 1;
      });
    }, 1000);

    return () => {
      if (timerRef.current) clearInterval(timerRef.current);
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [baseUrl]);

  // Calculate overall progress
  const totalRuns = summaries.reduce((acc, s) => acc + s.totalResults, 0);
  const totalPossibilities = summaries.reduce(
    (acc, s) => acc + s.totalCompositions,
    0
  );
  const percent =
    totalPossibilities > 0
      ? Math.round((totalRuns / totalPossibilities) * 100)
      : 0;

  return (
    <div
      className="min-h-screen flex flex-col items-center justify-between py-12 px-4"
      style={{ background: "var(--color-bg)", color: "var(--color-text)" }}
    >
      <main className="flex flex-col gap-10 items-center w-full max-w-6xl mx-auto">
        <h1
          className="heading text-4xl mb-4 flex items-center gap-2"
          style={{ color: "var(--color-gold)" }}
        >
          Bestiary Arena{" "}
          <span className="inline-block align-middle">
            <CrackerIcon />
          </span>
        </h1>
        {/* Overall Progress */}
        <div
          className="w-full max-w-2xl mb-6 rounded-xl shadow"
          style={{
            background: "var(--color-panel)",
            border: "2px solid var(--color-border)",
            padding: "1.5rem 2rem",
          }}
        >
          <h2 className="text-lg mb-2" style={{ color: "var(--color-gold)" }}>
            Overall Progress
          </h2>
          <ProgressBar
            label="Total Runs"
            value={totalRuns}
            max={totalPossibilities}
            percent={percent}
            color="var(--color-gold)"
          />
          <div className="mt-1 text-sm" style={{ color: "var(--color-muted)" }}>
            {totalRuns} / {totalPossibilities} ({percent}%)
          </div>
          <div className="mt-1 text-xs" style={{ color: "var(--color-muted)" }}>
            Auto-refresh in <b>{secondsLeft}s</b>
          </div>
        </div>
        <div
          className="card w-full flex flex-col items-center gap-6"
          style={{
            background: "var(--color-card)",
            border: "2px solid var(--color-border)",
            borderRadius: "1rem",
            boxShadow: "0 4px 24px 0 #000a",
            padding: "2rem",
            width: "100%",
            maxWidth: "100%",
          }}
        >
          <div className="w-full">
            <h2 className="text-xl mb-4" style={{ color: "var(--color-gold)" }}>
              Dashboard Summary
            </h2>
            {loading ? (
              <div
                className="text-center py-8"
                style={{ color: "var(--color-muted)" }}
              >
                Loading...
              </div>
            ) : (
              <DashboardList summaries={summaries} />
            )}
          </div>
        </div>
      </main>
      <footer
        className="mt-12 flex gap-8"
        style={{ color: "var(--color-muted)" }}
      >
        <a
          href="https://nextjs.org/learn"
          target="_blank"
          rel="noopener noreferrer"
        >
          Learn
        </a>
        <a
          href="https://vercel.com/templates?framework=next.js"
          target="_blank"
          rel="noopener noreferrer"
        >
          Examples
        </a>
        <a href="https://nextjs.org" target="_blank" rel="noopener noreferrer">
          Go to nextjs.org →
        </a>
      </footer>
    </div>
  );
}