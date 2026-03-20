using System;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier représentant un abonnement à une revue.
    /// Un abonnement est à la fois une commande (table commande) et un enregistrement
    /// spécifique dans la table abonnement, avec une date de fin et une revue associée.
    /// </summary>
    public class Abonnement
    {
        /// <summary>Identifiant unique de l'abonnement (partagé avec la table commande).</summary>
        public string Id { get; set; }

        /// <summary>Date à laquelle l'abonnement a été commandé.</summary>
        public DateTime DateCommande { get; set; }

        /// <summary>Montant de l'abonnement en euros.</summary>
        public double Montant { get; set; }

        /// <summary>Date d'expiration de l'abonnement.</summary>
        public DateTime DateFinAbonnement { get; set; }

        /// <summary>Identifiant de la revue concernée par l'abonnement.</summary>
        public string IdRevue { get; set; }

        /// <summary>
        /// Constructeur sans paramètre requis par Newtonsoft.Json pour la désérialisation.
        /// </summary>
        public Abonnement() { }

        /// <summary>
        /// Initialise un abonnement avec toutes ses propriétés.
        /// </summary>
        /// <param name="id">Identifiant unique de l'abonnement.</param>
        /// <param name="dateCommande">Date de commande de l'abonnement.</param>
        /// <param name="montant">Montant en euros.</param>
        /// <param name="dateFinAbonnement">Date d'expiration de l'abonnement.</param>
        /// <param name="idRevue">Identifiant de la revue.</param>
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
