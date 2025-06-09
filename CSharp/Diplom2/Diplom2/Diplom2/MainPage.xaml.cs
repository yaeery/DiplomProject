using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.CommunityToolkit.Core;
using Xamarin.CommunityToolkit.UI.Views;
using System.IO;
using System.Threading;
using System.Speech.Synthesis;
using System.Globalization;


namespace Diplom2
{
    public partial class MainPage : ContentPage
    {
        bool isInited;
        Plugin.AudioRecorder.AudioRecorderService Recorder;
        Plugin.AudioRecorder.AudioPlayer Player;
        Vosk.Model Model_Ru;
        Vosk.Model Model_En;
        Vosk.VoskRecognizer Rec;

        List<string> Name_Of_Assistent;
        public bool Is_Listnig_Command;
        List<string> On_Main_Option_List;
        List<string> Off_Main_Option_List;
        List<string> Name_Call_List;
        public static bool Is_Dark_Theme;
        public static bool Is_Vibration;
        public static bool Language;
        public static bool Off_Camera;
        public static bool Off_Micro;
        public static bool Block_Vision;
        public static bool Block_Listn;
        public static bool TellDeystvia;
        private bool Is_Text_Recognize;
        private bool Is_Obraz_Recognize;
        private bool Is_Money_Recognize;
        private bool Is_Time_Recognize;
        private float Old_Ver_Rec = 0;
        private int Is_Zoom = 0;
        private string Old_Text = "";
        private List<int> Old_Volume_List;
        private bool Is_No_Loaded;
        private bool Is_Start_Working_Assistetn;

        CancellationTokenSource _cts;
        CancellationTokenSource cancellationToken_Speech;
        string DataPath => Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        string ModelPathNew_En => DataPath + "/VoiceModelEn/";
        string ModelPathNew_Ru => DataPath + "/VoiceModelRu/";
        const string ModelWebLink_En = "https://alphacephei.com/vosk/models/vosk-model-small-en-us-0.15.zip";
        const string ModelWebLink_Ru = "https://alphacephei.com/vosk/models/vosk-model-small-ru-0.22.zip";

        string DownLoadedFileName_Ru
        {
            get
            {
                var Preresult = ModelWebLink_Ru.Split('/');
                return Preresult[Preresult.Length - 1];
            }
        }
        string DownLoadedFileName_En
        {
            get
            {
                var Preresult = ModelWebLink_En.Split('/');
                return Preresult[Preresult.Length - 1];
            }
        }
        string ArchivePath_Ru => DataPath + DownLoadedFileName_Ru;
        string ArchivePath_En => DataPath + DownLoadedFileName_En;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            Name_Of_Assistent = new List<string>() { "помощник", "друг", "friend", "assistant" };
            Is_Listnig_Command = true;
            On_Main_Option_List = new List<string>() { "включи", "запусти" };//Добавить английский
            Off_Main_Option_List = new List<string>() { "выключи", "отключи" };//Добавить английский
            Name_Call_List = new List<string>() { "позвони", "вызвови", "набери" };
            Is_Dark_Theme = Preferences.Get("Is_Dark_Theme", false);
            Is_Vibration = Preferences.Get("Is_Vibration", false);
            Language = Preferences.Get("Language", false);
            Off_Camera = Preferences.Get("Off_Camera", false);
            Off_Micro = Preferences.Get("Off_Micro", false);
            Block_Vision = Preferences.Get("Block_Vision", false);
            Block_Listn = Preferences.Get("Block_Listn", false);
            TellDeystvia = Preferences.Get("TellDeystvia", false);

            _cts = new CancellationTokenSource();
            cancellationToken_Speech = new CancellationTokenSource();

            string Name_of_Assistent_by_user = Preferences.Get("Name_of_Assistent_by_user", "");
            if (Name_of_Assistent_by_user != "")
            {
                Name_Of_Assistent.Add(Name_of_Assistent_by_user);
                string x = Name_of_Assistent_by_user;
                x = char.ToUpper(x[0]) + x.Substring(1);
                NameAssistEntry.Placeholder = x;
            }
            RefreshVideoPlayer();
            ChangeTheme();
            ChangeLanguage();
            await RequestAllPermissions();
            if (!isInited)
            {
                isInited = true;
                InitRecorder();
                InitPlayer();
                InitRecognizer();
            }
            Old_Volume_List = new List<int>();

            SwitchTheme.IsToggled = Is_Dark_Theme;
            VibroSwitch.IsToggled = Is_Vibration;
            OffMicroSwitch.IsToggled = Off_Micro;
            OffCameraSwitch.IsToggled = Off_Camera;
            ChangeLanguge.Text = (Language) ? "English" : "Русский";
        }
        async Task RequestAllPermissions()
        {
            
            var status_M = await Xamarin.Essentials.Permissions.RequestAsync<Xamarin.Essentials.Permissions.Microphone>();
            var status_RS = await Xamarin.Essentials.Permissions.RequestAsync<Xamarin.Essentials.Permissions.StorageRead>();
            var status_WS = await Xamarin.Essentials.Permissions.RequestAsync<Xamarin.Essentials.Permissions.StorageWrite>();
            var status_Co = await Xamarin.Essentials.Permissions.RequestAsync<Xamarin.Essentials.Permissions.ContactsRead>();
            var status_Ca = await Xamarin.Essentials.Permissions.RequestAsync<Xamarin.Essentials.Permissions.Camera>();
            DependencyService.Resolve<IPermissions>().RequestPermissions();
        }

        void InitRecorder()
        {
            Recorder = new Plugin.AudioRecorder.AudioRecorderService
            {
                PreferredSampleRate = 16000,
                StopRecordingOnSilence = false,
                StopRecordingAfterTimeout = false
            };
        }

        void InitPlayer()
        {
            Player = new Plugin.AudioRecorder.AudioPlayer();
        }

        async void InitRecognizer()
        {
            await CheckModelFolder(ModelPathNew_En, ArchivePath_En, ModelPathNew_Ru, ArchivePath_Ru);
            if (!Is_No_Loaded && !Is_Start_Working_Assistetn)
            {
                System.Threading.Thread LaunchAndShowRecogModule = new System.Threading.Thread(() =>
                {
                    string x_En = ModelPathNew_En.Remove(ModelPathNew_En.Length - 1, 1);
                    var ShellFolder_En = System.IO.Directory.GetDirectories(ModelPathNew_En);
                    Model_En = new Vosk.Model(x_En);
                    Rec = new Vosk.VoskRecognizer(Model_En, 16000);
                    string x_Ru = ModelPathNew_Ru.Remove(ModelPathNew_Ru.Length - 1, 1);
                    var ShellFolder_Ru = System.IO.Directory.GetDirectories(ModelPathNew_Ru);
                    Model_Ru = new Vosk.Model(x_Ru);
                    if (Language)
                    {
                        Rec = new Vosk.VoskRecognizer(Model_En, 16000);
                    }
                    else
                    {
                        Rec = new Vosk.VoskRecognizer(Model_Ru, 16000);
                    }
                });
                LaunchAndShowRecogModule.Start();
                while (LaunchAndShowRecogModule.IsAlive) await Task.Yield();
                StartRecognizeCycle();

            }
        }

