using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppOrbusAnalytics.Models
{
    public class Dal
    {
        APPLICATIONSEntities db = new APPLICATIONSEntities();
        public List<ResultatStatGlobalQuantite> GetStatGlobaleQuantite( DateTime startDate, DateTime endDate,string[] numerotarifdouane)
        {
            List<ResultatStatGlobalQuantite> results = new List<ResultatStatGlobalQuantite>();

            try
            {
                List<string> numerotarifdouaneTab = numerotarifdouane[0].Split(',').ToList();
                List<string> numerotarifdouane2 = new List<string>();
                if (numerotarifdouane.Length > 0)
                {
                    foreach (string item in numerotarifdouaneTab)
                    {
                        numerotarifdouane2.Add("'" + item.Trim() + "'");
                    }
                }

                string tarifDouaneParams = string.Join(", ", numerotarifdouane2);

                string query = @"
            SELECT
                do.IMPORTATIONOUEXPORTATION AS [Operation],
                co.NUMEROTARIFDOUANE AS [Produit],
                ta.LIBELLETARIFDOUANE AS [DescProduit],
                SUM(co.QUANTITEMESURE) AS [Quantite],
                co.UNITEMESURE AS [Mesure],
                DATEPART(MONTH, datedossiertps) AS [Chiffres],
                DATENAME(MONTH, do.DATEDOSSIERTPS) + '-' + RIGHT(DATENAME(YEAR, do.DATEDOSSIERTPS), 2) AS [Lettres],
                DATENAME(YEAR, do.DATEDOSSIERTPS) AS [Annee]
            FROM
                FACTURE fa
                INNER JOIN DOSSIERTPS do ON do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS
                INNER JOIN CONTENIR co ON co.NUMERODOSSIERTPS = do.NUMERODOSSIERTPS
                INNER JOIN TARIFDOUANE ta ON co.NUMEROTARIFDOUANE = ta.NUMEROTARIFDOUANE
            WHERE
                co.NUMEROTARIFDOUANE IN ({0})
                AND do.DATEDOSSIERTPS >= '{1}'
                AND do.DATEDOSSIERTPS <= '{2}'
            GROUP BY
                do.IMPORTATIONOUEXPORTATION,
                co.NUMEROTARIFDOUANE,
                ta.LIBELLETARIFDOUANE,
                co.UNITEMESURE,
                co.QUANTITEMESURE,
                DATENAME(MONTH, do.DATEDOSSIERTPS),
                DATEPART(MONTH, datedossiertps),
                DATENAME(YEAR, do.DATEDOSSIERTPS)
            ORDER BY
                do.IMPORTATIONOUEXPORTATION,
                co.NUMEROTARIFDOUANE,
                ta.LIBELLETARIFDOUANE,
                co.UNITEMESURE,
                DATENAME(YEAR, do.DATEDOSSIERTPS),
                DATEPART(MONTH, datedossiertps);
        ";

                query = string.Format(query, tarifDouaneParams, startDate.ToString("yyyy-dd-MM"), endDate.ToString("yyyy-dd-MM"));

                results = db.Database.SqlQuery<ResultatStatGlobalQuantite>(query).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreur s'est produite : " + ex.Message);
            }

            return results;
        }

        public List<BeneficiaireDetails> ChargerListeBeneficiaire(string codePPM, string beneficiaire)
        {
            List<BeneficiaireDetails> details = new List<BeneficiaireDetails>();

            try
            {
                var benefData = db.CLIENTS
                    .Where(c => c.CODEPPM.Contains(codePPM) && c.NOMOURAISONSOCIALEBENEFICIAIRE.Contains(beneficiaire))
                    .Select(c => new BeneficiaireDetails
                    {
                        codePPM = c.CODEPPM,
                        beneficiaire = c.NOMOURAISONSOCIALEBENEFICIAIRE
                    })
                    .ToList();

                details = benefData;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreur s'est produite : " + ex.Message);

                return null;
            }

            return details;
        }

        public List<ResultatStatGloblalProd> GetDataStatGloblalProduit(DateTime startDate, DateTime endDate, string[] numerotarifdouane)
        {
            List<ResultatStatGloblalProd> results = new List<ResultatStatGloblalProd>();

        try
        {
                List<string> numerotarifdouaneTab = numerotarifdouane[0].Split(',').ToList();
                List<string> numerotarifdouane2 = new List<string>();
                if (numerotarifdouane.Length > 0)
                {
                    foreach (string item in numerotarifdouaneTab)
                    {
                        numerotarifdouane2.Add("'" + item.Trim() + "'");
                    }
                }

                string tarifDouaneParams = string.Join(", ", numerotarifdouane2);

                string query = @"
                SELECT 
                    ltrim(do.NOMOURAISONSOCIALEBENEFICIAIRE) AS [Client], 
                    co.NUMEROTARIFDOUANE AS [Produit], 
                    ta.LIBELLETARIFDOUANE AS [DescProduit],  
                    SUM(co.QUANTITEMESURE) AS [Quantite], 
                    co.UNITEMESURE AS [Mesure], 
                    SUM(co.VALEURCFA) AS [Total],
                    COUNT(DISTINCT do.NUMERODOSSIERTPS) AS [Dossier]
                FROM 
                    FACTURE fa
                    INNER JOIN DOSSIERTPS do ON do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS
                    INNER JOIN CONTENIR co ON co.NUMERODOSSIERTPS = do.NUMERODOSSIERTPS  
                    INNER JOIN TARIFDOUANE ta ON co.NUMEROTARIFDOUANE = ta.NUMEROTARIFDOUANE 
                WHERE
                    co.NUMEROTARIFDOUANE IN ({0})
                    AND do.DATEDOSSIERTPS >= '{1}'
                    AND do.DATEDOSSIERTPS <= '{2}'
                GROUP BY 
                    do.NOMOURAISONSOCIALEBENEFICIAIRE, 
                    co.NUMEROTARIFDOUANE, 
                    ta.LIBELLETARIFDOUANE, 
                    co.UNITEMESURE
                ORDER BY 
                    do.NOMOURAISONSOCIALEBENEFICIAIRE, 
                    co.NUMEROTARIFDOUANE, 
                    ta.LIBELLETARIFDOUANE, 
                    co.UNITEMESURE;
            ";



        query = string.Format(query, tarifDouaneParams, startDate.ToString("yyyy-dd-MM"), endDate.ToString("yyyy-dd-MM"));

                results = db.Database.SqlQuery<ResultatStatGloblalProd>(query).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreur s'est produite : " + ex.Message);
            }

            return results;
        }

        public List<ResultatStatGlobalValeur> GetDataStatGloblalValeur(DateTime startDate, DateTime endDate)
        {
            try
            {
                string query = "SELECT do.IMPORTATIONOUEXPORTATION [Operation], " +
                               "SUM(fa.VALEURTOTALECFA) [Total], " +
                               "DATEPART(MONTH,datedossiertps) [Chiffres], " +
                               "DATENAME(MONTH, do.DATEDOSSIERTPS) + '-' + " +
                               "RIGHT(DATENAME(Year, do.DATEDOSSIERTPS), 2) [Lettres], " +
                               "DATENAME(YEAR, do.DATEDOSSIERTPS) [Annee] " +
                               "FROM FACTURE fa " +
                               "INNER JOIN DOSSIERTPS do ON do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS " +
                               "WHERE 1=1 AND do.DATEDOSSIERTPS >= @StartDate AND do.DATEDOSSIERTPS <= @EndDate " +
                               "GROUP BY do.IMPORTATIONOUEXPORTATION, " +
                               "DATENAME(MONTH, do.DATEDOSSIERTPS), " +
                               "DATEPART(MONTH, datedossiertps), " +
                               "DATENAME(YEAR, do.DATEDOSSIERTPS) " +
                               "ORDER BY do.IMPORTATIONOUEXPORTATION, " +
                               "DATENAME(YEAR, do.DATEDOSSIERTPS), " +
                               "DATEPART(MONTH, datedossiertps)";

                List<ResultatStatGlobalValeur> resultat = db.Database.SqlQuery<ResultatStatGlobalValeur>(query,
                    new SqlParameter("@StartDate", startDate),
                    new SqlParameter("@EndDate", endDate)).ToList();
                

                return resultat;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Une erreur s'est produite lors de l'exécution de la requête : " + ex.Message);
                return null; 
            }
        }

        public List<ResultatValeur> GetDataValeur(DateTime startDate, DateTime endDate)
        {


            string query = @"SELECT SUM(fa.VALEURTOTALECFA) AS Total,
                           FORMAT(do.DATEDOSSIERTPS, 'dd/MM/yyyy') AS Chiffres,
                           DATENAME(WEEKDAY, do.DATEDOSSIERTPS) AS Lettres,
                           DATEPART(month, do.DATEDOSSIERTPS) AS Mois,
                           DATEPART(WEEKDAY, do.DATEDOSSIERTPS) AS Jour
                    FROM FACTURE fa
                    INNER JOIN DOSSIERTPS do ON do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS
                    WHERE do.DATEDOSSIERTPS >= @StartDate
                      AND do.DATEDOSSIERTPS <= @EndDate
                      AND EXISTS (SELECT 1 FROM JOINDRE_16 jo WHERE jo.NUMERODOSSIERTPS = do.NUMERODOSSIERTPS)
                    GROUP BY DATEPART(WEEKDAY, do.DATEDOSSIERTPS),
                             FORMAT(do.DATEDOSSIERTPS, 'dd/MM/yyyy'),
                             DATEPART(month, do.DATEDOSSIERTPS),
                             DATENAME(WEEKDAY, do.DATEDOSSIERTPS)
                    ORDER BY Jour, Mois, Chiffres ASC";

            var getData = db.Database.SqlQuery<ResultatValeur>(query,
                new SqlParameter("@StartDate", startDate),
                new SqlParameter("@EndDate", endDate)).ToList();

            return getData;
        }

        public List<ResultatQuantites> GetDataQuantites(DateTime startDate, DateTime endDate, string[] numerotarifdouane)
        {

            try
            {
                List<string> numerotarifdouane2 = new List<string>();
                if (numerotarifdouane.Length > 0)
                    foreach (string item in numerotarifdouane)
                    { numerotarifdouane2.Add("'" + item + "'"); }
                        


                string query = string.Format(@"SELECT
                    do.IMPORTATIONOUEXPORTATION AS [Operation],
                    co.NUMEROTARIFDOUANE AS [Produit],
                    ta.LIBELLETARIFDOUANE AS [DescProduit],
                    SUM(co.QUANTITEMESURE) AS [Quantite],
                    co.UNITEMESURE AS [Mesure],
                    DATEPART(MONTH, do.DATEDOSSIERTPS) AS [Chiffres],
                    CONCAT(DATENAME(MONTH, do.DATEDOSSIERTPS), '-', RIGHT(DATENAME(YEAR, do.DATEDOSSIERTPS), 2)) AS [Lettres],
                    DATENAME(YEAR, do.DATEDOSSIERTPS) AS [Annee]
                FROM
                    FACTURE AS fa
                    INNER JOIN DOSSIERTPS AS do ON do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS
                    INNER JOIN CONTENIR AS co ON co.NUMERODOSSIERTPS = do.NUMERODOSSIERTPS
                    INNER JOIN TARIFDOUANE AS ta ON co.NUMEROTARIFDOUANE = ta.NUMEROTARIFDOUANE
                WHERE
                    co.NUMEROTARIFDOUANE IN ({0})
                    AND do.DATEDOSSIERTPS >= '{1}'
                    AND do.DATEDOSSIERTPS <= '{2}'
                GROUP BY
                    do.IMPORTATIONOUEXPORTATION,
                    co.NUMEROTARIFDOUANE,
                    ta.LIBELLETARIFDOUANE,
                    co.UNITEMESURE,
                    DATENAME(MONTH, do.DATEDOSSIERTPS),
                    DATEPART(MONTH, do.DATEDOSSIERTPS),
                    DATENAME(YEAR, do.DATEDOSSIERTPS)
                ORDER BY
                    do.IMPORTATIONOUEXPORTATION,
                    co.NUMEROTARIFDOUANE,
                    ta.LIBELLETARIFDOUANE,
                    co.UNITEMESURE,
                    DATENAME(YEAR, do.DATEDOSSIERTPS),
                    DATEPART(MONTH, do.DATEDOSSIERTPS)",
                    string.Join(",", numerotarifdouane2),
                    startDate,
                    endDate
                );


                
                List <ResultatQuantites> result = db.Database.SqlQuery<ResultatQuantites>(query).ToList();

                return result;
            }
            catch (Exception ex)
            {
                // Gérez l'exception appropriée ici
                return null;
            }
        }

        public List<ANA_ENTREPRISES> GetListeEntreprises()
        {
            try
            {
                List<ANA_ENTREPRISES> listeEntreprises = db.ANA_ENTREPRISES.ToList();

                return listeEntreprises;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public ANA_ENTREPRISES GetEntrepriseById(int id)
        {
            try
            {
                ANA_ENTREPRISES entreprise = db.ANA_ENTREPRISES.Where(u => u.id == id).FirstOrDefault();

                return entreprise;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public bool UpdateUtilisateur(ANA_UTILISATEURS utilisateur)
        {
            try
            {
                ANA_UTILISATEURS utilisateurUpd = db.ANA_UTILISATEURS.Where(u => u.id == utilisateur.id).FirstOrDefault();
                if (utilisateurUpd == null)
                    throw new Exception();

                utilisateurUpd.userLogin = utilisateur.userLogin;
                utilisateurUpd.Nom = utilisateur.Nom;
                utilisateurUpd.Prenom = utilisateur.Prenom;
                utilisateurUpd.Email = utilisateur.Email;
                utilisateurUpd.Telephone = utilisateur.Telephone;
                db.Entry(utilisateurUpd).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool UpdateEntreprise(ANA_ENTREPRISES entreprise)
        {
            try
            {
                ANA_ENTREPRISES entrepriseUpd = db.ANA_ENTREPRISES.Where(u => u.id == entreprise.id).FirstOrDefault();
                if (entrepriseUpd == null)
                    throw new Exception();

                entrepriseUpd.Adresse = entreprise.Adresse;
                entrepriseUpd.Telephone = entreprise.Telephone;

                db.Entry(entrepriseUpd).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public List<ANA_UTILISATEURS> GetListeUtlisateurs()
        {
            try
            {
                List<ANA_UTILISATEURS> listeUtlisateurs = db.ANA_UTILISATEURS.ToList();

                return listeUtlisateurs;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public ANA_UTILISATEURS GetUtlisateurById(int id)
        {
            try
            {
                ANA_UTILISATEURS Utlisateur = db.ANA_UTILISATEURS.Where(u => u.id == id).FirstOrDefault();

                return Utlisateur;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public ANA_UTILISATEURS AddUser(ANA_UTILISATEURS user)
        {
            try
            {
                ANA_UTILISATEURS useradd = db.ANA_UTILISATEURS.Add(user);
                db.SaveChanges();
                return useradd;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public ANA_ENTREPRISES AddEntreprise(ANA_ENTREPRISES entreprise)
        {
            try
            {
                ANA_ENTREPRISES entrepriseadd = db.ANA_ENTREPRISES.Add(entreprise);
                db.SaveChanges();
                return entrepriseadd;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        public List<PAYS> GetPays()
        {
            try
            {
                List<PAYS> listePays = db.PAYS.ToList();
                return listePays;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<TARIFDOUANE> GetTarifsDouane()
        {
            try
            {
                List<TARIFDOUANE> listeTarifsDouane = db.TARIFDOUANE
                  .Select(t => new TARIFDOUANE { NUMEROTARIFDOUANE = t.NUMEROTARIFDOUANE, LIBELLETARIFDOUANE = t.LIBELLETARIFDOUANE })
                  .OrderBy(t => t.LIBELLETARIFDOUANE)
                  .ToList();
                return listeTarifsDouane;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<DEVISE> GetDevises()
        {
            try
            {
                List<DEVISE> listDevise = db.DEVISE.Where(d => d.NOMDEVISE != "")
                .OrderBy(d => d.NOMDEVISE)
                .Select(d => new DEVISE { CODEDEVISE = d.CODEDEVISE, NOMDEVISE = d.NOMDEVISE })
                .ToList();
                return listDevise;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public decimal ConvertirEnDevise(decimal valeur, string codeDevise)
        {
            try
            {
                var tauxDevise = db.DEVISE
                    .Where(d => d.CODEDEVISE == codeDevise && d.TAUXDEVISE != null)
                    .Select(d => d.TAUXDEVISE)
                    .FirstOrDefault();

                if (tauxDevise != null)
                {
                    if (tauxDevise != 1)
                    {
                        valeur = valeur * (decimal)tauxDevise;
                    }
                    else
                    {
                        valeur = valeur / (decimal)tauxDevise;
                    }
                }

                return valeur;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public List<ResultatVolumetrie> GetDataVolumetrie(DateTime startDate, DateTime endDate)
        {
            string query = @"SELECT COUNT(DISTINCT do.NUMERODOSSIERTPS) AS Total,
                           CONVERT(VARCHAR(10), do.DATEDOSSIERTPS, 103) AS Chiffres,
                           DATENAME(WEEKDAY, do.DATEDOSSIERTPS) AS Lettres,
                           DATEPART(MONTH, do.DATEDOSSIERTPS) AS Mois,
                           DATEPART(WEEKDAY, do.DATEDOSSIERTPS) AS Jour
                             FROM FACTURE fa
                     INNER JOIN DOSSIERTPS do ON do.NUMERODOSSIERTPS = fa.NUMERODOSSIERTPS
                     WHERE do.DATEDOSSIERTPS >= @StartDate
                       AND do.DATEDOSSIERTPS <= @EndDate
                       AND EXISTS (SELECT 1 FROM JOINDRE_16 jo WHERE jo.NUMERODOSSIERTPS = do.NUMERODOSSIERTPS)
                     GROUP BY DATEPART(WEEKDAY, do.DATEDOSSIERTPS),
                              CONVERT(VARCHAR(10), do.DATEDOSSIERTPS, 103),
                              DATEPART(MONTH, do.DATEDOSSIERTPS),
                              DATENAME(WEEKDAY, do.DATEDOSSIERTPS)
                     ORDER BY Jour, Mois, Chiffres ASC";

            var getData = db.Database.SqlQuery<ResultatVolumetrie>(query,
                new SqlParameter("@StartDate", startDate),
                new SqlParameter("@EndDate", endDate)).ToList();

            return getData;
        }

        public List<ProduitListe> ChargerListeProduit(string codeProduit, string designation)
        {
            List<ProduitListe> detailsProduits = new List<ProduitListe>();

            if (!string.IsNullOrEmpty(codeProduit) && !string.IsNullOrEmpty(designation))
            {
                codeProduit = codeProduit.Replace("'", "''");
                designation = designation.Replace("'", "''");

                var produitData = from produit in db.TARIFDOUANE
                                  where produit.NUMEROTARIFDOUANE.StartsWith(codeProduit) &&
                                        produit.LIBELLETARIFDOUANE.ToUpper().Contains(designation.ToUpper())
                                  select new ProduitListe
                                  {
                                      codeProduit = produit.NUMEROTARIFDOUANE,
                                      designation = produit.LIBELLETARIFDOUANE
                                  };

                detailsProduits = produitData.ToList();
            }
            else if (!string.IsNullOrEmpty(codeProduit))
            {
                codeProduit = codeProduit.Replace("'", "''");

                var produitData = from produit in db.TARIFDOUANE
                                  where produit.NUMEROTARIFDOUANE.StartsWith(codeProduit)
                                  select new ProduitListe
                                  {
                                      codeProduit = produit.NUMEROTARIFDOUANE,
                                      designation = produit.LIBELLETARIFDOUANE
                                  };

                detailsProduits = produitData.ToList();
            }
            else if (!string.IsNullOrEmpty(designation))
            {
                designation = designation.Replace("'", "''");

                var produitData = from produit in db.TARIFDOUANE
                                  where produit.LIBELLETARIFDOUANE.ToUpper().Contains(designation.ToUpper())
                                  select new ProduitListe
                                  {
                                      codeProduit = produit.NUMEROTARIFDOUANE,
                                      designation = produit.LIBELLETARIFDOUANE
                                  };

                detailsProduits = produitData.ToList();
            }

            return detailsProduits;
        }
    }
}
