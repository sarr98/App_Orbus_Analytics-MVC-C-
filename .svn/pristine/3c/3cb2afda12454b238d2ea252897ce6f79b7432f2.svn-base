using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace AppOrbusAnalytics.Models
{
    public class ModelConnexion
    {
        [Required]
        [Display(Name = "Identifiant")]
        public string Login { get; set; }

        [Required]
        ////[StringLength(100, ErrorMessage = "La chaîne {0} doit comporter au moins {2} caractères.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; }
    }
}