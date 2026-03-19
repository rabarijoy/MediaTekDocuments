using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;

namespace MediaTekDocuments.Tests
{
    [TestClass]
    public class UtilitairesAbonnementTests
    {
        private static readonly DateTime dateDebut = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Local);
        private static readonly DateTime dateFin   = new DateTime(2024, 12, 31, 0, 0, 0, DateTimeKind.Local);

        [TestMethod]
        public void ParutionDansAbonnement_DateDansLaPeriode_RetourneVrai()
        {
            DateTime dateParution = new DateTime(2024, 6, 15, 0, 0, 0, DateTimeKind.Local);
            bool result = UtilitairesAbonnement.ParutionDansAbonnement(dateDebut, dateFin, dateParution);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ParutionDansAbonnement_DateAvantDebut_RetourneFaux()
        {
            DateTime dateParution = new DateTime(2023, 12, 31, 0, 0, 0, DateTimeKind.Local);
            bool result = UtilitairesAbonnement.ParutionDansAbonnement(dateDebut, dateFin, dateParution);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ParutionDansAbonnement_DateApresLaFin_RetourneFaux()
        {
            DateTime dateParution = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Local);
            bool result = UtilitairesAbonnement.ParutionDansAbonnement(dateDebut, dateFin, dateParution);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ParutionDansAbonnement_DateEgaleAuDebut_RetourneVrai()
        {
            bool result = UtilitairesAbonnement.ParutionDansAbonnement(dateDebut, dateFin, dateDebut);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ParutionDansAbonnement_DateEgaleALaFin_RetourneVrai()
        {
            bool result = UtilitairesAbonnement.ParutionDansAbonnement(dateDebut, dateFin, dateFin);
            Assert.IsTrue(result);
        }
    }
}
