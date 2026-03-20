
namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier représentant le public cible d'un document (ex. : "Adulte", "Jeunesse").
    /// Hérite de Categorie.
    /// </summary>
    public class Public : Categorie
    {
        /// <summary>
        /// Initialise un public cible avec son identifiant et son libellé.
        /// </summary>
        /// <param name="id">Identifiant unique du public.</param>
        /// <param name="libelle">Libellé du public (ex. : "Adulte", "Tout public").</param>
        public Public(string id, string libelle) : base(id, libelle)
        {
        }

    }
}
