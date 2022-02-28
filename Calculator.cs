using System;
using System.Collections.Generic;
using Objects;
using System.Text;

namespace BestInSlotCalculator
{
  class Calculator
  {
    List<Modifier> _ModSet;
    List<Modifier> _FinalModSet = new List<Modifier>();
    HashSet<string> cache = new HashSet<string>();

    int _MaxTech = 22, _AugAmounts;
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
    private void setupTempAugSet()
    {
      Console.WriteLine("----------------------------------");
      for (int i = 0; i < _ModSet.Count; i++)
      {
        if (!TypeNotAllowed(i))
        {
          _FinalModSet.Add(_ModSet[i]);
        }
      }
      Console.WriteLine("Amount of augmenters: " + _FinalModSet.Count);
    }

    //Returns a string of the augmenter's names using an array of their index's
    private string FullNameOfAugSet(int[] augIndexes)
    {
      string Names = "|";

      foreach (int i in augIndexes)
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

      int[] FinalAugSet = new int[_AugAmounts];
      int tempIndex = 0, ArrayIndex = 0;

      for (int i = 0; i < arrayIndex.Length; i++)
      {
        if (arrayIndex[i] == '-')
        {
          FinalAugSet[ArrayIndex] = tempIndex;
          tempIndex = 0;
          ArrayIndex++;
          continue;
        }
        tempIndex *= 10;
        tempIndex += arrayIndex[i] - '0';
      }

      return FinalAugSet;
    }

    //public string ProcessSetupWithClassAndAmount(string, string, string, int, int, double)
    //Process the information and gives back in string form the best possible aug setup
    public string ProcessSetupWithClassAndAmount(string setup, string charClass, int tech, int augSlots, double recoil)
    {
      string TotalNames = "";
      _AugAmounts = augSlots;
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
        case "gun":
          setClassGunner();
          break;
        case "flc":
          setClassFleetCommander();
          break;
        case "snp":
          setClassSniper();
          break;
        case "zrk":
          setClassBerserker();
          break;
        case "shm":
          setClassShieldMonkey();
          break;
        case "eng":
          setClassEngineer();
          break;
        case "spd":
          setClassSpeedDemon();
          break;
        case "ser":
          setClassSeer();
          break;
        case "nul":
          setClassNull();
          break;
      }

      BestSetup = new KeyValuePair<double, string>(0, "");
      int[] AugIndexes, temp = new int[augSlots];
      cache.Clear();
      _FinalModSet.Clear();

      setupTempAugSet();

      for (int i = 0; i < temp.Length; i++)
      {
        temp[i] = 0;
      }

      ProcessingSetup(temp, 0);

      AugIndexes = GetBestValueSet();

      TotalNames = FullNameOfAugSet(AugIndexes);

      return TotalNames;
    }

    //Recursive function to process the best aug setup
    private void ProcessingSetup(int[] temp, int augPlace)
    {

      if (augPlace == _AugAmounts)
        return;

      for (int i = 0; i < _FinalModSet.Count; i++)
      {
        temp[augPlace] = i;
        if (cache.Contains(getKeyFromArray(temp)))
          continue;
        cache.Add(getKeyFromArray(temp));
        if (augPlace + 1 == _AugAmounts)
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
          ProcessingSetup(temp, augPlace + 1);
      }
    }

