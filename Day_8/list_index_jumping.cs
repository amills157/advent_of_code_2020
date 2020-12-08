using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace advent_day_7 {
   class puzzle_7 {
      
      //Only change here between part 1 and part 2 is the return of the idx value
      public static List<int> crashBoy(StreamWriter sw, ref List <string> fileList){
         int accCount = 0;
         int idx = 0;

         List<int> idxList = new List<int>();
         
         while (true){
            // Make sure we haven't gone off the reservation
            if(idx >= fileList.Count || idx < 0){
               break;
            }

            
            String[] strlist = fileList[idx].Split(" ", 2);

            sw.WriteLine("Index -- {0}, Line -- {1}", idx, fileList[idx]);
            
            // Split our instruction down into all its parts - capturing 2 groups in 1 for the regex kept causing issues, so lazy single captures
            string instruction = strlist[0];
            var op_match = Regex.Match(strlist[1], "([-+])");
            var int_match = Regex.Match(strlist[1], "(\\d+)");
            string op = op_match.Groups[0].Value;
            int amount = Int32.Parse(int_match.Groups[0].Value);

            if (idxList.Contains(idx)){
               break;
            } else {
               idxList.Add(idx);
            }
            
            // There is probably a way to avoid splitting the op into an if (given that + a - is still -) - But it seemed easier to if statement it
            switch (instruction) {
            case "acc":
               if( op == "+"){
                  accCount += amount;
               } else{
                  accCount -= amount;
               }
               idx += 1;
               break;
            case "jmp":
               if( op == "+"){
                  idx += amount;
               } else{
                  idx -= amount;
               }
               break;
            case "nop":
               idx += 1;
               break;
               default:
                  ;
               break;
            }
         }

         // For part 2 we need the index
         List<int> returnList = new List<int>{accCount, idx};
         return returnList;
      } 


      static void Main(string[] args) 
      {
         // Load input
         IEnumerable<string> wholeFile = File.ReadAllLines("puzzle_8_input.txt");

         List<string> fileList = new List<string>(wholeFile);

         // Using a output file for debugging
         string outputFile = "puzzle_8_output.txt"; 
         // Which is wiped at the start of each run to avoid trawling through reams of output
         System.IO.File.WriteAllText(outputFile,string.Empty);
         
         using(StreamWriter sw = File.AppendText(outputFile)) 
         {  
            
            List <int> returnList = crashBoy(sw, ref fileList);

            int accCount = returnList[0];
            int returnIdx = returnList[1];

            Console.WriteLine("We have accumilated -- {0}, terminating at Index {1}", accCount, returnIdx);

            for (int i = 0; i < fileList.Count; i++){

               String[] strlist = fileList[i].Split(" ", 2);

               string instruction = strlist[0];

               bool instructionChanged = false;

               if(instruction == "jmp" || instruction == "nop"){
                  string new_instruction = "";
                  switch (instruction) {
                     case "jmp":
                        new_instruction = "nop ";
                        break;
                     case "nop":
                         new_instruction = "jmp ";
                        break;
                        default:
                           ;
                        break;
                     }
                  sw.WriteLine("Before -- {0}", fileList[i]);
                  fileList[i] = new_instruction + strlist[1];
                  sw.WriteLine("After -- {0}", fileList[i]);
                  instructionChanged = true;
               } 

               if (instructionChanged){
                  // For debugging - Nice delimin for checking the output
                  sw.WriteLine("------------------------");
                  returnList = crashBoy(sw, ref fileList);
                  sw.WriteLine("------------------------");
                  accCount = returnList[0];
                  returnIdx = returnList[1];
                  if (returnIdx >= fileList.Count){
                     Console.WriteLine("We have accumilated -- {0}, terminating at Index {1}", accCount, returnIdx);
                     goto exitLoop;
                  } else {
                     // If we don't change the instructions back we shortcut to failure
                     fileList[i] = instruction + " " + strlist[1];
                  }
               }
            }
            exitLoop:;
         }
      }
   }
}