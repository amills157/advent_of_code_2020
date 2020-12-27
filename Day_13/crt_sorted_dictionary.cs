using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace advent_day_13 {
   class puzzle_13 {
      
      public static  void partOne(StreamWriter sw, int depatureidStringStamp, ref string [] busIDs){

         sw.WriteLine(string.Join(" ", busIDs));

         int divisionCheck = depatureidStringStamp;

         int candidateBusID = 0;


         foreach(string idString in busIDs){
            if (idString != "x"){
               int idInt = int.Parse(idString);

               sw.WriteLine("Bus ID -- {0} -- {1}", idInt,(depatureidStringStamp / idInt));

               if((depatureidStringStamp / idInt) < divisionCheck){
                  divisionCheck = (depatureidStringStamp / idInt);
                  candidateBusID = idInt;
               } 

            }
         }

         sw.WriteLine("Bus ID -- {0} -- {1}", candidateBusID,divisionCheck);

         while(true){
            int departureidString = candidateBusID * divisionCheck;

            if (departureidString >=  depatureidStringStamp){
               
               // Not sure why I did this big block of output - But I'm gonna keep it like this
               Console.WriteLine("Bus ID -- {0}\nDeparts at -- {1}\nYou arrive at -- {2}\nMinutes waiting -- {3}\nAnswer (Part One)-- {4}", candidateBusID, departureidString, depatureidStringStamp, (departureidString - depatureidStringStamp),candidateBusID * (departureidString - depatureidStringStamp));

               break;


            } else {
               divisionCheck += 1;
            }
         }

      }

      public static  void partTwo(StreamWriter sw, ref string [] busIDs){
         
         SortedDictionary <long, long> busInfo = new SortedDictionary <long, long>();

         // create dictionary with indices and bus frequencies
         for(int i = 0; i < busIDs.Length; i ++){
            if (busIDs[i] != "x"){
               busInfo[i] = long.Parse(busIDs[i]);
               //sw.WriteLine("{0} -- {1}", i, long.Parse(busIDs[i]));
            }
         }

         var sortedDict = from entry in busInfo orderby entry.Value descending select entry;

         foreach(KeyValuePair<long, long> entry in sortedDict){
            sw.WriteLine("{0} -- {1}", entry.Key, entry.Value);
         }

         /*

         Ok. Let's get mathmatical.

         To solve this we need to work out what t is such it solves the following t + delay % busID (duration) = 0, so for the example:

         t % 7 = 0  / t % 7 = (7 - 0)
         t + 1 % 13 = 0 / t % 13 = (13 - 1)
         t + 4 % 59 = 0 / t % 59 = (59 - 4)
         t + 6 % 31 = 0 / t % 31 = (31 - 6)
         t + 7 % 19 = 0 / t % 19 = (19 - 7)

         So for example if we take t % 7 = 0 and t + 1 % 13 = 0 we get a common t at 77

         77 % 7 = 0 (11 * 7 = 77 )
         77 + 1 % 13 = 0 (77 + 1 = 78 / 13 * 6 = 78)

         Once we have our starting point of 77 we can then work through additional time points by adding the product of those Bus IDs to out t (which is now our incremement) - We can use this because they are prime numbers and we can consider each t as 0 and we know the next time they meet will be an increment of their LCM (which is their product)

         For example 77 + (7 * 13) = 168

         168 % 7 = 0 (24 * 7 = 168 )
         168 + 1 % 13 = 0 (168 + 1 = 169 / 13 * 13 = 169)

         We could then itterate through using this process to get our eventual timestamp. 

         Because we know all of the Bus IDs are primes and need to meet at a common t + delay we can start with the biggest increment first (in the case of our example 59) and work backwards

         ***********************************************************
         Importantly we're not trying to find a timestamp that leaves us with mod 0 for all bus IDs as is (the alignment of the buses. Like the alignment of the planets but rarer) 

         For example t mod 7 == t mod 13 == ... == 0
         
         But a timestamp that leaves us with the a mod of the delay for each busID

         t mod 7 == 0
         t mod 13 == 12
         ...
         t mod 19 == 12

         This would make our timestamp essentially the same as the intial starting timestamp of 0 for the state of the buses
         ***********************************************************

         */
         
         long timeStamp = 1;
         long incerement = 1;
         long remainder = ((timeStamp + sortedDict.ElementAt(0).Key) % sortedDict.ElementAt(0).Value);

         sw.WriteLine("Remainder = {0}", remainder);

         // Starting with our biggest increment we loop through each bus
         for(int i = 0; i < busInfo.Count; i++){
            
            while (true)
            {

               // The delay is the sorted key
               long delay = sortedDict.ElementAt(i).Key;

               // The Bus ID / duration are the same 
               long dur = sortedDict.ElementAt(i).Value;
               
               // Following the math above we know our eventual time stamp (t) will be the LCM apart (increment)
               timeStamp += incerement;
               remainder = ((timeStamp + delay) % dur);

               sw.WriteLine("TimeeStamp -- {0} -- Increment = {1} -- Remainder = {2}", timeStamp, incerement, remainder);

               if (remainder == 0)
               {
                  // If we hit 0 then either we have found t for this Bus ID (the try) or we have run out of Bus's and found the final answer (the catch)
                  try{
                     remainder = ((timeStamp + sortedDict.ElementAt(i +1).Key) % sortedDict.ElementAt(i +1).Value);
                  } catch (Exception e){
                     goto exitLoop;
                  }
                  
                  if (remainder != 0)
                  {
                     sw.WriteLine("Increment Before {0}", incerement);
                     // Because the Bus IDs are all prime numbers we know that the LCM will be the product of the IDs so we can just multiple the current increment with the busID duration.

                     // For example 59 * 31 * 19 is the same as 1829 * 19 (34751)
                     incerement *= dur;
                     sw.WriteLine("Increment After {0}", incerement);
                  }

                  goto exitLoop;
               }

            }
            exitLoop:;
         }
        Console.WriteLine("Answer (Part Two)-- {0}",timeStamp);

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

            partTwo(sw, ref busIDs);
                
            
         }

      }
   }
}