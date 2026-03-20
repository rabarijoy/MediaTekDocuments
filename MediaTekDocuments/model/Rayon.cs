
namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier représentant le rayon de bibliothèque auquel appartient un document
    /// (ex. : "Littérature", "Sciences"). Hérite de Categorie.
    /// </summary>
    public class Rayon : Categorie
    {
        /// <summary>
        /// Initialise un rayon avec son identifiant et son libellé.
        /// </summary>
        /// <param name="id">Identifiant unique du rayon.</param>
        /// <param name="libelle">Libellé du rayon (ex. : "Littérature", "Cinéma").</param>
        public Rayon(string id, string libelle) : base(id, libelle)
        {
        }

    }
}
