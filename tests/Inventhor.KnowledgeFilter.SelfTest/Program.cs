using Inventhor.KnowledgeFilter;

int passed = 0;
void Check(bool test, string name)
{
    if (!test) throw new InvalidOperationException("FAIL: " + name);
    Console.WriteLine("PASS: " + name);
    passed++;
}
ResearchSource S(string id, string url, string group,
    ResearchSourceKind kind, ResearchStance stance = ResearchStance.Supports,
    string country = "DE", string language = "de", string claim = "C") =>
    new(claim, id, url, "Synthetic publication " + id,
        group, kind, stance, ResearchEvidenceForm.AttributedStatement,
        country, language, null);

var official = S("o", "https://gov.example.invalid/record", "agency",
    ResearchSourceKind.Official, ResearchStance.Disputes);
var independent = S("i", "https://news.example.invalid/report", "origin-1",
    ResearchSourceKind.IndependentJournalism, country: "TR", language: "tr");
var academic = S("a", "https://university.example.invalid/paper", "origin-2",
    ResearchSourceKind.Academic, country: "US", language: "en");

var broad = ResearchEvidenceReview.Review("C", [official, independent, academic]);
Check(broad.CitedSources == 3 && broad.OriginalOriginGroups == 3,
    "3 independent origin families preserved");
Check(broad.NonOfficialOriginGroups == 2 && broad.SourceKinds == 3,
    "Official receives no automatic counting advantage or veto");
Check(broad.Coverage == "DIVERSE_INPUTS_NOT_TRUTH_VERIFIED"
    && !broad.IsTruthVerdict, "Coverage is not truth certification");
Check(broad.HasOpposingClaims
    && broad.Warnings.Contains("MATERIAL_SOURCE_DISAGREEMENT_PRESERVE_BOTH"),
    "Conflicting official and independent statements both retained");
Check(broad.Citations.Any(x => x.SourceId == "o")
    && broad.Citations.Any(x => x.SourceId == "i"),
    "Official evidence not hidden and contrary reporting not censored");
Check(broad.Countries == 3 && broad.Languages == 3,
    "Multilingual and multi-region coverage measured");

var authorityOnly = ResearchEvidenceReview.Review("C", [official]);
Check(authorityOnly.Coverage.StartsWith("LIMITED_INPUTS", StringComparison.Ordinal)
    && authorityOnly.Warnings.Contains("OFFICIAL_ONLY_OR_UNATTRIBUTED_COVERAGE"),
    "State agency alone cannot certify information");
Check(authorityOnly.Citations.Count == 1,
    "Official source not excluded merely because it is official");

var mirror = ResearchEvidenceReview.Review("C",
[
    S("m1", "https://a.example.invalid/1", "shared-wire",
        ResearchSourceKind.IndependentJournalism),
    S("m2", "https://b.example.invalid/2", "shared-wire",
        ResearchSourceKind.IndependentJournalism),
    S("m3", "https://c.example.invalid/3", "shared-wire",
        ResearchSourceKind.IndependentJournalism)
]);
Check(mirror.OriginalOriginGroups == 1
    && mirror.Warnings.Contains("SYNDICATED_OR_SHARED_ORIGINS_NOT_INDEPENDENT"),
    "Wire-service copies are not independent confirmations");
Check(!mirror.IsTruthVerdict && mirror.CitedSources == 3,
    "All original references remain visible with no invented truth");

var missing = ResearchEvidenceReview.Review("C",
[
    S("n1", "https://a.example.invalid/x", "",
        ResearchSourceKind.IndependentJournalism),
    S("n2", "https://b.example.invalid/y", "known-family",
        ResearchSourceKind.IndependentJournalism)
]);
Check(missing.OriginalOriginGroups == 1
    && missing.Warnings.Contains("UNKNOWN_ORIGINAL_PROVENANCE"),
    "Unknown original reporting origin never counted independently");

var invalid = ResearchEvidenceReview.Review("C",
[
    S("bad-url", "javascript:alert(1)", "f1",
        ResearchSourceKind.Community),
    S("wrong-claim", "https://x.example.invalid/", "f2",
        ResearchSourceKind.Academic, claim: "DIFFERENT"),
    S("trusted", "https://ok.example.invalid/", "f3",
        ResearchSourceKind.PrimaryDocument),
    S("trusted", "https://different.example.invalid/", "f4",
        ResearchSourceKind.PrimaryDocument),
    S("mirror-id", "https://ok.example.invalid/", "f5",
        ResearchSourceKind.Official)
]);
Check(invalid.CitedSources == 1 && invalid.Citations[0].SourceId == "trusted",
    "Invalid protocols, cross-claim and duplicate citations excluded");
Check(invalid.Warnings.Contains("INVALID_CITATION_URL_IGNORED")
    && invalid.Warnings.Contains("OTHER_CLAIM_OR_NULL_RECORD_IGNORED")
    && invalid.Warnings.Contains("DUPLICATE_SOURCE_ID_IGNORED")
    && invalid.Warnings.Contains("DUPLICATE_CITATION_URL_IGNORED"),
    "Rejected metadata is transparently reported");

var empty = ResearchEvidenceReview.Review("C", []);
Check(empty.CitedSources == 0
    && empty.Warnings.Contains("NO_SOURCES"),
    "No-source answer never claims research was conducted");

var tooMany = Enumerable.Range(0, 81).Select(i =>
    S("s" + i, "https://x.example.invalid/" + i, "g" + i,
        ResearchSourceKind.Academic)).ToArray();
var capped = ResearchEvidenceReview.Review("C", tooMany);
Check(capped.CitedSources == 0
    && capped.Warnings.Contains("INPUT_LIMIT_EXCEEDED"),
    "Oversized batch fails closed without partial evidence claims");

Console.WriteLine($"EVIDENCE_DIVERSITY_SELFTEST_PASS={passed}");
