using System;
using System.Collections.Generic;
using Objects;
using System.Text;

namespace BestInSlotCalculator
{
  class Calculator
  {
    List<Augmenter> _augSet;
    List<Augmenter> _FinalAugSet = new List<Augmenter>();
    HashSet<string> cache = new HashSet<string>();

    int _MaxTech = 22, _AugAmounts, _ElecRegen, _WeaponElecUsage;
    double _WepRecoil;
    float _critExtraDamage = 0.5f;

    classStats _classStats;
    AugType _TypesOfAugsAllowed;

    KeyValuePair<double, string> BestSetup = new KeyValuePair<double, string>(0, "");

    TypeOfSearch _ToS;


    public Calculator()
    {
      setClassNull();
      _TypesOfAugsAllowed.BASE = false;
      _TypesOfAugsAllowed.LF = false;
      _TypesOfAugsAllowed.HF = false;
      _TypesOfAugsAllowed.FREIGHTER = false;
      _TypesOfAugsAllowed.CAPITAL = false;
    }


    //Creates a filtered set of augmenters using the _FinalAugSet list
    private void setupTempAugSet()
    {
      Console.WriteLine("----------------------------------");
      for (int i = 0; i < _augSet.Count; i++)
      {
        if (!TypeNotAllowed(i))
        {
          _FinalAugSet.Add(_augSet[i]);
          Console.WriteLine(_augSet[i].augName);
        }
      }
      Console.WriteLine("Amount of augmenters: " + _FinalAugSet.Count);
    }

