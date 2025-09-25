using ArkenMath;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Moria
{
    [TestClass]
    public class BigIntegerTests
    {
        [TestMethod]
        public void Add_PositiveNumbers_ReturnsCorrectSum()
        {
            var a = new BigInteger(new uint[] { 5 }, 1); // +5
            var b = new BigInteger(new uint[] { 3 }, 1); // +3

            var result = a.Add(b);

            Assert.AreEqual("+[8]", result.ToString());
        }

        [TestMethod]
        public void Add_PositiveAndNegative_ReturnsCorrectResult()
        {
            var a = new BigInteger(new uint[] { 5 }, 1); // +5
            var b = new BigInteger(new uint[] { 3 }, -1); // -3

            var result = a.Add(b);

            Assert.AreEqual("+[2]", result.ToString());
        }

        [TestMethod]
        public void Subtract_BiggerMinusSmaller_ReturnsPositiveResult()
        {
            var a = new BigInteger(new uint[] { 7 }, 1); // +7
            var b = new BigInteger(new uint[] { 2 }, 1); // +2

            var result = a.Add(new BigInteger(new uint[] { 2 }, -1)); // triggers subtraction internally

            Assert.AreEqual("+[5]", result.ToString());
        }

        [TestMethod]
        public void Subtract_SmallerMinusBigger_ReturnsNegativeResult()
        {
            var a = new BigInteger(new uint[] { 2 }, 1); // +2
            var b = new BigInteger(new uint[] { 7 }, 1); // +7

            var result = a.Add(new BigInteger(new uint[] { 7 }, -1)); // triggers subtraction internally

            Assert.AreEqual("-[5]", result.ToString());
        }
    }
}
