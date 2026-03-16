using System;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier CommandeDocument : représente une commande de document (livre ou DVD)
    /// </summary>
    public class CommandeDocument
    {
        public string Id { get; set; }
        public DateTime DateCommande { get; set; }
        public double Montant { get; set; }
        public int NbExemplaire { get; set; }
        public string IdSuivi { get; set; }
        public string LibelleEtape { get; set; }
        public string IdLivreDvd { get; set; }

        public CommandeDocument() { }

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
