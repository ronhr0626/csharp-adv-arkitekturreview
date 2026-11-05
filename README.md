# C# Advanced – Arkitekturreview

Four things in the system that could be better.

![Architecture](Excalidraw/Architecture.png)

## 1. The token is not checked

**Problem:** The API reads the token, but never checks if it is real or expired.

**Consequence:** Anyone can make a fake token on jwt.io and see other users' tasks.

**Solution:** Use ASP.NET Core's built-in JWT check (`AddJwtBearer`) and `[Authorize]`.

**Priority:** High.

## 2. Passwords and secrets are on GitHub

**Problem:** The JWT secret and the database password are in `appsettings.json` and `docker-compose.yaml`, which are public on GitHub.

**Consequence:** Anyone who knows the secret can make valid tokens.

**Solution:** Move them to a `.env` file that is not pushed to GitHub.

**Priority:** High.

## 3. The database is open to the outside

**Problem:** Postgres, pgAdmin and the Traefik dashboard have open ports on the PC.

**Consequence:** You can go straight to the database and skip the API and its rules.

**Solution:** Remove the open ports, so only the API can talk to the database.

**Priority:** Medium.

## 4. AuthController talks directly to the database

**Problem:** `TaskController` uses service and repository, but `AuthController` uses the database directly.

**Consequence:** The code is not built the same way everywhere, and it is harder to test.

**Solution:** Add a `UserRepository`, like we have for tasks.

**Priority:** Low.

## What we fix

Problem 1 is fixed. See [adr.md](adr.md).