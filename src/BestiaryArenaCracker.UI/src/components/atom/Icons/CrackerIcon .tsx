import React from "react";

interface CrackerIconProps {
  size?: number;
  className?: string;
}

export const CrackerIcon: React.FC<CrackerIconProps> = ({ size = 48, className = "" }) => {
  return (
    <svg
      width={size}
      height={size}
      viewBox="0 0 48 48"
      fill="none"
      className={className}
      xmlns="http://www.w3.org/2000/svg"
    >
      {/* Corpo do cracker */}
      <rect
        x="6"
        y="6"
        width="36"
        height="36"
        rx="8"
        fill="#FFD180"
        stroke="#F9A825"
        strokeWidth="2"
      />
      {/* Pontinhos do cracker */}
      <circle cx="16" cy="16" r="1.5" fill="#F57C00" />
      <circle cx="24" cy="16" r="1.5" fill="#F57C00" />
      <circle cx="32" cy="16" r="1.5" fill="#F57C00" />
      <circle cx="16" cy="24" r="1.5" fill="#F57C00" />
      <circle cx="24" cy="24" r="1.5" fill="#F57C00" />
      <circle cx="32" cy="24" r="1.5" fill="#F57C00" />
      <circle cx="16" cy="32" r="1.5" fill="#F57C00" />
      <circle cx="24" cy="32" r="1.5" fill="#F57C00" />
      <circle cx="32" cy="32" r="1.5" fill="#F57C00" />
    </svg>
  );
};
