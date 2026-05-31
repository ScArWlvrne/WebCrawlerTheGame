# Terminal Outputs — Puzzle Design Spec

> **In-universe tool:** `spider-crawl` v0.4.1 — a read-only link harvester bundled with the player's journal shell. It fetches public HTML, extracts anchor tags, and caches responses locally. It does **not** authenticate, exploit, or bypass access controls.

> **Design filter (mandatory):** All outputs simulate an **information-retrieval puzzle**. They may reference sloppy security *as lore* (misconfigured robots.txt, leaked path names, commented-out links). They must **never** include real penetration-testing workflows, exploit payloads, credential stuffing steps, or instructions for attacking live systems.

---

## Command Reference

| Command | Syntax | Purpose |
|---------|--------|---------|
| `help` | `help [command]` | List commands or show usage for one command |
| `crawl` | `crawl <url>` | Fetch a URL, cache the body, extract same-origin links |
| `urls` | `urls [--discovered\|--cached]` | Show link queue or cached fetch list |
| `cat` | `cat <cache-id\|url>` | Print cached response body (truncated in terminal) |
| `history` | `history` | Show commands run this session |

**Fictional constraints baked into outputs:**
- Only `http://` and `https://` targets on allowlisted puzzle hosts resolve.
- Responses are pre-authored; the engine matches URL → canned output.
- HTTP status codes are narrative signals, not bypass prompts.
- Cross-origin links are listed but marked `[external — not fetched]`.

---

## `help`

### Default (`help`)

```
spider-crawl v0.4.1 — read-only link harvester
Type 'help <command>' for details.

  crawl <url>       Fetch a page and extract links
  urls              List discovered and cached URLs
  cat <target>      Show cached page content
  history           Show this session's command history

Notes:
  • Same-origin links are queued automatically after each crawl.
  • External links are recorded but never fetched.
  • Cached pages persist for this session only.
  • This tool cannot log in, submit forms, or run scripts.

Current allowlist: araknyd.io, *.araknyd.io, *.araknyd.local
Start here: crawl https://www.araknyd.io
```

### `help crawl`

```
Usage: crawl <url>

Fetches one URL over HTTP(S), stores the response body, and parses
<a href="..."> tags on the same host.

Examples:
  crawl https://www.araknyd.io
  crawl https://www.araknyd.io/robots.txt

Limits:
  • Max body size: 256 KB (terminal truncates display)
  • Redirects: followed once; final URL shown in output
  • Auth pages: body may be cached; login forms are not interactive

Tip: If a page looks empty, try 'cat' — some clues live in HTML comments.
```

### `help urls`

```
Usage: urls [--discovered | --cached]

  --discovered   Links extracted but not yet crawled (default)
  --cached       URLs already fetched this session

Discovered links are deduplicated by path. Visiting a URL via 'crawl'
moves it from discovered → cached.
```

### `help cat`

```
Usage: cat <cache-id | url>

Prints a cached response. Accepts either:
  • Numeric cache ID from 'urls --cached'
  • Full URL if that URL was crawled this session

Output is plain text / stripped HTML for readability.
Comment blocks and <meta> tags are preserved when present.
```

### `help history`

```
Usage: history

Lists commands entered this session, oldest first.
Does not persist across game saves (by design — diegetic amnesia).
```

---

## URL Discovery Chain — Araknyd → `/admin-beta`

**Puzzle goal:** Player reaches the hidden admin dashboard at  
`https://www.araknyd.io/admin-beta` (or `https://admin-beta.araknyd.local` if designers prefer subdomain flavor — path version used below).

**Solution path (designer view):**

```
www.araknyd.io
  └─ robots.txt          → Disallow: /admin-beta/  (path name leak)
  └─ /blog               → post footnote mentions "beta dashboard"
  └─ /careers            → footer comment: legacy paths not in sitemap
  └─ /sitemap.xml        → confirms /admin-beta is absent (contradiction)
  └─ /admin-beta         → gated dashboard HTML (view-source puzzle file)
```

Players who skip `robots.txt` can still reach `/admin-beta` via the blog footnote; multiple breadcrumbs reward thorough crawling.

---

## `crawl` — Stage-by-Stage Outputs

### Stage 0 — Session start (optional banner)

