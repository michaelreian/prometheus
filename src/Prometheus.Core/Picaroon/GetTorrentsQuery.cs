using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack.CssSelectors.NetCore;
using MediatR;

namespace Prometheus.Core.Picaroon
{
    public class GetTorrentDetailQuery : IRequest<TorrentDetail>
    {
        public string TorrentID { get; set; }
        public string ProxyUrl { get; set; }

    }

    public class TorrentDetail
    {
        public string TorrentID { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string Magnet { get; set; }
        public List<TorrentComment> Comments { get; set; } = new List<TorrentComment>();

        public string SubcategoryID { get; set; }
        public string FileCount { get; set; }
        public string Size { get; set; }
        public string Uploaded { get; set; }
        public string Uploader { get; set; }
        public string Seeders { get; set; }
        public string Leechers { get; set; }
        public string CommentCount { get; set; }
        public string Hash { get; set; }
    }

    public class TorrentComment
    {
        public string User { get; set; }
        public string UserID { get; set; }
        public string Commented { get; set; }
        public string Text { get; set; }
    }

    public class GetTorrentDetailQueryHandler : IAsyncRequestHandler<GetTorrentDetailQuery, TorrentDetail>
    {
        private readonly IMediator mediator;

        public GetTorrentDetailQueryHandler(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<TorrentDetail> Handle(GetTorrentDetailQuery message)
        {
            using (var restClient = new RestClient(message.ProxyUrl))
            {
                var resource = "torrent/{torrentID}";

                var request = new RestRequest(HttpMethod.Get, resource);
                request.AddParameter("torrentID", message.TorrentID, ParameterType.UrlSegment);

                var response = await restClient.Execute<string>(request);

                var html = await response.GetContent();

                return this.Parse(html, message.TorrentID);
            }
        }

        private TorrentDetail Parse(string html, string torrentID)
        {
            var document = new HtmlAgilityPack.HtmlDocument()
            {
                OptionFixNestedTags = true,
                OptionAutoCloseOnEnd = true
            };

            document.LoadHtml(html);

            var detailsSection = document.QuerySelector("#detailsframe");

            if (detailsSection == null)
            {
                return null;
            }
            
            var response = new TorrentDetail();
            response.TorrentID = torrentID;
            response.Name = detailsSection.QuerySelector("#title")?.InnerText;
            response.Description = detailsSection.QuerySelector(".nfo pre")?.InnerHtml;

            response.Magnet = detailsSection.QuerySelector(".download a")?.Attributes["href"]?.Value;

            var commentNodes = detailsSection.QuerySelectorAll("#comments div[id^=\"comment-\"]");

            foreach (var commentNode in commentNodes)
            {
                response.Comments.Add(new TorrentComment
                {
                    User = commentNode.QuerySelector("p.byline a")?.InnerText,
                    UserID = commentNode.QuerySelector("p.byline a")?.Attributes["href"]?.Value,
                    Text = commentNode.QuerySelector(".comment")?.InnerHtml,
                    Commented = commentNode.QuerySelector("p.byline")?.LastChild?.InnerText
                });
            }

            var dts = detailsSection.QuerySelectorAll("dl[class^=\"col\"] dt");

            foreach (var dt in dts)
            {
                var dd = dt.NextSibling?.NextSibling;
                if (dd != null)
                {
                    if (dt.InnerText.Equals("Type:", StringComparison.OrdinalIgnoreCase))
                    {
                        response.SubcategoryID = dd.QuerySelector("a")?.Attributes["href"]?.Value;
                    }
                    else if (dt.InnerText.Equals("Files:", StringComparison.OrdinalIgnoreCase))
                    {
                        response.FileCount = dd.QuerySelector("a")?.InnerText;
                    }
                    else if (dt.InnerText.Equals("Size:", StringComparison.OrdinalIgnoreCase))
                    {
                        response.Size = dd.InnerText;
                    }
                    else if (dt.InnerText.Equals("Uploaded:", StringComparison.OrdinalIgnoreCase))
                    {
                        response.Uploaded = dd.InnerText;
                    }
                    else if (dt.InnerText.Equals("By:", StringComparison.OrdinalIgnoreCase))
                    {
                        response.Uploader = dd.InnerText;
                    }
                    else if (dt.InnerText.Equals("Seeders:", StringComparison.OrdinalIgnoreCase))
                    {
                        response.Seeders = dd.InnerText;
                    }
                    else if (dt.InnerText.Equals("Leechers:", StringComparison.OrdinalIgnoreCase))
                    {
                        response.Leechers = dd.InnerText;
                    }
                    else if (dt.InnerText.Equals("Comments", StringComparison.OrdinalIgnoreCase))
                    {
                        response.CommentCount = dd.QuerySelector("span#NumComments")?.InnerText;
                    }
                    else if (dt.InnerText.Equals("Info Hash:", StringComparison.OrdinalIgnoreCase))
                    {
                        response.Hash = dd.InnerText;
                    }
                }
            }

            return response;
        }
    }

    public class GetTorrentsQuery : IRequest<GetTorrentsResponse>
    {
        public string Keywords { get; set; }
        public string CategoryID { get; set; }
        public string ProxyUrl { get; set; }
        public int Page { get; set; } = 0;
        public Direction Direction { get; set; } = Direction.Descending;
        public OrderBy OrderBy { get; set; } = OrderBy.Uploaded;
    }

    public class GetTorrentsResponse
    {
        public List<Torrent> Torrents { get; set; } = new List<Torrent>();
    }

