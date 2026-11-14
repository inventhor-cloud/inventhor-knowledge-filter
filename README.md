# inventhor-knowledge-filter

INVENTHOR's privacy-safe knowledge filter, anonymization gate, evidence scoring and brain-injection control layer.

## Source-only research evidence diversity (new)

`src/Inventhor.KnowledgeFilter/` contains a pure .NET 10, **read-only metadata evaluator**. It does not score truth by source authority, fetch pages, call models, alter prompts, change permissions, or send any data to third parties.

The evaluator distinguishes source count from *original independent reporting groups*, preserves opposing attributed statements, reports insufficient coverage, and treats state/official claims as evidence rather than final authority. The separate ResearchBreadthPlan specifies seven independent discovery lanes (including counterevidence, local voices and attributed official records), without making network calls or granting any source a veto. An unknown origin never counts as an independent source. Country/language diversity is descriptive rather than proof.

Run deterministic self-tests:

```powershell
dotnet run --project tests/Inventhor.KnowledgeFilter.SelfTest/Inventhor.KnowledgeFilter.SelfTest.csproj -c Release
```

Policy: `policy/research_evidence_diversity.v1.json`

Main/BrainHost integration prerequisites, fail-closed and security boundaries: `INTEGRATION_HANDOFF.md`.

**Not deployed:** No runtime adapter, source crawler, authenticated Brain test, search backend, Owner-approved install, signed QA factory acceptance or automatic moderation-rule change is claimed. User-private data and keys must never enter this repository.
