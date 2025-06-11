import Image from "next/image";
import { Composition } from "./Model/Compositions";

export function MonsterModal({
  composition,
  onClose,
}: {
  composition: Composition;
  onClose: () => void;
}) {
  // Build a 15x11 grid (165 tiles)
  const grid = Array.from({ length: 165 }, (_, i) => i + 1);
  // Map tile number to board entry
  const boardByTile = Object.fromEntries(
    (composition.board ?? []).map((b) => [b.tile, b])
  );

  return (
    <div
      className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-60"
      onClick={onClose}
    >
      <div
        className="bg-[var(--color-card)] rounded-xl p-8 max-w-4xl w-full relative"
        style={{
          border: "2px solid var(--color-border)",
          boxShadow: "0 4px 24px 0 #000a",
          minHeight: 600,
        }}
        onClick={(e) => e.stopPropagation()}
      >
        <button
          className="absolute top-2 right-4 text-2xl font-bold"
          style={{ color: "var(--color-gold)" }}
          onClick={onClose}
        >
          ×
        </button>
        <h3
          className="text-2xl font-bold mb-4"
          style={{ color: "var(--color-gold)" }}
        >
          Composition #{composition.compositionId} - Board
        </h3>
        <div className="mb-6 flex flex-wrap gap-4">
          {(composition.board ?? []).map((b, idx) => (
            <div
              key={idx}
              className="rounded bg-[var(--color-panel)] px-4 py-3 text-sm"
              style={{
                border: "1px solid var(--color-border)",
                color: "var(--color-gold)",
                minWidth: 160,
              }}
            >
              <div>
                <b>{b.monster.name}</b> (Lvl {b.monster.level})
              </div>
              <div>
                HP: {b.monster.hp} | AD: {b.monster.ad} | AP: {b.monster.ap}
              </div>
              <div>
                ARM: {b.monster.armor} | MR: {b.monster.magicResist}
              </div>
              <div>
                Equipment: {b.equipment.name} (Stat: {b.equipment.stat}, Tier:{" "}
                {b.equipment.tier})
              </div>
              <div>Tile: {b.tile}</div>
            </div>
          ))}
        </div>
        <div className="overflow-auto">
          <div
            className="grid"
            style={{
              gridTemplateColumns: "repeat(15, 40px)",
              gridTemplateRows: "repeat(11, 40px)",
              gap: 3,
              background: "#18151a",
              border: "1px solid var(--color-border)",
              margin: "0 auto",
              width: 15 * 40 + 44,
              maxWidth: "100%",
            }}
          >
            {grid.map((tile) => {
              const boardEntry = boardByTile[tile];
              return (
                <div
                  key={tile}
                  className="flex flex-col items-center justify-center text-xs rounded overflow-hidden"
                  style={{
                    background: boardEntry
                      ? "var(--color-gold)"
                      : "var(--color-panel)",
                    color: boardEntry ? "#232026" : "var(--color-muted)",
                    border: "1px solid var(--color-border)",
                    height: 40,
                    width: 40,
                    fontWeight: boardEntry ? "bold" : "normal",
                    fontSize: boardEntry ? 16 : 11,
                    padding: 0,
                  }}
                  title={
                    boardEntry
                      ? `${boardEntry.monster.name} (${boardEntry.equipment.name}, ${boardEntry.equipment.stat}, Tier ${boardEntry.equipment.tier})`
                      : undefined
                  }
                >
                  {boardEntry ? (
                    <>
                      <Image
                        src={`/portraits/${boardEntry.monster.name}.png`}
                        alt={boardEntry.monster.name}
                        width={38}
                        height={38}
                        style={{
                          objectFit: "cover",
                          borderRadius: 4,
                          background: "#232026",
                        }}
                      />
                      {/* Optionally show equipment icon or stat below */}
                    </>
                  ) : (
                    tile
                  )}
                </div>
              );
            })}
          </div>
        </div>
      </div>
    </div>
  );
}
