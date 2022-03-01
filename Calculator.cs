using System;
using System.Collections.Generic;
using Objects;

namespace BestInSlotCalculator
{
  class Calculator
  {
    List<Modifier> _ModSet;
    List<Modifier> _FinalModSet = new List<Modifier>();
    HashSet<string> cache = new HashSet<string>();

    int _MaxTech = 22, _ModAmounts;
    double _WepRecoil, _BaseCritExtraDamage = 0.5, _BaseCritPercentage = 0.1;

    classStats _classStats;

    KeyValuePair<double, string> BestSetup = new KeyValuePair<double, string>(0, "");

    TypeOfSearch _ToS;

    public Calculator()
    {
      setClassNull();
    }

    public void SetupModSet(List<Modifier> ModSet)
    {
      _ModSet = ModSet;
    }

    //Creates a filtered set of augmenters using the _FinalModSet list
    private void setupTempModSet()
    {
      Console.WriteLine("----------------------------------");
      for (int i = 0; i < _ModSet.Count; i++)
      {
        if (!TypeNotAllowed(i))
        {
          _FinalModSet.Add(_ModSet[i]);
        }
      }
      Console.WriteLine("Amount of modifiers: " + _FinalModSet.Count);
    }

    //Returns a string of the augmenter's names using an array of their index's
    private string FullNameOfModSet(int[] ModIndexes)
    {
      string Names = "|";

      foreach (int i in ModIndexes)
      {
        Names += _FinalModSet[i].Name + "|";
      }
      Names += "\n";
      return Names;
    }

    //Returns an array of augmenters that's the highest value from the list
    private int[] GetBestValueSet()
    {
      string arrayIndex = "";

      arrayIndex = BestSetup.Value;

      int[] FinalModSet = new int[_ModAmounts];
      int tempIndex = 0, ArrayIndex = 0;

      for (int i = 0; i < arrayIndex.Length; i++)
      {
        if (arrayIndex[i] == '-')
        {
          FinalModSet[ArrayIndex] = tempIndex;
          tempIndex = 0;
          ArrayIndex++;
          continue;
        }
        tempIndex *= 10;
        tempIndex += arrayIndex[i] - '0';
      }

      return FinalModSet;
    }

    //public string ProcessSetupWithClassAndAmount(string, string, string, int, int, double)
    //Process the information and gives back in string form the best possible aug setup
    public string ProcessSetupWithClassAndAmount(string setup, string charClass, int tech, int modSlots, double recoil)
    {
      string TotalNames = "";
      _ModAmounts = modSlots;
      _MaxTech = tech;
      _WepRecoil = recoil;

      //Sets which setup to go by
      switch (setup.ToLower())
      {
        case "dps":
          _ToS = TypeOfSearch.DPS;
          break;
        case "hps":
          _ToS = TypeOfSearch.HPS;
          break;
        case "dph":
          _ToS = TypeOfSearch.DAMAGE;
          break;
        default:
          return "Error with setup type.";
      }
      //Sets which class to go by
      switch (charClass.ToLower())
      {
        case "aoe":
          SetClassAoE();
          break;
        case "commander":
          SetClassCommander();
          break;
        case "artillary":
          SetClassArtillary();
          break;
        case "tank":
          SetClassTank();
          break;
        case "healer":
          SetClassHealer();
          break;
        case "rouge":
          SetClassRouge();
          break;
        case "assassin":
          SetClassAssassin();
          break;
        case "null":
          setClassNull();
          break;
      }

      BestSetup = new KeyValuePair<double, string>(0, "");
      int[] ModIndexes, temp = new int[modSlots];
      cache.Clear();
      _FinalModSet.Clear();

      setupTempModSet();

      for (int i = 0; i < temp.Length; i++)
      {
        temp[i] = 0;
      }

      ProcessingSetup(temp, 0);

      ModIndexes = GetBestValueSet();

      TotalNames = FullNameOfModSet(ModIndexes);

      return TotalNames;
    }

