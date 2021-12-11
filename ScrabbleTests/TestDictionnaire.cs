using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace A2_POO_Scrabble
{
    [TestClass]
    public class TestDictionnaire
    {
        [TestMethod]
        public void TestRechercheDichoRecursif()
        {
            Dictionnaire dico = new Dictionnaire("Francais", File.ReadAllLines("../../../../runtime/Francais.txt"));

            Assert.IsTrue(dico.RechercheDichoRecursif("SALUT"));
            Assert.IsTrue(dico.RechercheDichoRecursif("BONJOUR"));

            Assert.IsFalse(dico.RechercheDichoRecursif(""));
            Assert.IsFalse(dico.RechercheDichoRecursif("AZERTY"));
            Assert.IsFalse(dico.RechercheDichoRecursif("ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        }
    }
}
