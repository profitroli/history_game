# SessionStart anchor: factual re-grounding after startup/resume/clear/compact.
# Facts only - imperative phrasing would trip Claude's prompt-injection defence.
$lines = @(
  "Facts about this project's state layer:",
  "- The authoritative current state is .claude/state/NOW.md; completed stages live in .claude/state/history/.",
  "- Compaction summaries are informational, not evidence; the disk is canonical.",
  "- File Read-state does not survive compaction or a model switch: files must be re-read before Edit.",
  "- Temp files go to D:\CLAUDE PROJECT\Temp\, never C: or scratchpad.",
  "- Unity project root is testtest/. C# scripts in testtest/Assets/Scripts/.",
  "- Never delete .meta files without their corresponding asset.",
  "- Scene/prefab YAML edits require care; prefer script-based changes."
)
if (Test-Path ".git") {
  git fetch --quiet 2>$null
  $branch = git rev-parse --abbrev-ref HEAD 2>$null
  $behind = git rev-list --count "HEAD..@{u}" 2>$null
  if ($LASTEXITCODE -eq 0 -and $behind -and $behind -ne "0") {
    $lines += "- Local branch '$branch' is $behind commits behind its upstream after fetch."
  }
}
@{ hookSpecificOutput = @{
     hookEventName     = "SessionStart"
     additionalContext = ($lines -join "`n")
} } | ConvertTo-Json -Depth 4