    //Returns a string of the augmenter's names using an array of their index's
    private string FullNameOfAugSet(int[] augIndexes)
    {
      string Names = "|";

      foreach (int i in augIndexes)
      {
        Names += _FinalAugSet[i].augName + "|";
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
    public string ProcessSetupWithClassAndAmount(string setup, string charClass, string hullType, int tech, int augSlots, double recoil)
    {
      string TotalAugNames = "";
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
        case "tank":
          _ToS = TypeOfSearch.TANK;
          break;
        case "burst":
          _ToS = TypeOfSearch.BURST;
          break;
        default:
          return "Error with setup type.";
      }
      //Sets which hull to go by
      switch (hullType.ToLower())
      {
        case "base":
          _TypesOfAugsAllowed.BASE = true;
          break;
        case "lf":
          _TypesOfAugsAllowed.LF = true;
          break;
        case "hf":
          _TypesOfAugsAllowed.HF = true;
          break;
        case "sf":
          _TypesOfAugsAllowed.FREIGHTER = true;
          break;
        case "cap":
          _TypesOfAugsAllowed.CAPITAL = true;
          break;
        default:
          return "Error with hull type.";
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
      _FinalAugSet.Clear();

      setupTempAugSet();

      for (int i = 0; i < temp.Length; i++)
      {
        temp[i] = 0;
      }

      ProcessingSetup(temp, 0);

      AugIndexes = GetBestValueSet();

      TotalAugNames = FullNameOfAugSet(AugIndexes);

      return TotalAugNames;
    }

    //Recursive function to process the best aug setup
    private void ProcessingSetup(int[] temp, int augPlace)
    {

      if (augPlace == _AugAmounts)
        return;

      for (int i = 0; i < _FinalAugSet.Count; i++)
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
      string type = _augSet[i].type;

      if (_TypesOfAugsAllowed.BASE)
      {
        if (type == "Base")
        {
          //if (_augSet[i].Value.tech < 11 || _augSet[i].Value.tech > 16)
          if (_augSet[i].tech < 17)
            return true;
          return false;
        }
        else
        {
          return true;
        }
      }
      else
      {
        if (type == "Base")
        {
          return true;
        }
      }

      switch (type)
      {
        case "LightFighter":
          if (!_TypesOfAugsAllowed.LF)
            return true;
          break;
        case "HeavyFighter":
          if (!_TypesOfAugsAllowed.HF)
            return true;
          break;
        case "Freighter":
          if (!_TypesOfAugsAllowed.FREIGHTER)
            return true;
          break;
        case "Capital":
          if (!_TypesOfAugsAllowed.CAPITAL)
            return true;
          break;
      }

      switch (_ToS)
      {
        case TypeOfSearch.DAMAGE:
          if (_augSet[i].CritStr == 0 && _augSet[i].Damage == 0 && _augSet[i].Multifiring == 0)
            return true;
          break;
        case TypeOfSearch.DPS:
          if (_augSet[i].CritStr == 0 && _augSet[i].Damage == 0 && _augSet[i].Multifiring == 0 && _augSet[i].RoF == 0 && _augSet[i].CritPerc == 0)
            return true;
          break;
        case TypeOfSearch.HPS:
          if (_augSet[i].CritStr == 0 && _augSet[i].Damage == 0 && _augSet[i].TransPower == 0 && _augSet[i].RoF == 0 && _augSet[i].CritPerc == 0)
            return true;
          break;
      }

      //if (_augSet[i].Value.skill != -1)
      //  return true;

      if (_augSet[i].tech < (_MaxTech - 3) || _augSet[i].tech > _MaxTech)
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
        damage += _FinalAugSet[i].Damage;
      }
      damage++;
      damage *= _classStats.Damage;

      //Total CritStr Stats
      foreach (int i in AugIndexes)
      {
        critStr += _FinalAugSet[i].CritStr;
      }
      critStr++;
      critStr *= _classStats.CritStr;

      //Total CritPerc Stats
      foreach (int i in AugIndexes)
      {
        critPerc += _FinalAugSet[i].CritPerc;
      }
      critPerc += _classStats.CritPerc;
      if (critPerc > 1)
        critPerc = 1;
      if (critPerc < .5)
        return 0;

      //Total MF stats and round Down
      foreach (int i in AugIndexes)
      {
        multifiring += _FinalAugSet[i].Multifiring;
      }
      multifiring++;
      multifiring *= _classStats.Multifiring;
      if (multifiring > 5) multifiring = 5;
      else multifiring = Math.Floor(multifiring);

      return damage * (1 + _critExtraDamage * critStr) * multifiring;
    }

    //Returns a double thats the Max Dps from the array of index's of augmenters
    private double MaxDps(int[] AugIndexes)
    {
      double damage = 0;
      double critStr = 0;
      double multifiring = 0;
      double RateOfFire = 0;
      double critPerc = 0;
      double energyRegen = 0;
      double electemp = 0;
      double tempDPE = 0;
      double tempRegen = 0;
      double DPEGoal = 50;

      //Total Electrical Regeneration Stats
      foreach (int i in AugIndexes)
      {
        energyRegen += _FinalAugSet[i].ElecRegen;
      }
      energyRegen++;
      energyRegen *= _classStats.ElecRegen;

      //Total Electrical Tempering Stats
      foreach (int i in AugIndexes)
      {
        electemp += negativeAdd(_FinalAugSet[i].ElectricalTempering);
      }
      electemp = negativeFinalize(electemp);
      electemp = (1 + electemp) * (1 + _classStats.ElectricalTempering) - 1;

      //Total Damage Stats
      foreach (int i in AugIndexes)
      {
        damage += _FinalAugSet[i].Damage;
      }
      damage++;
      damage *= _classStats.Damage;

      //Total RateOfFire Stats
      foreach (int i in AugIndexes)
      {
        RateOfFire += _FinalAugSet[i].RoF;
      }
      RateOfFire++;
      RateOfFire *= _classStats.RoF;

      if (_WepRecoil / (_TypesOfAugsAllowed.BASE ? .5 : .1) < RateOfFire)
      {
        RateOfFire = _WepRecoil / (_TypesOfAugsAllowed.BASE ? .5 : .1);
      }

      //Total CritStr Stats
      foreach (int i in AugIndexes)
      {
        critStr += _FinalAugSet[i].CritStr;
      }
      critStr++;
      critStr *= _classStats.CritStr;

      //Total CritPerc Stats
      foreach (int i in AugIndexes)
      {
        critPerc += _FinalAugSet[i].CritPerc;
      }

      critPerc += _classStats.CritPerc;
      if (critPerc > .60)
        critPerc = .60;
      /*
      critPerc += _classStats.CritPerc + _BaseCritPerc;
      if (critPerc > 1)
        critPerc = 1;
        */
      //Total MF stats and round Down
      foreach (int i in AugIndexes)
      {
        multifiring += _FinalAugSet[i].Multifiring;
      }
      multifiring++;
      multifiring *= _classStats.Multifiring;
      if (multifiring > 5) multifiring = 5;
      else multifiring = Math.Floor(multifiring);

      tempDPE = damage / (_WeaponElecUsage * (1 + electemp) * multifiring);
      tempRegen = _ElecRegen * energyRegen - (_WeaponElecUsage * (1 + electemp) * multifiring);



      return damage * RateOfFire * (1 + _critExtraDamage * critStr * critPerc) * multifiring * (tempRegen < 0 ? (1 / -tempRegen) : 1);
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
        RateOfFire += _FinalAugSet[i].RoF;
      }
      RateOfFire++;
      RateOfFire *= _classStats.RoF;

      if (_WepRecoil / (_TypesOfAugsAllowed.BASE ? .5 : .1) < RateOfFire)
      {
        RateOfFire = _WepRecoil / (_TypesOfAugsAllowed.BASE ? .5 : .1);
      }

      //Total Damage Stats
      foreach (int i in AugIndexes)
      {
        damage += _FinalAugSet[i].Damage;
      }
      damage++;
      damage *= _classStats.Damage;

      //Total CritStr Stats
      foreach (int i in AugIndexes)
      {
        critStr += _FinalAugSet[i].CritStr;
      }
      critStr++;
      critStr *= _classStats.CritStr;

      //Total CritPerc Stats
      foreach (int i in AugIndexes)
      {
        critPerc += _FinalAugSet[i].CritPerc;
      }
      critPerc += _classStats.CritPerc;
      if (critPerc > 1)
        critPerc = 1;

      //Total Trans Power Stats
      foreach (int i in AugIndexes)
      {
        transPow += _FinalAugSet[i].TransPower;
      }
      transPow++;
      transPow *= _classStats.TransPower;

      return damage * RateOfFire * (1 + _critExtraDamage * critStr * critPerc) * transPow;
    }

    //Returns a value for adding negative percentages
    private double negativeAdd(double value)
    {
      if (value < 0)
      {
        return (-value / (-value - 1));
      }
      return value;
    }

    //Returns the value to be used after adding negative percentages
    private double negativeFinalize(double value)
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

