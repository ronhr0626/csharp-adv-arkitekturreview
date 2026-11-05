# Architecture Decision Record

## Problem

The API reads the token, but never checks if it is real or expired.
A fake token from jwt.io still works.

## Alternatives

**A: Check the token ourselves in `TokenReader`**

- Pros: Small change, easy to understand.
- Cons: Must remember the check in every controller.
- Complexity: Low.
- Security: Only as good as our own code.
- Maintenance: Every new endpoint needs the same check.

**B: Use ASP.NET Core's built-in check (`AddJwtBearer` + `[Authorize]`)**

- Pros: Standard solution, less code in controllers.
- Cons: More happens behind the scenes.
- Complexity: Some setup in `Program.cs`, then simple.
- Security: High. Checks signature and expiry.
- Maintenance: Easy. Just add `[Authorize]`.

## Decision

We choose **B**.

## Why?

It is the standard way in ASP.NET Core, it is safer than our own code, and new endpoints only need `[Authorize]`.

## Consequences

- Fake or expired tokens get `401 Unauthorized`.
- The controllers get shorter.
- The secret must be the same where the token is made and where it is checked.
- The secret is still in `appsettings.json` (see weakness 2).