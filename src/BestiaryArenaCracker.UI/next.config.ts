import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  env: {
    NEXT_PUBLIC_API_BASEURL:
      process.env["services__bestiaryarenacracker-api__https__0"],
  },
};

export default nextConfig;
