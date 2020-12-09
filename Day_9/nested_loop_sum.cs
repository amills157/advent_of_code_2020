using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace advent_day_9 {
   class puzzle_9 {
      // Hello Day 1 my old friend...
      public static long nestedLoop(StreamWriter sw, ref long[] longArray){
         
         long weakNumber = 0;
     
         for (int i = 25; i < longArray.Length; i++)
         {
            bool cont = false; 

            for (int j = (i-25); j <= (i-1); j++)
            {
               sw.WriteLine("{0} | {1}",i, j);
               for (int k = (i-25); k <= (i-1); k++)
               {  
                  sw.WriteLine("{0} | {1}",i, k);
                  if (longArray[j] != longArray[k])
                  {
                     long sum = longArray[j] + longArray[k];
                     if (sum == longArray[i]){
                        sw.WriteLine("{0} = {1} + {2}", longArray[i], longArray[j], longArray[k]);
                        cont = true;
                        goto exitLoop1;
                     }
                  }
               }
            }
            exitLoop1:
            if (!cont){
               weakNumber = longArray[i];
               break;
            }
         }
         return weakNumber;
      } 


      public static long nestedSetLoop(StreamWriter sw, ref long[] longArray, long weakNumber){
         
         long result = 0;

         List<long> contiguousSet = new List<long>();

         for (int i = 0; i < longArray.Length; i++)
         {
            // New set each time
            contiguousSet.Clear();
            // Starting from the same place as our outer loop we add elements until...
            for (int j = i; j < longArray.Length; j++)
            {               
               contiguousSet.Add(longArray[j]);
               long total = contiguousSet.Sum();
               // We've gone too far (Jim)
               if (total > weakNumber){
                  goto exitLoop2;
               }

               // Or we get our target
               if (total == weakNumber){
                  Console.WriteLine("Min -- {0}", contiguousSet.Min());
                  Console.WriteLine("Max -- {0}", contiguousSet.Max());
                  result =  (contiguousSet.Min() + contiguousSet.Max());
                  goto exitLoop3;
               }
            }
            exitLoop2:;
         }
         exitLoop3:;

         return result;
      }


      static void Main(string[] args) 
      {
         // Load input
         IEnumerable<string> lines = File.ReadLines("puzzle_9_input.txt");

          // Using a output file for debugging
         string outputFile = "puzzle_9_output.txt"; 
         // Which is wiped at the start of each run to avoid trawling through reams of output
         System.IO.File.WriteAllText(outputFile,string.Empty);

         List <long> longList = new List<long>();

         // Convert each string int to int
         foreach (string r in lines)
         {  
            longList.Add(Convert.ToInt64(r));
         }

         // Needs to be an array not a list for .Length
         long[] longArray = longList.ToArray();
         
         using(StreamWriter sw = File.AppendText(outputFile)) 
         { 
            long weakNumber = nestedLoop(sw, ref longArray);

            Console.WriteLine("You are the weakest link -- {0}", weakNumber);

            long weakNumber2 = nestedSetLoop(sw, ref longArray, weakNumber);

            Console.WriteLine("You are also the weakest link -- {0}", weakNumber2);

            // If you're not sure why this is here you missed the reference
            Console.WriteLine("Goodbye");

         }
      }
   }
}