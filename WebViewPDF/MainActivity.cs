using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Webkit;
using Java.Interop;
using ZXing.Mobile;
using Java.Lang;
using ZXing;
using ZXing.Common;
using Android.Graphics;
using ZXing.QrCode;

namespace WebViewPDF
{
    [Activity(Label = "WebViewPDF", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        WebView web_view;

        //const string html = @"
        //<html>
        //  <body>
        //    <p>Demo calling C# from JavaScript</p>
        //    <button type=""button"" onClick=""CSharp.ShowToast()"">Call C#</button>
        //  </body>
        //</html>";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            web_view = FindViewById<WebView>(Resource.Id.webview);
            web_view.Settings.JavaScriptEnabled = true;
            web_view.SetWebChromeClient(new WebChromeClient());
            web_view.AddJavascriptInterface(new MyJSInterface(this, Application), "CSharp");
            //WebView.SetWebContentsDebuggingEnabled(true);
            web_view.LoadUrl("file:///android_asset/Content/index.html");
            //web_view.LoadData(html, "text/html", null);

        }
    }

    class MyJSInterface : Java.Lang.Object
    {
        Context context;
        Application application;
        Activity activity;

        MobileBarcodeScanner scanner;

        public MyJSInterface(Activity activity, Application application)
        {
            this.context = activity;
            this.application = application;
            this.activity = activity;


            // Initialize the scanner first so we can track the current context
            MobileBarcodeScanner.Initialize(application);

            //Create a new instance of our Scanner
            scanner = new MobileBarcodeScanner();


        }

        [Export]
        [JavascriptInterface]
        public void ShowToast()
        {
            Toast.MakeText(context, "Hello from C#", ToastLength.Short).Show();
        }

        [Export]
        [JavascriptInterface]
        public async void ScanPDF()
        {
            var beachImage = new Xamarin.Forms.Image { Aspect = Xamarin.Forms.Aspect.AspectFit };
            beachImage.Source = Xamarin.Forms.ImageSource.FromFile("QR_Code_Test.jpg");

            //var logo = Resources.GetDrawable(Resource.Drawable.QR_Code_Test);

            string uriString = "file:///android_asset/QR_Code_Test.jpg";
            Bitmap bMap = BitmapFactory.DecodeFile(uriString);
            ReadBarcode();

            //Tell our scanner to activiytuse the default overlay
            scanner.UseCustomOverlay = false;

            //We can customize the top and bottom text of the default overlay
            scanner.TopText = "Hold the camera up to the barcode\nAbout 6 inches away";
            scanner.BottomText = "Wait for the barcode to automatically scan!";

            //Start scanning
            var result = await scanner.Scan();

            HandleScanResult(result);
        }

        void HandleScanResult(ZXing.Result result)
        {
            string msg = "";

            if (result != null && !string.IsNullOrEmpty(result.Text))
                msg = "Found Barcode: " + result.Text;
            else
                msg = "Scanning Canceled!";

            WebView web_view = activity.FindViewById<WebView>(Resource.Id.webview);

            var rtnString = string.Format("javascript:document.getElementById('txtContent').value = '{0}'", msg);

            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Kitkat)
            {
                //web_view.EvaluateJavascript(string.Format("javascript:zxinResult({0})", msg), null);
                //activity.RunOnUiThread(() =>
                //    web_view.EvaluateJavascript(rtnString, null)
                //);
                using (var h = new Handler(Looper.MainLooper))
                    h.Post(() => { web_view.EvaluateJavascript(rtnString, null); });
            }
            else
            {
                //web_view.LoadUrl(string.Format("javascript:zxinResult({0})", msg))
                //activity.RunOnUiThread(() =>
                //    web_view.LoadUrl(rtnString)
                //);

                using (var h = new Handler(Looper.MainLooper))
                    h.Post(() => { web_view.LoadUrl(rtnString); Console.WriteLine("complete"); });

            }



            //activity.RunOnUiThread(
            //    new Runnable() {
            //        public void run()
            //        {
            //            WebView web_view = activity.FindViewById<WebView>(Resource.Id.webview);
            //            web_view.EvaluateJavascript("alert('hi ryan');", null);
            //        }
            //    }
            //);
        }

        //Interesting method
        //public static string decodeQRImage(string path)
        //{
        //    Bitmap bMap = BitmapFactory.DecodeFile(path);
        //    string decoded = null;

        //    int[] intArray = new int[bMap.Width * bMap.Height];
        //    Java.IO.InputStream ist = getResources().openRawResource(R.drawable.balloons);
        //    byte[] data = new byte[ist.Available()];

        //    bMap.GetPixels(intArray, 0, bMap.Width, 0, 0, bMap.Width, bMap.Height);
        //    LuminanceSource source = new RGBLuminanceSource(data, bMap.Width, bMap.Height);
        //    BinaryBitmap bitmap = new BinaryBitmap(new HybridBinarizer(source));

        //    Reader reader = new QRCodeReader();

        //    ZXing.Result result = reader.decode(bitmap);
        //    decoded = result.Text;

        //    return decoded;
        //}

        private void ReadBarcode()
        {
            try
            {
                var imageBytes = GetImage("file:///android_asset/QR_Code_Test.jpg");
                var width = GetWidth("file:///android_asset/QR_Code_Test.jpg");
                var height = GetHeight("file:///android_asset/QR_Code_Test.jpg");
                LuminanceSource source = new ZXing.RGBLuminanceSource(imageBytes, (int)width, (int)height);
                HybridBinarizer hb = new HybridBinarizer(source);
                var a = hb.createBinarizer(source);
                BinaryBitmap bBitmap = new BinaryBitmap(a);
                BarcodeReader reader = new BarcodeReader();
                reader.Options.TryHarder = true;

                MultiFormatReader reader1 = new MultiFormatReader();
                try
                {
                    var r = reader1.decodeWithState(bBitmap);
                    var result = reader.Decode(source);
                    var result2 = reader.DecodeMultiple(source);
                    if (result != null)
                    {
                        return;
                    }
                    return;
                }
                catch (Java.Lang.Exception ex)
                {
                    //HandleError("ReadBarcode Error: ", ex.Message);
                }
            }
            catch (Java.Lang.Exception ex)
            {
                //HandleError("ReadBarcode Error: ", ex.Message);
            }
        }

        public uint GetWidth(string photoPath)
        {
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InPreferredConfig = Bitmap.Config.Argb8888;
            Bitmap bitmap = BitmapFactory.DecodeFile(photoPath, options);
            return bitmap.GetBitmapInfo().Width;
        }

        public uint GetHeight(string photoPath)
        {
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InPreferredConfig = Bitmap.Config.Argb8888;
            Bitmap bitmap = BitmapFactory.DecodeFile(photoPath, options);
            return bitmap.GetBitmapInfo().Height;
        }

        public byte[] GetImage(string photoPath)
        {
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InPreferredConfig = Bitmap.Config.Argb8888;
            Bitmap bitmap = BitmapFactory.DecodeFile(photoPath, options);

            var width = bitmap.GetBitmapInfo().Width;
            var height = bitmap.GetBitmapInfo().Height;
            int size = bitmap.RowBytes * (int)bitmap.GetBitmapInfo().Height;
            Java.Nio.ByteBuffer byteBuffer = Java.Nio.ByteBuffer.Allocate(size);
            bitmap.CopyPixelsToBuffer(byteBuffer);
            byte[] byteArray = new byte[byteBuffer.Remaining()];
            byteBuffer.Get(byteArray);
            return byteArray;
        }
    }
}

