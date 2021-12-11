using System;
using System.Collections.Generic;

namespace LmcSimulator
{
    internal class AsmCompiler
    {
        private List<string> AsmCode;
        private ushort[] MachineRAM = new ushort[100];
        
        public AsmCompiler(string[] asmCode)
        {
            AsmCode = new List<string>(asmCode);
        }
        public void Compile()
        {
            int COMPILER_CurAddrPointer = 0;
            var mnemonics = new string[] { "HLT", "ADD", "SUB", "STA", "LDA", "BRA", "BRZ", "BRP", "INP", "OUT", "OTC", "DAT" };
                                        //  000 ,  1xx ,  2xx ,  3xx ,  5xx ,  6xx ,  7xx ,  8xx ,  901 ,  902 ,  922

            List<string[]> splitAsmList = new() { };
            //splits string[] AsmCode into the string[][] splitAsm containing [[line_1_word_1, line_1_word_2], [line_2_word_1, line_2_word_2]] ignoring spaces

            foreach (var line in AsmCode)
            {
                string[] lineSplitEmptyStrs = line.Split(" ");
                List<string> lineSplit_list = new() { };
                foreach (var word in lineSplitEmptyStrs)
                {
                    if (!(string.IsNullOrWhiteSpace(word)) )
                    {

                        lineSplit_list.Add(word);
                    }
                    
                }
                if (lineSplit_list.Count > 0)
                {
                    splitAsmList.Add(lineSplit_list.ToArray());
                }
                
            }

            string[][] splitAsm = splitAsmList.ToArray();
            //address label finder
            Dictionary<string, ushort> labelDict = new() { };

            foreach (string[] line in splitAsm)
            {
                // checks for any label definitions which are the first word in a line
                if (line[0].IsLabel())
                {
                    labelDict.Add(line[0], (ushort) COMPILER_CurAddrPointer);
                    //Todo: add error if label is used multiple times
                }
                COMPILER_CurAddrPointer++;
            }
            COMPILER_CurAddrPointer = -1;
            //convert to machine code
            foreach (string[] line in splitAsm)
            {
                COMPILER_CurAddrPointer++;
                ushort curMachineInstrc = 0;

                bool dataNeeded = false;
                bool dataWanted = false;
                bool instrcNeeded = true;
                for (int index = 0; index < line.Length; index++)
                {
                    string word = line[index];
                    //ignores line label declarations
                    if (index == 0 && word.IsLabel())
                    {
                        continue;
                    }
                    if (instrcNeeded)
                    {
                        switch (word.ToUpper())
                        {
                            case "HLT":
                                curMachineInstrc = 000;
                                dataNeeded = false;
                                dataWanted = false;
                                break;
                            case "ADD":
                                curMachineInstrc= 100;
                                dataNeeded = true;
                                dataWanted = false;
                                break;
                            case "SUB":
                                curMachineInstrc = 200;
                                dataNeeded = true;
                                dataWanted = false;
                                break;
                            case "STA":
                                curMachineInstrc = 300;
                                dataNeeded = true;
                                dataWanted = false;
                                break;
                            case "LDA":
                                curMachineInstrc = 500;
                                dataNeeded = true;
                                dataWanted = false;
                                break;
                            case "BRA":
                                curMachineInstrc = 600;
                                dataNeeded = true;
                                dataWanted = false;
                                break;
                            case "BRZ":
                                curMachineInstrc = 700;
                                dataNeeded = true;
                                dataWanted = false;
                                break;
                            case "BRP":
                                curMachineInstrc = 800;
                                dataNeeded = true;
                                dataWanted = false;
                                break;
                            case "INP":
                                curMachineInstrc = 901;
                                dataNeeded = false;
                                dataWanted = false;
                                break;
                            case "OUT":
                                curMachineInstrc = 902;
                                dataNeeded = false;
                                dataWanted = false;
                                break;
                            case "OTC":
                                curMachineInstrc = 922;
                                dataNeeded = false;
                                dataWanted = false;
                                break;
                            case "DAT":
                                dataNeeded = false;
                                dataWanted = true;
                                break;
                            default:
                                throw new SyntaxException("instruction expected, recieved data",COMPILER_CurAddrPointer,index);
                        }
                        instrcNeeded = false;
                    }
                    else if (dataNeeded || dataWanted)
                    {
                        ushort data;
                        //data can either be a number directly or it can be a label
                        if (word.IsData())
                        {
                             data = ushort.Parse(word);
                            if (data < 0 || data > 999)
                            {
                                throw new AddressBoundsException("data/address entered is out of range 0 to 999",COMPILER_CurAddrPointer,index);
                            }
                        }
                        else if (word.IsLabel())
                        {
                            //if the label used exists
                            if (labelDict.ContainsKey(word))
                            {
                                data = labelDict[word];
                            }
                            else
                            {
                                throw new UndeclaredLabelException($"\"{word}\" is not a defined label", COMPILER_CurAddrPointer, index);
                            }


                        }
                        else
                        {
                            throw new SyntaxException("Data or Address expected received a Mnemonic",COMPILER_CurAddrPointer, index);
                        }
                        //check in range of addresses and then add to RAM address
                        if (data < 0 || data > 999)
                        {
                            throw new AddressBoundsException("line continues after it should end", COMPILER_CurAddrPointer, index);
                        }
                        curMachineInstrc += data;
                        dataNeeded = false;
                        dataWanted = false;
                    }
                    else
                    {
                        throw new SyntaxException("Line expected to end yet it was continued",COMPILER_CurAddrPointer, index);
                    }
                }

                if (instrcNeeded)
                {
                    throw new SyntaxException("Line ends when instruction was expected", COMPILER_CurAddrPointer, line.Length);
                }
                else if (dataNeeded)
                {
                    throw new SyntaxException("Line ends when an address was expected", COMPILER_CurAddrPointer, line.Length);
                }
                MachineRAM[COMPILER_CurAddrPointer] = curMachineInstrc;
                curMachineInstrc = 0;
            }
            
        }
        public ushort[] GetMachineRAM()
        {
            return MachineRAM; 
        }
        public void DisplayRAMContents()
        {

            Console.WriteLine("--------------------------------------------RAM--------------------------------------------");
            for (int i = 0; i < 10; i++)
            {
                Console.Write("|");
                for (int x = 0; x < 10; x++)
                {
                    int address = i * 10 + x;
                    Console.Write($"{address.ToString().PadLeft(2, '0')} - {MachineRAM[address].ToString().PadLeft(3, '0')}|");
                }
                Console.WriteLine();
            }
            Console.WriteLine("-------------------------------------------------------------------------------------------");
        }
    }
}