        async Task CheckModelFolder(string ModelPathNew_En, string ArchivePath_En, string ModelPathNew_Ru, string ArchivePath_Ru)
        {
            bool isModelReady = true;
            bool isZipLoaded = true;
            if (!System.IO.Directory.Exists(ModelPathNew_En))
            {
                System.IO.Directory.CreateDirectory(ModelPathNew_En);
                System.IO.Directory.CreateDirectory(ModelPathNew_Ru);
                isZipLoaded = false;
                isModelReady = false;
            }
            if (System.IO.Directory.GetDirectories(ModelPathNew_En).Length == 0)
            {
                isModelReady = false;
            }
            if (!System.IO.File.Exists(ArchivePath_En))
            {
                isZipLoaded = false;
            }
            if (!isModelReady)
            {
                if (!isZipLoaded)
                {
                    bool IsPickedYes = await DisplayAlert("Скачивание", "Для возможности работы голосового ассистента необходимо усановить модели", "Скачать", "Не скачивать");
                    if (IsPickedYes)
                    {
                        await DownloadModel(ModelWebLink_En, ArchivePath_En);
                        if (Is_No_Loaded == false)
                        {
                            await DownloadModel(ModelWebLink_Ru, ArchivePath_Ru);
                            Unzip(ArchivePath_En, ModelPathNew_En);
                            MoveToModelFolder(ModelPathNew_En);
                            DeleteArchive(ArchivePath_En);
                            Unzip(ArchivePath_Ru, ModelPathNew_Ru);
                            MoveToModelFolder(ModelPathNew_Ru);
                            DeleteArchive(ArchivePath_Ru);
                            Say_I_Am_Ready();
                        }
                    }
                    else
                    {
                        DeleteArchiveDer(ModelPathNew_En);
                        DeleteArchiveDer(ModelPathNew_Ru);
                        Is_No_Loaded = true;
                    }
                }
            }
        }
        private void Say_I_Am_Ready()
        {
            if(Language)
            {
                Voice_Result("Models have been successfully created to activate me, say assistant or friend");
            }
            else
            {
                Voice_Result("Модели успешно установлены, чтобы меня активировать произнесите помощник или друг");
            }
        }
        async Task DownloadModel(string ModelWebLink, string ArchivePath)
        {
            try
            {
                using (var Client = new System.Net.WebClient())
                {
                    System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                    await Client.DownloadFileTaskAsync(new Uri(ModelWebLink), ArchivePath);
                    int i = 1;
                    while (Client.IsBusy)
                    {
                        //Console.WriteLine("качаю ", i);
                        await Task.Yield();
                        i++;
                    }

                    Client.Dispose();
                    Is_No_Loaded = false;
                }
            }
            catch (System.Net.WebException ex)
            {
                Is_No_Loaded = true;
                DeleteArchiveDer(ModelPathNew_En);
                DeleteArchiveDer(ModelPathNew_Ru);
                await DisplayAlert("Ошибка", "Отсутсвует подключение к интернету", "Ок");
                //throw ex;
            }
        }

        void Unzip(string ArchivePath, string ModelPathNew)
        {
            System.IO.Compression.ZipFile.ExtractToDirectory(ArchivePath, ModelPathNew);
        }

        void MoveToModelFolder(string ModelPathNew)
        {
            var ShellFolder = System.IO.Directory.GetDirectories(ModelPathNew)[0];
            var TargetFolders = System.IO.Directory.GetDirectories(ShellFolder);
            for (int i = 0; i < TargetFolders.Length; i++)
            {
                var info = new System.IO.DirectoryInfo(TargetFolders[i]);
                info.MoveTo(ModelPathNew + info.Name);
                //var x = System.IO.Directory.GetFiles(ModelPathNew + info.Name);
                var y = System.IO.Directory.GetDirectories(ModelPathNew + info.Name);
            }
            //var x = System.IO.Directory.GetFiles(ModelPathNew + "graph/phones");
            //System.IO.File.Copy(ModelPathNew + "graph/phones", "storage/emulated/0/Download/testic.ini", true);
            //System.IO.File.SetAttributes("storage/emulated/0/Download/testic.ini", System.IO.FileAttributes.Normal);
            //var Target = System.IO.Directory.GetFiles(ModelPathNew + "graph");
            string[] TargetFiles = System.IO.Directory.GetFiles(ShellFolder);
            for (int i = 0; i < TargetFiles.Length; i++)
            {
                var info = new System.IO.FileInfo(TargetFiles[i]);
                info.MoveTo(ModelPathNew + info.Name);
            }
            System.IO.Directory.Delete(ShellFolder, true);

        }
        void DeleteArchiveDer(string ArchivePath)
        {
            if (System.IO.Directory.Exists(ArchivePath))
            {
                System.IO.Directory.Delete(ArchivePath);
            }
        }
        void DeleteArchive(string ArchivePath)
        {
            if (System.IO.File.Exists(ArchivePath))
            {
                System.IO.File.Delete(ArchivePath);
            }
        }

