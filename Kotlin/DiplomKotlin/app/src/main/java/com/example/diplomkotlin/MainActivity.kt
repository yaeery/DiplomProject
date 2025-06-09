package com.example.diplomkotlin

import android.app.Activity
import android.content.Intent
import android.graphics.Color
import android.graphics.drawable.ColorDrawable
import android.graphics.drawable.Drawable
import android.graphics.drawable.GradientDrawable
import android.net.Uri
import android.os.Bundle
import android.provider.MediaStore
import android.view.View
import android.view.WindowManager
import android.widget.Button
import android.widget.CheckBox
import android.widget.FrameLayout
import android.widget.GridLayout
import android.widget.ImageButton
import android.widget.ImageView
import android.widget.LinearLayout
import android.widget.ResourceCursorAdapter
import android.widget.TextView
import android.widget.VideoView
import androidx.activity.enableEdgeToEdge
import androidx.appcompat.app.AppCompatActivity
import androidx.camera.view.PreviewView
import androidx.core.content.ContextCompat
import androidx.core.graphics.drawable.toDrawable
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat
import androidx.core.view.isVisible
import com.example.diplomkotlin.R.color.black
import com.google.android.material.button.MaterialButton
import android.Manifest
import android.content.pm.PackageManager
import androidx.camera.core.CameraSelector
import androidx.camera.core.Preview
import androidx.camera.lifecycle.ProcessCameraProvider
import androidx.core.app.ActivityCompat
import java.util.concurrent.ExecutorService
import java.util.concurrent.Executors

