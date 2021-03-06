using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Advent
{
    public class DayFour2016

    {
        public static int goodCount = 0;
        public static int selector = 2;
        public static string alpha = "abcdefghijklmnopqrstuvwxyz";

        public static void Entry()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            //readfile
            WebClient client = new WebClient();
            Console.WriteLine("@ {0} - Get File", watch.Elapsed);
            Stream stream = client.OpenRead("https://gist.githubusercontent.com/endotnick/f13990e921490b3a09f8582910d9d3f3/raw/2b5efafec706c73a11a01c7013c08e0146458b43/AoC2016d4p1");
            Console.WriteLine("@ {0} - File opened", watch.Elapsed);
            StreamReader reader = new StreamReader(stream);

            Console.WriteLine("@ {0} - Beginning Read ", watch.Elapsed);
            string content = reader.ReadToEnd();
            int sectorSum = 0;
            Console.WriteLine("@ {0} - Beginning line split", watch.Elapsed);
            //convert file to lines
            string[] lines = content.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            Console.WriteLine("@ {0} - Line[0] = {1}", watch.Elapsed, lines[lines.Length - 2]);

            foreach (string s in lines)
            {
                //Console.WriteLine("@ {0} - Beginning value Split",watch.Elapsed);
                //parse line
                string[] values = s.Split('-');
                string encryptedName = "";
                string decryptedName = "";

                //get roomName			
                for (int i = 0; i < (values.Length - 1); i++)
                {
                    encryptedName += values[i];

                }
                //Console.WriteLine("@ {0} - encryptedName == {1} ",watch.Elapsed, encryptedName);

                //split sectorID/Checksum
                string[] idCheck = values[values.Length - 1].Split('[');

                //get sector ID
                int sectorID = 0;
                try
                {
                    sectorID = int.Parse(idCheck[0]);
                }
                catch
                {
                    Console.WriteLine("Couldn't parse int, check input");
                    continue;
                };

                //Console.WriteLine("@ {0} - idCheck[0] == {1}, sectorID == {2}",watch.Elapsed, idCheck[0], sectorID);

                //get checksum
                string checkSum = idCheck[1].Trim(']');
                //Console.WriteLine("@ {0} - idCheck[1] == {1}, checkSum == {2}",watch.Elapsed, idCheck[1], checkSum);



                //Find Real Rooms:
                //validate checksum
                //determine most common letters 
                string mostCommonLetters = getMostCommonLetters(encryptedName);
                //Console.WriteLine("@ {0} - Most common letters in name == {1}",watch.Elapsed, mostCommonLetters);
                //string compare w/ checksum
                if (string.Equals(checkSum, mostCommonLetters))
                {
                    //if good, add sector ID to sectorSum
                    sectorSum += sectorID;
                    //Console.WriteLine("@ {0} - sectorSum == {1}",watch.Elapsed, sectorSum);
                    Console.WriteLine(s);
                    for (int i = 0; i < (values.Length - 1); i++)
                    {
                        decryptedName += " " + decryptString(values[i], sectorID % 26);
                    }

                    Console.WriteLine(decryptedName);
                }




                //
            }
            Console.WriteLine("@ {0} - Complete.", watch.Elapsed);
            watch.Stop();
        }
        //given a string and an int x, shift each char in the string forward x times through the alphabet
        public static string decryptString(string source, int shift)
        {
            string decryptedString = "";

            foreach (char c in source)
            {
                int index = alpha.IndexOf(c);
                int shiftedIndex = ((index + shift) % 26);
                decryptedString += alpha.ElementAt(shiftedIndex);
            }

            return decryptedString;
        }

        //given a string, return a string sorted by most common char
        public static string getMostCommonLetters(string source)
        {
            string commonLetters = "";
            char[] chars = source.ToCharArray();
            //count 
            Dictionary<char, int> dictionary = new Dictionary<char, int>();
            foreach (char c in chars)
            {
                if (dictionary.ContainsKey(c))
                {
                    dictionary[c] += 1;
                }
                else
                {
                    dictionary.Add(c, 1);
                }

            }
            //sort by count, then alpha
            var sortedDict = from entry in dictionary
                             orderby entry.Value descending, entry.Key ascending
                             select entry;
            //create sorted string
            foreach (KeyValuePair<char, int> pair in sortedDict)
            {
                commonLetters += pair.Key;
            }
            //strip to first 5
            commonLetters = commonLetters.Substring(0, 5);

            return commonLetters;
        }
    }
}