    //Recursive function to process the best aug setup
    private void ProcessingSetup(int[] temp, int modPlace)
    {

      if (modPlace == _ModAmounts)
        return;

      for (int i = 0; i < _FinalModSet.Count; i++)
      {
        temp[modPlace] = i;
        if (cache.Contains(getKeyFromArray(temp)))
          continue;
        cache.Add(getKeyFromArray(temp));
        if (modPlace + 1 == _ModAmounts)
        {
          switch (_ToS)
          {
            case TypeOfSearch.DAMAGE:
              if (BestSetup.Key < MaxDamage(temp))
                BestSetup = new KeyValuePair<double, string>(MaxDamage(temp), getKeyFromArray(temp));
              break;
            case TypeOfSearch.DPS:
              if (BestSetup.Key < MaxDps(temp))
                BestSetup = new KeyValuePair<double, string>(MaxDps(temp), getKeyFromArray(temp));
              break;
            case TypeOfSearch.HPS:
              if (BestSetup.Key < MaxHps(temp))
                BestSetup = new KeyValuePair<double, string>(MaxHps(temp), getKeyFromArray(temp));
              break;
          }
          continue;
        }
        else
          ProcessingSetup(temp, modPlace + 1);
      }
    }

    //Returns if the augmenter is allowed or not from an index value
    private bool TypeNotAllowed(int i)
    {
      string type = _ModSet[i].type;

      switch (_ToS)
      {
        case TypeOfSearch.DAMAGE:
          if (_ModSet[i].CritStr == 0 && _ModSet[i].Damage == 0 && _ModSet[i].multishot == 0)
            return true;
          break;
        case TypeOfSearch.DPS:
          if (_ModSet[i].CritStr == 0 && _ModSet[i].Damage == 0 && _ModSet[i].multishot == 0 && _ModSet[i].RoF == 0 && _ModSet[i].CritPerc == 0)
            return true;
          break;
        case TypeOfSearch.HPS:
          if (_ModSet[i].CritStr == 0 && _ModSet[i].Damage == 0 && _ModSet[i].HealingPower == 0 && _ModSet[i].RoF == 0 && _ModSet[i].CritPerc == 0)
            return true;
          break;
      }

      //if (_ModSet[i].Value.skill != -1)
      //  return true;

      if (_ModSet[i].tech < (_MaxTech - 3) || _ModSet[i].tech > _MaxTech)
        return true;

      return false;
    }

    //Returns a double thats the Max Damage from the array of index's of augmenters
    private double MaxDamage(int[] ModIndexes)
    {
      double damage = 0;
      double critStr = 0;
      double critPerc = 0;
      double multishot = 0;

      //Total Damage Stats
      foreach (int i in ModIndexes)
      {
        damage += NegativeAdd(_FinalModSet[i].Damage);
      }
      damage = NegativeFinalize(damage);
      damage++;
      damage *= _classStats.Damage;

      //Total CritStr Stats
      foreach (int i in ModIndexes)
      {
        critStr += NegativeAdd(_FinalModSet[i].CritStr);
      }
      critStr = NegativeFinalize(critStr);
      critStr++;
      critStr *= _classStats.CritStr;

      //Total CritPerc Stats
      foreach (int i in ModIndexes)
      {
        critPerc += NegativeAdd(_FinalModSet[i].CritPerc);
      }
      critPerc = NegativeFinalize(critPerc);
      critPerc += _classStats.CritPerc;
      if (critPerc > 0) critPerc = 1;
      if (critPerc <= 0) critPerc = 0;

      //Total MF stats and round Down
      foreach (int i in ModIndexes)
      {
        multishot += NegativeAdd(_FinalModSet[i].multishot);
      }
      multishot = NegativeFinalize(multishot);
      multishot++;
      multishot *= _classStats.multishot;
      if (multishot > 5) multishot = 5;
      else multishot = Math.Floor(multishot);

      return damage * (1 + _BaseCritExtraDamage * critStr * critPerc) * multishot;
    }

