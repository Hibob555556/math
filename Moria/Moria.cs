using ArkenMath;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Moria
{
    [TestClass]
    public class BigIntegerTests
    {
        #region Constructor Tests

        [TestMethod]
        public void Constructor_Int_Positive()
        {
            var bi = new BigInteger(123);
            Assert.AreEqual("123", bi.ToString());
            Assert.AreEqual(1, bi.Sign);
        }

        [TestMethod]
        public void Constructor_Int_Negative()
        {
            var bi = new BigInteger(-456);
            Assert.AreEqual("-456", bi.ToString());
            Assert.AreEqual(-1, bi.Sign);
        }

        [TestMethod]
        public void Constructor_Long_LargeValue()
        {
            var bi = new BigInteger(9876543210L);
            Assert.AreEqual("9876543210", bi.ToString());
        }

        [TestMethod]
        public void Constructor_UInt_Implicit()
        {
            BigInteger bi = (uint)12345;
            Assert.AreEqual("12345", bi.ToString());
            Assert.AreEqual(1, bi.Sign);
        }

        [TestMethod]
        public void Constructor_ULong_Implicit()
        {
            BigInteger bi = (ulong)9876543210;
            Assert.AreEqual("9876543210", bi.ToString());
        }

        [TestMethod]
        public void Constructor_String_Positive()
        {
            var bi = new BigInteger("789");
            Assert.AreEqual("789", bi.ToString());
        }

        [TestMethod]
        public void Constructor_String_Negative()
        {
            var bi = new BigInteger("-321");
            Assert.AreEqual("-321", bi.ToString());
        }

        #endregion

        #region Addition/Subtraction Tests

        [TestMethod]
        public void Add_PositiveNumbers()
        {
            var a = new BigInteger(5);
            var b = new BigInteger(3);
            var sum = a.Add(b);
            Assert.AreEqual("8", sum.ToString());
        }

        [TestMethod]
        public void Add_PositiveAndNegative()
        {
            var a = new BigInteger(5);
            var b = new BigInteger(-3);
            var sum = a.Add(b);
            Assert.AreEqual("2", sum.ToString());
        }

        [TestMethod]
        public void Add_NegativeNumbers()
        {
            var a = new BigInteger(-7);
            var b = new BigInteger(-2);
            var sum = a.Add(b);
            Assert.AreEqual("-9", sum.ToString());
        }

        [TestMethod]
        public void Subtract_BiggerMinusSmaller()
        {
            var a = new BigInteger(7);
            var result = a.Add(new BigInteger(-2));
            Assert.AreEqual("5", result.ToString());
        }

        [TestMethod]
        public void Subtract_SmallerMinusBigger()
        {
            var a = new BigInteger(2);
            var result = a.Add(new BigInteger(-7));
            Assert.AreEqual("-5", result.ToString());
        }

        #endregion

        #region CompareAbs Tests

        [TestMethod]
        public void CompareAbs_Greater()
        {
            var a = new BigInteger(10);
            var b = new BigInteger(5);
            Assert.AreEqual(1, BigInteger.CompareAbs(a, b));
        }

        [TestMethod]
        public void CompareAbs_Less()
        {
            var a = new BigInteger(3);
            var b = new BigInteger(7);
            Assert.AreEqual(-1, BigInteger.CompareAbs(a, b));
        }

        [TestMethod]
        public void CompareAbs_Equal()
        {
            var a = new BigInteger(8);
            var b = new BigInteger(8);
            Assert.AreEqual(0, BigInteger.CompareAbs(a, b));
        }

        #endregion

        #region Absolute Value / IsZero Tests

        [TestMethod]
        public void GetAbsoluteValue_Positive()
        {
            var a = new BigInteger(5);
            Assert.AreEqual("5", a.GetAbsoluteValue().ToString());
        }

        [TestMethod]
        public void GetAbsoluteValue_Negative()
        {
            var a = new BigInteger(-5);
            Assert.AreEqual("5", a.GetAbsoluteValue().ToString());
        }

        [TestMethod]
        public void IsZero_Zero()
        {
            var a = new BigInteger(0);
            Assert.IsTrue(BigInteger.IsZero(a));
        }

        [TestMethod]
        public void IsZero_NonZero()
        {
            var a = new BigInteger(42);
            Assert.IsFalse(BigInteger.IsZero(a));
        }

        #endregion

        #region Multi-Limb / Edge Case Tests

        [TestMethod]
        public void Add_LargeNumbers_MultipleLimbs()
        {
            ulong aVal = (ulong)uint.MaxValue + 1;
            ulong bVal = (ulong)uint.MaxValue + 2;

            BigInteger a = aVal;
            BigInteger b = bVal;

            BigInteger sum = a.Add(b);

            Assert.AreEqual("8589934593", sum.ToString());
        }

        [TestMethod]
        public void Add_LargeNumbers_WithCarryPropagation()
        {
            BigInteger a = new BigInteger(new uint[] { uint.MaxValue, uint.MaxValue }, 1);
            BigInteger b = new BigInteger(1);

            BigInteger sum = a.Add(b);
            BigInteger expected = new BigInteger(new uint[] { 0, 0, 1 }, 1);
            Assert.AreEqual(expected.ToString(), sum.ToString());
        }

        [TestMethod]
        public void Subtract_LargeNumbers_MultipleLimbs()
        {
            BigInteger a = new BigInteger(new uint[] { 0, 2 }, 1);
            BigInteger result = a.Add(new BigInteger(-1));
            Assert.AreEqual("8589934591", result.ToString());
        }

        [TestMethod]
        public void Add_PositiveAndNegative_LargeNumbers()
        {
            BigInteger a = new BigInteger(new uint[] { 0, 2 }, 1);
            BigInteger b = new BigInteger(new uint[] { 1 }, -1);

            BigInteger result = a.Add(b);
            Assert.AreEqual("8589934591", result.ToString());
        }

        [TestMethod]
        public void Add_NegativeNumbers_WithCarryPropagation()
        {
            BigInteger a = new BigInteger(new uint[] { 0, 2 }, -1);
            BigInteger b = new BigInteger(new uint[] { 1 }, -1);

            BigInteger result = a.Add(b);
            Assert.AreEqual("-8589934593", result.ToString());
        }

        [TestMethod]
        public void ToString_LargeNumber()
        {
            BigInteger large = new BigInteger(new uint[] { uint.MaxValue, uint.MaxValue }, 1);
            Assert.AreEqual("18446744073709551615", large.ToString());
        }

        [TestMethod]
        public void GetAbsoluteValue_LargeNegativeNumber()
        {
            BigInteger a = new BigInteger(new uint[] { 1, 2 }, -1);
            BigInteger abs = a.GetAbsoluteValue();
            Assert.AreEqual("8589934593", abs.ToString());
            Assert.AreEqual(1, abs.Sign);
        }

        #endregion
    }
}
