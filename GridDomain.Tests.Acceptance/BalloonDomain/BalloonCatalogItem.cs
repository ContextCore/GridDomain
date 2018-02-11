using System;
using System.ComponentModel.DataAnnotations;

namespace GridDomain.Tests.Acceptance.BalloonDomain
{
    public class BalloonCatalogItem
    {
        [Key]
        public string BalloonId { get; set; }
        public string Title { get; set; }
        public int TitleVersion { get; set; }
        public DateTime LastChanged { get; set; }
    }
}