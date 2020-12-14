using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace advent_day_3 {
   class puzzle_3 {
      static void Main(string[] args) 
      {
         // Load input
         IEnumerable<string> lines = File.ReadLines("puzzle_3_input.txt");

         // Using a output file for debugging
         string outputFile = "puzzle_3_output.txt"; 
         // Which is wiped at the start of each run to avoid trawling through reams of output
         System.IO.File.WriteAllText(outputFile,string.Empty);

         int[] posArray = new int[] { 0, 0, 0, 0};
         int[] rightArray = new int[] { 1, 3, 5, 7};

         // Make tree Array 1 longer to handle the Down 2 results
         int[] treeArray = new int[] { 0, 0, 0, 0, 0};

         // Cheating method to get the Right 1 Down 2 in the same loop
         int line_count = 0;
         int pos2 = 0;
         int right2 = 1; 
         using(StreamWriter sw = File.AppendText(outputFile)) 
         {             
            // Down is 1 so we can just go line by line
            foreach (string r in lines)
            {
               // posArray and rightArray are both the same length so we can use the same index var i
               for (int i = 0; i < posArray.Length; i++){
                  // Tree count BEFORE moving - Otherwise we're behind if we start on a tree
                  if (r[posArray[i]] == '#'){
                     treeArray[i] += 1;
                  }

                  sw.WriteLine("Position -- {0} -- char {1}", posArray[i], r[posArray[i]]);
                  sw.WriteLine("Tree Count -- {0}", treeArray[i]);

                  // Move after count, ready for next line
                  posArray[i] += rightArray[i];

                  // If we enter "Here be dragons" modulo to get the rollover place. As if we were extending the line
                  if (posArray[i] >= r.Length){
                     posArray[i] %= r.Length;
                  }

               }

               if ((line_count % 2) == 0){
                  if (r[pos2] == '#'){
                     treeArray[4] += 1;
                  }

                  sw.WriteLine("Position -- {0} -- char {1}", pos2, r[pos2]);
                  sw.WriteLine("Tree Count (2 Down) -- {0}", treeArray[4]);

                  // Move after count, ready for next line
                  pos2 += right2;

                  // If we enter "Here be dragons" modulo to get the rollover place. As if we were extending the line
                  if (pos2 >= r.Length){
                     pos2 %= r.Length;
                  }
               } else{
                  // Debugging to make sure loop worked as expected
                  sw.WriteLine("Line Count-- {0}", line_count);
               }

               line_count +=1;
            }
         }
         // Long to handle the answer - Init to 1 to avoid a * 0 error
         long TreeProd = 1;
         foreach (int value in treeArray)
         {
            TreeProd *= value;
         }

         Console.WriteLine(string.Join(",", treeArray));
         Console.WriteLine("Total Tree Prod -- {0}", TreeProd);

      }
   }
}