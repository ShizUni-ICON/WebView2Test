using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;

namespace WebView2Test
{
	public partial class Form1 : Form
	{
		readonly CountdownEvent condition = new CountdownEvent(1);
		string dir = Directory.GetCurrentDirectory().Replace("\\", "/");

		public Form1()
		{
			InitializeComponent();
			InitializeAsync();
		}

		async void InitializeAsync()
		{
			await webView21.EnsureCoreWebView2Async(null);
			webView21.CoreWebView2.Navigate("file:///" + dir + "/htdocs/index.html");
			webView21.CoreWebView2.NavigationCompleted += webView2_NavigationCompleted;
			webView21.CoreWebView2.WebMessageReceived += MessageReceived;
		}

		private void MessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs args)
		{
			string text = args.TryGetWebMessageAsString();
			File.WriteAllText(dir + "\\savedata", text);
			MessageBox.Show("セーブしました。");
		}

		private void webView2_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
		{
			//読み込み成功なら処理続行
			if (e.IsSuccess) {
				condition.Signal();
				Thread.Sleep(1);
				condition.Reset();
				string fileName = dir + "\\savedata";
				if (File.Exists(fileName)) {
					StreamReader sr = new StreamReader(fileName, Encoding.GetEncoding("UTF-8"));
					string str = sr.ReadToEnd();
					sr.Close();
					webView21.CoreWebView2.PostWebMessageAsString(str);
				}
			}
		}
	}
}