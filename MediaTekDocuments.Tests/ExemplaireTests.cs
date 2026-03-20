using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;

namespace MediaTekDocuments.Tests
{
    [TestClass]
    public class ExemplaireTests
    {
        private static readonly DateTime dateAchat = new DateTime(2023, 6, 15, 0, 0, 0, DateTimeKind.Local);

        [TestMethod]
        public void Exemplaire_Constructeur_InitialiseCorrectement()
        {
            Exemplaire exemplaire = new Exemplaire(1, dateAchat, "photo.jpg", "E001", "L001");

            Assert.AreEqual(1, exemplaire.Numero);
            Assert.AreEqual(dateAchat, exemplaire.DateAchat);
            Assert.AreEqual("photo.jpg", exemplaire.Photo);
            Assert.AreEqual("E001", exemplaire.IdEtat);
            Assert.AreEqual("L001", exemplaire.Id);
        }

        [TestMethod]
        public void Exemplaire_Proprietes_SontMutables()
        {
            Exemplaire exemplaire = new Exemplaire(1, dateAchat, "photo.jpg", "E001", "L001");

            exemplaire.Numero   = 2;
            exemplaire.Photo    = "nouvelle_photo.jpg";
            exemplaire.IdEtat   = "E002";
            exemplaire.Id       = "L002";
            exemplaire.LibelleEtat = "Neuf";

            Assert.AreEqual(2, exemplaire.Numero);
            Assert.AreEqual("nouvelle_photo.jpg", exemplaire.Photo);
            Assert.AreEqual("E002", exemplaire.IdEtat);
            Assert.AreEqual("L002", exemplaire.Id);
            Assert.AreEqual("Neuf", exemplaire.LibelleEtat);
        }

        [TestMethod]
        public void Exemplaire_ConstructeurSansParametre_CreeSansErreur()
        {
            Exemplaire exemplaire = new Exemplaire();
            Assert.IsNotNull(exemplaire);
        }

        [TestMethod]
        public void Etat_Constructeur_InitialiseCorrectement()
        {
            Etat etat = new Etat("E001", "Neuf");

            Assert.AreEqual("E001", etat.Id);
            Assert.AreEqual("Neuf", etat.Libelle);
        }

        [TestMethod]
        public void Etat_Proprietes_SontMutables()
        {
            Etat etat = new Etat("E001", "Neuf");

            etat.Id      = "E002";
            etat.Libelle = "Usagé";

            Assert.AreEqual("E002", etat.Id);
            Assert.AreEqual("Usagé", etat.Libelle);
        }
    }
}
