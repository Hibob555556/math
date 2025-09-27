# ArkenMath

**ArkenMath** is a custom C# library for arbitrary-precision integer arithmetic. It allows you to perform mathematical operations on integers of virtually unlimited size, beyond the limits of built-in numeric types. This project is a hands-on exploration of low-level math operations, unit testing, and CI/CD workflows in C#.

---

## Features

* Addition and subtraction of arbitrarily large integers
* Supports positive and negative numbers
* Implicit conversions from `int`, `long`, `uint`, and `ulong`
* Internal representation using arrays of 32-bit limbs (`uint[]`)
* Absolute value (`GetAbsoluteValue`) and zero-check (`IsZero`)
* Comparison of magnitudes with `CompareAbs`
* Full support for positive, negative, and zero values
* String-based initialization for numeric input
* Fully tested with MSTest, covering constructors, arithmetic, comparisons, and utility methods
* Configured for CI/CD via GitHub Actions

---

## Installation

1. Clone the repository:

```bash
git clone https://github.com/yourusername/ArkenMath.git
cd ArkenMath
```

2. Open the project in Visual Studio 2022 or later.
3. Restore NuGet packages (if applicable):

```bash
dotnet restore
```

---

## Usage

You can use `ArkenMath` in your own C# projects by adding a **Project Reference** to `ArkenMath.csproj`:

```csharp
using ArkenMath;

class Program
{
    static void Main(string[] args)
    {
        // Implicit conversions
        BigInteger a = 12345;        // int
        BigInteger b = (ulong)67890; // ulong

        // Addition
        var sum = a.Add(b);
        Console.WriteLine($"Sum: {sum}"); // Output: 80235

        // Subtraction (using Add with negative numbers)
        BigInteger c = 5000;
        BigInteger d = -1234;
        var diff = c.Add(d);
        Console.WriteLine($"Difference: {diff}"); // Output: 3766

        // Multi-limb number
        BigInteger e = new BigInteger(new uint[] { 0xFFFFFFFF, 1 }, 1); // Large number across two limbs
        Console.WriteLine($"Multi-limb: {e}");

        // Absolute value
        BigInteger f = new BigInteger(-9876);
        Console.WriteLine($"Absolute: {f.GetAbsoluteValue()}"); // Output: 9876

        // Comparison
        Console.WriteLine($"Compare |a| vs |b|: {BigInteger.CompareAbs(a, b)}"); // -1, 0, or 1
    }
}
```

---

## Running the Demo Program

A simple console program is included for quick testing:

```bash
cd ArkenMath
dotnet run --project .\Program.csproj
```

The program will prompt you to enter two integers, perform addition/subtraction, and display the result.

---

## Running Tests

Unit tests are implemented with MSTest. To run them:

```bash
cd tests
dotnet test BigIntegerTests.csproj
```

The tests now cover:

* All constructors (`int`, `long`, `uint`, `ulong`, `string`)
* Arithmetic (`Add` and subtraction via `Add` with negative values)
* Multi-limb numbers
* Comparison (`CompareAbs`)
* Utility methods (`GetAbsoluteValue`, `IsZero`)

Make sure your test project references the `ArkenMath` library via a `<ProjectReference>` in its `.csproj` file.

---

## Contributing

Feel free to submit pull requests or open issues.

---

## License

This project is open source and available under the MIT License.
