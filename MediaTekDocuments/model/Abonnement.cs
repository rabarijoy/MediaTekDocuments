using System;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe représentant un abonnement à une revue
    /// </summary>
    public class Abonnement
    {
        public string Id { get; set; }
        public DateTime DateCommande { get; set; }
        public double Montant { get; set; }
        public DateTime DateFinAbonnement { get; set; }
        public string IdRevue { get; set; }

        public Abonnement() { }

        public Abonnement(string id, DateTime dateCommande, double montant,
            DateTime dateFinAbonnement, string idRevue)
        {
            Id = id;
            DateCommande = dateCommande;
            Montant = montant;
            DateFinAbonnement = dateFinAbonnement;
            IdRevue = idRevue;
        }
    }
}