        private async void Create_Subtitels(string inputPath, string Filepath)
        {
            Vosk.VoskRecognizer Recognizer;
            if (Language)
            {
                Recognizer = new Vosk.VoskRecognizer(Model_En, 16000);
            }
            else
            {
                Recognizer = new Vosk.VoskRecognizer(Model_Ru, 16000);
            }
            ((MediaElement)SubtitersGrid.Children[1]).Source = new FileMediaSource
            {
                File = Filepath
            };
            var Reader = System.IO.File.OpenRead(inputPath);
            byte[] buffer = new byte[4096];
            int bytesRead;
            string Result = String.Empty;
            while ((bytesRead = await Reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                Recognizer.AcceptWaveform(buffer, bytesRead);
                Result = Recognizer.PartialResult();
                if (Result.Length > 20)
                {
                    Result = Result.Substring(17, Result.Length - 20);
                    SubtitrsLabel.Text = SubtitrsLabel.Text + Result + " ";
                    Recognizer.Reset();
                    if (SubtitrsLabel.Text.Length > 100)
                    {
                        SubtitrsLabel.Text = "";
                    }
                }
            }
            Result = Recognizer.FinalResult();
            //SubtitrsLabel.Text = prom;
            if (Result.Length > 20) Result = Result.Substring(14, Result.Length - 17);
            Reader.Close();
            Reader.Dispose();
            Recognizer.Dispose();
        }

        private async void RecognizeFromDisk(string inputPath)
        {
            if (Language)
            {
                SubtitrsLabel.Text = "Generate subtitles";
            }
            else
            {
                SubtitrsLabel.Text = "Генерируем субтитры";
            }
            string OutputFile = DependencyService.Resolve<ISubtitels>().Work(inputPath);
            if (!File.Exists(OutputFile))
            {
                await Task.Delay(2000);
            }
            try
            {
                SubtitrsLabel.Text = "";
                Create_Subtitels(OutputFile, inputPath);
            }
            catch
            {
                if (Language)
                {
                    Voice_Result("An error occurred, please try again");
                }
                else
                {
                    Voice_Result("Произошла ошибка, попробуйте снова");
                }
                SubtitrsLabel.Text = "";
            }

        }

        async void StartRecognizeCycle()
        {
            try
            {
                byte[] buffer = new byte[4096];
                int bytesRead;
                string Result;
                await Recorder.StartRecording();
                var Reader = Recorder.GetAudioFileStream();
                Is_Start_Working_Assistetn = true;
                while (true)
                {

                    while (Is_Listnig_Command)
                    {
                        bytesRead = await Reader.ReadAsync(buffer, 0, buffer.Length);
                        Rec.AcceptWaveform(buffer, bytesRead);
                        Result = Rec.PartialResult();
                        if (Result.Length > 20 && !Off_Micro)
                        {
                            Result = Result.Substring(17, Result.Length - 20);
                            for (int i = 0; i < Name_Of_Assistent.Count; i++)
                            {
                                if (Result.Contains(Name_Of_Assistent[i]))
                                {
                                    Old_Volume_List = DependencyService.Resolve<IDoVoiceCommand>().Get_Sr_Volume();
                                    DependencyService.Resolve<IDoVoiceCommand>().Set_Volum("EmptyString", "ноль");
                                    if (MicroFrame.IsVisible == false)
                                    {
                                        Change_Interface(true, false, false, false, false, false, false);
                                    }

                                    Rec.Reset();
                                    var timer = new System.Timers.Timer() { Interval = 5000 };
                                    timer.Start();
                                    timer.Elapsed += (o, e) =>
                                    {
                                        Is_Listnig_Command = false;
                                        timer.Dispose();
                                    };
                                }
                            }
                        }
                        else if (Result.Length > 300)
                        {
                            Rec.Reset();
                        }
                    }
                    if (!Off_Micro)
                    {
                        Result = Rec.FinalResult();
                        if (Result.Length > 20)
                        {
                            Result = Result.Substring(14, Result.Length - 17);
                        }
                        Check_Command(Result);
                    }
                    //Timer_Listnig_Thread.Join();

                }
                await Recorder.StopRecording();
                Reader.Close();
                Reader.Dispose();
            }
            catch
            {
                if (Language)
                {
                    Voice_Result("You must allow access to the microphone in setting app");
                }
                else
                {
                    Voice_Result("Необходимо разрешить доступ к микрофону в приложении настройки");
                }
                
            }
        }
        void Check_Command(string input)
        {
            List<string> List_Words = input.Split(' ').ToList();
            bool Is_Change_Volume = false;
            string Result_Operation = "";
            try
            {
                if (!Language)
                {
                    if (input.Contains("расп"))
                    {
                        if (input.Contains("текст"))
                        {
                            Change_Interface(false, true, false, false, false, false, false);
                        }
                        else if (input.Contains("ден"))
                        {
                            Change_Interface(false, false, true, false, false, false, false);
                        }
                        else if (input.Contains("образ"))
                        {
                            Change_Interface(false, false, false, true, false, false, false);
                        }
                    }
                    else if(input.Contains("страниц") && input.Contains("наст"))
                    {
                        Change_Interface(false, false, false, false, false, false, true);
                    }
                    else if (input.Contains("субт"))
                    {
                        Change_Interface(false, false, false, false, false, true, false);
                    }
                    else if ((input.Contains("опи")) && ((input.Contains("карт")) || (input.Contains("изоб"))))
                    {
                        Change_Interface(false, false, false, false, true, false, false);
                    }
                    else if((input.Contains("смен")) && (input.Contains("язык")))
                    {
                        Language = true;
                        ChangeLanguge.Text = "English";
                        Rec = new Vosk.VoskRecognizer(Model_En, 16000);
                        ChangeLanguage();
                        Result_Operation = "System language changed";
                    }
                    else if((input.Contains("имя")) && (input.Contains("установи")))
                    {
                        Preferences.Set("Name_of_Assistent_by_user", List_Words[List_Words.Count-1]);
                        string x = List_Words[List_Words.Count - 1];
                        x = char.ToUpper(x[0]) + x.Substring(1);
                        NameAssistEntry.Placeholder = x;
                        if (Name_Of_Assistent.Count == 5)
                        {
                            Name_Of_Assistent[4] = List_Words[List_Words.Count - 1];
                        }
                        else
                        {
                            Name_Of_Assistent.Add(List_Words[List_Words.Count - 1]);
                        }
                        Result_Operation = "Имя ассистента изменено";
                    }
                    else if(input.Contains("вибра"))
                    {
                        if(input.Contains("включи"))
                        {
                            if(Is_Vibration)
                            {
                                Result_Operation = "Вывод в виде вибраций уже включен";
                            }
                            else
                            {
                                Result_Operation = "Включаю режим вывода в виде вибраций";
                                VibroSwitch.IsToggled = true;
                            }
                        }
                        else if(input.Contains("выключи"))
                        {
                            if(Is_Vibration)
                            {
                                Result_Operation = "Выключаю режим вывода в виде вибраций";
                                VibroSwitch.IsToggled = false;
                            }
                            else
                            {
                                Result_Operation = "Вывод в виде вибраций уже выключен";
                                
                            }
                        }
                    }
                    else if(input.Contains("озвучив"))
                    {
                        if(input.Contains("включи"))
                        {
                            if(TellDeystvia)
                            {
                                Result_Operation = "Озвучивание уже включено";
                            }
                            else
                            {
                                TellSwitch.IsToggled = true;
                            }
                        }
                        else if(input.Contains("выключи"))
                        {
                            if (TellDeystvia)
                            {
                                TellSwitch.IsToggled = false;
                            }
                            else
                            {
                                Result_Operation = "Озвучивание уже выключено";
                            }
                        }
                    }
                    else if ((FuzzySharp.Fuzz.Ratio(List_Words[0], On_Main_Option_List[0]) > 95) || (FuzzySharp.Fuzz.Ratio(List_Words[0], On_Main_Option_List[1]) > 95))
                    {
                        if ((FuzzySharp.Fuzz.Ratio(List_Words[1], "местоположение") > 90) || (FuzzySharp.Fuzz.Ratio(List_Words[1], "геолокация") > 90))// добавить английский
                        {
                            Result_Operation = DependencyService.Resolve<IDoVoiceCommand>().EnableGPS(Language);
                        }
                        else if (FuzzySharp.Fuzz.Ratio(List_Words[1], "вай фай") > 50)
                        {
                            if ((List_Words.Count > 2 && (FuzzySharp.Fuzz.Ratio(List_Words[1] + " " + List_Words[2], "вай фай") > 90)) || (List_Words.Count > 2 && (FuzzySharp.Fuzz.Ratio(List_Words[1] + List_Words[2], "вай-фай") > 90)))
                            {
                                Result_Operation = DependencyService.Resolve<IDoVoiceCommand>().EnableWiFi(Language);
                            }
                        }
                        else if (FuzzySharp.Fuzz.Ratio(List_Words[1], "блютуз") > 90)
                        {
                            Result_Operation = DependencyService.Resolve<IDoVoiceCommand>().EnableBLuetooth(Language);
                        }
                        else if ((FuzzySharp.Fuzz.Ratio(List_Words[1], "интернет") > 90) || ((List_Words.Count > 3) && (FuzzySharp.Fuzz.Ratio(List_Words[1] + " " + List_Words[2] + " " + List_Words[3], "мобильная передача данных") > 80)))
                        {
                            Result_Operation = DependencyService.Resolve<IDoVoiceCommand>().EnableInternet(Language);
                        }
                        else
                        {
                            Result_Operation = "Извините, я не могу выполнить эту команду";
                        }
                    }
                    else if ((FuzzySharp.Fuzz.Ratio(List_Words[0], Off_Main_Option_List[0]) > 90) || (FuzzySharp.Fuzz.Ratio(List_Words[0], Off_Main_Option_List[1]) > 90))
                    {
                        if ((FuzzySharp.Fuzz.Ratio(List_Words[1], "местоположение") > 90) || (FuzzySharp.Fuzz.Ratio(List_Words[1], "геолокация") > 90))// добавить английский
                        {
                            Result_Operation = DependencyService.Resolve<IDoVoiceCommand>().DisableGPS(Language);
                        }
                        else if (FuzzySharp.Fuzz.Ratio(List_Words[1], "вай фай") > 50)
                        {
                            if ((List_Words.Count > 2 && (FuzzySharp.Fuzz.Ratio(List_Words[1] + " " + List_Words[2], "вай фай") > 90)) || (List_Words.Count > 2 && (FuzzySharp.Fuzz.Ratio(List_Words[1] + List_Words[2], "вай-фай") > 90)))
                            {
                                Result_Operation = DependencyService.Resolve<IDoVoiceCommand>().DisableWiFi(Language);
                            }
                        }
                        else if (FuzzySharp.Fuzz.Ratio(List_Words[1], "блютуз") > 90)
                        {
                            Result_Operation = DependencyService.Resolve<IDoVoiceCommand>().DisableBLuetooth(Language);
                        }
                        else if ((FuzzySharp.Fuzz.Ratio(List_Words[1], "интернет") > 90) || ((List_Words.Count > 3) && (FuzzySharp.Fuzz.Ratio(List_Words[1] + " " + List_Words[2] + " " + List_Words[3], "мобильная передача данных") > 80)))
                        {
                            Result_Operation = DependencyService.Resolve<IDoVoiceCommand>().DisableInternet(Language);
                        }
                        else
                        {
                            Result_Operation = "Извините, я не могу выполнить эту команду";
                        }
                    }
                    else if ((FuzzySharp.Fuzz.Ratio(List_Words[0], Name_Call_List[0]) > 90) || (FuzzySharp.Fuzz.Ratio(List_Words[0], Name_Call_List[1]) > 90) || (FuzzySharp.Fuzz.Ratio(List_Words[0], Name_Call_List[0]) > 90))
                    {
                        if (FuzzySharp.Fuzz.Ratio(List_Words[1], "контакту") > 90)
                        {
                            string FullContaxctName = "";
                            for (int i = 2; i < List_Words.Count; i++)
                            {
                                FullContaxctName += List_Words[i];
                            }
                            Result_Operation = DependencyService.Resolve<IDoVoiceCommand>().CallCaontact(Language, FullContaxctName);
                        }
                        else
                        {
                            string FullContaxctName = "";
                            for (int i = 1; i < List_Words.Count; i++)
                            {
                                FullContaxctName += List_Words[i];
                            }
                            Result_Operation = DependencyService.Resolve<IDoVoiceCommand>().CallCaontact(Language, FullContaxctName);
                        }
                    }
                    else if (input.Contains("ярко"))
                    {
                        DependencyService.Resolve<IDoVoiceCommand>().Set_Brightness(List_Words[List_Words.Count - 2], List_Words[List_Words.Count - 1]);
                    }
                    else if (input.Contains("врем"))
                    {
                        Result_Operation = DependencyService.Resolve<IDoVoiceCommand>().Get_Time(Language);
                    }
                    else if (input.Contains("погод"))
                    {
                        Result_Operation = DependencyService.Resolve<IDoVoiceCommand>().Get_Weather(Language, List_Words[List_Words.Count - 1]);
                    }
                    else if (input.Contains("курс"))
                    {
                        Result_Operation = DependencyService.Resolve<IDoVoiceCommand>().Get_Money(Language, List_Words[1]);
                    }
                    //else if (input.Contains("фото"))
                    //{
                    //    Result_Operation = DependencyService.Resolve<IDoVoiceCommand>().Take_Photo(Language);
                    //}
                    else if (input.Contains("буд"))
                    {
                        Result_Operation = DependencyService.Resolve<IDoVoiceCommand>().Budka(Language, List_Words);
                    }
                    else if (input.Contains("тайм"))
                    {
                        Result_Operation = DependencyService.Resolve<IDoVoiceCommand>().Timer(Language, List_Words);
                    }
                    else if (input.Contains("привет") || input.Contains("здрав"))
                    {
                        Result_Operation = "Приветсвую, чем я могу вам помочь?"; //англ
                    }
                    else if (FuzzySharp.Fuzz.Ratio(List_Words[0], "открой") > 95)
                    {
                        string InputApp = input.Substring(7, input.Length - 7);
                        Result_Operation = DependencyService.Resolve<IDoVoiceCommand>().LaunchApp(Language, InputApp);
                    }
                    else if (input.Contains("громк"))
                    {
                        Is_Change_Volume = true;
                        DependencyService.Resolve<IDoVoiceCommand>().Set_Volum(List_Words[List_Words.Count - 2], List_Words[List_Words.Count - 1]);
                    }
                    else
                    {
                        Result_Operation = "Извините, я не могу выполнить эту команду";
                    }
                }
                else
                {
                    if (input.Contains("recogn"))
                    {
                        if (input.Contains("text"))
                        {
                            Change_Interface(false, true, false, false, false, false, false);
                        }
                        else if (input.Contains("money"))
                        {
                            Change_Interface(false, false, true, false, false, false, false);
                        }
                        else if (input.Contains("image"))
                        {
                            Change_Interface(false, false, false, true, false, false, false);
                        }
                    }
                    if (input.Contains("page") && input.Contains("setti"))
                    {
                        Change_Interface(false, false, false, false, false, false, true);
                    }
                    else if (input.Contains("subtit"))
                    {
                        Change_Interface(false, false, false, false, false, true, false);
                    }
                    else if ((input.Contains("descrip")) && (input.Contains("image")))
                    {
                        Change_Interface(false, false, false, false, true, false, false);
                    }
                    else if ((input.Contains("chang")) && (input.Contains("lang")))
                    {
                        Language = false;
                        ChangeLanguge.Text = "Русский";
                        Rec = new Vosk.VoskRecognizer(Model_Ru, 16000);
                        ChangeLanguage();
                        Result_Operation = "Язык системы изменён";
                    }
                    else if ((input.Contains("name")) && (input.Contains("set")))
                    {
                        Preferences.Set("Name_of_Assistent_by_user", List_Words[List_Words.Count - 1]);
                        string x = List_Words[List_Words.Count - 1];
                        x = char.ToUpper(x[0]) + x.Substring(1);
                        NameAssistEntry.Placeholder = x;
                        if (Name_Of_Assistent.Count == 5)
                        {
                            Name_Of_Assistent[4] = List_Words[List_Words.Count - 1];
                        }
                        else
                        {
                            Name_Of_Assistent.Add(List_Words[List_Words.Count - 1]);
                        }
                        Result_Operation = "Name assistent changed";
                    }
                    else if (input.Contains("vibr") && input.Contains("turn"))
                    {
                        if (input.Contains("on"))
                        {
                            if (Is_Vibration)
                            {
                                Result_Operation = "Vibration output is already enabled";
                            }
                            else
                            {
                                Result_Operation = "I turn on the vibration output mode";
                                VibroSwitch.IsToggled = true;
                            }
                        }
                        else if (input.Contains("of"))
                        {
                            if (Is_Vibration)
                            {
                                Result_Operation = "I turn off the vibration output mode";
                                VibroSwitch.IsToggled = false;
                            }
                            else
                            {
                                Result_Operation = "The vibration output is already turned off";

                            }
                        }
                    }
                    else if(FuzzySharp.Fuzz.Ratio(List_Words[0], "turn") > 95)
                    {
                        if(FuzzySharp.Fuzz.Ratio(List_Words[0], "on") > 95) //проверить
                        {
                            if(List_Words[2].Contains("w"))
                            {
                                Result_Operation = DependencyService.Resolve<IDoVoiceCommand>().EnableWiFi(Language);
                            }
                            else if(List_Words[2].Contains("blue"))
                            {
                                Result_Operation = DependencyService.Resolve<IDoVoiceCommand>().EnableBLuetooth(Language);
                            }
                            else if(List_Words[2].Contains("locati"))
                            {
                                Result_Operation = DependencyService.Resolve<IDoVoiceCommand>().EnableGPS(Language);
                            }
                            else if(List_Words[2].Contains("mob") && List_Words[3].Contains("dat"))
                            {
                                Result_Operation = DependencyService.Resolve<IDoVoiceCommand>().EnableInternet(Language);
                            }
                            else if (input.Contains("brightness"))
                            {
                                DependencyService.Resolve<IDoVoiceCommand>().Set_Brightness(List_Words[List_Words.Count - 2], List_Words[List_Words.Count - 1]);
                            }
                            else
                            {
                                Result_Operation = "Sorry, I can't execute this command.";
                            }
                        }
                        else if(FuzzySharp.Fuzz.Ratio(List_Words[0], "off") > 95)
                        {
                            if (List_Words[2].Contains("w"))
                            {
                                Result_Operation = DependencyService.Resolve<IDoVoiceCommand>().DisableWiFi(Language);
                            }
                            else if (List_Words[2].Contains("blue"))
                            {
                                Result_Operation = DependencyService.Resolve<IDoVoiceCommand>().DisableBLuetooth(Language);
                            }
                            else if (List_Words[2].Contains("locati"))
                            {
                                Result_Operation = DependencyService.Resolve<IDoVoiceCommand>().DisableGPS(Language);
                            }
                            else if (List_Words[2].Contains("mob") && List_Words[3].Contains("dat"))
                            {
                                Result_Operation = DependencyService.Resolve<IDoVoiceCommand>().DisableInternet(Language);
                            }
                            else
                            {
                                Result_Operation = "Sorry, I can't execute this command.";
                            }
                        }
                    }
                    else if(FuzzySharp.Fuzz.Ratio(List_Words[0], "call") > 90)
                    {
                        if (FuzzySharp.Fuzz.Ratio(List_Words[1], "contact") > 90)
                        {
                            string FullContaxctName = "";
                            for (int i = 2; i < List_Words.Count; i++)
                            {
                                FullContaxctName += List_Words[i];
                            }
                            Result_Operation = DependencyService.Resolve<IDoVoiceCommand>().CallCaontact(Language, FullContaxctName);
                        }
                        else
                        {
                            string FullContaxctName = "";
                            for (int i = 1; i < List_Words.Count; i++)
                            {
                                FullContaxctName += List_Words[i];
                            }
                            Result_Operation = DependencyService.Resolve<IDoVoiceCommand>().CallCaontact(Language, FullContaxctName);
                        }
                    }
                    else if (input.Contains("rate"))
                    {
                        Result_Operation = DependencyService.Resolve<IDoVoiceCommand>().Get_Money(Language, List_Words[0]);
                    }
                    else if (input.Contains("alarm"))
                    {
                        Result_Operation = DependencyService.Resolve<IDoVoiceCommand>().Budka(Language, List_Words);
                    }
                    else if (input.Contains("timer"))
                    {
                        Result_Operation = DependencyService.Resolve<IDoVoiceCommand>().Timer(Language, List_Words);
                    }
                    else if (input.Contains("time"))
                    {
                        Result_Operation = DependencyService.Resolve<IDoVoiceCommand>().Get_Time(Language);
                    }
                    else if (input.Contains("w") && input.Contains("er"))
                    {
                        Result_Operation = DependencyService.Resolve<IDoVoiceCommand>().Get_Weather(Language, List_Words[List_Words.Count - 1]);
                    }
                    else if (FuzzySharp.Fuzz.Ratio(List_Words[0], "launch") > 95)
                    {
                        string InputApp = input.Substring(6, input.Length - 6);
                        Result_Operation = DependencyService.Resolve<IDoVoiceCommand>().LaunchApp(Language, InputApp);
                    }
                    else if (input.Contains("volume"))
                    {
                        Is_Change_Volume = true;
                        DependencyService.Resolve<IDoVoiceCommand>().Set_Volum(List_Words[List_Words.Count - 2], List_Words[List_Words.Count - 1]);
                    }
                    else if (input.Contains("hello"))
                    {
                        Result_Operation = "Hello, how can I help you?"; //англ
                    }
                    else
                    {
                        Result_Operation = "Sorry, I can't execute this command.";
                    }
                }
            }
            catch
            {
                if (Language)
                {
                    Result_Operation = "Sorry, something went wrong";
                }
                else
                {
                    Result_Operation = "Извините, что-то пошло не так";
                }
            }
            if (!Is_Change_Volume)
            {
                DependencyService.Resolve<IDoVoiceCommand>().Set_Volum(Old_Volume_List);
                Old_Volume_List.Clear();
            }
            Voice_Result(Result_Operation);
            Is_Listnig_Command = true;
        }
        async void Voice_Result(string Input)
        {
            if (Input != "")
            {
                if (Is_Vibration)
                {
                    const int dotDuration = 200;     // Точка: 200 мс
                    const int dashDuration = 600;    // Тире: 600 мс
                    const int symbolPause = 200;     // Пауза между точками/тире
                    const int letterPause = 600;     // Пауза между буквами
                    const int wordPause = 1000;      // Пауза между словами
                    Dictionary<char, string> MorseCode = new Dictionary<char, string>
                {
                    {'А', ".-"},     {'Б', "-..."},   {'В', ".--"},
                    {'Г', "--."},    {'Д', "-.."},    {'Е', "."},
                    {'Ж', "...-"},   {'З', "--.."},   {'И', ".."},
                    {'Й', ".---"},   {'К', "-.-"},    {'Л', ".-.."},
                    {'М', "--"},     {'Н', "-."},     {'О', "---"},
                    {'П', ".--."},   {'Р', ".-."},    {'С', "..."},
                    {'Т', "-"},      {'У', "..-"},    {'Ф', "..-."},
                    {'Х', "...."},   {'Ц', "-.-."},   {'Ч', "---."},
                    {'Ш', "----"},   {'Щ', "--.-"},   {'Ъ', "--.--"},
                    {'Ы', "-.--"},   {'Ь', "-..-"},   {'Э', "..-.."},
                    {'Ю', "..--"},   {'Я', ".-.-"},

                    {'A', ".-"},     {'B', "-..."},   {'C', "-.-."},
                    {'D', "-.."},    {'E', "."},      {'F', "..-."},
                    {'G', "--."},    {'H', "...."},   {'I', ".."},
                    {'J', ".---"},   {'K', "-.-"},    {'L', ".-.."},
                    {'M', "--"},     {'N', "-."},     {'O', "---"},
                    {'P', ".--."},   {'Q', "--.-"},   {'R', ".-."},
                    {'S', "..."},    {'T', "-"},      {'U', "..-"},
                    {'V', "...-"},   {'W', ".--"},    {'X', "-..-"},
                    {'Y', "-.--"},   {'Z', "--.."},

                    {'1', ".----"},  {'2', "..---"},  {'3', "...--"},
                    {'4', "....-"},  {'5', "....."},  {'6', "-...."},
                    {'7', "--..."},  {'8', "---.."},  {'9', "----."},
                    {'0', "-----"},  {' ', " "}
                };
                    Input = Input.ToUpper();

                    foreach (char c in Input)
                    {
                        if (!MorseCode.ContainsKey(c))
                            continue;

                        string morse = MorseCode[c];
                        foreach (char symbol in morse)
                        {
                            if (symbol == '.')
                                Vibration.Vibrate(dotDuration);
                            else if (symbol == '-')
                                Vibration.Vibrate(dashDuration);

                            await Task.Delay(symbolPause);
                        }

                        await Task.Delay(c == ' ' ? wordPause : letterPause);
                    }
                }
                else
                {
                    var locales = await TextToSpeech.GetLocalesAsync();
                    Locale locale_by_lenguage;
                    if (Language)
                    {
                        locale_by_lenguage = locales.FirstOrDefault(l => l.Language == "en" && l.Country == "US");
                    }
                    else
                    {
                        locale_by_lenguage = locales.FirstOrDefault(l => l.Language == "ru" && l.Country == "RU");
                    }
                    try
                    {
                        await TextToSpeech.SpeakAsync(Input, new SpeechOptions { Locale = locale_by_lenguage }, cancellationToken_Speech.Token);
                    }
                    catch (System.OperationCanceledException ex) { }
                }
            }
        }
        public void CancelSpeech()
        {
            if (cancellationToken_Speech?.IsCancellationRequested ?? true)
                return;

            cancellationToken_Speech.Cancel();
            cancellationToken_Speech.Dispose();
            cancellationToken_Speech = new CancellationTokenSource();
        }
        void ChangeTheme()
        {
            Color Maincolor;
            Color Dopcolor;
            string Dop = ".png";
            if (Is_Dark_Theme == true)
            {
                Maincolor = Color.FromRgba(4, 155, 204, 255);
                Dopcolor = Color.Black;
                Dop = "D.png";
                DependencyService.Resolve<IChangeHotBar>().ChengeHotBarByTheme(4, 155, 204, 255);
            }
            else
            {
                Maincolor = Color.FromRgba(190, 1, 190, 255);
                Dopcolor = Color.White;
                DependencyService.Resolve<IChangeHotBar>().ChengeHotBarByTheme(190, 1, 190, 255);
            }
            MainFrame.BackgroundColor = Maincolor;
            MicroButton.BackgroundColor = Maincolor;
            SettingButton.BackgroundColor = Maincolor;
            SubtitrsButton.BackgroundColor = Maincolor;
            ImageOpisButton.BackgroundColor = Maincolor;
            ObrazButton.BackgroundColor = Maincolor;
            MoneyButton.BackgroundColor = Maincolor;
            TextButton.BackgroundColor = Maincolor;

            MicroButton.BorderColor = Dopcolor;
            SettingButton.BorderColor = Dopcolor;
            SubtitrsButton.BorderColor = Dopcolor;
            ImageOpisButton.BorderColor = Dopcolor;
            ObrazButton.BorderColor = Dopcolor;
            MoneyButton.BorderColor = Dopcolor;
            TextButton.BorderColor = Dopcolor;

            MainStacklayout.BackgroundColor = Dopcolor;

            LangugeText.TextColor = Maincolor;
            ChangeLanguge.TextColor = Maincolor;
            ChangeLanguge.BackgroundColor = Dopcolor;
            ChangeLanguge.BorderColor = Maincolor;

            ThemeText.TextColor = Maincolor;
            SwitchTheme.ThumbColor = Maincolor;

            VibroText.TextColor = Maincolor;
            VibroSwitch.ThumbColor = Maincolor;

            OffMicroText.TextColor = Maincolor;
            OffMicroSwitch.ThumbColor = Maincolor;

            OffCameraText.TextColor = Maincolor;
            OffCameraSwitch.ThumbColor = Maincolor;

            BlockVisionText.TextColor = Maincolor;
            BlockVisionSwitch.ThumbColor = Maincolor;

            BlockListnText.TextColor = Maincolor;
            BlockListnSwitch.ThumbColor = Maincolor;

            NameAssistText.TextColor = Maincolor;
            NameAssistEntry.PlaceholderColor = Maincolor;
            NameAssistEntry.TextColor = Maincolor;

            MicroFrame.BackgroundColor = Dopcolor;
            MicroFrame.BorderColor = Maincolor;

            TellText.TextColor = Maincolor;
            TellSwitch.ThumbColor = Maincolor;

            MicroButton.ImageSource = "MicroSmall" + Dop;
            TextButton.ImageSource = "Text" + Dop;
            MoneyButton.ImageSource = "Money" + Dop;
            ObrazButton.ImageSource = "Obraz" + Dop;
            ImageOpisButton.ImageSource = "Image" + Dop;
            SubtitrsButton.ImageSource = "Subtitrs" + Dop;
            SettingButton.ImageSource = "Setting" + Dop;
            LangImage.Source = "Lang" + Dop;
            ThemeImage.Source = "Theme" + Dop;
            VibroImage.Source = "Hear" + Dop;
            MicroImage.Source = "Micro" + Dop;
            OffMicro.Source = "OffMicro" + Dop;
            OffCamera.Source = "OffCamera" + Dop;
            BlockVision.Source = "BlockVision" + Dop;
            BlockListn.Source = "BlockListn" + Dop;
            NameAssist.Source = "NameAssist" + Dop;
            TellImage.Source = "TelDey" + Dop;
            



            SubtitrsLabel.TextColor = Maincolor;
            SelectFileFromSub.TextColor = Maincolor;
            SelectFileFromSub.BackgroundColor = Dopcolor;
            SelectFileFromSub.BorderColor = Maincolor;
            SubtitersGrid.Children[1].BackgroundColor = Dopcolor;
            SubtitrsFrame.BackgroundColor = Dopcolor;
            SelectImageForOpis.TextColor = Maincolor;
            SelectImageForOpis.BackgroundColor = Dopcolor;
            SelectImageForOpis.BorderColor = Maincolor;
            OpisImageFrame.BackgroundColor = Dopcolor;


        }
        void RefreshVideoPlayer()
        {
            if (SubtitersGrid.Children.Count == 3)
            {
                SubtitersGrid.Children.Remove(SubtitersGrid.Children[1]);
            }
            SubtitersGrid.Children.Insert(1, new MediaElement
            {
                HeightRequest = 300,
                VerticalOptions = LayoutOptions.FillAndExpand,
            });
            Grid.SetColumn(SubtitersGrid.Children[1],0);
            Grid.SetRow(SubtitersGrid.Children[1],1);
            SubtitrsLabel.Text = "";
            //SubtitersGrid.Children.Insert(1, new MediaElement()
            //{
            //    HeightRequest = 300,
            //    VerticalOptions = LayoutOptions.FillAndExpand,
            //    Grid

            //});
        }
        void ChangeLanguage()
        {
            if (Language == true)
            {
                LangugeText.Text = "Language";
                ThemeText.Text = "Dark theme";
                VibroText.Text = "Vibration";
                ChangeLanguge.Text = "English";
                OffMicroText.Text = "Microphine";
                OffCameraText.Text = "Camera";
                SelectImageForOpis.Text = "Select photo";
                SelectFileFromSub.Text = "Select video";
                NameAssistText.Text = "Name";
                BlockVisionText.Text = "Patterns";
                BlockListnText.Text = "Subtitles";
                TellText.Text = "Dubbing";
                Preferences.Set("Language", Language);
            }
            else
            {
                LangugeText.Text = "Язык";
                NameAssistText.Text = "Имя";
                ThemeText.Text = "Темная тема";
                VibroText.Text = "Вибрация";
                ChangeLanguge.Text = "Русский";
                OffMicroText.Text = "Микрофон";
                OffCameraText.Text = "Камера";
                SelectImageForOpis.Text = "Выбрать фото";
                SelectFileFromSub.Text = "Выбрать видео";
                BlockVisionText.Text = "Образы";
                BlockListnText.Text = "Субтитры";
                TellText.Text = "Озвучивание";
                Preferences.Set("Language", Language);
            }
        }
        private async void SelectFileFromSub_Clicked(object sender, EventArgs e)
        {
            if (!Is_No_Loaded)
            {
                if(Permissions.CheckStatusAsync<Permissions.StorageWrite>().Result == PermissionStatus.Granted)
                {
                    RefreshVideoPlayer();
                    var Video = await MediaPicker.PickVideoAsync();
                    if (!string.IsNullOrWhiteSpace(Video.FullPath))
                    {
                        SubtitrsLabel.Text = "";
                        RecognizeFromDisk(Video.FullPath);
                    }
                }
                else
                {
                    if (Language)
                    {
                        Voice_Result("Allow access to files and media content in the settings");
                    }
                    else
                    {
                        Voice_Result("Разрешите доступ к файлам и медиаконтенту в настройках");
                    }
                }
            }
            else
            {
                if(Language)
                {
                    await DisplayAlert("Attention!", "The speech related module is not loaded to load the load on the main page.", "Ок");
                }
                else
                {
                    await DisplayAlert("Внимание!", "Не загружен модуль распознавания речи, для загрузки перейдите на главную станицу", "Ок");
                }
            }

        }

        private async void ChangeLanguge_Clicked(object sender, EventArgs e)
        {
           // Is_Listnig_Command = false;
            //Off_Micro = true;
            var action = await DisplayActionSheet("Выбрать", "Отмена", "Выбрать", "Русский", "English");

            if (action == "English")
            {
                Language = true;
                ChangeLanguge.Text = action;
                Rec = new Vosk.VoskRecognizer(Model_En, 16000);
                ChangeLanguage();
            }
            else if (action == "Русский")
            {
                ChangeLanguge.Text = action;
                Rec = new Vosk.VoskRecognizer(Model_Ru, 16000);
                Language = false;
                ChangeLanguage();
            }
            Is_Listnig_Command = true;
            Off_Micro = false;
        }

        private void Change_Interface(bool MicroFrameIsVis, bool TextRecStackIsVis, bool MoneyFrameIsVis, bool ObrazRecStackIsVis, bool PictureStacktIsVis, bool SubtitrsFrameIsVis, bool SettingStackLayoutIsVis)
        {
            MicroFrame.IsVisible = MicroFrameIsVis;
            if (TextRecStackIsVis || MoneyFrameIsVis || ObrazRecStackIsVis)
            {
                if (Permissions.CheckStatusAsync<Permissions.Camera>().Result==PermissionStatus.Granted)
                {
                    RecognizeStackLayout.IsVisible = true;
                    Old_Ver_Rec = 0;
                    Is_Zoom = 0;
                    Thread thread = new Thread(Take_Capture);
                    thread.Start();
                }
            }
            else
            {
                RecognizeStackLayout.IsVisible = false;
            }
            OpisImageFrame.IsVisible = PictureStacktIsVis;
            SubtitrsFrame.IsVisible = SubtitrsFrameIsVis;
            SettingStackLayout.IsVisible = SettingStackLayoutIsVis;
            if (((Is_Text_Recognize == true) && (TextRecStackIsVis == false)) || ((Is_Money_Recognize == true) && (MoneyFrameIsVis == false))||((Is_Text_Recognize == true) && (ObrazRecStackIsVis == false)))
            {
                _cts?.Cancel();
                _cts.Dispose(); 
                _cts = new CancellationTokenSource();
                CancelSpeech();
            }
            Is_Text_Recognize = TextRecStackIsVis;
            Is_Money_Recognize = MoneyFrameIsVis;
            Is_Obraz_Recognize = ObrazRecStackIsVis;
            //TextRecognizeScaner.IsScanning = TextRecStackIsVis;
            if(!PictureStacktIsVis)
            {
                ImageForOpis.Source = null;
            }
            if (!SubtitrsFrameIsVis)
            {
                ((MediaElement)SubtitersGrid.Children[1]).Stop();
                RefreshVideoPlayer();
            }
        }
        private void SettingButton_Clicked(object sender, EventArgs e)
        {
            Change_Interface(false, false, false, false, false, false, true);
        }

        private void MicroButton_Clicked(object sender, EventArgs e)
        {
            InitRecognizer();
            Change_Interface(true, false, false, false, false, false, false);
        }
        private void TextButton_Clicked(object sender, EventArgs e)
        {
            Change_Interface(false, true, false, false, false, false, false);
            // Text_Recognize();
        }
        private void SubtitrsButton_Clicked(object sender, EventArgs e)
        {
            Change_Interface(false, false, false, false, false, true, false);
        }
        private void MoneyButton_Clicked(object sender, EventArgs e)
        {
            Change_Interface(false, false, true, false, false, false, false);
            //var photo = await MediaPicker.CapturePhotoAsync();
            //ImageMoney.Source = ImageSource.FromFile(photo.FullPath);
            //FinalFrame = new VideoCaptureDevice(CaptureDevices[0].MonikerString);
            //FinalFrame.NewFrame += new NewFrameEventHandler(FinalFrame_NewFrame);
            //FinalFrame.Start();
        }
        private void SwitchTheme_Toggled(object sender, ToggledEventArgs e)
        {
            if (SwitchTheme.IsToggled)
            {
                Preferences.Set("Is_Dark_Theme", true);
                Is_Dark_Theme = true;
                ChangeTheme();
            }
            else
            {
                Preferences.Set("Is_Dark_Theme", false);
                Is_Dark_Theme = false;
                ChangeTheme();
            }
            if (TellDeystvia)
            {
                if (Language && Is_Dark_Theme)
                {
                    Voice_Result("I turn on the dark theme");
                }
                else if (Language && !Is_Dark_Theme)
                {
                    Voice_Result("I turn on the light theme");
                }
                else if (!Language && Is_Dark_Theme)
                {
                    Voice_Result("Включаю темную тему");
                }
                else if (!Language && !Is_Dark_Theme)
                {
                    Voice_Result("Включаю светлую тему");
                }
            }
        }

        private void VibroSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            Is_Vibration = VibroSwitch.IsToggled;
            Preferences.Set("Is_Vibration", Is_Vibration);
            if (TellDeystvia)
            {
                if (Language && Is_Vibration)
                {
                    Voice_Result("I turn on the vibration output function");
                }
                else if (Language && !Is_Vibration)
                {
                    Voice_Result("I turn off the vibration output function");
                }
                else if (!Language && Is_Vibration)
                {
                    Voice_Result("Включаю функцию вывода в виде вибраций");
                }
                else if (!Language && !Is_Vibration)
                {
                    Voice_Result("Отключаю функцию вывода в виде вибраций");
                }
            }
        }

