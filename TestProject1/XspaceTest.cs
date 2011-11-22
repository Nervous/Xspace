using Xspace;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using ProjectMercury;

namespace TestProject1
{
    
    
    /// <summary>
    ///Classe de test pour XspaceTest, destinée à contenir tous
    ///les tests unitaires XspaceTest
    ///</summary>
    [TestClass()]
    public class XspaceTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Obtient ou définit le contexte de test qui fournit
        ///des informations sur la série de tests active ainsi que ses fonctionnalités.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Attributs de tests supplémentaires
        // 
        //Vous pouvez utiliser les attributs supplémentaires suivants lorsque vous écrivez vos tests :
        //
        //Utilisez ClassInitialize pour exécuter du code avant d'exécuter le premier test dans la classe
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Utilisez ClassCleanup pour exécuter du code après que tous les tests ont été exécutés dans une classe
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Utilisez TestInitialize pour exécuter du code avant d'exécuter chaque test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Utilisez TestCleanup pour exécuter du code après que chaque test a été exécuté
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///Test pour collisions
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Xspace.exe")]
        public void collisionsTest()
        {
            Xspace_Accessor target = new Xspace_Accessor(); // TODO: initialisez à une valeur appropriée
            List<Vaisseau_ennemi_Accessor> listeVaisseau = null; // TODO: initialisez à une valeur appropriée
            List<Missiles_Accessor[]> listeMissiles = null; // TODO: initialisez à une valeur appropriée
            ParticleEffect particule = null; // TODO: initialisez à une valeur appropriée
            bool expected = false; // TODO: initialisez à une valeur appropriée
            bool actual;
            actual = target.collisions(listeVaisseau, listeMissiles, particule);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Vérifiez l\'exactitude de cette méthode de test.");
        }
    }
}
