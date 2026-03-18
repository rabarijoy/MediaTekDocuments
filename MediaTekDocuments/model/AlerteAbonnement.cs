using System;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe représentant une alerte d'abonnement expirant bientôt
    /// </summary>
    public class AlerteAbonnement
    {
        public string IdRevue { get; set; }
        public string TitreRevue { get; set; }
        public DateTime DateFinAbonnement { get; set; }

        public AlerteAbonnement() { }

        public AlerteAbonnement(string idRevue, string titreRevue, DateTime dateFinAbonnement)
        {
            IdRevue = idRevue;
            TitreRevue = titreRevue;
            DateFinAbonnement = dateFinAbonnement;
        }
    }
}
