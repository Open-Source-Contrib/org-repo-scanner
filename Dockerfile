# Set the base image as the .NET 5.0 SDK (this includes the runtime)
FROM mcr.microsoft.com/dotnet/sdk:5.0 as build-env

# Copy everything and publish the release (publish implicitly restores and builds)
COPY . ./
RUN dotnet publish ./src/OrgRepoScanner.Runner/OrgRepoScanner.Runner.csproj -c Release -o out --no-self-contained

# Label the container
LABEL maintainer="Pradeep Singh <pradeep.singh@talabat.com>"
LABEL repository="https://github.com/Open-Source-Contrib/org-repo-scanner"
LABEL homepage="https://github.com/Open-Source-Contrib/org-repo-scanner"

# Label as GitHub action
LABEL com.github.actions.name="Org Repo Scanner"
# Limit to 160 characters
LABEL com.github.actions.description="Scans the organization repositories & then fetches the code metrics from Sonarcloud and generates a nice report."
# See branding:
# https://docs.github.com/actions/creating-actions/metadata-syntax-for-github-actions#branding
LABEL com.github.actions.icon="activity"
LABEL com.github.actions.color="orange"

# Relayer the .NET SDK, anew with the build output
FROM mcr.microsoft.com/dotnet/runtime:5.0
COPY --from=build-env /out .
ENTRYPOINT [ "dotnet", "/OrgRepoScanner.Runner.dll" ]