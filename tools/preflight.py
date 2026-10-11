import json, glob, os
base = os.path.join(os.path.dirname(os.path.dirname(os.path.abspath(__file__))), "sheets")
issues = []
sheets = {}
for p in sorted(glob.glob(os.path.join(base, "*.json"))):
    name = os.path.basename(p)[:-5]
    sheets[name] = json.load(open(p))["rows"]
for name, rows in sheets.items():
    for r in rows:
        for k, v in r.items():
            if v in ("", None):
                issues.append(f"{name}:{r.get('id')}.{k} is unfilled")
        if r.get("verified") is not True:
            issues.append(f"{name}:{r.get('id')} unverified")
track_ids = {r["id"] for r in sheets["tracks"]}
for r in sheets["rules"]:
    if r["track"] not in track_ids:
        issues.append(f"rules:{r['id']} references missing track {r['track']}")
print("\n".join(issues) or "clean")
print(len(issues), "issue(s)")
