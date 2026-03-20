using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;

namespace MediaTekDocuments.Tests
{
    [TestClass]
    public class DocumentTests
    {
        [TestMethod]
        public void Livre_Constructeur_InitialiseCorrectement()
        {
            Livre livre = new Livre(
                "L001", "Le Petit Prince", "img.jpg",
                "978-2-07", "Antoine de Saint-Exupéry", "Folio",
                "G001", "Roman", "P001", "Adulte", "R001", "Littérature"
            );

            Assert.AreEqual("L001", livre.Id);
            Assert.AreEqual("Le Petit Prince", livre.Titre);
            Assert.AreEqual("img.jpg", livre.Image);
            Assert.AreEqual("978-2-07", livre.Isbn);
            Assert.AreEqual("Antoine de Saint-Exupéry", livre.Auteur);
            Assert.AreEqual("Folio", livre.Collection);
            Assert.AreEqual("G001", livre.IdGenre);
            Assert.AreEqual("Roman", livre.Genre);
            Assert.AreEqual("P001", livre.IdPublic);
            Assert.AreEqual("Adulte", livre.Public);
            Assert.AreEqual("R001", livre.IdRayon);
            Assert.AreEqual("Littérature", livre.Rayon);
        }

        [TestMethod]
        public void Livre_EstUnLivreDvd_EtUnDocument()
        {
            Livre livre = new Livre(
                "L001", "Le Petit Prince", "img.jpg",
                "978-2-07", "Antoine de Saint-Exupéry", "Folio",
                "G001", "Roman", "P001", "Adulte", "R001", "Littérature"
            );

            Assert.IsInstanceOfType(livre, typeof(LivreDvd));
            Assert.IsInstanceOfType(livre, typeof(Document));
        }

        [TestMethod]
        public void Dvd_Constructeur_InitialiseCorrectement()
        {
            Dvd dvd = new Dvd(
                "D001", "Inception", "inception.jpg",
                148, "Christopher Nolan", "Un voleur s'infiltre dans les rêves.",
                "G002", "Science-Fiction", "P002", "Tout public", "R002", "Cinéma"
            );

            Assert.AreEqual("D001", dvd.Id);
            Assert.AreEqual("Inception", dvd.Titre);
            Assert.AreEqual("inception.jpg", dvd.Image);
            Assert.AreEqual(148, dvd.Duree);
            Assert.AreEqual("Christopher Nolan", dvd.Realisateur);
            Assert.AreEqual("Un voleur s'infiltre dans les rêves.", dvd.Synopsis);
            Assert.AreEqual("G002", dvd.IdGenre);
            Assert.AreEqual("Science-Fiction", dvd.Genre);
            Assert.AreEqual("P002", dvd.IdPublic);
            Assert.AreEqual("Tout public", dvd.Public);
            Assert.AreEqual("R002", dvd.IdRayon);
            Assert.AreEqual("Cinéma", dvd.Rayon);
        }

        [TestMethod]
        public void Dvd_EstUnLivreDvd_EtUnDocument()
        {
            Dvd dvd = new Dvd(
                "D001", "Inception", "inception.jpg",
                148, "Christopher Nolan", "Synopsis.",
                "G002", "Science-Fiction", "P002", "Tout public", "R002", "Cinéma"
            );

            Assert.IsInstanceOfType(dvd, typeof(LivreDvd));
            Assert.IsInstanceOfType(dvd, typeof(Document));
        }

        [TestMethod]
        public void Revue_Constructeur_InitialiseCorrectement()
        {
            Revue revue = new Revue(
                "REV001", "Science & Vie", "sv.jpg",
                "G003", "Sciences", "P001", "Adulte", "R003", "Presse",
                "Mensuelle", 30
            );

            Assert.AreEqual("REV001", revue.Id);
            Assert.AreEqual("Science & Vie", revue.Titre);
            Assert.AreEqual("sv.jpg", revue.Image);
            Assert.AreEqual("Mensuelle", revue.Periodicite);
            Assert.AreEqual(30, revue.DelaiMiseADispo);
            Assert.AreEqual("G003", revue.IdGenre);
            Assert.AreEqual("P001", revue.IdPublic);
            Assert.AreEqual("R003", revue.IdRayon);
        }

        [TestMethod]
        public void Revue_EstUnDocument()
        {
            Revue revue = new Revue(
                "REV001", "Science & Vie", "sv.jpg",
                "G003", "Sciences", "P001", "Adulte", "R003", "Presse",
                "Mensuelle", 30
            );

            Assert.IsInstanceOfType(revue, typeof(Document));
        }
    }
}
