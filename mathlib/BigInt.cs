namespace ArkenMath
{
    public class BigInteger
    {
        private readonly uint[] _limbs;
        private readonly int _sign;

        public BigInteger(uint[] limbs, int sign)
        {
            _limbs = TrimLeadingZeros(limbs);
            _sign = _limbs.Length == 0 ? 0 : (sign >= 0 ? 1 : -1);
        }

        public int Sign => _sign;
        public uint[] Limbs => _limbs;

        public override string ToString()
        {
            // For now, just debug print
            return $"{(Sign < 0 ? "-" : "+")}[{string.Join(",", _limbs)}]";
        }

        private static uint[] AddLimbs(uint[] a, uint[] b)
        {
            int length = Math.Max(a.Length, b.Length);
            uint[] result = new uint[length + 1]; // +1 for carry overflow
            ulong carry = 0;

            for (int i = 0; i < length; i++)
            {
                ulong av = i < a.Length ? a[i] : 0;
                ulong bv = i < b.Length ? b[i] : 0;

                ulong sum = av + bv + carry;
                result[i] = (uint)(sum & 0xFFFFFFFF); // lower 32 bits
                carry = sum >> 32; // upper bits become carry
            }

            if (carry > 0)
                result[length] = (uint)carry;

            // Trim leading zeros if the top limb is 0
            return TrimLeadingZeros(result);
        }

        public BigInteger GetAbsoluteValue()
        {
            if (this.Sign >= 0)
                return this;
            else
                return new BigInteger(this.Limbs, 1);
        }

        private static uint[] TrimLeadingZeros(uint[] limbs)
        {
            int i = limbs.Length - 1;
            while (i >= 0 && limbs[i] == 0)
                i--;

            if (i < 0)
                return Array.Empty<uint>();

            uint[] result = new uint[i + 1];
            Array.Copy(limbs, result, i + 1);
            return result;
        }

        public static int CompareAbs(BigInteger a, BigInteger b)
        {
            // Check if |a| >= |b|
            if (a.Limbs.Length > b.Limbs.Length)
            {
                return 1;
            }
            // check if |a| < |b|
            else if (a.Limbs.Length < b.Limbs.Length)
            {
                return -1;
            }

            // same length, compare limb by limb
            for (int i = a.Limbs.Length - 1; i >= 0; i--)
            {
                if (a.Limbs[i] > b.Limbs[i])
                    return 1;
                else if (a.Limbs[i] < b.Limbs[i])
                    return -1;
            }

            return 0; // equal
        }

        public static bool IsAllZero(BigInteger a)
        {
            foreach (var limb in a.Limbs)
            {
                if (limb != 0) return false;
            }
            return true;
        }

        private static uint[] SubtractLimbs(uint[] bigger, uint[] smaller)
        {
            int length = bigger.Length;
            uint[] result = new uint[length];
            long borrow = 0;

            for (int i = 0; i < length; i++)
            {
                long bi = bigger[i];
                long si = i < smaller.Length ? smaller[i] : 0;

                long diff = bi - si - borrow;
                if (diff < 0)
                {
                    borrow = 1;
                    diff += 0x100000000; // 2^32
                }
                else
                {
                    borrow = 0;
                }

                result[i] = (uint)diff;
            }

            return result;
        }


        private static BigInteger SubtractInternal(BigInteger a, BigInteger b)
        {
            BigInteger bigger, smaller;
            int resultSign;

            if (CompareAbs(a, b) >= 0)
            {
                bigger = a;
                smaller = b;
                resultSign = a.Sign;
            }
            else
            {
                bigger = b;
                smaller = a;
                resultSign = b.Sign;
            }

            uint[] resultLimbs = SubtractLimbs(bigger.Limbs, smaller.Limbs);
            resultLimbs = TrimLeadingZeros(resultLimbs);

            if (resultLimbs.Length == 0)
                resultSign = 0;

            return new BigInteger(resultLimbs, resultSign);
        }

        public BigInteger Add(BigInteger other)
        {
            if (this.Sign == 0) return other;
            if (other.Sign == 0) return this;

            if (this.Sign == other.Sign)
            {
                // same sign , add magnitudes
                var newLimbs = AddLimbs(this.Limbs, other.Limbs);
                return new BigInteger(newLimbs, this.Sign);
            }
            else
            {
                return SubtractInternal(this, other);
            }
        }
    }
}
