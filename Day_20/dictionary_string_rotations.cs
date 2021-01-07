using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace advent_day_20 {
   class puzzle_20 {

      public static int seaMonsterScan(StreamWriter sw, ref string[] photo)
      {
         
         // If our tail is at 0, 0 then these would be the coords for the rest of the monster
         int [] yOffsets = {1, 1, 0, 0, 1, 1, 0, 0, 1, 1, 0, 0, 0, -1};
         int [] xOffsets = {1, 4, 5, 6, 7, 10, 11, 12, 13, 16, 17, 18, 19, 18};

         int seaMonsterCount = 0;
         //  Count one up from the bottom or our -1 will break
         for (int y = 1; y < photo.Length - 1; y++)
         {
            // Need to leave room for complete sea monster or we get index error if we try using newX and newY beyond that point
            for (int x = 0; x < photo[y].Length - 19; x++){  
            
               // Grab a Sea Monster by it's tail
               if (photo[y][x] == '#'){

                  int count = 0;

                  // I tried doing this with a bool - But I kept getting false positivies and a massively inflated count - So this instead
                  for(int i = 0; i < yOffsets.Length; i ++){
                     int newY = y + yOffsets[i];
                     int newX = x + xOffsets[i];
                     if (photo[newY][newX] == '#'){
                        count += 1;
                     }
                  }

                  if(count == yOffsets.Length){
                     // Get some read only errors here - But would be a fun one to implement - Will revist
                     /*photo[y][x] = '*';
                     for(int i = 0; i < yOffsets.Length; i ++){
                        int newY = y + yOffsets[i];
                        int newX = x + xOffsets[i];
                        photo[newY][newX] = '*';
                     }*/
                  
                     seaMonsterCount++;
                     //  Manual debug for overlap - Turns out I had mismatched thr X and Y offsets - d'oh
                     sw.WriteLine($"Found sea monster at row {y}, col {x}");
                  } 
               }
            }
         }
         return seaMonsterCount;
      }

      // Let the hunt begin!
      public static void monsterHunt(StreamWriter sw, int sqrt, ref Dictionary<int, List<string>> tileDict, ref int[,] arrangedTiles){

         // Create a photo from our correctly arranged tiles - The dimensions for this won't include the boarders (top, left, right and bottom)
         string[] photo = new string[sqrt * 8];

         for (int y = 0; y < sqrt; y++){

            string[] fullRow = new string[8]; 

            for (int x = 0; x < sqrt; x++)
            {
               for (int i = 1; i < 9; i++)
               {
                  if (x == 0){
                     fullRow[i - 1] = "";
                  }
                  string temp = tileDict[arrangedTiles[x, y]][i];
                  fullRow[i - 1] += temp.Substring(1, 8); 
               }
               
            }

            for (int i = 0; i < 8; i++)
            {  
               photo[8 * y + i] = fullRow[i];
               sw.WriteLine(fullRow[i]);
            }
         }

         int seaMonsterCount = 0;
         for (int i = 0; i < 8; i++){  
            
            // Once we have created the photo we need to scan it to find our sea monsters
            seaMonsterCount = seaMonsterScan(sw, ref photo);

            // Because all the sea monsters are facing the right direction once we have found one we can exit the loop
            if(seaMonsterCount == 0){
               // If we have gone around 4 times we need to flip the photo 
               if (i == 4){
                  photo = flipTile(photo);
               }else{
                  photo = rotateTile(photo, 1);
               }
            } else {
               sw.WriteLine("Found {0} sea monsters after {1} rotations", seaMonsterCount, i);
               break;
            }            
         }

         int totalHashes = 0;

         // Count all the hashes in our photo
         foreach(string str in photo){
            totalHashes += str.Count(x => x == '#');
         }

         sw.WriteLine("Total hashes -- {0}", totalHashes);

         // Takeway the number of sea monsters multipled by how many hashes each sea monster contains
         int partTwoAnswer = totalHashes - (seaMonsterCount * 15);

         Console.WriteLine("Part Two Answer -- {0}", partTwoAnswer);
                  
      }

      // Flips the tile - We don't need to bother with multiple flips (in a row)
      public static string[] flipTile(string[] tileData)
      {
         string[] flippedTile = new string[tileData.Length];
         for (int i = 0; i < tileData.Length; i++)
         {
               flippedTile[tileData.Length - 1 - i] = tileData[i];
         }
         return flippedTile;
      }
      
      // Rotates clockwise / 90 degress x number of times - Need to revist to deal with the mismatch between array and list if possible
      public static string[] rotateTile(string[] tileData, int rotations){
         
         // Copy the input data
         string[] temp = tileData.Clone() as string[];

         for(int i = 0; i< rotations; i++){
            
            // Create a rotatedTile the same size as the input tile
            string[] rotatedTile = new string[temp.Length];
            
            // Loop through creating new rows from the existing tile data
            for (int x = 0; x < temp.Length; x++)
            {
               string newRow = "";
               for (int y = temp.Length - 1; y >=0 ; y--)
               {
                  newRow += temp[y][x];
               }
               rotatedTile[x] = newRow;
            }

            // Clear the existing input data and then set it to our rotated tile - Allows multiple rotations
            Array.Clear(temp, 0, temp.Length);
            temp = rotatedTile;

         }

         // Returns the rotated tile NOT the original input
         return temp;
      }

      //We approach this from either the X or Y approach
      public static int reorientateTile(StreamWriter sw, int alreadyPlacedTileName, ref Dictionary<int, List<string>> tileDict, ref Dictionary<int, List<int>> matchedtileEdgesDict, string mode ){
         
         int reorientedTileName = -1;
         //  get the correct tile from the possible connected ones
         foreach (int tileName in matchedtileEdgesDict[alreadyPlacedTileName])
         {

            string[] reorientedTile = new string[10];

            List<string> connection = getTileEdgesByType(sw, alreadyPlacedTileName, tileName, ref tileDict);
            
            // If X we are working through the row from lfet to right - So only want to reorientate tiles which are to the right of ours (connection by edge type) 
            if(mode == "X"){

               if (connection[0] == "right")
               {
                  reorientedTileName = tileName;
                  // Depending on the edge that connects the other tile we rotate or flip as required
                  switch (connection[1])
                  {
                     case "top":
                        reorientedTile = flipTile(tileDict[reorientedTileName].ToArray());
                        reorientedTile = rotateTile(reorientedTile, 1);
                        break;
                     case "right":
                        reorientedTile = flipTile(tileDict[reorientedTileName].ToArray());
                        reorientedTile = rotateTile(reorientedTile, 2);
                        break;
                     case "bottom":
                        reorientedTile = rotateTile(tileDict[reorientedTileName].ToArray(), 1);
                        break;
                     case "left":
                        reorientedTile = tileDict[reorientedTileName].ToArray();
                        break;
                     case "-top":
                        reorientedTile = rotateTile(tileDict[reorientedTileName].ToArray(), 3);
                        break;
                     case "-right":
                        reorientedTile = rotateTile(tileDict[reorientedTileName].ToArray(), 2);
                        break;
                     case "-bottom":
                        reorientedTile = flipTile(tileDict[reorientedTileName].ToArray());
                        reorientedTile = rotateTile(reorientedTile, 3);
                        break;
                     case "-left":
                        reorientedTile = flipTile(tileDict[reorientedTileName].ToArray());
                        break;

                  }
                  // We then replace the existing tile value with the reorientated version
                  tileDict[reorientedTileName] = reorientedTile.ToList();
                  goto exitLoop;

               }

            // Y mode, working from top to bottom
            }else{

               if (connection[0] == "bottom"){

                  reorientedTileName = tileName;

                  switch (connection[1])
                  {
                     case "top":
                        reorientedTile = tileDict[reorientedTileName].ToArray();
                        break;
                     case "right":
                        reorientedTile = rotateTile(tileDict[reorientedTileName].ToArray(), 3);
                        break;
                     case "bottom":
                        reorientedTile = flipTile(tileDict[reorientedTileName].ToArray());
                        break;
                     case "left":
                        reorientedTile = flipTile(tileDict[reorientedTileName].ToArray());
                        reorientedTile = rotateTile(reorientedTile, 1);
                        break;
                     case "-top":
                        reorientedTile = flipTile(tileDict[reorientedTileName].ToArray());
                        reorientedTile = rotateTile(reorientedTile, 2);
                        break;
                     case "-right":
                        reorientedTile = flipTile(tileDict[reorientedTileName].ToArray());
                        reorientedTile = rotateTile(reorientedTile, 3);
                        break;
                     case "-bottom":
                        reorientedTile = rotateTile(tileDict[reorientedTileName].ToArray(), 2);
                        break;
                     case "-left":
                        reorientedTile = rotateTile(tileDict[reorientedTileName].ToArray(), 1);
                        break;

                  }
                  tileDict[reorientedTileName] = reorientedTile.ToList();
                  goto exitLoop;
               
               }

            }
            
         }

         exitLoop:;

         return reorientedTileName;
         
      }

            
      public static void partTwo(StreamWriter sw, int sqrt, ref Dictionary<int, List<string>> tileDict, ref Dictionary<int, List<int>> matchedtileEdgesDict, ref List <int> cornerTiles){
         
         // We need to arrange our tiles(names) - Then we'll need to store them once they are correctly arranged - We know the dimensions will be the sqrt of number of tiles
         int[,] arrangedTiles = new int[sqrt, sqrt];
         
         // A sort of cheating method - We need to start with the right corner tile for this to work - So try and except on all four 
         foreach(int i in cornerTiles){

            try{

               arrangedTiles[0, 0] = i;

               for (int y = 0; y < sqrt; y++){

                  int nextTile = 0;

                  // Go through and oritate our tiles so they all line up properly and then add the tilenames to the arranged tiles array so we can create the photo later on.
                  for (int x = 0; x < sqrt - 1; x++)
                  {
                     nextTile = reorientateTile(sw, arrangedTiles[x, y], ref tileDict, ref matchedtileEdgesDict, "X");
                     arrangedTiles[x + 1, y] = nextTile;
                  }
                  if (y < sqrt - 1)
                  {
                     nextTile = reorientateTile(sw, arrangedTiles[0, y], ref tileDict, ref matchedtileEdgesDict, "Y");
                     arrangedTiles[0, y + 1] = nextTile;
                  }
               }

               monsterHunt(sw, sqrt, ref tileDict, ref arrangedTiles);

            } catch (Exception e){
               sw.WriteLine("{0} -- Using corner {1}", e, i);
            }
         }
         
      }

      // We need to get each edge from our tiles
      public static List<string> getTileEdges(StreamWriter sw, int tile, ref Dictionary<int, List<string>> tileDict)
      {  
         // The top and bottom edge are pretty easy as they already exist in a forat we can easily index 
         List<string> tileEdges = new List<string>();

         string top = tileDict[tile][0];
         string bottom = tileDict[tile].Last();
         string right = "";
         string left = "";

         // Left and right are going to be the first and last entry in each row
         for (int i = 0; i < tileDict[tile].Count; i++)
         {     
            left += tileDict[tile][i][0];
            right += tileDict[tile][i].Last();
         }
         
         // Add the edges in a specific order so we know which index is which edge, making our life easier down the line
         tileEdges.Add(top);
         tileEdges.Add(left);
         tileEdges.Add(bottom);
         tileEdges.Add(right);

         sw.WriteLine("Tile {0}\nTop -- {1}\nLeft -- {2}\nBottom -- {3}\nRight --{4}", tile, top, left, bottom, right);

         return tileEdges;
      }

      public static void getMatchedTileEdges(StreamWriter sw, int tile1, int tile2, ref Dictionary<int, List<string>> tileDict, ref Dictionary<int, List<int>> matchedtileEdgesDict){

         List<string> matchedEdge1s = getTileEdges(sw, tile1, ref tileDict);
         List<string>  matchedEdge2s = getTileEdges(sw, tile2, ref tileDict);
                 
         for (int i = 0; i < matchedEdge1s.Count; i++){

            for (int j = 0; j < matchedEdge2s.Count; j++){
               
               // Need to check the edge as is and also reversed 
               char[] temp = matchedEdge2s[j].ToCharArray();
               Array.Reverse(temp);
               string tile2ReversedEdge = string.Join("", temp);

               if (matchedEdge1s[i] == matchedEdge2s[j])
               {  
                  // If we get a match we add the matched tile to that entry - We use the entry count to work out which tiles are corner pieces
                  if(matchedtileEdgesDict.ContainsKey(tile1)){
                     matchedtileEdgesDict[tile1].Add(tile2);
                  } else {
                     matchedtileEdgesDict[tile1] = new List<int>();
                     matchedtileEdgesDict[tile1].Add(tile2);
                  }

                  if(matchedtileEdgesDict.ContainsKey(tile2)){
                     matchedtileEdgesDict[tile2].Add(tile1);
                  } else {
                     matchedtileEdgesDict[tile2] = new List<int>();
                     matchedtileEdgesDict[tile2].Add(tile1);
                  }

                  goto exitLoop;
               }
               // If matched as a reverse we need to indicate this for when we reorianted the tiles
               else if (matchedEdge1s[i] == tile2ReversedEdge)
               {  
                  if(matchedtileEdgesDict.ContainsKey(tile1)){
                     matchedtileEdgesDict[tile1].Add(tile2);
                  } else {
                     matchedtileEdgesDict[tile1] = new List<int>();
                     matchedtileEdgesDict[tile1].Add(tile2);
                  }

                  if(matchedtileEdgesDict.ContainsKey(tile2)){
                     matchedtileEdgesDict[tile2].Add(tile1);
                  } else {
                     matchedtileEdgesDict[tile2] = new List<int>();
                     matchedtileEdgesDict[tile2].Add(tile1);
                  }

                  goto exitLoop;
               }
               else
               {
                  ;
               }
            }
            exitLoop:;
         }

      }

      // There should be a way to do this in the above func - Need to revist
      public static List<string> getTileEdgesByType(StreamWriter sw, int tile1, int tile2, ref Dictionary<int, List<string>> tileDict){

         List<string> matchedEdge1s = getTileEdges(sw, tile1, ref tileDict);
         List<string>  matchedEdge2s = getTileEdges(sw, tile2, ref tileDict);

         List<string> edges = new List<string>{ "top", "left", "bottom", "right" };
         List<string> matches = new List<string>();
                 
         for (int i = 0; i < matchedEdge1s.Count; i++){

            for (int j = 0; j < matchedEdge2s.Count; j++){

               // Need to check the edge as is and also reversed 
               char[] temp = matchedEdge2s[j].ToCharArray();
               Array.Reverse(temp);
               string tile2ReversedEdge = string.Join("", temp);

               if (matchedEdge1s[i] == matchedEdge2s[j])
               {  

                  matches.Add(edges[i]);
                  matches.Add(edges[j]);

                  goto exitLoop;
               }
               // If matched as a reverse we need to indicate this for when we reorianted the tiles
               else if (matchedEdge1s[i] == tile2ReversedEdge)
               {                    
                  matches.Add(edges[i]);
                  matches.Add("-" + edges[j]);

                  goto exitLoop;
               }
               else
               {
                  ;
               }
            }
            exitLoop:;
         }

         return matches;

      }

      public static List<int> partOne(StreamWriter sw, ref Dictionary<int, List<string>> tileDict, ref Dictionary <int, List<int>> matchedtileEdgesDict){

         List <int> cornerTiles = new List<int>();

         List<int> keyList = new List<int>(tileDict.Keys);

         // Standard nested loop to check all tiles in the dictionary against each other
         for (int i = 0; i < keyList.Count; i++){
            for (int j = i + 1; j < keyList.Count; j++){
               getMatchedTileEdges(sw, keyList[i], keyList[j], ref tileDict, ref matchedtileEdgesDict);                  
            }
         }

         long partOneAnswer = 1;

         foreach(KeyValuePair<int, List<int>> entry in matchedtileEdgesDict){
            sw.WriteLine("{0} -- {1}", entry.Key, entry.Value.Count);
            // If we have a count of two that means two matched egdes = corner piece
            if(entry.Value.Count == 2){
               partOneAnswer *= Convert.ToInt64(entry.Key);
               cornerTiles.Add(Convert.ToInt32(entry.Key));
            }
         }
         
         Console.WriteLine("Part One Answer -- {0}",partOneAnswer);

         return cornerTiles;
      } 

      static void Main(string[] args) 
      {
         // Load input
         List <string> lines = File.ReadLines("puzzle_20_input.txt").ToList();

         // Using a output file for debugging
         string outputFile = "puzzle_20_output.txt"; 
         // Which is wiped at the start of each run to avoid trawling through reams of output
         System.IO.File.WriteAllText(outputFile,string.Empty);

         Dictionary<int, List<string>> tileDict = new Dictionary<int, List<string>>();

         Dictionary<int, List<int>> matchedtileEdgesDict = new Dictionary<int,List<int>>();
         
         bool addTile = false;

         int dictKey = 0;

         using(StreamWriter sw = File.AppendText(outputFile)) 
         {  
            
            foreach(string r in lines){

               if(addTile){
                  if(r != ""){
                     tileDict[dictKey].Add(r);
                  } else {
                     addTile = false;
                  }
               }

               if (r.StartsWith("Tile")){
                  var match = Regex.Match(r, @"([0-9]+)");
                  dictKey = Convert.ToInt32(match.Groups[1].Value);
                  sw.WriteLine(dictKey);
                  tileDict[dictKey] = new List <string> ();
                  addTile = true;
               }
               
            }
            
            foreach (KeyValuePair<int, List<string>> entry in tileDict)
            {
               sw.WriteLine("{0}", entry.Key);
               foreach(string str in entry.Value){
                  sw.WriteLine("{0}", str);
               }
               sw.WriteLine("----------------");
            }

            List <int> cornerTiles = partOne(sw, ref tileDict, ref matchedtileEdgesDict);

            partTwo(sw, Convert.ToInt32(Math.Sqrt(tileDict.Count)), ref tileDict, ref matchedtileEdgesDict, ref cornerTiles);

         }

      }
   }
}