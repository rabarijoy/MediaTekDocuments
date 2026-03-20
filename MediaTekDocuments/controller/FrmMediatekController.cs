using System.Collections.Generic;
using MediaTekDocuments.model;
using MediaTekDocuments.dal;

namespace MediaTekDocuments.controller
{
    /// <summary>
    /// Contrôleur de la fenêtre principale FrmMediatek.
    /// Fait le lien entre la vue et la couche d'accès aux données (Access) en délégant
    /// toutes les opérations CRUD et métier sans contenir de logique propre.
    /// </summary>
    class FrmMediatekController
    {
        /// <summary>
        /// Objet d'accès aux données
        /// </summary>
        private readonly Access access;

        /// <summary>
        /// Récupération de l'instance unique d'accès aux données
        /// </summary>
        public FrmMediatekController()
        {
            access = Access.GetInstance();
        }

        /// <summary>
        /// getter sur la liste des genres
        /// </summary>
        /// <returns>Liste d'objets Genre</returns>
        public List<Categorie> GetAllGenres()
        {
            return access.GetAllGenres();
        }

        /// <summary>
        /// getter sur la liste des livres
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            return access.GetAllLivres();
        }

        /// <summary>
        /// getter sur la liste des Dvd
        /// </summary>
        /// <returns>Liste d'objets dvd</returns>
        public List<Dvd> GetAllDvd()
        {
            return access.GetAllDvd();
        }

        /// <summary>
        /// getter sur la liste des revues
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public List<Revue> GetAllRevues()
        {
            return access.GetAllRevues();
        }

        /// <summary>
        /// getter sur les rayons
        /// </summary>
        /// <returns>Liste d'objets Rayon</returns>
        public List<Categorie> GetAllRayons()
        {
            return access.GetAllRayons();
        }

        /// <summary>
        /// getter sur les publics
        /// </summary>
        /// <returns>Liste d'objets Public</returns>
        public List<Categorie> GetAllPublics()
        {
            return access.GetAllPublics();
        }

        /// <summary>
        /// récupère les exemplaires d'une revue
        /// </summary>
        /// <param name="idDocuement">id de la revue concernée</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesRevue(string idDocuement)
        {
            return access.GetExemplairesRevue(idDocuement);
        }

