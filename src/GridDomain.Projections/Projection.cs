﻿using System;
using System.Threading.Tasks;

namespace GridDomain.Projections
{
    public class Projection
    {
        public string Name { get; set; }
        public string Projector { get; set; }
        public string Event { get; set; }
        public long Offset { get; set; }
        public long Version { get; set; }
    }
}