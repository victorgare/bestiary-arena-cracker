import Image from "next/image";
import { DashboardSummary } from "./Model/DashboardSummary";
import { ProgressBar } from "../atom/ProgressBar";

type DashboardCardProps = {
  summary: DashboardSummary;
  maxResults: number;
  maxCompositions: number;
  imageSrc: string;
};

export function DashboardCard({ summary, imageSrc }: DashboardCardProps) {
  const progressPercent =
    summary.totalCompositions > 0
      ? Math.min(
          100,
          Math.round((summary.totalResults / summary.totalCompositions) * 100)
        )
      : 0;

  return (
    <div
      className="flex flex-col w-full rounded-xl overflow-hidden"
      style={{
        background: "var(--color-card)",
        border: "2px solid var(--color-border)",
        boxShadow: "0 2px 8px 0 #000a",
        minHeight: 160,
        maxWidth: 320,
        margin: "0 auto",
      }}
    >
      <div
        className="relative w-full"
        style={{ aspectRatio: "4/3", background: "#18151a" }}
      >
        <Image
          src={imageSrc}
          alt={summary.roomName}
          fill
          style={{ objectFit: "cover" }}
          sizes="(max-width: 768px) 100vw, 100vw"
        />
        <span
          className="absolute top-2 right-2 px-2 py-1 rounded font-bold text-xs"
          style={{
            background: "var(--color-gold)",
            color: "#232026",
            boxShadow: "0 1px 4px #000a",
          }}
        >
          {summary.grade}
        </span>
      </div>
      <div
        className="flex flex-col gap-1 px-2 py-2"
        style={{ background: "var(--color-panel)" }}
      >
        <div className="flex justify-between items-center mb-1">
          <span
            className="font-bold text-base"
            style={{ color: "var(--color-gold)" }}
          >
            {summary.roomName}
          </span>
          <span style={{ color: "var(--color-muted)", fontSize: 11 }}>
            {summary.ticks}t • {summary.points}pt
          </span>
        </div>
        <ProgressBar
          label="Progress"
          value={summary.totalResults}
          max={summary.totalCompositions}
          percent={progressPercent}
          color="var(--color-gold)"
        />
      </div>
    </div>
  );
}
