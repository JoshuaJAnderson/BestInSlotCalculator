using System;
using System.Collections.Generic;
using Objects;

namespace BestInSlotCalculator
{
  class Program
  {
    static SQL_Reader sql_Reader = new SQL_Reader();
    static Calculator calculator = new Calculator();

    static void Main(string[] args)
    {
      string Console_Input = "";
      calculator.SetupModSet(sql_Reader.GetModifierList());

      while (Console_Input != "exit")
      {
        Console_Input = Console.ReadLine();
        Parse_Input(Console_Input);

      }
    }

    static void Parse_Input(string input)
    {
      string output = "";
      switch (input.ToLower())
      {
        case "dps":
          output = calculator.ProcessSetupWithClassAndAmount("dps", "nul", 1, 3, 1);
          break;

        default:
          break;
      }

      Console.WriteLine(output);
    }
  }
}
