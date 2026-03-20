using System;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier représentant un exemplaire physique d'un document (livre, DVD ou revue).
    /// Chaque exemplaire est identifié par la combinaison de l'id du document parent et de son numéro.
    /// </summary>
    public class Exemplaire
    {
        /// <summary>Numéro de l'exemplaire au sein du document parent.</summary>
        public int Numero { get; set; }

        /// <summary>Nom ou chemin de la photo de l'exemplaire.</summary>
        public string Photo { get; set; }

        /// <summary>Date d'achat de l'exemplaire.</summary>
        public DateTime DateAchat { get; set; }

        /// <summary>Identifiant de l'état de l'exemplaire (ex. : "E001" = Neuf).</summary>
        public string IdEtat { get; set; }

        /// <summary>Identifiant du document parent auquel appartient l'exemplaire.</summary>
        public string Id { get; set; }

        /// <summary>Libellé de l'état de l'exemplaire, renseigné par jointure côté API.</summary>
        public string LibelleEtat { get; set; }

        /// <summary>
        /// Constructeur sans paramètre requis par Newtonsoft.Json pour la désérialisation.
        /// </summary>
        public Exemplaire() { }

        /// <summary>
        /// Initialise un exemplaire avec toutes ses propriétés.
        /// </summary>
        /// <param name="numero">Numéro de l'exemplaire.</param>
        /// <param name="dateAchat">Date d'achat de l'exemplaire.</param>
        /// <param name="photo">Nom ou chemin de la photo.</param>
        /// <param name="idEtat">Identifiant de l'état initial.</param>
        /// <param name="idDocument">Identifiant du document parent.</param>
        public Exemplaire(int numero, DateTime dateAchat, string photo, string idEtat, string idDocument)
        {
            this.Numero = numero;
            this.DateAchat = dateAchat;
            this.Photo = photo;
            this.IdEtat = idEtat;
            this.Id = idDocument;
        }

    }
}
