using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using Objects;

namespace BestInSlotCalculator
{
  class SQL_Reader
  {
    SqlConnection cnn;
    List<Modifier> _ModSet = new List<Modifier>();

    public SQL_Reader()
    {
      //Might need to edit this depending on your settings
      string cwd = Directory.GetCurrentDirectory();
      cwd = Path.GetFullPath(Path.Combine(cwd, @"..\..\..\"));
      cnn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + cwd + "Database1.mdf;Integrated Security=True;");

      string strCommand = "SELECT * FROM modifiers";
      SqlCommand myCommand = new SqlCommand(strCommand, cnn);
      SqlDataReader reader;
      try
      {
        cnn.Open();
        reader = myCommand.ExecuteReader();
        while (reader.Read())
        {
          Modifier tempMod = create_modifier(reader);
          _ModSet.Add(tempMod);
          //Console.WriteLine(tempMod.Display());

        }

      }
      catch
      {
        Console.WriteLine("error");
      }
    }

    public List<Modifier> GetModifierList()
    {
      return _ModSet;
    }

    Modifier create_modifier(SqlDataReader reader)
    {
      Modifier mod = new Modifier();

      mod.Name = (string)reader.GetValue(0);
      mod.Damage = (double)reader.GetValue(1);
      mod.RoF = (double)reader.GetValue(2);
      mod.CritStr = (double)reader.GetValue(3);
      mod.CritPerc = (double)reader.GetValue(4);
      mod.Multifiring = (double)reader.GetValue(5);

      return mod;
    }

  }
}