    public class GetTorrentsQueryHandler : IAsyncRequestHandler<GetTorrentsQuery, GetTorrentsResponse>
    {
        private static readonly Regex categoryIDRegex = new Regex(@"/browse/(?<categoryID>[\d]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex torrentIDRegex = new Regex(@"/torrent/(?<torrentID>[\d]+)/.*", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private readonly IMediator mediator;

        public GetTorrentsQueryHandler(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task<GetTorrentsResponse> Handle(GetTorrentsQuery message)
        {
            if (message.CategoryID == "500")
            {
                throw new Exception("You are not allowed to see this.");
            }
            
            using (var restClient = new RestClient(message.ProxyUrl))
            {
                string resource;

                if (string.IsNullOrEmpty(message.Keywords) && message.CategoryID != "0")
                {
                    resource = "browse/{categoryID}/{page}/{sortID}";
                }
                else if (!string.IsNullOrEmpty(message.Keywords))
                {
                    resource = "search/{keywords}/{page}/{sortID}/{categoryID}";
                }
                else
                {
                    resource = "top/all";
                }

                var sortID = GetSortID(message.OrderBy, message.Direction);

                var request = new RestRequest(HttpMethod.Get, resource);
                request.AddParameter("keywords", message.Keywords, ParameterType.UrlSegment);
                request.AddParameter("categoryID", message.CategoryID, ParameterType.UrlSegment);
                request.AddParameter("page", message.Page, ParameterType.UrlSegment);
                request.AddParameter("sortID", sortID, ParameterType.UrlSegment);

                var response = await restClient.Execute<string>(request);

                var html = await response.GetContent();

                return this.Parse(html);
            }
        }

        private int GetSortID(OrderBy orderBy, Direction direction)
        {
            if (orderBy == OrderBy.Leechers && direction == Direction.Ascending)
            {
                return 8;
            }
            else if (orderBy == OrderBy.Leechers && direction == Direction.Descending)
            {
                return 7;
            }
            else if (orderBy == OrderBy.Seeders && direction == Direction.Ascending)
            {
                return 10;
            }
            else if (orderBy == OrderBy.Seeders && direction == Direction.Descending)
            {
                return 9;
            }
            else if (orderBy == OrderBy.Size && direction == Direction.Ascending)
            {
                return 6;
            }
            else if (orderBy == OrderBy.Size && direction == Direction.Descending)
            {
                return 5;
            }
            else if (orderBy == OrderBy.Uploaded && direction == Direction.Ascending)
            {
                return 4;
            }
            else if (orderBy == OrderBy.Uploaded && direction == Direction.Descending)
            {
                return 3;
            }
            else if (orderBy == OrderBy.Uploader && direction == Direction.Ascending)
            {
                return 11;
            }
            else if (orderBy == OrderBy.Uploader && direction == Direction.Descending)
            {
                return 12;
            }
            else if (orderBy == OrderBy.Name && direction == Direction.Ascending)
            {
                return 1;
            }
            else if (orderBy == OrderBy.Name && direction == Direction.Descending)
            {
                return 2;
            }
            return 0;
        }

        private GetTorrentsResponse Parse(string html)
        {
            var document = new HtmlAgilityPack.HtmlDocument()
            {
                OptionFixNestedTags = true,
                OptionAutoCloseOnEnd = true
            };

            document.LoadHtml(html);

            var response = new GetTorrentsResponse();

            var nodes = document.QuerySelectorAll("table#searchResult tr");

            foreach (var node in nodes)
            {
                var torrent = new Torrent();

                torrent.CategoryID = GetValue(categoryIDRegex, "categoryID", node.QuerySelector("td.vertTh center a:nth-child(1)")?.Attributes["href"]?.Value);
                torrent.SubcategoryID = GetValue(categoryIDRegex, "categoryID", node.QuerySelector("td.vertTh center a:nth-child(3)")?.Attributes["href"]?.Value);
                torrent.Category = node.QuerySelector("td.vertTh center a:nth-child(1)")?.InnerText;
                torrent.Subcategory = node.QuerySelector("td.vertTh center a:nth-child(3)")?.InnerText;
                torrent.Name = node.QuerySelector("a.detLink")?.InnerText;
                torrent.TorrentID = GetValue(torrentIDRegex, "torrentID", node.QuerySelector("a.detLink")?.Attributes["href"]?.Value);
                torrent.Seeders = int.Parse(node.QuerySelector("td:nth-child(3)")?.InnerText ?? "0");
                torrent.Leechers = int.Parse(node.QuerySelector("td:nth-child(4)")?.InnerText ?? "0");

                var description = node.QuerySelector("font.detDesc")?.InnerText;

                if (!string.IsNullOrEmpty(description))
                {
                    var items = description.Split(',').Select(x => x.Trim()).ToList();

                    torrent.Uploaded = items.Where(x => x.StartsWith("Uploaded ")).Select(x => x.Replace("Uploaded ", string.Empty).Trim()).FirstOrDefault();
                    torrent.Size = this.SanitizeSize(items.Where(x => x.StartsWith("Size ")).Select(x => x.Replace("Size ", string.Empty).Trim()).FirstOrDefault());
                    torrent.Uploader = items.Where(x => x.StartsWith("ULed by ")).Select(x => x.Replace("ULed by ", string.Empty).Trim()).FirstOrDefault();
                }

                if (!string.IsNullOrEmpty(torrent.TorrentID))
                {
                    response.Torrents.Add(torrent);
                }
            }

            return response;
        }

        private string SanitizeSize(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            s = s.Replace("GiB", "G");
            s = s.Replace("MiB", "M");
            
            return s.Trim();
        }

        private string GetValue(Regex regex, string name, string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                var match = regex.Match(text);

                if (match.Success)
                {
                    return match.Groups[name]?.Value;
                }
            }

            return null;
        }
    }
}