using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;

namespace MediaTekDocuments.Tests
{
    [TestClass]
    public class CategorieTests
    {
        [TestMethod]
        public void Constructeur_InitialiseCorrectement()
        {
            Categorie categorie = new Genre("G001", "Roman");
            Assert.AreEqual("G001", categorie.Id);
            Assert.AreEqual("Roman", categorie.Libelle);
        }

        [TestMethod]
        public void ToString_RetourneLibelle()
        {
            Categorie categorie = new Genre("G001", "Roman");
            Assert.AreEqual("Roman", categorie.ToString());
        }

        [TestMethod]
        public void Genre_HeriteDe_Categorie()
        {
            Genre genre = new Genre("G001", "Roman");
            Assert.IsInstanceOfType(genre, typeof(Categorie));
        }

        [TestMethod]
        public void Public_HeriteDe_Categorie()
        {
            Public lePublic = new Public("P001", "Adulte");
            Assert.IsInstanceOfType(lePublic, typeof(Categorie));
            Assert.AreEqual("P001", lePublic.Id);
            Assert.AreEqual("Adulte", lePublic.Libelle);
        }

        [TestMethod]
        public void Rayon_HeriteDe_Categorie()
        {
            Rayon rayon = new Rayon("R001", "Sciences");
            Assert.IsInstanceOfType(rayon, typeof(Categorie));
            Assert.AreEqual("R001", rayon.Id);
            Assert.AreEqual("Sciences", rayon.Libelle);
        }

        [TestMethod]
        public void Rayon_ToString_RetourneLibelle()
        {
            Rayon rayon = new Rayon("R001", "Sciences");
            Assert.AreEqual("Sciences", rayon.ToString());
        }
    }
}
