using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace advent_day_23 {

   //C# There seem to be two approaches to this:
   // LinkedListType (https://www.geeksforgeeks.org/linked-list-implementation-in-c-sharp/)
   // LinkedList with Node Class (https://www.c-sharpcorner.com/article/linked-list-implementation-in-c-sharp/)

   internal class Node{

      internal readonly int value;
      internal Node next;
      public Node(int v){
         this.value = v;
         this.next = null;
      }
   }

   
   class NodeLinkedList
   {
      public Node current;
      public NodeLinkedList()
      {
         current = null;
      }

      public Node addNewLinkedNode(int value)
      {
         Node n = new Node(value);
         // New list - Creating first node
         if (current == null)
         {
            current = n;

         // List with only 1 node created - Add circular linking between the two nodes
         }else if (current.next == null){
            current.next = n;
            n.next = current;
            current = n;
         
         //  Shift the links - Add the new node after the current and shift the links round. New node points back to the start of list and the current points to this new node. Finally the new node becomes our current node
         }else{
            n.next = current.next;
            current.next = n;
            current = n;
         }

         return n;
      }

      public void placeCupsDown(Node firstCup, Node secondCup, Node lastCup, int destCup)
      {
         Node n = this.current;
         
         // Find the destination node
         while(n.value != destCup){
            n = n.next;
         }

         // We re-arrange the links. So the last cup we place down is linked to where our destionation cup was and the first cup we place down is now linked to our dest cup
         lastCup.next = n.next;
         n.next = firstCup;
         firstCup.next = secondCup;
      }
      
   }

   class puzzle_23 {

      public static int currentCupValue = 0;
 
      
      public static void partTwo(ref int [] cupNumbers)
      {

         int numberOfCups = 1000000;
         int moves = 10000000;
         
         NodeLinkedList cupLinkedList = new NodeLinkedList();
         
         // All credit for this idea to James and Tim
         Dictionary<int, Node> cupsDict = new Dictionary<int, Node>();

         foreach(int cup in cupNumbers){
            Node n = cupLinkedList.addNewLinkedNode(cup);
            cupsDict.Add(cup, n);
         }

         // Because we need to bulk out our cup Numbers to match 1000000 we itterate through and add i + 1.
         // Because the array length = 9 and always contains numbers 1 - 9 we can just use the length as our starting point
         for (int i = cupNumbers.Length; i < numberOfCups; i++){
            Node n = cupLinkedList.addNewLinkedNode(i + 1);
            cupsDict.Add(i + 1, n);
         }

         // We'll need this later for the dest cup values
         List<int> dictKeys = cupsDict.Keys.ToList();
         
         int firstCup = cupNumbers[0];
         cupLinkedList.current = cupsDict[firstCup];

         for (int i = 0; i < moves; i++)
         {
               partTwoMove(ref cupLinkedList, ref cupsDict,ref dictKeys);
         }

         cupLinkedList.current = cupsDict[1];

         long firstCupValue = cupsDict[1].next.value;
         long secondCupValue = cupsDict[Convert.ToInt32(firstCupValue)].next.value;

         Console.WriteLine("Part Two Answer -- {0}", firstCupValue * secondCupValue);

      }

      public static void partTwoMove(ref NodeLinkedList cupLinkedList, ref Dictionary<int, Node> cupsDict, ref List<int> dictKeys)
      {
         // The three cups the crab will pick up
         Node firstCup = cupLinkedList.current.next;
         Node secondCup = firstCup.next;
         Node lastCup = secondCup.next;

         //  Same as partOne
         int destCup = cupLinkedList.current.value - 1;

         // Set now, before we move the above cups and the next cup for our 3 picked up cups changes
         int newCurrentCup = lastCup.next.value;
         cupLinkedList.current.next = lastCup.next;

         while(true){

            bool foundCup = false;

            if (destCup < 1){
               // Similar to what we did in partOne but without the array itself we need the dictKeys
               destCup = dictKeys.Max();
            }

            if (destCup == firstCup.value || destCup == secondCup.value || destCup == lastCup.value)
            {
               destCup -=1;
               foundCup = false;
            } else {
               foundCup = true;
            }
            
            if (foundCup){
               break;
            }
         }

         // Set our current cup as our destCup to make it easier for when we place the cups down (left of the dest cup)
         cupLinkedList.current = cupsDict[destCup];
         cupLinkedList.placeCupsDown(firstCup, secondCup, lastCup, destCup);

         // Once we've placed the cups down we get our new current cup, based off the old lastcup next cup
         cupLinkedList.current = cupsDict[newCurrentCup];
      }

      
      public static void partOne(StreamWriter sw, ref int [] cupNumbers)
      {
         int numberOfCups = cupNumbers.Length;
         int moves = 100;
         currentCupValue = cupNumbers[0];

         int[] cupNumbersCopy = new int[numberOfCups];

         int[] currentArray = new int[numberOfCups];
         int[] targetArray = new int[numberOfCups];
         
         //  Create a copy of our original cup numbers
         cupNumbers.CopyTo(cupNumbersCopy, 0);

         //  currentArray and targetArray will flip back and forth as to which ones they're referencing

         currentArray = cupNumbers;
         targetArray = cupNumbersCopy;

         for (int i = 0; i < moves; i++)
         {
            partOneMove(sw, ref cupNumbers, ref cupNumbersCopy, ref currentArray, ref targetArray);
         }

         // Move 1 to index [0]
         moveCurrentCup(sw, 1, ref cupNumbers, ref cupNumbersCopy, ref currentArray, ref targetArray);

         // Then skip it and print the rest of array out as our answer
         int [] partOneAnswer = currentArray.Skip(1).ToArray();

         Console.WriteLine("Part One Answer -- {0}", string.Join("", partOneAnswer));

      }

      public static void partOneMove(StreamWriter sw, ref int [] cupNumbers, ref int[] cupNumbersCopy, ref int [] currentArray, ref int [] targetArray)
      {
         // Move the current cup to [0]
         moveCurrentCup(sw, currentCupValue, ref cupNumbers, ref cupNumbersCopy, ref currentArray, ref targetArray);

         //  make a copy of the data
         Buffer.BlockCopy(currentArray, 0, targetArray, 0, currentArray.Length * sizeof(int));

         // Our dest cup should just be the currentCup -1
         int destCup = currentCupValue - 1;

         // But we need to make sure it's a valid dest cup
         while (true)
         {
            bool foundDest = false;

            if(destCup < 1){
               destCup = cupNumbers.Max();
            }

            foreach(int cup in cupNumbers){
               if(destCup == cup){

                  // Check the match isn't one of the cups the crab "picked up"
                  for (int i = 1; i < 4; i++){
                     if (cupNumbers[i] == destCup){
                        destCup-=1;
                        foundDest = false;
                        break;
                     } else {
                        foundDest = true;
                     }
                  }
               }
            }
            
            if (foundDest){
               break;
            } 
         }

         int destIndex = Array.IndexOf(currentArray, destCup);

         //  put the cups back down up to and including the destination index

         sw.WriteLine("Dest Value {0}", destCup);

         sw.WriteLine("Dest Index {0}", destIndex);

         

         sw.WriteLine("Before Place Down c -- {0}", string.Join("", currentArray));

         sw.WriteLine("Before Place Down t -- {0}", string.Join("", targetArray));

         Buffer.BlockCopy(currentArray, 4 * sizeof(int), targetArray, sizeof(int), (destIndex - 3) * sizeof(int));

         sw.WriteLine("After Place Down c -- {0}", string.Join("", currentArray));

         sw.WriteLine("After Place Down t -- {0}", string.Join("", targetArray));

         // Get the values of the 3 cups picked up currentArray[1 - 4]
         for (int i = 1; i < 4; i++){
            // Replace the values in the target Array with them - Offest from the destIndex which was taken from the CurrentArray
            targetArray[destIndex - 3 + i] = currentArray[i];
         }

         sw.WriteLine("Before Switch t -- {0}", string.Join("", targetArray));

         sw.WriteLine("Before Switch c -- {0}", string.Join("", currentArray));

         //  switch the arrays
         if (currentArray == cupNumbers)
         {
            currentArray = cupNumbersCopy;
            targetArray = cupNumbers;
         }else{
            currentArray = cupNumbers;
            targetArray = cupNumbersCopy;
         }

         sw.WriteLine("After Switch t -- {0}", string.Join("", targetArray));

         sw.WriteLine("AfterS witch c -- {0}", string.Join("", currentArray));

         sw.WriteLine("------------------");

         currentCupValue = currentArray[1];

      }

      public static void moveCurrentCup(StreamWriter sw, int currentCupValue, ref int [] cupNumbers, ref int [] cupNumbersCopy, ref int [] currentArray, ref int [] targetArray)
      {
         int targetCupPos = Array.IndexOf(currentArray, currentCupValue);


         int lengthToEnd = currentArray.Length - targetCupPos;

         sw.WriteLine("------------------");

         sw.WriteLine("Before Move t -- {0}", string.Join("", targetArray));

         sw.WriteLine("Before Move c -- {0}", string.Join("", currentArray));

         // Copy from the target int to the end of the current array from the current array to the start of the target array
         Buffer.BlockCopy(currentArray, targetCupPos * sizeof(int), targetArray, 0, lengthToEnd * sizeof(int));

         sw.WriteLine("After Move 1 t -- {0}", string.Join("", targetArray));

         sw.WriteLine("After Move 1 c -- {0}", string.Join("", currentArray));

         // Then copy the ints left from current array (those before the target int) to the end of the target array
         Buffer.BlockCopy(currentArray, 0, targetArray, lengthToEnd * sizeof(int), (currentArray.Length - lengthToEnd) * sizeof(int));

         sw.WriteLine("After Move 2 t -- {0}", string.Join("", targetArray));

         sw.WriteLine("After Move 2 c -- {0}", string.Join("", currentArray));
         
         if (currentArray == cupNumbers)
         {
            currentArray = cupNumbersCopy;
            targetArray = cupNumbers;
         }
         else
         {
            currentArray = cupNumbers;
            targetArray = cupNumbersCopy;
         }
      }
          
         
      static void Main(string[] args){

         // Load input
         List <string> lines = File.ReadLines("puzzle_23_input.txt").ToList();

         // Using a output file for debugging
         string outputFile = "puzzle_23_output.txt"; 
         // Which is wiped at the start of each run to avoid trawling through reams of output
         System.IO.File.WriteAllText(outputFile,string.Empty);

         using(StreamWriter sw = File.AppendText(outputFile))
         {  
                         
            int [] cupNumbers = Array.ConvertAll(lines[0].ToCharArray(), c => (int)Char.GetNumericValue(c));
            
            int [] cupNumbersTwo = new int [cupNumbers.Length];

            // Because we make changes to the cup Numbers array in partOne we need a sperate copy for partTwo to work properly
            cupNumbers.CopyTo(cupNumbersTwo, 0);

            partOne(sw, ref cupNumbers);

            partTwo(ref cupNumbersTwo);
         }
      }
   }
}