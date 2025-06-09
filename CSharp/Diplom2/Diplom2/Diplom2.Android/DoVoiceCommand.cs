using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Locations;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xamarin.Forms;
using Android.Provider;
using Android.Net;
using Xamarin.Essentials;
using System.Text.Json;
using System.Net.Http;
using Android.Content.PM;
using Android.Media;

[assembly: Dependency(typeof(Diplom2.Droid.DoVoiceCommand))]
namespace Diplom2.Droid
{
    public class DoVoiceCommand : IDoVoiceCommand
    {
        private Dictionary<string, int> wordsToNumbers_sixty = new Dictionary<string, int>()
            {
                { "ноль", 0 },
                { "один", 1 }, { "одна", 1 },
                { "два", 2 }, { "две", 2 },
                { "три", 3 },
                { "четыре", 4 },
                { "пять", 5 },
                { "шесть", 6 },
                { "семь", 7 },
                { "восемь", 8 },
                { "девять", 9 },
                { "десять", 10 },
                { "одиннадцать", 11 },
                { "двенадцать", 12 },
                { "тринадцать", 13 },
                { "четырнадцать", 14 },
                { "пятнадцать", 15 },
                { "шестнадцать", 16 },
                { "семнадцать", 17 },
                { "восемнадцать", 18 },
                { "девятнадцать", 19 },
                { "двадцать", 20 },
                { "двадцать один", 21 },
                { "двадцать два", 22 },
                { "двадцать три", 23 },
                { "двадцать четыре", 24 },
                { "двадцать пять", 25 },
                { "двадцать шесть", 26 },
                { "двадцать семь", 27 },
                { "двадцать восемь", 28 },
                { "двадцать девять", 29 },
                { "тридцать", 30 },
                { "тридцать один", 31 },
                { "тридцать два", 32 },
                { "тридцать три", 33 },
                { "тридцать четыре", 34 },
                { "тридцать пять", 35 },
                { "тридцать шесть", 36 },
                { "тридцать семь", 37 },
                { "тридцать восемь", 38 },
                { "тридцать девять", 39 },
                { "сорок", 40 },
                { "сорок один", 41 },
                { "сорок два", 42 },
                { "сорок три", 43 },
                { "сорок четыре", 44 },
                { "сорок пять", 45 },
                { "сорок шесть", 46 },
                { "сорок семь", 47 },
                { "сорок восемь", 48 },
                { "сорок девять", 49 },
                { "пятьдесят", 50 },
                { "пятьдесят один", 51 },
                { "пятьдесят два", 52 },
                { "пятьдесят три", 53 },
                { "пятьдесят четыре", 54 },
                { "пятьдесят пять", 55 },
                { "пятьдесят шесть", 56 },
                { "пятьдесят семь", 57 },
                { "пятьдесят восемь", 58 },
                { "пятьдесят девять", 59 },
                { "zero", 0 },
                { "one", 1 },
                { "two", 2 },
                { "three", 3 },
                { "four", 4 },
                { "five", 5 },
                { "six", 6 },
                { "seven", 7 },
                { "eight", 8 },
                { "nine", 9 },
                { "ten", 10 },
                { "eleven", 11 },
                { "twelve", 12 },
                { "thirteen", 13 },
                { "fourteen", 14 },
                { "fifteen", 15 },
                { "sixteen", 16 },
                { "seventeen", 17 },
                { "eighteen", 18 },
                { "nineteen", 19 },
                { "twenty", 20 },
                { "twenty one", 21 },
                { "twenty two", 22 },
                { "twenty three", 23 },
                { "twenty four", 24 },
                { "twenty five", 25 },
                { "twenty six", 26 },
                { "twenty seven", 27 },
                { "twenty eight", 28 },
                { "twenty nine", 29 },
                { "thirty", 30 },
                { "thirty one", 31 },
                { "thirty two", 32 },
                { "thirty three", 33 },
                { "thirty four", 34 },
                { "thirty five", 35 },
                { "thirty six", 36 },
                { "thirty seven", 37 },
                { "thirty eight", 38 },
                { "thirty nine", 39 },
                { "forty", 40 },
                { "forty one", 41 },
                { "forty two", 42 },
                { "forty three", 43 },
                { "forty four", 44 },
                { "forty five", 45 },
                { "forty six", 46 },
                { "forty seven", 47 },
                { "forty eight", 48 },
                { "forty nine", 49 },
                { "fifty", 50 },
                { "fifty one", 51 },
                { "fifty two", 52 },
                { "fifty three", 53 },
                { "fifty four", 54 },
                { "fifty five", 55 },
                { "fifty six", 56 },
                { "fifty seven", 57 },
                { "fifty eight", 58 },
                { "fifty nine", 59 },
                };
        private Dictionary<string, int> roundedNumberWords = new Dictionary<string, int>
            {
                { "ноль", 0 },
                { "один", 0 },
                { "два", 0 },
                { "три", 0 },
                { "четыре", 0 },
                { "пять", 10 },
                { "шесть", 10 },
                { "семь", 10 },
                { "восемь", 10 },
                { "девять", 10 },
                { "десять", 10 },
                { "одиннадцать", 10 },
                { "двенадцать", 10 },
                { "тринадцать", 10 },
                { "четырнадцать", 10 },
                { "пятнадцать", 20 },
                { "шестнадцать", 20 },
                { "семнадцать", 20 },
                { "восемнадцать", 20 },
                { "девятнадцать", 20 },
                { "двадцать", 20 },
                { "двадцать один", 20 },
                { "двадцать два", 20 },
                { "двадцать три", 20 },
                { "двадцать четыре", 20 },
                { "двадцать пять", 30 },
                { "двадцать шесть", 30 },
                { "двадцать семь", 30 },
                { "двадцать восемь", 30 },
                { "двадцать девять", 30 },
                { "тридцать", 30 },
                { "тридцать один", 30 },
                { "тридцать два", 30 },
                { "тридцать три", 30 },
                { "тридцать четыре", 30 },
                { "тридцать пять", 40 },
                { "тридцать шесть", 40 },
                { "тридцать семь", 40 },
                { "тридцать восемь", 40 },
                { "тридцать девять", 40 },
                { "сорок", 40 },
                { "сорок один", 40 },
                { "сорок два", 40 },
                { "сорок три", 40 },
                { "сорок четыре", 40 },
                { "сорок пять", 50 },
                { "сорок шесть", 50 },
                { "сорок семь", 50 },
                { "сорок восемь", 50 },
                { "сорок девять", 50 },
                { "пятьдесят", 50 },
                { "пятьдесят один", 50 },
                { "пятьдесят два", 50 },
                { "пятьдесят три", 50 },
                { "пятьдесят четыре", 50 },
                { "пятьдесят пять", 60 },
                { "пятьдесят шесть", 60 },
                { "пятьдесят семь", 60 },
                { "пятьдесят восемь", 60 },
                { "пятьдесят девять", 60 },
                { "шестьдесят", 60 },
                { "шестьдесят один", 60 },
                { "шестьдесят два", 60 },
                { "шестьдесят три", 60 },
                { "шестьдесят четыре", 60 },
                { "шестьдесят пять", 70 },
                { "шестьдесят шесть", 70 },
                { "шестьдесят семь", 70 },
                { "шестьдесят восемь", 70 },
                { "шестьдесят девять", 70 },
                { "семьдесят", 70 },
                { "семьдесят один", 70 },
                { "семьдесят два", 70 },
                { "семьдесят три", 70 },
                { "семьдесят четыре", 70 },
                { "семьдесят пять", 80 },
                { "семьдесят шесть", 80 },
                { "семьдесят семь", 80 },
                { "семьдесят восемь", 80 },
                { "семьдесят девять", 80 },
                { "восемьдесят", 80 },
                { "восемьдесят один", 80 },
                { "восемьдесят два", 80 },
                { "восемьдесят три", 80 },
                { "восемьдесят четыре", 80 },
                { "восемьдесят пять", 90 },
                { "восемьдесят шесть", 90 },
                { "восемьдесят семь", 90 },
                { "восемьдесят восемь", 90 },
                { "восемьдесят девять", 90 },
                { "девяносто", 90 },
                { "девяносто один", 90 },
                { "девяносто два", 90 },
                { "девяносто три", 90 },
                { "девяносто четыре", 90 },
                { "девяносто пять", 100 },
                { "девяносто шесть", 100 },
                { "девяносто семь", 100 },
                { "девяносто восемь", 100 },
                { "девяносто девять", 100 },
                { "сто", 100 },
                { "zero", 0 },
                { "one", 0 },
                { "two", 0 },
                { "three", 0 },
                { "four", 0 },
                { "five", 10 },
                { "six", 10 },
                { "seven", 10 },
                { "eight", 10 },
                { "nine", 10 },
                { "ten", 10 },
                { "eleven", 10 },
                { "twelve", 10 },
                { "thirteen", 10 },
                { "fourteen", 10 },
                { "fifteen", 20 },
                { "sixteen", 20 },
                { "seventeen", 20 },
                { "eighteen", 20 },
                { "nineteen", 20 },
                { "twenty", 20 },
                { "twenty one", 20 },
                { "twenty two", 20 },
                { "twenty three", 20 },
                { "twenty four", 20 },
                { "twenty five", 30 },
                { "twenty six", 30 },
                { "twenty seven", 30 },
                { "twenty eight", 30 },
                { "twenty nine", 30 },
                { "thirty", 30 },
                { "thirty one", 30 },
                { "thirty two", 30 },
                { "thirty three", 30 },
                { "thirty four", 30 },
                { "thirty five", 40 },
                { "thirty six", 40 },
                { "thirty seven", 40 },
                { "thirty eight", 40 },
                { "thirty nine", 40 },
                { "forty", 40 },
                { "forty one", 40 },
                { "forty two", 40 },
                { "forty three", 40 },
                { "forty four", 40 },
                { "forty five", 50 },
                { "forty six", 50 },
                { "forty seven", 50 },
                { "forty eight", 50 },
                { "forty nine", 50 },
                { "fifty", 50 },
                { "fifty one", 50 },
                { "fifty two", 50 },
                { "fifty three", 50 },
                { "fifty four", 50 },
                { "fifty five", 60 },
                { "fifty six", 60 },
                { "fifty seven", 60 },
                { "fifty eight", 60 },
                { "fifty nine", 60 },
                { "sixty", 60 },
                { "sixty one", 60 },
                { "sixty two", 60 },
                { "sixty three", 60 },
                { "sixty four", 60 },
                { "sixty five", 70 },
                { "sixty six", 70 },
                { "sixty seven", 70 },
                { "sixty eight", 70 },
                { "sixty nine", 70 },
                { "seventy", 70 },
                { "seventy one", 70 },
                { "seventy two", 70 },
                { "seventy three", 70 },
                { "seventy four", 70 },
                { "seventy five", 80 },
                { "seventy six", 80 },
                { "seventy seven", 80 },
                { "seventy eight", 80 },
                { "seventy nine", 80 },
                { "eighty", 80 },
                { "eighty one", 80 },
                { "eighty two", 80 },
                { "eighty three", 80 },
                { "eighty four", 80 },
                { "eighty five", 90 },
                { "eighty six", 90 },
                { "eighty seven", 90 },
                { "eighty eight", 90 },
                { "eighty nine", 90 },
                { "ninety", 90 },
                { "ninety one", 90 },
                { "ninety two", 90 },
                { "ninety three", 90 },
                { "ninety four", 90 },
                { "ninety five", 100 },
                { "ninety six", 100 },
                { "ninety seven", 100 },
                { "ninety eight", 100 },
                { "ninety nine", 100 },
                { "one hundred", 100 }
            };
        public string EnableWiFi(bool Language)
        {
            string Output = "";
            var context = Android.App.Application.Context;
            var wifiManager = (WifiManager)context.GetSystemService(Context.WifiService);
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Q)
            {
                if (!wifiManager.IsWifiEnabled)
                {
                    var intent = new Intent(Android.Provider.Settings.ActionWifiSettings);
                    intent.SetFlags(ActivityFlags.NewTask);
                    Android.App.Application.Context.StartActivity(intent);
                    if (Language)
                    {
                        Output = "Launch the settings, move the slider to the on position";
                    }
                    else
                    {
                        Output = "Запускаю настройки, переведите ползунок в положение вкл";
                    }
                }
                else
                {
                    if (Language)
                    {
                        Output = "Wi-Fi is already on";
                    }
                    else
                    {
                        Output = "Wi-Fi уже включен";
                    }
                }
            }
            else
            {
                if (!wifiManager.IsWifiEnabled)
                {
                    wifiManager.SetWifiEnabled(true);
                    if (Language)
                    {
                        Output = "I turn on Wi-Fi";
                    }
                    else
                    {
                        Output = "Включаю Wi-Fi";
                    }
                }
                else
                {
                    if (Language)
                    {
                        Output = "Wi-Fi is already on";
                    }
                    else
                    {
                        Output = "Wi-Fi уже включен";
                    }
                }
            }
            return Output;
        }
        public string DisableWiFi(bool Language)
        {
            string Output = "";
            var context = Android.App.Application.Context;
            var wifiManager = (WifiManager)context.GetSystemService(Context.WifiService);
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Q)
            {
                if (wifiManager.IsWifiEnabled)
                {
                    var intent = new Intent(Android.Provider.Settings.ActionWifiSettings);
                    intent.SetFlags(ActivityFlags.NewTask);
                    Android.App.Application.Context.StartActivity(intent);
                    if (Language)
                    {
                        Output = "Launch the settings, move the slider to the off position";
                    }
                    else
                    {
                        Output = "Запускаю настройки, переведите ползунок в положение выкл";
                    }
                }
                else
                {
                    if (Language)
                    {
                        Output = "Wi-Fi is already turned off";
                    }
                    else
                    {
                        Output = "Wi-Fi уже выключен";
                    }
                }
            }
            else
            {
                if (wifiManager.IsWifiEnabled)
                {
                    wifiManager.SetWifiEnabled(false);
                    if (Language)
                    {
                        Output = "I turn off Wi-Fi";
                    }
                    else
                    {
                        Output = "Выключаю Wi-Fi";
                    }

                }
                else
                {
                    if (Language)
                    {
                        Output = "Wi-Fi is already turned off";
                    }
                    else
                    {
                        Output = "Wi-Fi уже выключен";
                    }
                }
            }
            return Output;
        }
        public string EnableBLuetooth(bool Language)
        {
            string Output = "";
            var bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            // Для API < 31
            if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.Q)
            {
                if (bluetoothAdapter?.IsEnabled == false)
                {
                    bluetoothAdapter.Enable();
                    if (Language)
                    {
                        Output = "I turn on Bluetooth";
                    }
                    else
                    {
                        Output = "Включаю Bluetooth";
                    }
                }
                else
                {
                    if (Language)
                    {
                        Output = "Bluetooth is already turned on";
                    }
                    else
                    {
                        Output = "Bluetooth уже включен";
                    }
                }
            }
            else
            {
                // Для Android 12+ - открываем настройки Bluetooth
                if (bluetoothAdapter?.IsEnabled == false)
                {
                    var context = Android.App.Application.Context;
                    var intent = new Intent(Android.Provider.Settings.ActionBluetoothSettings);
                    intent.AddFlags(ActivityFlags.NewTask);
                    context.StartActivity(intent);
                    if (Language)
                    {
                        Output = "Launch the settings, move the slider to the on position";
                    }
                    else
                    {
                        Output = "Запускаю настройки, переведите ползунок в положение вкл";
                    }
                }
                else
                {
                    if (Language)
                    {
                        Output = "Bluetooth is already turned on";
                    }
                    else
                    {
                        Output = "Bluetooth уже включен";
                    }
                }
            }
            return Output;
        }
        public string DisableBLuetooth(bool Language)
        {
            string Output = "";
            var bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            // Для API < 31
            if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.Q)
            {
                if (bluetoothAdapter?.IsEnabled == true)
                {
                    bluetoothAdapter.Disable();
                    if (Language)
                    {
                        Output = "Turn off Bluetooth";
                    }
                    else
                    {
                        Output = "Выключаю Bluetooth";
                    }
                }
                else
                {
                    if (Language)
                    {
                        Output = "Bluetooth is already turned off";
                    }
                    else
                    {
                        Output = "Bluetooth уже выключен";
                    }
                }
            }
            else
            {
                // Для Android 12+ - открываем настройки Bluetooth
                if (bluetoothAdapter?.IsEnabled == true)
                {
                    var context = Android.App.Application.Context;
                    var intent = new Intent(Android.Provider.Settings.ActionBluetoothSettings);
                    intent.AddFlags(ActivityFlags.NewTask);
                    context.StartActivity(intent);
                    if (Language)
                    {
                        Output = "Launch the settings, move the slider to the off position";
                    }
                    else
                    {
                        Output = "Запускаю настройки, переведите ползунок в положение выкл";
                    }
                }
                else
                {
                    if (Language)
                    {
                        Output = "Bluetooth is already turned off";
                    }
                    else
                    {
                        Output = "Bluetooth уже выключен";
                    }
                }
            }
            return Output;
        }

        public string EnableGPS(bool Language)
        {
            string Output = "";
            var locationManager = (LocationManager)Android.App.Application.Context.GetSystemService(Context.LocationService);
            if (locationManager.IsProviderEnabled(LocationManager.GpsProvider) == false)
            {
                if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.Q)
                {
                    var intent = new Intent(Settings.ActionSettings);
                    intent.AddFlags(ActivityFlags.NewTask);
                    Android.App.Application.Context.StartActivity(intent);
                    if (Language)
                    {
                        Output = "Launch the settings, go to the Location section and move the slider to the application on";
                    }
                    else
                    {
                        Output = "Запускаю настройки, перейдите в раздел Местоположеине и переведите ползунок в приложение вкл";
                    }
                }
                else
                {
                    var intent = new Intent(Settings.ActionLocationSourceSettings);
                    intent.AddFlags(ActivityFlags.NewTask);
                    Android.App.Application.Context.StartActivity(intent);
                    if (Language)
                    {
                        Output = "Launch the settings, move the slider to the on position";
                    }
                    else
                    {
                        Output = "Запускаю настройки, переведите ползунок в положение вкл";
                    }
                }
            }
            else
            {
                if (Language)
                {
                    Output = "Geolocation is already enabled";
                }
                else
                {
                    Output = "Геолокация уже включена";
                }
            }
            return Output;
        }
        public string DisableGPS(bool Language)
        {
            string Output = "";
            var locationManager = (LocationManager)Android.App.Application.Context.GetSystemService(Context.LocationService);
            if (locationManager.IsProviderEnabled(LocationManager.GpsProvider) == true)
            {
                if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.Q)
                {
                    var intent = new Intent(Settings.ActionSettings);
                    intent.AddFlags(ActivityFlags.NewTask);
                    Android.App.Application.Context.StartActivity(intent);
                    if (Language)
                    {
                        Output = "Launch settings, go to the Location section and move the slider to the application off";
                    }
                    else
                    {
                        Output = "Запускаю настройки, перейдите в раздел Местоположеине и переведите ползунок в приложение выкл";
                    }
                }
                else
                {
                    var intent = new Intent(Settings.ActionLocationSourceSettings);
                    intent.AddFlags(ActivityFlags.NewTask);
                    Android.App.Application.Context.StartActivity(intent);
                    if (Language)
                    {
                        Output = "Launch the settings, move the slider to the off position";
                    }
                    else
                    {
                        Output = "Запускаю настройки, переведите ползунок в положение выкл";
                    }
                }
            }
            else
            {
                if (Language)
                {
                    Output = "Geolocation is already turned off";
                }
                else
                {
                    Output = "Геолокация уже выключена";
                }
            }
            return Output;
        }
        public string EnableInternet(bool Language)
        {
            string Output = "";
            var connectivityManager = (ConnectivityManager)Android.App.Application.Context.GetSystemService(Context.ConnectivityService);
            if (connectivityManager.ActiveNetworkInfo.Type != ConnectivityType.Mobile)
            {
                if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.Q)
                {
                    var intent = new Intent(Android.Provider.Settings.ActionDataRoamingSettings);
                    intent.AddFlags(ActivityFlags.NewTask);
                    Android.App.Application.Context.StartActivity(intent);
                }
                else
                {
                    var intent = new Intent(Android.Provider.Settings.ActionNetworkOperatorSettings);
                    intent.AddFlags(ActivityFlags.NewTask);
                    Android.App.Application.Context.StartActivity(intent);
                }
                if (Language)
                {
                    Output = "Launch the settings, move the slider to the on position";
                }
                else
                {
                    Output = "Запускаю настройки, переведите ползунок в положение вкл";
                }
            }
            else
            {
                if (Language)
                {
                    Output = "Mobile data is already enabled";
                }
                else
                {
                    Output = "Мобильная передача данных уже включена";
                }
            }
            return Output;
        } //проверить на другом телефоне
        public string DisableInternet(bool Language)
        {
            string Output = "";
            var connectivityManager = (ConnectivityManager)Android.App.Application.Context.GetSystemService(Context.ConnectivityService);
            if (connectivityManager.ActiveNetworkInfo?.IsConnected == true)
            {
                if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.Q)
                {
                    var intent = new Intent(Android.Provider.Settings.ActionDataRoamingSettings);
                    intent.AddFlags(ActivityFlags.NewTask);
                    Android.App.Application.Context.StartActivity(intent);
                }
                else
                {
                    var intent = new Intent(Android.Provider.Settings.ActionNetworkOperatorSettings);
                    intent.AddFlags(ActivityFlags.NewTask);
                    Android.App.Application.Context.StartActivity(intent);
                }
                if (Language)
                {
                    Output = "Launch the settings, move the slider to the off position";
                }
                else
                {
                    Output = "Запускаю настройки, переведите ползунок в положение выкл";
                }
            }
            else
            {
                if (Language)
                {
                    Output = "Mobile data is already turned off";
                }
                else
                {
                    Output = "Мобильная передача данных уже выключена";
                }
            }
            return Output;
        }//проверить на другом телефоне
        public string CallCaontact(bool Language, string ContactName)
        {
            string Output = "";

            var context = Android.App.Application.Context;

            var contactUri = ContactsContract.Contacts.ContentUri;
            string[] projection =
            {
                    ContactsContract.Contacts.InterfaceConsts.Id,
                    ContactsContract.Contacts.InterfaceConsts.DisplayName
            };
            var cursor = context.ContentResolver.Query(
                contactUri,
                projection,
                ContactsContract.Contacts.InterfaceConsts.DisplayName + " LIKE ?",
                new[] {ContactName + "%" },
                null
            );
            int x = cursor.Count;
            if (cursor?.Count == 0 || !cursor.MoveToFirst())
            {
                if (Language)
                {
                    Output = "Contact not found";
                }
                else
                {
                    Output = "Контакт не найден";
                }
            }
            else
            {
                List<string> _contactIds = new List<string>();
                List<string> _contactNames = new List<string>();
                for (int i = 0; i < cursor.Count; i++)
                {
                    _contactIds.Add(cursor.GetString(0));
                    _contactNames.Add(cursor.GetString(1));
                }

                string contactId = "";//cursor.GetString(cursor.GetColumnIndex(projection[0]));
                for (int i = 0; i < _contactIds.Count; i++)
                {
                    string Prom = _contactNames[i];
                    Prom = Prom.ToLower();
                    if (Prom == ContactName)
                    {
                        contactId = _contactIds[i];
                        break;
                    }
                }
                if (contactId == "")
                {
                    contactId = _contactIds[0];
                }

                // 3. Получаем номер телефона
                var phoneCursor = context.ContentResolver.Query(
                    ContactsContract.CommonDataKinds.Phone.ContentUri,
                    null,
                    ContactsContract.CommonDataKinds.Phone.InterfaceConsts.ContactId + " = ?",
                    new[] { contactId },
                    null
                );

                if (phoneCursor?.MoveToFirst() == true)
                {
                    string number = phoneCursor.GetString(
                        phoneCursor.GetColumnIndex(ContactsContract.CommonDataKinds.Phone.Number));

                    // 4. Звонок
                    var intent = new Intent(Intent.ActionDial);
                    intent.SetData(Android.Net.Uri.Parse("tel:" + number));
                    intent.AddFlags(ActivityFlags.NewTask);
                    context.StartActivity(intent);
                    if (Language)
                    {
                        Output = Output+ "I call the contact" + ContactName;
                    }
                    else
                    {
                        Output = Output + "Звоню контакту" + ContactName;
                    }
                }
                phoneCursor?.Close();
            }
            cursor?.Close();
            return Output;
        }
        public string Get_Time(bool Language)
        {
            string Output = "";
            string Prom = DateTime.Now.ToString("t");
            List<string> List_Words = Prom.Split(':').ToList();
            List<int> List_Date_Int = new List<int>() { Convert.ToInt32(List_Words[0]), Convert.ToInt32(List_Words[1]) };
            if (Language)
            {
                Output = Output + List_Date_Int[0] + ":" + Output + List_Date_Int[1];
                if (List_Date_Int[0] <= 12)
                {
                    Output += " am";
                }
                else
                {
                    Output += " pm";
                }
            }
            else
            {
                if (List_Date_Int[0] >= 1 && List_Date_Int[0] <= 4)
                {
                    Output = Output + List_Date_Int[0] + " час ";
                }
                else
                {
                    Output = Output + List_Date_Int[0] + " часов ";
                }
                if (List_Date_Int[1] == 1)
                {
                    Output = Output + List_Date_Int[1] + " минута ";
                }
                else if (List_Date_Int[1] >= 2 && List_Date_Int[1] <= 4)
                {
                    Output = Output + List_Date_Int[1] + " минуты ";
                }
                else
                {
                    Output = Output + List_Date_Int[1] + " минут ";
                }
            }
            return Output;
        }
        public string Get_Weather(bool Language, string Input_City)
        {
            string Output = "";
            string cityEncoded = System.Uri.EscapeDataString(Input_City);
            string url = "";
            if (Language)
            {
                url = $"https://wttr.in/{cityEncoded}?format=\"%t, %C\"";
            }
            else
            {
                url = $"https://wttr.in/{cityEncoded}?lang=ru&format=\"%t,%C\"";
            }

            HttpClient client = new HttpClient();
            try
            {
                var response = client.GetStringAsync(url).Result;
                List<string> List_Words = response.Split(',').ToList();
                if (Language)
                {
                    Output = Output + (List_Words[0][1] == '-' ? "minus" : "") + List_Words[0].Substring(2, 2) + " degrees Celsius " + List_Words[1].Substring(0, List_Words[1].Length - 1);
                }
                else
                {

                    Output = Output + (List_Words[0][1] == '-' ? "минус" : "") + List_Words[0].Substring(2, 2) + " градусов по цельсию " + List_Words[1].Substring(0, List_Words[1].Length - 1);
                }
            }
            catch (Exception e)
            {
                if (Language)
                {
                    Output = "Problems with internet connection";
                }
                else
                {
                    Output = "Проблемы с интернет соединением";
                }
            }
            return Output;
        }
        public string Get_Money(bool Language, string Input_Money)
        {
            string Output = "";
            string Valut = "";
            if (Input_Money.Contains("долл"))
                Valut = "USD";
            else if (Input_Money.Contains("евро"))
                Valut = "EUR";
            else if (Input_Money.Contains("юань"))
                Valut = "CNY";
            else if (Input_Money.Contains("тенге"))
                Valut = "KZT";
            else if (Input_Money.Contains("дирхам"))
                Valut = "AED";
            else if (Input_Money.Contains("doll"))
                Valut = "USD";
            else if (Input_Money.Contains("euro"))
                Valut = "EUR";
            else if (Input_Money.Contains("yuan"))
                Valut = "CNY";
            else if (Input_Money.Contains("tenge"))
                Valut = "KZT";
            else if (Input_Money.Contains("dirham"))
                Valut = "AED";
            if (Valut != "")
            {
                string url = $"https://open.er-api.com/v6/latest/{Valut}";
                HttpClient client = new HttpClient();
                try
                {
                    var response = client.GetStringAsync(url).Result;
                    using var doc = JsonDocument.Parse(response);
                    var rate = doc.RootElement.GetProperty("rates").GetProperty("RUB").GetDecimal();
                    if (Language)
                    {

                        Output = $"one  {Input_Money} costs  {Math.Round(rate,2)} rubles";
                    }
                    else
                    {

                        Output = $"один {Input_Money} стоит {Math.Round(rate,2)} рублей";
                    }
                }
                catch (Exception e)
                {
                    if (Language)
                    {
                        Output = "Problems with internet connection";
                    }
                    else
                    {
                        Output = "Проблемы с интернет соединением";
                    }
                }
            }
            else
            {
                if (Language)
                {
                    Output = "Sorry, I can't find such currency";
                }
                else
                {

                    Output = "Извините, не могу найти такую валюту";
                }
            }


            return Output;
        }//Округлить
        //public string Take_Photo(bool Language)
        //{
        //    var cameraIntent = new Intent(Android.Provider.MediaStore.ActionImageCapture);
        //    string Path = "storage/emulated/0/DCIM/Camera/IMG_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + ".jpg";
        //    cameraIntent.PutExtra(Android.Provider.MediaStore.ExtraOutput, Path);
        //    Xamarin.Forms.Forms.Context.StartActivity(cameraIntent);
        //    return "";
        //}// делать
        public string Budka(bool Language, List<string> Input_Time)
        {
            string Output = "";
           
            int Minuts = -1;
            int Hour = -1;
            if (!Language)
            {
                for (int i = 0; i < Input_Time.Count; i++)
                {
                    if (Input_Time[i].Contains("ча") || Input_Time[i].Contains("мин"))
                    {
                        Input_Time.Remove(Input_Time[i]);
                        i--;
                    }
                }
                if (Input_Time.Count == 5)
                {
                    Hour = wordsToNumbers_sixty[Input_Time[3]];
                    Minuts = wordsToNumbers_sixty[Input_Time[4]];
                }
                else if (Input_Time.Count == 7)
                {
                    Hour = wordsToNumbers_sixty[Input_Time[3] + " " + Input_Time[4]];
                    Minuts = wordsToNumbers_sixty[Input_Time[5] + " " + Input_Time[6]];
                }
                else if (Input_Time.Count == 6)
                {
                    try
                    {
                        Hour = wordsToNumbers_sixty[Input_Time[3] + " " + Input_Time[4]];
                        Minuts = wordsToNumbers_sixty[Input_Time[5]];
                    }
                    catch
                    {
                        Hour = wordsToNumbers_sixty[Input_Time[3]];
                        Minuts = wordsToNumbers_sixty[Input_Time[4] + " " + Input_Time[5]];
                    }
                }
            }
            else
            {
                try
                {
                    Hour = wordsToNumbers_sixty[Input_Time[4] + " " + Input_Time[5]];
                    Hour -= 12;
                    Minuts = wordsToNumbers_sixty[Input_Time[6] + " " + Input_Time[7]];
                }
                catch
                {
                    if(Hour>=0)
                    {
                        Minuts = wordsToNumbers_sixty[Input_Time[6]];
                    }
                    else
                    {
                        Hour = wordsToNumbers_sixty[Input_Time[4]];
                        try
                        {
                            Minuts = wordsToNumbers_sixty[Input_Time[5] + " " + Input_Time[6]];
                        }
                        catch
                        {
                            Minuts = wordsToNumbers_sixty[Input_Time[5]];
                        }
                    }
                }
                if((Input_Time[Input_Time.Count-2]+ Input_Time[Input_Time.Count - 1]=="pm") || (Input_Time[Input_Time.Count - 1] == "pm"))
                {
                    Hour += 12;
                }
            }
            if (Hour == -1)
            {
                Hour++;
            }
            if (Minuts == -1)
            {
                Minuts++;
            }
            if (Hour > 23)
            {
                if (Language)
                {
                    Output = "You have specified an incorrect time.";
                }
                else
                {
                    Output = "Вы указали некоректное время";
                }
            }
            else
            {
                var context = Android.App.Application.Context;
                Intent intent = new Intent(AlarmClock.ActionSetAlarm);
                intent.PutExtra(AlarmClock.ExtraHour, Hour);
                intent.PutExtra(AlarmClock.ExtraMinutes, Minuts);
                intent.SetFlags(ActivityFlags.NewTask);
                context.StartActivity(intent);
                if (Language)
                {
                    Output = "Setting alarm, confirm time";
                }
                else
                {
                    Output = "Устанавливаю будильник, подтвердите время";
                }
            }
            return Output;
        }
        public string Timer(bool Language, List<string> Input_Time)
        {
            string Output = "";
            int Seconds = 0;
            int Minuts = 0;
            int Hour = 0;
            string Prom_Time = "";
            int totalSeconds = 0;
            int startposition = 3;
            if(Language)
            {
                startposition = 4;
            }
            else
            {
                startposition = 3;
            }
            for (int i = startposition; i < Input_Time.Count; i++)
            {
                if (Input_Time[i].Contains("час")|| Input_Time[i].Contains("hour"))
                {
                    Hour = wordsToNumbers_sixty[Prom_Time];
                    totalSeconds += Hour * 3600;
                    Prom_Time = "";
                }
                else if (Input_Time[i].Contains("мин") || Input_Time[i].Contains("min"))
                {
                    Minuts = wordsToNumbers_sixty[Prom_Time];
                    totalSeconds += Minuts * 60;
                    if (totalSeconds > 86400)
                    {
                        if (Language)
                        {
                            Output = "The timer can only be set for 24 hours";
                        }
                        else
                        {
                            Output = "Таймер можно установить только на 24 часа";
                        }
                        break;
                    }
                    Prom_Time = "";
                }
                else if (Input_Time[i].Contains("сек") || Input_Time[i].Contains("sex"))
                {
                    Seconds = wordsToNumbers_sixty[Prom_Time];
                    totalSeconds += Seconds;
                    if (totalSeconds > 86400)
                    {
                        if (Language)
                        {
                            Output = "The timer can only be set for 24 hours";
                        }
                        else
                        {
                            Output = "Таймер можно установить только на 24 часа";
                        }
                        break;
                    }
                    Prom_Time = "";
                }
                else
                {
                    Prom_Time += Input_Time[i];
                    Prom_Time += " ";
                }
            }
            Intent intent = new Intent(AlarmClock.ActionSetTimer);
            intent.PutExtra(AlarmClock.ExtraLength, totalSeconds);
            intent.PutExtra(AlarmClock.ExtraSkipUi, true);
            intent.SetFlags(ActivityFlags.NewTask);

            Android.App.Application.Context.StartActivity(intent);
            if (Language)
            {
                Output = "Timer set";
            }
            else
            {
                Output = "Таймер установлен";
            }
            return Output;
        }

        private int LevenshteinDistance(string s, string t)
        {
            int[,] d = new int[s.Length + 1, t.Length + 1];

            for (int i = 0; i <= s.Length; i++) d[i, 0] = i;
            for (int j = 0; j <= t.Length; j++) d[0, j] = j;

            for (int i = 1; i <= s.Length; i++)
            {
                for (int j = 1; j <= t.Length; j++)
                {
                    int cost = (s[i - 1] == t[j - 1]) ? 0 : 1;

                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost
                    );
                }
            }

            return d[s.Length, t.Length];
        }
        private string Transliterate(string input)
        {
            var map = new Dictionary<char, string>
            {
                {'а',"a"}, {'б',"b"}, {'в',"v"}, {'г',"g"}, {'д',"d"}, {'е',"e"}, {'ё',"yo"}, {'ж',"zh"}, {'з',"z"},
                {'и',"i"}, {'й',"y"}, {'к',"k"}, {'л',"l"}, {'м',"m"}, {'н',"n"}, {'о',"o"}, {'п',"p"}, {'р',"r"},
                {'с',"s"}, {'т',"t"}, {'у',"u"}, {'ф',"f"}, {'х',"kh"}, {'ц',"ts"}, {'ч',"ch"}, {'ш',"sh"}, {'щ',"shch"},
                {'ы',"y"}, {'э',"e"}, {'ю',"yu"}, {'я',"ya"}
            };

            var sb = new StringBuilder();
            foreach (var ch in input.ToLower())
            {
                sb.Append(map.ContainsKey(ch) ? map[ch] : ch.ToString());
            }

            return sb.ToString();
        }
        public string LaunchApp(bool Language, string InputApp)
        {
            string Output = "";

            var context = Android.App.Application.Context;
            var pm = context.PackageManager;

            // Ищем все лаунчер-приложения
            Intent intent = new Intent(Intent.ActionMain, null);
            intent.AddCategory(Intent.CategoryLauncher);

            var resolveInfos = pm.QueryIntentActivities(intent, PackageInfoFlags.MatchAll);

            string input = InputApp.ToLower().Trim();
            string input_eng = Transliterate(InputApp.ToLower().Trim());

            string bestPackage = null;
            int bestScore = int.MaxValue;

            foreach (var resolveInfo in resolveInfos)
            {
                string label = pm.GetApplicationLabel(resolveInfo.ActivityInfo.ApplicationInfo).ToString().ToLower();
                string packageName = resolveInfo.ActivityInfo.PackageName.ToLower();

                // Сравниваем по расстоянию Левенштейна
                int distLabel = LevenshteinDistance(input, label);
                int distPackage = LevenshteinDistance(input, packageName);

                int distLabel_eng = LevenshteinDistance(input_eng, label);
                int distPackage_eng = LevenshteinDistance(input_eng, packageName);

                int score_ru = Math.Min(distLabel, distPackage);
                int score_eng = Math.Min(distLabel_eng, distPackage_eng);
                int score = Math.Min(score_ru, score_eng);
                if (score < bestScore)
                {
                    bestScore = score;
                    bestPackage = resolveInfo.ActivityInfo.PackageName;
                }
            }

            if (bestPackage != null && bestScore <= 4) // Можно настроить "порог" совпадения
            {
                var intentToLaunch = pm.GetLaunchIntentForPackage(bestPackage);
                if (intent != null)
                {
                    if (Language)
                    {
                        Output = "I'm launching" + InputApp;
                    }
                    else
                    {
                        Output = "Запускаю " + InputApp;
                    }
                    intentToLaunch.SetFlags(ActivityFlags.NewTask);
                    context.StartActivity(intentToLaunch);
                }
            }
            else
            {
                if (Language)
                {
                    Output = "I can't find the app";
                }
                else
                {
                    Output = "Не могу найти приложение";
                }
            }
            return Output;
        }

        public void Set_Volum(string InputVolumFirst, string InputVolumSecond)
        {
            var audioManager = (AudioManager)Android.App.Application.Context.GetSystemService(Context.AudioService);
            Android.Media.Stream[] streams = new Android.Media.Stream[]
            {
                Stream.Music,
                Stream.Alarm,
                Stream.Ring,
                Stream.Notification,
                Stream.System,
                Stream.VoiceCall
            };
          
            int Level_Volum = -1;
            try
            {
                Level_Volum = roundedNumberWords[InputVolumFirst + " " + InputVolumSecond];
            }
            catch
            {
                Level_Volum = roundedNumberWords[InputVolumSecond];
            }
            foreach (var stream in streams)
            {
                int max = audioManager.GetStreamMaxVolume(stream);
                int volumeLevel = (int)Math.Round((Level_Volum / 100.0) * max);
                audioManager.SetStreamVolume(stream, volumeLevel, VolumeNotificationFlags.RemoveSoundAndVibrate);
            }
        }//возможно удалить дефис
        public void Set_Volum(List<int> old_volume)
        {
            var audioManager = (AudioManager)Android.App.Application.Context.GetSystemService(Context.AudioService);
            audioManager.SetStreamVolume(Stream.Alarm, old_volume[0], VolumeNotificationFlags.RemoveSoundAndVibrate);
            audioManager.SetStreamVolume(Stream.Music, old_volume[1], VolumeNotificationFlags.RemoveSoundAndVibrate);
            audioManager.SetStreamVolume(Stream.Notification, old_volume[2], VolumeNotificationFlags.RemoveSoundAndVibrate);
            audioManager.SetStreamVolume(Stream.Ring, old_volume[3], VolumeNotificationFlags.RemoveSoundAndVibrate);
            audioManager.SetStreamVolume(Stream.System, old_volume[4], VolumeNotificationFlags.RemoveSoundAndVibrate);
            audioManager.SetStreamVolume(Stream.VoiceCall, old_volume[5], VolumeNotificationFlags.RemoveSoundAndVibrate);
        }
        public List<int> Get_Sr_Volume()
        {
            AudioManager audioManager = (AudioManager)Android.App.Application.Context.GetSystemService(Context.AudioService);

            List<int> Old_Volume = new List<int>();
            Old_Volume.Add(audioManager.GetStreamVolume(Stream.Alarm));

            Old_Volume.Add(audioManager.GetStreamVolume(Stream.Music));
            Old_Volume.Add(audioManager.GetStreamVolume(Stream.Notification));

            Old_Volume.Add(audioManager.GetStreamVolume(Stream.Ring));

            Old_Volume.Add(audioManager.GetStreamVolume(Stream.System));

            Old_Volume.Add(audioManager.GetStreamVolume(Stream.VoiceCall));

            return Old_Volume;
        }
        public void Set_Brightness(string Count_first, string Count_second)
        {
            int _brightness;
            try
            {
                _brightness = roundedNumberWords[Count_first + " " + Count_second];
            }
            catch
            {
                _brightness = roundedNumberWords[Count_second];
            }
            float brightness = _brightness / 100;
            if(brightness<0f)
            {
                brightness = 0f;
            }
            if(brightness>1f)
            {
                brightness = 1f;
            }
            var LayoutParans = Xamarin.Essentials.Platform.CurrentActivity.Window.Attributes;
            LayoutParans.ScreenBrightness = brightness;
            Xamarin.Essentials.Platform.CurrentActivity.Window.Attributes= LayoutParans;
        }
    }
}