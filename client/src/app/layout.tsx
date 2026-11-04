import type { Metadata } from "next";
import { Geist, Geist_Mono } from "next/font/google";
import "./globals.css";
import { AppShell } from "@/features/app-shell/app-shell";
import { sidebarInitScript } from "@/shared/lib/sidebar-preference";
import { themeInitScript } from "@/shared/lib/theme-preference";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "Educon",
  description: "Educon mediaplatform",
};

export default function RootLayout({ children }: LayoutProps<"/">) {
  return (
    <html
      lang="ru"
      data-sidebar-open="true"
      suppressHydrationWarning
      className={`${geistSans.variable} ${geistMono.variable} dark h-full antialiased`}
    >
      <head>
        <script id="theme-preference" dangerouslySetInnerHTML={{ __html: themeInitScript }} />
        <script
          id="sidebar-preference"
          dangerouslySetInnerHTML={{ __html: sidebarInitScript }}
        />
      </head>
      <body className="min-h-full flex flex-col">
        <AppShell>{children}</AppShell>
      </body>
    </html>
  );
}