class MainActivity : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContentView(R.layout.activity_main)
        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main)) { v, insets ->
            val systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars())
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom)
            insets
        }
    }
    private var Is_Black_Theme:Boolean = false


    public fun ChangeInterface(IsMicroPageVis: Boolean, IsTextPageVis: Boolean, IsMoneyPageVis: Boolean, IsObrazPageVis: Boolean, IsImageOpisPageVis: Boolean, IsSubtitlePageVis: Boolean, IsSettingPageVis: Boolean,) {
        val BigMicroLayout: LinearLayout = findViewById(R.id.BigMicroLayout)
        BigMicroLayout.isVisible = IsMicroPageVis;
        val SettingGrid:GridLayout = findViewById(R.id.GridSetting)
        SettingGrid.isVisible = IsSettingPageVis;
        val GridImageOpis: GridLayout = findViewById(R.id.GridImageOpis)
        if(GridImageOpis.isVisible==true && IsImageOpisPageVis==false){
            val ImageForOpis:ImageView = findViewById(R.id.ImageForOpis)
            ImageForOpis.setImageURI(null)
        }
        GridImageOpis.isVisible = IsImageOpisPageVis
        val GridSubtitles: GridLayout = findViewById(R.id.GridSubtitles)
        if(GridSubtitles.isVisible==true && IsSubtitlePageVis==false){
            val Videoplayer:VideoView = findViewById(R.id.Videoplayer)
            Videoplayer.setVideoURI(null)
            Videoplayer.background = GridSubtitles.background
        }
        GridSubtitles.isVisible = IsSubtitlePageVis
        val previewView:PreviewView = findViewById(R.id.previewView)
        if(IsTextPageVis||IsMoneyPageVis||IsObrazPageVis==true){
            previewView.isVisible = true
            startCamera()
        }
        else{
            previewView.isVisible = false
        }
    }
    public fun MicroSmall_Click(view: View) {
        ChangeInterface(true,false,false,false,false,false,false)
    }
    public fun TextButton_Click(view: View) {
        ChangeInterface(false,true,false,false,false,false,false)
    }
    public fun MoneyButton_Click(view: View) {
        ChangeInterface(false,false,true,false,false,false,false)
    }
    public fun RecognizeObrazButton_Click(view: View) {
        ChangeInterface(false,false,false,true,false,false,false)
    }
    public fun ImageOpisButton_Click(view: View) {
        ChangeInterface(false,false,false,false,true,false,false)
    }
    public fun SubtitlesButton_Click(view: View) {
        ChangeInterface(false,false,false,false,false,true,false)
    }
    public fun SettingButton_Click(view: View) {
        ChangeInterface(false,false,false,false,false,false,true)
    }
    public fun ChooseImageForOpis(view: View) {
        val pickImageIntent = Intent(Intent.ACTION_PICK,MediaStore.Images.Media.EXTERNAL_CONTENT_URI)
        startActivityForResult(pickImageIntent,100)
    }
    public fun ChooseVideo(view: View) {
        val pickImageIntent = Intent(Intent.ACTION_PICK,MediaStore.Video.Media.EXTERNAL_CONTENT_URI)
        startActivityForResult(pickImageIntent,101)
    }
    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?){
        super.onActivityResult(requestCode,resultCode,data)
        if(requestCode==100 && resultCode==Activity.RESULT_OK && data!=null) {
            GetResultImageaOpis(data.data)
        }
        else if(requestCode==101 && resultCode==Activity.RESULT_OK && data!=null){
            CreateSubtitles(data.data)
        }
    }
    private fun GetResultImageaOpis(data: Uri?){
        val ImageForOpis:ImageView = findViewById(R.id.ImageForOpis)
        ImageForOpis.setImageURI(data)
    }
    private fun CreateSubtitles(data: Uri?){
        val Videoplayer: VideoView = findViewById(R.id.Videoplayer)
        Videoplayer.background = null
        Videoplayer.setVideoURI(data)
        Videoplayer.start()
    }
    public fun Clicked_ChengeTheme(view: View){
        ChangeTheme()
    }
    private fun ChangeTheme(){
        var DarkThemeCheck: CheckBox = findViewById(R.id.DarkThemeCheck)
        Is_Black_Theme = DarkThemeCheck.isChecked
        var Dop_Simbol: String
        var Color: Int
        var DopColor: Int
        if(Is_Black_Theme){
            Color = 0xFF000000.toInt()
            DopColor = 0xFF049bcc.toInt()
            Dop_Simbol = "d"
            window.addFlags(WindowManager.LayoutParams.FLAG_DRAWS_SYSTEM_BAR_BACKGROUNDS)
            window.statusBarColor = ContextCompat.getColor(this, R.color.cyan)
        }
        else{
            Color = 0xFFFFFFFF.toInt()
            DopColor = 0xFFBE01BE.toInt()
            Dop_Simbol = ""
            window.addFlags(WindowManager.LayoutParams.FLAG_DRAWS_SYSTEM_BAR_BACKGROUNDS)
            window.statusBarColor = ContextCompat.getColor(this, R.color.purple)
        }

        val BigMicroLayout: LinearLayout = findViewById(R.id.BigMicroLayout)
        BigMicroLayout.setBackgroundColor(Color)
        val BigMicroFrame: FrameLayout = findViewById(R.id.BigMicroFrame)
        BigMicroFrame.background = if(Is_Black_Theme)  ContextCompat.getDrawable(this,R.drawable.borderd) else ContextCompat.getDrawable(this,R.drawable.border)
        val MicroImage: ImageView = findViewById(R.id.MicroImage)
        MicroImage.setImageResource(this.resources.getIdentifier("micro"+Dop_Simbol,"drawable",this.packageName))


        val StackButton: LinearLayout = findViewById(R.id.StackButton)
        StackButton.setBackgroundColor(DopColor)
        val GridSetting: GridLayout = findViewById(R.id.GridSetting)
        GridSetting.setBackgroundColor(Color)


        val ButtonLang: Button = findViewById(R.id.ButtonLang)
        ButtonLang.setTextColor(DopColor)
        ButtonLang.background = if(Is_Black_Theme)  ContextCompat.getDrawable(this,R.drawable.buttonstyleblack) else ContextCompat.getDrawable(this,R.drawable.buttonstyle)

        val ImageLang: ImageView = findViewById(R.id.ImageLang)
        ImageLang.setImageResource(this.resources.getIdentifier("teldey"+Dop_Simbol,"drawable",this.packageName))
        val DarkThemeImg: ImageView = findViewById(R.id.DarkThemeImg)
        DarkThemeImg.setImageResource(this.resources.getIdentifier("theme"+Dop_Simbol,"drawable",this.packageName))
        var SpeackTouch: ImageView = findViewById(R.id.SpeackTouch)
        SpeackTouch.setImageResource(this.resources.getIdentifier("teldey"+Dop_Simbol,"drawable",this.packageName))
        var VibrationImg: ImageView = findViewById(R.id.VibrationImg)
        VibrationImg.setImageResource(this.resources.getIdentifier("hear"+Dop_Simbol,"drawable",this.packageName))
        var MicroofImg: ImageView = findViewById(R.id.MicroofImg)
        MicroofImg.setImageResource(this.resources.getIdentifier("offmicro"+Dop_Simbol,"drawable",this.packageName))
        var OffCameraImg: ImageView = findViewById(R.id.OffCameraImg)
        OffCameraImg.setImageResource(this.resources.getIdentifier("offcamera"+Dop_Simbol,"drawable",this.packageName))
        var BlocVisionImg: ImageView = findViewById(R.id.BlocVisionImg)
        BlocVisionImg.setImageResource(this.resources.getIdentifier("blockvision"+Dop_Simbol,"drawable",this.packageName))
        var BlocListnImg: ImageView = findViewById(R.id.BlocListnImg)
        BlocListnImg.setImageResource(this.resources.getIdentifier("blocklistn"+Dop_Simbol,"drawable",this.packageName))
        var NameImg: ImageView = findViewById(R.id.NameImg)
        NameImg.setImageResource(this.resources.getIdentifier("nameassist"+Dop_Simbol,"drawable",this.packageName))

        var MicroPage: ImageButton = findViewById(R.id.MicroPage)
        MicroPage.setImageResource(this.resources.getIdentifier("microsmall"+Dop_Simbol,"drawable",this.packageName))
        MicroPage.background = if(Is_Black_Theme)  ContextCompat.getDrawable(this,R.drawable.blackborder) else ContextCompat.getDrawable(this,R.drawable.whiteborder)
        var TextPage: ImageButton = findViewById(R.id.TextPage)
        TextPage.setImageResource(this.resources.getIdentifier("text"+Dop_Simbol,"drawable",this.packageName))
        TextPage.background = if(Is_Black_Theme)  ContextCompat.getDrawable(this,R.drawable.blackborder) else ContextCompat.getDrawable(this,R.drawable.whiteborder)
        var MoneyPage: ImageButton = findViewById(R.id.MoneyPage)
        MoneyPage.setImageResource(this.resources.getIdentifier("money"+Dop_Simbol,"drawable",this.packageName))
        MoneyPage.background = if(Is_Black_Theme)  ContextCompat.getDrawable(this,R.drawable.blackborder) else ContextCompat.getDrawable(this,R.drawable.whiteborder)
        var ObrazPage: ImageButton = findViewById(R.id.ObrazPage)
        ObrazPage.setImageResource(this.resources.getIdentifier("obraz"+Dop_Simbol,"drawable",this.packageName))
        ObrazPage.background = if(Is_Black_Theme)  ContextCompat.getDrawable(this,R.drawable.blackborder) else ContextCompat.getDrawable(this,R.drawable.whiteborder)
        var ImageOpisPage: ImageButton = findViewById(R.id.ImageOpisPage)
        ImageOpisPage.setImageResource(this.resources.getIdentifier("image"+Dop_Simbol,"drawable",this.packageName))
        ImageOpisPage.background = if(Is_Black_Theme)  ContextCompat.getDrawable(this,R.drawable.blackborder) else ContextCompat.getDrawable(this,R.drawable.whiteborder)
        var SubtitlesPage: ImageButton = findViewById(R.id.SubtitlesPage)
        SubtitlesPage.setImageResource(this.resources.getIdentifier("subtitrs"+Dop_Simbol,"drawable",this.packageName))
        SubtitlesPage.background = if(Is_Black_Theme)  ContextCompat.getDrawable(this,R.drawable.blackborder) else ContextCompat.getDrawable(this,R.drawable.whiteborder)
        var SettingPage: ImageButton = findViewById(R.id.SettingPage)
        SettingPage.setImageResource(this.resources.getIdentifier("setting"+Dop_Simbol,"drawable",this.packageName))
        SettingPage.background = if(Is_Black_Theme)  ContextCompat.getDrawable(this,R.drawable.blackborder) else ContextCompat.getDrawable(this,R.drawable.whiteborder)

        var TextLang: TextView = findViewById(R.id.TextLang)
        TextLang.setTextColor(DopColor)
        var DarkThemeText: TextView = findViewById(R.id.DarkThemeText)
        DarkThemeText.setTextColor(DopColor)
        var TextSpeackTouch: TextView = findViewById(R.id.TextSpeackTouch)
        TextSpeackTouch.setTextColor(DopColor)
        var TextVibrate: TextView = findViewById(R.id.TextVibrate)
        TextVibrate.setTextColor(DopColor)
        var OffMicroText: TextView = findViewById(R.id.OffMicroText)
        OffMicroText.setTextColor(DopColor)
        var OffCameraText: TextView = findViewById(R.id.OffCameraText)
        OffCameraText.setTextColor(DopColor)
        var BlocVisionText: TextView = findViewById(R.id.BlocVisionText)
        BlocVisionText.setTextColor(DopColor)
        var BlocListnText: TextView = findViewById(R.id.BlocListnText)
        BlocListnText.setTextColor(DopColor)
        var NameAssistText: TextView = findViewById(R.id.NameAssistText)
        NameAssistText.setTextColor(DopColor)

    }


    private fun startCamera() {
        var previewView: PreviewView = findViewById(R.id.previewView)
        val cameraProviderFuture = ProcessCameraProvider.getInstance(this)

        cameraProviderFuture.addListener({
            val cameraProvider = cameraProviderFuture.get()
            val preview = Preview.Builder().build().also {
                it.setSurfaceProvider(previewView.surfaceProvider)
            }

            val cameraSelector = CameraSelector.DEFAULT_BACK_CAMERA

            try {
                cameraProvider.unbindAll()
                cameraProvider.bindToLifecycle(this, cameraSelector, preview)
            } catch (exc: Exception) {
                exc.printStackTrace()
            }
        }, ContextCompat.getMainExecutor(this))
    }
}