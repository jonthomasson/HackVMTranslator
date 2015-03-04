using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace HackVMTranslator
{
    public static class Code
    {
           public static BitArray Dest(string mnemonic){
             var ret = new BitArray(3);

             ret.Set(0, mnemonic.Contains("A"));
             ret.Set(1, mnemonic.Contains("D"));
             ret.Set(2, mnemonic.Contains("M"));

             return ret;
         }

           public static BitArray Jump(string mnemonic)
           {
               var ret = new BitArray(3);

               switch (mnemonic)
               {
                   case "JGT":
                       ret = new BitArray(new bool[] { false, false, true });
                       break;
                   case "JEQ":
                       ret = new BitArray(new bool[] { false, true, false });
                       break;
                   case "JGE":
                       ret = new BitArray(new bool[] { false, true, true });
                       break;
                   case "JLT":
                       ret = new BitArray(new bool[] { true, false, false });
                       break;
                   case "JNE":
                       ret = new BitArray(new bool[] { true, false, true });
                       break;
                   case "JLE":
                       ret = new BitArray(new bool[] { true, true, false });
                       break;
                   case "JMP":
                       ret = new BitArray(new bool[] { true, true, true });
                       break;
               }
               

               return ret;
           }

           public static BitArray Comp(string mnemonic)
           {
               var ret = new BitArray(7);

               switch (mnemonic)
               {
                   case "0":
                       ret = new BitArray(new bool[] { false, true, false, true, false, true, false });
                       break;
                   case "1":
                       ret = new BitArray(new bool[] { false, true, true, true, true, true, true });
                       break;
                   case "-1":
                       ret = new BitArray(new bool[] { false, true, true, true, false, true, false });
                       break;
                   case "D":
                       ret = new BitArray(new bool[] { false, false, false, true, true, false, false });
                       break;
                   case "A":
                       ret = new BitArray(new bool[] { false, true, true, false, false, false, false });
                       break;
                   case "!D":
                       ret = new BitArray(new bool[] { false, false, false, true, true, false, true });
                       break;
                   case "!A":
                       ret = new BitArray(new bool[] { false, true, true, false, false, false, true });
                       break;
                   case "-D":
                       ret = new BitArray(new bool[] { false, false, false, true, true, true, true });
                       break;
                   case "-A":
                       ret = new BitArray(new bool[] { false, true, true, false, false, true, true });
                       break;
                   case "D+1":
                       ret = new BitArray(new bool[] { false, false, true, true, true, true, true });
                       break;
                   case "A+1":
                       ret = new BitArray(new bool[] { false, true, true, false, true, true, true });
                       break;
                   case "D-1":
                       ret = new BitArray(new bool[] { false, false, false, true, true, true, false });
                       break;
                   case "A-1":
                       ret = new BitArray(new bool[] { false, true, true, false, false, true, false });
                       break;
                   case "D+A":
                       ret = new BitArray(new bool[] { false, false, false, false, false, true, false });
                       break;
                   case "D-A":
                       ret = new BitArray(new bool[] { false, false, true, false, false, true, true });
                       break;
                   case "A-D":
                       ret = new BitArray(new bool[] { false, false, false, false, true, true, true });
                       break;
                   case "D&A":
                       ret = new BitArray(new bool[] { false, false, false, false, false, false, false });
                       break;
                   case "D|A":
                       ret = new BitArray(new bool[] { false, false, true, false, true, false, true });
                       break;
                   case "M-1":
                       ret = new BitArray(new bool[] { true, true, true, false, false, true, false });
                       break;
                   case "D+M":
                       ret = new BitArray(new bool[] { true, false, false, false, false, true, false });
                       break;
                   case "D-M":
                       ret = new BitArray(new bool[] { true, false, true, false, false, true, true });
                       break;
                   case "M-D":
                       ret = new BitArray(new bool[] { true, false, false, false, true, true, true });
                       break;
                   case "D&M":
                       ret = new BitArray(new bool[] { true, false, false, false, false, false, false });
                       break;
                   case "D|M":
                       ret = new BitArray(new bool[] { true, false, true, false, true, false, true });
                       break;
                   case "M":
                       ret = new BitArray(new bool[] { true, true, true, false, false, false, false });
                       break;
                   case "!M":
                       ret = new BitArray(new bool[] { true, true, true, false, false, false, true });
                       break;
                   case "-M":
                       ret = new BitArray(new bool[] { true, true, true, false, false, true, true });
                       break;
                   case "M+1":
                       ret = new BitArray(new bool[] { true, true, true, false, true, true, true });
                       break;
               }


               return ret;
           }
    }
}
