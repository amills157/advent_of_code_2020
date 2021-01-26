using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace advent_day_19 {
   class puzzle_19 {
      
      // Initially these were passed as arguments - But as partTwo is recursive that causes issues - So had to go with a global usage for these two
      public static Dictionary<string, List<string>> originalRuleDict = new Dictionary<string, List<string>>();

      public static Dictionary<string, List<string>> newRuleDict = new Dictionary<string, List<string>>();

      public static bool partTwo(StreamWriter sw, string message, List<string> rule)
      {
         sw.WriteLine("Rule -- {0}", string.Join("-",rule));
         
         // If the rule is longer than the message it's not valid
         if (rule.Count > message.Length){ 
            return false; 
         }

         // If the rule count is 0 but our message isn't 
         if (rule.Count == 0){
            if(message.Length == 0){ 
               return true;
            } else {
               return false;
            }
         }

         // Check to see if we have a number or a letter
         if (Regex.IsMatch(rule[0], "([0-9])"))
         {
            // If its a number we to find the rule it matches

            List<string> rulesFromDict = new List<string>();

            if(originalRuleDict.ContainsKey(rule[0])){
               rulesFromDict = originalRuleDict[rule[0]];
            } else {
               rulesFromDict = newRuleDict[rule[0]];
            }

            // Get the matching rules for that rule            
            foreach (string returnedRule in rulesFromDict){

               // COnvert to list and remove the whitespace
               List<string> ruleList = returnedRule.Split(" ",StringSplitOptions.RemoveEmptyEntries).ToList();
               
               sw.WriteLine("ruleList -- {0}", string.Join("-",ruleList));

               // Get the outer / parent rules - But remove the rule we have subsituted for it's own rules
               List<string> parentRuleList = new List<string>(rule.Skip(1));
               
               // Add them all into one list
               ruleList.AddRange(parentRuleList);

               // Run again - eventually we get down to letters
               if (partTwo(sw, message, ruleList))
               {
                  return true;
               }
            }

         }else{
            // It's a letter and we can do a comparision
            if (message[0].ToString() == rule[0]){
               // If the first letters of both the rule and message match then we go again to test the next of each

               string newMessage = message.Substring(1);

               List<string> newRule = new List<string>(rule.Skip(1));

               return partTwo(sw, newMessage, newRule);
            // Otherwise it's not valid and we can just return false
            }else{
               return false;
            }
         }

         return false;
      }
     
      public static void partOne(StreamWriter sw, ref Dictionary<string, List<string>> originalRuleDict, ref Dictionary<string, List<string>> newRuleDict, ref HashSet<string> receivedMessages){

         int validCount = 0;
         
         // While rule zero has not yet had been added to the new dictionary we loop through doing rule replacement
         //int passCount = 0;
         while (!newRuleDict.ContainsKey("0"))
         {  

            //Console.WriteLine("Pass = {0}", passCount);

            //passCount += 1;
            // Because we'll be itterating through the original dict we can't make changes to the dictionary directly so we need a temp dict   
            Dictionary<string, List<string>> tempDict = new Dictionary<string, List<string>>();

            // We need to loop through our original rules and then replace the original rules for those in the new rules (reducded down to their a / b values) 
            foreach (KeyValuePair<string, List<string>> entry in originalRuleDict)
            {
               // Copy the entry value into a list we can edit etc
               List<string> entryValue = new List<string>(entry.Value);

               foreach(string str in entry.Value)
               {
                  sw.WriteLine(str);
                  List<string> newValues = new List<string>();

                  foreach(KeyValuePair<string, List<string>> newEntry in newRuleDict)
                  {  
                     // If the string contains a key present in our newDict we know that rule can be swapped out for the replacement a / b value(s)
                     if (str.Contains(" " + newEntry.Key + " "))
                     {
                        // We then replace the old value (the dict key) with the a / b value rule - Maintaining the spacing otherwise we get joined up values / rules
                        foreach (string repValue in newEntry.Value)
                        {
                           newValues.Add(str.Replace(" " + newEntry.Key + " ", " " + repValue + " "));
                        }
                        break;
                     }
                     // Didn't really add anything except reams of a and b
                     //sw.WriteLine(string.Join("",newValues));
                  }
                  // If we have newValues to add then we remove the old str from the dict entry and add the new value
                  if (newValues.Count > 0)
                  {
                     entryValue.Remove(str);
                     foreach(string newStr in newValues){
                        sw.WriteLine(newStr);
                        entryValue.Add(newStr);
                     }
                  }

               }
               tempDict[entry.Key] = entryValue;
            }
            // We then do a "clear" of the old originalRules and copy in the new versions
            originalRuleDict = new Dictionary<string, List<string>>(tempDict);

            // We need a second loop to then move them from original to new as the rules might only be partially expanded (i.e. a 1 b)
            foreach(KeyValuePair<string, List<string>> entry in originalRuleDict){

               // Make sure that our rules are down to a / b values only 
               if (!(string.Join(" ", entry.Value).Any(char.IsDigit))){
                  for (int i = 0; i < entry.Value.Count; i++)
                  {
                     string condensedVal = entry.Value[i].Trim() + " ";
                     entry.Value[i] = condensedVal;
                  }
                  newRuleDict[entry.Key] = entry.Value;
               } else {
                  sw.WriteLine("----------------------------");
                  sw.WriteLine(string.Join("", entry.Value));
                  sw.WriteLine("----------------------------");
               }
            }
            
            //  We need a another loop to remove entries from the original dictionary
            foreach (KeyValuePair<string, List<string>> entry in  newRuleDict)
            {
               if (originalRuleDict.ContainsKey(entry.Key)){
                  originalRuleDict.Remove(entry.Key);
               }
            }
         }

         // Once we have rule zero we remove the trailing space left over from the replacement processs and convert the list to a hashset so we are only left with unique values

         List <string> temp = newRuleDict["0"].Select(s => s.Replace(" ", string.Empty)).ToList();

         HashSet<string> uniqueValidMessages = new HashSet<string>(temp);

         // Seperate loop using hashsets as doing a by value check directly from rule 0 has around 900 matches - Likely due to duplicates
         foreach(string message in uniqueValidMessages){
            if(receivedMessages.Contains(message)){
               validCount += 1;
               sw.WriteLine(message);
            }
         }

         Console.WriteLine("PartOne Answer --{0}", validCount);
      }
      
      static void Main(string[] args) 
      {
         // Load input
         List <string> lines = File.ReadLines("puzzle_19_input.txt").ToList();

          // Using a output file for debugging
         string outputFile = "puzzle_19_output.txt"; 
         // Which is wiped at the start of each run to avoid trawling through reams of output
         System.IO.File.WriteAllText(outputFile,string.Empty);
         
         HashSet<string> receivedMessages = new HashSet<string>(); 


         using(StreamWriter sw = File.AppendText(outputFile)) 
         {  
            
            foreach(string r in lines){

               if (r.Contains(":")){  
                  
                  // New list each time which avoids issues with either very nested lists or rules being hard to seperate later on
                  List<string> ruleList = new List<string>();

                  string[] splitString = r.Split(": ");
                  // These are our original / "end" rules (a or b)
                  var match = Regex.Matches(splitString[1], "([a-z])");
                  if(match.Count > 0){
                     sw.WriteLine(splitString[1][1].ToString().Trim());
                     ruleList.Add(splitString[1][1].ToString().Trim() + " ");
                     newRuleDict.Add(splitString[0], ruleList);
                  }
                  else{
                     string[] splitRules = splitString[1].Split(" | ");
                     foreach (var rule in splitRules)
                     {
                        ruleList.Add(" " + rule + " ");
                     }

                     sw.WriteLine(string.Join(" ", ruleList));
                  
                     originalRuleDict.Add(splitString[0], ruleList);
                  }
               }
               else
               {
                  receivedMessages.Add(r);
               }
            
            }

            // We need to copy the dictionaries for partOne not to create issues with the way partTwo works. 
            // PartOne could just be removed - But as this is being used for learning c# it makes sense to keep both
            Dictionary<string, List<string>> copyOriginalRuleDict = new Dictionary<string, List<string>>(originalRuleDict);

            Dictionary<string, List<string>> copyNewRuleDict = new Dictionary<string, List<string>>(newRuleDict);

            partOne(sw, ref copyOriginalRuleDict, ref copyNewRuleDict, ref receivedMessages);

            // This only needs to be once - So we do it here to avoid errors at the point partTwo becomes recursive 
            List<string> ruleZero = originalRuleDict["0"][0].Split(" ",StringSplitOptions.RemoveEmptyEntries).ToList();

            // Make changes to the rules 8 and 11 in line with partTwo requirements 
            originalRuleDict["8"].Add(" " + "42 8" + " ");

            originalRuleDict["11"].Add(" " + "42 11 31" + " ");
            
            
            int total = 0;
            foreach (string msg in receivedMessages)
            {
               bool checksOut = partTwo(sw, msg, ruleZero);
               if (checksOut)
               {
                  total++;
               }
            }

            Console.WriteLine("PartTwo Answer --{0}", total);
                   
         }

      }
   }
}