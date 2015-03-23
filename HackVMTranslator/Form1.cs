using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HackVMTranslator.Modules;
using System.Collections;

namespace HackVMTranslator
{
    //nand2tetris forum pages that helped with this project:
    //http://nand2tetris-questions-and-answers-forum.32033.n3.nabble.com/A-question-before-starting-td4027143.html#a4027148
    //http://nand2tetris-questions-and-answers-forum.32033.n3.nabble.com/JackCompiler-gt-Vm2Asm-gt-Assembler-td4026189.html#a4026207
    //http://nand2tetris-questions-and-answers-forum.32033.n3.nabble.com/Stuck-on-7-5-project-stage-I-td2277392.html#a3484994
    //http://nand2tetris-questions-and-answers-forum.32033.n3.nabble.com/Problem-with-arithmetic-implementation-of-codewriter-td4028049.html#a4028050
    
    public partial class Form1 : Form
    {
        private string _fileName = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {


            var dlg = new OpenFileDialog();

            dlg.Filter = "Virtual Machine (*.vm)|*.vm";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _fileName = dlg.FileName;
                lblStatus.Text = "File Loaded Successfully: " + _fileName;
                frmStatus.Refresh();
            }






        }

        /// <summary>
        /// first pass will parse labels and add them to the symbol table
        /// </summary>
        private SymbolTable FirstPass()
        {
            var symbols = new SymbolTable();//instantiate symbol table

            if (!string.IsNullOrEmpty(_fileName))
            {
                var prsr = new Parser(_fileName);
                var sbsource = new StringBuilder(); //used to hold source text

                int lineNo = 0; //used to track line number of current command

                lblStatus.Text = "Adding Labels to Symbol Table... ";
                frmStatus.Refresh();

                while (prsr.HasMoreCommands)
                {

                    prsr.Advance();
                    sbsource.AppendLine(prsr.CurrentCommand);

                
                }

                prsr.GarbageCollection();
                rtbSource.Text = sbsource.ToString();
            }

            lblStatus.Text = "File " + _fileName + " Parsed Successfully.";
            frmStatus.Refresh();

            return symbols;
        }

        /// <summary>
        /// second pass will parse line by line and add remaining variables to symbol table
        /// </summary>
        private void SecondPass(SymbolTable symbols)
        {
            if (!string.IsNullOrEmpty(_fileName))
            {
                var prsr = new Parser(_fileName);
                var sbuilder = new StringBuilder(); //used to hold our output

                while (prsr.HasMoreCommands)
                {

                    prsr.Advance();

                    lblStatus.Text = "Parsing Line: " + sbuilder.Length;
                    frmStatus.Refresh();

                }

                prsr.GarbageCollection();
                rtbDestination.Text = sbuilder.ToString();
            }

            lblStatus.Text = "File " + _fileName + " Parsed Successfully.";
            frmStatus.Refresh();
        }

        private void ParseFile()
        {
            if (!string.IsNullOrEmpty(_fileName))
            {
                var prsr = new Parser(_fileName);
                var cw = new CodeWriter();
                cw.SetFileName(System.IO.Path.GetFileName(_fileName));
                var sbuilder = new StringBuilder(); //used to hold our output
                var sbsource = new StringBuilder();

                while (prsr.HasMoreCommands)
                {

                    prsr.Advance();
                    sbsource.AppendLine(prsr.CurrentCommand);

                    lblStatus.Text = "Parsing Line: " + sbuilder.Length;
                    frmStatus.Refresh();

                    if (prsr.CommandType == Enums.Enumerations.CommandType.C_ARITHMETIC)
                    {
                        //process arithmetic command
                        cw.WriteArithmetic(prsr.Arg1);
                    }
                    else if (prsr.CommandType == Enums.Enumerations.CommandType.C_PUSH || prsr.CommandType == Enums.Enumerations.CommandType.C_POP)
                    {
                        cw.WritePushPop(prsr.CommandType, prsr.Arg1, int.Parse(prsr.Arg2));
                    }
                    else if (prsr.CommandType == Enums.Enumerations.CommandType.C_GOTO)
                    {
                        cw.WriteGoto(prsr.Arg1);
                    }
                    else if (prsr.CommandType == Enums.Enumerations.CommandType.C_IF)
                    {
                        cw.WriteIf(prsr.Arg1);
                    }
                    else if (prsr.CommandType == Enums.Enumerations.CommandType.C_LABEL)
                    {
                        cw.WriteLabel(prsr.Arg1);
                    }
                    else if (prsr.CommandType == Enums.Enumerations.CommandType.C_RETURN)
                    {
                        cw.WriteReturn();
                    }
                    else if (prsr.CommandType == Enums.Enumerations.CommandType.C_CALL)
                    {
                        cw.WriteCall(prsr.Arg1, int.Parse(prsr.Arg2));
                    }
                    else if (prsr.CommandType == Enums.Enumerations.CommandType.C_FUNCTION)
                    {
                        cw.SetFunctionName(prsr.Arg1);
                        cw.WriteFunction(prsr.Arg1, int.Parse(prsr.Arg2));
                    }

                }

                prsr.GarbageCollection();
                rtbDestination.Text = cw.ToString();
            }

            lblStatus.Text = "File " + _fileName + " Parsed Successfully.";
            frmStatus.Refresh();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            //read one line at a time from rtbSource and parse it with appropriate modules
            rtbDestination.Clear();
            ParseFile();


        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            //open save file dialog
            var savefile = new SaveFileDialog();
            // set a default file name
            var fileName = System.IO.Path.GetFileName(_fileName).Replace("vm", "asm");
            savefile.FileName = fileName;

            // set filters - this can be done in properties as well
            savefile.Filter = "Assembly files (*.asm)|*.asm*";

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                rtbDestination.SaveFile(savefile.FileName, RichTextBoxStreamType.PlainText);

                lblStatus.Text = "File " + savefile.FileName + " Saved Successfully.";
                frmStatus.Refresh();
            }
        }


    }
}
