using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Microsoft.VisualBasic;
//==============================================================================================================
//this application is used to download ts files from several web sites
//==============================================================================================================
//history:
//	2017.08.01: version 1.0.0.5: add an inputbox to ask people what's password of the remote database, actually for putting the whole source code onto github.com with some extent of safety
//	2017.07.31: version 1.0.0.4: modify the function getM3u8Path to be able to accept a new kind of symbol "307721_dashinit"
//	2017.07.24: version 1.0.0.3: add mimized box
//	2017.07.21: version 1.0.0.2: alpha version good to try
//	2017.07.18: version 1.0.0.1: initial version

namespace xTsGetter
{
	public partial class frmMain : Form
	{
		public frmMain()
		{
			InitializeComponent();
		}

		public class TsInfo
		{
			public int id { set; get; }
			public string WebUrlPrefix { set; get; }
			public int total { set; get; }
			public string filename { set; get; }
		}
		
		private WebBrowser IE2UrlBrowse = new WebBrowser();//webbrowser object to browse url
		private WebBrowser IE2JavyNow = new WebBrowser();//a webbrowser object dedicated fro javynow website
		private string website = @"https://inordinate-formatio.000webhostapp.com/dbflv/";//string indicate the base url of the web site
		private int curID = 0;//current id of table tblfrmflv
		private string curURL = "";//curent working url
		private TsInfo tsinfo = new TsInfo();//info about downloading ts segments
		private int curTsIdx = 0;//while downloading ts files, to indicate the index of ts files that currently downloading
		private string dbpwd = "";

