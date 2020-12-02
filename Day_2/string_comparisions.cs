using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace advent_day_2 {
   class puzzle_2 {
      static void Main(string[] args) 
      {
         // Load input
         IEnumerable<string> lines = File.ReadLines("puzzle_2_input.txt");

         // Using a output file for debugging
         string outputFile = "puzzle_2_output.txt"; 
         // Which is wiped at the start of each run to avoid trawling through reams of output
         System.IO.File.WriteAllText(outputFile,string.Empty);

         int passed_count_sled=0;
         int failed_count_sled=0;

         int passed_count_toboggan=0;
         int failed_count_toboggan=0;

         // Keep earything within one loop for ease
         foreach (string r in lines)
         {  
            string clean = r.Replace(":", "").Replace("-", " ");
            string[] splitString = clean.Split(' ').Select(str => str.Trim()).ToArray();

            int min = Convert.ToInt32(splitString[0]);
            int max = Convert.ToInt32(splitString[1]);

            char expected = char.Parse(splitString[2]);

            // Only really needed for debugging purposes
            char firstPos = '-';
            char secondPos = '-';

            int firstPosCheck = 0;
            int secondPosCheck = 0;

            string toCheck = splitString[3];

            int count = toCheck.Count(x => x == expected);

            // Write detailed output for debugging
            using(StreamWriter sw = File.AppendText(outputFile)) 
            { 
               // "Original" input so we can compare - Our ground truth
               sw.WriteLine(string.Join(" ", splitString));

               sw.WriteLine("Count -- {0}", count);

               if (count >= min && count <= max)
               {
                  passed_count_sled +=1;
                  sw.WriteLine("Passed (Sled Rental) -- {0}", passed_count_sled);
               } else 
               {
                  failed_count_sled +=1;
                  sw.WriteLine("Failed (Sled Rental) -- {0}", failed_count_sled);
               }

               // Nested if which is largely the same for first and second pos - Check if we can assign before comparision 
               if (toCheck.Length >= (min-1)) {
                  firstPos = toCheck[(min-1)];
                  if (toCheck[(min-1)] == expected){
                     firstPosCheck = 1;
                  }
                  sw.WriteLine("firstPos -- {0}, index(non zero) -- {1}", firstPos, min);
               }

               if (toCheck.Length >= (max-1)) {
                  secondPos = toCheck[(max-1)];
                  if (toCheck[(max-1)] == expected){
                     secondPosCheck = 1;
                  }
                  sw.WriteLine("secondPos -- {0}, index(non zero) -- {1}", secondPos, max);
               }
               
               // Use of int style bool (0 and 1) for ease of validation check
               if ((firstPosCheck + secondPosCheck)==1)
               {
                  passed_count_toboggan +=1;
                  sw.WriteLine("Passed (Toboggan Rental) -- {0}", passed_count_toboggan);
               } else 
               {
                  failed_count_toboggan +=1;
                  sw.WriteLine("Failed (Toboggan Rental) -- {0}", failed_count_toboggan);
               }

            }
         }
         // Terminal output for the important parameters
         Console.WriteLine("Total Passed (Sled Rental) -- {0}", passed_count_sled);

         Console.WriteLine("Total Failed (Sled Rental) -- {0}", failed_count_sled);

         Console.WriteLine("Total Passed (Toboggan Rental) -- {0}", passed_count_toboggan);

         Console.WriteLine("Total Failed (Toboggan Rental) -- {0}", failed_count_toboggan);
      }
   }
}