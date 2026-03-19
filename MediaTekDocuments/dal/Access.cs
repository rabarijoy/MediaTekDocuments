using System;
using System.Collections.Generic;
using MediaTekDocuments.model;
using MediaTekDocuments.manager;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Linq;

namespace MediaTekDocuments.dal
{
    /// <summary>
    /// Classe d'accès aux données
    /// </summary>
    public class Access
    {
        /// <summary>
        /// instance unique de la classe
        /// </summary>
        private static Access instance = null;
        /// <summary>
        /// instance de ApiRest pour envoyer des demandes vers l'api et recevoir la réponse
        /// </summary>
        private readonly ApiRest api = null;
        /// <summary>
        /// méthode HTTP pour select
        /// </summary>
        private const string GET = "GET";
        /// <summary>
        /// méthode HTTP pour insert
        /// </summary>
        private const string POST = "POST";
        /// <summary>
        /// méthode HTTP pour update
        /// </summary>
        private const string PUT = "PUT";
        /// <summary>
        /// méthode HTTP pour delete
        /// </summary>
        private const string DELETE = "DELETE";

        /// <summary>
        /// Méthode privée pour créer un singleton
        /// initialise l'accès à l'API en lisant les paramètres depuis App.config
        /// </summary>
        private Access()
        {
            try
            {
                string uriApi  = ConfigurationManager.AppSettings["ApiBaseUrl"];
                string user    = ConfigurationManager.AppSettings["ApiUser"];
                string pwd     = ConfigurationManager.AppSettings["ApiPassword"];
                string authStr = user + ":" + pwd;
                api = ApiRest.GetInstance(uriApi, authStr);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Création et retour de l'instance unique de la classe
        /// </summary>
        /// <returns>instance unique de la classe</returns>
        public static Access GetInstance()
        {
            if(instance == null)
            {
                instance = new Access();
            }
            return instance;
        }

        /// <summary>
        /// Retourne tous les genres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Genre</returns>
        public List<Categorie> GetAllGenres()
        {
            IEnumerable<Genre> lesGenres = TraitementRecup<Genre>(GET, "genre", null);
            return new List<Categorie>(lesGenres);
        }

        /// <summary>
        /// Retourne tous les rayons à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Rayon</returns>
        public List<Categorie> GetAllRayons()
        {
            IEnumerable<Rayon> lesRayons = TraitementRecup<Rayon>(GET, "rayon", null);
            return new List<Categorie>(lesRayons);
        }

        /// <summary>
        /// Retourne toutes les catégories de public à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Public</returns>
        public List<Categorie> GetAllPublics()
        {
            IEnumerable<Public> lesPublics = TraitementRecup<Public>(GET, "public", null);
            return new List<Categorie>(lesPublics);
        }

        /// <summary>
        /// Retourne toutes les livres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            List<Livre> lesLivres = TraitementRecup<Livre>(GET, "livre", null);
            return lesLivres;
        }

        /// <summary>
        /// Retourne toutes les dvd à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Dvd</returns>
        public List<Dvd> GetAllDvd()
        {
            List<Dvd> lesDvd = TraitementRecup<Dvd>(GET, "dvd", null);
            return lesDvd;
        }

        /// <summary>
        /// Retourne toutes les revues à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public List<Revue> GetAllRevues()
        {
            List<Revue> lesRevues = TraitementRecup<Revue>(GET, "revue", null);
            return lesRevues;
        }

        /// <summary>
        /// Retourne les exemplaires d'une revue
        /// </summary>
        /// <param name="idDocument">id de la revue concernée</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesRevue(string idDocument)
        {
            String jsonIdDocument = convertToJson("id", idDocument);
            List<Exemplaire> lesExemplaires = TraitementRecup<Exemplaire>(GET, "exemplaire/" + jsonIdDocument, null);
            return lesExemplaires;
        }

        /// <summary>
        /// ecriture d'un exemplaire en base de données
        /// </summary>
        /// <param name="exemplaire">exemplaire à insérer</param>
        /// <returns>true si l'insertion a pu se faire (retour != null)</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            String jsonExemplaire = JsonConvert.SerializeObject(exemplaire, new CustomDateTimeConverter());
            try
            {
                List<Exemplaire> liste = TraitementRecup<Exemplaire>(POST, "exemplaire", "champs=" + jsonExemplaire);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        // =====================================================================
        // LIVRE : ajout, modification, suppression
        // =====================================================================

        /// <summary>
        /// Insère un livre dans les tables document, livres_dvd et livre
        /// </summary>
        /// <param name="livre">le livre à insérer</param>
        /// <returns>true si l'opération a réussi</returns>
        public bool AjouterLivre(Livre livre)
        {
            Dictionary<string, object> champs = new Dictionary<string, object>
            {
                { "id",         livre.Id },
                { "titre",      livre.Titre },
                { "image",      livre.Image },
                { "idRayon",    livre.IdRayon },
                { "idPublic",   livre.IdPublic },
                { "idGenre",    livre.IdGenre },
                { "ISBN",       livre.Isbn },
                { "auteur",     livre.Auteur },
                { "collection", livre.Collection }
            };
            string jsonChamps = JsonConvert.SerializeObject(champs);
            return TraitementAction(POST, "livre", "champs=" + jsonChamps);
        }

        /// <summary>
        /// Modifie un livre dans les tables document et livre
        /// </summary>
        /// <param name="livre">le livre avec les nouvelles valeurs</param>
        /// <returns>true si l'opération a réussi</returns>
        public bool ModifierLivre(Livre livre)
        {
            Dictionary<string, object> champs = new Dictionary<string, object>
            {
                { "titre",      livre.Titre },
                { "image",      livre.Image },
                { "idRayon",    livre.IdRayon },
                { "idPublic",   livre.IdPublic },
                { "idGenre",    livre.IdGenre },
                { "ISBN",       livre.Isbn },
                { "auteur",     livre.Auteur },
                { "collection", livre.Collection }
            };
            string jsonChamps = JsonConvert.SerializeObject(champs);
            return TraitementAction(PUT, "livre/" + livre.Id, "champs=" + jsonChamps);
        }

        /// <summary>
        /// Supprime un livre (et ses entrées dans document et livres_dvd)
        /// Refuse si des exemplaires ou commandes existent
        /// </summary>
        /// <param name="id">identifiant du livre</param>
        /// <returns>true si la suppression a réussi</returns>
        public bool SupprimerLivre(string id)
        {
            string jsonFiltreId = convertToJson("id", id);
            return TraitementAction(DELETE, "livre/" + jsonFiltreId, "");
        }

        // =====================================================================
        // DVD : ajout, modification, suppression
        // =====================================================================

        /// <summary>
        /// Insère un dvd dans les tables document, livres_dvd et dvd
        /// </summary>
        /// <param name="dvd">le dvd à insérer</param>
        /// <returns>true si l'opération a réussi</returns>
        public bool AjouterDvd(Dvd dvd)
        {
            Dictionary<string, object> champs = new Dictionary<string, object>
            {
                { "id",          dvd.Id },
                { "titre",       dvd.Titre },
                { "image",       dvd.Image },
                { "idRayon",     dvd.IdRayon },
                { "idPublic",    dvd.IdPublic },
                { "idGenre",     dvd.IdGenre },
                { "synopsis",    dvd.Synopsis },
                { "realisateur", dvd.Realisateur },
                { "duree",       dvd.Duree }
            };
            string jsonChamps = JsonConvert.SerializeObject(champs);
            return TraitementAction(POST, "dvd", "champs=" + jsonChamps);
        }

        /// <summary>
        /// Modifie un dvd dans les tables document et dvd
        /// </summary>
        /// <param name="dvd">le dvd avec les nouvelles valeurs</param>
        /// <returns>true si l'opération a réussi</returns>
        public bool ModifierDvd(Dvd dvd)
        {
            Dictionary<string, object> champs = new Dictionary<string, object>
            {
                { "titre",       dvd.Titre },
                { "image",       dvd.Image },
                { "idRayon",     dvd.IdRayon },
                { "idPublic",    dvd.IdPublic },
                { "idGenre",     dvd.IdGenre },
                { "synopsis",    dvd.Synopsis },
                { "realisateur", dvd.Realisateur },
                { "duree",       dvd.Duree }
            };
            string jsonChamps = JsonConvert.SerializeObject(champs);
            return TraitementAction(PUT, "dvd/" + dvd.Id, "champs=" + jsonChamps);
        }

        /// <summary>
        /// Supprime un dvd (et ses entrées dans document et livres_dvd)
        /// Refuse si des exemplaires ou commandes existent
        /// </summary>
        /// <param name="id">identifiant du dvd</param>
        /// <returns>true si la suppression a réussi</returns>
        public bool SupprimerDvd(string id)
        {
            string jsonFiltreId = convertToJson("id", id);
            return TraitementAction(DELETE, "dvd/" + jsonFiltreId, "");
        }

        // =====================================================================
        // REVUE : ajout, modification, suppression
        // =====================================================================

        /// <summary>
        /// Insère une revue dans les tables document et revue
        /// </summary>
        /// <param name="revue">la revue à insérer</param>
        /// <returns>true si l'opération a réussi</returns>
        public bool AjouterRevue(Revue revue)
        {
            Dictionary<string, object> champs = new Dictionary<string, object>
            {
                { "id",              revue.Id },
                { "titre",           revue.Titre },
                { "image",           revue.Image },
                { "idRayon",         revue.IdRayon },
                { "idPublic",        revue.IdPublic },
                { "idGenre",         revue.IdGenre },
                { "periodicite",     revue.Periodicite },
                { "delaiMiseADispo", revue.DelaiMiseADispo }
            };
            string jsonChamps = JsonConvert.SerializeObject(champs);
            return TraitementAction(POST, "revue", "champs=" + jsonChamps);
        }

        /// <summary>
        /// Modifie une revue dans les tables document et revue
        /// </summary>
        /// <param name="revue">la revue avec les nouvelles valeurs</param>
        /// <returns>true si l'opération a réussi</returns>
        public bool ModifierRevue(Revue revue)
        {
            Dictionary<string, object> champs = new Dictionary<string, object>
            {
                { "titre",           revue.Titre },
                { "image",           revue.Image },
                { "idRayon",         revue.IdRayon },
                { "idPublic",        revue.IdPublic },
                { "idGenre",         revue.IdGenre },
                { "periodicite",     revue.Periodicite },
                { "delaiMiseADispo", revue.DelaiMiseADispo }
            };
            string jsonChamps = JsonConvert.SerializeObject(champs);
            return TraitementAction(PUT, "revue/" + revue.Id, "champs=" + jsonChamps);
        }

        /// <summary>
        /// Supprime une revue (et son entrée dans document)
        /// Refuse si des exemplaires ou abonnements existent
        /// </summary>
        /// <param name="id">identifiant de la revue</param>
        /// <returns>true si la suppression a réussi</returns>
        public bool SupprimerRevue(string id)
        {
            string jsonFiltreId = convertToJson("id", id);
            return TraitementAction(DELETE, "revue/" + jsonFiltreId, "");
        }

        // =====================================================================
        // COMMANDE DOCUMENT : récupération, ajout, modification, suppression
        // =====================================================================

        /// <summary>
        /// Retourne les commandes d'un livre ou DVD triées par date décroissante
        /// </summary>
        /// <param name="idLivreDvd">identifiant du livre ou DVD</param>
        /// <returns>Liste d'objets CommandeDocument</returns>
        public List<CommandeDocument> GetCommandesLivreDvd(string idLivreDvd)
        {
            string jsonFiltre = convertToJson("idLivreDvd", idLivreDvd);
            return TraitementRecup<CommandeDocument>(GET, "commandedocument/" + jsonFiltre, null);
        }

        /// <summary>
        /// Retourne toutes les étapes de suivi
        /// </summary>
        /// <returns>Liste d'objets Suivi</returns>
        public List<Suivi> GetAllSuivi()
        {
            return TraitementRecup<Suivi>(GET, "suivi", null);
        }

        /// <summary>
        /// Insère une commande de document (tables commande + commandedocument, étape initiale 00001)
        /// </summary>
        /// <param name="commande">la commande à insérer</param>
        /// <returns>true si l'insertion a réussi</returns>
        public bool AjouterCommandeDocument(CommandeDocument commande)
        {
            Dictionary<string, object> champs = new Dictionary<string, object>
            {
                { "idCommande",   commande.Id },
                { "dateCommande", commande.DateCommande.ToString("yyyy-MM-dd") },
                { "montant",      commande.Montant },
                { "nbExemplaire", commande.NbExemplaire },
                { "idLivreDvd",   commande.IdLivreDvd },
                { "idSuivi",      commande.IdSuivi }
            };
            string jsonChamps = JsonConvert.SerializeObject(champs);
            return TraitementAction(POST, "commandedocument", "champs=" + jsonChamps);
        }

        /// <summary>
        /// Modifie l'étape de suivi d'une commande de document
        /// </summary>
        /// <param name="id">identifiant de la commande</param>
        /// <param name="idSuivi">nouvel identifiant d'étape</param>
        /// <returns>true si la modification a réussi</returns>
        public bool ModifierEtapeSuivi(string id, string idSuivi)
        {
            Dictionary<string, object> champs = new Dictionary<string, object>
            {
                { "idSuivi", idSuivi }
            };
            string jsonChamps = JsonConvert.SerializeObject(champs);
            return TraitementAction(PUT, "commandedocument/" + id, "champs=" + jsonChamps);
        }

        /// <summary>
        /// Supprime une commande de document (tables commandedocument + commande)
        /// Refuse côté API si la commande est livrée ou réglée
        /// </summary>
        /// <param name="id">identifiant de la commande</param>
        /// <returns>true si la suppression a réussi</returns>
        public bool SupprimerCommandeDocument(string id)
        {
            string jsonFiltreId = convertToJson("id", id);
            return TraitementAction(DELETE, "commandedocument/" + jsonFiltreId, "");
        }

        // =====================================================================
        // EXEMPLAIRE : récupération enrichie, modification de l'état, suppression
        // =====================================================================

        /// <summary>
        /// Retourne les exemplaires d'un document (livre, DVD ou revue) avec le libellé de l'état
        /// </summary>
        /// <param name="idDocument">identifiant du document parent</param>
        /// <returns>Liste d'objets Exemplaire (avec LibelleEtat renseigné)</returns>
        public List<Exemplaire> GetExemplairesDocument(string idDocument)
        {
            string jsonFiltre = convertToJson("id", idDocument);
            return TraitementRecup<Exemplaire>(GET, "exemplaire/" + jsonFiltre, null);
        }

        /// <summary>
        /// Retourne tous les états disponibles
        /// </summary>
        /// <returns>Liste d'objets Etat</returns>
        public List<Etat> GetAllEtats()
        {
            return TraitementRecup<Etat>(GET, "etat", null);
        }

        /// <summary>
        /// Modifie l'état d'un exemplaire identifié par idDocument + numero
        /// </summary>
        /// <param name="idDocument">identifiant du document parent</param>
        /// <param name="numero">numéro de l'exemplaire</param>
        /// <param name="idEtat">nouvel état</param>
        /// <returns>true si la modification a réussi</returns>
        public bool ModifierEtatExemplaire(string idDocument, int numero, string idEtat)
        {
            Dictionary<string, object> champs = new Dictionary<string, object>
            {
                { "numero", numero },
                { "idEtat", idEtat }
            };
            string jsonChamps = JsonConvert.SerializeObject(champs);
            return TraitementAction(PUT, "exemplaire/" + idDocument, "champs=" + jsonChamps);
        }

        /// <summary>
        /// Supprime un exemplaire identifié par idDocument + numero
        /// </summary>
        /// <param name="idDocument">identifiant du document parent</param>
        /// <param name="numero">numéro de l'exemplaire</param>
        /// <returns>true si la suppression a réussi</returns>
        public bool SupprimerExemplaire(string idDocument, int numero)
        {
            Dictionary<string, object> filtre = new Dictionary<string, object>
            {
                { "id",     idDocument },
                { "numero", numero }
            };
            string jsonFiltre = JsonConvert.SerializeObject(filtre);
            return TraitementAction(DELETE, "exemplaire/" + jsonFiltre, "");
        }

        // =====================================================================
        // ABONNEMENT : récupération, ajout, suppression
        // =====================================================================

        /// <summary>
        /// Retourne les abonnements d'une revue triés par date décroissante
        /// </summary>
        /// <param name="idRevue">identifiant de la revue</param>
        /// <returns>Liste d'objets Abonnement</returns>
        public List<Abonnement> GetAbonnementsRevue(string idRevue)
        {
            string jsonFiltre = convertToJson("idRevue", idRevue);
            return TraitementRecup<Abonnement>(GET, "abonnement/" + jsonFiltre, null);
        }

        /// <summary>
        /// Retourne les abonnements expirant dans les 30 prochains jours
        /// </summary>
        /// <returns>Liste d'objets AlerteAbonnement</returns>
        public List<AlerteAbonnement> GetAbonnementsExpirantBientot()
        {
            return TraitementRecup<AlerteAbonnement>(GET, "abonnementexpiration", null);
        }

        /// <summary>
        /// Insère un abonnement (tables commande + abonnement)
        /// </summary>
        /// <param name="abonnement">l'abonnement à insérer</param>
        /// <returns>true si l'insertion a réussi</returns>
        public bool AjouterAbonnement(Abonnement abonnement)
        {
            Dictionary<string, object> champs = new Dictionary<string, object>
            {
                { "id",                 abonnement.Id },
                { "dateCommande",       abonnement.DateCommande.ToString("yyyy-MM-dd") },
                { "montant",            abonnement.Montant },
                { "dateFinAbonnement",  abonnement.DateFinAbonnement.ToString("yyyy-MM-dd") },
                { "idRevue",            abonnement.IdRevue }
            };
            string jsonChamps = JsonConvert.SerializeObject(champs);
            return TraitementAction(POST, "abonnement", "champs=" + jsonChamps);
        }

        /// <summary>
        /// Supprime un abonnement (tables abonnement + commande)
        /// Refuse côté API si des exemplaires ont été reçus pendant la période
        /// </summary>
        /// <param name="id">identifiant de l'abonnement</param>
        /// <returns>true si la suppression a réussi</returns>
        public bool SupprimerAbonnement(string id)
        {
            string jsonFiltreId = convertToJson("id", id);
            return TraitementAction(DELETE, "abonnement/" + jsonFiltreId, "");
        }

        // =====================================================================
        // UTILISATEUR : connexion
        // =====================================================================

        /// <summary>
        /// Vérifie les identifiants et retourne l'utilisateur connecté, ou null si incorrects
        /// </summary>
        /// <param name="login">identifiant de connexion</param>
        /// <param name="pwd">mot de passe en clair</param>
        /// <returns>Objet Utilisateur si authentification réussie, null sinon</returns>
        public Utilisateur GetUtilisateur(string login, string pwd)
        {
            Dictionary<string, object> filtre = new Dictionary<string, object>
            {
                { "login", login },
                { "pwd",   pwd   }
            };
            string jsonFiltre = JsonConvert.SerializeObject(filtre);
            List<Utilisateur> liste = TraitementRecup<Utilisateur>(GET, "utilisateur/" + jsonFiltre, null);
            return liste != null && liste.Count > 0 ? liste[0] : null;
        }

        // =====================================================================
        // Méthodes privées
        // =====================================================================

        /// <summary>
        /// Traitement de la récupération du retour de l'api, avec conversion du json en liste pour les select (GET)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methode">verbe HTTP (GET, POST, PUT, DELETE)</param>
        /// <param name="message">information envoyée dans l'url</param>
        /// <param name="parametres">paramètres à envoyer dans le body, au format "chp1=val1&chp2=val2&..."</param>
        /// <returns>liste d'objets récupérés (ou liste vide)</returns>
        private List<T> TraitementRecup<T> (String methode, String message, String parametres)
        {
            List<T> liste = new List<T>();
            try
            {
                JObject retour = api.RecupDistant(methode, message, parametres);
                String code = (String)retour["code"];
                if (code.Equals("200"))
                {
                    if (methode.Equals(GET))
                    {
                        String resultString = JsonConvert.SerializeObject(retour["result"]);
                        liste = JsonConvert.DeserializeObject<List<T>>(resultString, new CustomBooleanJsonConverter());
                    }
                }
                else
                {
                    Console.WriteLine("code erreur = " + code + " message = " + (String)retour["message"]);
                }
            }catch(Exception e)
            {
                Console.WriteLine("Erreur lors de l'accès à l'API : "+e.Message);
                Environment.Exit(0);
            }
            return liste;
        }

        /// <summary>
        /// Traitement d'une action (POST, PUT, DELETE) : vérifie le code retour "200"
        /// </summary>
        /// <param name="methode">verbe HTTP</param>
        /// <param name="message">endpoint dans l'url</param>
        /// <param name="parametres">body de la requête</param>
        /// <returns>true si code retour est "200", false sinon</returns>
        private bool TraitementAction(string methode, string message, string parametres)
        {
            try
            {
                JObject retour = api.RecupDistant(methode, message, parametres);
                string code = (string)retour["code"];
                if (!code.Equals("200"))
                {
                    Console.WriteLine("code erreur = " + code + " message = " + (string)retour["message"]);
                }
                return code.Equals("200");
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur lors de l'accès à l'API : " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Convertit en json un couple nom/valeur
        /// </summary>
        /// <param name="nom"></param>
        /// <param name="valeur"></param>
        /// <returns>couple au format json</returns>
        private String convertToJson(Object nom, Object valeur)
        {
            Dictionary<Object, Object> dictionary = new Dictionary<Object, Object>();
            dictionary.Add(nom, valeur);
            return JsonConvert.SerializeObject(dictionary);
        }

        /// <summary>
        /// Modification du convertisseur Json pour gérer le format de date
        /// </summary>
        private sealed class CustomDateTimeConverter : IsoDateTimeConverter
        {
            public CustomDateTimeConverter()
            {
                base.DateTimeFormat = "yyyy-MM-dd";
            }
        }

        /// <summary>
        /// Modification du convertisseur Json pour prendre en compte les booléens
        /// classe trouvée sur le site :
        /// https://www.thecodebuzz.com/newtonsoft-jsonreaderexception-could-not-convert-string-to-boolean/
        /// </summary>
        private sealed class CustomBooleanJsonConverter : JsonConverter<bool>
        {
            public override bool ReadJson(JsonReader reader, Type objectType, bool existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                return Convert.ToBoolean(reader.ValueType == typeof(string) ? Convert.ToByte(reader.Value) : reader.Value);
            }

            public override void WriteJson(JsonWriter writer, bool value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value);
            }
        }

    }
}
