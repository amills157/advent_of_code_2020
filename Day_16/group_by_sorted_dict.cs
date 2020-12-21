using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace advent_day_16 {
   class puzzle_16 {     

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
         List <string> lines = File.ReadLines("puzzle_16_input.txt").ToList();

          // Using a output file for debugging
         string outputFile = "puzzle_16_output.txt"; 
         // Which is wiped at the start of each run to avoid trawling through reams of output
         System.IO.File.WriteAllText(outputFile,string.Empty);

         Dictionary<string, int []> ticketDict = new Dictionary<string, int []>();

         List<int []> nearbyTickets = new List<int []>();

         int [] myTicket = {0};

         using(StreamWriter sw = File.AppendText(outputFile)) 
         {  
            
            int nearbyTicketsLine = 0;

            for(int i = 0; i < lines.Count; i++){
               
               if(nearbyTicketsLine != 0){
                  nearbyTicketsLine = i;
                  string stringInts = string.Join(",", Regex.Matches(lines[nearbyTicketsLine], @"-?\d+").Select(m => m.Value)).Replace("-", string.Empty);

                  int [] splitInts = stringInts.Split(",").Select(int.Parse).ToArray(); 

                  nearbyTickets.Add(splitInts);
               }

               if (lines[i].Contains("or")){
                  string [] splitStrings = lines[i].Split(":");

                  string stringInts = string.Join(",", Regex.Matches(splitStrings[1], @"-?\d+").Select(m => m.Value)).Replace("-", string.Empty);

                  int [] splitInts = stringInts.Split(",").Select(int.Parse).ToArray(); 

                  ticketDict[splitStrings[0]] = splitInts;
                                 
               }else if (lines[i].Contains("your")){
                  string stringInts = string.Join(",", Regex.Matches(lines[i+1], @"-?\d+").Select(m => m.Value)).Replace("-", string.Empty);

                  int [] splitInts = stringInts.Split(",").Select(int.Parse).ToArray(); 

                  myTicket = splitInts;

               } else if (lines[i].Contains("nearby")){
                  nearbyTicketsLine = i;
               } else {
                  ;
               }
            }

            List<int []> validTickets  = partOne( sw, ref  nearbyTickets, ref ticketDict);

            nearbyTickets.Clear();
            
            partTwo( sw, ref validTickets, ref ticketDict, ref myTicket);

         }

      }
   }
}