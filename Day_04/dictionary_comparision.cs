using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace advent_day_4 {
   class puzzle_4 {
      static void Main(string[] args) 
      {
         // Load input
         IEnumerable<string> wholeFile = File.ReadAllLines("puzzle_4_input.txt");

         List<string> temp = new List<string>();

         List<string> passports = new List<string>();

         // Using a output file for debugging
         string outputFile = "puzzle_4_output.txt"; 
         // Which is wiped at the start of each run to avoid trawling through reams of output
         System.IO.File.WriteAllText(outputFile,string.Empty);

         string[] reqKeys = new string[] { "byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid" };

         int validCountStrict = 0;
         int validCountLax = 0;

         string value;
         
         using(StreamWriter sw = File.AppendText(outputFile)) 
         {  

            foreach (string r in wholeFile)
            {
               if (r == ""){
                  string passport = string.Join(" ", temp);
                  passports.Add(passport);

                  temp.Clear();

               } else{
                  temp.Add(r);
               }
            }

            string final_passport = string.Join(" ", temp);
            passports.Add(final_passport);

            foreach (string r in passports)
            {
               var dict = r.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries).Select(part => part.Split(':')).ToDictionary(split => split[0], split => split[1]);

               sw.WriteLine(string.Join(Environment.NewLine, dict.Select(a => $"{a.Key}: {a.Value}")));

               bool failedCheckStrict = false;
               bool failedCheckLax = false;
               string failReason = "";

               foreach (string key in reqKeys)
               {

                  if (dict.TryGetValue(key, out value)){
                     switch (key) {
                        case "byr":
                           int byr = Int32.Parse(dict[key]);
                           if (!(byr >= 1920 && byr <= 2002)){
                              failedCheckStrict = true;
                              failReason = "BYR";
                           }
                           break;
                        case "iyr":
                           int iyr = Int32.Parse(dict[key]);
                           if (!(iyr >= 2010 && iyr <= 2020)){
                              failedCheckStrict = true;
                              failReason = "IYR";
                           }
                           break;
                        case "eyr":
                           int eyr = Int32.Parse(dict[key]);
                           if (!(eyr >= 2020 && eyr <= 2030)){
                              failedCheckStrict = true;
                              failReason = "EYR";
                           }
                           break;
                        case "hgt":
                           var hgt = Regex.Match(dict[key], "(\\d+)");
                           int hgt_int = Int32.Parse(hgt.Groups[0].Value);
                           var hgt_unit = Regex.Match(dict[key], "([A-Z|a-z]+)");
                           string hgt_str = hgt_unit.Groups[0].Value;
                           if (hgt_str == "cm")
                           {
                              if (!(hgt_int >= 150 && hgt_int <= 193))
                              {
                                 failedCheckStrict = true;
                                 failReason = "HGT";
                              }
                           } else if (hgt_str == "in")
                           {
                              if (!(hgt_int >= 59 && hgt_int <= 76))
                              {
                                 failedCheckStrict = true;
                                 failReason = "HGT";
                              }
                           }
                           else
                           {
                              failedCheckStrict = true;
                              failReason = "HGT";
                           }
                           break;
                        case "hcl":
                           string hcl = dict[key];
                           if (!(hcl[0] == '#'))
                           {
                              failedCheckStrict = true;
                              failReason = "HCL";
                           }
                           if (!(hcl.Length == 7))
                           {
                              failedCheckStrict = true;
                              failReason = "HCL";
                           }

                           var match = Regex.Match(hcl, "([G-Z|g-z]+)");

                           if (match.Length >=1)
                           {
                              failedCheckStrict = true;
                              failReason = "HCL";
                           }

                           break;
                        case "ecl":
                           string ecl = dict[key];
                           switch (ecl) {
                              case "amb":
                              case "blu":
                              case "brn": 
                              case "gry": 
                              case "grn": 
                              case "hzl": 
                              case "oth":
                                 break;
                                 default:
                              failedCheckStrict = true;
                              failReason = "ECL";
                                 break;
                           }
                           break;
                        case "pid":
                           string pid = dict[key];
                           if (!(pid.Length == 9))
                           {
                              failedCheckStrict = true;
                              failReason = "PID";
                           }

                           break;
                           default:
                        ;
                           break;
                     }
                     
                  } else 
                  {
                     failedCheckStrict = true;
                     failedCheckLax = true;
                     break;
                  }
               }

               if (failedCheckStrict){
                  sw.WriteLine("Failed -- {0}", failReason);
               } else {
                  validCountStrict += 1;
                  sw.WriteLine("Valid -- {0}", validCountStrict);
               }

               if (failedCheckLax){
                  sw.WriteLine("Failed -- Missing Field");
               } else {
                  validCountLax += 1;
                  sw.WriteLine("Valid -- {0}", validCountLax);
               }

               sw.WriteLine("\n-----\n");
            }

            Console.WriteLine("Valid Count (Strict)--{0}", validCountStrict);
            Console.WriteLine("Valid Count (Lax)--{0}", validCountLax);

            
         }
      }
   }
}