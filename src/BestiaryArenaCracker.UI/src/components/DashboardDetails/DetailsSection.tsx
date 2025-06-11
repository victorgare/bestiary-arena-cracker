import { useState } from "react";
import { Composition } from "./Model/Compositions";
import { MonsterModal } from "./MonsterModal";

export function DetailsSection({
  title,
  compositions,
}: {
  title: string;
  compositions: Composition[];
}) {
  const [selected, setSelected] = useState<Composition | null>(null);

  return (
    <>
      <h2
        className="text-lg font-bold mb-4"
        style={{ color: "var(--color-gold)" }}
      >
        {title}
      </h2>
      <div className="flex flex-col gap-4">
        {compositions.map((comp, idx) => (
          <button
            key={comp.compositionId}
            className={`rounded-lg p-4 text-left transition hover:scale-[1.02]`}
            style={{
              background:
                idx === 0
                  ? "#f3f0d1"
                  : "var(--color-panel)",
              border:
                idx === 0
                  ? "2px solid var(--color-gold)"
                  : "1px solid var(--color-border)",
              boxShadow:
                idx === 0 ? "0 2px 12px 0 #c2a93a33" : "0 1px 4px #0002",
              cursor: "pointer",
              color: idx === 0 ? "#232026" : undefined,
              position: "relative",
              overflow: "hidden",
            }}
            onClick={() => setSelected(comp)}
          >
            {idx === 0 && (
              <div
                style={{
                  position: "absolute",
                  inset: 0,
                  background:
                    "radial-gradient(circle at 60% 40%, #fffbe6cc 0%, #f6e9b700 80%)",
                  zIndex: 0,
                  pointerEvents: "none",
                }}
              />
            )}
            <div
              className="mb-1 font-bold relative"
              style={{
                color: idx === 0 ? "#b89c2a" : "var(--color-gold)",
                zIndex: 1,
              }}
            >
              Composition #{comp.compositionId}
            </div>
            <div
              className="flex flex-wrap gap-4 text-xs relative"
              style={{
                color: idx === 0 ? "#232026" : "var(--color-muted)",
                zIndex: 1,
              }}
            >
              <span>
                <b>Min Ticks:</b> {comp.minTicks}
              </span>
              <span>
                <b>Max Points:</b> {comp.maxPoints}
              </span>
              <span>
                <b>Results:</b> {comp.totalResults}
              </span>
              <span>
                <b>Victories:</b> {comp.victoryCount}
              </span>
              <span>
                <b>Victory Rate:</b> {(comp.victoryRate * 100).toFixed(0)}%
              </span>
            </div>
          </button>
        ))}
      </div>
      {selected && (
        <MonsterModal
          composition={selected}
          onClose={() => setSelected(null)}
        />
      )}
    </>
  );
}
