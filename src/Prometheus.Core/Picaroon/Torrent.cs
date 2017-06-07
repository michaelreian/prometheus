namespace Prometheus.Core.Picaroon
{
    public class Torrent
    {
        public string CategoryID { get; set; }
        public string SubcategoryID { get; set; }
        public string Category { get; set; }
        public string Subcategory { get; set; }
        public string TorrentID { get; set; }
        public string Name { get; set; }
        public int Seeders { get; set; }
        public int Leechers { get; set; }
        public string Uploaded { get; set; }
        public string Size { get; set; }
        public string Uploader { get; set; }
    }
}