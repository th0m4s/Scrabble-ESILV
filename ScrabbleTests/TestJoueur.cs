using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace A2_POO_Scrabble
{
    [TestClass]
    public class TestJoueur
    {
        [TestMethod]
        public void TestToString()
        {
            Joueur joueur = new Joueur("Thomas");
            Assert.AreEqual("Thomas : 0 points\nMots trouvés :\n   \nJetons courants :\n", joueur.ToString());
        }

        [TestMethod]
        public void TestProperties()
        {
            Joueur joueur = new Joueur("Thomas");

            Assert.AreEqual(0, joueur.Tours);
            joueur.Tours = 5;
            Assert.AreEqual(5, joueur.Tours);

            Assert.AreEqual("Thomas", joueur.Nom);
        }

        [TestMethod]
        public void TestScore()
        {
            Joueur joueur = new Joueur("Thomas");

            Assert.AreEqual(0, joueur.Score);

            joueur.Add_Score(10);
            Assert.AreEqual(10, joueur.Score);

            joueur.Add_Score(-5);
            Assert.AreEqual(10, joueur.Score);
        }

        [TestMethod]
        public void TestJetons()
        {
            Joueur joueur = new Joueur("Thomas");

            Assert.AreEqual(0, joueur.Nombre_Jetons());
            joueur.Add_Main_Courante(new Jeton('*', 0));

            joueur.Add_Main_Courante(new Jeton('A', 1));
            joueur.Add_Main_Courante(new Jeton('A', 1));
            Assert.AreEqual(2, joueur.Nombre_Lettre('A'));
            Assert.AreEqual(3, joueur.Nombre_Jetons());

            joueur.Remove_Main_Courante('A');
            Assert.AreEqual(2, joueur.Nombre_Jetons());
            Assert.AreEqual(1, joueur.Nombre_Lettre('A'));
        }
    }
}
