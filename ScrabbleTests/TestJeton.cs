using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace A2_POO_Scrabble
{
    [TestClass]
    public class TestJeton
    {
        [TestMethod]
        public void TestToString()
        {
            Jeton a = new Jeton('*', 0);
            Jeton b = new Jeton('E', 1);
            Jeton c = new Jeton('Z', 10);

            Assert.AreEqual("Jeton '*' (0 point)", a.ToString());
            Assert.AreEqual("Jeton 'E' (1 point)", b.ToString());
            Assert.AreEqual("Jeton 'Z' (10 points)", c.ToString());
        }

        [TestMethod]
        public void TestEqualityOperator()
        {
            Jeton e = new Jeton('E', 1);
            Jeton e2 = new Jeton('E', 2);
            Jeton z = new Jeton('Z', 10);

            Assert.IsTrue(e == e);
            Assert.IsFalse(e == z);

            Assert.IsTrue(e == e2); // seule la lettre compte
            Assert.IsFalse(z == null);

            Assert.IsTrue(z == 'Z');
            Assert.IsFalse(z == 'A');
        }

        [TestMethod]
        public void TestInequalityOperator()
        {
            Jeton e = new Jeton('E', 1);
            Jeton e2 = new Jeton('E', 2);
            Jeton z = new Jeton('Z', 10);

            Assert.IsFalse(e != e);
            Assert.IsTrue(e != z);

            Assert.IsFalse(e != e2); // seule la lettre compte
            Assert.IsTrue(z != null);

            Assert.IsFalse(z != 'Z');
            Assert.IsTrue(z != 'A');
        }

        [TestMethod]
        public void TestEquals()
        {
            Jeton e = new Jeton('E', 1);
            Jeton e2 = new Jeton('E', 2);
            Jeton z = new Jeton('Z', 10);

            Assert.IsTrue(e.Equals(e));
            Assert.IsFalse(e.Equals(z));

            Assert.IsTrue(e.Equals(e2)); // seule la lettre compte
            Assert.IsFalse(e.Equals(null));

            Assert.IsFalse(e.Equals("salut"));
            Assert.IsFalse(e.Equals(10));
            Assert.IsTrue(z.Equals('Z'));
            Assert.IsFalse(z.Equals('A'));
        }
    }
}