		//add urls to database, which is in 000webhost server.
		private void btnAdd_Click(object sender, EventArgs e)
		{
			string res = "";
			//1.return if couldn't get the database password
			if (!getDbPwd()) {
				lblStatus.Text = "please enter password of the remote database";
				return;
			}
			//2.split the input textbox into array
			string[] arrUrl = txtURLs.Text.Split(new char[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);
			//3.call function to put its elements into database.
			for (int i = 0; i < arrUrl.Length; i++ ) {
				lblStatus.Text = "adding " + arrUrl[i];
				Application.DoEvents();
				res = GetIt(@website + @"insertURL.php?url=" + Uri.EscapeDataString(arrUrl[i]) + "&dbpwd=" + dbpwd);
				if (res != "done") {
					lblStatus.Text = "data error: " + arrUrl[i];
					return;
				}
			}
			lblStatus.Text = "all urls added";
			txtURLs.Clear();
		}
		
		//event hanlder when the form is just loaded
		private void frmMain_Load(object sender, EventArgs e)
		{
			//add documentcompleted event hanlder to IE2UrlBrowse and IE2JavyNow
			IE2UrlBrowse.ScriptErrorsSuppressed = true;
			IE2UrlBrowse.DocumentCompleted += IE2UrlBrowse_DocumentCompleted;
			IE2JavyNow.ScriptErrorsSuppressed = true;
			IE2JavyNow.DocumentCompleted += IE2JavyNow_DocumentCompleted;
		}

		//event hanlder of DocumentCompleted of IE2UrlBrowse
		void IE2JavyNow_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			string[] arrSrc = ((WebBrowser)sender).Document.Body.InnerHtml.Split(new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
			//if the video isn't a javynow one, just skip and move to next one, there are much more to be downloaded.
			if (arrSrc.Length == 1) {
				GetIt(@website + @"delRec.php?tblname=flv&id=" + curID.ToString() + "&dbpwd=" + dbpwd);
				nextURLDecode();
				return;
			}
			string combinedPath = getM3u8Path(arrSrc, e.Url.ToString());
			string m3u8Data = GetIt(combinedPath);
			if (m3u8Data != "") {
				string[] arrM3u8 = m3u8Data.Split(new char[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
				Random rnd = new Random();
				string filename = rnd.Next(65535).ToString("X");
				if (filename.Length == 1) filename = "000" + filename;
				if (filename.Length == 2) filename = "00" + filename;
				if (filename.Length == 3) filename = "0" + filename;
				string res = GetIt(@website + @"insertTsData.php?url=" + Uri.EscapeDataString(getHttpFolder(combinedPath)) + "&filename=" + filename + "&total=" + ((arrM3u8.Length - 5) / 2).ToString() + "&source=" + Uri.EscapeDataString(curURL) +"&dbpwd=" + dbpwd);
				if (res == "done") {
					//ts m3u8 file got, that's fine to delete
					GetIt(@website + @"delRec.php?tblname=flv&id=" + curID.ToString() + "&dbpwd=" + dbpwd);
					nextURLDecode();
				}
			}
		}

		//event hanlder of DocumentCompleted of IE2UrlBrowse
		void IE2UrlBrowse_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			WebBrowser objWB = (WebBrowser)sender;
			HtmlElement eleA = objWB.Document.Body;
			HtmlElement eleB = objWB.Document.Body;
			string s = e.Url.ToString();
			string iFrameSrc = "";
			//case 1: nukistream, poyopara
			if ((s.IndexOf("nukistream") > 0) || (s.IndexOf("poyopara") > 0) || (s.IndexOf("iqoo") > 0) || (s.IndexOf("hikaritube") > 0)) {
				//case 1.1: javynow embedded
				eleA = objWB.Document.All["player"];
				if (eleA != null) {
					if (eleA.InnerHtml.IndexOf("javynow") > 0) {
						eleB = eleA.Children[0].Children[0];
						iFrameSrc = eleB.GetAttribute("src").ToString();
						//browse the target source
						IE2JavyNow.Navigate(iFrameSrc);
						return;
					}
				}
			}
		}
		
		//event hanlder of button btnDecode, which is used to load and decoding the url to real url of ts files that can be downloaded directly
		private void btnDecode_Click(object sender, EventArgs e)
		{
			//1.return if couldn't get the database password
			if (!getDbPwd()) {
				lblStatus.Text = "please enter password of the remote database";
				return;
			}
			nextURLDecode();
		}

		//get next record which is not decoded yet from remote database
		private void nextURLDecode()
		{
			//query first row of the record in queue and proceed
			string[] tmpArr = GetIt(@website + @"getQueueData.php?dbpwd=" + dbpwd).Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries);
			if (tmpArr.Length == 2) {
				curID = Convert.ToInt32(tmpArr[0]);
				curURL = tmpArr[1];
				lblStatus.Text = "Decoding, id=" + curID.ToString();
				Application.DoEvents();
				IE2UrlBrowse.Navigate(curURL);
			} else lblStatus.Text = "no more url to be decoded!!";
		}

		//simple http access, sometimes the web access is just for getting content of a text file, or even jus string, no need to use the webbrowser object to do so, make ie easier.
		private string GetIt(string url)
		{
			string ret = "";
			//read the content from url
			try { 
				WebClient client = new WebClient();
				Stream stream = client.OpenRead(url);
				StreamReader reader = new StreamReader(stream);
				ret = reader.ReadToEnd();
			}
			catch (Exception ex) {
				lblStatus.Text = ex.Message + ", id=" + curID.ToString();
			}
			return ret;
		}

		//a function to assemble url from an string array
		private string getM3u8Path(string[] arrIn, string javynowUrl)
		{
			string ret = @"http://";
			string states = "alabama,alaska,arizona,arkansas,atlanta,california,colorado,connecticut,delaware,florida,georgia,hawaii,idaho,illinois,indiana,iowa,kansas,kentucky,louisiana,maine,maryland,massachusetts,michigan,minnesota,mississippi,missouri,montana,nebraska,nevada,ohio,oklahoma,oregon,pennsylvania,tennessee,texas,utah,vermont,virginia,washington,wisconsin,wyoming";
			int passes = 0;
			//==section 1: server name==
			for (int i = 0; i < arrIn.Length; i++) {
				if (states.IndexOf(arrIn[i]) != -1) {
					ret = ret + arrIn[i] + @".javynow.com/files/";
					passes++;
					break;
				}
			}
			//==section 2: folder/file name - case 1: use 480P keyword==
			for (int i = 0; i < arrIn.Length; i++) {
				if (arrIn[i] == "") continue;
				if (arrIn[i].IndexOf("480P") != -1) {
					ret = ret + arrIn[i] + @"/" + arrIn[i] + ".m3u8";
					passes++;
					break;
				}
			}
			if (passes == 2) return ret;
			//==section 2: folder/file name - case 2: from xvideos==
			if (arrIn.Contains("xvideos")) {
				//search for element which contains 'com_'
				for (int i = 0; i < arrIn.Length; i++) {
					if (arrIn[i].IndexOf("com_") != -1) {
						ret = ret + "xvideos." + arrIn[i] + @"/" + "xvideos." + arrIn[i] + ".m3u8";
						passes++;
						break;
					}
				}
			}
			if (passes == 2) return ret;
			//==section 2: folder/file name - case 3: just a number after the logo keyword, for ex: 3134924==
			//2017.07.31 there is one more way to represents the target, like this one "307721_dashinit". modify the function to try if the characters before "_" is a number
			for (int i = 0; i < arrIn.Length; i++) {
				if (arrIn[i] == "") continue;
				long n;
				string[] tmpData = arrIn[i].Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
				//bool isNumeric = long.TryParse(arrIn[i], out n);
				bool isNumeric = long.TryParse(tmpData[0], out n);
				if (isNumeric) {
					//if the number is part of Url, that's not the answer
					if (javynowUrl.IndexOf(arrIn[i]) != -1) continue;
					ret = ret + arrIn[i] + @"/" + arrIn[i] + ".m3u8";
					passes++;
					break;
				}
			}
			if (passes == 2) return ret;
			return "";
		}

		//a url might contains its browsing filename, what if I need the http address and folder name only
		private string getHttpFolder(string url)
		{
			string ret = "";
			url = url + " ";
			string[] tmpArr = url.Split(new char[] { '/' });
			for (int i = 0; i < tmpArr.Length - 1; i++) ret = ret + tmpArr[i] + "/";
			return ret;
		}

		//when user clicks the download button
		private void btnDownload_Click(object sender, EventArgs e)
		{
			if (!getDbPwd()) {
				lblStatus.Text = "please enter password of the remote database";
				return;
			}
			nextDownloadInfo();
		}

		//get next download info, and start downloading
		private void nextDownloadInfo()
		{
			string[] tmpArr = GetIt(@website + "getTsData.php?dbpwd=" + dbpwd).Split(new char[] {'~'}, StringSplitOptions.RemoveEmptyEntries);
			if (tmpArr.Length == 4) { 
				tsinfo.id = Convert.ToInt32(tmpArr[0]);
				tsinfo.WebUrlPrefix = tmpArr[1];
				tsinfo.filename = tmpArr[2];
				tsinfo.total = Convert.ToInt32(tmpArr[3]);
				curTsIdx = 0;
				nextDownloadTsFile();
			} else lblStatus.Text = "no more files to be downloaded.";
		}

		private void nextDownloadTsFile() {
			//move on next ts file if this one is done
			if (curTsIdx == tsinfo.total) {
				lblStatus.Text = "done downloading " + tsinfo.filename + ".ts";
				//create a bat file and write commands to it for putting all ts files as a single one.
				string text = @"for /L %%p in (1, 1, " + (curTsIdx - 1) + ") do copy /b " + txtDest.Text + tsinfo.filename + ".ts+" + txtDest.Text + tsinfo.filename + "_%%p.ts " + txtDest.Text + tsinfo.filename + ".ts /b" + "\r\n" + "del " + txtDest.Text + tsinfo.filename +"_*.ts";
				//create a specific batch file for combning these ts files.
				System.IO.File.WriteAllText(@txtDest.Text + "join.bat", text);
				string theCMD = @"/C " + txtDest.Text + "join.bat";
				System.Diagnostics.Process.Start("cmd.exe", theCMD);
				lblStatus.Text = "done combining " + tsinfo.filename + ".ts";
				GetIt(@website + @"delRec.php?tblname=ts&id=" + tsinfo.id + "&dbpwd=" + dbpwd);
				nextDownloadInfo();
			} else {
				if (curTsIdx == 0) DownloadFile(tsinfo.WebUrlPrefix + "stream_" + curTsIdx + ".ts", txtDest.Text + tsinfo.filename + ".ts"); else DownloadFile(tsinfo.WebUrlPrefix + "stream_" + curTsIdx + ".ts", txtDest.Text + tsinfo.filename + "_" + curTsIdx.ToString() + ".ts");
				curTsIdx++;
				if (!chkStop.Checked) nextDownloadTsFile();
			}
		}

		//function to download a certain file from remote source url to a local destination path
		private bool DownloadFile(string sSourceURL, string sDestinationPath)
		{
			DateTime tStart = DateTime.Now;
			DateTime tNow = DateTime.Now;
			string[] arrPath = sDestinationPath.Split('\\');
			bool completed = false;
			TimeSpan TS = DateTime.Now - DateTime.Now;
			long iFileSize = 0;
			int iBufferSize = 1024;
			iBufferSize *= 100;
			long iExistLen = 0;
			System.IO.FileStream saveFileStream;
			if (System.IO.File.Exists(sDestinationPath)) return true;
			saveFileStream = new System.IO.FileStream(sDestinationPath, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite);
			System.Net.HttpWebRequest hwRq;
			System.Net.HttpWebResponse hwRes;
			hwRq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(sSourceURL);
			hwRq.AddRange((int)iExistLen);
			System.IO.Stream smRespStream = null;
			try {
				hwRes = (System.Net.HttpWebResponse)hwRq.GetResponse();
				smRespStream = hwRes.GetResponseStream();
				iFileSize = hwRes.ContentLength;
			}
			catch (Exception) {
				// XVideo會移動檔案的位置, 所以如果那個檔案已經brwose不到了, 就直接結束跳到下一個
				return true;
			}
			double bytesIn = double.Parse(iExistLen.ToString());
			double totalBytes = double.Parse(iFileSize.ToString()) + bytesIn;
			int iByteSize;
			double thisDown = 0;
			byte[] downBuffer = new byte[iBufferSize];
			tStart = DateTime.Now;
			try {
				while ((iByteSize = smRespStream.Read(downBuffer, 0, downBuffer.Length)) > 0) {
					saveFileStream.Write(downBuffer, 0, iByteSize);
					tNow = DateTime.Now;
					TS = tNow - tStart;
					bytesIn = bytesIn + iByteSize;
					thisDown = thisDown + iByteSize;
					double spd = Math.Truncate(thisDown / TS.TotalSeconds / 1024);
					if (TS.TotalSeconds == 0.0) spd = 0.0;
					double percentage = bytesIn / totalBytes * 100;
					lblStatus.Text = spd.ToString() + " KB/s..." + remain(totalBytes, bytesIn, spd) + "..." + Math.Round(percentage, 2).ToString() + "%... (id=" + tsinfo.id.ToString() + ", file=" + arrPath[arrPath.Length - 1] + ", " + (curTsIdx + 1).ToString() + "/" + tsinfo.total.ToString() +" )";
					this.Text = Math.Round(percentage, 2).ToString() + "% - " + spd.ToString() + " KB/s";
					Application.DoEvents();
				}
			}
			catch (Exception) {
				return true;
			}
			smRespStream.Close();
			saveFileStream.Close();
			return completed;
		}

		//function to return how much more time is required for download whole file
		private string remain(double totalBytes, double bytesIn, double spd)
		{
			string ret = "00:00:00";
			if (spd > 0.0) {
				ret = "";
				double dEst = Math.Truncate((totalBytes - bytesIn) / (spd * 1024));
				int iEst = Convert.ToInt32(dEst);
				ret = Math.Floor((double)iEst / 3600).ToString();
				iEst = iEst % 3600;
				ret = ret + ":" + Math.Floor((double)iEst / 60).ToString().PadLeft(2, '0');
				iEst = iEst % 60;
				ret = ret + ":" + iEst.ToString().PadLeft(2, '0');
			}
			return ret;
		}

		//function to get password of the remote database, return true if the password is set, otherwise return false;
		private bool getDbPwd()
		{
			bool ret = false;
			if (dbpwd != "") return true;
			else {
				string input = Interaction.InputBox("Please enter the password of remote database", "DB password input", "", -1, -1);
				input = input.Trim();
				if (input != "") {
					dbpwd = input;
					ret = true;
				}
			}
			return ret;
		}
	}
}
