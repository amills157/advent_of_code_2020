using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace advent_day_12 {
   class puzzle_12 {
      public static  void moveVessel(char moveDirection, int value, ref List<int> vesselPos){

         switch(moveDirection) 
         {
         case 'E':
            vesselPos[0] += value;
            break;
         case 'W':
            vesselPos[0] -= value;
            break;
         case 'N':
            vesselPos[1] += value;
            break;
         case 'S':
            vesselPos[1] -= value;
            break;
         default:
            ;
            break;
         }

      } 

      public static char rotateVessel(char facingDirection, char rotateDir, int value){

         int clicks = (value / 90);

         int newIdx = 0;

         // Directions organised for right turns from start to finish. Right = +, Left = -
         List <char> directions = new List <char> {'E', 'S', 'W', 'N'};

         int idx = directions.FindIndex(a => a == facingDirection);

         if(rotateDir == 'R'){
            newIdx = idx + clicks;
         } else {
            newIdx = idx - clicks;  
         }

         // To handle when we go out of bounds we need to bring the newIdx back within the range of our direction array

         if(newIdx < 0 ){
            newIdx += directions.Count;
         }

         if(newIdx >= directions.Count){
            newIdx %= directions.Count;
         }

         return directions[newIdx];
      }

      public static  void moveWayPoint(char moveDirection, int value, ref List<int> wayPointPos){

         switch(moveDirection) 
         {
         case 'E':
            wayPointPos[0] += value;
            break;
         case 'W':
            wayPointPos[0] -= value;
            break;
         case 'N':
            wayPointPos[1] += value;
            break;
         case 'S':
            wayPointPos[1] -= value;
            break;
         default:
            ;
            break;
         }

      }

      public static  void moveToWayPoint(int value, ref List<int> vesselPos, ref List<int> wayPointPos){
         
         char facingEastWest ='E';

         char facingNorthSouth ='N';

         int eastWest = wayPointPos[0];

         int northSouth = wayPointPos[1];

         if(wayPointPos[0] < 0){
            eastWest = Math.Abs(wayPointPos[0]);
            facingEastWest = 'W';
         } 

         if(wayPointPos[1] < 0){
            northSouth = Math.Abs(wayPointPos[1]);
            facingNorthSouth = 'S';
         }

         switch(facingEastWest) 
         {
         case 'E':
            vesselPos[0] += (eastWest * value);
            break;
         case 'W':
            vesselPos[0] -= (eastWest * value);
            break;
         default:
            ;
            break;
         }

         switch(facingNorthSouth) 
         {         
         case 'N':
            vesselPos[1] += (northSouth * value);
            break;
         case 'S':
            vesselPos[1] -= (northSouth * value);
            break;
         default:
            ;
            break;
         }

      }

      public static void rotateWayPoint(char rotateDir, int value, ref List<int> wayPointPos){

         // For rotation we need copies of the original positions otherwise we end up using adjusted positions for wayPointPos[1]
         
         int clicks = (value / 90);

         for(int i = 0; i < clicks; i++){
            int original_x = wayPointPos[0];
            int original_y = wayPointPos[1];


            if(rotateDir == 'R'){
               wayPointPos[0] = original_y;
               wayPointPos[1] = -original_x;
            } else {
               wayPointPos[0] = -original_y;
               wayPointPos[1] = original_x;
            }
         }
         
      }

      public static  void partOne(StreamWriter sw, ref IEnumerable<string>  lines, ref char facingDirection, ref List<int> vesselPos){

         // Convert each string int to int
         foreach (string r in lines)
         {  
            sw.WriteLine(r);
            var match = Regex.Match(r, "([A-Z])([0-9]+)");
            char action = char.Parse(match.Groups[1].Value);
            int value = int.Parse(match.Groups[2].Value);
            switch(action) 
            {
            case 'F':
               moveVessel(facingDirection, value, ref vesselPos);
               break;
            case 'N':
               moveVessel('N', value, ref vesselPos);
               break;
            case 'S':
               moveVessel('S', value, ref vesselPos);
               break;
            case 'E':
               moveVessel('E', value, ref vesselPos);
               break;
            case 'W':
               moveVessel('W', value, ref vesselPos);
               break;
            case 'R':
               facingDirection = rotateVessel(facingDirection, 'R', value);
               break;
            case 'L':
               facingDirection = rotateVessel(facingDirection, 'L', value);
               break;
            default:
               ;
               break;
            }
            sw.WriteLine(string.Join(" ", vesselPos));
         }

         int abs1 = Math.Abs(vesselPos[0]);
         int abs2 = Math.Abs(vesselPos[1]);

         Console.WriteLine("Part One Project Distance -- {0}", (abs1 + abs2));

      }  

      public static  void partTwo(StreamWriter sw, ref IEnumerable<string>  lines, ref char facingDirection, ref List<int> vesselPos, ref List<int> wayPointPos){

         // Convert each string int to int
         foreach (string r in lines)
         {  
            sw.WriteLine(r);
            var match = Regex.Match(r, "([A-Z])([0-9]+)");
            char action = char.Parse(match.Groups[1].Value);
            int value = int.Parse(match.Groups[2].Value);
            switch(action) 
            {
            case 'F':
               moveToWayPoint(value, ref vesselPos, ref wayPointPos);
               break;
            case 'N':
               moveWayPoint('N', value, ref wayPointPos);
               break;
            case 'S':
               moveWayPoint('S', value, ref wayPointPos);
               break;
            case 'E':
               moveWayPoint('E', value, ref wayPointPos);
               break;
            case 'W':
               moveWayPoint('W', value, ref wayPointPos);
               break;
            case 'R':
               sw.WriteLine("Before Rotation - R");
               sw.WriteLine(string.Join(" ", wayPointPos));
               rotateWayPoint('R', value, ref wayPointPos);
               sw.WriteLine("After Rotation - R");
               sw.WriteLine(string.Join(" ", wayPointPos));
               break;
            case 'L':
               sw.WriteLine("Before Rotation - L");
               sw.WriteLine(string.Join(" ", wayPointPos));
               rotateWayPoint('L', value, ref wayPointPos);
               sw.WriteLine("After Rotation - L");
               sw.WriteLine(string.Join(" ", wayPointPos));
               break;
            default:
               ;
               break;
            }
            sw.WriteLine(string.Join(" ", vesselPos));
         }

         int abs1 = Math.Abs(vesselPos[0]);
         int abs2 = Math.Abs(vesselPos[1]);

         Console.WriteLine("Part Two Project Distance -- {0}", (abs1 + abs2));

      }

      
      static void Main(string[] args) 
      {
         // Load input
         IEnumerable<string> lines = File.ReadLines("puzzle_12_input.txt");

          // Using a output file for debugging
         string outputFile = "puzzle_12_output.txt"; 
         // Which is wiped at the start of each run to avoid trawling through reams of output
         System.IO.File.WriteAllText(outputFile,string.Empty);

         // East / West = list[0], North/South = list[1]
         List <int> vesselPos = new List<int> {0, 0};

         List <int> wayPointPos = new List<int> {10, 1};

         char facingDirection = 'E';

         using(StreamWriter sw = File.AppendText(outputFile)) 
         {   
            
            partOne(sw, ref lines, ref facingDirection, ref vesselPos);

            // We need to reset the boat position between runs
            vesselPos[0] = 0;
            vesselPos[1] = 0;

            partTwo(sw, ref lines, ref facingDirection, ref vesselPos, ref wayPointPos);
            
         }

      }
   }
}