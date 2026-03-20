
namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier abstraite regroupant les informations communes aux catégories de classification
    /// des documents : Genre, Public et Rayon.
    /// </summary>
    public class Categorie
    {
        /// <summary>Identifiant unique de la catégorie.</summary>
        public string Id { get; }

        /// <summary>Libellé affiché de la catégorie.</summary>
        public string Libelle { get; }

        /// <summary>
        /// Initialise une catégorie avec son identifiant et son libellé.
        /// </summary>
        /// <param name="id">Identifiant unique de la catégorie.</param>
        /// <param name="libelle">Libellé affiché de la catégorie.</param>
        public Categorie(string id, string libelle)
        {
            this.Id = id;
            this.Libelle = libelle;
        }

        /// <summary>
        /// Retourne le libellé de la catégorie, utilisé pour l'affichage dans les ComboBox.
        /// </summary>
        /// <returns>Le libellé de la catégorie.</returns>
        public override string ToString()
        {
            return this.Libelle;
        }

    }
}
