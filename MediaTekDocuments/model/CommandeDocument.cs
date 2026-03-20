using System;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier représentant une commande d'exemplaires pour un livre ou un DVD.
    /// Une commande passe par plusieurs étapes de suivi (en cours, relancée, livrée, réglée).
    /// </summary>
    public class CommandeDocument
    {
        /// <summary>Identifiant unique de la commande (format "Cxxxx").</summary>
        public string Id { get; set; }

        /// <summary>Date à laquelle la commande a été passée.</summary>
        public DateTime DateCommande { get; set; }

        /// <summary>Montant total de la commande en euros.</summary>
        public double Montant { get; set; }

        /// <summary>Nombre d'exemplaires commandés.</summary>
        public int NbExemplaire { get; set; }

        /// <summary>Identifiant de l'étape de suivi actuelle.</summary>
        public string IdSuivi { get; set; }

        /// <summary>Libellé de l'étape de suivi actuelle, renseigné par jointure côté API.</summary>
        public string LibelleEtape { get; set; }

        /// <summary>Identifiant du livre ou DVD concerné par la commande.</summary>
        public string IdLivreDvd { get; set; }

        /// <summary>
        /// Constructeur sans paramètre requis par Newtonsoft.Json pour la désérialisation.
        /// </summary>
        public CommandeDocument() { }

        /// <summary>
        /// Initialise une commande de document avec toutes ses propriétés.
        /// </summary>
        /// <param name="id">Identifiant unique de la commande.</param>
        /// <param name="dateCommande">Date de la commande.</param>
        /// <param name="montant">Montant en euros.</param>
        /// <param name="nbExemplaire">Nombre d'exemplaires commandés.</param>
        /// <param name="idSuivi">Identifiant de l'étape de suivi.</param>
        /// <param name="libelleEtape">Libellé de l'étape de suivi.</param>
        /// <param name="idLivreDvd">Identifiant du document commandé.</param>
        public CommandeDocument(string id, DateTime dateCommande, double montant, int nbExemplaire,
            string idSuivi, string libelleEtape, string idLivreDvd)
        {
            Id = id;
            DateCommande = dateCommande;
            Montant = montant;
            NbExemplaire = nbExemplaire;
            IdSuivi = idSuivi;
            LibelleEtape = libelleEtape;
            IdLivreDvd = idLivreDvd;
        }
    }
}
