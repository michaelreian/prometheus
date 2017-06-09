using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Prometheus.Core.Usenet;
using Group = Prometheus.Core.Usenet.Group;

namespace Prometheus.Core.Usenet
{
    public class UsenetClient : IUsenetClient
    {
        private readonly UsenetSettings usenetSettings;
        private readonly INntpConnection connection;
        private NntpClient client;
        private bool isInitialized = false;
        private List<string> headers;

        public UsenetClient(UsenetSettings usenetSettings)
        {
            this.usenetSettings = usenetSettings;
            this.connection = new NntpConnection();
        }

        private async Task Initialize()
        {
            if (!this.isInitialized)
            {
                this.client = new NntpClient(this.connection);
                if (await client.Connect(this.usenetSettings.Host, this.usenetSettings.Port, this.usenetSettings.UseSsl))
                {
                    if (this.client.Authenticate(this.usenetSettings.Username, this.usenetSettings.Password))
                    {
                        this.client.SetReaderMode();
                        this.isInitialized = true;
                    }
                }
            }
        }


        public async Task<Group> GetGroup(string groupName)
        {
            await this.Initialize();
            return client.SelectGroup(groupName);
        }

        public async Task<List<Overview>> GetOverviews(string groupName, long first, long last)
        {
            await this.Initialize();
            var headers = await this.GetHeaders();
            var group = client.SelectGroup(groupName);
            var response = connection.XOVER(Range.From(first).To(last));
            var dictionaries = response.Lines.Select(x => ParseOverviewLine(headers, x));
            var overviews = dictionaries.Select(CreateOverview);
            return overviews.ToList();
        }

        private Overview CreateOverview(Dictionary<string, string> dictionary)
        {
            return new Overview
            {
                ArticleNo = long.Parse(dictionary["ArticleNo"]),
                Subject = dictionary["Subject"],
                From = dictionary["From"],
                Date = DateTime.Parse(dictionary["Date"]),
                References = dictionary["References"],
                MessageID = dictionary["Message-ID"].Trim('<', '>'),
                Bytes = int.Parse(dictionary["Bytes"]),
                Lines = int.Parse(dictionary["Lines"]),
                XRef = dictionary["Xref:full"],

            };
        }

        private Dictionary<string, string> ParseOverviewLine(List<string> headers, string overview)
        {
            var items = overview.Split('\t');

            if (items.Length != headers.Count)
            {
                throw new Exception();
            }

            var dictionary = new Dictionary<string, string>();

            for (var i = 0; i < headers.Count; i++)
            {
                dictionary.Add(headers[i], items[i]);
            }

            return dictionary;
        }

        public async Task<List<string>> GetHeaders()
        {
            if (this.headers == null)
            {
                await this.Initialize();
                var overviewFormat = this.connection.List(ListKeyword.OverviewFormat);
                var headers = overviewFormat.Lines.Select(x => x.Trim(':')).ToList();
                headers.Insert(0, "ArticleNo");
                this.headers = headers;
            }

            return this.headers;
        }

        public void Dispose()
        {
            if (this.connection.IsConnected)
            {
                this.connection.Close();
            }
            this.connection.Dispose();
            this.isInitialized = false;
        }
    }
}