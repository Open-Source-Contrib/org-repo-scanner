namespace OrgRepoScanner.Core.Sonarcloud
{
    public class SonarcloudOptions
    {
        public string Username { get; set; }
        public string OrganizationKey { get; set; }
        public string ProjectKeyFormat { get; set; }
        public string ApiBaseAddress { get; set; }
    }
}