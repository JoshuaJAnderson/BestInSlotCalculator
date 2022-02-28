using System;
using System.Collections.Generic;
using Objects;

namespace BestInSlotCalculator
{
  class Program
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


    static void Main(string[] args)
    {

    }

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
    private double MaxBurst(int[] AugIndexes)
    {
      double damage = 0;
      double critStr = 0;
      double multifiring = 0;
      double RateOfFire = 0;
      double critPerc = 0;
      double _BaseCritPerc = 0.05;

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

      critPerc += _classStats.CritPerc + _BaseCritPerc;
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

      return damage * RateOfFire * (1 + _critExtraDamage * critStr * critPerc) * multifiring;
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
  }
}
