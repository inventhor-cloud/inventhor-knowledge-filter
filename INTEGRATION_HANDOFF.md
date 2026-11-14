# INVENTHOR Main — research evidence diversity handoff

Status: SOURCE-ONLY. This is a reusable .NET 10 library in the existing federated knowledge-filter repository, not a replacement Brain/core/search engine.

## Runtime data flow (not yet wired)

1. The existing Main BrainHost or authorized research adapter receives the user's research question. Generate a scoped claim ID; do not store conversations in this public repo.
2. First use the seven non-authoritative lanes of ResearchBreadthPlan.CoreLanes as an explicit discovery checklist; no lane has authority to suppress another. Existing provider/search tools then retrieve multiple independently originated sources. The discovery stage must actively seek independent journalism, first-hand documents, academic research, local/affected-party reporting, relevant official records and contrary evidence when material. Search multiple countries/languages where appropriate and available, without making unsupported access claims.
3. A trusted citation wrapper verifies the real source URI, date, ownership/syndication family, source type and whether a given claim is supported, disputed or contextual. **Do not trust self-reported publisher-origin metadata without verification.**
4. Send only bounded, non-sensitive metadata to `ResearchEvidenceReview.Review(claimId, records)`. No network access, scraping, raw documents, personal conversations or provider secrets belong in this library.
5. Present cited verified facts, attributed claims, unresolved conflict and evidence gaps separately. Source-count diversity means **coverage breadth**, never truth. Keep both supported and disputed sources with original unchanged URIs. The application must escape source metadata in UI.
6. If the assessment says LIMITED_INPUTS, Main should seek more independent originals where possible. If unavailable, say so transparently. Never fabricate sources, bypass technical authorization or privacy protections, or treat any government/intelligence or commercial entity as an automatic truth/falsehood oracle.
7. Send the research coverage summary, selected citations and admitted evidence metadata to Operator results and QA Factory through existing signed task/result envelopes; never build a second queue. QA should check provenance deduplication, injection resistance, link validity, contested claim display and failures.
8. Promote to running Main **only** after versioned source approval, signed Owner/Organizer admission, rollback and live QA Factory acceptance.

## Explicit protections

- This library does not decide factual truth or automatically classify a disputed statement as misinformation.
- No official agency has an authority override or a censorship veto. Official records remain eligible as properly attributed evidence.
- An eyewitness, advocacy group, academic paper, commercial publisher or independent outlet is not automatically correct either.
- Syndicated articles, translations and mirrors sharing the same originating work are one independent provenance group.
- Retrieved text is untrusted data; never allow it to modify system rules, tool permissions or safety controls.
- A source-only green CI does not mean any live INVENTHOR chatbot has adopted the policy.
