﻿using AppOrbusAnalytics.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

            ANA_UTILISATEURS utilisateur = dal.GetUtlisateurById(id);

            if (utilisateur == null)
                utilisateur = new ANA_UTILISATEURS();

            return View(utilisateur);
        }
        [HttpPost]
        public ActionResult ModifierProfilUtilisateur(ANA_UTILISATEURS utilisateur)
        {

            bool utilisateurSaved = dal.UpdateUtilisateur(utilisateur);

            if (!utilisateurSaved)
                return View(utilisateur);

            utilisateur = dal.GetUtlisateurById(utilisateur.id);

            return View(utilisateur);
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
                int pageSize = 5; 
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
        {/*
            List<PAYS> listePays = dal.GetPays();
            ViewBag.ListePays = new SelectList(listePays, "CODE_PAYS", "NOM_PAYS");*/
            return View();
        }
        [HttpPost]
        public ActionResult Valeur(DateTime DebutPeriode, DateTime FinPeriode)
        {
            List<ResultatValeur> resultats = dal.GetDataValeur(DebutPeriode, FinPeriode).ToList();

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

            // Convertir les données dans un format approprié pour le graphique
            List<string> labels = resultats.Select(r => joursSemaine[r.Jour]).ToList();
            List<double> data = resultats.Select(r => r.Total).ToList();

            // Passer les données à la vue
            ViewBag.ChartLabels = labels;
            ViewBag.ChartData = data;

            return View(resultats);
        }
      


        public ActionResult Volumetrie()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Volumetrie(DateTime DebutPeriode, DateTime FinPeriode)
        {
            List<ResultatVolumetrie> resultats = dal.GetDataVolumetrie(DebutPeriode, FinPeriode);

            // Convertir les données dans un format approprié pour le graphique
            List<string> labels = resultats.Select(r => r.Lettres).ToList();
            List<int> data = resultats.Select(r => r.Total).ToList();

            // Passer les données à la vue
            ViewBag.ChartLabels = labels;
            ViewBag.ChartData = data;

            return View(resultats);
        }

        public ActionResult Quantites()
        {
            return View();
        }
        //[HttpPost]
        //public ActionResult Quantites(DateTime DebutPeriode, DateTime FinPeriode, string Produit)
        //{
        //    string[] numerotarifdouane = Produit.Split(',');
        //    List<ResultatQuantites> resultats = dal.GetDataQuantites(DebutPeriode, FinPeriode, numerotarifdouane);

        //    var cumulParPeriode = resultats.GroupBy(r => r.Lettres)
        //                                    .Select(g => new ResultatQuantites
        //                                    {
        //                                        Lettres = g.Key,
        //                                        Cumul = g.Sum(r => r.Quantite)
        //                                    })
        //                                    .ToList();

        //    foreach (var resultat in resultats)
        //    {
        //        var cumul = cumulParPeriode.FirstOrDefault(c => c.Lettres == resultat.Lettres);
        //        if (cumul != null)
        //        {
        //            resultat.Periode = cumul.Lettres;
        //            resultat.Cumul = cumul.Cumul;
        //        }
        //    }

        //    return View(resultats);

        //}

        [HttpPost]
        public ActionResult Quantites(DateTime DebutPeriode, DateTime FinPeriode, string[] Produit)
        {
             //string[] numerotarifdouane = Produit.Split(',');
            //List<ResultatQuantites> resultats = dal.GetDataQuantites(DebutPeriode, FinPeriode, numerotarifdouane);
            List<ResultatQuantites> resultats = dal.GetDataQuantites(DebutPeriode, FinPeriode, Produit);

            var moisDistincts = resultats.Select(r => r.Periode).Distinct().ToList();
            var operationsDistinctes = resultats.Select(r => r.Operation).Distinct().ToList();
            var produitsDistincts = resultats.Select(r => r.DescProduit).Distinct().ToList();
            resultats = resultats.OrderBy(r => r.Periode).ToList();


            var tableData = new List<List<string>>();

            // Créer les lignes du tableau avec les valeurs vides
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

            // Remplir les données dans le tableau
            foreach (var resultat in resultats)
            {
                var rowData = tableData.FirstOrDefault(r => r[0] == resultat.Operation && r[1] == resultat.DescProduit);
                if (rowData != null)
                {
                    var moisIndex = moisDistincts.IndexOf(resultat.Periode);
                    rowData[moisIndex + 2] = resultat.Quantite.ToString();
                }
            }

            // Calculer les totaux cumulatifs par période
            //for (int i = 0; i < tableData.Count; i++)
            //{
            //    int cumul = 0;
            //    for (int j = 2; j < tableData[i].Count - 1; j++)
            //    {
            //        if (int.TryParse(tableData[i][j], out int quantite))
            //        {
            //            cumul += quantite;
            //        }
            //    }
            //    tableData[i][tableData[i].Count - 1] = cumul.ToString();
            //}
            

            return View();
        }

        [HttpPost]
        public JsonResult ChargerListeProduit(string codeProduit, string designation)
        {
            List<ProduitListe> produits = dal.ChargerListeProduit(codeProduit, designation);
            return Json(produits, JsonRequestBehavior.AllowGet);
        }

        public ActionResult StatsGlobales()
        {
            return View();
        }
       

        [HttpPost]
        public ActionResult StatsGlobales(DateTime DebutPeriode, DateTime FinPeriode)
        {
            List<ResultatStatGlobalValeur> resultats = dal.GetDataStatGloblalValeur(DebutPeriode, FinPeriode);

            var moisUniques = resultats.Select(r => r.Lettres).Distinct().ToList();

            //var resultatsGroupes = resultats.GroupBy(r => r.Operation).Select(group => new ResultatStatGlobalValeur
            //{
            //    Operation = group.Key,
            //    Total = group.SelectMany(r => r.ValeursMois.Values).Sum(),
            //    Cumul = group.Sum(r => r.Cumul),
            //    ValeursMois = moisUniques.ToDictionary(
            //        m => m,
            //        m => group.Sum(r => r.ValeursMois.ContainsKey(m) ? r.ValeursMois[m] : 0)
            //    )
            //}).ToList();
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


        public ActionResult StatsGlobalesQuantite()
        {
            return View();
        }
        [HttpPost]
        public ActionResult StatsGlobalesProd(DateTime DebutPeriode, DateTime FinPeriode, string[] Produit, int? page)
        {
            List<ResultatStatGloblalProd> resultats = dal.GetDataStatGloblalProduit(DebutPeriode, FinPeriode, Produit);

            int pageSize = 10; // Nombre d'éléments par page
            int pageNumber = (page ?? 1); // Numéro de la page actuelle (par défaut : 1)

            // Utilisez la bibliothèque PagedList pour paginer les résultats
            var pagedResultats = resultats.ToPagedList(pageNumber, pageSize);

            return View(pagedResultats);
        }


        public ActionResult StatsGlobalesProd()
        {
            return View();
        }

        public ActionResult Guide()
        {
            return View();
        }
        public ActionResult DetailsEntreprise(int id)
        {
            ANA_ENTREPRISES entreprise = dal.GetEntrepriseById(id);

            if (entreprise == null)
                entreprise = new ANA_ENTREPRISES();

            return View(entreprise);
        }

        [HttpPost]
        public ActionResult DetailsEntreprise(ANA_ENTREPRISES entreprise)
        {

            bool entrepriseSaved = dal.UpdateEntreprise(entreprise);

            if (!entrepriseSaved)
                return View(entreprise);

            entreprise = dal.GetEntrepriseById(entreprise.id);

            return View(entreprise);
        }
    }
}