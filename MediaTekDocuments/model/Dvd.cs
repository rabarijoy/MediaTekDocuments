
namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier représentant un DVD de la médiathèque.
    /// Hérite de LivreDvd et ajoute les propriétés spécifiques : durée, réalisateur et synopsis.
    /// </summary>
    public class Dvd : LivreDvd
    {
        /// <summary>Durée du DVD en minutes.</summary>
        public int Duree { get; }

        /// <summary>Nom du réalisateur du DVD.</summary>
        public string Realisateur { get; }

        /// <summary>Synopsis du DVD.</summary>
        public string Synopsis { get; }

        /// <summary>
        /// Initialise un DVD avec toutes ses propriétés.
        /// </summary>
        /// <param name="id">Identifiant unique du DVD.</param>
        /// <param name="titre">Titre du DVD.</param>
        /// <param name="image">Nom ou chemin du fichier image de couverture.</param>
        /// <param name="duree">Durée en minutes.</param>
        /// <param name="realisateur">Nom du réalisateur.</param>
        /// <param name="synopsis">Résumé du film.</param>
        /// <param name="idGenre">Identifiant du genre.</param>
        /// <param name="genre">Libellé du genre.</param>
        /// <param name="idPublic">Identifiant du public cible.</param>
        /// <param name="lePublic">Libellé du public cible.</param>
        /// <param name="idRayon">Identifiant du rayon.</param>
        /// <param name="rayon">Libellé du rayon.</param>
        public Dvd(string id, string titre, string image, int duree, string realisateur, string synopsis,
            string idGenre, string genre, string idPublic, string lePublic, string idRayon, string rayon)
            : base(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon)
        {
            this.Duree = duree;
            this.Realisateur = realisateur;
            this.Synopsis = synopsis;
        }

    }
}
