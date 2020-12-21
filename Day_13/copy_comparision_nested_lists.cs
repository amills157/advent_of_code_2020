using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace advent_day_13 {
   class puzzle_13 {
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

         // For Right and Left rotation we need copies of the original positions otherwise we end up using adjusted positions for wayPointPos[1]
         
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

      public static  void partOne(StreamWriter sw, int depatureTimeStamp, ref string [] busIDs){

         sw.WriteLine(string.Join(" ", busIDs));

         int divisionCheck = depatureTimeStamp;

         int candidateBusID = 0;


         foreach(string idString in busIDs){
            if (!(idString == "x")){
               int idInt = int.Parse(idString);

               sw.WriteLine("Bus ID -- {0} -- {1}", idInt,(depatureTimeStamp / idInt));

               if((depatureTimeStamp / idInt) < divisionCheck){
                  divisionCheck = (depatureTimeStamp / idInt);
                  candidateBusID = idInt;
               } 

            }
         }

         sw.WriteLine("Bus ID -- {0} -- {1}", candidateBusID,divisionCheck);

         while(true){
            int departureTime = candidateBusID * divisionCheck;

            if (departureTime >=  depatureTimeStamp){

               Console.WriteLine("Bus ID -- {0}\nDeparts at -- {1}\nYou arrive at -- {2}\nMinutes waiting -- {3}\nAnswer (Part One)-- {4}", candidateBusID, departureTime, depatureTimeStamp, (departureTime - depatureTimeStamp),candidateBusID * (departureTime - depatureTimeStamp));

               break;


            } else {
               divisionCheck += 1;
            }
         }

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
         IEnumerable<string> lines = File.ReadLines("puzzle_13_input.txt");

          // Using a output file for debugging
         string outputFile = "puzzle_13_output.txt"; 
         // Which is wiped at the start of each run to avoid trawling through reams of output
         System.IO.File.WriteAllText(outputFile,string.Empty);


         using(StreamWriter sw = File.AppendText(outputFile)) 
         {   
            int depatureTimeStamp = int.Parse(lines.ElementAt(0));

            string[] busIDs = lines.ElementAt(1).Split(',');

            
            partOne(sw, depatureTimeStamp, ref busIDs);

            List <List<int>> departures = new List <List<int>> ();

            for(int i = 0; i < busIDs.Length; i ++){
               if (!(busIDs[i] == "x")){

                  departures.Add(new List <int> {i, int.Parse(busIDs[i]), (i + int.Parse(busIDs[i]))});

               }
            }

            foreach(List <int> pair in departures){
               Console.WriteLine(string.Join(" ", pair));
            }
                
            
         }

      }
   }
}