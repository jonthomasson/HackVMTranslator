using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HackVMTranslator.Enums;

namespace HackVMTranslator.Modules
{
    public class CodeWriter
    {
        private StringBuilder Code { get; set; }

        public CodeWriter()
        {
            //initialize code
            InitCode();
        }

        private void InitCode()
        {
            //setup stack pointer
            Code = new StringBuilder();
            Code.AppendLine("@256");
            Code.AppendLine("D=A");
            Code.AppendLine("@SP");
            Code.AppendLine("M=D");
        }

        public void SetFileName(string fileName){

        }

        /// <summary>
        /// writes artmetic code for a given command. Note: later on will want to come back here and make these more efficient probably.
        /// </summary>
        /// <param name="command"></param>
        public void WriteArithmetic(string command)
        {
            if (command == "add")
            {
                Code.AppendLine("@SP");     //popping current SP - 1
                Code.AppendLine("M=M-1");    //SP is now 257
                Code.AppendLine("A=M");
                Code.AppendLine("D=M");      //D now = 8
                Code.AppendLine("@SP");      //pop again
                Code.AppendLine("M=M-1");    //SP is now 256
                Code.AppendLine("A=M");
                Code.AppendLine("D=D+M");    //do addition
                Code.AppendLine("@SP");      //push result to SP ram[256]
                Code.AppendLine("A=M");
                Code.AppendLine("M=D");
                Code.AppendLine("@SP");
                Code.AppendLine("M=M+1");    //increment after push SP now = 257

            }
            else if (command == "sub") //x - y
            {

            }
            else if (command == "neg") //-y
            {

            }
            else if (command == "eq")  //true if x=y else false
            {

            }
            else if (command == "gt")  //true if x > y else false
            {

            }
            else if (command == "lt") //true if x < y else false
            {

            }
            else if (command == "and")  //bitwise x and y
            {

            }
            else if (command == "or")  //bitwise x or y
            {

            }
            else if (command == "not")  //bitwise not y
            {

            }
        }

        public string ToString()
        {
            return Code.ToString();
        }

        public void WritePushPop(Enumerations.CommandType command, string segment, int index)
        {
             //depending on RAM segment, need to push/pop index appropriately
            if (segment == "constant")
            {
                 //no need to store in ram with this segment
                if (command == Enumerations.CommandType.C_PUSH)
                {
                    Code.AppendLine("@" + index);
                    Code.AppendLine("D=A");
                    Code.AppendLine("@SP");
                    Code.AppendLine("A=M");
                    Code.AppendLine("M=D");
                    Code.AppendLine("@SP");
                    Code.AppendLine("M=M+1");
                }
                else if(command == Enumerations.CommandType.C_POP)
                {
                    Code.AppendLine("@SP");
                    Code.AppendLine("M=M-1");
                    Code.AppendLine("A=M");
                    Code.AppendLine("D=M");
                }
            }
            else if (segment == "argument")
            {

            }else if (segment == "local")
            {

            }
            else if (segment == "static")
            {

            }
            else if (segment == "this")
            {

            }
            else if (segment == "that")
            {

            }
            else if (segment == "pointer")
            {

            }
            else if (segment == "temp")
            {

            }
        }

        public void Close()
        {

        }
    }
}
