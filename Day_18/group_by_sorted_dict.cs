using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace advent_day_18 {
   class puzzle_18 {     

      public static  void partTwo(StreamWriter sw, ref  List<int []> validTickets, ref Dictionary<string, int []> ticketDict, ref int [] myTicket){

         Dictionary<string, List<int>> fieldDict = new Dictionary<string, List<int>> ();

         foreach(KeyValuePair<string, int []> entry in ticketDict){

            foreach(int [] arr in validTickets){

               for(int i = 0; i < arr.Length; i++){

                  sw.WriteLine("{0} : {1}", entry.Key, string.Join(" ", entry.Value));
            
                  if(( arr[i]>= entry.Value[0] && arr[i] <= entry.Value[1]) || ( arr[i]>= entry.Value[2] && arr[i]<= entry.Value[3])){
                     if (fieldDict.ContainsKey(entry.Key)){
                        fieldDict[entry.Key].Add(i);
                     } else {
                        fieldDict[entry.Key] = new List<int>(); 
                        fieldDict[entry.Key].Add(i);
                     }
                     
                  } 
                  
               }               
            }
         }

         foreach(KeyValuePair<string, List<int>> entry in fieldDict){
            sw.WriteLine("{0} : {1}", entry.Key, string.Join(" ", entry.Value));

            List <int> temp = new List <int>(fieldDict[entry.Key]);

            fieldDict[entry.Key].Clear();

            var groups = temp.GroupBy( i => i );
            
            foreach( var grp in groups )
            {  
               if(grp.Count() == validTickets.Count){
                  sw.WriteLine( "{0}", grp.Key);
                  fieldDict[entry.Key].Add(grp.Key);
               }
               
            }

         }

         IOrderedEnumerable<KeyValuePair<string, List<int>>> sortedDict = fieldDict.OrderBy(i => i.Value.Count).ThenBy(i => i.Key);

         List<int> depIndexs = new List <int>();

         for(int i = 0; i < fieldDict.Count; i++){
            
            try{
               var diffList = sortedDict.ElementAt(i+1).Value.Except(sortedDict.ElementAt(i).Value).ToList();
            
               sw.WriteLine("{0} = {1}", sortedDict.ElementAt(i+1).Key, string.Join("", diffList));

               if(sortedDict.ElementAt(i+1).Key.Contains("departure")){
                  depIndexs.Add(diffList[0]);
               }
            } catch (Exception e){
               sw.WriteLine(e);
            }
            
         }

         long total = 1;

         foreach(int i in depIndexs){

            total *= myTicket[i];
         }

         Console.WriteLine("Part Two Aswer {0}",total);

      } 
          

      public static  List<int []> partOne(StreamWriter sw, ref  List<int []> nearbyTickets, ref Dictionary<string, int []> ticketDict){

         int invalidTotal = 0;

         List<int []> validTickets = new List<int []>();
            
         foreach(int [] arr in nearbyTickets){

            bool validTicket = true;
            
            sw.WriteLine("{0}", string.Join(" ", arr));
            foreach(int i in arr){

               bool valid = false;
               
               foreach(KeyValuePair<string, int []> entry in ticketDict){

                  sw.WriteLine("{0} : {1}", entry.Key, string.Join(" ", entry.Value));
            
                  if(( i>= entry.Value[0] && i <= entry.Value[1]) || ( i>= entry.Value[2] && i<= entry.Value[3])){
                     valid = true;
                  } 

               }

               if (!(valid)){
                  invalidTotal += i;
                  validTicket = false;
               }
            }

            if (validTicket){
                  validTickets.Add(arr);
               }

         }

         Console.WriteLine("Part One Answer {0}", invalidTotal);

         return validTickets;

      }
      
      static void Main(string[] args) 
      {
         // Load input
         List <string> lines = File.ReadLines("puzzle_18_test_input.txt").ToList();

          // Using a output file for debugging
         string outputFile = "puzzle_16_output.txt"; 
         // Which is wiped at the start of each run to avoid trawling through reams of output
         System.IO.File.WriteAllText(outputFile,string.Empty);


         using(StreamWriter sw = File.AppendText(outputFile)) 
         {  
            foreach(string r in lines){
               Console.WriteLine(r);
               var groups = Regex.Match(r, @"(\(([^)]*)\))+").Groups;

               foreach( var grp in groups ){
                  Console.WriteLine(grp);
               }

               Console.WriteLine("-------------------------");
            }

         }

      }
   }
}