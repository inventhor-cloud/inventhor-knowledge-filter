namespace Inventhor.KnowledgeFilter;

// Metadata only; caller must verify cited pages, document contents, origin
// ownership and publication dates. Nothing in this layer fetches the web.
public enum ResearchSourceKind
{
    Official, IndependentJournalism, Academic, PrimaryDocument, Community, Other
}
public enum ResearchStance { Supports, Disputes, ContextOnly }
public enum ResearchEvidenceForm
{
    PrimaryRecord, OriginalReporting, AttributedStatement, SecondarySummary, Unknown
}

public sealed record ResearchSource(
    string ClaimId, string SourceId, string Url, string Publisher,
    string OriginGroup, ResearchSourceKind Kind, ResearchStance Stance,
    ResearchEvidenceForm EvidenceForm, string Country, string Language,
    DateTimeOffset? PublishedAt);

public sealed record ResearchCitation(
    string SourceId, string Url, string Publisher, string OriginGroup,
    ResearchSourceKind Kind, ResearchStance Stance,
    ResearchEvidenceForm EvidenceForm, string Country, string Language,
    DateTimeOffset? PublishedAt);

public sealed record ResearchCoverage(
    string ClaimId, string Coverage,
    int CitedSources, int OriginalOriginGroups,
    int NonOfficialOriginGroups, int SourceKinds,
    int Countries, int Languages,
    bool HasOpposingClaims, IReadOnlyList<string> Warnings,
    IReadOnlyList<ResearchCitation> Citations)
{
    // Source variety never proves truth or authorizes a restriction.
    public bool IsTruthVerdict => false;
}

public static class ResearchEvidenceReview
{
    const int MaxSources = 80;
    const int MaxUrl = 2048;

    public static ResearchCoverage Review(string claimId,
        IReadOnlyCollection<ResearchSource>? submitted)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(claimId);
        var warnings = new HashSet<string>(StringComparer.Ordinal);
        var records = new List<ResearchSource>();
        var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var urls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (submitted is null || submitted.Count == 0)
            warnings.Add("NO_SOURCES");
        else if (submitted.Count > MaxSources)
            warnings.Add("INPUT_LIMIT_EXCEEDED");
        else
            foreach (var entry in submitted)
            {
                if (entry is null || !string.Equals(entry.ClaimId, claimId,
                    StringComparison.Ordinal))
                {
                    warnings.Add("OTHER_CLAIM_OR_NULL_RECORD_IGNORED");
                    continue;
                }
                if (string.IsNullOrWhiteSpace(entry.SourceId)
                    || entry.SourceId.Length > 80
                    || !entry.SourceId.All(x => char.IsLetterOrDigit(x)
                        || x is '_' or '-' or ':' or '.'))
                {
                    warnings.Add("INVALID_SOURCE_ID_IGNORED");
                    continue;
                }
                if (!IsReferenceUrl(entry.Url))
                {
                    warnings.Add("INVALID_CITATION_URL_IGNORED");
                    continue;
                }
                if (!ids.Add(entry.SourceId))
                {
                    warnings.Add("DUPLICATE_SOURCE_ID_IGNORED");
                    continue;
                }
                if (!urls.Add(entry.Url))
                {
                    warnings.Add("DUPLICATE_CITATION_URL_IGNORED");
                    continue;
                }
                if (string.IsNullOrWhiteSpace(entry.OriginGroup))
                    warnings.Add("UNKNOWN_ORIGINAL_PROVENANCE");
                records.Add(entry);
            }

        // Only known original reporting families can count as independent.
        // Publisher, host, syndication, or displayed byline alone are not proof.
        var attributed = records.Where(s =>
            !string.IsNullOrWhiteSpace(s.OriginGroup)).ToArray();
        var origins = attributed.Select(s => s.OriginGroup.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase).Count();
        var nonOfficialOrigins = attributed
            .Where(s => s.Kind != ResearchSourceKind.Official)
            .Select(s => s.OriginGroup.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase).Count();
        var kinds = records.Select(s => s.Kind).Distinct().Count();
        var countries = records.Select(s => s.Country?.Trim())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct(StringComparer.OrdinalIgnoreCase).Count();
        var languages = records.Select(s => s.Language?.Trim())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct(StringComparer.OrdinalIgnoreCase).Count();
        var opposing = records.Any(s => s.Stance == ResearchStance.Supports)
            && records.Any(s => s.Stance == ResearchStance.Disputes);

        if (records.Count > origins && attributed.Length > origins)
            warnings.Add("SYNDICATED_OR_SHARED_ORIGINS_NOT_INDEPENDENT");
        if (records.Any(s => s.Kind == ResearchSourceKind.Official))
            warnings.Add("OFFICIAL_STATEMENTS_REQUIRE_INDEPENDENT_EVALUATION");
        if (records.Any(s => s.Kind == ResearchSourceKind.Official)
            && nonOfficialOrigins == 0)
            warnings.Add("OFFICIAL_ONLY_OR_UNATTRIBUTED_COVERAGE");
        if (opposing)
            warnings.Add("MATERIAL_SOURCE_DISAGREEMENT_PRESERVE_BOTH");
        if (countries == 1 && records.Count > 1)
            warnings.Add("SINGLE_COUNTRY_COVERAGE");
        if (languages == 1 && records.Count > 1)
            warnings.Add("SINGLE_LANGUAGE_COVERAGE");

        // No truth verdict or source veto follows from these counts.
        var diverse = origins >= 3 && nonOfficialOrigins >= 2 && kinds >= 2;
        if (!diverse)
            warnings.Add("INDEPENDENT_COVERAGE_LIMITED");

        return new ResearchCoverage(claimId,
            diverse ? "DIVERSE_INPUTS_NOT_TRUTH_VERIFIED"
                : "LIMITED_INPUTS_NOT_TRUTH_VERIFIED",
            records.Count, origins, nonOfficialOrigins, kinds,
            countries, languages, opposing,
            warnings.OrderBy(x => x, StringComparer.Ordinal).ToArray(),
            records.Select(s => new ResearchCitation(s.SourceId, s.Url,
                s.Publisher, s.OriginGroup, s.Kind, s.Stance, s.EvidenceForm,
                s.Country, s.Language, s.PublishedAt)).ToArray());
    }

    static bool IsReferenceUrl(string? raw) =>
        !string.IsNullOrWhiteSpace(raw) && raw.Length <= MaxUrl
        && Uri.TryCreate(raw, UriKind.Absolute, out var uri)
        && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
        && string.IsNullOrEmpty(uri.UserInfo) && !string.IsNullOrWhiteSpace(uri.Host);
}
