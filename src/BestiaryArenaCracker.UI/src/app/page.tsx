import Image from "next/image";

export default function Home() {
  return (
    <div className="min-h-screen flex flex-col items-center justify-between py-12 px-4" style={{ background: "var(--color-bg)", color: "var(--color-text)" }}>
      <main className="flex flex-col gap-10 items-center w-full max-w-2xl">
        <h1 className="heading text-4xl mb-4" style={{ color: "var(--color-gold)" }}>Bestiary Arena Cracker</h1>
        <div className="card w-full flex flex-col items-center gap-6" style={{
          background: "var(--color-card)",
          border: "2px solid var(--color-border)",
          borderRadius: "1rem",
          boxShadow: "0 4px 24px 0 #000a",
          padding: "2rem"
        }}>
          <Image
            src="/next.svg"
            alt="Next.js logo"
            width={120}
            height={32}
            className="mb-2"
          />
          <ol className="list-decimal list-inside text-lg" style={{ color: "var(--color-muted)" }}>
            <li>
              Get started by editing{" "}
              <code style={{ background: "var(--color-input)", padding: "0.2em 0.5em", borderRadius: "0.25em" }}>{`src/app/page.tsx`}</code>
              .
            </li>
            <li>Save and see your changes instantly.</li>
          </ol>
          <div className="flex gap-4 mt-4">
            <a
              className="btn"
              style={{
                background: "var(--color-gold)",
                color: "#232026",
                borderRadius: "0.5em",
                fontWeight: "bold",
                padding: "0.75rem 2rem",
                boxShadow: "0 2px 8px 0 #0006",
                border: "none"
              }}
              href="https://vercel.com/new?utm_source=create-next-app&utm_medium=appdir-template-tw&utm_campaign=create-next-app"
              target="_blank"
              rel="noopener noreferrer"
            >
              Deploy now
            </a>
            <a
              className="btn"
              style={{
                background: "var(--color-btn)",
                color: "var(--color-gold)",
                border: "2px solid var(--color-gold)",
                borderRadius: "0.5em",
                fontWeight: "bold",
                padding: "0.75rem 2rem"
              }}
              href="https://nextjs.org/docs?utm_source=create-next-app&utm_medium=appdir-template-tw&utm_campaign=create-next-app"
              target="_blank"
              rel="noopener noreferrer"
            >
              Read our docs
            </a>
          </div>
        </div>
      </main>
      <footer className="mt-12 flex gap-8" style={{ color: "var(--color-muted)" }}>
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
        <a
          href="https://nextjs.org"
          target="_blank"
          rel="noopener noreferrer"
        >
          Go to nextjs.org →
        </a>
      </footer>
    </div>
  );
}