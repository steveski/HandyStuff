namespace au.steveski.Crypto.UnitTests;

using Shouldly;

public class CheckDigitGeneratorTests
{
    [Fact]
    public void GenerateCheckDigit_For3DigitInput_GeneratesValidCheckDigit_OddLength()
    {
        var sut = new CheckDigitGenerator();

        var checkDigit = sut.GenerateCheckDigit("123");

        // Expected weights:
        // Digit 1: 3
        // Digit 2: 1
        // Digit 3: 2
        //
        // Therefore ( ( '1' * 3) + ('2' * 1) + ('3' * 2) ) % 26 + 'A'
        //    Values ( ( 49 * 3) +  (50 * 1) +  (51 * 2) )  % 26 + 65

        var expected = (char)(('1' * 3 + '2' * 1 + '3' * 2) % 26 + 'A');
        checkDigit.ShouldBe(expected);
    }

    [Fact]
    public void GenerateCheckDigit_For4DigitInput_GeneratesValidCheckDigit_EvenLength()
    {
        var sut = new CheckDigitGenerator();

        var checkDigit = sut.GenerateCheckDigit("1234");

        // Expected weights:
        // Digit 1: 4
        // Digit 2: 2
        // Digit 3: 1
        // Digit 4: 3
        //
        // Therefore ( ( '1' * 4) + ('2' * 2) + ('3' * 1) + ('4' * 3 ) % 26 + 'A'
        //    Values ( ( 49 * 4) +  (50 * 2) +  (51 * 1) +  (52  * 3) )  % 26 + 65

        var expected = (char)(('1' * 4 + '2' * 2 + '3' * 1 + '4' * 3) % 26 + 'A');
        checkDigit.ShouldBe(expected);
    }


    [Fact]
    public void GenerateCheckDigit_For6CharacterInput_GeneratesValidCheckDigit_EvenLength()
    {
        var sut = new CheckDigitGenerator();

        var checkDigit = sut.GenerateCheckDigit("ABC321");

        // Expected weights:
        // Digit 1: A
        // Digit 2: B
        // Digit 3: C
        // Digit 4: 3
        // Digit 5: 2
        // Digit 6: 1
        //
        // Therefore ( ( 'A' * 6 ) + ( 'B' * 4 ) + ( 'C' * 2 ) + ( '3' * 1 ) + ( '2' * 3 ) + ( '1' * 5 ) % 26 + 'A'
        //    Values ( ( 65 * 6 )  + ( 65 * 4 )  + ( 66 * 2 )  + ( 51  * 1 ) + ( 50  * 3 ) + ( 49  * 5 ) )  % 26 + 65

        var expected = (char)(('A' * 6 + 'B' * 4 + 'C' * 2 + '3' * 1 + '2' * 3 + '1' * 5) % 26 + 'A');
        checkDigit.ShouldBe(expected);
    }

    [Fact]
    public void GenerateCheckDigitWithWeightingSequenceProvided()
    {
        var sut = new CheckDigitGenerator();
        int[] weightingSequence = [7, 5, 3, 1, 2, 4, 6];

        var checkDigit = sut.GenerateCheckDigit("ABC4321", weightingSequence);

        var expected = (char)((
            'A' * weightingSequence[0] +
            'B' * weightingSequence[1] +
            'C' * weightingSequence[2] +
            '4' * weightingSequence[3] +
            '3' * weightingSequence[4] +
            '2' * weightingSequence[5] +
            '1' * weightingSequence[6])
            % 26 + 'A');
        checkDigit.ShouldBe(expected);
    }

    [Theory]
    [InlineData("ABC4321", "7,5,3,1,2,4,6", 'W')]
    [InlineData("ABC7654321", "4,2,1,3", 'T')]
    [InlineData("ABC4321", "10,8,6,4,2,1,3,5,7,9", 'H')]
    public void GenerateCheckDigitWithVaryingWeightingSequences(string input, string weightingSequenceString, char expected)
    {
        var sut = new CheckDigitGenerator();
        int[] weightingSequence = weightingSequenceString
            .Split(',')
            .Select(int.Parse)
            .ToArray();

        var checkDigit = sut.GenerateCheckDigit(input, weightingSequence);

        checkDigit.ShouldBe(expected);
    }

    [Theory]
    [InlineData("ABC4321", 'W')]
    [InlineData("0192837465", 'X')]
    [InlineData("ABC123DEF456", 'E')]
    public void GenerateCheckDigitWithNoWeightinSequenceProvided(string input, char expected)
    {
        var sut = new CheckDigitGenerator();

        var checkDigit = sut.GenerateCheckDigit(input);

        checkDigit.ShouldBe(expected);
    }

    [Theory]
    [InlineData("ABC4321W", true)]
    [InlineData("0192837465X", true)]
    [InlineData("ABC123DEF456E", true)]
    [InlineData("ASDASDT", false)]
    public void TryIsCheckDigitValidReturnsExpected(string inputWithCheckDigit, bool expected)
    {
        var sut = new CheckDigitGenerator();
        var isvalidCheckDigit = sut.TryIsCheckDigitValid(inputWithCheckDigit);
        isvalidCheckDigit.ShouldBe(expected);
    }

    [Fact]
    public void NewRandomNumberSequenceWithCheckDigitReturnsIdWithCorrectCheckDigit()
    {
        var sut = new CheckDigitGenerator();
        var newId = sut.NewRandomNumberSequenceWithCheckDigit();
        var isvalidCheckDigit = sut.TryIsCheckDigitValid(newId);
        isvalidCheckDigit.ShouldBeTrue();
    }

}
