﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AppOrbusAnalytics.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class APPLICATIONSEntities : DbContext
    {
        public APPLICATIONSEntities()
            : base("name=APPLICATIONSEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ANA_ENTREPRISES> ANA_ENTREPRISES { get; set; }
        public virtual DbSet<ANA_UTILISATEURS> ANA_UTILISATEURS { get; set; }
        public virtual DbSet<PAYS> PAYS { get; set; }
        public virtual DbSet<TARIFDOUANE> TARIFDOUANE { get; set; }
        public virtual DbSet<CLIENTS> CLIENTS { get; set; }
        public virtual DbSet<DEVISE> DEVISE { get; set; }
        public virtual DbSet<OPERATEUR> OPERATEUR { get; set; }
        public virtual DbSet<DOSSIERTPS> DOSSIERTPS { get; set; }
        public virtual DbSet<FACTURE> FACTURE { get; set; }
        public virtual DbSet<CONTENIR> CONTENIR { get; set; }
    }
}
