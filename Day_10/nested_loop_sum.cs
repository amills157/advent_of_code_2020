using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace advent_day_10 {
   class puzzle_10 {
      public static List <int> comparisionLoop(StreamWriter sw, ref int[] intArray){
         
         // From 0 to 1
            int countOne = 1;
            // Our final adapter (Dun a na na)
            int countThree = 1;

            // For our 0 - 1 diff (required for part 2)
            List <int> diffList = new List<int>{1};

            for (int i = 0; i < (intArray.Length -1); i++)
            {
               int diff = (intArray[i+1] - intArray[i]);
               diffList.Add(diff);

               if(diff == 1){
                  countOne += 1;
               }

               if(diff == 3){
                  countThree += 1;
               }

               sw.WriteLine("-------------------------------");
               sw.WriteLine("Int at idx {0} -- {1}",i, intArray[i]);
               sw.WriteLine("Int at idx {0} -- {1}",(i+1), intArray[i+1]);
               sw.WriteLine("Difference -- {0}",diff);
               sw.WriteLine("-------------------------------");
               
            }

            sw.WriteLine("Number of 1 differences -- {0}", countOne);

            sw.WriteLine("Number of 3 differences -- {0}", countThree);

            Console.WriteLine("Answer (the first) -- {0}", (countOne * countThree));

         return diffList;
      } 


      public static long comparisionLoop2(StreamWriter sw, ref List<int> diffList){
         
         sw.WriteLine(string.Format("Diff list: ({0}).", string.Join("", diffList)));

         // Some of this is probably very inefficent - Re visit

         // We don't need to know the numbers, just the differences
         string temp = string.Join("", diffList);

         // We split our string on 3 as we're only interested in contiguous sets of numbers
         string [] sets = temp.Split(new string[] { "3" }, StringSplitOptions.RemoveEmptyEntries);

         List <int> products = new List<int>();

         foreach(string i in sets){
            //Remove empty above didn't seem to work, so safety measure
            i.Replace(" ", string.Empty);

            /* If we have a group of more than 3 differences it means we have more than 4 numbers.

            A group of 4 (contiguous) numbers has a combo of 4
            {4,5,6,7}
            4 - 7
            4,5,6,7
            4,6,7
            4,5,7

            For each number we add we get an additional 3 combos
            {4,5,6,7,8}
            5-8
            5,6,8
            5,7,8

            So for each group of differences greater than 3 we have 4 + numbers. we know 4 = 7 and +1 = +3 so we can simply add the diffrence * 3 to our original 4

            */
            if(i.Length > 3){
               int difference = i.Length - 3;
               products.Add( 4 + (difference * 3));

            // The rest are just switch statements as they are 'simples'
            } else {
               switch (i.Length) {
                  case 1:
                     products.Add(1);
                     break;
                  case 2:
                     products.Add(2);
                     break;
                  case 3:
                     products.Add(4);
                     break;
                     default:
                        ;
                     break;
               }
            }
         }

         sw.WriteLine(string.Format("Product list: ({0}).", string.Join("", products)));

         // Our answer is the product of our difference groups
         long prod =1;

         foreach(int i in products) {
            prod = prod*i;
         }

         return prod;
      }


      static void Main(string[] args) 
      {
         // Load input
         IEnumerable<string> lines = File.ReadLines("puzzle_10_input.txt");

          // Using a output file for debugging
         string outputFile = "puzzle_10_output.txt"; 
         // Which is wiped at the start of each run to avoid trawling through reams of output
         System.IO.File.WriteAllText(outputFile,string.Empty);

         List <int> intList = new List<int>();

         // Convert each string int to int
         foreach (string r in lines)
         {  
            intList.Add(Convert.ToInt32(r));
            
         }
                  
         using(StreamWriter sw = File.AppendText(outputFile)) 
         { 

            intList.Sort();

            sw.WriteLine(string.Format("Sorted list: ({0}).", string.Join(", ", intList)));

            // Needs to be an array not a list for .Length
            int[] intArray = intList.ToArray();

            List <int> diffList = comparisionLoop(sw, ref intArray);

            long combinations = comparisionLoop2(sw, ref diffList);  

            Console.WriteLine("Answer (the second) -- {0}", combinations); 

         }

      }
   }
}