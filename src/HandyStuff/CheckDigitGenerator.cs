namespace HandyStuff;

using System.Text;

public class CheckDigitGenerator
{
    /// <summary>
    /// Generate a new randomised ID with a checkdigit
    /// </summary>
    /// <param name="length">The total length of the random number string to be generated. The check digit will be an extra character after this</param>
    /// <returns>Random sequence of numbers including a checkdigit appended</returns>
    public string NewRandomNumberSequenceWithCheckDigit(int length = 8, int[]? weightingSequence = null)
    {
        var numberString = GenerateRandomNumericString(length - 1);
        var checkDigit = GenerateCheckDigit(numberString, weightingSequence);

        return numberString + checkDigit;
    }

    /// <summary>
    /// Generate a check digit for an provided input string
    /// </summary>
    /// <param name="input">Any string that you would like to generate a digit for</param>
    /// <param name="weightingSequence">A sequence of characters that will be used for the weighting of the each character in the input string</param>
    /// <returns>The check digit for the provided input</returns>
    public char GenerateCheckDigit(string input, int[]? weightingSequence = null)
    {
        if (weightingSequence == null)
        {
            SetDefaultWeightingSequence(ref weightingSequence, input);
        }

        int sum = 0;
        int weightingSequenceIndex = 0;

        for (int index = 0; index < input.Length; index++)
        {
            sum += input[index] * weightingSequence![weightingSequenceIndex];
            weightingSequenceIndex = NextWeightSequenceIndex(weightingSequenceIndex, weightingSequence!);
        }

        // Map sum to an ascii letter
        return (char)('A' + sum % 26);
    }

    private void SetDefaultWeightingSequence(ref int[]? weightingSequence, string input)
    {
        int length = input.Length;
        weightingSequence = new int[length];
        int left = 0, right = length - 1;
        int value = length;

        while (left <= right)
        {
            if (left == right)
            {
                weightingSequence[left] = value--;
            }
            else
            {
                weightingSequence[left] = value--;
                weightingSequence[right] = value--;
            }

            left++;
            right--;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="currentIndex"></param>
    /// <param name="sequence"></param>
    /// <returns>The weightingSequence index</returns>
    private int NextWeightSequenceIndex(int currentIndex, int[] sequence)
    {
        if (currentIndex == sequence.Length - 1)
        {
            return 0;
        }

        currentIndex++;

        return currentIndex;
    }

    /// <summary>
    /// Generate a random sequence numbers returning them as a string
    /// </summary>
    /// <param name="length">How many digits to generate</param>
    /// <returns>The randomised string of the length specified</returns>
    private string GenerateRandomNumericString(int length)
    {
        var random = new Random();
        var number = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            number.Append(random.Next(0, 9)).ToString();
        }

        return number.ToString();
    }

    /// <summary>
    /// Attempt to validate the check digit of the specified input string
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public bool TryIsCheckDigitValid(string input)
    {
        var theString = input[..^1];
        var checkDigit = GenerateCheckDigit(theString);

        return input[^1] == checkDigit;
    }

}
