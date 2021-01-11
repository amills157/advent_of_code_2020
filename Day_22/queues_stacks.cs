using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace advent_day_22 {
   class puzzle_22 {


      public static void partOne(StreamWriter sw, ref Queue<int> playerOneDeck, ref Queue<int> playerTwoDeck){

         while (playerOneDeck.Count > 0 && playerTwoDeck.Count > 0){

            int playerOneCard = playerOneDeck.Dequeue();
            int playerTwoCard = playerTwoDeck.Dequeue();

            if (playerOneCard > playerTwoCard){
               playerOneDeck.Enqueue(playerOneCard);
               playerOneDeck.Enqueue(playerTwoCard);
            }else if (playerTwoCard > playerOneCard){
               playerTwoDeck.Enqueue(playerTwoCard);
               playerTwoDeck.Enqueue(playerOneCard);
            }
         }

         Queue<int> fullDeck = new Queue<int>();

         int winner = 0;

         if (playerOneDeck.Count > 0){
            fullDeck = playerOneDeck;
            winner = 1;
         }else{
            fullDeck = playerTwoDeck;
            winner = 2;
         }

         int score = 0;

         while (fullDeck.Count > 0){
            // We can use Dequeue here to multiple the top card by the current count - Dequeue returns the top item (pop)
            score += fullDeck.Count * fullDeck.Dequeue();
         }

         Console.WriteLine("Part One Answer:\nPlayer {0} Won! with a score of {1}", winner, score);

      }

      public static bool repeatRoundCheck(StreamWriter sw, Queue<int> pastRound, Queue<int> currentRound){
         
         // If the queues are different size we know they don't match
         if (pastRound.Count != currentRound.Count){
            return false;
         }

         Queue<int> pastRoundCopy = new Queue<int>(pastRound);
         Queue<int> currentRoundCopy = new Queue<int>(currentRound);

         // If the cards aren't the same / in the same order again they don't match
         while (pastRoundCopy.Count > 0 && currentRoundCopy.Count > 0){
            if (pastRoundCopy.Dequeue() != currentRoundCopy.Dequeue()) {
               return false;
            } 
         }
         sw.WriteLine("----------------------------------");
         sw.WriteLine("Past Round -- {0}", string.Join(" ", pastRound));
         sw.WriteLine("Current Rouand -- {0}", string.Join(" ", currentRound));
         sw.WriteLine("----------------------------------");
         return true;
      }

      public static bool roundCeption(StreamWriter sw, Queue<int> playerOneDeck, Queue<int> playerTwoDeck){

         int playerOneCard = playerOneDeck.Dequeue();
         int playerTwoCard = playerTwoDeck.Dequeue();

         // If the decks are >= than the value of the repsective cards, we need to star a sub-game (round-ception)
         if (playerOneDeck.Count >= playerOneCard && playerTwoDeck.Count >= playerTwoCard){
            return false;
         }

         // Otherwise we carry on as we would normally
         if (playerOneCard > playerTwoCard){
            playerOneDeck.Enqueue(playerOneCard);
            playerOneDeck.Enqueue(playerTwoCard);
         }else if (playerTwoCard > playerOneCard){
            playerTwoDeck.Enqueue(playerTwoCard);
            playerTwoDeck.Enqueue(playerOneCard);
         }

         return true;
      }

      public static Queue<int> roundCeptionDecks(StreamWriter sw, int lastCard, Queue<int> currentDeck){

         // Need to create a temp copy so we don't make any changes to the original deck itself
         Queue<int> temp = new Queue<int>(currentDeck);
         Queue<int> newDeck = new Queue<int>();

         for (int i = 0; i < lastCard; i++)
         {
            newDeck.Enqueue(temp.Dequeue());
         }

         sw.WriteLine("Card Value -- {0} -- New Deck {1}", lastCard, string.Join(" ", newDeck));

         return newDeck;
      }

         
      public static void partTwo(StreamWriter sw,  ref Queue<int> playerOneDeck, ref Queue<int> playerTwoDeck){

         //LIFO
         Stack<Queue<int>> gameStack = new Stack<Queue<int>>();

         // A neat new trick I've learnt - You can name variables for access in some classes - But only for basic types - Attempting to do it with Queues etc causes errors at compile
         Stack<(int playerOneLastCard, int playerTwoLastCard)> cardsStack = new Stack<(int playerOneLastCard, int playerTwoLastCard)>();

         Stack<List<Queue<int>>> playerOneStack = new Stack<List<Queue<int>>>();

         Stack<List<Queue<int>>> playerTwoStack = new Stack<List<Queue<int>>>();

         int turns = 1;
         int winner = 0;

         List<Queue<int>> playerOneHistory = new List<Queue<int>>();

         List<Queue<int>> playerTwoHistory = new List<Queue<int>>();

         while (true)
         {

            // Check our default exit condition
            bool repeatRound = false;

            for(int i = 0; i < playerOneHistory.Count; i++){
               Queue<int> playerOnePastDeck = playerOneHistory[i];
               Queue<int> playerTwoPastDeck = playerTwoHistory[i];

               if (repeatRoundCheck(sw, playerOnePastDeck, playerOneDeck) ||  repeatRoundCheck(sw, playerTwoPastDeck, playerTwoDeck)){
                  repeatRound = true;
                  break;
               }
            }


            if (repeatRound){
               //  If the gameStack is empty there are no more rounds to play so we crash out of the while loop
               if (gameStack.Count == 0)
               {
                  winner = 1;
                  sw.WriteLine("Player {0} wins by default -- Repeat Round", winner);
                  break;
               }
               (int playerOneLastCard, int playerTwoLastCard) = cardsStack.Pop();
               // Reset our playerOneHistory 
               playerOneHistory = playerOneStack.Pop();
               playerTwoHistory = playerTwoStack.Pop();
               playerTwoDeck = gameStack.Pop();
               playerOneDeck = gameStack.Pop();
               //  player 1 always wins in case of infinite loop
               playerOneDeck.Enqueue(playerOneLastCard);
               playerOneDeck.Enqueue(playerTwoLastCard);
            }

            playerOneHistory.Add(new Queue<int>(playerOneDeck));
            playerTwoHistory.Add(new Queue<int>(playerTwoDeck));

            // Peek = Dequeue - But doesn't remove the item
            int playerOneCard = playerOneDeck.Peek();
            int playerTwoCard = playerTwoDeck.Peek();

            bool roundComplete = roundCeption(sw, playerOneDeck, playerTwoDeck);

            // We have to go deeper
            if (!roundComplete){

               Queue<int> newPlayerOneDeck = roundCeptionDecks(sw, playerOneCard, playerOneDeck);

               Queue<int> newPlayerTwoDeck = roundCeptionDecks(sw, playerTwoCard, playerTwoDeck);

               Queue<int> stackedDeckOne = new Queue<int>(playerOneDeck);
               Queue<int> stackedDeckTwo = new Queue<int>(playerTwoDeck);

               gameStack.Push(stackedDeckOne); 
               gameStack.Push(stackedDeckTwo);

               List<Queue<int>> tempPlayerOne = new List<Queue<int>>(playerOneHistory);

               playerOneStack.Push(tempPlayerOne);

               List<Queue<int>> tempPlayerTwo = new List<Queue<int>>(playerTwoHistory);

               playerTwoStack.Push(tempPlayerTwo);
               
               cardsStack.Push((playerOneCard, playerTwoCard));
               
               playerOneDeck = newPlayerOneDeck;
               playerTwoDeck = newPlayerTwoDeck;

            }

            if (playerOneDeck.Count == 0 || playerTwoDeck.Count == 0 ){
               
               // Need to check this before we potentially crash out the while loop
               if (playerOneDeck.Count > 0){
                  winner = 1;
               } else {
                  winner = 2;
               }

               // If the gameStack is empty we're not in round-ception so we go to our "final" winner outsie the while loop
               if (gameStack.Count == 0){
                  break;
               }

               (int playerOneLastCard, int playerTwoLastCard) = cardsStack.Pop();
               
               playerOneHistory = playerOneStack.Pop();
               playerTwoHistory = playerTwoStack.Pop();
               playerTwoDeck = gameStack.Pop();
               playerOneDeck = gameStack.Pop();
               
               if (winner == 1){
                  playerOneDeck.Enqueue(playerOneLastCard);
                  playerOneDeck.Enqueue(playerTwoLastCard);
               }else{
                  playerTwoDeck.Enqueue(playerTwoLastCard);
                  playerTwoDeck.Enqueue(playerOneLastCard);
               }
            }
            // Used to track effecenciy - From over 29900 turns down to 47733
            turns++;
         }

         Queue <int> fullDeck = new Queue <int>();

         if (winner == 1){
            fullDeck = playerOneDeck;
         }else if (winner == 2){
            fullDeck = playerTwoDeck;
         }

         sw.WriteLine("Winning Deck -- {0}", string.Join(" ", fullDeck));

         int score = 0;

         while (fullDeck.Count > 0){
         // We can use Dequeue here to multiple the top card by the current count - Dequeue returns the top item (pop)
         score += fullDeck.Count * fullDeck.Dequeue();
         }

         Console.WriteLine("Part Two Answer:\nPlayer {0} Won! with a score of {1} after {2} turns", winner, score, turns);

         
      } 
         
      static void Main(string[] args){

         // Load input
         List <string> lines = File.ReadLines("puzzle_22_input.txt").ToList();

         // Using a output file for debugging
         string outputFile = "puzzle_22_output.txt"; 
         // Which is wiped at the start of each run to avoid trawling through reams of output
         System.IO.File.WriteAllText(outputFile,string.Empty);

         Queue<int> playerOneDeckOriginal = new Queue<int>();
         Queue<int> playerTwoDeckOriginal = new Queue<int>();

         using(StreamWriter sw = File.AppendText(outputFile))
         {  

            //Previously used a bool / int choice within the if statement to select decks, but this is more elegent
            Queue<int> temp = new Queue<int>();

            foreach (string r in lines){
               if (r != ""){

                  if (r.StartsWith("Player 1")){
                     temp = playerOneDeckOriginal;

                  }else if (r.StartsWith("Player 2")){
                     temp = playerTwoDeckOriginal;
                  }else{
                     temp.Enqueue(int.Parse(r));
                  }
               }
            }

            sw.WriteLine("Player One Deck -- {0}", string.Join(" ", playerOneDeckOriginal));

            sw.WriteLine("Player Two Deck -- {0}", string.Join(" ", playerTwoDeckOriginal));

            // Need to pass copies of original decks so we can reuse the decks for game two
            Queue <int> playerOneDeckCopy = new Queue <int>(playerOneDeckOriginal);
            Queue <int> playerTwoDeckCopy = new Queue <int>(playerTwoDeckOriginal);

            partOne(sw, ref playerOneDeckCopy, ref playerTwoDeckCopy);

            partTwo(sw, ref playerOneDeckOriginal, ref playerTwoDeckOriginal);

         }
      }
   }
}