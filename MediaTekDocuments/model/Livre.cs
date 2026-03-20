
namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier représentant un livre de la médiathèque.
    /// Hérite de LivreDvd et ajoute les propriétés spécifiques : ISBN, auteur et collection.
    /// </summary>
    public class Livre : LivreDvd
    {
        /// <summary>Numéro ISBN du livre.</summary>
        public string Isbn { get; }

        /// <summary>Nom de l'auteur du livre.</summary>
        public string Auteur { get; }

        /// <summary>Nom de la collection éditoriale du livre.</summary>
        public string Collection { get; }

        /// <summary>
        /// Initialise un livre avec toutes ses propriétés.
        /// </summary>
        /// <param name="id">Identifiant unique du livre.</param>
        /// <param name="titre">Titre du livre.</param>
        /// <param name="image">Nom ou chemin du fichier image de couverture.</param>
        /// <param name="isbn">Numéro ISBN.</param>
        /// <param name="auteur">Auteur du livre.</param>
        /// <param name="collection">Collection éditoriale.</param>
        /// <param name="idGenre">Identifiant du genre.</param>
        /// <param name="genre">Libellé du genre.</param>
        /// <param name="idPublic">Identifiant du public cible.</param>
        /// <param name="lePublic">Libellé du public cible.</param>
        /// <param name="idRayon">Identifiant du rayon.</param>
        /// <param name="rayon">Libellé du rayon.</param>
        public Livre(string id, string titre, string image, string isbn, string auteur, string collection,
            string idGenre, string genre, string idPublic, string lePublic, string idRayon, string rayon)
            : base(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon)
        {
            this.Isbn = isbn;
            this.Auteur = auteur;
            this.Collection = collection;
        }



    }
}
