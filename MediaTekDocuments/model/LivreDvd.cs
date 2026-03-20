
namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier abstraite intermédiaire regroupant les propriétés communes
    /// aux Livres et aux DVD (documents physiques pouvant faire l'objet de commandes).
    /// Hérite de Document.
    /// </summary>
    public abstract class LivreDvd : Document
    {
        /// <summary>
        /// Initialise un LivreDvd en déléguant toutes les propriétés communes à Document.
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
        protected LivreDvd(string id, string titre, string image, string idGenre, string genre,
            string idPublic, string lePublic, string idRayon, string rayon)
            : base(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon)
        {
        }

    }
}
