using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Diplom2.Droid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tesseract;
using Tesseract.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(TesseractHelper))]
namespace Diplom2.Droid
{
    public class TesseractHelper:ITesseractHelper
    {
        public async Task<string> Main(byte[] imageData,bool Language, CancellationToken token)
        {
            string Output = "";
            try
            {
                ITesseractApi api = new TesseractApi(Android.App.Application.Context, AssetsDeployment.OncePerInitialization);
                await Task.Run(async () =>
                {
                    if (await api.Init("eng+rus"))
                    {
                        //var bitmap = BitmapFactory.DecodeStream(stream);
                        var bitmap_ = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);

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
                        bitmap = bitmap.Copy(Bitmap.Config.Argb8888, true);
                        bitmap = ResizeBitmap(bitmap);
                        bitmap = ToGrayscale(bitmap);
                        var st = new MemoryStream();
                        bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, st);
                        bool success = await api.SetImage(st.ToArray());
                        token.ThrowIfCancellationRequested();
                        if (success)
                        {
                            Output = api.Text;
                            if (Output.Contains("...")|| Output.Contains(",,,")|| Output.Contains("///"))
                            {
                                if (Language)
                                {
                                    Output = "Unable to recognize text";
                                }
                                else
                                {
                                    Output = "Не удалось распознать текст";
                                }
                            }
                            else
                            {
                                Output = Output.Trim('_');
                            }
                        }
                        else
                        {
                            if (Language)
                            {
                                Output = "Unable to recognize text";
                            }
                            else
                            {
                                Output = "Не удалось распознать текст";
                            }
                        }
                    }
                    else
                    {
                        if (Language)
                        {
                            Output = "Initialization error";
                        }
                        else
                        {
                            Output = "Ошибка инициализации";
                        }

                    }
                }, token);
                return Output;
            }
            catch(System.OperationCanceledException ex)
            {
                return "";
            }
            catch
            {
                if (Language)
                {
                    return "Something went wrong";
                }
                else
                {
                    return "Что-то пошло не так";
                }

            }

        }
        public Bitmap ResizeBitmap(Bitmap original, int maxWidth = 1024, int maxHeight = 1024)
        {
            int width = original.Width;
            int height = original.Height;
            float ratio = Math.Min((float)maxWidth / width, (float)maxHeight / height);
            int newWidth = (int)(width * ratio);
            int newHeight = (int)(height * ratio);
            return Bitmap.CreateScaledBitmap(original, newWidth, newHeight, true);
        }
        public Bitmap ToGrayscale(Bitmap original)
        {
            Bitmap grayscale = Bitmap.CreateBitmap(original.Width, original.Height, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(grayscale);
            Paint paint = new Paint();
            ColorMatrix cm = new ColorMatrix();
            cm.SetSaturation(0);
            ColorMatrixColorFilter f = new ColorMatrixColorFilter(cm);
            paint.SetColorFilter(f);
            canvas.DrawBitmap(original, 0, 0, paint);
            return grayscale;
        }
    }
}