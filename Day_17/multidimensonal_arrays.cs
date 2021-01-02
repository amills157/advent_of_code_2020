using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace advent_day_17 {
   class puzzle_17 {     

      // Originally we used arrays intialised solely within a partOne function - But as partTwo was just partOne with an extra dimension it made sense to try utilise the same loops etc - So a switch to global variables to avoid duplication of code where possible
      public static int[,,] cubeArray3;
      public static int[,,,] cubeArray4;

      public static int[,,] nextCycle3;
      public static int[,,,] nextCycle4;

      public static int cubeCycler(StreamWriter sw, ref List <string> lines, int partSelector){
         

         // We need to intialise the size of each dimension - Which needs to include the size of the starting array plus 2 per cycle - Initially set to 100 which works but takes a while to run
         int arrayInitializer = lines[0].Length + 12;
         // We need an array with 3 dimensions (x,y & z)

         if(partSelector == 1){
            cubeArray3 = new int[arrayInitializer, arrayInitializer , arrayInitializer];
         } else {
            cubeArray4 = new int[arrayInitializer, arrayInitializer , arrayInitializer, arrayInitializer];
         }
         
         // Loop through the input lines
         for (int y = 0; y < lines[0].Length; y++){
            // Loop through the chars in the lines
            for (int x = 0; x < lines[0].Length; x++){
               if(partSelector==1){
                  // Line char needs to be reversed for correct X and Y axis input
                  if (lines[y][x] == '#'){
                     cubeArray3[x, y, 0] = 1;
                  }else{
                     cubeArray3[x, y, 0] = 0;
                  }
               } else {
                  if (lines[y][x] == '#'){
                     cubeArray4[x, y, 0, 0] = 1;
                  }else{
                     cubeArray4[x, y, 0, 0] = 0;
                  }
               }
            }
         }
         
        //It took a little bit to get round the example, but the view / array needs to scroll as all the poisitions changes- So we need to adjust the "view" by -1 each time - This gives us the starting positions for our next cycle
        //(https://www.reddit.com/r/adventofcode/comments/kioa2v/2020_day_17_help_understanding_the_requirements/)
         for (int i = 0; i < 6; i++)
         {  
            adjustView(arrayInitializer, partSelector);
            newValues(sw, arrayInitializer, partSelector);
         }

         int activeCubes = 0;
         if(partSelector == 1){
            foreach(int cube in cubeArray3){
               activeCubes += cube;
            }
         } else {
            foreach(int cube in cubeArray4){
               activeCubes += cube;
            }
         }
         
         return activeCubes;
      }

     public static void newValues(StreamWriter sw, int arrayInitializer, int partSelector){

         //(https://i.redd.it/xko67mbnet561.png)
         for (int x = 0; x < arrayInitializer; x++){
            for (int y = 0; y < arrayInitializer; y++){
               for (int z = 0; z < arrayInitializer; z++){
                  if(partSelector == 1){
                     if (isActive(sw, x, y, z, 0, arrayInitializer, partSelector)){
                        cubeArray3[x, y, z] = 1;
                     }else{
                        cubeArray3[x, y, z] = 0;
                     }
                  } else {
                     for (int w = 0; w < arrayInitializer; w++){
                        if (isActive(sw, x, y, z, w, arrayInitializer, partSelector)){
                           cubeArray4[x, y, z, w] = 1;
                        }else{
                           cubeArray4[x, y, z, w] = 0;
                        }
                     }
                  }
                  
               }
            }
         }
      }

     public static bool isActive(StreamWriter sw, int x, int y, int z, int w, int arrayInitializer, int partSelector)
     {
         bool active = false;
         int activeNeighbours = 0;

         // We need to loop through and check the neighbouring nodes - We start at one lower than the current position and go until we reach one above (on all axis)
         for (int i = x - 1; i <= x + 1; i++){
            if (Enumerable.Range(0,(arrayInitializer - 1)).Contains(i)){
               for (int j = y - 1; j <= y + 1; j++){
                  if (Enumerable.Range(0,(arrayInitializer - 1)).Contains(j)){ 
                     for (int k = z - 1; k <= z + 1; k++){
                        if (Enumerable.Range(0,(arrayInitializer - 1)).Contains(k)){
                           if(partSelector == 1){
                              if (nextCycle3[i, j, k] == 1){
                                 // Safety check that we aren't counting the node in question as it's own neighbour
                                 if (!(x == i && y == j && z == k)){ 
                                    activeNeighbours += 1;
                                 }
                              }
                           } else {
                              for (int l = w - 1; l <= w + 1; l++){
                                 if (Enumerable.Range(0,(arrayInitializer - 1)).Contains(l)){
                                    if (nextCycle4[i, j, k, l] == 1){
                                       // Safety check that we aren't counting the node in question as it's own neighbour
                                       if (!(x == i && y == j && z == k && w == l)){ 
                                          activeNeighbours += 1;
                                       }
                                    }
                                 }
                              }
                           }
                           
                        }
                     }
                  }
               }
            }
         }

         if(partSelector == 1){
            if (nextCycle3[x, y, z] == 1){
               sw.WriteLine("Node at {0} {1} {2} already active", x, y, z);
               if (activeNeighbours == 2 || activeNeighbours == 3){
                  active = true;
                  sw.WriteLine("Node at {0} {1} {2} remains active -- Active neighbours {3}", x, y, z, activeNeighbours);
               }
            }else{
               if (activeNeighbours == 3)
               {
                  active = true;
                  sw.WriteLine("Node at {0} {1} {2} now active", x, y, z);
               }
            }
         }else{
            if (nextCycle4[x, y, z, w] == 1){
               sw.WriteLine("Node at {0} {1} {2} {3} already active", x, y, z, w);
               if (activeNeighbours == 2 || activeNeighbours == 3){
                  active = true;
                  sw.WriteLine("Node at {0} {1} {2} {3} remains active -- Active neighbours {4}", x, y, z, w, activeNeighbours);
               }
            }else{
               if (activeNeighbours == 3)
               {
                  active = true;
                  sw.WriteLine("Node at {0} {1} {2} {3} now active", x, y, z, w);
               }
            }
         }

         return active;
     }

     public static void adjustView(int arrayInitializer, int partSelector){

        // Adjusts the "view" and returns the next cycle position

        // These need to be initalised else we get a break in the logic as they would be local to any if statements
        int[,,] temp3 = new int[arrayInitializer, arrayInitializer , arrayInitializer];
        int[,,,] temp4 = new int[arrayInitializer, arrayInitializer , arrayInitializer,arrayInitializer];

         //(https://i.redd.it/xko67mbnet561.png)
         for (int x = 0; x < arrayInitializer; x++){
            for (int y = 0; y < arrayInitializer; y++){
               for (int z = 0; z < arrayInitializer; z++){
                  if(partSelector == 1){
                     //Either a new row
                     if (x == 0 || y == 0 || z == 0){
                        temp3[x, y, z] = 0;
                     // Or moved down a row   
                     }else{
                        temp3[x, y, z] = cubeArray3[x - 1, y - 1, z - 1];
                     }
                  } else {
                     for (int w = 0; w < arrayInitializer; w++){
                        //Either a new row
                        if (x == 0 || y == 0 || z == 0 || w== 0){
                           temp4[x, y, z, w] = 0;
                        // Or moved down a row   
                        }else{
                           temp4[x, y, z, w] = cubeArray4[x - 1, y - 1, z - 1, w - 1];
                        }
                     }
                  }      
               }
            }
         }
         if(partSelector == 1){
            nextCycle3 = temp3;
         }else{
            nextCycle4 = temp4;
         }
      }

      
      static void Main(string[] args) 
      {
         // Load input
         List <string> lines = File.ReadLines("puzzle_17_input.txt").ToList();

         // Using a output file for debugging
         string outputFile = "puzzle_17_output.txt"; 
         // Which is wiped at the start of each run to avoid trawling through reams of output
         System.IO.File.WriteAllText(outputFile,string.Empty);

         using(StreamWriter sw = File.AppendText(outputFile)) 
         {  
            
            // This could probably be re-written to remove the part selector and have the cycler do both at the same time - To revist 
            int partOneAnswer = cubeCycler(sw, ref lines, 1);

            Console.WriteLine("Part One Answer -- {0}",partOneAnswer);

            int partTwoAnswer = cubeCycler(sw, ref lines, 2);

            Console.WriteLine("Part Two Answer -- {0}",partTwoAnswer);

         }

      }
   }
}