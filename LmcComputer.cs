using System;

namespace LmcSimulator
{
    public class LmcComputer
    {
        private ushort[] RAM = new ushort[99];
        private byte PC; //Program Counter
        private byte IR; //Instruction Register
        private byte AR; //Address Register
        private ushort ACC;//Accumulator
        private bool ACC_Neg_Flag = false; //when false value in acc is positive
        private ushort INP;
        private ushort OUT;
        public bool Running = true;

        public LmcComputer()
        {
            PC = 0;
            IR = 0;
            ACC = 0;
        }
        public void LoadRAM(ushort[] ram)
        {
            RAM = ram;
        }
        public void Run()
        {
            while (Running)
            {
                Step();
            }
        }
        public void Step()
        {
            FDE_Fetch();
            FDE_Decode_Execute();
        }
        private void FDE_Fetch()
        {
            //get instruction address PC from RAM, load opcode into IR, Operand into AR
            IR = (byte) (RAM[PC] / (ushort)100); //the 100's digit represents the opcode so integer division of nxx/100 will give n
            AR = (byte)(RAM[PC] % 100); // the remainder when divided by 100 will give the operand (10's and 1's)
            //increment PC
            PC++;
        }
        private void FDE_Decode_Execute()
        {
            switch (IR)
            {
                case 0:
                    MICROCODE_HLT();
                    break;
                case 1:
                    MICROCODE_ADD();
                    break;
                case 2:
                    MICROCODE_SUB();
                    break;
                case 3:
                    MICROCODE_STA();
                    break;
                case 5:
                    MICROCODE_LDA();
                    break;
                case 6:
                    MICROCODE_BRA();
                    break;
                case 7:
                    MICROCODE_BRZ();
                    break;
                case 8:
                    MICROCODE_BRP();
                    break;
                case 9:
                    MICROCODE_INP_OUT_OTC();
                    break;
            }
        }
        //microcode goes here
        private void MICROCODE_HLT() 
        {
            Running = false;
        }
        private void MICROCODE_ADD()
        {
            //simulate Overflow of over 999;
            if (ACC + RAM[AR] > 999)
            {
                ACC = (ushort)(ACC + RAM[AR] - 999); //if over 999 the the acc loops back around to 0
                ACC_Neg_Flag = !ACC_Neg_Flag; //flip the negative switch to give same behaviour as -ve
            }
            else
            {
                //ACC += RAM[AR];
                if (ACC_Neg_Flag) 
                {
                    MICROCODE_SUB();
                }
                else
                {
                    ACC += RAM[AR];
                }
            }
        }
        private void MICROCODE_SUB()
        {
            
            if (ACC < RAM[AR])
            {
                ACC =(ushort)( RAM[AR] - ACC); //set's the value to the modulous of what the actual value is (e.g 10 - 25 = -15 but here would give 15 )
                ACC_Neg_Flag = !ACC_Neg_Flag; //set the negative flag to switch
            }
            else
            {
                ACC -= RAM[AR]; 
            }
        }
        private void MICROCODE_STA()
        {
            RAM[AR] = ACC;
        }
        private void MICROCODE_LDA()
        {
            ACC = RAM[AR];
            ACC_Neg_Flag = false; //number loaded will always be postive so ensure the negative flag is set to false
        }
        private void MICROCODE_BRA()
        {
            PC = AR;
        }
        private void MICROCODE_BRZ()
        {
            //this is ignoring the wikipedia reccomended behaviour for BRZ to only branch if not -0 as I feel -0 == 0 therefor it should still branch
            if (ACC == 0)  
            {
                PC = AR;
            }
        }
        private void MICROCODE_BRP()
        {
            if (ACC == 0 || !ACC_Neg_Flag)
            {
                PC = AR;
            }
        }
        private void MICROCODE_INP_OUT_OTC()
        {
            switch (AR)
            {
                case 1:
                    //input method can be changed
                    while (true)
                    {
                        Console.Write("Input: ");
                        ushort temp;
                        if (ushort.TryParse(Console.ReadLine(), out temp))
                        {
                            if (0 <= temp && temp < 1000)
                            {
                                INP = temp;
                                break;
                            }
                        }
                        Console.WriteLine("ERROR: INVALID INPUT");

                    }

                    ACC = INP; //write from input register to ACCUMULATOR
                    break;
                case 2:
                    OUT = ACC; //write from ACCUMULATOR to output register
                    Console.WriteLine("OUTPUT: "+OUT); //output method can be changed
                    break;
                case 22:
                    //this is non-standard and how exactly it is implemented seemed unclear, however I implementing it such that I use the base
                    //C# character conversion
                    OUT = ACC; //write from ACCUMULATOR to output register
                    Console.WriteLine("OUTPUT: " +(char) OUT); //output method can be changed
                    break;
            }
        }
    }
}
