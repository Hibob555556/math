namespace ArkenMath
{
    public class BigInteger
    {
        // Internal representation
        private readonly uint[] _limbs; // least significant limb at index 0, stores magnitude (absolute value)
        private readonly int _sign; // -1 for negative, 0 for zero, +1 for positive

        // Properties to access sign and limbs
        public int Sign => _sign;
        public uint[] Limbs => _limbs;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BigInteger"/> class using the specified array of unsigned
        /// integers and a sign value.
        /// </summary>
        /// <remarks>The <paramref name="limbs"/> parameter is trimmed of any leading zeros to ensure a
        /// canonical representation. If the resulting magnitude is zero, the <paramref name="sign"/> is ignored, and
        /// the instance represents zero.</remarks>
        /// <param name="limbs">An array of unsigned integers representing the magnitude of the number, where the least significant limb is
        /// at index 0. The array must not contain leading zeros unless the number is zero.</param>
        /// <param name="sign">The sign of the number. Use a positive value for positive numbers, a negative value for negative numbers, or
        /// zero for zero.</param>
        public BigInteger(uint[] limbs, int sign)
        {
            _limbs = TrimLeadingZeros(limbs);
            _sign = _limbs.Length == 0 ? 0 : (sign >= 0 ? 1 : -1);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BigInteger"/> structure using a 32-bit signed integer.
        /// </summary>
        /// <remarks>If <paramref name="value"/> is zero, the resulting <see cref="BigInteger"/>
        /// represents the value zero. For non-zero values, the sign of the <see cref="BigInteger"/> matches the sign of
        /// <paramref name="value"/>.</remarks>
        /// <param name="value">The 32-bit signed integer to initialize the <see cref="BigInteger"/> with.</param>
        public BigInteger(int value)
        {
            if (value == 0)
            {
                _limbs = Array.Empty<uint>();
                _sign = 0;
            }
            else
            {
                _limbs = [(uint)Math.Abs(value)];
                _sign = value > 0 ? 1 : -1;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BigInteger"/> structure using a 64-bit signed integer.
        /// </summary>
        /// <remarks>This constructor creates a <see cref="BigInteger"/> that represents the value of the
        /// specified 64-bit signed integer. The sign of the resulting <see cref="BigInteger"/> is determined by the
        /// sign of <paramref name="value"/>. If <paramref name="value"/> is zero, the resulting <see
        /// cref="BigInteger"/> will represent zero.</remarks>
        /// <param name="value">The 64-bit signed integer to initialize the <see cref="BigInteger"/> with.</param>
        public BigInteger(long value)
        {
            if (value == 0)
            {
                _limbs = Array.Empty<uint>();
                _sign = 0;
            }
            else
            {
                ulong abs = (ulong)(value < 0 ? -value : value);
                if (abs <= uint.MaxValue)
                {
                    _limbs = new uint[] { (uint)abs };
                }
                else
                {
                    _limbs = new uint[] { (uint)(abs & 0xFFFFFFFF), (uint)(abs >> 32) };
                }
                _sign = value > 0 ? 1 : -1;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BigInteger"/> class using the specified string representation
        /// of a number.
        /// </summary>
        /// <remarks>This constructor currently supports parsing numeric values that fit within the range
        /// of a 32-bit signed integer. Larger numbers are not yet supported and will result in a <see
        /// cref="NotImplementedException"/>.</remarks>
        /// <param name="value">A string representing the numeric value to initialize the <see cref="BigInteger"/> instance.  The string may
        /// optionally include a leading '+' or '-' sign to indicate the number's sign.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is <see langword="null"/>, empty, or consists only of whitespace.</exception>
        /// <exception cref="NotImplementedException">Thrown if the numeric value represented by <paramref name="value"/> exceeds the range of a 32-bit signed
        /// integer.</exception>
        public BigInteger(string value)
        {
            // TODO: Implement full parsing for larger numbers
            // TODO: add support for arbitrary-precision parsing (currently only handles int)
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Input string cannot be null or whitespace.", nameof(value));

            _sign = value.StartsWith("-") ? -1 : 1;
            string digits = value.TrimStart('+', '-');

            if (int.TryParse(digits, out int parsed))
            {
                _limbs = parsed == 0 
                    ? Array.Empty<uint>() 
                    : new uint[] { (uint)Math.Abs(parsed) };
                if (parsed == 0) _sign = 0;
            }
            else
            {
                throw new NotImplementedException("Parsing larger numbers not yet implemented.");
            }
        }
        #endregion

        #region Implicit Conversions
        /// <summary>
        /// Converts an <see cref="int"/> value to a <see cref="BigInteger"/> implicitly.
        /// </summary>
        /// <param name="value">The 32-bit signed integer to convert.</param>
        public static implicit operator BigInteger(int value) => new BigInteger(value);

        /// <summary>
        /// Defines an implicit conversion from a <see cref="long"/> to a <see cref="BigInteger"/>.
        /// </summary>
        /// <param name="value">The 64-bit signed integer to convert to a <see cref="BigInteger"/>.</param>
        public static implicit operator BigInteger(long value) => new BigInteger(value);

        /// <summary>
        /// Converts a 32-bit unsigned integer to a <see cref="BigInteger"/> implicitly.
        /// </summary>
        /// <param name="value">The 32-bit unsigned integer to convert.</param>
        /// <returns>A <see cref="BigInteger"/> representing the same numeric value as <paramref name="value"/>.</returns>
        public static implicit operator BigInteger(uint value) => new BigInteger((long)value);

        /// <summary>
        /// Converts a 64-bit unsigned integer to a <see cref="BigInteger"/> implicitly.
        /// </summary>
        /// <param name="value">The 64-bit unsigned integer to convert.</param>
        /// <returns>A <see cref="BigInteger"/> representing the same numeric value as <paramref name="value"/>.</returns>
        public static implicit operator BigInteger(ulong value)
        {
            if (value == 0)
                return new BigInteger(0);

            uint lower = (uint)(value & 0xFFFFFFFF);
            uint upper = (uint)(value >> 32);

            if (upper == 0)
                return new BigInteger(new uint[] { lower }, 1);
            else
                return new BigInteger(new uint[] { lower, upper }, 1);
        }
        #endregion

        /// <summary>
        /// Returns a string representation of the current object.
        /// </summary>
        /// <remarks>The string representation includes the sign and the values of the internal limbs,
        /// formatted as a comma-separated list.</remarks>
        /// <returns>A string that represents the current object, including its sign and internal limb values.</returns>
        public override string ToString()
        {
            // handle zero case
            if (Sign == 0)
                return "0";

            // work with the absolute value
            var temp = new BigInteger(this.Limbs, 1);

            // store digits here
            List<char> digits = new();

            // repeatedly divide by 10 to extract digits
            // stop when temp is 0
            while (temp.Sign != 0)
            {
                (temp, uint remainder) = DivRem(temp, 10);
                digits.Add((char)('0' + remainder));
            }

            // add sign if negative
            if (this.Sign < 0)
                digits.Add('-');

            // reverse the digits to get the correct order
            digits.Reverse();

            // return as string
            return new string(digits.ToArray());
        }

        /// <summary>
        /// Adds two arrays of unsigned 32-bit integers, treating them as limbs of large integers.
        /// </summary>
        /// <remarks>Each input array is treated as a large integer in little-endian format, where the
        /// least significant limb is at index 0. The method handles arrays of different lengths by treating missing
        /// limbs as zero. Leading zeros in the result are trimmed before returning.</remarks>
        /// <param name="a">The first array of unsigned 32-bit integers, representing the first large integer. Cannot be null.</param>
        /// <param name="b">The second array of unsigned 32-bit integers, representing the second large integer. Cannot be null.</param>
        /// <returns>An array of unsigned 32-bit integers representing the sum of the two input arrays.  The result may have one
        /// additional limb to account for carry overflow.</returns>
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


        /// <summary>
        /// Returns the absolute value of the current <see cref="BigInteger"/> instance.
        /// </summary>
        /// <returns>A <see cref="BigInteger"/> representing the absolute value of the current instance.  If the value is
        /// non-negative, the instance itself is returned; otherwise, a new  <see cref="BigInteger"/> with the same
        /// magnitude but a positive sign is returned.</returns>
        public BigInteger GetAbsoluteValue()
            => this.Sign >= 0 ? this : new BigInteger(this.Limbs, 1);


        /// <summary>
        /// Removes leading zero elements from the specified array of unsigned integers.
        /// </summary>
        /// <param name="limbs">An array of unsigned integers to process. Cannot be null.</param>
        /// <returns>A new array containing the elements of <paramref name="limbs"/> without leading zeros.  Returns an empty
        /// array if all elements in <paramref name="limbs"/> are zero.</returns>
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

        /// <summary>
        /// Compares the absolute values of two <see cref="BigInteger"/> instances.
        /// </summary>
        /// <remarks>This method compares the magnitudes of the two <see cref="BigInteger"/> instances
        /// without considering their signs.</remarks>
        /// <param name="a">The first <see cref="BigInteger"/> to compare.</param>
        /// <param name="b">The second <see cref="BigInteger"/> to compare.</param>
        /// <returns>A signed integer that indicates the relative order of the absolute values of <paramref name="a"/> and
        /// <paramref name="b"/>: <list type="bullet"> <item> <description>1 if the absolute value of <paramref
        /// name="a"/> is greater than the absolute value of <paramref name="b"/>.</description> </item> <item>
        /// <description>-1 if the absolute value of <paramref name="a"/> is less than the absolute value of <paramref
        /// name="b"/>.</description> </item> <item> <description>0 if the absolute values of <paramref name="a"/> and
        /// <paramref name="b"/> are equal.</description> </item> </list></returns>
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

        /// <summary>
        /// Subtracts the values of two unsigned integer arrays, treating them as multi-precision numbers.
        /// </summary>
        /// <remarks>This method assumes that <paramref name="bigger"/> is greater than or equal to
        /// <paramref name="smaller"/> in magnitude. If this condition is not met, the result will be incorrect. The
        /// subtraction is performed element-wise, with proper handling of borrow propagation across array
        /// indices.</remarks>
        /// <param name="bigger">The larger array representing the minuend. Must have a length greater than or equal to <paramref
        /// name="smaller"/>.</param>
        /// <param name="smaller">The smaller array representing the subtrahend. If shorter than <paramref name="bigger"/>, it is treated as
        /// zero-padded on the higher indices.</param>
        /// <returns>An array of unsigned integers representing the result of the subtraction. The result has the same length as
        /// <paramref name="bigger"/>.</returns>
        private static uint[] SubtractLimbs(uint[] bigger, uint[] smaller)
        {
            int length = bigger.Length; // get the length of the bigger array which will be the larger number
            uint[] result = new uint[length]; // result will be at most the size of the bigger number
            long borrow = 0; // use long to handle borrow properly

            // Perform subtraction with borrow
            for (int i = 0; i < length; i++)
            {
                long bi = bigger[i]; // current limb from bigger
                long si = i < smaller.Length ? smaller[i] : 0; // current limb from smaller or 0 if out of bounds

                long diff = bi - si - borrow; // subtract with borrow
                if (diff < 0) // need to borrow
                {
                    borrow = 1;
                    diff += 0x100000000; // 2^32
                }
                else // no borrow needed
                {
                    borrow = 0;
                }

                // Store the result limb
                result[i] = (uint)diff;
            }
            // Trim leading zeros if any
            return TrimLeadingZeros(result);
        }

        /// <summary>
        /// Subtracts the absolute values of two <see cref="BigInteger"/> instances and determines the resulting sign.
        /// </summary>
        /// <remarks>This method assumes that the caller has already ensured the inputs are valid and is
        /// intended for internal use as part of the subtraction operation. The result is normalized to remove any
        /// leading zeros in its representation.</remarks>
        /// <param name="a">The first <see cref="BigInteger"/> operand.</param>
        /// <param name="b">The second <see cref="BigInteger"/> operand.</param>
        /// <returns>A new <see cref="BigInteger"/> representing the result of the subtraction. The result's sign is determined
        /// based on the relative magnitudes and signs of the input values.</returns>
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

        /// <summary>
        /// Adds the value of the specified <see cref="BigInteger"/> to the current instance.
        /// </summary>
        /// <remarks>If either the current instance or <paramref name="other"/> is zero, the non-zero
        /// value is returned.  If both values have the same sign, their magnitudes are added. Otherwise, the result is
        /// determined  by subtracting the smaller magnitude from the larger magnitude, with the appropriate
        /// sign.</remarks>
        /// <param name="other">The <see cref="BigInteger"/> to add to the current instance.</param>
        /// <returns>A new <see cref="BigInteger"/> representing the sum of the current instance and the specified <paramref
        /// name="other"/>.</returns>
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

        /// <summary>
        /// Divides a <see cref="BigInteger"/> value by a 32-bit unsigned integer and returns the quotient and
        /// remainder.
        /// </summary>
        /// <remarks>The division is performed using the most significant limb of the <see
        /// cref="BigInteger"/> value first. The method ensures that the remainder is always less than the
        /// divisor.</remarks>
        /// <param name="value">The <see cref="BigInteger"/> value to be divided.</param>
        /// <param name="divisor">The 32-bit unsigned integer divisor. Must be greater than zero.</param>
        /// <returns>A tuple containing the quotient as a <see cref="BigInteger"/> and the remainder as a 32-bit unsigned
        /// integer.</returns>
        /// <exception cref="DivideByZeroException">Thrown if <paramref name="divisor"/> is zero.</exception>
        private static (BigInteger quotient, uint remainder) DivRem(BigInteger value, uint divisor)
        {
            // Handle zero case
            if (divisor == 0)
                throw new DivideByZeroException("Divisor cannot be zero."); // division by zero is not allowed

            // create array to store result
            uint[] result = new uint[value.Limbs.Length];
            ulong remainder = 0;

            // perform division from most significant limb to least
            for (int i = value._limbs.Length - 1; i >= 0; i--)
            {
                ulong current = (remainder << 32) | value._limbs[i];
                result[i] = (uint)(current / divisor);
                remainder = current % divisor;
            }

            // convert result to BigInteger and return
            var q = new BigInteger(result, value.Sign);
            return (q, (uint)remainder);
        }


        /// <summary>
        /// Determines whether the specified <see cref="BigInteger"/> value is zero.
        /// </summary>
        /// <param name="a">The <see cref="BigInteger"/> value to evaluate.</param>
        /// <returns><see langword="true"/> if the specified value is zero; otherwise, <see langword="false"/>.</returns>
        public static bool IsZero(BigInteger a) => a.Sign == 0;
    }
}