```
spider@journal:~$ crawl https://www.araknyd.io
[GET] https://www.araknyd.io/          → 200 OK (14.2 KB)
[cache] id=1

Title: "Araknyd — We Web Better™"
Host: www.araknyd.io

Extracted links (same-origin):
  /about
  /blog
  /careers
  /status
  /robots.txt          ← also declared in <link rel="robots">
  /assets/spider-mascot.png

External (not fetched):
  https://twitter.com/AraknydStartup
  https://jobs.lever.co/araknyd

3 new URLs queued. Use 'urls' to review.
```

---

### Stage 1 — `crawl https://www.araknyd.io/robots.txt`

```
spider@journal:~$ crawl https://www.araknyd.io/robots.txt
[GET] https://www.araknyd.io/robots.txt  → 200 OK (412 B)
[cache] id=2

User-agent: *
Allow: /
Disallow: /admin-beta/
Disallow: /tmp/
Disallow: /internal-export/

Sitemap: https://www.araknyd.io/sitemap.xml

# NOTE FROM DEV (2019): CEO said delete /admin-beta before launch.
# NOTE FROM DEV (2026): still here. not my problem anymore.

Extracted links (same-origin):
  /sitemap.xml

★ Journal hint (auto): robots.txt lists a path not linked on the homepage.
```

---

### Stage 2 — `crawl https://www.araknyd.io/sitemap.xml`

```
spider@journal:~$ crawl https://www.araknyd.io/sitemap.xml
[GET] https://www.araknyd.io/sitemap.xml  → 200 OK (1.1 KB)
[cache] id=3

<?xml version="1.0" encoding="UTF-8"?>
<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
  <url><loc>https://www.araknyd.io/</loc></url>
  <url><loc>https://www.araknyd.io/about</loc></url>
  <url><loc>https://www.araknyd.io/blog</loc></url>
  <url><loc>https://www.araknyd.io/careers</loc></url>
  <url><loc>https://www.araknyd.io/status</loc></url>
</urlset>

Extracted links: (none — XML leaf node)

★ Puzzle signal: /admin-beta appears in robots.txt but NOT in sitemap.xml.
  Intentional contradiction — rewards comparing sources.
```

---

### Stage 3 — `crawl https://www.araknyd.io/blog`

```
spider@journal:~$ crawl https://www.araknyd.io/blog
[GET] https://www.araknyd.io/blog  → 200 OK (22.8 KB)
[cache] id=4

Title: "Araknyd Engineering Blog"
Latest posts:
  • "Webhook Retries Without Tears" — Lily Chen, Aug 2024
  • "Why We Chose Duct Tape Over Kubernetes" — Anonymous, Jun 2024
  • "Incident Postmortem #47 (We Rolled Forward)" — CEO, May 2024

Extracted links (same-origin):
  /blog/webhook-retries-without-tears
  /blog/duct-tape-over-kubernetes
  /blog/incident-postmortem-47
  /blog/feed.xml

External (not fetched):
  https://github.com/araknyd-labs
```

---

### Stage 4 — `crawl https://www.araknyd.io/blog/webhook-retries-without-tears`

```
spider@journal:~$ crawl https://www.araknyd.io/blog/webhook-retries-without-tears
[GET] .../webhook-retries-without-tears  → 200 OK (8.4 KB)
[cache] id=5

Author: Lily Chen · Platform Team
Excerpt: "Most retry logic is copy-pasted StackOverflow. Here's ours..."

[article body truncated for terminal]

--- footer comment (visible in 'cat' full view) ---
<!--
  If you're reading source on old admin pages: /admin/v2 is production.
  /admin-beta is the staging shell we never took down. Don't click Export.
  — L.C.
-->

Extracted links (same-origin):
  /blog
  /blog/author/lily-chen

★ Journal hint (auto): Blog HTML comment references /admin-beta and /admin/v2.
```

---

### Stage 5 — `crawl https://www.araknyd.io/careers`

```
spider@journal:~$ crawl https://www.araknyd.io/careers
[GET] https://www.araknyd.io/careers  → 200 OK (11.6 KB)
[cache] id=6

Title: "Work at Araknyd — Move Fast, Document Never"
Open roles: Senior Backend Engineer, "DevOps (Just SSH)", Intern #12

Perks listed: unlimited PTO*, equity (probably), "exposure to production"

Extracted links (same-origin):
  /careers/backend-engineer
  /careers/devops-just-ssh
  /assets/handbook-2023.pdf

HTML comment (end of page):
  <!-- careers page rebuilt 2023; legacy admin-beta path kept for
       "bookmark compatibility" per CEO memo ARK-1337 -->

★ Reinforces /admin-beta as a named legacy path — still not a login bypass.
```

