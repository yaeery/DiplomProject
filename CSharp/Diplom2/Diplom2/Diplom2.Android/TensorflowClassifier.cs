using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Diplom2.Droid;
using Java.IO;
using Java.Nio;
using Java.Nio.Channels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.TensorFlow.Lite;

[assembly: Dependency(typeof(TensorflowClassifier))]
namespace Diplom2.Droid
{
    public class TensorflowClassifier : ITensorflowClassifier
    {
        const int FloatSize = 4;
        const int PixelSize = 3;
        public async Task<ImageClassificationModel> Classify(byte[] image, bool Language, bool Is_Money_Recognize, CancellationToken token)
        {
            try
            {
               return await Task.Run(async () =>
               {
                   StreamReader streamReader;
                   if (Is_Money_Recognize)
                   {
                       streamReader = new StreamReader(Android.App.Application.Context.Assets.Open("MoneyLabels.txt"));
                   }
                   else
                   {
                       streamReader = new StreamReader(Android.App.Application.Context.Assets.Open("MainObraz.txt"));
                   }
                   var mappedByteBuffer = GetModelAsMappedByteBuffer(Is_Money_Recognize);



               var interpreter = new Interpreter(mappedByteBuffer); //-старое

               var tensor = interpreter.GetInputTensor(0);
                   var shape = tensor.Shape();

                   var width = shape[1];
                   var height = shape[2];
                   var byteBuffer = GetPhotoAsByteBuffer(image, width, height);


               var labels = streamReader.ReadToEnd().Split('\n').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();

               var outputLocations = new float[1][] { new float[labels.Count] };

                   var outputs = Java.Lang.Object.FromArray(outputLocations);

                   interpreter.Run(byteBuffer, outputs);
                   var classificationResult = outputs.ToArray<float[]>();

               var classificationModelList = new List<ImageClassificationModel>();

                   for (var i = 0; i < labels.Count; i++)
                   {
                       var label = labels[i];
                       classificationModelList.Add(new ImageClassificationModel(label, classificationResult[0][i]));
                   }
                   classificationModelList.Sort((l, r) => r.Probability.CompareTo(l.Probability));

                   string Result_Ru = "";
                   string Result_En = "";
                   string Result = "";
                   if (Is_Money_Recognize)
                   {
                       switch (classificationModelList[0].TagName)
                       {
                           case "5_rub":
                               Result_Ru = "Пять рублей";
                               Result_En = "Five rubles";
                               break;
                           case "10_rub":
                               Result_Ru = "Десять рублей";
                               Result_En = "Ten rubles";
                               break;
                           case "50_rub":
                               Result_Ru = "Пятьдесят рублей";
                               Result_En = "Fifty rubles";
                               break;
                           case "100_rub":
                               Result_Ru = "Сто рублей";
                               Result_En = "One hundred rubles";
                               break;
                           case "500_rub":
                               Result_Ru = "Пятьсот рублей";
                               Result_En = "Five hundred rubles";
                               break;
                           case "1000_rub":
                               Result_Ru = "Тысяча рублей";
                               Result_En = "One thousand rubles";
                               break;
                           case "2000_rub":
                               Result_Ru = "Две тысячи рублей";
                               Result_En = "Two thousand rubles";
                               break;
                           case "5000_rub":
                               Result_Ru = "Пять тысяч рублей";
                               Result_En = "Five thousand rubles";
                               break;
                       }
                   }
                   else
                   {
                       switch (classificationModelList[0].TagName)
                       {
                           case "banana":
                               Result_Ru = "Банан";
                               Result_En = "banana";
                               break;
                           case "minibus":
                               Result_Ru = "Микроавтобус";
                               Result_En = "minibus";
                               break;
                           case "minivan":
                               Result_Ru = "Минивен";
                               Result_En = "minivan";
                               break;
                           case "laptop":
                               Result_Ru = "Ноутбук";
                               Result_En = "laptop";
                               break;
                           case "mouse":
                               Result_Ru = "Компьютерная мышь";
                               Result_En = "mouse";
                               break;
                           case "coffee mug":
                               Result_Ru = "Кружка";
                               Result_En = "coffee mug";
                               break;
                               //case "Apple":
                               //    Result_Ru = "Яблоко";
                               //    Result_En = "Apple";
                               //    break;
                               //case "Apricot":
                               //    Result_Ru = "Абрикос";
                               //    Result_En = "Apricot";
                               //    break;
                               //case "Avocado":
                               //    Result_Ru = "Авокадо";
                               //    Result_En = "Avocado";
                               //    break;
                               //case "Banana":
                               //    Result_Ru = "Банан";
                               //    Result_En = "Banana";
                               //    break;
                               //case "Bat":
                               //    Result_Ru = "Летучая мышь";
                               //    Result_En = "Bat";
                               //    break;
                               //case "Bathtub":
                               //    Result_Ru = "Ванна";
                               //    Result_En = "Bathtub";
                               //    break;
                               //case "Bear":
                               //    Result_Ru = "Медведь";
                               //    Result_En = "Bear";
                               //    break;
                               //case "Bed":
                               //    Result_Ru = "Кровать";
                               //    Result_En = "Bed";
                               //    break;
                               //case "Beetroot":
                               //    Result_Ru = "Свёкла";
                               //    Result_En = "Beetroot";
                               //    break;
                               //case "Blueberry":
                               //    Result_Ru = "Черника";
                               //    Result_En = "Blueberry";
                               //    break;
                               //case "Bulldozer":
                               //    Result_Ru = "Бульдозер";
                               //    Result_En = "Bulldozer";
                               //    break;
                               //case "Butterfly":
                               //    Result_Ru = "Бабочка";
                               //    Result_En = "Butterfly";
                               //    break;
                               //case "Cactus fruit":
                               //    Result_Ru = "Плод кактуса";
                               //    Result_En = "Cactus fruit";
                               //    break;
                               //case "Cake":
                               //    Result_Ru = "Торт";
                               //    Result_En = "Cake";
                               //    break;
                               //case "Camel":
                               //    Result_Ru = "Верблюд";
                               //    Result_En = "Camel";
                               //    break;
                               //case "Cantaloupe 1":
                               //    Result_Ru = "Дыня";
                               //    Result_En = "Cantaloupe";
                               //    break;
                               //case "Cauliflower":
                               //    Result_Ru = "Цветная капуста";
                               //    Result_En = "Cauliflower";
                               //    break;
                               //case "Cherry":
                               //    Result_Ru = "Вишня";
                               //    Result_En = "Cherry";
                               //    break;
                               //case "Chimp":
                               //    Result_Ru = "Шимпанзе";
                               //    Result_En = "Chimp";
                               //    break;
                               //case "Closet":
                               //    Result_Ru = "Шкаф";
                               //    Result_En = "Closet";
                               //    break;
                               //case "Cocos":
                               //    Result_Ru = "Кокос";
                               //    Result_En = "Coconut";
                               //    break;
                               //case "ComputerKeyboard":
                               //    Result_Ru = "Компьютерная клавиатура";
                               //    Result_En = "Computer keyboard";
                               //    break;
                               //case "ComputerMonitor":
                               //    Result_Ru = "Монитор";
                               //    Result_En = "Computer monitor";
                               //    break;
                               //case "ComputerMouse":
                               //    Result_Ru = "Компьютерная мышь";
                               //    Result_En = "Computer mouse";
                               //    break;
                               //case "Corn":
                               //    Result_Ru = "Кукуруза";
                               //    Result_En = "Corn";
                               //    break;
                               //case "Crab":
                               //    Result_Ru = "Краб";
                               //    Result_En = "Crab";
                               //    break;
                               //case "Cup":
                               //    Result_Ru = "Чашка";
                               //    Result_En = "Cup";
                               //    break;
                               //case "Dog":
                               //    Result_Ru = "Собака";
                               //    Result_En = "Dog";
                               //    break;
                               //case "Dolphin":
                               //    Result_Ru = "Дельфин";
                               //    Result_En = "Dolphin";
                               //    break;
                               //case "Door":
                               //    Result_Ru = "Дверь";
                               //    Result_En = "Door";
                               //    break;
                               //case "Doorknob":
                               //    Result_Ru = "Дверная ручка";
                               //    Result_En = "Doorknob";
                               //    break;
                               //case "Duck":
                               //    Result_Ru = "Утка";
                               //    Result_En = "Duck";
                               //    break;
                               //case "Eggplant":
                               //    Result_Ru = "Баклажан";
                               //    Result_En = "Eggplant";
                               //    break;
                               //case "Elephant":
                               //    Result_Ru = "Слон";
                               //    Result_En = "Elephant";
                               //    break;
                               //case "Elk":
                               //    Result_Ru = "Лось";
                               //    Result_En = "Elk";
                               //    break;
                               //case "FireExtinguisher":
                               //    Result_Ru = "Огнетушитель";
                               //    Result_En = "Fire extinguisher";
                               //    break;
                               //case "FireTruck":
                               //    Result_Ru = "Пожарная машина";
                               //    Result_En = "Fire truck";
                               //    break;
                               //case "Fork":
                               //    Result_Ru = "Вилка";
                               //    Result_En = "Fork";
                               //    break;
                               //case "FriedEgg":
                               //    Result_Ru = "Яичница";
                               //    Result_En = "Fried egg";
                               //    break;
                               //case "Frog":
                               //    Result_Ru = "Лягушка";
                               //    Result_En = "Frog";
                               //    break;
                               //case "Frying-pan":
                               //    Result_Ru = "Сковорода";
                               //    Result_En = "Frying pan";
                               //    break;
                               //case "Giraffe":
                               //    Result_Ru = "Жираф";
                               //    Result_En = "Giraffe";
                               //    break;
                               //case "Goat":
                               //    Result_Ru = "Коза";
                               //    Result_En = "Goat";
                               //    break;
                               //case "Goldfish":
                               //    Result_Ru = "Золотая рыбка";
                               //    Result_En = "Goldfish";
                               //    break;
                               //case "Goose":
                               //    Result_Ru = "Гусь";
                               //    Result_En = "Goose";
                               //    break;
                               //case "Gorilla":
                               //    Result_Ru = "Горилла";
                               //    Result_En = "Gorilla";
                               //    break;
                               //case "Grape White":
                               //    Result_Ru = "Белый виноград";
                               //    Result_En = "White grape";
                               //    break;
                               //case "Hamburger":
                               //    Result_Ru = "Гамбургер";
                               //    Result_En = "Hamburger";
                               //    break;
                               //case "Hawksbill":
                               //    Result_Ru = "Черепаха";
                               //    Result_En = "Hawksbill turtle";
                               //    break;
                               //case "HeadPhones":
                               //    Result_Ru = "Наушники";
                               //    Result_En = "Headphones";
                               //    break;
                               //case "Horse":
                               //    Result_Ru = "Лошадь";
                               //    Result_En = "Horse";
                               //    break;
                               //case "HotDog":
                               //    Result_Ru = "Хот-дог";
                               //    Result_En = "Hot dog";
                               //    break;
                               //case "IceCeamCone":
                               //    Result_Ru = "Рожок мороженого";
                               //    Result_En = "Ice cream cone";
                               //    break;
                               //case "Kangaroo":
                               //    Result_Ru = "Кенгуру";
                               //    Result_En = "Kangaroo";
                               //    break;
                               //case "Kiwi":
                               //    Result_Ru = "Киви";
                               //    Result_En = "Kiwi";
                               //    break;
                               //case "Knife":
                               //    Result_Ru = "Нож";
                               //    Result_En = "Knife";
                               //    break;
                               //case "Laptop":
                               //    Result_Ru = "Ноутбук";
                               //    Result_En = "Laptop";
                               //    break;
                               //case "Lemon":
                               //    Result_Ru = "Лимон";
                               //    Result_En = "Lemon";
                               //    break;
                               //case "Limes":
                               //    Result_Ru = "Лайм";
                               //    Result_En = "Lime";
                               //    break;
                               //case "Mandarine":
                               //    Result_Ru = "Мандарин";
                               //    Result_En = "Mandarin";
                               //    break;
                               //case "Mango":
                               //    Result_Ru = "Манго";
                               //    Result_En = "Mango";
                               //    break;
                               //case "Microwave":
                               //    Result_Ru = "Микроволновая печь";
                               //    Result_En = "Microwave";
                               //    break;
                               //case "Motorbikes":
                               //    Result_Ru = "Мотоциклы";
                               //    Result_En = "Motorbikes";
                               //    break;
                               //case "MountainBike":
                               //    Result_Ru = "Горный велосипед";
                               //    Result_En = "Mountain bike";
                               //    break;
                               //case "MultyTool":
                               //    Result_Ru = "Мультитул";
                               //    Result_En = "Multitool";
                               //    break;
                               //case "Mushroom":
                               //    Result_Ru = "Гриб";
                               //    Result_En = "Mushroom";
                               //    break;
                               //case "Onion":
                               //    Result_Ru = "Лук";
                               //    Result_En = "Onion";
                               //    break;
                               //case "Orange":
                               //    Result_Ru = "Апельсин";
                               //    Result_En = "Orange";
                               //    break;
                               //case "Owl":
                               //    Result_Ru = "Сова";
                               //    Result_En = "Owl";
                               //    break;
                               //case "Pear":
                               //    Result_Ru = "Груша";
                               //    Result_En = "Pear";
                               //    break;
                               //case "Penguin":
                               //    Result_Ru = "Пингвин";
                               //    Result_En = "Penguin";
                               //    break;
                               //case "People":
                               //    Result_Ru = "Люди";
                               //    Result_En = "People";
                               //    break;
                               //case "Pepper":
                               //    Result_Ru = "Перец";
                               //    Result_En = "Pepper";
                               //    break;
                               //case "Potato":
                               //    Result_Ru = "Картофель";
                               //    Result_En = "Potato";
                               //    break;
                               //case "Refrigerator":
                               //    Result_Ru = "Холодильник";
                               //    Result_En = "Refrigerator";
                               //    break;
                               //case "SchoolBus":
                               //    Result_Ru = "Школьный автобус";
                               //    Result_En = "School bus";
                               //    break;
                               //case "Sink":
                               //    Result_Ru = "Раковина";
                               //    Result_En = "Sink";
                               //    break;
                               //case "Snake":
                               //    Result_Ru = "Змея";
                               //    Result_En = "Snake";
                               //    break;
                               //case "Spaghetti":
                               //    Result_Ru = "Спагетти";
                               //    Result_En = "Spaghetti";
                               //    break;
                               //case "Spoon":
                               //    Result_Ru = "Ложка";
                               //    Result_En = "Spoon";
                               //    break;
                               //case "Sushi":
                               //    Result_Ru = "Суши";
                               //    Result_En = "Sushi";
                               //    break;
                               //case "Table":
                               //    Result_Ru = "Стол";
                               //    Result_En = "Table";
                               //    break;
                               //case "Teapot":
                               //    Result_Ru = "Чайник";
                               //    Result_En = "Teapot";
                               //    break;
                               //case "Toaster":
                               //    Result_Ru = "Тостер";
                               //    Result_En = "Toaster";
                               //    break;
                               //case "Toilet":
                               //    Result_Ru = "Туалет";
                               //    Result_En = "Toilet";
                               //    break;
                               //case "Tomato":
                               //    Result_Ru = "Помидор";
                               //    Result_En = "Tomato";
                               //    break;
                               //case "TrafficLight":
                               //    Result_Ru = "Светофор";
                               //    Result_En = "Traffic light";
                               //    break;
                               //case "WashingMachine":
                               //    Result_Ru = "Стиральная машина";
                               //    Result_En = "Washing machine";
                               //    break;
                               //case "Watermelon":
                               //    Result_Ru = "Арбуз";
                               //    Result_En = "Watermelon";
                               //    break;
                       }
                   }
                   if (Language)
                   {
                       Result = Result_En;
                   }
                   else
                   {
                       Result = Result_Ru;
                   }
                   return new ImageClassificationModel(Result, classificationModelList[0].Probability);
               }, token);
            }
            catch (System.OperationCanceledException ex)
            {
                return new ImageClassificationModel("", 0);
            }


        }

