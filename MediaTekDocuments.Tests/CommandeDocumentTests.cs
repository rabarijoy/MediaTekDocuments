using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;

namespace MediaTekDocuments.Tests
{
    [TestClass]
    public class CommandeDocumentTests
    {
        private static readonly DateTime dateCommande = new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Local);

        [TestMethod]
        public void CommandeDocument_Constructeur_InitialiseCorrectement()
        {
            CommandeDocument commande = new CommandeDocument(
                "C0001", dateCommande, 49.99, 3, "00001", "En cours", "L001"
            );

            Assert.AreEqual("C0001", commande.Id);
            Assert.AreEqual(dateCommande, commande.DateCommande);
            Assert.AreEqual(49.99, commande.Montant, 0.001);
            Assert.AreEqual(3, commande.NbExemplaire);
            Assert.AreEqual("00001", commande.IdSuivi);
            Assert.AreEqual("En cours", commande.LibelleEtape);
            Assert.AreEqual("L001", commande.IdLivreDvd);
        }

        [TestMethod]
        public void CommandeDocument_ConstructeurSansParametre_CreeSansErreur()
        {
            CommandeDocument commande = new CommandeDocument();
            Assert.IsNotNull(commande);
        }

        [TestMethod]
        public void CommandeDocument_Proprietes_SontMutables()
        {
            CommandeDocument commande = new CommandeDocument(
                "C0001", dateCommande, 49.99, 3, "00001", "En cours", "L001"
            );

            commande.IdSuivi     = "00003";
            commande.LibelleEtape = "Livrée";
            commande.Montant     = 59.99;

            Assert.AreEqual("00003", commande.IdSuivi);
            Assert.AreEqual("Livrée", commande.LibelleEtape);
            Assert.AreEqual(59.99, commande.Montant, 0.001);
        }

        [TestMethod]
        public void Suivi_Constructeur_InitialiseCorrectement()
        {
            Suivi suivi = new Suivi("00001", "En cours");

            Assert.AreEqual("00001", suivi.Id);
            Assert.AreEqual("En cours", suivi.Libelle);
        }

        [TestMethod]
        public void Suivi_ToString_RetourneLibelle()
        {
            Suivi suivi = new Suivi("00003", "Livrée");
            Assert.AreEqual("Livrée", suivi.ToString());
        }
    }
}
