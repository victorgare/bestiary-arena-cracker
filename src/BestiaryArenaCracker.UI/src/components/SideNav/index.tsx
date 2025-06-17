"use client";
import Link from "next/link";
import { usePathname } from "next/navigation";

export default function SideNav() {
  const pathname = usePathname();

  const links = [
    { href: "/", label: "Dashboard" },
    { href: "/timespan", label: "Timespan Dashboard" },
  ];

  return (
    <nav className="flex md:flex-col gap-2 p-4 md:p-6 w-full md:w-60 bg-[var(--color-panel)] text-[var(--color-gold)] border-b-2 md:border-b-0 md:border-r-2 border-[var(--color-border)] md:min-h-screen">
      <h2 className="mb-4 font-bold" style={{ color: "var(--color-gold)" }}>
        Navigation
      </h2>
      {links.map((link) => (
        <Link
          key={link.href}
          href={link.href}
          className={`block px-3 py-2 rounded transition-colors hover:bg-[var(--color-card)] ${
            pathname === link.href ? "bg-[var(--color-card)]" : ""
          }`}
          style={{ textDecoration: "none" }}
        >
          {link.label}
        </Link>
      ))}
    </nav>
  );
}
