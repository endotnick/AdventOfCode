using System;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Advent
{
    public class DayFive2016

    {
        public static void Entry()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            //string password = "";
            char[] passwordArray = new char[8];
            string doorId = "uqwqemis";
            string tell = "00000";

            using (MD5 md5Hash = MD5.Create())
            {
                int i = 0;
                int tick = 0;
                List<char> usedPos = new List<char>();
                while (tick < 8)
                {
                    //append index to door id

                    string testString = doorId + i;

                    string hash = GetMd5Hash(md5Hash, testString);
                    //Console.WriteLine(hash);

                    //check for five leading zeroes
                    if (hash.Contains(tell) && hash.IndexOf(tell) == 0)
                    {
                        //if so, find position via 6th char
                        //verify 6th char is valid
                        char val = hash[5];
                        switch (val)
                        {
                            //assign 7th char value to position
                            case '0':
                            case '1':
                            case '2':
                            case '3':
                            case '4':
                            case '5':
                            case '6':
                            case '7':
                                if (usedPos.Contains(val))
                                {
                                    Console.WriteLine("Pos {0} already in use", val);
                                    break;
                                }
                                Console.WriteLine("@ {0} - {1} is a valid position, assigning {2} to password", watch.Elapsed, val, hash[6]);
                                passwordArray[int.Parse(val.ToString())] = hash[6];
                                tick++;
                                usedPos.Add(val);
                                break;
                            default:
                                Console.WriteLine("@ {0} - {1} is an invalid position", watch.Elapsed, val);
                                break;
                        }

                        //password += hash[5];
                        //Console.WriteLine("@ {0} - {1} has five leading 0s, {2} added to password. Index: {3}",watch.Elapsed, hash,hash[5], i);
                        //Console.WriteLine("@ {0} - Password == {1}",watch.Elapsed, password);
                    }

                    i++;
                }
                foreach (char c in passwordArray)
                {
                    Console.WriteLine(c);
                }
                Console.WriteLine("@ {0} - Search complete", watch.Elapsed);
                //if not, iterate and rehash
            }

        }
        // from https://msdn.microsoft.com/en-us/library/system.security.cryptography.md5%28v=vs.110%29.aspx
        static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }




    }
}
