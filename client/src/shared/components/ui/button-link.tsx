"use client";

import Link from "next/link";
import type { ComponentProps } from "react";
import type { VariantProps } from "class-variance-authority";
import { Button, buttonVariants } from "./button";

export function ButtonLink({ className, variant, size, ...props }:
  ComponentProps<typeof Link> & VariantProps<typeof buttonVariants>) {
  return <Button nativeButton={false} render={<Link {...props} />} variant={variant} size={size} className={className} />;
}