    //Returns if the augmenter is allowed or not from an index value
    private bool TypeNotAllowed(int i)
    {
      string type = _ModSet[i].type;

      switch (_ToS)
      {
        case TypeOfSearch.DAMAGE:
          if (_ModSet[i].CritStr == 0 && _ModSet[i].Damage == 0 && _ModSet[i].Multifiring == 0)
            return true;
          break;
        case TypeOfSearch.DPS:
          if (_ModSet[i].CritStr == 0 && _ModSet[i].Damage == 0 && _ModSet[i].Multifiring == 0 && _ModSet[i].RoF == 0 && _ModSet[i].CritPerc == 0)
            return true;
          break;
        case TypeOfSearch.HPS:
          if (_ModSet[i].CritStr == 0 && _ModSet[i].Damage == 0 && _ModSet[i].TransPower == 0 && _ModSet[i].RoF == 0 && _ModSet[i].CritPerc == 0)
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
    private double MaxDamage(int[] AugIndexes)
    {
      double damage = 0;
      double critStr = 0;
      double critPerc = 0;
      double multifiring = 0;

      //Total Damage Stats
      foreach (int i in AugIndexes)
      {
        damage += _FinalModSet[i].Damage;
      }
      damage++;
      damage *= _classStats.Damage;

      //Total CritStr Stats
      foreach (int i in AugIndexes)
      {
        critStr += _FinalModSet[i].CritStr;
      }
      critStr++;
      critStr *= _classStats.CritStr;

      //Total CritPerc Stats
      foreach (int i in AugIndexes)
      {
        critPerc += _FinalModSet[i].CritPerc;
      }
      critPerc += _classStats.CritPerc;
      if (critPerc > 1)
        critPerc = 1;
      if (critPerc < .5)
        return 0;

      //Total MF stats and round Down
      foreach (int i in AugIndexes)
      {
        multifiring += _FinalModSet[i].Multifiring;
      }
      multifiring++;
      multifiring *= _classStats.Multifiring;
      if (multifiring > 5) multifiring = 5;
      else multifiring = Math.Floor(multifiring);

      return damage * (1 + _BaseCritExtraDamage * critStr) * multifiring;
    }

    //Returns a double thats the Max Dps from the array of index's of augmenters
    private double MaxDps(int[] AugIndexes)
    {
      double damage = 0;
      double critStr = 0;
      double multifiring = 0;
      double RateOfFire = 0;
      double critPerc = 0;

      //Total Damage Stats
      foreach (int i in AugIndexes)
      {
        damage += _FinalModSet[i].Damage;
      }
      damage++;
      damage *= _classStats.Damage;

      //Total RateOfFire Stats
      foreach (int i in AugIndexes)
      {
        RateOfFire += _FinalModSet[i].RoF;
      }
      RateOfFire++;
      RateOfFire *= _classStats.RoF;

      if (_WepRecoil / .1 < RateOfFire)
      {
        RateOfFire = _WepRecoil / .1;
      }

      //Total CritStr Stats
      foreach (int i in AugIndexes)
      {
        critStr += _FinalModSet[i].CritStr;
      }
      critStr++;
      critStr *= _classStats.CritStr;

      //Total CritPerc Stats
      foreach (int i in AugIndexes)
      {
        critPerc += _FinalModSet[i].CritPerc;
      }

      critPerc += _classStats.CritPerc;
      if (critPerc > .60)
        critPerc = .60;
      
      critPerc += _classStats.CritPerc + _BaseCritPercentage;
      if (critPerc > 1)
        critPerc = 1;
        
      //Total MF stats and round Down
      foreach (int i in AugIndexes)
      {
        multifiring += _FinalModSet[i].Multifiring;
      }
      multifiring++;
      multifiring *= _classStats.Multifiring;
      if (multifiring > 5) multifiring = 5;
      else multifiring = Math.Floor(multifiring);

      return damage * RateOfFire * (1 + _BaseCritExtraDamage * critStr * critPerc) * multifiring;
    }

    //Returns a double thats the Max Hps from the array of index's of augmenters
    private double MaxHps(int[] AugIndexes)
    {
      double damage = 0;
      double critStr = 0;
      double RateOfFire = 0;
      double critPerc = 0;
      double transPow = 0;

      //Total RateOfFire Stats
      foreach (int i in AugIndexes)
      {
        RateOfFire += _FinalModSet[i].RoF;
      }
      RateOfFire++;
      RateOfFire *= _classStats.RoF;

      if (_WepRecoil / .1 < RateOfFire)
      {
        RateOfFire = _WepRecoil / .1;
      }

      //Total Damage Stats
      foreach (int i in AugIndexes)
      {
        damage += _FinalModSet[i].Damage;
      }
      damage++;
      damage *= _classStats.Damage;

      //Total CritStr Stats
      foreach (int i in AugIndexes)
      {
        critStr += _FinalModSet[i].CritStr;
      }
      critStr++;
      critStr *= _classStats.CritStr;

      //Total CritPerc Stats
      foreach (int i in AugIndexes)
      {
        critPerc += _FinalModSet[i].CritPerc;
      }
      critPerc += _classStats.CritPerc + _BaseCritPercentage;
      if (critPerc > 1)
        critPerc = 1;

      //Total Trans Power Stats
      foreach (int i in AugIndexes)
      {
        transPow += _FinalModSet[i].TransPower;
      }
      transPow++;
      transPow *= _classStats.TransPower;

      return damage * RateOfFire * (1 + _BaseCritExtraDamage * critStr * critPerc) * transPow;
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
    public string getKeyFromArray(int[] augSetIndex)
    {
      string AugIndexString = "";
      int[] tempArray = new int[augSetIndex.Length];
      Array.Copy(augSetIndex, tempArray, augSetIndex.Length);
      Array.Sort(tempArray);
      foreach (int i in tempArray)
      {
        AugIndexString += i + "-";
      }

      return AugIndexString;
    }


    //setClass(); These functions set the class for the calculator.

    public void setClassSeer()
    {
      //Seer - Done
      _classStats.ClassName = "Seer";
      _classStats.CritPerc = 0.21;
      _classStats.CritStr = 1.99;
      _classStats.Damage = 2.414;
      _classStats.Multifiring = 1;
      _classStats.RoF = 1;
      _classStats.TransEff = 1;
      _classStats.TransPower = 1;
      _classStats.WeaponHold = 1;
      _classStats.Shield = 1;
      _classStats.Resist = 1;
      _classStats.ElecRegen = 1;
      _classStats.ShieldRegen = 1;
    }

    public void setClassSpeedDemon()
    {
      //Speed Demon - Done
      _classStats.ClassName = "Speed Demon";
      _classStats.CritPerc = 0.21;
      _classStats.CritStr = 1;
      _classStats.Damage = 4.0964;
      _classStats.Multifiring = 1;
      _classStats.RoF = 1.8816;
      _classStats.TransEff = 1;
      _classStats.TransPower = 1;
      _classStats.WeaponHold = 1;
      _classStats.Shield = 1;
      _classStats.Resist = 1;
      _classStats.ElecRegen = 1;
      _classStats.ShieldRegen = 1;
    }

    public void setClassShieldMonkey()
    {
      //Shield Monkey - Done
      _classStats.ClassName = "Shield Monkey";
      _classStats.CritPerc = 0;
      _classStats.CritStr = 1;
      _classStats.Damage = 1 * (1 + 0.24) * (1 + 0.35);
      _classStats.Multifiring = 1;
      _classStats.RoF = 1;
      _classStats.TransEff = 1 * (1 + 0.2);
      _classStats.TransPower = 1 * (1 + 0.2) * (1 + 0.01);
      _classStats.WeaponHold = 1;
      _classStats.Shield = 1;
      _classStats.Resist = 1;
      _classStats.ElecRegen = 1;
      _classStats.ShieldRegen = 1 * (1 + 0.2) * (1 + 0.01);
    }

    public void setClassEngineer()
    {
      //Engineer - Done
      _classStats.ClassName = "Engineer";
      _classStats.CritPerc = 0;
      _classStats.CritStr = 1;
      _classStats.Damage = 1;
      _classStats.Multifiring = 1;
      _classStats.RoF = 1;
      _classStats.TransEff = 1;
      _classStats.TransPower = 1;
      _classStats.WeaponHold = 1;
      _classStats.Shield = 1;
      _classStats.Resist = 1;
      _classStats.ElecRegen = 1;
      _classStats.ShieldRegen = 1;
    }

    public void setClassFleetCommander()
    {
      //Fleet Commander
      _classStats.ClassName = "Fleet Commander";
      _classStats.CritPerc = 0;
      _classStats.CritStr = 1;
      _classStats.Damage = 1;
      _classStats.Multifiring = 1;
      _classStats.RoF = 1;
      _classStats.TransEff = 1;
      _classStats.TransPower = 1;
      _classStats.WeaponHold = 1;
      _classStats.Shield = 1;
      _classStats.Resist = 1;
      _classStats.ElecRegen = 1;
      _classStats.ShieldRegen = 1;
    }

    public void setClassGunner()
    {
      //Gunner
      _classStats.ClassName = "Gunner";
      _classStats.CritPerc = 0;
      _classStats.CritStr = 1;
      _classStats.Damage = 1;
      _classStats.Multifiring = 1;
      _classStats.RoF = 1;
      _classStats.TransEff = 1;
      _classStats.TransPower = 1;
      _classStats.WeaponHold = 1.4 * 1.5;
      _classStats.Shield = 1;
      _classStats.Resist = 1;
      _classStats.ElecRegen = 1;
      _classStats.ShieldRegen = 1;
    }

    public void setClassBerserker()
    {
      //Berserker
      _classStats.ClassName = "Berserker";
      _classStats.CritPerc = 0;
      _classStats.CritStr = 1;
      _classStats.Damage = 1 * 1.03 * 1.03 * 1.03 * 1.3 * (1 + 0.36 * 3) * (1 + 0.002 * 50) * (1 + 0.01 * 50);
      _classStats.Multifiring = 3;
      _classStats.RoF = 1;
      _classStats.TransEff = 1;
      _classStats.TransPower = 1;
      _classStats.WeaponHold = 1.6;
      _classStats.Shield = 1 * 1.04 * 1.04 * 1.04 * (1 + 0.01 * 50);
      _classStats.Resist = 1 * (1 + 0.1 * 3);
      _classStats.ElecRegen = 1;
      _classStats.ShieldRegen = 1;
      _classStats.ElectricalTempering = -0.33;
    }

    public void setClassSniper()
    {
      //Sniper
      _classStats.ClassName = "Sniper";
      _classStats.CritPerc = 0.21;
      _classStats.CritStr = 1;
      _classStats.Damage = 9.26184336;
      _classStats.Multifiring = 1;
      _classStats.RoF = 0.7;
      _classStats.TransEff = 1;
      _classStats.TransPower = 1;
      _classStats.WeaponHold = 1;
      _classStats.Shield = 1;
      _classStats.Resist = 1;
      _classStats.ElecRegen = 1;
      _classStats.ShieldRegen = 1;
    }

    public void setClassNull()
    {
      //Null - Done
      _classStats.ClassName = "Null";
      _classStats.CritPerc = 0;
      _classStats.CritStr = 1;
      _classStats.Damage = 1.4;
      _classStats.Multifiring = 1;
      _classStats.RoF = 1.4;
      _classStats.TransEff = 1;
      _classStats.TransPower = 1;
      _classStats.WeaponHold = 1;
      _classStats.Shield = 1;
      _classStats.Resist = 1;
      _classStats.ElecRegen = 1;
      _classStats.ShieldRegen = 1;
    }
  }
}

