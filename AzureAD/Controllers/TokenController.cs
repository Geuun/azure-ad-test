using AzureAD.Utils.XmlReader;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace AzureAD.Controllers
{
    [RoutePrefix("api/v1")]
    public class TokenController : ApiController
    {
        [Route("chart")]
        [HttpGet]
        public async Task<JObject> GetOrgChart()
        {
            string token;
            // accessToken 발급
            using (var client = new HttpClient())
            {
                string clientSecret = XmlReader.GetAppSettingConfig("client_secret");

                var tokenUrl = "https://login.microsoftonline.com/0095b6b4-3292-4100-9deb-08df80bf25a6/oauth2/v2.0/token";

                var parameters = new Dictionary<string, string>
                {
                    {"client_id", "01f04ac7-1700-4e13-a8bc-df1d9a36f6df" },
                    {"client_secret", clientSecret },
                    {"scope", "https://graph.microsoft.com/.default" },
                    {"grant_type", "client_credentials" }
                };

                var content = new FormUrlEncodedContent(parameters);

                using (var res = await client.PostAsync(tokenUrl, content))
                {
                    res.EnsureSuccessStatusCode();

                    var result = await res.Content.ReadAsStringAsync();

                    JObject jsonData = JObject.Parse(result);

                    token = jsonData["access_token"].ToString();
                }
            }

            // OrgChart 요청
            using (var client = new HttpClient())
            {
                var url = "https://graph.microsoft.com/v1.0/directoryObjects/delta?$filter=isof('microsoft.graph.user') or isof('microsoft.graph.group')";

                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                using (var res = await client.GetAsync(url))
                {
                    res.EnsureSuccessStatusCode();

                    var result = await res.Content.ReadAsStringAsync();

                    JObject jsonData = JObject.Parse(result);

                    return jsonData;
                }
            }
        }
    }
}
