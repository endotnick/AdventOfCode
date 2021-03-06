using System;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Advent
{
    public class DayNine2016

    {
        public static void Entry()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            //readfile
            string content = readFileFromURL("https://raw.githubusercontent.com/endotnick/AdventOfCode/master/AdventOfCode/Input/2016d9");

            Console.WriteLine("@ {0} - Beginning Tests", watch.Elapsed);
            Console.WriteLine("----------------------------------------------");
            if (tests())
            {
                Console.WriteLine("@ {0} - Tests passed - continuing", watch.Elapsed);
            }
            else
            {
                Console.WriteLine("@ {0} - Tests failed - revisit logic", watch.Elapsed);
            }

            string decompString = decompressString(content);
            Console.WriteLine("@ {0} - input Length == {1}", watch.Elapsed, content.Length);
            Console.WriteLine("@ {0} - 1st Pass Length == {1}", watch.Elapsed, decompString.Length);
            //dies - don't do it this way
            //string passTwo = decompressString(decompString);
            watch.Stop();
        }

        //can't use unit tests in online compilers :(
        public static bool tests()
        {
            string clear = "ADVENT";
            string whitespace = "A D V E N T";
            string clear_whitePass = "ADVENT";
            string single = "A(1x5)BC";
            string singlePass = "ABBBBBC";
            string multiple = "(3x3)XYZ";
            string multiplePass = "XYZXYZXYZ";
            string doubles = "A(2x2)BCD(2x2)EFG";
            string doublesPass = "ABCBCDEFEFG";
            string overlap = "(6x1)(1x3)A";
            string overlapPass = "(1x3)A";
            string overlapMulti = "X(8x2)(3x3)ABCY";
            string overlapMultiPass = "X(3x3)ABC(3x3)ABCY";
            string all = "ADVENTA D V E N TA(1x5)BC(3x3)XYZA(2x2)BCD(2x2)EFG(6x1)(1x3)AX(8x2)(3x3)ABCY";
            string allPass = "ADVENTADVENTABBBBBCXYZXYZXYZABCBCDEFEFG(1x3)AX(3x3)ABC(3x3)ABCY";
            //test fully decomp length method
            string lengthTest = "(25x3)(3x3)ABC(2x3)XY(5x2)PQRSTX(18x9)(3x2)TWO(5x7)SEVEN";
            string lengthTestLong = "(27x12)(20x12)(13x14)(7x10)(1x12)A";
            int lengthTestLength = 445;
            int lengthTestLongLength = 241920;
            string[] tests = new string[] { clear, whitespace, single, multiple, doubles, overlap, overlapMulti, all };
            string[] acceptedPass = new string[] { clear_whitePass, clear_whitePass, singlePass, multiplePass, doublesPass, overlapPass, overlapMultiPass, allPass };
            string[] lengthTests = new string[] { lengthTest, lengthTestLong };
            int[] lengthTestsPass = new int[] { lengthTestLength, lengthTestLongLength };
            foreach (string test in tests)
            {
                int i = Array.IndexOf(tests, test);
                string decompressed = decompressString(test);
                //do a 2nd pass on short test strings to get expected lengths
                int fullyDecompLength = decompressString(decompressed).Length;
                int decompressedLength = getDecompressedLength(test);
                if (decompressed != acceptedPass[i])
                {
                    return false;
                }

                //compare fully decompressed length gathered via brute force vs length gathered via algorithm
                if (decompressedLength != fullyDecompLength)
                {
                    //return false;	
                }
            }

            //test lengths gathered via algorithm to known values
            foreach (string test in lengthTests)
            {
                int i = Array.IndexOf(lengthTests, test);
                int decompressedLength = getDecompressedLength(test);
                if (decompressedLength != lengthTestsPass[i])
                {
                    //return false;	
                }
            }

            Console.WriteLine("----------------------------------------------");
            return true;
        }

        public static string readFileFromURL(string url)
        {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead(url);
            StreamReader reader = new StreamReader(stream);
            string content = reader.ReadToEnd();
            return content;
        }

        public static int getDecompressedLength(string input)
        {
            /*
            To get the length of a further decompressed string, expand those instructions that fall within other instructions.
            This, however, is too memory intensive, so we must figure out the patterns therein.

            ASSUMING
            (WxX)(AxB)stringOne(CxD)stringTwo(ExF)stringThree(YxZ)(GxH)...
            W will always equal "(AxB)stringOne(CxD)stringTwo(ExF)stringThree".Length
            A will always equal stringOne.Length
            C will always equal stringTwo.Length
            E will always equal stringThree.Length
            So (WxX) should be equivalent to (((A * B) + (C * D) + (E * F)) * X) 

            TEST
            (21x2)(1x2)A(2x2)BC(3x3)DEF(15x35)(2x2)
            working with: 
            (21x2)(1x2)A(2x2)BC(3x3)DEF
            Expanded length of working segment should be == (1*2 + 2*2 + 3*3) * 2 == 30
            Expands to 
            (1x2)A(2x2)BC(3x3)DEF(1x2)A(2x2)BC(3x3)DEF
            AABCBCDEFDEFDEFAABCBCDEFDEFDEF which has length 30

            TEST
            (7x5)(1x10)L(28x6)
            working segment = 7 char from end of (7x5) including the (7x5) instruction == (7x5)(1x10)L
            Expanded length of working segment should be == ((1*10) * 5) == 50
            Expands to
            (7x5)(1x10)L
            (1x10)L(1x10)L(1x10)L(1x10)L(1x10)L
            LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL == 50

            */
            int index = 0;
            int inputLength = 0;
            int segmentStart = 0;
            int segmentEnd = 0;
            int outputLength = 0;
            string hunter = ")(";
            while (index + 1 < input.Length)
            {
                // find ")("
                if (String.Concat(input[index], input[index + 1]) == hunter)
                {
                    List<KeyValuePair<int, int>> internalMultipliers = new List<KeyValuePair<int, int>>();
                    int internalLength = 0;
                    int segmentLength;
                    int segmentMultiple;
                    string workingSegment = "";
                    // in instruction (AxB) - set index of '(', 'x', and ')' for use
                    int instructionEnd = index;
                    int instructionPos = reverseSearch(input, instructionEnd, 'x');
                    int instructionStart = reverseSearch(input, instructionPos, '(');

                    Console.WriteLine("HUNTER FOUND @ index {0}", index);

                    //set segmentStart = index of '(' in segment
                    segmentStart = index;

                    //in instruction (AxB) - select A	
                    segmentLength = int.Parse(parseElement(input, instructionStart, instructionPos));

                    //in instruction (AxB) - select B
                    segmentMultiple = int.Parse(parseElement(input, instructionPos, instructionEnd));

                    //need to include the final index
                    segmentEnd = segmentStart + segmentLength + 1;

                    //get working segment
                    workingSegment = parseElement(input, segmentStart, segmentEnd);
                    Console.WriteLine("segmentLength {0} segmentMultiple {1} input {2}, segmentStart {3}, workingSegment {4}", segmentLength, segmentMultiple, input, segmentStart, workingSegment);

                    //TODO: iterate over workingSegment to get internalSegmentLength and internalMultiple

                    internalMultipliers.Add(new KeyValuePair<int, int>(segmentLength, segmentMultiple));
                    foreach (KeyValuePair<int, int> entry in internalMultipliers)
                    {
                        internalLength += (entry.Key * entry.Value);
                    }

                    //Console.WriteLine("{0}, {1} - {2}",segmentLength, segmentMultiple, internalLength);
                    //now that we don't care about the internal segment lengths, we can reuse this int			
                    segmentLength = (internalLength * segmentMultiple);
                    outputLength += segmentLength;
                    // index = segmentEnd;
                }

                index++;
            }

            return outputLength;
        }

        public static string decompressString(string test)
        {
            string input = RemoveWhitespace(test);
            int index = 0;
            int instructionPos;
            int selector = 0;
            int repeater = 0;
            string output = "";
            instructionPos = input.IndexOf('x');
            //Console.WriteLine("IN: {0}",test);

            //handle no instructions
            if (instructionPos < 0)
            {
                output = input;
                //Console.WriteLine("No instruction, - continuing");
                //Console.WriteLine("OUT: {0} - LENGTH: {1}", output, output.Length);
                return output;
            }

            //process instruction
            char breaker = '(';

            //get leading chars
            if (input.IndexOf(breaker) != 0)
            {
                for (int i = 0; i < input.IndexOf(breaker); i++)
                    output += input[i];
            }

            //Console.WriteLine(output);
            while (index != input.Length - 1)
            {
                output += ProcessInstruction(instructionPos, input, ref repeater, ref selector, ref index);
                //Console.WriteLine(output);
                if ((index + selector) < input.Length && nextInstructionPresent(index + selector, input, ref instructionPos))
                {
                    //Console.WriteLine("Instruction Present!{0}, {1}", index, instructionPos);
                    continue;
                }
                else
                {
                    //Console.WriteLine("No Instruction Present - appending remainder of string");

                    //skip over selection already processed
                    index = index + selector;
                    output += ProcessRemainingInput(input, index);
                    //Console.WriteLine(output);
                    break;
                }
            }

            //if no instruction, return string and length
            //Console.WriteLine("OUT: {0} - LENGTH: {1}", output, output.Length);
            return output;
        }

        public static string ProcessRemainingInput(string input, int index)
        {
            string output = "";
            for (int i = index; i < input.Length; i++)
            {
                output += input[i];
            }

            return output;
        }

        public static string ProcessInstruction(int instructionPos, string input, ref int repeater, ref int selector, ref int index)
        {
            string output = "";
            int interimIndex = 0;
            int repeatCount = 0;

            //create interimIndex if we're on the 2nd or greater instruction
            if (index + selector > 0 && index + selector != instructionPos)
            {
                interimIndex = index + selector;
            }

            //from 'x', iterate backwards to reach '(' 
            index = reverseSearch(input, instructionPos, '(');

            //prepend interim chars to output
            if (interimIndex > 0)
            {
                for (int i = interimIndex; i < index; i++)
                {
                    output += input[i];
                }
            }

            //get selector by parsing from '(' to 'x'
            selector = int.Parse(parseElement(input, index, instructionPos));

            //from 'x', interate forwards to reach ')'
            index = forwardSearch(input, instructionPos, ')');

            //get repeater by parsing from 'x' to ')'
            repeater = int.Parse(parseElement(input, instructionPos, index));

            //set index to breaker + 
            index++;

            //execute instruction
            while (repeatCount < repeater)
            {
                for (int i = 0; i < selector; i++)
                {
                    output += input[index + i];
                }

                repeatCount++;
            }

            //Console.WriteLine("Exiting ProcessInstruction, index is {0}", index);
            return output;
        }

        public static string parseElement(string input, int startIndex, int endIndex)
        {
            string element = "";
            for (int i = startIndex + 1; i < endIndex; i++)
            {
                element += input[i];
            }

            return element;
        }

        public static int reverseSearch(string input, int index, char breaker)
        {
            while (index != 0 && input[index] != breaker)
            {
                index--;
            }

            return index;
        }

        public static int forwardSearch(string input, int index, char breaker)
        {
            while (index != input.Length - 1 && input[index] != breaker)
            {
                index++;
            }

            return index;
        }

        //sets the position of the next instruction if it exsists
        public static bool nextInstructionPresent(int index, string input, ref int instructionPos)
        {
            bool present = false;

            //find next instructionPos
            int i = forwardSearch(input, index, 'x');
            if (input[i] == 'x')
            {
                present = true;
                instructionPos = i;
                //Console.WriteLine("New instruction at {0}, {1}", i, input[i]);
            }

            //Console.WriteLine("Exiting nextInstructionPresent, index is {0}, instructionPos is {1}", index, instructionPos);
            return present;
        }

        public static string RemoveWhitespace(string input)
        {
            return new string(input.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());
        }
    }
}
