using System.Collections.Generic;
using MediaTekDocuments.model;
using MediaTekDocuments.dal;

namespace MediaTekDocuments.controller
{
    /// <summary>
    /// Contrôleur lié à FrmMediatek
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
        /// Retourne les commandes d'un livre ou DVD
        /// </summary>
        public List<CommandeDocument> GetCommandesLivreDvd(string idLivreDvd)
        {
            return access.GetCommandesLivreDvd(idLivreDvd);
        }

        /// <summary>
        /// Retourne toutes les étapes de suivi
        /// </summary>
        public List<Suivi> GetAllSuivi()
        {
            return access.GetAllSuivi();
        }

        /// <summary>
        /// Ajoute une commande de document
        /// </summary>
        public bool AjouterCommandeDocument(CommandeDocument commande)
        {
            return access.AjouterCommandeDocument(commande);
        }

        /// <summary>
        /// Modifie l'étape de suivi d'une commande
        /// </summary>
        public bool ModifierEtapeSuivi(string id, string idSuivi)
        {
            return access.ModifierEtapeSuivi(id, idSuivi);
        }

        /// <summary>
        /// Supprime une commande de document
        /// </summary>
        public bool SupprimerCommandeDocument(string id)
        {
            return access.SupprimerCommandeDocument(id);
        }
    }
}
