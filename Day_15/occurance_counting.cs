using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace advent_day_15{
   class puzzle_15 {

      public static void partTwo(StreamWriter sw, List<int> startingInts){
         
         // Chnage from a list to a dictionary approach
         Dictionary<int, List<int>> memDict = new Dictionary<int, List<int>>();
         List<int> turnsSpoken = new List <int>();
         
         int lastSpoken = -1;
         int nextSpoken = 0;

         int turns = 0;
         // Because our "startingInts" now include all the ints up to turn 2020 (after running PartOne) our dictionary gets pre-populated with all those values and turns. 
         for(int i = 0; i<startingInts.Count;i++)
         {
               turns++;
               //Clearing the list seems to cause issues, so re-set the list using new for each turn
               turnsSpoken = new List <int>();

               if (memDict.ContainsKey(startingInts[i])){
                  turnsSpoken = memDict[startingInts[i]];
               } 

               if (turnsSpoken.Count == 0){
                  // New spoken int so we need to add a new list with the current turn
                  memDict[startingInts[i]] = new List<int>(new int[] { turns });
               }
               else if (turnsSpoken.Count == 1){
                  // If we've only got one value for our lastSpoken int we need to add the most recent turn
                  turnsSpoken.Add(turns);
               }
               else{
                  // We actually only care about the last two turns - Storing everything just slows down the script / runtime and takes up memory. The last turn gets bumped down and we add in the current turn as the most recent
                  turnsSpoken[0] = turnsSpoken[1];
                  turnsSpoken[1] = turns;
               }

               lastSpoken = startingInts[i];
         }

         while(turns < 30000000)
         {
            turns++;

            if (memDict[lastSpoken].Count == 1)
            {
               nextSpoken = 0;
            }
            else
            {
               nextSpoken = memDict[lastSpoken][1] - memDict[lastSpoken][0];
            }

            // I could / should functionalise this - But it's a fairly small if statement section - Will re-visit
            turnsSpoken = new List <int>();

            if (memDict.ContainsKey(nextSpoken)){
               turnsSpoken = memDict[nextSpoken];
            } 

            if (turnsSpoken.Count == 0){
               memDict[nextSpoken] = new List<int>(new int[] { turns });
            }
            else if (turnsSpoken.Count == 1){
               turnsSpoken.Add(turns);
            }
            else{
               turnsSpoken[0] = turnsSpoken[1];
               turnsSpoken[1] = turns;
            }

            // Our nextSpoken from this turn becomes our next turn lastSpoken
            lastSpoken = nextSpoken;
         }

         Console.WriteLine("{0} -- {1}", turns, lastSpoken);
      }

      // Originally this was just the whole script and I changed the param for turns == . It turns out a brute force attempting using the below takes longer than I am willing to have it run. But I kept this as the part one rather than re-write the whole thing because it was the "initial" implementation and did work for part one
      public static void partOne(StreamWriter sw, List<int> startingInts){

         int turns = 0;

         while(turns < 2020){
            
            // Increment turns at the start to move away from 0 index - Otherwise our "turns" are out of sync
            turns +=1;

            if (turns-1 >= startingInts.Count){
               int lastSpoken = startingInts[turns-2];

               int occurances = startingInts.Where(x => x.Equals(lastSpoken)).Count();

               if (occurances > 1){
                  List <int> results = Enumerable.Range(0, startingInts.Count).Where(i => startingInts[i] == lastSpoken).ToList();

                  results.Reverse();

                  int turn = results[0] + 1;

                  int turn_1 = results[1] + 1;
                  
                  startingInts.Add(turn - turn_1);

               } else {
                  startingInts.Add(0);
               }

            }

            // Debugging output was left on for partone as the slowdown wasn't significant enough
            sw.WriteLine("{0} -- {1}", turns, startingInts[turns-1]);

         }
         Console.WriteLine("{0} -- {1}", turns, startingInts[turns-1]);
      }       

      static void Main(string[] args) 
      {
         // Load input
         var lines = File.ReadAllLines("puzzle_15_input.txt");

         string [] temp = lines[0].Split(",");

         List<int> startingInts = temp.Select(idString => new { ParseSuccessful = Int32.TryParse(idString, out var id), Value = id }).Where(id => id.ParseSuccessful).Select(id => id.Value).ToList();

         //var listOfInts = temp.Select<string, int>(q => Convert.ToInt32(q));

          // Using a output file for debugging
         string outputFile = "puzzle_15_output.txt"; 
         // Which is wiped at the start of each run to avoid trawling through reams of output
         System.IO.File.WriteAllText(outputFile,string.Empty);

         using(StreamWriter sw = File.AppendText(outputFile)) 
         {
            
            partOne(sw, startingInts);

            partTwo(sw, startingInts);
         
         }
            

      }
   }
}