﻿// <auto-generated />

using System.CodeDom.Compiler;
using System.Data.Entity.Migrations.Infrastructure;
using System.Resources;

namespace GridDomain.EventStore.MSSQL.Migrations
{
    [GeneratedCode("EntityFramework.Migrations", "6.1.3-40302")]
    public sealed partial class CompactLogRecord : IMigrationMetadata
    {
        private readonly ResourceManager Resources = new ResourceManager(typeof(CompactLogRecord));
        
        string IMigrationMetadata.Id
        {
            get { return "201509111831351_CompactLogRecord"; }
        }
        
        string IMigrationMetadata.Source
        {
            get { return null; }
        }
        
        string IMigrationMetadata.Target
        {
            get { return Resources.GetString("Target"); }
        }
    }
}