        /// <summary>
        /// Crée un exemplaire d'une revue dans la bdd
        /// </summary>
        /// <param name="exemplaire">L'objet Exemplaire concerné</param>
        /// <returns>True si la création a pu se faire</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            return access.CreerExemplaire(exemplaire);
        }

        // =====================================================================
        // LIVRE : ajout, modification, suppression
        // =====================================================================

        /// <summary>
        /// Ajoute un livre dans la base de données
        /// </summary>
        /// <param name="livre">le livre à ajouter</param>
        /// <returns>true si l'ajout a réussi</returns>
        public bool AjouterLivre(Livre livre)
        {
            return access.AjouterLivre(livre);
        }

        /// <summary>
        /// Modifie un livre dans la base de données
        /// </summary>
        /// <param name="livre">le livre modifié</param>
        /// <returns>true si la modification a réussi</returns>
        public bool ModifierLivre(Livre livre)
        {
            return access.ModifierLivre(livre);
        }

        /// <summary>
        /// Supprime un livre de la base de données
        /// </summary>
        /// <param name="id">identifiant du livre</param>
        /// <returns>true si la suppression a réussi</returns>
        public bool SupprimerLivre(string id)
        {
            return access.SupprimerLivre(id);
        }

        // =====================================================================
        // DVD : ajout, modification, suppression
        // =====================================================================

        /// <summary>
        /// Ajoute un dvd dans la base de données
        /// </summary>
        /// <param name="dvd">le dvd à ajouter</param>
        /// <returns>true si l'ajout a réussi</returns>
        public bool AjouterDvd(Dvd dvd)
        {
            return access.AjouterDvd(dvd);
        }

        /// <summary>
        /// Modifie un dvd dans la base de données
        /// </summary>
        /// <param name="dvd">le dvd modifié</param>
        /// <returns>true si la modification a réussi</returns>
        public bool ModifierDvd(Dvd dvd)
        {
            return access.ModifierDvd(dvd);
        }

        /// <summary>
        /// Supprime un dvd de la base de données
        /// </summary>
        /// <param name="id">identifiant du dvd</param>
        /// <returns>true si la suppression a réussi</returns>
        public bool SupprimerDvd(string id)
        {
            return access.SupprimerDvd(id);
        }

        // =====================================================================
        // REVUE : ajout, modification, suppression
        // =====================================================================

        /// <summary>
        /// Ajoute une revue dans la base de données
        /// </summary>
        /// <param name="revue">la revue à ajouter</param>
        /// <returns>true si l'ajout a réussi</returns>
        public bool AjouterRevue(Revue revue)
        {
            return access.AjouterRevue(revue);
        }

        /// <summary>
        /// Modifie une revue dans la base de données
        /// </summary>
        /// <param name="revue">la revue modifiée</param>
        /// <returns>true si la modification a réussi</returns>
        public bool ModifierRevue(Revue revue)
        {
            return access.ModifierRevue(revue);
        }

        /// <summary>
        /// Supprime une revue de la base de données
        /// </summary>
        /// <param name="id">identifiant de la revue</param>
        /// <returns>true si la suppression a réussi</returns>
        public bool SupprimerRevue(string id)
        {
            return access.SupprimerRevue(id);
        }

        // =====================================================================
        // COMMANDE DOCUMENT
        // =====================================================================

        /// <summary>
        /// Retourne les commandes associées à un livre ou DVD, triées par date décroissante.
        /// </summary>
        /// <param name="idLivreDvd">Identifiant du livre ou DVD.</param>
        /// <returns>Liste d'objets CommandeDocument.</returns>
        public List<CommandeDocument> GetCommandesLivreDvd(string idLivreDvd)
        {
            return access.GetCommandesLivreDvd(idLivreDvd);
        }

        /// <summary>
        /// Retourne toutes les étapes de suivi disponibles (En cours, Relancée, Livrée, Réglée).
        /// </summary>
        /// <returns>Liste d'objets Suivi.</returns>
        public List<Suivi> GetAllSuivi()
        {
            return access.GetAllSuivi();
        }

        /// <summary>
        /// Insère une nouvelle commande de document (livre ou DVD) en base de données.
        /// </summary>
        /// <param name="commande">La commande à insérer.</param>
        /// <returns>True si l'insertion a réussi.</returns>
        public bool AjouterCommandeDocument(CommandeDocument commande)
        {
            return access.AjouterCommandeDocument(commande);
        }

        /// <summary>
        /// Met à jour l'étape de suivi d'une commande de document.
        /// </summary>
        /// <param name="id">Identifiant de la commande à modifier.</param>
        /// <param name="idSuivi">Identifiant de la nouvelle étape de suivi.</param>
        /// <returns>True si la modification a réussi.</returns>
        public bool ModifierEtapeSuivi(string id, string idSuivi)
        {
            return access.ModifierEtapeSuivi(id, idSuivi);
        }

        /// <summary>
        /// Supprime une commande de document. Refusé côté API si la commande est livrée ou réglée.
        /// </summary>
        /// <param name="id">Identifiant de la commande à supprimer.</param>
        /// <returns>True si la suppression a réussi.</returns>
        public bool SupprimerCommandeDocument(string id)
        {
            return access.SupprimerCommandeDocument(id);
        }

        // =====================================================================
        // EXEMPLAIRE
        // =====================================================================

        /// <summary>
        /// Retourne les exemplaires d'un document (livre, DVD ou revue) avec le libellé de l'état,
        /// triés par date d'achat décroissante.
        /// </summary>
        /// <param name="idDocument">Identifiant du document parent.</param>
        /// <returns>Liste d'objets Exemplaire avec LibelleEtat renseigné.</returns>
        public List<Exemplaire> GetExemplairesDocument(string idDocument)
        {
            return access.GetExemplairesDocument(idDocument);
        }

        /// <summary>
        /// Retourne tous les états d'usure disponibles pour les exemplaires.
        /// </summary>
        /// <returns>Liste d'objets Etat.</returns>
        public List<Etat> GetAllEtats()
        {
            return access.GetAllEtats();
        }

        /// <summary>
        /// Met à jour l'état d'usure d'un exemplaire identifié par son document parent et son numéro.
        /// </summary>
        /// <param name="idDocument">Identifiant du document parent.</param>
        /// <param name="numero">Numéro de l'exemplaire.</param>
        /// <param name="idEtat">Identifiant du nouvel état.</param>
        /// <returns>True si la modification a réussi.</returns>
        public bool ModifierEtatExemplaire(string idDocument, int numero, string idEtat)
        {
            return access.ModifierEtatExemplaire(idDocument, numero, idEtat);
        }

        /// <summary>
        /// Supprime un exemplaire identifié par son document parent et son numéro.
        /// </summary>
        /// <param name="idDocument">Identifiant du document parent.</param>
        /// <param name="numero">Numéro de l'exemplaire à supprimer.</param>
        /// <returns>True si la suppression a réussi.</returns>
        public bool SupprimerExemplaire(string idDocument, int numero)
        {
            return access.SupprimerExemplaire(idDocument, numero);
        }

        // =====================================================================
        // ABONNEMENT
        // =====================================================================

        /// <summary>
        /// Retourne les abonnements associés à une revue, triés par date décroissante.
        /// </summary>
        /// <param name="idRevue">Identifiant de la revue.</param>
        /// <returns>Liste d'objets Abonnement.</returns>
        public List<Abonnement> GetAbonnementsRevue(string idRevue)
        {
            return access.GetAbonnementsRevue(idRevue);
        }

        /// <summary>
        /// Retourne les abonnements dont la date de fin est dans les 30 prochains jours.
        /// Utilisé au démarrage pour afficher les alertes aux utilisateurs autorisés.
        /// </summary>
        /// <returns>Liste d'objets AlerteAbonnement.</returns>
        public List<AlerteAbonnement> GetAbonnementsExpirantBientot()
        {
            return access.GetAbonnementsExpirantBientot();
        }

        /// <summary>
        /// Insère un nouvel abonnement (tables commande + abonnement) en base de données.
        /// </summary>
        /// <param name="abonnement">L'abonnement à insérer.</param>
        /// <returns>True si l'insertion a réussi.</returns>
        public bool AjouterAbonnement(Abonnement abonnement)
        {
            return access.AjouterAbonnement(abonnement);
        }

        /// <summary>
        /// Supprime un abonnement. Refusé côté API si des exemplaires ont été reçus pendant la période.
        /// </summary>
        /// <param name="id">Identifiant de l'abonnement à supprimer.</param>
        /// <returns>True si la suppression a réussi.</returns>
        public bool SupprimerAbonnement(string id)
        {
            return access.SupprimerAbonnement(id);
        }

        /// <summary>
        /// Vérifie les identifiants de connexion et retourne l'utilisateur authentifié,
        /// ou null si le login ou le mot de passe est incorrect.
        /// </summary>
        /// <param name="login">Identifiant de connexion saisi.</param>
        /// <param name="pwd">Mot de passe saisi en clair.</param>
        /// <returns>Objet Utilisateur si authentifié, null sinon.</returns>
        public Utilisateur GetUtilisateur(string login, string pwd)
        {
            return access.GetUtilisateur(login, pwd);
        }
    }
}
