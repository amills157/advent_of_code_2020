using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace advent_day_7 {
   class puzzle_7 {
      
      public static int dictComplierOne(StreamWriter sw, ref Dictionary <string, string> sorted_dict, ref Dictionary <string, string> dict){
         List<string> keyList = new List<string>(sorted_dict.Keys);

         sw.WriteLine(string.Join(" ", keyList));
         
         foreach (string key in keyList){
            List<string> temp = dict.Where(kvp => kvp.Value.Contains(key)).Select(kvp => kvp.Key).ToList();

            foreach (string value in temp){
               if (!(sorted_dict.ContainsKey(value))){
                  var dict_value = Regex.Match(dict[value], "(\\d+ "+ key +")");
                  sorted_dict.Add(value, "-> " + dict_value.Groups[0].Value + sorted_dict[key]);
            
                  dict.Remove(value);
               } else {
                  Console.WriteLine("Collision!");
               }
            }
            sw.WriteLine(key);
            sw.WriteLine(string.Join(" ", temp));
         }
         return sorted_dict.Count;
      } 

      public static List<string []> dictComplierTwo(List<string []> innerList, ref Dictionary <string, string> sg_dict, ref Dictionary <string, string> dict){

         List<string []> outerList = new List<string []>(innerList);

            innerList.Clear();
            
            foreach (string [] list in outerList)
            {
               foreach (string key in list)
               {
                  string dict_value = Regex.Replace(key, @"[\d-]", string.Empty).Trim();
                  if (dict.ContainsKey(dict_value+" "))
                  {
                     if(dict[dict_value+" "].Contains("no other bags")){
                        sg_dict[dict_value] = dict[dict_value+" "];
                     } else {
                        sg_dict[dict_value] = dict[dict_value+" "];
                        innerList.Add(dict[dict_value+" "].Split(new string[] {"contain", ",", "bags", "bag"}, StringSplitOptions.RemoveEmptyEntries));
                     }
                  }
               }            
            }

         return innerList;
      } 

      static void Main(string[] args) 
      {
         // Load input
         IEnumerable<string> wholeFile = File.ReadAllLines("puzzle_7_input.txt");

         // Dict to hold our initial input - Makes it easier to work through the data set and we don't have to keep allocating memory as we can access by ref in the methods
         Dictionary<string, string> dict = new Dictionary<string, string>();
         
         // Dict count vars used to manage the compilation of our sorted dict(s)
         int sgDictCount = 0;
         int refCount = 0;

         // Req for while loop(s)
         bool cont = true;

         // Using a output file for debugging
         string outputFile = "puzzle_7_output.txt"; 
         // Which is wiped at the start of each run to avoid trawling through reams of output
         System.IO.File.WriteAllText(outputFile,string.Empty);
         
         using(StreamWriter sw = File.AppendText(outputFile)) 
         {  
            foreach (string r in wholeFile)
            {              
               String[] strlist = r.Split("bags", 2);
               dict.Add(strlist[0],strlist[1]);             
            }

            //The initial population of the sorted dict - I need to try and merge this with the complier function - To revist

            List<string> keyList = dict.Where(kvp => kvp.Value.Contains("shiny gold")).Select(kvp => kvp.Key).ToList();


            Dictionary<string, string> sorted_dict = new Dictionary<string, string>();

            string test = "shiny gold";

            foreach (string key in keyList)
            {
               var dict_value = Regex.Match(dict[key], "(\\d+ "+ test +")");
               sorted_dict.Add(key, "-> " + dict_value.Groups[0].Value);
                  
               sw.WriteLine(key);
               sw.WriteLine(string.Join(" ", keyList));
            }

            // Eventually our dict count will stabalise because we've got all the relevant KVP added - This count becomes our answer
            while (cont){
               sgDictCount = dictComplierOne( sw, ref sorted_dict, ref dict);
               if (sgDictCount == refCount){
                  cont = false;
               } else {
                  refCount = sgDictCount;
               }
            }

            sw.WriteLine("-------------------");
            sw.WriteLine(string.Join(Environment.NewLine, sorted_dict.Select(a => $"{a.Key}: {a.Value}")));
            sw.WriteLine("-------------------");
   
            Console.WriteLine("Number of bags that can hold a shiny gold bag -- {0}",sorted_dict.Count);

            // Dict round 2 - To hold SG and child bags

            Dictionary<string, string> sg_dict = new Dictionary<string, string>();

            // My attempts at c# regex tailed off here - I need to revist
            String[] sgStrList = dict["shiny gold "].Split(new string[] {"contain", ",", "bags", "bag"}, StringSplitOptions.RemoveEmptyEntries);

            List<string []> innerList = new List <string []>();

            // As per above  the initialisation should be merged with the method - To revist
            foreach (string key in sgStrList)
            {
               
               string dict_value = Regex.Replace(key, @"[\d-]", string.Empty).Trim();
               /*
               var bag_count = Regex.Match(key, "(\\d+)");
               Console.WriteLine(bag_count.Groups[0].Value);
               */

               if (dict.ContainsKey(dict_value+" "))
               {
                  if(dict[dict_value+" "].Contains("no other bags")){
                     sg_dict[dict_value] = dict[dict_value+" "];
                  } else {
                     sg_dict[dict_value] = dict[dict_value+" "];
                     innerList.Add(dict[dict_value+" "].Split(new string[] {"contain", ",", "bags", "bag"}, StringSplitOptions.RemoveEmptyEntries));
                  }
               }
            }

            // Reset required vars for the second round - Should method this maybe(?)
            sgDictCount = 0;
            refCount = 0;
            cont = true;

            List<string []> outerList = new List<string []>();

            // Eventually our dict count will stabalise because we've got all the relevant KVP added - The count won't be our answer this time - But we can use the collated data to work out the number of bags 
            while (cont){
               outerList = dictComplierTwo(innerList, ref sg_dict, ref dict);
               sgDictCount = sg_dict.Count;
               if (sgDictCount == refCount){
                  cont = false;
               } else {
                  refCount = sgDictCount;
               }
            }
            
            // My attempts to auotmate the math on this one slowly drove me insane - In the end I resorted to a 'quick' pen and paper math (file included)
            Console.WriteLine("-------------------");
            Console.WriteLine(string.Join(Environment.NewLine, sg_dict.Select(a => $"{a.Key}: {a.Value}")));
            Console.WriteLine("-------------------");

            sw.WriteLine(string.Join(Environment.NewLine, sg_dict.Select(a => $"{a.Key}: {a.Value}")));

            Console.WriteLine(sg_dict.Count);

         }
      }
   }
}