using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace advent_day_5 {
   class puzzle_5 {
      static void Main(string[] args) 
      {
         // Load input
         IEnumerable<string> lines = File.ReadLines("puzzle_5_input.txt");

         // Using a output file for debugging
         string outputFile = "puzzle_5_output.txt"; 
         // Which is wiped at the start of each run to avoid trawling through reams of output
         System.IO.File.WriteAllText(outputFile,string.Empty);

         using(StreamWriter sw = File.AppendText(outputFile)) 
         {  
            
            List<int> seats = new List<int>();

            foreach (string r in lines)
            {
               int max = 127;
               int min = 0;
               int column_max = 7;
               int column_min = 0;
               foreach (char c in r){
                  int difference = ((max - min) / 2) +1;
                  int column_difference = ((column_max - column_min) / 2) +1;
                  if (c == 'F'){
                     max -= difference;
                  }
                  if (c == 'B'){
                     min += difference;
                  }
                  if (c == 'R'){
                     column_min += column_difference;
                  }
                  if (c == 'L'){
                     column_max -= column_difference;
                  }
                  sw.WriteLine("Char {0} Seat Min - {1}, Seat Max - {2}, C Min - {3} , C Max - {4}", c, min, max, column_min, column_max);
               }

               int seat_id = max * 8 + column_max;

               sw.WriteLine("Seat ID -- {0}", seat_id);
               sw.WriteLine("-------");

               seats.Add(seat_id);
               
            }
            Console.WriteLine("Highest seat ID -- {0}", seats.Max());

            int min_poss_seat = seats.Min();
            int max_poss_seat = seats.Max();
            for (int i = min_poss_seat; i <= max_poss_seat; i++){
               if (!(seats.Contains(i))){
                  Console.WriteLine("Possible seat -- {0}", i);
               }
            }
            
         }
      }
   }
}