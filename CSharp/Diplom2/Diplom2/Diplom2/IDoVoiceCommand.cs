using System.Collections.Generic;

public interface IDoVoiceCommand
{
    string EnableWiFi(bool Language);
    string DisableWiFi(bool Language);
    string EnableBLuetooth(bool Language);
    string DisableBLuetooth(bool Language);
    string EnableGPS(bool Language);
    string DisableGPS(bool Language);
    string EnableInternet(bool Language);
    string DisableInternet(bool Language);
    string CallCaontact(bool Language, string ContactName);
    string Get_Time(bool Language);
    string Get_Weather(bool Language, string Input_City);
    string Get_Money(bool Language, string Input_Money);
    //string Take_Photo(bool Language);
    string Budka(bool Language, List<string> Input_Time);
    string Timer(bool Language, List<string> Input_Time);
    string LaunchApp(bool Language, string InputApp);
    void Set_Volum(string InputVolumFirst, string InputVolumSecond);
    void Set_Volum(List<int> Old_Volum_List);
    List<int> Get_Sr_Volume();
    void Set_Brightness(string Count_first, string Count_second);

}

