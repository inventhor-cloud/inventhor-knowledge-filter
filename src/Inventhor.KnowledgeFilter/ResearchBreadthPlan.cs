namespace Inventhor.KnowledgeFilter;

// An input-discovery checklist, not an autonomous crawler or a truth ranking.
// The caller decides which sources are actually available and must report gaps.
public sealed record ResearchLane(
    string Id, string Objective, string IndependentOriginRule,
    bool IsTrustedByDefault, bool MaySuppressContraryEvidence);

public static class ResearchBreadthPlan
{
    public static IReadOnlyList<ResearchLane> CoreLanes() =>
    [
        new("independent-original-reporting",
            "Seek independently produced reporting, interviews and investigations.",
            "Attribute the underlying original report, not each republisher.",
            false, false),
        new("primary-documents",
            "Find original records, full transcripts, datasets and dated documents.",
            "Verify document authenticity and ownership; preserve context.",
            false, false),
        new("academic-technical",
            "Seek relevant peer-reviewed work, methods and independent analyses.",
            "Do not count multiple articles from the same research data as independent experiments.",
            false, false),
        new("local-affected-parties",
            "Seek directly affected local groups and attributed first-hand testimony.",
            "Separate firsthand observation from uncorroborated inference.",
            false, false),
        new("contrary-evidence",
            "Actively look for material counterevidence and conflicting interpretations.",
            "Preserve disagreements and describe the evidentiary gap.",
            false, false),
        new("official-attributed-records",
            "Examine relevant government, regulatory, court or agency records as attributed claims.",
            "Official or intelligence origin does not grant a truth veto or automatic corroboration.",
            false, false),
        new("international-language-check",
            "Compare independently originated regional and language perspectives where relevant.",
            "Translations and syndicated mirrors retain one original origin family.",
            false, false)
    ];
}