        private void ObrazButton_Clicked(object sender, EventArgs e)
        {
            Change_Interface(false, false, false, true, false, false, false);
        }

        private void ImageOpisButton_Clicked(object sender, EventArgs e)
        {
            Change_Interface(false, false, false, false, true, false, false);
        }


        private async void cameraView_MediaCaptured(object sender, MediaCapturedEventArgs e)
        {
            string x = "";
            if (Is_Text_Recognize==true && Is_Money_Recognize==false && Is_Obraz_Recognize ==false)
            {
                if (Language)
                {
                    Voice_Result("Recognizing text, please wait");
                }
                else
                {
                    Voice_Result("Распознаю текст, пожалуйста, подождите");
                }
                x = await Work_Recognize_Text(e.ImageData);
            }
            else if (Is_Text_Recognize == false && (Is_Money_Recognize == true || Is_Obraz_Recognize == true))
            {
                 x = await  Work_Recognize_Opraz_And_Money(e.ImageData, _cts.Token);
            }
            if (x != "")
            {
                cameraView.Shutter();
            }

        }
        private async Task<string> Work_Recognize_Text(byte[] data)
        {
            string Result_Text_Recognize = await DependencyService.Get<ITesseractHelper>().Main(data,Language,_cts.Token);
            if ((FuzzySharp.Fuzz.Ratio(Result_Text_Recognize, Old_Text) < 75)&& (Result_Text_Recognize!=""))
            {
                Old_Text = Result_Text_Recognize;
                Voice_Result(Result_Text_Recognize);
            }
            return Result_Text_Recognize;
        }

