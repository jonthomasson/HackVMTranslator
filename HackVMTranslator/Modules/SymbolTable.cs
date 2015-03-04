using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackVMTranslator
{
    public class SymbolTable
    {
        private Dictionary<string, int> _symbolTable ;

        public SymbolTable()
        {
            _symbolTable = new Dictionary<string, int>();//create a new empty symbol table
            //add default addresses
            _symbolTable.Add("SP", 0);
            _symbolTable.Add("LCL", 1);
            _symbolTable.Add("ARG", 2);
            _symbolTable.Add("THIS", 3);
            _symbolTable.Add("THAT", 4);
            _symbolTable.Add("R0", 0);
            _symbolTable.Add("R1", 1);
            _symbolTable.Add("R2", 2);
            _symbolTable.Add("R3", 3);
            _symbolTable.Add("R4", 4);
            _symbolTable.Add("R5", 5);
            _symbolTable.Add("R6", 6);
            _symbolTable.Add("R7", 7);
            _symbolTable.Add("R8", 8);
            _symbolTable.Add("R9", 9);
            _symbolTable.Add("R10", 10);
            _symbolTable.Add("R11", 11);
            _symbolTable.Add("R12", 12);
            _symbolTable.Add("R13", 13);
            _symbolTable.Add("R14", 14);
            _symbolTable.Add("R15", 15);
            _symbolTable.Add("SCREEN", 16384);
            _symbolTable.Add("KBD", 24576);



        }

        /// <summary>
        /// adds the pair (symbol, address) to the table
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="address"></param>
        public void AddEntry(string symbol, int address)
        {
            _symbolTable.Add(symbol, address);
        }

        /// <summary>
        /// does the symbol table contain the given symbol?
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public bool Contains(string symbol)
        {
            int item;
            return _symbolTable.TryGetValue(symbol, out item);
        }

        /// <summary>
        /// Returns the address associated with the symbol
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public int GetAddress(string symbol)
        {
            return _symbolTable.FirstOrDefault(x => x.Key == symbol).Value;
        }
    }
}
