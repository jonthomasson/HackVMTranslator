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
        private int _Id = 0;

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

            _Id = 0;
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
                Code.AppendLine("@SP");     //popping current SP - 1
                Code.AppendLine("M=M-1");    //SP is now 257
                Code.AppendLine("A=M");
                Code.AppendLine("D=M");      //D now = 8
                Code.AppendLine("@SP");      //pop again
                Code.AppendLine("M=M-1");    //SP is now 256
                Code.AppendLine("A=M");
                Code.AppendLine("D=M-D");    //do subtraction
                Code.AppendLine("@SP");      //push result to SP ram[256]
                Code.AppendLine("A=M");
                Code.AppendLine("M=D");
                Code.AppendLine("@SP");
                Code.AppendLine("M=M+1");    //increment after push SP now = 257
            }
            else if (command == "neg") //-y
            {

                //1. need to pop last value off SP
                //2. negate the output
                //3. push answer back onto the stack

                Code.AppendLine("@SP");  //popping current SP - 1
                Code.AppendLine("M=M-1");//SP is now 257 which is address of y
                Code.AppendLine("A=M");
                Code.AppendLine("D=-M"); //D now = -y hopefully

                Code.AppendLine("@SP");  //push value back to stack   
                Code.AppendLine("A=M");
                Code.AppendLine("M=D");
                Code.AppendLine("@SP");
                Code.AppendLine("M=M+1"); 
            }
            else if (command == "eq")  //true if x=y else false
            {
                //these should submit 0 if false and -1 if true
                //logical operation for eq:
                //1. subtract x from y
                //2. not the output
                //3. if output is 0 then false anything else then true
                //http://nand2tetris-questions-and-answers-forum.32033.n3.nabble.com/What-is-true-And-what-is-false-td4025881.html
                //http://nand2tetris-questions-and-answers-forum.32033.n3.nabble.com/Logical-operations-td72625.html#a862659

                Code.AppendLine("@SP");     //popping current SP - 1
                Code.AppendLine("M=M-1");    //SP is now 257
                Code.AppendLine("A=M");
                Code.AppendLine("D=M");      //D now = 8
                Code.AppendLine("@SP");      //pop again
                Code.AppendLine("M=M-1");    //SP is now 256
                Code.AppendLine("A=M");
                Code.AppendLine("D=D-M");    //do subtraction
                //if d > 0 
                Code.AppendLine("@ISEQ" + _Id);
                Code.AppendLine("D;JEQ");
                //else  SET D = 0 //false
                Code.AppendLine("D=0");
                Code.AppendLine("@NOTEQ" + _Id);
                Code.AppendLine("0;JMP");

                //set D = -1 //true
                Code.AppendLine("(ISEQ" + _Id + ")");
                Code.AppendLine("D=-1");

                Code.AppendLine("(NOTEQ" + _Id + ")");

                Code.AppendLine("@SP");      //push result to SP ram[256]
                Code.AppendLine("A=M");
                Code.AppendLine("M=D");
                Code.AppendLine("@SP");
                Code.AppendLine("M=M+1");    //increment af
            }
            else if (command == "gt")  //true if x > y else false
            {
                 //logical operation for gt:
                 //1.substract x from y
                 //2. use JGT to determine if x is greater than y
                 //3. if gt then write -1 to register else 0

                Code.AppendLine("@SP");     //popping current SP - 1
                Code.AppendLine("M=M-1");    //SP is now 257
                Code.AppendLine("A=M");
                Code.AppendLine("D=M");      //D now = 8
                Code.AppendLine("@SP");      //pop again
                Code.AppendLine("M=M-1");    //SP is now 256
                Code.AppendLine("A=M");
                Code.AppendLine("D=D-M");    //do subtraction
                //if d > 0 
                Code.AppendLine("@ISGT" + _Id);
                Code.AppendLine("D;JLT");
                //else  SET D = 0 //false
                Code.AppendLine("D=0");
                Code.AppendLine("@NOTGT" + _Id);
                Code.AppendLine("0;JMP");

                //set D = -1 //true
                Code.AppendLine("(ISGT" + _Id + ")");
                Code.AppendLine("D=-1");

                Code.AppendLine("(NOTGT" + _Id + ")");

                Code.AppendLine("@SP");      //push result to SP ram[256]
                Code.AppendLine("A=M");
                Code.AppendLine("M=D");
                Code.AppendLine("@SP");
                Code.AppendLine("M=M+1");    //increment af
            }
            else if (command == "lt") //true if x < y else false
            {
                //logical operation for lt:
                //1.substract x from y
                //2. use JLT to determine if x is less than y
                //3. if lt then write -1 to register else 0

                Code.AppendLine("@SP");     //popping current SP - 1
                Code.AppendLine("M=M-1");    //SP is now 257
                Code.AppendLine("A=M");
                Code.AppendLine("D=M");      //D now = 8
                Code.AppendLine("@SP");      //pop again
                Code.AppendLine("M=M-1");    //SP is now 256
                Code.AppendLine("A=M");
                Code.AppendLine("D=D-M");    //do subtraction
                //if d > 0 
                Code.AppendLine("@ISLT" + _Id);
                Code.AppendLine("D;JGT");
                //else  SET D = 0 //false
                Code.AppendLine("D=0");
                Code.AppendLine("@NOTLT" + _Id);
                Code.AppendLine("0;JMP");

                //set D = -1 //true
                Code.AppendLine("(ISLT" + _Id + ")");
                Code.AppendLine("D=-1");

                Code.AppendLine("(NOTLT" + _Id + ")");

                Code.AppendLine("@SP");      //push result to SP ram[256]
                Code.AppendLine("A=M");
                Code.AppendLine("M=D");
                Code.AppendLine("@SP");
                Code.AppendLine("M=M+1");    //increment af
            }
            else if (command == "and")  //bitwise x and y
            {
                Code.AppendLine("@SP");     //popping current SP - 1
                Code.AppendLine("M=M-1");    //SP is now 257
                Code.AppendLine("A=M");
                Code.AppendLine("D=M");      //D now = 8
                Code.AppendLine("@SP");      //pop again
                Code.AppendLine("M=M-1");    //SP is now 256
                Code.AppendLine("A=M");
                Code.AppendLine("D=D&M");    //do bitwise and
                Code.AppendLine("@SP");      //push result to SP ram[256]
                Code.AppendLine("A=M");
                Code.AppendLine("M=D");
                Code.AppendLine("@SP");
                Code.AppendLine("M=M+1");    //increment after push SP now = 257
            }
            else if (command == "or")  //bitwise x or y
            {
                Code.AppendLine("@SP");     //popping current SP - 1
                Code.AppendLine("M=M-1");    //SP is now 257
                Code.AppendLine("A=M");
                Code.AppendLine("D=M");      //D now = 8
                Code.AppendLine("@SP");      //pop again
                Code.AppendLine("M=M-1");    //SP is now 256
                Code.AppendLine("A=M");
                Code.AppendLine("D=D|M");    //do bitwise or
                Code.AppendLine("@SP");      //push result to SP ram[256]
                Code.AppendLine("A=M");
                Code.AppendLine("M=D");
                Code.AppendLine("@SP");
                Code.AppendLine("M=M+1");    //increment after push SP now = 257
            }
            else if (command == "not")  //bitwise not y
            {
                Code.AppendLine("@SP");     //popping current SP - 1
                Code.AppendLine("M=M-1");    //SP is now 257
                Code.AppendLine("A=M");
                Code.AppendLine("D=M");      //D now = 8
               
                Code.AppendLine("D=!D");    //do bitwise or
                Code.AppendLine("@SP");      //push result to SP ram[256]
                Code.AppendLine("A=M");
                Code.AppendLine("M=D");
                Code.AppendLine("@SP");
                Code.AppendLine("M=M+1");    //increment after push SP now = 257
            }
            _Id++; //increment our unique id for labels/symbols
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
                //use @ARG as the pointer to the ram
                if (command == Enumerations.CommandType.C_PUSH)
                {
                    //push the value of argument[index] onto the stack
                    //get value of argument[index] and put in register D

                    Code.AppendLine("@" + index); //set A to value of index
                    Code.AppendLine("D=A"); //Set D reg to A
                    Code.AppendLine("@ARG"); //the value at @ARG
                    Code.AppendLine("D=M"); //set D to RAM[@ARG]

                    Code.AppendLine("A=D+A"); //A = value of arg plus index
                    Code.AppendLine("D=M"); //Assign reg D to memory address of @ARG  + index

                    //push onto stack
                    Code.AppendLine("@SP");
                    Code.AppendLine("A=M");
                    Code.AppendLine("M=D");
                    Code.AppendLine("@SP");
                    Code.AppendLine("M=M+1"); //increment stack pointer
                }
                else if (command == Enumerations.CommandType.C_POP)
                {
                    //pop the top stack value and store it in argument[index]
                    Code.AppendLine("@SP");
                    Code.AppendLine("M=M-1");
                    Code.AppendLine("A=D"); //set A = D
                    Code.AppendLine("D=A"); //store value in D
                    Code.AppendLine("@R13"); //set A to R13
                    Code.AppendLine("M=D"); //set RAM[13] to D

                    //store in argument[index]
                    Code.AppendLine("@" + index);
                    //Code.AppendLine("A=M"); //set A = RAM[index]
                   
                    Code.AppendLine("D=A"); //Set D reg to A

                    Code.AppendLine("@ARG"); //the value at @ARG
                    Code.AppendLine("A=D+M"); //A = value of arg plus index
                    Code.AppendLine("D=A"); 
                    Code.AppendLine("@R14"); //put value of arg plus index in R14
                    Code.AppendLine("M=D");   //RAM[14] = D
                    //now put value of R14 into memory address that R14 points to
                    //first put R13 into reg D
                    Code.AppendLine("@R13");
                    Code.AppendLine("D=M");
                    //next put value at R13 into RAM[R14]
                    Code.AppendLine("@R14");
                    Code.AppendLine("A=M");
                    Code.AppendLine("M=D"); //memory of RAM[A] = reg D
                    
                }

            }else if (segment == "local")
            {

            }
            else if (segment == "this")
            {

            }
            else if (segment == "that")
            {

            }
            else if (segment == "static")
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
