//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class ANA_UTILISATEURS
    {
        public int id { get; set; }
        public string userLogin { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string Profil { get; set; }
        public Nullable<int> Telephone { get; set; }
        public int IdEntreprise { get; set; }
        public string Etat { get; set; }
        public System.DateTime DateCreation { get; set; }
        public string Signature { get; set; }
    }
}
