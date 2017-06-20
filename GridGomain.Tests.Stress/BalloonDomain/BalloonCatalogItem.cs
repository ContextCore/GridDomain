using System;
using System.ComponentModel.DataAnnotations;

namespace GridGomain.Tests.Stress.BalloonDomain
{
    public class BalloonCatalogItem
    {
        [Key]
        public Guid BalloonId { get; set; }
        public string Title { get; set; }
        public int TitleVersion { get; set; }
        public DateTime LastChanged { get; set; }
    }
}