using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace advent_day_21 {
   class puzzle_21 {

      public static Dictionary<string, HashSet<string>> partOne(StreamWriter sw, ref List <string> lines){

          Dictionary<string, HashSet<string>> allergenDict = new Dictionary<string, HashSet<string>>();

            List<string> ingredeintsList = new List<string>();
            
            foreach (string line in lines){

            string [] allergens = Regex.Match(line, @"(?<=\().+?(?=\))").Value.Replace("contains", string.Empty).Replace(" ", string.Empty).Split(",");

            sw.WriteLine(string.Join(" ", allergens));

            string [] temp = line.Split("(");

            string[] ingredients = temp[0].Split(" ", StringSplitOptions.RemoveEmptyEntries);

            sw.WriteLine(string.Join(" ", ingredients));

            // Create a list which contains all ingrendients - Populated by each line by line ingredient array

            // Needs to be a List not HashSet as we want duplictae values in this instance 
            foreach (string ingredient in ingredients){

               ingredeintsList.Add(ingredient);
            }


            foreach (string allergen in allergens){
               //  if it exists already whittle down the potential allergens
               if (allergenDict.ContainsKey(allergen)){

                  // Use of IntersectsWith - Returns a HashSet which only contains values found in both the original HashSet and the passed object
                  allergenDict[allergen].IntersectWith(ingredients);

               //  otherwise, create a new dictionary entry
               }else{

                  allergenDict[allergen] = new HashSet<string>(ingredients);

               }
            }
         }

         foreach (KeyValuePair <string, HashSet<string>> entry in allergenDict)
         {
            foreach (string ingredient in entry.Value){
               ingredeintsList.RemoveAll(x => x == ingredient);
            }
         }

         Console.WriteLine("Part One Answer -- {0}", ingredeintsList.Count);

         return allergenDict;
      }

      public static void partTwo(StreamWriter sw, ref Dictionary<string, HashSet<string>> allergenDict){
         // A sort of List / Dictionary HyBrid - Allows us to have KeyPairValues in a List
         List<(string, HashSet<string>)> allergensDictList = new List<(string, HashSet<string>)>();

         foreach(KeyValuePair <string, HashSet<string>> entry in allergenDict)
         {
            allergensDictList.Add((entry.Key, entry.Value));
         }
         
         SortedDictionary <string, string> dangerousIngredients = new SortedDictionary <string, string> ();
         
         // This means we can loop through our Dictionary List
         while (allergensDictList.Count > 0){
            for (int i = 0; i < allergensDictList.Count; i++){
               // If our allergen / ingredient pair count is 1 then we can just add it to our final / dangerous dict
               if (allergensDictList[i].Item2.Count == 1){
                  dangerousIngredients[allergensDictList[i].Item1] = allergensDictList[i].Item2.ElementAt(0);
                  // Then remove that ingredient from all remaining allergen / ingredeint KVPs
                  for (int j = 0; j < allergensDictList.Count; j++){
                     if (j != i){
                        if (allergensDictList[j].Item2.Contains(allergensDictList[i].Item2.ElementAt(0))){
                           allergensDictList[j].Item2.Remove(allergensDictList[i].Item2.ElementAt(0));
                        }
                     }
                  }
                  // Finally remove the entry itself - This is where working with a Dictionary fell over / got complex as we needed a seperate KeyList and it kept returning key not found errors etc
                  allergensDictList.RemoveAt(i);
                  break;
               }
            }
         }
         
         Console.WriteLine("Danger Will Robinson! Please Avoid:");

         Console.WriteLine(string.Join(",", dangerousIngredients.Values.ToList()));
      } 
      
      static void Main(string[] args) 
      {
         // Load input
         List <string> lines = File.ReadLines("puzzle_21_input.txt").ToList();

         // Using a output file for debugging
         string outputFile = "puzzle_21_output.txt"; 
         // Which is wiped at the start of each run to avoid trawling through reams of output
         System.IO.File.WriteAllText(outputFile,string.Empty);

         using(StreamWriter sw = File.AppendText(outputFile)) 
         {  
                       
            Dictionary<string, HashSet<string>> allergenDict = partOne(sw, ref lines);

            partTwo(sw, ref allergenDict);

         }

      }
   }
}