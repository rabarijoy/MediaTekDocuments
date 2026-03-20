
namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier représentant l'état d'usure d'un exemplaire
    /// (ex. : "Neuf", "Bon état", "Détérioré").
    /// </summary>
    public class Etat
    {
        /// <summary>Identifiant unique de l'état.</summary>
        public string Id { get; set; }

        /// <summary>Libellé de l'état affiché à l'utilisateur.</summary>
        public string Libelle { get; set; }

        /// <summary>
        /// Initialise un état avec son identifiant et son libellé.
        /// </summary>
        /// <param name="id">Identifiant unique de l'état.</param>
        /// <param name="libelle">Libellé de l'état (ex. : "Neuf").</param>
        public Etat(string id, string libelle)
        {
            this.Id = id;
            this.Libelle = libelle;
        }

    }
}
