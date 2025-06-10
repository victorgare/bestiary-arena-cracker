"use client";
import { useParams, useRouter } from "next/navigation";

export default function RoomDetails() {
  const { roomId } = useParams();
  const router = useRouter();

  return (
    <div
      className="min-h-screen flex flex-col items-center justify-center py-12 px-4"
      style={{ background: "var(--color-bg)", color: "var(--color-text)" }}
    >
      <div
        className="w-full max-w-2xl rounded-xl p-8"
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

        <h1
          className="text-3xl font-bold mb-4"
          style={{ color: "var(--color-gold)" }}
        >
          Room Details
        </h1>
        <p className="mb-2">
          <span style={{ color: "var(--color-muted)" }}>Room ID:</span> {roomId}
        </p>
        <div
          className="mt-8 text-center"
          style={{ color: "var(--color-muted)" }}
        >
          {/* Add more details here */}
          Details coming soon...
        </div>
      </div>
    </div>
  );
}