
namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier représentant une revue (périodique) de la médiathèque.
    /// Hérite de Document et ajoute les propriétés spécifiques : périodicité et délai de mise à disposition.
    /// </summary>
    public class Revue : Document
    {
        /// <summary>Périodicité de la revue (ex. : "Mensuelle", "Hebdomadaire").</summary>
        public string Periodicite { get; set; }

        /// <summary>Délai en jours entre la parution et la mise à disposition en rayon.</summary>
        public int DelaiMiseADispo { get; set; }

        /// <summary>
        /// Initialise une revue avec toutes ses propriétés.
        /// </summary>
        /// <param name="id">Identifiant unique de la revue.</param>
        /// <param name="titre">Titre de la revue.</param>
        /// <param name="image">Nom ou chemin du fichier image de couverture.</param>
        /// <param name="idGenre">Identifiant du genre.</param>
        /// <param name="genre">Libellé du genre.</param>
        /// <param name="idPublic">Identifiant du public cible.</param>
        /// <param name="lePublic">Libellé du public cible.</param>
        /// <param name="idRayon">Identifiant du rayon.</param>
        /// <param name="rayon">Libellé du rayon.</param>
        /// <param name="periodicite">Périodicité de la revue.</param>
        /// <param name="delaiMiseADispo">Délai de mise à disposition en jours.</param>
        public Revue(string id, string titre, string image, string idGenre, string genre,
            string idPublic, string lePublic, string idRayon, string rayon,
            string periodicite, int delaiMiseADispo)
             : base(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon)
        {
            Periodicite = periodicite;
            DelaiMiseADispo = delaiMiseADispo;
        }

    }
}