    //Returns a double thats the Max Dps from the array of index's of augmenters
    private double MaxDps(int[] ModIndexes)
    {
      double damage = 0;
      double critStr = 0;
      double multishot = 0;
      double RateOfFire = 0;
      double critPerc = 0;

      //Total Damage Stats
      foreach (int i in ModIndexes)
      {
        damage += NegativeAdd(_FinalModSet[i].Damage);
      }
      damage = NegativeFinalize(damage);
      damage++;
      damage *= _classStats.Damage;

      //Total RateOfFire Stats
      foreach (int i in ModIndexes)
      {
        RateOfFire += NegativeAdd(_FinalModSet[i].RoF);
      }
      RateOfFire = NegativeFinalize(RateOfFire);
      RateOfFire++;
      RateOfFire *= _classStats.RoF;

      if (_WepRecoil / .1 < RateOfFire)
      {
        RateOfFire = _WepRecoil / .1;
      }

      //Total CritStr Stats
      foreach (int i in ModIndexes)
      {
        critStr += NegativeAdd(_FinalModSet[i].CritStr);
      }
      critStr = NegativeFinalize(critStr);
      critStr++;
      critStr *= _classStats.CritStr;

      //Total CritPerc Stats
      foreach (int i in ModIndexes)
      {
        critPerc += NegativeAdd(_FinalModSet[i].CritPerc);
      }
      critPerc = NegativeFinalize(critPerc);
      
      critPerc += _classStats.CritPerc + _BaseCritPercentage;
      if (critPerc > 1)
        critPerc = 1;
      if (critPerc < 0)
        critPerc = 0;

      //Total MF stats and round Down
      foreach (int i in ModIndexes)
      {
        multishot += NegativeAdd(_FinalModSet[i].multishot);
      }
      multishot = NegativeFinalize(multishot);
      multishot++;
      multishot *= _classStats.multishot;
      if (multishot > 5) multishot = 5;
      else multishot = Math.Floor(multishot);

      return damage * RateOfFire * (1 + _BaseCritExtraDamage * critStr * critPerc) * multishot;
    }

    //Returns a double thats the Max Hps from the array of index's of augmenters
    private double MaxHps(int[] ModIndexes)
    {
      double damage = 0;
      double critStr = 0;
      double RateOfFire = 0;
      double critPerc = 0;
      double healingPow = 0;

      //Total RateOfFire Stats
      foreach (int i in ModIndexes)
      {
        RateOfFire += NegativeAdd(_FinalModSet[i].RoF);
      }
      RateOfFire = NegativeFinalize(RateOfFire);
      RateOfFire++;
      RateOfFire *= _classStats.RoF;

      if (_WepRecoil / .1 < RateOfFire)
      {
        RateOfFire = _WepRecoil / .1;
      }

      //Total Damage Stats
      foreach (int i in ModIndexes)
      {
        damage += NegativeAdd(_FinalModSet[i].Damage);
      }
      damage = NegativeFinalize(damage);
      damage++;
      damage *= _classStats.Damage;

      //Total CritStr Stats
      foreach (int i in ModIndexes)
      {
        critStr += NegativeAdd(_FinalModSet[i].CritStr);
      }
      critStr = NegativeFinalize(critStr);
      critStr++;
      critStr *= _classStats.CritStr;

      //Total CritPerc Stats
      foreach (int i in ModIndexes)
      {
        critPerc += NegativeAdd(_FinalModSet[i].CritPerc);
      }
      critPerc = NegativeFinalize(critPerc);
      critPerc += _classStats.CritPerc + _BaseCritPercentage;
      if (critPerc > 1)
        critPerc = 1;
      if (critPerc < 0)
        critPerc = 0;

      //Total Trans Power Stats
      foreach (int i in ModIndexes)
      {
        healingPow += NegativeAdd(_FinalModSet[i].HealingPower);
      }
      healingPow = NegativeFinalize(healingPow);
      healingPow++;
      healingPow *= _classStats.HealingPower;

      return damage * RateOfFire * (1 + _BaseCritExtraDamage * critStr * critPerc) * healingPow;
    }

