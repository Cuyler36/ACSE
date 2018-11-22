using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using ACSE.Core.Utilities;
using Newtonsoft.Json.Linq;

namespace ACSE.Core.Updater
{
    public sealed class Updater
    {
        private int _versionMajor;
        private int _versionMinor;
        private int _versionRevision;

        public string UpdateUrl { get; private set; }

        private void SetCurrentVersionInfo()
        {
            var versionInfo = FileVersionInfo.GetVersionInfo(PathUtility.GetExeLocation());
            _versionMajor = versionInfo.FileMajorPart;
            _versionMinor = versionInfo.FileMinorPart;
            _versionRevision = versionInfo.ProductBuildPart;
        }

        public Updater()
        {
            SetCurrentVersionInfo();
        }

        /// <summary>
        /// Gets the current version string.
        /// </summary>
        /// <returns>The current version string.</returns>
        public string GetVersion() => $"{_versionMajor}.{_versionMinor}.{_versionRevision}";

        private (bool, string) CheckForUpdate(string url)
        {
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultCredentials;
            request.ContentType = "application/json";
            request.Method = "GET";
            request.Accept = "application/json";
            request.UserAgent = "ACSE";

            // Read response.
            string content;
            try
            {
                var response = (HttpWebResponse) request.GetResponse();
                using (var contentStream = response.GetResponseStream())
                {
                    if (contentStream == null) return (false, null);
                    using (var reader = new StreamReader(contentStream))
                    {
                        content = reader.ReadToEnd();
                    }
                }
            }
            catch
            {
                return (false, null);
            }

            // Parse JSON.
            JObject jObj;
            switch (JToken.Parse(content))
            {
                case JArray jArray:
                    jObj = (JObject) jArray[0];
                    break;
                case JObject jObject:
                    jObj = jObject;
                    break;
                default:
                    return (false, null);
            }

            var version = (string) jObj["tag_name"];
            var updateUrl = (string) jObj["html_url"];

            try
            {
                var versionInfo = version.Split('.');
                var updateMajor = int.Parse(versionInfo[0]);
                var updateMinor = int.Parse(versionInfo[1]);
                var updateRevision = int.Parse(versionInfo[2]);

                return (updateMajor > _versionMajor || updateMinor > _versionMinor || updateRevision > _versionRevision, updateUrl);
            }
            catch
            {
                return (false, null);
            }
        }

        public bool HasUpdate()
        {
            // Check the "latest" release first.
            var (latestResult, latestUrl) = CheckForUpdate(@"https://api.github.com/repos/cuyler36/ACSE/releases/latest");
            UpdateUrl = latestUrl;
            if (latestResult) return true;

            // Now check all releases to see if the most recent version is newer.
            var (releasesResult, releaseUrl) = CheckForUpdate(@"https://api.github.com/repos/cuyler36/ACSE/releases");
            UpdateUrl = releaseUrl;
            return releasesResult;
        }
    }
}
