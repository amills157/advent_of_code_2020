using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace advent_day_24 {

   class puzzle_24 {

      public static Dictionary<(int x, int y), bool> partOne(StreamWriter sw, ref List <string> lines){

         Dictionary<(int x, int y), bool> tileDict = new Dictionary<(int x, int y), bool>();

         foreach(string line in lines){

            (int x, int y) tileLoc = (0, 0);

            for (int i = 0; i < line.Length; i++)
            {  

               switch (line[i]){
                  case 'e':
                     tileLoc.x += 1;
                     break;
                  case 'w':
                     tileLoc.x -= 1;
                     break;
                  case 'n':
                     tileLoc.y -= 1;
                     if (line[i + 1] == 'e'){
                        tileLoc.x += 1;
                     }
                     i++;
                     break;
                  case 's':
                     tileLoc.y += 1;
                     if (line[i + 1] == 'w'){
                        tileLoc.x -= 1;
                     }
                     i++;
                     break;
                  default:
                     ;
                     break;
               }
            }

            sw.WriteLine(tileLoc);

            // We can treat black and white as true and false - By setting a entry to it's not value we can flip them easily if they exist and we've landed back on them
            if(tileDict.ContainsKey(tileLoc)){
               tileDict[tileLoc] = !tileDict[tileLoc];
            }else{
               tileDict.Add(tileLoc, true);
            }
         }

         int total = 0;
         foreach (KeyValuePair<(int x, int y), bool> entry in tileDict){
            if (entry.Value){
               total++;
            }
         }

         Console.WriteLine("Part One Answer -- {0}", total);

         sw.WriteLine("-------------------");

         return tileDict;
      }


      public static void partTwo(StreamWriter sw, ref Dictionary<(int x, int y), bool> tileDict)
      {
         
         // Not every tile has its adjacent tiles populated - Add those in and default them to false / white
         // Because we're itterating through the tileDict we need to make a copy to push changes to
         Dictionary<(int x, int y), bool> tileDictCopy = new Dictionary<(int x, int y), bool>(tileDict);
         
         foreach (KeyValuePair<(int x, int y), bool> entry in tileDict)
         {
            // True = Black tile
            if (entry.Value){
               (int x, int y) tileLoc = entry.Key;
               for (int y = -1; y < 2; y++)
               {
                  for (int x = -1; x < 2; x++)
                  {
                     // If x == y it's either our tile loc or not adjacent 
                     if (y != x) {
                        if (!tileDictCopy.ContainsKey((tileLoc.x + x, tileLoc.y + y)))
                        {
                           tileDictCopy.Add((tileLoc.x + x, tileLoc.y + y), false);
                           sw.WriteLine("{0} - {1}", tileLoc.x + x, tileLoc.y + y);
                        }
                     }
                  }
               }
            }
                              
         }
         // Push changes back to our original tileDict
         tileDict = tileDictCopy;

         for (int i = 0; i < 100; i++){

            // As we're looping through the tileDict we need to make chanegs to a tempDict - We can't re-use the copy above as we need to re-set it at the start of each loop
            Dictionary<(int x, int y), bool> tempDict = new Dictionary<(int x, int y), bool>(tileDict);
                        
            foreach (KeyValuePair<(int x, int y), bool> entry in tileDict)
            {
               int blackTileCount = 0;
               (int x, int y) tileLoc = entry.Key;

               for (int y = -1; y < 2; y++)
               {
                  for (int x = -1; x < 2; x++)
                  {
                     if (y != x) {

                        (int x, int y) checkLoc = (tileLoc.x + x, tileLoc.y + y);
                        
                        if (tileDict.ContainsKey(checkLoc) && tileDict[checkLoc])
                        {
                           blackTileCount+=1;
                           sw.WriteLine("Black tile {0}", checkLoc);

                        } else if (!tempDict.ContainsKey(checkLoc)){

                           tempDict.Add(checkLoc, false);
                           sw.WriteLine("New tile {0}", checkLoc);
                        }
                     }
                  }
               }
                              
               if (tileDict[tileLoc]){
                  if (blackTileCount == 0 || blackTileCount > 2){
                     sw.WriteLine("Flipped to white -- {0}",blackTileCount);
                     tempDict[tileLoc] = false;
                  }
               }else{
                  if (blackTileCount == 2){
                     sw.WriteLine("Flipped to black -- {0}",blackTileCount);
                     tempDict[tileLoc] = true;
                  }
               }
            }

            tileDict = tempDict;
         }

         int total = 0;
            foreach (KeyValuePair<(int x, int y), bool> entry in tileDict)
            {
               if (entry.Value){
                  total+=1;
               }
            }
            
         Console.WriteLine("Part Two Answer -- {0}",total);
      }
        
         
      static void Main(string[] args){

         // Load input
         List <string> lines = File.ReadLines("puzzle_24_input.txt").ToList();

         // Using a output file for debugging
         string outputFile = "puzzle_24_output.txt"; 
         // Which is wiped at the start of each run to avoid trawling through reams of output
         System.IO.File.WriteAllText(outputFile,string.Empty);

         using(StreamWriter sw = File.AppendText(outputFile))
         {         
            Dictionary<(int x, int y), bool> tileDict = partOne(sw, ref lines);

            partTwo(sw, ref tileDict);

            

         }
      }
   }
}