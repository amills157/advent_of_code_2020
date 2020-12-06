using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace advent_day_6 {
   class puzzle_6 {
      static void Main(string[] args) 
      {
         // Load input
         IEnumerable<string> wholeFile = File.ReadAllLines("puzzle_6_input.txt");

         // Used HashSets here in part 1 - This became problematic in part 2 due to the de-dup so revert back to Lists
         List<string> temp = new List<string>();
         List<string> forms = new List<string>();
         
         int lineCount = 0;

         // Using a output file for debugging
         string outputFile = "puzzle_6_output.txt"; 
         // Which is wiped at the start of each run to avoid trawling through reams of output
         System.IO.File.WriteAllText(outputFile,string.Empty);

         
         using(StreamWriter sw = File.AppendText(outputFile)) 
         {  

            foreach (string r in wholeFile)
            {
               if (r == ""){
                  temp.Add('-'+lineCount.ToString());
                  string form = string.Join("", temp);
                  forms.Add(form);

                  temp.Clear();
                  lineCount = 0;

               } else{
                  temp.Add(r);
                  lineCount += 1;
               }
            }

            // <insert DBZ joke here>
            temp.Add('-'+lineCount.ToString());
            string final_form = string.Join("", temp);
            forms.Add(final_form);

            int anyCount = 0;

            int allCount = 0;

            foreach (string r in forms)
            {  
               String[] strlist = r.Split('-');
               
               // LINQ get the unique chars from each form
               var uniqueCharArray = strlist[0].ToCharArray().Distinct().ToArray();

               anyCount += uniqueCharArray.Length;
               sw.WriteLine("Line -- {0} ", r);

               foreach (char c in uniqueCharArray){
                  // LINQ get the char count in the string
                  int occuranceCount = strlist[0].Count(o => (o == c));
                  if (occuranceCount == Int32.Parse(strlist[1])){
                     allCount += 1;
                     sw.WriteLine("Char -- {0} Line Count -- {1} Char Count -- {2}", c, strlist[1], occuranceCount);
                  }
                  
               }               
            }
            Console.WriteLine("Sum of Any Count -- {0}", anyCount);
            Console.WriteLine("Sum of All Count -- {0}", allCount);

            sw.WriteLine("Sum of Any Count -- {0}", anyCount);
            sw.WriteLine("Sum of All Count -- {0}", allCount);
         }
      }
   }
}