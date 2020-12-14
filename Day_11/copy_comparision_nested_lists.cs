using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace advent_day_10 {
   class puzzle_10 {
      public static  List <List<char>> copyComparision(StreamWriter sw, ref List <List<char>> seats, ref List <List<int>> seatsToCheck, int tolerance){

         int occupiedSeats = 0;
         
         // We need to copy the seats so we can alter them without impacting the existing structure (rules applied blanket not seat by seat)
         List <List<char>> temp = new List <List<char>>();

         for (int i = 0; i < seats.Count; i++){
            
            // Copy each row in so we can then alter it
            List <char> tempSeats = new List <char>(seats[i]);
            temp.Add(tempSeats);
               
            for (int j = 0; j < seats[i].Count; j++){

               int occupiedCount = 0;

               foreach(List <int> toCheck in seatsToCheck){
                  // Try and except for out of bounds catching
                  try
                  {
                     int row = toCheck[0];
                     int seat = toCheck[1];
                     while(true){
                        
                        if (seats[i + row][ j + seat] == '#'){
                           occupiedCount += 1;
                           break;
                        }

                        if (seats[i + row][ j + seat] == 'L'){
                           break;
                        }

                        if (seats[i + row][ j + seat] == '.'){
                           // Cheating way to have the same function work for part 1 and 2
                           if(tolerance == 5){
                              // If we're on part 2 we itterate out the rows and seats until we get past the floor
                              if(row > 0){
                              row +=1;
                              }

                              if(row < 0){
                              row += -1;
                              }

                              if(seat > 0){
                              seat +=1;
                              }

                              if(seat < 0){
                              seat += -1;
                              }
                           } else {
                              break;
                           }
                           

                        }

                     }                     
                  }
                     catch (Exception e)
                  {
                     sw.WriteLine(e.Message);
                  }

               }

               if(seats[i][j] == 'L' && occupiedCount == 0){
                  temp[i][j] = '#';
               }

               if(seats[i][j] == '#' && occupiedCount >= tolerance){
                  temp[i][j] = 'L';
               }

            }
            
         }

         return temp;

      } 

      public static void copyComparisionWrapper(StreamWriter sw, ref List <List<char>> seats, ref List <List<int>> seatsToCheck, int tolerance){

         List <List<char>> newSeats = new List <List<char>>();

         int occupiedSeats = 0;
         int oldCount = 0;

         while (true){

            newSeats = copyComparision(sw, ref seats, ref seatsToCheck, tolerance);

            seats = newSeats;

            // There is probably a way to tack this onto the the curret seat check itself
            occupiedSeats = 0;
            for (int i = 0; i < seats.Count; i++){
               sw.WriteLine(string.Format("Row: ({0}).", string.Join(", ", seats[i])));
               for (int j = 0; j < seats[i].Count; j++){
                  if(seats[i][j] == '#'){
                     occupiedSeats += 1;
                  }
               }
            }

            sw.WriteLine("------------{0}------------", occupiedSeats);

            if(oldCount == occupiedSeats){
               // Avoids the need for a return value
               Console.WriteLine("Stable at {0} seats occupied", occupiedSeats);
               break;
            } else {
               oldCount = occupiedSeats;
            }
         }
      }

      static void Main(string[] args) 
      {
         // Load input
         IEnumerable<string> lines = File.ReadLines("puzzle_11_input.txt");

          // Using a output file for debugging
         string outputFile = "puzzle_11_output.txt"; 
         // Which is wiped at the start of each run to avoid trawling through reams of output
         System.IO.File.WriteAllText(outputFile,string.Empty);

         List <List<char>> seatsPartOne = new List <List<char>>();
         List <List<char>> seatsPartTwo = new List <List<char>>();

         List <List<int>> seatsToCheck = new List <List<int>>();

         //Directly Above
         seatsToCheck.Add(new List<int> {-1, 0});
         //Directly Below
         seatsToCheck.Add(new List<int> {+1, 0});
         //Left
         seatsToCheck.Add(new List<int> {0, -1});
         //Right
         seatsToCheck.Add(new List<int> {0, +1});
         //Above Left
         seatsToCheck.Add(new List<int> {-1, -1});
         //Above Right
         seatsToCheck.Add(new List<int> {-1, +1});
         //Below Left
         seatsToCheck.Add(new List<int> {+1, -1});
         //Bewlo Right
         seatsToCheck.Add(new List<int> {+1, +1});

         // Convert each string int to int
         foreach (string r in lines)
         {  
            char[] charArr = r.ToCharArray();
            List<char> charList = charArr.OfType<char>().ToList();

            // Two copies so we get the answer from the original layout for both part 1 and 2
            seatsPartOne.Add(charList);
            seatsPartTwo.Add(charList);
            
         }

         using(StreamWriter sw = File.AppendText(outputFile)) 
         {   
            // Part One
            copyComparisionWrapper(sw, ref seatsPartOne, ref seatsToCheck, 4);

            // Part Two
            copyComparisionWrapper(sw, ref seatsPartTwo, ref seatsToCheck, 5);
            
         }

      }
   }
}