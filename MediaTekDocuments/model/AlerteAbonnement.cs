using System;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier représentant une alerte pour un abonnement expirant dans les 30 prochains jours.
    /// Utilisée au démarrage de l'application pour notifier les utilisateurs autorisés.
    /// </summary>
    public class AlerteAbonnement
    {
        /// <summary>Identifiant de la revue concernée par l'alerte.</summary>
        public string IdRevue { get; set; }

        /// <summary>Titre de la revue concernée.</summary>
        public string TitreRevue { get; set; }

        /// <summary>Date d'expiration de l'abonnement.</summary>
        public DateTime DateFinAbonnement { get; set; }

        /// <summary>
        /// Constructeur sans paramètre requis par Newtonsoft.Json pour la désérialisation.
        /// </summary>
        public AlerteAbonnement() { }

        /// <summary>
        /// Initialise une alerte d'abonnement avec les informations de la revue.
        /// </summary>
        /// <param name="idRevue">Identifiant de la revue.</param>
        /// <param name="titreRevue">Titre de la revue.</param>
        /// <param name="dateFinAbonnement">Date d'expiration de l'abonnement.</param>
        public AlerteAbonnement(string idRevue, string titreRevue, DateTime dateFinAbonnement)
        {
            IdRevue = idRevue;
            TitreRevue = titreRevue;
            DateFinAbonnement = dateFinAbonnement;
        }
    }
}
