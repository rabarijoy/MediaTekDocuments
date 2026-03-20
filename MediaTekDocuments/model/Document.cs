
namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier abstraite regroupant les informations communes à tous les documents
    /// de la médiathèque : Livre, DVD et Revue.
    /// </summary>
    public class Document
    {
        /// <summary>Identifiant unique du document.</summary>
        public string Id { get; }

        /// <summary>Titre du document.</summary>
        public string Titre { get; }

        /// <summary>Chemin ou nom du fichier image de couverture.</summary>
        public string Image { get; }

        /// <summary>Identifiant du genre du document.</summary>
        public string IdGenre { get; }

        /// <summary>Libellé du genre du document.</summary>
        public string Genre { get; }

        /// <summary>Identifiant du public cible du document.</summary>
        public string IdPublic { get; }

        /// <summary>Libellé du public cible du document.</summary>
        public string Public { get; }

        /// <summary>Identifiant du rayon de classement du document.</summary>
        public string IdRayon { get; }

        /// <summary>Libellé du rayon de classement du document.</summary>
        public string Rayon { get; }

        /// <summary>
        /// Initialise un document avec toutes ses propriétés communes.
        /// </summary>
        /// <param name="id">Identifiant unique du document.</param>
        /// <param name="titre">Titre du document.</param>
        /// <param name="image">Nom ou chemin du fichier image de couverture.</param>
        /// <param name="idGenre">Identifiant du genre.</param>
        /// <param name="genre">Libellé du genre.</param>
        /// <param name="idPublic">Identifiant du public cible.</param>
        /// <param name="lePublic">Libellé du public cible.</param>
        /// <param name="idRayon">Identifiant du rayon.</param>
        /// <param name="rayon">Libellé du rayon.</param>
        public Document(string id, string titre, string image, string idGenre, string genre, string idPublic, string lePublic, string idRayon, string rayon) // NOSONAR : 9 paramètres imposés par le schéma BDD
        {
            Id = id;
            Titre = titre;
            Image = image;
            IdGenre = idGenre;
            Genre = genre;
            IdPublic = idPublic;
            Public = lePublic;
            IdRayon = idRayon;
            Rayon = rayon;
        }
    }
}
