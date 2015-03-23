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
        private string FileName { get; set; }
        private string FunctionName { get; set; }
        private int _Id = 0;

        public CodeWriter()
        {
            //initialize code
            InitCode();
        }

        /// <summary>
        /// this code is used to setup our stack pointer and memory locations.
        /// </summary>
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

        /// <summary>
        /// Sets the file name for the current command block.
        /// This is mainly used in writing unique labels
        /// </summary>
        /// <param name="fileName"></param>
        public void SetFileName(string fileName){
            FileName = fileName;
        }

        /// <summary>
        /// Sets the function name for the current command block.
        /// This is mainly used in writing unique labels.
        /// </summary>
        /// <param name="functionName"></param>
        public void SetFunctionName(string functionName)
        {
            FunctionName = functionName;
        }

        /// <summary>
        /// handles parsing the label name together with the class/function names
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public string GetLabelName(string label)
        {
            var labelName = FileName.Split('.')[0] + (string.IsNullOrEmpty(FunctionName) ? "" : "." + FunctionName) + "$" + label;

            return labelName;
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
                    Code.AppendLine("D=D+M"); //set D to RAM[@ARG] + index

                    //get RAM[D]
                    Code.AppendLine("A=D");
                    Code.AppendLine("D=M");

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
                    //first we'll set R13 equal to the value of RAM[SP]
                    Code.AppendLine("@SP");
                    Code.AppendLine("M=M-1");    //decrement the sp
                    Code.AppendLine("A=M"); //set A = Ram[sp]  
                    Code.AppendLine("D=M"); //store value Ram[sp] in D
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
                //use @LCL as the pointer to the ram
                if (command == Enumerations.CommandType.C_PUSH)
                {
                    //push the value of argument[index] onto the stack
                    //get value of argument[index] and put in register D

                    Code.AppendLine("@" + index); //set A to value of index
                    Code.AppendLine("D=A"); //Set D reg to A
                    Code.AppendLine("@LCL"); //the value at @LCL
                    Code.AppendLine("D=D+M"); //set D to RAM[@LCL] + index

                    //get RAM[D]
                    Code.AppendLine("A=D");
                    Code.AppendLine("D=M");

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
                    Code.AppendLine("M=M-1");    //decrement the sp
                    Code.AppendLine("A=M"); //set A = Ram[sp]  
                    Code.AppendLine("D=M"); //store value Ram[sp] in D
                    Code.AppendLine("@R13"); //set A to R13
                    Code.AppendLine("M=D"); //set RAM[13] to D

                    //store in argument[index]
                    Code.AppendLine("@" + index);
                    //Code.AppendLine("A=M"); //set A = RAM[index]

                    Code.AppendLine("D=A"); //Set D reg to A

                    Code.AppendLine("@LCL"); //the value at @LCL
                    Code.AppendLine("A=D+M"); //A = value of LCL plus index
                    Code.AppendLine("D=A");
                    Code.AppendLine("@R14"); //put value of LCL plus index in R14
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
            }
            else if (segment == "this")
            {
                //use @THIS as the pointer to the ram
                if (command == Enumerations.CommandType.C_PUSH)
                {
                    //push the value of THIS[index] onto the stack
                    //get value of THIS[index] and put in register D

                    Code.AppendLine("@" + index); //set A to value of index
                    Code.AppendLine("D=A"); //Set D reg to A
                    Code.AppendLine("@THIS"); //the value at @THIS
                    Code.AppendLine("D=D+M"); //set D to RAM[@THIS] + index

                    //get RAM[D]
                    Code.AppendLine("A=D");
                    Code.AppendLine("D=M");

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
                    Code.AppendLine("M=M-1");    //decrement the sp
                    Code.AppendLine("A=M"); //set A = Ram[sp]  
                    Code.AppendLine("D=M"); //store value Ram[sp] in D
                    Code.AppendLine("@R13"); //set A to R13
                    Code.AppendLine("M=D"); //set RAM[13] to D

                    //store in argument[index]
                    Code.AppendLine("@" + index);
                    //Code.AppendLine("A=M"); //set A = RAM[index]

                    Code.AppendLine("D=A"); //Set D reg to A

                    Code.AppendLine("@THIS"); //the value at @THIS
                    Code.AppendLine("A=D+M"); //A = value of THIS plus index
                    Code.AppendLine("D=A");
                    Code.AppendLine("@R14"); //put value of THIS plus index in R14
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
            }
            else if (segment == "that")
            {
                //use @THAT as the pointer to the ram
                if (command == Enumerations.CommandType.C_PUSH)
                {
                    //push the value of argument[index] onto the stack
                    //get value of argument[index] and put in register D

                    Code.AppendLine("@" + index); //set A to value of index
                    Code.AppendLine("D=A"); //Set D reg to A
                    Code.AppendLine("@THAT"); //the value at @THAT
                    Code.AppendLine("D=D+M"); //set D to RAM[@THAT] + index

                    //get RAM[D]
                    Code.AppendLine("A=D");
                    Code.AppendLine("D=M");

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
                    Code.AppendLine("M=M-1");    //decrement the sp
                    Code.AppendLine("A=M"); //set A = Ram[sp]  
                    Code.AppendLine("D=M"); //store value Ram[sp] in D
                    Code.AppendLine("@R13"); //set A to R13
                    Code.AppendLine("M=D"); //set RAM[13] to D

                    //store in argument[index]
                    Code.AppendLine("@" + index);
                    //Code.AppendLine("A=M"); //set A = RAM[index]

                    Code.AppendLine("D=A"); //Set D reg to A

                    Code.AppendLine("@THAT"); //the value at @THAT
                    Code.AppendLine("A=D+M"); //A = value of THAT plus index
                    Code.AppendLine("D=A");
                    Code.AppendLine("@R14"); //put value of THAT plus index in R14
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
            }
            else if (segment == "static")
            {
                   //static memory starts at RAM[16] and goes to RAM[255]
                if (command == Enumerations.CommandType.C_PUSH)
                {
                    //push the value of argument[index] onto the stack
                    //get value of argument[index] and put in register D

                    Code.AppendLine("@" + index); //set A to value of index
                    Code.AppendLine("D=A"); //Set D reg to A
                    Code.AppendLine("@16"); //the value at @16 which is start of static ram
                    Code.AppendLine("D=D+A"); //set D to @16 + index

                    //get RAM[D]
                    Code.AppendLine("A=D");
                    Code.AppendLine("D=M");  //this should be where we store the value

                    //push onto stack
                    Code.AppendLine("@SP");
                    Code.AppendLine("A=M");
                    Code.AppendLine("M=D");
                    Code.AppendLine("@SP");
                    Code.AppendLine("M=M+1"); //increment stack pointer
                }
                else if (command == Enumerations.CommandType.C_POP)
                {
                    //pop the top stack value and store it in static[index]
                    Code.AppendLine("@SP");
                    Code.AppendLine("M=M-1");    //decrement the sp
                    Code.AppendLine("A=M"); //set A = Ram[sp]  
                    Code.AppendLine("D=M"); //store value Ram[sp] in D

                    Code.AppendLine("@R13"); //set A to R13
                    Code.AppendLine("M=D"); //set RAM[13] to D

                    //store in static[index]
                    Code.AppendLine("@" + index);

                    Code.AppendLine("D=A"); //Set D reg to A

                    Code.AppendLine("@16"); //the value at @16
                    Code.AppendLine("A=D+A"); //A = value of 16 plus index
                    Code.AppendLine("D=A");

                    Code.AppendLine("@R14"); //put value of LCL plus index in R14
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
            }
            else if (segment == "pointer")
            {
                 //setting pointer 0 updates THIS pointer R3
                //setting pointer 1 updates THAT pointer R4
                var ptrFor = index == 0 ? "R3" : "R4";     //determine whether pointer is for THIS (R3) or THAT (R4)

                if (command == Enumerations.CommandType.C_PUSH)
                {
                    //push command (pushes the value of either THIS or THAT pointer onto top of stack
                    

                    //set A to appropriate pointer
                    Code.AppendLine("@" + ptrFor);
                    Code.AppendLine("D=M"); //set D to RAM[ptrFor]
                    //now push value in D onto stack and increment SP
                    //push onto stack
                    Code.AppendLine("@SP");
                    Code.AppendLine("A=M");
                    Code.AppendLine("M=D");
                    Code.AppendLine("@SP");
                    Code.AppendLine("M=M+1"); //increment stack pointer
                }
                else if (command == Enumerations.CommandType.C_POP)
                {
                    //pop command pops the value off the stack and puts it in either THIS or THAT pointer
                    
                    Code.AppendLine("@SP");
                    Code.AppendLine("M=M-1");  //decrement the sp
                    Code.AppendLine("A=M"); //set A = Ram[sp]  
                    Code.AppendLine("D=M"); //store value Ram[sp] in D
                    Code.AppendLine("@" + ptrFor); //set A to pointer
                    Code.AppendLine("M=D"); //set RAM[ptrFor] to D
                }
            }
            else if (segment == "temp")
            {
                //use @R5 as the pointer to the ram
                if (command == Enumerations.CommandType.C_PUSH)
                {
                    //push the value of argument[index] onto the stack
                    //get value of argument[index] and put in register D

                    Code.AppendLine("@" + index); //set A to value of index
                    Code.AppendLine("D=A"); //Set D reg to A
                    Code.AppendLine("@R5"); //the value at @R5
                    Code.AppendLine("D=D+A"); //set D to R5 + index

                    //get RAM[D]
                    Code.AppendLine("A=D");
                    Code.AppendLine("D=M");

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
                    Code.AppendLine("M=M-1");    //decrement the sp
                    Code.AppendLine("A=M"); //set A = Ram[sp]  
                    Code.AppendLine("D=M"); //store value Ram[sp] in D
                    Code.AppendLine("@R13"); //set A to R13
                    Code.AppendLine("M=D"); //set RAM[13] to D

                    //store in argument[index]
                    Code.AppendLine("@" + index);
                    //Code.AppendLine("A=M"); //set A = RAM[index]

                    Code.AppendLine("D=A"); //Set D reg to A

                    Code.AppendLine("@R5"); //the value at @R5
                    Code.AppendLine("A=D+A"); //A = value of arg plus index    todo-changed from A=D+M
                    Code.AppendLine("D=A");
                    Code.AppendLine("@R14"); //put value of R5 plus index in R14
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
            }
        }

        /// <summary>
        /// writes assembly code that effects the goto command.
        /// </summary>
        /// <param name="label"></param>
        public void WriteGoto(string label)
        {

        }

        /// <summary>
        /// writes assembly code that effects the label command.
        /// </summary>
        /// <param name="label"></param>
        public void WriteLabel(string label)
        {
            Code.AppendLine("(" + GetLabelName(label) + ")");  
        }

        /// <summary>
        /// writes assembly code that effects the VM initialization, also called bootstrap code. 
        /// This code must be placed at the beginning of the output file.
        /// </summary>
        public void WriteInit()
        {

        }

        /// <summary>
        /// writes assembly code that effects the if-goto command.
        /// </summary>
        /// <param name="label"></param>
        public void WriteIf(string label)
        {
            //need to pop last value off stack and see if it's 0.
            //if it's > 0, go to the loop start for specified label
            Code.AppendLine("@SP"); //get SP pointer
            Code.AppendLine("A=M-1"); //set D = RAM[SP] - 1
            //not sure why, but seems like we need to decrement stack pointer here... in order to match emulator
            Code.AppendLine("D=A");
            Code.AppendLine("@SP");
            Code.AppendLine("M=M-1");
            Code.AppendLine("A=M");
           
            Code.AppendLine("D=M");
            Code.AppendLine("@" + GetLabelName(label));
            Code.AppendLine("D;JGT");

        }

        /// <summary>
        /// writes assembly code that effects the call command.
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="numArgs"></param>
        public void WriteCall(string functionName, int numArgs)
        {

        }

        /// <summary>
        /// writes assembly code that effects the return command.
        /// </summary>
        public void WriteReturn()
        {

        }

        /// <summary>
        /// writes assembly code that effects the function command.
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="numLocals"></param>
        public void WriteFunction(string functionName, int numLocals)
        {

        }

        public void Close()
        {

        }
    }
}
