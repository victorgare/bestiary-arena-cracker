"use client";
import { DetailsSection } from "@/components/DashboardDetails/DetailsSection";
import { DetailsData } from "@/components/DashboardDetails/Model/DetailsData";
import { useParams, useRouter } from "next/navigation";
import { useEffect, useState } from "react";

export default function RoomDetails() {
  const { roomId } = useParams();
  const router = useRouter();
  const [data, setData] = useState<DetailsData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetch(`${process.env.NEXT_PUBLIC_API_BASEURL}/dashboards/room/${roomId}/details`)
      .then((res) => res.json())
      .then(setData)
      .finally(() => setLoading(false));
  }, [roomId]);

  return (
    <div
      className="min-h-screen flex flex-col items-center justify-center py-12 px-4"
      style={{ background: "var(--color-bg)", color: "var(--color-text)" }}
    >
      <div
        className="w-full max-w-6xl rounded-xl p-8"
        style={{
          background: "var(--color-card)",
          border: "2px solid var(--color-border)",
          boxShadow: "0 4px 24px 0 #000a",
        }}
      >
        {/* Breadcrumb */}
        <nav className="mb-4 text-sm" aria-label="Breadcrumb">
          <span
            className="cursor-pointer hover:underline"
            style={{ color: "var(--color-gold)" }}
            onClick={() => router.push("/")}
          >
            Home
          </span>
          <span style={{ color: "var(--color-muted)" }}> / </span>
          <span style={{ color: "var(--color-gold)" }}>Room</span>
          <span style={{ color: "var(--color-muted)" }}> / </span>
          <span>{roomId}</span>
        </nav>

        {/* Back Button */}
        <button
          onClick={() => router.back()}
          className="mb-6 px-4 py-2 rounded bg-[var(--color-gold)] text-[var(--color-card)] font-bold hover:bg-yellow-300 transition"
        >
          ← Back
        </button>

        <h1 className="text-3xl font-bold mb-4" style={{ color: "var(--color-gold)" }}>
          Room Details
        </h1>
        <p className="mb-6">
          <span style={{ color: "var(--color-muted)" }}>Room ID:</span> {roomId}
        </p>

        {loading && (
          <div className="text-center" style={{ color: "var(--color-muted)" }}>
            Loading...
          </div>
        )}

         {!loading && data && (
          <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
            <div
              className="rounded-xl p-4 h-full"
              style={{
                background: "var(--color-panel)",
                border: "2px solid var(--color-border)",
                boxShadow: "0 2px 8px 0 #0004",
                minHeight: 200,
              }}
            >
              <DetailsSection title="Least Ticks" compositions={data.leastTicks} />
            </div>
            <div
              className="rounded-xl p-4 h-full"
              style={{
                background: "var(--color-panel)",
                border: "2px solid var(--color-border)",
                boxShadow: "0 2px 8px 0 #0004",
                minHeight: 200,
              }}
            >
              <DetailsSection title="Highest Points" compositions={data.highestPoints} />
            </div>
            <div
              className="rounded-xl p-4 h-full"
              style={{
                background: "var(--color-panel)",
                border: "2px solid var(--color-border)",
                boxShadow: "0 2px 8px 0 #0004",
                minHeight: 200,
              }}
            >
              <DetailsSection title="Highest Victory Rate" compositions={data.highestVictoryRate} />
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
