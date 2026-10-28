namespace Spike.Data;

public record PublicationDto(
    string Title,
    string Summary,
    VersionDto LatestVersion
);