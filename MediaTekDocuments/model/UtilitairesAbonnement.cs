using System;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe utilitaire pour les opérations sur les abonnements
    /// </summary>
    public static class UtilitairesAbonnement
    {
        /// <summary>
        /// Retourne vrai si dateParution est comprise entre dateDebut et dateFin (bornes incluses).
        /// </summary>
        /// <param name="dateDebut">date de début de l'abonnement (dateCommande)</param>
        /// <param name="dateFin">date de fin de l'abonnement</param>
        /// <param name="dateParution">date de parution de l'exemplaire</param>
        /// <returns>true si dateParution est dans la période [dateDebut, dateFin]</returns>
        public static bool ParutionDansAbonnement(DateTime dateDebut, DateTime dateFin, DateTime dateParution)
        {
            return dateParution >= dateDebut && dateParution <= dateFin;
        }
    }
}
