# INVENTHOR Software Brain Foundation v1

## Purpose

This repository is the privacy-safe, platform-neutral knowledge and decision
filter for every INVENTHOR runtime. It is shared by the main INVENTHOR system,
Operator Core, cloud workers, and future modules through versioned contracts.
It is not an Android UI module and must not contain application-specific
business logic.

## Hybrid architecture

The brain is composed of replaceable layers:

1. **Source adapters** — GitHub, local files, Termux, Operator results, user
   input, and future AI providers. Adapters only collect and normalize data.
2. **Evidence and privacy filter** — detects secrets and personal data,
   validates provenance, scores evidence, and rejects unsafe payloads.
3. **Knowledge index** — stores canonical records by stable identity and
   content fingerprint; supports exact duplicate and similarity lookup.
4. **Conflict and merge policy** — proposes merges, preserves history, and
   requires approval for contradictory or destructive changes.
5. **Project memory / execution ledger** — records state, decisions, actions,
   files, results, handoff context, and the next safe step per project.
6. **Decision planner** — produces a bounded, explainable plan. It never
   executes an action without a registered executor and policy approval.
7. **Runtime adapters** — deliver approved plans to INVENTHOR, Operator, or a
   platform-specific executor. No adapter may silently change target scope.
8. **AI connectors** — GPT, Gemini, Google AI Studio/Mode, and future providers
   consume the same filtered context and return structured proposals.

Every layer has an explicit contract, version, owner, input/output schema, and
failure result. A local cache is used for continuity; cloud storage is the
canonical shared record when authenticated and reachable. Offline work queues
outbox events and synchronizes later without duplicating them.

## Canonical knowledge record

Each accepted record must contain:

- `record_id`: immutable UUID.
- `project_id`: isolated project namespace.
- `kind`: architecture, code, design, social, language, automation, policy,
  task, result, or decision.
- `title` and normalized `content`.
- `content_fingerprint`: deterministic hash of normalized content.
- `source`, `source_ref`, `observed_at`, and `recorded_at`.
- `confidence`, `relevance`, and `sensitivity`.
- `status`: proposed, accepted, superseded, rejected, or archived.
- `supersedes` / `related_records` where applicable.
- `schema_version`.

The record must never contain raw access tokens, passwords, private keys,
keystore material, cookies, or unredacted personal secrets. A rejected or
redacted record produces a reason code, not the sensitive value.

## Deduplication and merge rules

1. Exact fingerprint match: do not create a second record; append provenance.
2. Similarity match: create a merge proposal with both record IDs and a
   comparison summary.
3. Compatible improvement: keep the prior record, create a new version, and
   link `supersedes`.
4. Conflict or permission change: keep both versions and require explicit
   approval before activation.
5. Never delete history automatically. Archive and rollback metadata are
   mandatory.
6. A record is injectable into an AI context only when its project scope,
   status, sensitivity, relevance, and compatibility policy allow it.

## Decision and execution boundary

The brain may analyze, classify, summarize, compare, and prepare a bounded plan.
Execution is separate:

- `ANALYSIS_ONLY`, `READ_ONLY`, and `REPORT_ONLY` forbid mutation.
- File writes, builds, installs, pushes, releases, deletion, and external
  messages require a registered executor and the applicable approval gate.
- Unsupported work returns `BF_UNSUPPORTED_TASK`; it must never emit a fake
  success.
- Every task uses one stable `task_id` from acceptance through final
  `BS`/ `BF`.
- A result is final only after delivery is acknowledged by the originating
  runtime.

## Ledger contract

Each project has an isolated ledger with:

- `CURRENT_STATE`
- `ACTIVE_TASK`
- `HANDOFF`
- `EVENT_LOG`
- `DECISION_LOG`
- `DONE`
- `TODO`
- `NEXT_SAFE_STEP`

Ledger events are append-only and idempotent by `event_id`. A continuation
reader must be able to reconstruct what was done, what failed, why a decision
was made, which files changed, and the next bounded task without relying on
chat history.

## Portability and module isolation

The same contracts are usable on Android, Windows, Linux, macOS, and iOS.
Platform code belongs only in adapters. Brain domains remain separate and can
be enabled independently:

- architecture/software
- visual/design
- social/accounts
- language
- automation
- memory/ledger
- security/policy

The main INVENTHOR app and Operator Core remain separate products. They share
contracts and adapters, not source files or UI implementations.

## Initial acceptance criteria

The foundation is ready for the next implementation slices when:

- the contract is versioned and reviewed;
- secret filtering and project scoping are mandatory;
- duplicate, merge, conflict, approval, archive, and rollback states are
  representable;
- task identity and result delivery are end-to-end stable;
- a continuation reader can reconstruct project state;
- no main INVENTHOR APK or `MainActivity` source is modified.