---

### Stage 6 — `crawl https://www.araknyd.io/admin-beta` (discovery payoff)

```
spider@journal:~$ crawl https://www.araknyd.io/admin-beta
[GET] https://www.araknyd.io/admin-beta  → 200 OK (4.9 KB)
[cache] id=7

Title: "Araknyd Admin — beta shell (internal)"
Warning banner: "⚠ STAGING — metrics are fake, Export button is not"

Extracted links (same-origin):
  /admin-beta/dashboard
  /admin-beta/login
  /assets/admin-v2.css

Response headers (display only — not interactive):
  X-Araknyd-Build: v2-staging-2019
  X-Robots-Tag: noindex

★ Objective flag: admin-beta dashboard discovered.
  Next gameplay beat: in-browser view-source on dashboard (see admin_dashboard_source.html).
  Terminal does NOT expose exportCustomerDatabase() — that lives in the HTML puzzle layer.
```

---

### Stage 6b — Optional deeper crawl: `/admin-beta/dashboard`

```
spider@journal:~$ crawl https://www.araknyd.io/admin-beta/dashboard
[GET] .../admin-beta/dashboard  → 302 Found → .../admin-beta/login?next=/dashboard
[cache] id=8 (login page body stored)

Title: "Araknyd SSO — Sign in with Google (@araknyd.io)"

Body snippet:
  "Internal tools require an @araknyd.io account.
   Contractors: ask Lily. Everyone else: good luck."

Extracted links: (none — form page, links not queued)

★ Narrative tie-in: aligns with Lily Venom dialogue (SSO gate).
  Player must progress via social puzzle / journal intel — not terminal brute force.
```

---

## `urls`

### After Stages 0–4 (mid-puzzle)

```
spider@journal:~$ urls
Discovered (not yet crawled):
  https://www.araknyd.io/about
  https://www.araknyd.io/careers
  https://www.araknyd.io/status
  https://www.araknyd.io/sitemap.xml
  https://www.araknyd.io/blog/webhook-retries-without-tears
  https://www.araknyd.io/blog/duct-tape-over-kubernetes
  https://www.araknyd.io/blog/incident-postmortem-47
  https://www.araknyd.io/blog/feed.xml
  https://www.araknyd.io/admin-beta          ← if robots.txt already crawled

Cached (crawled this session):
  [1] https://www.araknyd.io/
  [2] https://www.araknyd.io/robots.txt
  [3] https://www.araknyd.io/sitemap.xml
  [4] https://www.araknyd.io/blog

Tip: crawl https://www.araknyd.io/admin-beta
```

### `urls --cached` (after full chain)

```
spider@journal:~$ urls --cached
  [1] https://www.araknyd.io/
  [2] https://www.araknyd.io/robots.txt
  [3] https://www.araknyd.io/sitemap.xml
  [4] https://www.araknyd.io/blog
  [5] https://www.araknyd.io/blog/webhook-retries-without-tears
  [6] https://www.araknyd.io/careers
  [7] https://www.araknyd.io/admin-beta
  [8] https://www.araknyd.io/admin-beta/dashboard  (redirect target: login)
```

---

## `cat`

### `cat 2` (robots.txt)

```
spider@journal:~$ cat 2
User-agent: *
Allow: /
Disallow: /admin-beta/
Disallow: /tmp/
Disallow: /internal-export/

Sitemap: https://www.araknyd.io/sitemap.xml

# NOTE FROM DEV (2019): CEO said delete /admin-beta before launch.
# NOTE FROM DEV (2026): still here. not my problem anymore.
```

### `cat 5` (Lily blog post — comment block)

```
spider@journal:~$ cat 5
[... article text ...]

<!--
  If you're reading source on old admin pages: /admin/v2 is production.
  /admin-beta is the staging shell we never took down. Don't click Export.
  — L.C.
-->

Author: Lily Chen · Aug 14, 2024
```

### `cat https://www.araknyd.io/admin-beta`

