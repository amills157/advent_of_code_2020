using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace advent_day_14 {
   class puzzle_14 {      

      private static IEnumerable<string> GetVariations(string word)
      {
         if(!word.Any(c => c.Equals('X')))
         {
               return new[] { word };
         } 
         else
         {
               string newWord1 = ReplaceFirstMatch(word, "X", "0");
               string newWord2 = ReplaceFirstMatch(word, "X", "1");
               return GetVariations(newWord1).Concat(GetVariations(newWord2));                
         }
      }

      private static string ReplaceFirstMatch(string theString, string oldValue, string newValue)
      {
         int loc = theString.IndexOf(oldValue);
         if(loc < 0)
         {
            return theString;
         }
         return theString.Remove(loc, oldValue.Length).Insert(loc, newValue);
      }            

      public static  void partOne(StreamWriter sw, ref  List <long> bitValues, ref IEnumerable<string> lines){

         Dictionary<string, long> memDict = new Dictionary<string, long>();

         List <char> bitMask = new List <char>();

         sw.WriteLine(string.Join(" ", bitValues));

         foreach(string r in lines){
            sw.WriteLine(r);

            string [] splitStrings = r.Split("=");

            if(splitStrings[0].Contains("mask")){

               char[] charArr = splitStrings[1].Replace(" ", string.Empty).ToCharArray();

               bitMask = charArr.ToList();

            }else{
               
               var match = Regex.Match(splitStrings[0], "([0-9]+)");

               string dictKey = match.Groups[1].Value;

               long toConvert = Convert.ToInt64(splitStrings[1]);

               List <int> valueMask = new List <int>();

               foreach(long value in bitValues){
                  if(value > toConvert){
                     valueMask.Add(0);
                  } else {
                     toConvert = toConvert - value;
                     valueMask.Add(1);
                  }
               }

               sw.WriteLine(string.Join(" ", valueMask));

               long decimalResult = 0;

               for(int i = 0; i < bitMask.Count; i++){
                  if(bitMask[i] != 'X'){

                     if(bitMask[i] == '1' && valueMask[i] == 0){
                        valueMask[i] = 1;
                     }

                     if(bitMask[i] == '0' && valueMask[i] == 1){
                        valueMask[i] = 0;
                     }
                  }
                  if(valueMask[i] != 0){
                     decimalResult += bitValues[i];
                  }
               }

               sw.WriteLine(string.Join(" ", valueMask));
               sw.WriteLine(decimalResult);

               memDict[dictKey] = decimalResult;

            }
            
         }

         long total = 0;

         foreach(KeyValuePair<string, long> entry in memDict){
            sw.WriteLine("{0} = {1}", entry.Key, entry.Value);
            total += entry.Value;
         }

         Console.WriteLine("Part One Answer {0}", total);

      }
      
      static void Main(string[] args) 
      {
         // Load input
         IEnumerable<string> lines = File.ReadLines("puzzle_14_test_input_2.txt");

          // Using a output file for debugging
         string outputFile = "puzzle_14_output.txt"; 
         // Which is wiped at the start of each run to avoid trawling through reams of output
         System.IO.File.WriteAllText(outputFile,string.Empty);

         using(StreamWriter sw = File.AppendText(outputFile)) 
         {  
            List <long> bitValues = new List <long>{1};

            for(int i = 1; i < 36; i ++){
               bitValues.Add(bitValues[i-1]*2);
            }

            bitValues.Reverse();

            List <char> bitMask = new List <char>();

            //partOne(sw, ref bitValues, ref lines);

            Dictionary<string, List<int>> memDict = new Dictionary<string, List <int>>();

            foreach(string r in lines){
               sw.WriteLine(r);

               string [] splitStrings = r.Split("=");

               if(splitStrings[0].Contains("mask")){

                  char[] charArr = splitStrings[1].Replace(" ", string.Empty).ToCharArray();

                  bitMask = charArr.ToList();

               }else{
                  
                  var match = Regex.Match(splitStrings[0], "([0-9]+)");

                  string dictKey = match.Groups[1].Value;

                  long memAddress = Convert.ToInt64(dictKey);

                  long toConvert = Convert.ToInt64(splitStrings[1]);

                  List <int> memAddressMask = new List <int>();

                  List <List <char>> floatingBitMasks = new List <List <char>>();

                  foreach(long value in bitValues){
                     if(value > memAddress){
                        memAddressMask.Add(0);
                     } else {
                        memAddress = memAddress - value;
                        memAddressMask.Add(1);
                     }
                  }

                  List <int> valueMask = new List <int>();

                  foreach(long value in bitValues){
                     if(value > toConvert){
                        valueMask.Add(0);
                     } else {
                        toConvert = toConvert - value;
                        valueMask.Add(1);
                     }
                  }

                  for(int i = 0; i < memAddressMask.Count; i++){
                     if(bitMask[i] != 'X'){

                        if(memAddressMask[i] == 0 && bitMask[i] == '1'){
                           bitMask[i] = '1';
                        }

                        if(memAddressMask[i] == 1 && bitMask[i] == '0'){
                           bitMask[i] = '1';
                        }
                     }
                  }

                  // 2840973881433 too high
                  
                  // 1929359196296 too low

                  foreach(string variation in GetVariations(string.Join("", bitMask)))
                  {
                     //sw.WriteLine(variation);

                     long memAddr = 0;

                     char [] temp = variation.ToCharArray();

                     for(int i = 0; i < temp.Length; i++){
                        if(temp[i] != '0'){
                           memAddr += bitValues[i];
                        }
                     }

                     if(memDict.ContainsKey(memAddr.ToString())){
                        for(int i = 0; i < temp.Length; i++){
                           if(memDict[memAddr.ToString()][i] == 0 && valueMask[i] == 1){
                              valueMask[i] = 1;
                           }

                           if(memDict[memAddr.ToString()][i] == 1 && valueMask[i] == 0){
                              valueMask[i] = 0;
                           }
                        }

                     }

                     memDict[memAddr.ToString()] = valueMask;
                  }

               }
            
            }
            
            long total = 0;

            foreach(KeyValuePair<string, List<int>> entry in memDict){

               for(int i = 0; i < entry.Value.Count; i++){

                  if(entry.Value[i] == 1){
                     total += bitValues[i];
                  }
               }
               sw.WriteLine("Key = {0}, Value = {1}, Total ={2}", entry.Key, string.Join("",entry.Value), total);

               sw.WriteLine(total);
            }

            Console.WriteLine("Part Two Answer {0}", total);
            
 
         }

      }
   }
}