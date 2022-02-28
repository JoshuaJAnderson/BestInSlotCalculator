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

      Console.WriteLine(Help_Case());
      while (Console_Input != "exit")
      {
        Console_Input = Console.ReadLine();
        Parse_Input(Console_Input);

      }
    }

    static void Parse_Input(string input)
    {
      string output = "";
      string[] inputs = input.Split(' ');
      if(inputs[0] == "help")
      {
        output = Help_Case();
      }
      else
      {
        output = calculator.ProcessSetupWithClassAndAmount(inputs[0], inputs[1], int.Parse(inputs[2]), int.Parse(inputs[3]), int.Parse(inputs[4]));
      }

      Console.WriteLine(output);
    }

    static string Help_Case()
    {
      return "Syntax: [calculation_type] [class_type] [highest_tier] [mod_slots] [recoil]";
    }
  }
}