    //Returns a value for adding negative percentages
    private double NegativeAdd(double value)
    {
      if (value < 0)
      {
        return (-value / (-value - 1));
      }
      return value;
    }

    //Returns the value to be used after adding negative percentages
    private double NegativeFinalize(double value)
    {
      if (value < 0)
      {
        return (-value / (value - 1));
      }
      return value;
    }

    //Returns a key in the form of a string from an array of the index's of a group of augmenters
    public string getKeyFromArray(int[] SetIndex)
    {
      string IndexString = "";
      int[] tempArray = new int[SetIndex.Length];
      Array.Copy(SetIndex, tempArray, SetIndex.Length);
      Array.Sort(tempArray);
      foreach (int i in tempArray)
      {
        IndexString += i + "-";
      }

      return IndexString;
    }


    //setClass(); These functions set the class for the calculator.

    public void SetClassAssassin()
    {
      //Seer - Done
      _classStats.ClassName = "Assassin";
      _classStats.CritPerc = 0.2;
      _classStats.CritStr = 2.5;
      _classStats.Damage = 2.5;
      _classStats.multishot = 1;
      _classStats.RoF = 1;
      _classStats.HealingPower = 1;
    }

    public void SetClassRouge()
    {
      //Speed Demon - Done
      _classStats.ClassName = "Rouge";
      _classStats.CritPerc = 0;
      _classStats.CritStr = 1;
      _classStats.Damage = 4;
      _classStats.multishot = 1;
      _classStats.RoF = 2;
      _classStats.HealingPower = 1;
    }

    public void SetClassHealer()
    {
      //Shield Monkey - Done
      _classStats.ClassName = "Healer";
      _classStats.CritPerc = 0;
      _classStats.CritStr = 1;
      _classStats.Damage = 2;
      _classStats.multishot = 1;
      _classStats.RoF = 1.5;
      _classStats.HealingPower = 2;
    }

    public void SetClassCommander()
    {
      //Fleet Commander
      _classStats.ClassName = "Commander";
      _classStats.CritPerc = 0;
      _classStats.CritStr = 1;
      _classStats.Damage = 1;
      _classStats.multishot = 1;
      _classStats.RoF = 1;
      _classStats.HealingPower = 1;
    }

    public void SetClassAoE()
    {
      //Gunner
      _classStats.ClassName = "Gunner";
      _classStats.CritPerc = 0;
      _classStats.CritStr = 1;
      _classStats.Damage = 1;
      _classStats.multishot = 2;
      _classStats.RoF = 1;
      _classStats.HealingPower = 1;
    }

    public void SetClassTank()
    {
      //Berserker
      _classStats.ClassName = "Tank";
      _classStats.CritPerc = 0;
      _classStats.CritStr = 1;
      _classStats.Damage = 2;
      _classStats.multishot = 3;
      _classStats.RoF = 1;
      _classStats.HealingPower = 1;
    }

    public void SetClassArtillary()
    {
      //Sniper
      _classStats.ClassName = "Artillary";
      _classStats.CritPerc = 0.1;
      _classStats.CritStr = 1.5;
      _classStats.Damage = 6;
      _classStats.multishot = 1;
      _classStats.RoF = 0.7;
      _classStats.HealingPower = 1;
    }

    public void setClassNull()
    {
      //Null - Done
      _classStats.ClassName = "Null";
      _classStats.CritPerc = 0;
      _classStats.CritStr = 1;
      _classStats.Damage = 1;
      _classStats.multishot = 1;
      _classStats.RoF = 1;
      _classStats.HealingPower = 1;
    }
  }
}

