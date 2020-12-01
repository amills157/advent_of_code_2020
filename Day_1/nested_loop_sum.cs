using System;
using System.IO;
using System.Collections.Generic;

namespace advent_day_1 {
   class puzzle_1 {
      static void Main(string[] args) 
      {
         // Load input
         IEnumerable<string> lines = File.ReadLines("puzzle_1_input.txt");

         var intList = new List<int>();

         // Convert each string int to int
         foreach (string r in lines)
         {  
            intList.Add(Convert.ToInt32(r));
         }

         // Needs to be an array not a list for .Length
         int[] intArray = intList.ToArray();

         // Classic nested loop comparision - Not the most effective. But understandable. Does mean we get the answer twice though        
         for (int i = 0; i < intArray.Length; i++)
         {
            for (int j = 0; j < intArray.Length; j++)
            {
               if (i != j)
               {
                  int sum = intArray[i] + intArray[j];
                  if (sum == 2020){
                     Console.WriteLine(intArray[i] * intArray[j]);
                     goto exitLoop1;
                  }
               }
            }
         }

         exitLoop1:

         for (int i = 0; i < intArray.Length; i++)
         {
            for (int j = 0; j < intArray.Length; j++)
            {
               for (int k = 0; k < intArray.Length; k++)
               {  
                  if (i != j && i != k)
                  {
                     int sum = intArray[i] + intArray[j] + intArray[k];
                     if (sum == 2020){
                        Console.WriteLine(intArray[i] * intArray[j] * intArray[k]);
                        goto exitLoop2;
                     }
                  }
               }
            }
         }
         exitLoop2:;
      }
   }
}