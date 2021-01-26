using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace advent_day_25 {

   class puzzle_25 {
      
      public static void partOne(StreamWriter sw, ref List<string> lines){

         long doorPubKey = Convert.ToInt64(lines[0]);
         long cardPubKey = Convert.ToInt64(lines[1]);
                              
         int doorLoop = 0;
         int cardLoop = 0;

         int loops = 1;
         int subjectNum = 1;

         while (true)
         {
            subjectNum = (subjectNum * 7) % 20201227;

            if (subjectNum == cardPubKey ){
               cardLoop = loops;
            }else if (subjectNum == doorPubKey){
               doorLoop = loops;
            }

            if (doorLoop != 0 && cardLoop != 0){ 
               break;
            }

            loops++;
         }

         sw.WriteLine("Card loop = {0}\nDoor loop = {1}",cardLoop, doorLoop);

         long encryptionKey = 1;

         for (long i = 0; i < cardLoop; i++)
         {
            encryptionKey = (encryptionKey * doorPubKey) % 20201227;
         }

         Console.WriteLine("Encryption key = {0}", encryptionKey);
      }
        
         
      static void Main(string[] args){

         // Load input
         List <string> lines = File.ReadLines("puzzle_25_input.txt").ToList();

         // Using a output file for debugging
         string outputFile = "puzzle_25_output.txt"; 
         // Which is wiped at the start of each run to avoid trawling through reams of output
         System.IO.File.WriteAllText(outputFile,string.Empty);

         using(StreamWriter sw = File.AppendText(outputFile))
         {         
            partOne(sw, ref lines);

         }
      }
   }
}