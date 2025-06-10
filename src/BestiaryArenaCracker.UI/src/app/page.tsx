"use client";
import { CrackerIcon } from "@/components/atom/Icons/CrackerIcon ";
import { DashboardList } from "@/components/DashboardSummary/DashboardList";
import { DashboardSummary } from "@/components/DashboardSummary/Model/DashboardSummary";
import { useEffect, useState } from "react";

export default function Home() {
  const [summaries, setSummaries] = useState<DashboardSummary[]>([]);
  const [loading, setLoading] = useState(true);

  const baseUrl = process.env.NEXT_PUBLIC_API_BASEURL;
  useEffect(() => {
    fetch(`${baseUrl}/dashboards/summary`)
      .then((res) => res.json())
      .then((data) => setSummaries(data))
      .catch(() => setSummaries([]))
      .finally(() => setLoading(false));
  }, [baseUrl]);

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