        private void OffMicroSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            Off_Micro = OffMicroSwitch.IsToggled;
            Preferences.Set("Off_Micro", Off_Micro);
            if (TellDeystvia)
            {
                if (Language && Off_Micro)
                {
                    Voice_Result("I turn off the microphone");
                }
                else if (Language && !Off_Micro)
                {
                    Voice_Result("I turn on the microphone");
                }
                else if (!Language && Off_Micro)
                {
                    Voice_Result("Отключаю работу микрофона");
                }
                else if (!Language && !Off_Micro)
                {
                    Voice_Result("Включаю работу микрофона");
                }
            }
        }

        private void OffCameraSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            Off_Camera = OffCameraSwitch.IsToggled;
            Preferences.Set("Off_Camera", Off_Camera);
            TextButton.IsEnabled = !Off_Camera;
            MoneyButton.IsEnabled = !Off_Camera;
            ObrazButton.IsEnabled = !Off_Camera;
            if (TellDeystvia)
            {
                if (Language && Off_Camera)
                {
                    Voice_Result("I turn off the camera");
                }
                else if (Language && !Off_Camera)
                {
                    Voice_Result("I turn on the camera");
                }
                else if (!Language && Off_Camera)
                {
                    Voice_Result("Отключаю работу камеры");
                }
                else if (!Language && !Off_Camera)
                {
                    Voice_Result("Включаю работу камеры");
                }
            }
        }

        private async void NameAssistEntry_Focused(object sender, FocusEventArgs e)
        {
            string result = "";
            if (Language)
            {
                result = await DisplayPromptAsync("Setting the name", "Please enter name (max 10 characters)");
            }
            else
            {
                result = await DisplayPromptAsync("Установка имени", "Пожалуйста, введите имя (не более 10 символов)","Ок","Выход");
            }
            if ((result != null))
            {
                char[] russianLetters = new char[]
                {
                    // Заглавные буквы (А-Я)
                    'А', 'Б', 'В', 'Г', 'Д', 'Е', 'Ё', 'Ж', 'З', 'И', 'Й',
                    'К', 'Л', 'М', 'Н', 'О', 'П', 'Р', 'С', 'Т', 'У', 'Ф',
                    'Х', 'Ц', 'Ч', 'Ш', 'Щ', 'Ъ', 'Ы', 'Ь', 'Э', 'Ю', 'Я',
    
                    // Строчные буквы (а-я)
                    'а', 'б', 'в', 'г', 'д', 'е', 'ё', 'ж', 'з', 'и', 'й',
                    'к', 'л', 'м', 'н', 'о', 'п', 'р', 'с', 'т', 'у', 'ф',
                    'х', 'ц', 'ч', 'ш', 'щ', 'ъ', 'ы', 'ь', 'э', 'ю', 'я'
                };
                string PromResult = result.Trim().ToLower();
                for (int i = 0; i < PromResult.Length; i++)
                {
                    bool Is_Ok = false;
                    for (int j = 0; j < russianLetters.Length; j++)
                    {
                        if(PromResult[i]== russianLetters[j])
                        {
                            Is_Ok = true;
                            break;
                        }
                    }
                    if(!Is_Ok)
                    {
                        PromResult = PromResult.Remove(i,1);
                        if(PromResult.Length==0)
                        {
                            break;
                        }
                        i--;
                    }
                }
                if ((PromResult.Length > 10 || PromResult.Length < 1))
                {
                    if (Language)
                    {
                        await DisplayAlert("Attention!", "The length must not exceed 10 characters.", "Ок");
                    }
                    else
                    {
                        await DisplayAlert("Внимание!", "Длина не должна превышать 10 символов или имя не должно быть пyстым", "Ок");
                    }
                }
                else
                {
                    
                    Preferences.Set("Name_of_Assistent_by_user", PromResult);
                    string x = PromResult;
                    x = char.ToUpper(x[0]) + x.Substring(1);
                    NameAssistEntry.Placeholder = x;
                    if (Name_Of_Assistent.Count == 5)
                    {
                        Name_Of_Assistent[4] = PromResult;
                    }
                    else
                    {
                        Name_Of_Assistent.Add(PromResult);
                    }
                }
            }
        }

        private void Take_Capture()
        {
            Thread.Sleep(2000);
            cameraView.Shutter();

        }
        private async Task<string> Work_Recognize_Opraz_And_Money(byte[] data, CancellationToken token)
        {
            try
            {
                string Otdal = "";
                string Pribl = "";
                string Output = "";
                if (Language)
                {
                    Otdal = "Zoom out the camera";
                    Pribl = "Zoom in the camera";
                }
                else
                {
                    Otdal = "Отдалите камеру";
                    Pribl = "Приблизьте камеру";
                }
                Is_Time_Recognize = true;
                ImageClassificationModel imageClassificationModel = await DependencyService.Get<ITensorflowClassifier>().Classify(data, Language, Is_Money_Recognize, _cts.Token);
                Output = imageClassificationModel.TagName;
                token.ThrowIfCancellationRequested();
                if (Output != "")
                {
                    if (imageClassificationModel.Probability >= 0.75)
                    {
                        Voice_Result(imageClassificationModel.TagName);
                        cameraView.FlashMode = Xamarin.CommunityToolkit.UI.Views.CameraFlashMode.Off;
                    }
                    else
                    {
                        cameraView.FlashMode = Xamarin.CommunityToolkit.UI.Views.CameraFlashMode.On;
                        if (Is_Zoom == 0)
                        {
                            Is_Zoom = 1;
                            Voice_Result(Pribl);
                        }
                        else if (Is_Zoom == 1 && imageClassificationModel.Probability <= Old_Ver_Rec)
                        {
                            Is_Zoom = -1;
                            Voice_Result(Otdal);
                        }
                        else if (Is_Zoom == -1 && imageClassificationModel.Probability <= Old_Ver_Rec)
                        {
                            Is_Zoom = 1;
                            Voice_Result(Pribl);
                        }
                        else if (Is_Zoom == 1 && imageClassificationModel.Probability >= Old_Ver_Rec)
                        {
                            Voice_Result(Pribl);
                        }
                        else if (Is_Zoom == -1 && imageClassificationModel.Probability >= Old_Ver_Rec)
                        {
                            Voice_Result(Otdal);
                        }
                        Old_Ver_Rec = imageClassificationModel.Probability;
                    }
                }
                return Output;
            }
            catch (System.OperationCanceledException ex)
            {
                return "";
            }
            catch
            {
                return "";
            }
        }

        private async void SelectImageForOpis_Clicked(object sender, EventArgs e)
        {
            if (Permissions.CheckStatusAsync<Permissions.StorageWrite>().Result == PermissionStatus.Granted)
            {
                var photo = await MediaPicker.PickPhotoAsync();
                if (!string.IsNullOrWhiteSpace(photo.FullPath))
                {

                    ImageForOpis.Source = photo.FullPath;
                    var stream = await photo.OpenReadAsync();
                    MemoryStream Mem = new MemoryStream();
                    stream.CopyTo(Mem);
                    byte[] input = Mem.ToArray();
                    ImageClassificationModel imageClassificationModel = await DependencyService.Get<ITensorflowClassifier>().Classify(input, Language, false, _cts.Token);
                    if (imageClassificationModel.TagName != "")
                    {
                        if (imageClassificationModel.Probability >= 0.75)
                        {
                            Voice_Result(imageClassificationModel.TagName);
                        }
                        else
                        {
                            if (Language)
                            {
                                Voice_Result("No objects found");
                            }
                            else
                            {
                                Voice_Result("Объекты не обнаружены");
                            }
                        }
                    }
                }
            }
            else
            {
                if (Language)
                {
                    Voice_Result("Allow access to files and media content in the settings");
                }
                else
                {
                    Voice_Result("Разрешите доступ к файлам и медиаконтенту в настройках");
                }
            }
        }

        private void BlockVisionSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            Block_Vision = BlockVisionSwitch.IsToggled;
            Preferences.Set("Block_Vision", Block_Vision);
            TextButton.IsVisible = !Block_Vision;
            MoneyButton.IsVisible = !Block_Vision;
            ObrazButton.IsVisible = !Block_Vision;
            ImageOpisButton.IsVisible = !Block_Vision;
            if (TellDeystvia)
            {
                if (Language && Block_Vision)
                {
                    Voice_Result("Disabling image recognition features");
                }
                else if (Language && !Block_Vision)
                {
                    Voice_Result("I turn on the subtitle creation function");
                }
                else if (!Language && Block_Vision)
                {
                    Voice_Result("Отключаю функции распознавания образов");
                }
                else if (!Language && !Block_Vision)
                {
                    Voice_Result("Включаю функции распознавания образов");
                }
            }
        }

        private void BlockListnSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            Block_Listn = BlockListnSwitch.IsToggled;
            Preferences.Set("Block_Listn", Block_Listn);
            SubtitrsButton.IsVisible = !Block_Listn;
            if(TellDeystvia)
            {
                if(Language && Block_Listn)
                {
                    Voice_Result("I turn off the subtitle creation function");
                }
                else if(Language && !Block_Listn)
                {
                    Voice_Result("I turn on the subtitle creation function");
                }
                else if(!Language && Block_Listn)
                {
                    Voice_Result("Отключаю функцию создания субтитров");
                }
                else if(!Language && !Block_Listn)
                {
                    Voice_Result("Включаю функцию создания субтитров");
                }
            }
        }
        private void TellSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            TellDeystvia = TellSwitch.IsToggled;
            Preferences.Set("TellDeystvia", TellDeystvia);
            if (TellDeystvia)
            {
                if (Language)
                {
                    Voice_Result("I turn on the sound of keystrokes");
                }
                else
                {
                    Voice_Result("Включаю озвучивание нажатий");
                }
            }
            else
            {
                if (Language)
                {
                    Voice_Result("I turn off the sound of keystrokes");
                }
                else
                {
                    Voice_Result("Выключаю озвучивание нажатий");
                }
            }
        }
    }
}
