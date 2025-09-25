# ArkenMath

**ArkenMath** is a custom C# library for arbitrary-precision integer arithmetic. It allows you to perform mathematical operations on integers of virtually unlimited size, beyond the limits of built-in numeric types. This project is a hands-on exploration of low-level math operations, unit testing, and CI/CD workflows in C#.

---

## Features

* Addition and subtraction of arbitrarily large integers
* Supports positive and negative numbers
* Internal representation using arrays of 32-bit limbs (`uint[]`)
* Absolute value and comparison methods
* Fully tested with MSTest
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
        var a = new BigInteger(new uint[] { 5 }, 1);  // +5
        var b = new BigInteger(new uint[] { 3 }, -1); // -3

        var result = a.Add(b); // internally handles subtraction
        Console.WriteLine(result); // Output: +[2]
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

Make sure your test project references the `ArkenMath` library via a `<ProjectReference>` in its `.csproj` file.

---

## Contributing

Feel free submit pull requests or open issues.

---

## License

This project is open source and available under the MIT License.
