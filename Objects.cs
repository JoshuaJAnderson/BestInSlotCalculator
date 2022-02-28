namespace Objects
{
  public enum TypeOfSearch
  {
    DAMAGE,
    DPS,
    HPS
  }

  public struct Modifier
  {
    public string Name;
    public string type;
    public int skill;
    public int tech;
    public double Damage,
      RoF,
      CritPerc,
      CritStr,
      Multifiring,
      TransPower,
      TransEff,
      ElecRegen,
      ShieldRegen,
      Energy,
      Shield,
      Resist,
      ElectricalTempering,
      WeaponHold;

    public string Display()
    {
      string displaystr = "";

      displaystr += "Name: " + Name;
      displaystr += "\tDamage: " + (Damage * 100).ToString() + '%';
      displaystr += "\tRate Of Fire: " + (RoF * 100).ToString() + '%';
      displaystr += "\tCrit Strength: " + (CritStr * 100).ToString() + '%';
      displaystr += "\tCrit Perc: " + (CritPerc * 100).ToString() + '%';

      return displaystr;
    }
  };

  public struct classStats
  {
    public string ClassName;
    public double Damage,
      RoF,
      CritPerc,
      CritStr,
      Multifiring,
      TransPower,
      TransEff,
      ElecRegen,
      ShieldRegen,
      Energy,
      Shield,
      Resist,
      ElectricalTempering,
      WeaponHold;
  };
}
