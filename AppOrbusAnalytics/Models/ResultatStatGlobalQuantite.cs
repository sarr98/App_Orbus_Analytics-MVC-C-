using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppOrbusAnalytics.Models
{
    public class ResultatStatGlobalQuantite
    {
        public string Operation { get; set; }
        public string Produit { get; set; }
        public string DescProduit { get; set; }
        public double Quantite { get; set; }
        public string Mesure { get; set; }
        public int Chiffres { get; set; }
        public string Lettres { get; set; }
        public string Annee { get; set; }
        public Dictionary<string, int> ValeursMois { get; set; }
    }
}