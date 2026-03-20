
namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier représentant le genre littéraire ou cinématographique d'un document.
    /// Hérite de Categorie.
    /// </summary>
    public class Genre : Categorie
    {
        /// <summary>
        /// Initialise un genre avec son identifiant et son libellé.
        /// </summary>
        /// <param name="id">Identifiant unique du genre.</param>
        /// <param name="libelle">Libellé du genre (ex. : "Roman", "Science-Fiction").</param>
        public Genre(string id, string libelle) : base(id, libelle)
        {
        }

    }
}