        private MappedByteBuffer GetModelAsMappedByteBuffer(bool Is_Money_Recognize)
        {
            Android.Content.Res.AssetFileDescriptor assetDescriptor;
            if (Is_Money_Recognize)
            {
                assetDescriptor = Android.App.Application.Context.Assets.OpenFd("money_classifier.tflite");
            }
            else
            {
                assetDescriptor = Android.App.Application.Context.Assets.OpenFd("mobilenetv2_compatible.tflite");
            }

            var inputStream = new FileInputStream(assetDescriptor.FileDescriptor);

            var mappedByteBuffer = inputStream.Channel.Map(FileChannel.MapMode.ReadOnly, assetDescriptor.StartOffset, assetDescriptor.DeclaredLength);

            return mappedByteBuffer;

        }

        private ByteBuffer GetPhotoAsByteBuffer(byte[] image, int width, int height)
        {
            var bitmap_ = BitmapFactory.DecodeByteArray(image, 0, image.Length);
            Matrix matrix = new Matrix();
            matrix.PostRotate(90);
            Bitmap bitmap = Bitmap.CreateBitmap(
                bitmap_,
                0, 0,
                bitmap_.Width,
                bitmap_.Height,
                matrix,
                true
            );
            var st = new MemoryStream();
            bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, st);

            var resizedBitmap = Bitmap.CreateScaledBitmap(bitmap, width, height, true);

            var modelInputSize = FloatSize * height * width * PixelSize;
            var byteBuffer = ByteBuffer.AllocateDirect(modelInputSize);
            byteBuffer.Order(ByteOrder.NativeOrder());

            var pixels = new int[width * height];
            resizedBitmap.GetPixels(pixels, 0, resizedBitmap.Width, 0, 0, resizedBitmap.Width, resizedBitmap.Height);

            var pixel = 0;

            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    var pixelVal = pixels[pixel++];

                    //byteBuffer.PutFloat(pixelVal >> 16 & 0xFF);
                    //byteBuffer.PutFloat(pixelVal >> 8 & 0xFF);
                    //byteBuffer.PutFloat(pixelVal & 0xFF);
                    byteBuffer.PutFloat((pixelVal >> 16 & 0xFF) / 255.0f); // R
                    byteBuffer.PutFloat((pixelVal >> 8 & 0xFF) / 255.0f);  // G
                    byteBuffer.PutFloat((pixelVal & 0xFF) / 255.0f);       // B
                }
            }

            bitmap.Recycle();

            return byteBuffer;
        }
    }
}