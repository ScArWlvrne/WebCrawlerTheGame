# X Bank Terminal Outputs

> **In-universe tool:** `spider-crawl` v0.4.1 remains read-only. It discovers public links and cached page text for fictional puzzle hosts only. It cannot log in, submit forms, run scripts, bypass MFA, or attack real systems.

## Command Reference

| Command | Syntax | Purpose |
|---------|--------|---------|
| `help` | `help` | List available commands |
| `crawl` | `crawl <url>` | Fetch an allowlisted public URL and extract links |
| `urls` | `urls` | Show discovered links |
| `cat` | `cat <url>` | Print cached public content |

Current allowlist: `xbank.com`, `*.xbank.com`.

Start here:

```text
crawl https://www.xbank.com
```

## Main Sequence

```text
[GET] https://www.xbank.com/ -> 200 OK
Title: X Bank -- private banking for public titans
Extracted links:
  /help/security
  /business/executive
  /robots.txt
```

```text
[GET] https://www.xbank.com/robots.txt -> 200 OK
User-agent: *
Allow: /
Disallow: /portal/private-banking/
Disallow: /executive/
# Executive portal moved to online.xbank.com/executive
```

```text
[GET] https://www.xbank.com/help/security -> 200 OK
Executive customers verify username, password, and two profile questions.
Common question families: mother's maiden name, first pet, primary phone.
```

```text
[GET] https://www.xbank.com/portal/private-banking -> 403 Forbidden
HTML comment: exec login moved to online.xbank.com/executive
```

Journal output: `usr/xbank/urls.txt` contains `https://online.xbank.com/executive`.
