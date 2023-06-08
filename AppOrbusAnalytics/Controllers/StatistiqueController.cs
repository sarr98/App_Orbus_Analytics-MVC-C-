using AppOrbusAnalytics.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppOrbusAnalytics.Controllers
{
    public class StatistiqueController : Controller
    {
        Dal dal = new Dal();
        // GET: Statistique
        public ActionResult Accueil()
        {
            return View();
        }

        public ActionResult ModifierProfilUtilisateur(int id)
        {
            try
            {
                ANA_UTILISATEURS utilisateur = dal.GetUtlisateurById(id);

                if (utilisateur == null)
                {
                    return RedirectToAction("InvalidUser");
                }

                return View(utilisateur);
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult ModifierProfilUtilisateur(ANA_UTILISATEURS utilisateur)
        {
            try
            {
                if (utilisateur == null)
                {
                    return RedirectToAction("InvalidUser");
                }

                bool utilisateurSaved = dal.UpdateUtilisateur(utilisateur);

                if (!utilisateurSaved)
                {
                    return View(utilisateur);
                }

                utilisateur = dal.GetUtlisateurById(utilisateur.id);

                return View(utilisateur);
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }


        public ActionResult Inscription()
        {
            return View();
        }

        public ActionResult ListeEntreprises(int? page)
        {
            try
            {
                List<ANA_ENTREPRISES> listeEntreprises = dal.GetListeEntreprises()
                    .OrderByDescending(u => u.DateInscription).ToList(); ;
                int pageSize = 10; 
                int pageNumber = (page ?? 1);
                return View(listeEntreprises.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }
        [HttpPost]
        public ActionResult AjouterUtilisateur(string txtPrenom, string txtNom,string txtEmail, string txtLogin, string txtTelephone, string Profil)
        {
            try
            {

                ANA_UTILISATEURS users = new ANA_UTILISATEURS();
                users.Prenom = txtPrenom;
                users.Nom = txtNom;
                users.Email = txtEmail;
                users.userLogin = txtLogin;
                users.Telephone = int.Parse(txtTelephone);
                users.Profil = (Profil == "1") ? "Agent" : "Superviseur";
                users.DateCreation = DateTime.Now;
                users.Etat = "A";
                users.IdEntreprise = 1;
                users.Signature = "Test";
                dal.AddUser(users);
                return RedirectToAction("ListeUtilisateurs");
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult AjouterEntreprise([Required] string IdOrbus, [Required] string txtAdresse,[Required] string txtTelephone, [Required] string TypeEntreprise)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ANA_ENTREPRISES entreprise = new ANA_ENTREPRISES();
                    entreprise.IdOrbus = IdOrbus;
                    entreprise.Adresse = txtAdresse;
                    entreprise.Telephone = int.Parse(txtTelephone);
                    entreprise.TypeEntreprise = TypeEntreprise;
                    entreprise.NomouRaisonSociale = "test";
                    entreprise.Etat = "A";
                    entreprise.DateInscription = DateTime.Now;
                    entreprise.DateExpiration = DateTime.Now;
                    entreprise.LibelleTypeEntreprise = "test";
                    dal.AddEntreprise(entreprise);
                    return RedirectToAction("ListeEntreprises");
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

        public ActionResult ListeUtilisateurs(int? page)
        {
            try
            {
                List<ANA_UTILISATEURS> listeUtilisateurs = dal.GetListeUtlisateurs()
                    .OrderByDescending(u => u.DateCreation)
                    .ToList();

                int pageSize = 5;
                int pageNumber = (page ?? 1);
                return View(listeUtilisateurs.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

        public ActionResult Valeur()
        {

             return View();

        }

        [HttpPost]
        public ActionResult Valeur(DateTime DebutPeriode, DateTime FinPeriode)
        {
            try
            {
                // Vérifier si les champs ont été saisis
                if (DebutPeriode == default(DateTime) || FinPeriode == default(DateTime))
                {
                    ViewBag.Error = "Veuillez saisir une valeur pour les champs DebutPeriode et FinPeriode.";
                    return View();
                }

                // Vérifier si DebutPeriode est inférieur à FinPeriode
                if (DebutPeriode > FinPeriode)
                {
                    ViewBag.Error = "La date de début doit être inférieure à la date de fin.";
                    return View();
                }

                // Vérifier si l'écart entre DebutPeriode et FinPeriode est supérieur à 12 mois
                if (FinPeriode > DebutPeriode.AddMonths(12))
                {
                    ViewBag.Error = "L'écart entre la date de début et la date de fin ne peut pas dépasser 12 mois.";
                    return View();
                }

                List<ResultatValeur> resultats = dal.GetDataValeur(DebutPeriode, FinPeriode).ToList();

                if (resultats == null || resultats.Count == 0)
                {
                    return RedirectToAction("NoDataFound");
                }

                Dictionary<int, string> joursSemaine = new Dictionary<int, string>
                {
                    { 1, "lundi" },
                    { 2, "mardi" },
                    { 3, "mercredi" },
                    { 4, "jeudi" },
                    { 5, "vendredi" },
                    { 6, "samedi" },
                    { 7, "dimanche" }
                };

                List<string> labels = resultats.Select(r => joursSemaine.ContainsKey(r.Jour) ? joursSemaine[r.Jour] : "").ToList();
                List<double> data = resultats.Select(r => r.Total).ToList();

                ViewBag.ChartLabels = labels;
                ViewBag.ChartData = data;

                return View(resultats);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Une erreur s'est produite lors du traitement de la demande.";
                return View("Error");
            }
        }





        public ActionResult Volumetrie()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Volumetrie(DateTime DebutPeriode, DateTime FinPeriode)
        {
            try
            {
                // Vérifier si les champs ont été saisis
                if (DebutPeriode == default(DateTime) || FinPeriode == default(DateTime))
                {
                    ViewBag.Error = "Veuillez saisir une valeur pour les champs DebutPeriode et FinPeriode.";
                    return View();
                }

                // Vérifier si DebutPeriode est inférieur à FinPeriode
                if (DebutPeriode > FinPeriode)
                {
                    ViewBag.Error = "La date de début doit être inférieure à la date de fin.";
                    return View();
                }

                // Vérifier si l'écart entre DebutPeriode et FinPeriode est supérieur à 12 mois
                if (FinPeriode > DebutPeriode.AddMonths(12))
                {
                    ViewBag.Error = "L'écart entre la date de début et la date de fin ne peut pas dépasser 12 mois.";
                    return View();
                }

                List<ResultatVolumetrie> resultats = dal.GetDataVolumetrie(DebutPeriode, FinPeriode);

                if (resultats == null || resultats.Count == 0)
                {
                    return RedirectToAction("NoDataFound");
                }

                List<string> labels = resultats.Select(r => r.Lettres).ToList();
                List<int> data = resultats.Select(r => r.Total).ToList();

                ViewBag.ChartLabels = labels;
                ViewBag.ChartData = data;

                return View(resultats);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Une erreur s'est produite lors du traitement de la demande.";
                return View("Error");
            }
        }



        public ActionResult Quantites()
        {
            return View();
        }
       

        [HttpPost]
        public ActionResult Quantites(DateTime DebutPeriode, DateTime FinPeriode, string[] Produit)
        {
            try
            {
                // Vérifier si les champs ont été saisis
                if (DebutPeriode == default(DateTime) || FinPeriode == default(DateTime))
                {
                    ViewBag.Error = "Veuillez saisir une valeur pour les champs DebutPeriode et FinPeriode.";
                    return View();
                }

                // Vérifier si DebutPeriode est inférieur à FinPeriode
                if (DebutPeriode > FinPeriode)
                {
                    ViewBag.Error = "La date de début doit être inférieure à la date de fin.";
                    return View();
                }

                // Vérifier si l'écart entre DebutPeriode et FinPeriode est supérieur à 12 mois
                if (FinPeriode > DebutPeriode.AddMonths(12))
                {
                    ViewBag.Error = "L'écart entre la date de début et la date de fin ne peut pas dépasser 12 mois.";
                    return View();
                }

                // Vérifier si le champ Produit a été saisi
                if (Produit == null || Produit.Length == 0)
                {
                    ViewBag.Error = "Veuillez sélectionner au moins un produit.";
                    return View();
                }

                List<ResultatQuantites> resultats = dal.GetDataQuantites(DebutPeriode, FinPeriode, Produit);

                var moisDistincts = resultats.Select(r => r.Periode).Distinct().ToList();
                var operationsDistinctes = resultats.Select(r => r.Operation).Distinct().ToList();
                var produitsDistincts = resultats.Select(r => r.DescProduit).Distinct().ToList();
                resultats = resultats.OrderBy(r => r.Periode).ToList();

                var tableData = new List<List<string>>();

                foreach (var operation in operationsDistinctes)
                {
                    foreach (var produit in produitsDistincts)
                    {
                        var rowData = new List<string>();
                        rowData.Add(operation);
                        rowData.Add(produit);

                        foreach (var mois in moisDistincts)
                        {
                            rowData.Add("");
                        }

                        rowData.Add("");
                        tableData.Add(rowData);
                    }
                }

                foreach (var resultat in resultats)
                {
                    var rowData = tableData.FirstOrDefault(r => r[0] == resultat.Operation && r[1] == resultat.DescProduit);
                    if (rowData != null)
                    {
                        var moisIndex = moisDistincts.IndexOf(resultat.Periode);
                        rowData[moisIndex + 2] = resultat.Quantite.ToString();
                    }
                }

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Une erreur s'est produite lors du traitement de la demande.";
                return View("Error");
            }
        }


        [HttpPost]
        public JsonResult ChargerListeProduit(string codeProduit, string designation)
        {
            try
            {
                List<ProduitListe> produits = dal.ChargerListeProduit(codeProduit, designation);

                if (produits == null)
                {
                    return Json(new { error = "Aucun produit trouvé." }, JsonRequestBehavior.AllowGet);
                }

                return Json(produits, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = "Une erreur s'est produite lors du chargement de la liste des produits." }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult StatsGlobales()
        {
            return View();
        }
       

        [HttpPost]
        public ActionResult StatsGlobales(DateTime DebutPeriode, DateTime FinPeriode)
        {
            try
            {
                // Vérifier si les champs ont été saisis
                if (DebutPeriode == default(DateTime) || FinPeriode == default(DateTime))
                {
                    ViewBag.Error = "Veuillez saisir une valeur pour les champs DebutPeriode et FinPeriode.";
                    return View();
                }

                // Vérifier si DebutPeriode est inférieur à FinPeriode
                if (DebutPeriode > FinPeriode)
                {
                    ViewBag.Error = "La date de début doit être inférieure à la date de fin.";
                    return View();
                }

                // Vérifier si l'écart entre DebutPeriode et FinPeriode est supérieur à 12 mois
                if (FinPeriode > DebutPeriode.AddMonths(12))
                {
                    ViewBag.Error = "L'écart entre la date de début et la date de fin ne peut pas dépasser 12 mois.";
                    return View();
                }

                List<ResultatStatGlobalValeur> resultats = dal.GetDataStatGloblalValeur(DebutPeriode, FinPeriode);

                if (resultats == null)
                {
                    return RedirectToAction("NoDataFound");
                }

                var moisUniques = resultats.Select(r => r.Lettres).Distinct().ToList();

                var resultatsGroupes = resultats.GroupBy(r => r.Operation).Select(group => new ResultatStatGlobalValeur
                {
                    Operation = group.Key,
                    ValeursMois = moisUniques.ToDictionary(
                        m => m,
                        m => group.Sum(r => r.ValeursMois.ContainsKey(m) ? r.ValeursMois[m] : 0)
                    )
                }).ToList();

                ViewBag.MoisUniques = moisUniques;
                ViewBag.ResultatsGroupes = resultatsGroupes;

                return View(resultats);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Une erreur s'est produite lors du traitement de la demande.";
                return View("Error");
            }
        }




        public ActionResult StatsGlobalesQuantite()
        {
            return View();
        }

        [HttpPost]
        public ActionResult StatsGlobalesQuantite(DateTime DebutPeriode, DateTime FinPeriode, string[] Produit)
        {
            try
            {
                // Vérifier si les champs ont été saisis
                if (DebutPeriode == default(DateTime) || FinPeriode == default(DateTime))
                {
                    ViewBag.Error = "Veuillez saisir une valeur pour les champs DebutPeriode et FinPeriode.";
                    return View();
                }

                // Vérifier si DebutPeriode est inférieur à FinPeriode
                if (DebutPeriode > FinPeriode)
                {
                    ViewBag.Error = "La date de début doit être inférieure à la date de fin.";
                    return View();
                }

                // Vérifier si l'écart entre DebutPeriode et FinPeriode est supérieur à 12 mois
                if (FinPeriode > DebutPeriode.AddMonths(12))
                {
                    ViewBag.Error = "L'écart entre la date de début et la date de fin ne peut pas dépasser 12 mois.";
                    return View();
                }

                // Vérifier si le champ Produit a été saisi
                if (Produit == null || Produit.Length == 0)
                {
                    ViewBag.Error = "Veuillez sélectionner au moins un produit.";
                    return View();
                }

                List<ResultatStatGlobalQuantite> resultats = dal.GetStatGlobaleQuantite(DebutPeriode, FinPeriode, Produit);

                if (resultats == null)
                {
                    return RedirectToAction("NoDataFound");
                }

                var moisUniques = resultats.Select(r => r.Lettres).Distinct().OrderBy(m => m).ToList();

                var resultatsGroupes = resultats.GroupBy(r => new { r.Operation, r.Produit }).Select(group => new ResultatStatGlobalQuantite
                {
                    Operation = group.Key.Operation,
                    Produit = group.Key.Produit,
                    DescProduit = group.First().DescProduit,
                    Quantite = group.Sum(r => r.Quantite),
                    Mesure = group.First().Mesure,
                    Chiffres = group.First().Chiffres,
                    Lettres = group.First().Lettres,
                    Annee = group.First().Annee,
                    ValeursMois = moisUniques.ToDictionary(
                        m => m,
                        m => (int)group.Where(r => r.Lettres == m).Sum(r => r.Quantite)
                    )
                }).ToList();

                ViewBag.MoisUniques = moisUniques;
                ViewBag.ResultatsGroupes = resultatsGroupes;

                return View(resultats);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Une erreur s'est produite lors du traitement de la demande.";
                return View("Error");
            }
        }




        public ActionResult StatsGlobalesProd(DateTime? DebutPeriode, DateTime? FinPeriode, string[] Produit, int? page)
        {
            try
            {
                if (!DebutPeriode.HasValue || !FinPeriode.HasValue)
                {
                    ModelState.AddModelError("", "Veuillez saisir une valeur pour les champs DebutPeriode et FinPeriode.");
                    return View();
                }

                if (DebutPeriode > FinPeriode)
                {
                    ModelState.AddModelError("", "La date de début ne peut pas être supérieure à la date de fin.");
                    return View();
                }

                if (Produit == null || Produit.Length == 0)
                {
                    ModelState.AddModelError("", "Veuillez sélectionner au moins un produit.");
                    return View();
                }

                List<ResultatStatGloblalProd> resultats = dal.GetDataStatGloblalProduit(DebutPeriode.Value, FinPeriode.Value, Produit);

                int pageSize = 20;
                int pageNumber = (page ?? 1);

                var pagedResultats = resultats?.ToPagedList(pageNumber, pageSize) ?? new PagedList<ResultatStatGloblalProd>(new List<ResultatStatGloblalProd>(), pageNumber, pageSize);

                ViewBag.produit = Produit.Length > 0 ? Produit[0] : "";
                ViewBag.DebutPeriode = DebutPeriode;
                ViewBag.FinPeriode = FinPeriode;

                return View(pagedResultats);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Une erreur s'est produite lors du traitement de la demande.");
                return View();
            }
        }




        public ActionResult Guide()
        {
            return View();
        }
        public ActionResult DetailsEntreprise(int id)
        {
            try
            {
                ANA_ENTREPRISES entreprise = dal.GetEntrepriseById(id);

                if (entreprise == null)
                {
                    return RedirectToAction("EntrepriseNotFound");
                }

                return View(entreprise);
            }
            catch (Exception ex)
            {
                
                return View("Error");
            }
        }


        [HttpPost]
        public ActionResult DetailsEntreprise(ANA_ENTREPRISES entreprise)
        {
            try
            {
                bool entrepriseSaved = dal.UpdateEntreprise(entreprise);

                if (!entrepriseSaved)
                    return View(entreprise);

                entreprise = dal.GetEntrepriseById(entreprise.id);

                return View(entreprise);
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }

    }
}