```
spider@journal:~$ cat https://www.araknyd.io/admin-beta
<!DOCTYPE html>
<html lang="en">
<head>
  <title>Araknyd Admin — beta shell (internal)</title>
  ...
</head>
<body>
  <div class="banner">⚠ STAGING — NOT FOR MERIDIAN DEMO DAY</div>
  <p>Redirecting operators to <a href="/admin-beta/dashboard">dashboard</a>...</p>
  <!-- bundled for "speed" — see /admin/v2/dashboard.js in prod -->
</body>
</html>

[truncated — open in in-game browser for full view-source puzzle]
```

### Error — not cached

```
spider@journal:~$ cat https://www.araknyd.io/about
Error: URL not in cache. Run 'crawl https://www.araknyd.io/about' first.
```

---

## `history`

### Mid-session example

```
spider@journal:~$ history
  1  help
  2  crawl https://www.araknyd.io
  3  urls
  4  crawl https://www.araknyd.io/robots.txt
  5  cat 2
  6  crawl https://www.araknyd.io/sitemap.xml
  7  crawl https://www.araknyd.io/blog
  8  crawl https://www.araknyd.io/blog/webhook-retries-without-tears
  9  cat 5
 10  urls --discovered
```

### After puzzle completion

```
spider@journal:~$ history
  1  help
  2  crawl https://www.araknyd.io
  3  crawl https://www.araknyd.io/robots.txt
  4  crawl https://www.araknyd.io/blog
  5  crawl https://www.araknyd.io/blog/webhook-retries-without-tears
  6  crawl https://www.araknyd.io/careers
  7  crawl https://www.araknyd.io/admin-beta
  8  cat https://www.araknyd.io/admin-beta
  9  urls --cached
```

---

## Error & Edge-Case Outputs

### Unknown host

```
spider@journal:~$ crawl https://example.com
Error: host not on allowlist. This tool only crawls puzzle-scenario domains.
```

### Malformed URL

```
spider@journal:~$ crawl admin-beta
Error: expected full URL (https://...). Try: crawl https://www.araknyd.io/admin-beta
```

### Already cached (idempotent)

```
spider@journal:~$ crawl https://www.araknyd.io/robots.txt
[cache hit] id=2 — no new links queued.
Use 'cat 2' to review content.
```

### Unknown command

```
spider@journal:~$ scan https://www.araknyd.io/admin-beta
Unknown command: scan
Type 'help' for available commands.
```

---

## Integration Notes (for implementers)

| Game beat | Terminal role | Handoff |
|-----------|----------------|---------|
| Araknyd arc opens | Player runs first `crawl` on homepage | Queues blog + robots.txt |
| OSINT / journal phase | `robots.txt` + sitemap mismatch | Journal auto-entry |
| Lily high-trust Venom | Mentions `/admin/v2/dashboard.js` | Confirms prod vs beta paths |
| `/admin-beta` reached | `crawl` returns staging shell | Switch to in-browser view-source (`admin_dashboard_source.html`) |
| Post-discovery | Terminal has nothing left to brute | SSO login is a separate puzzle layer |

**Journal auto-entries (suggested):**
- `"robots.txt disallows /admin-beta/ — path not on homepage."`
- `"Lily's blog comment: /admin-beta is legacy staging admin."`
- `"Located /admin-beta dashboard. Inspect source in browser for exportCustomerDatabase()."`

**Flags (suggested):**
- `araknyd_robots_crawled`
- `araknyd_admin_beta_discovered`
- `journal_urls_araknyd_updated` → writes `usr/araknyd/urls.txt` with `https://www.araknyd.io/admin-beta`

---

## Content Safety Checklist

Before shipping any new terminal output, verify:

- [ ] No shell commands, SQL, XSS, or RCE strings presented as player actions  
- [ ] No credential formats presented as "try this password"  
- [ ] No port scans, proxy chains, or tunnel setup instructions  
- [ ] HTTP status codes used as story beats only (403/302 ≠ "hack this")  
- [ ] "Misconfigurations" are **named paths in fiction**, not recipes for real sites  
- [ ] Cross-references to exploit steps deferred to **non-terminal** puzzle layers (dialogue, view-source, journal)

---

*Document version: 1.0 · Target narrative: Araknyd · Maintainer: puzzle design*
