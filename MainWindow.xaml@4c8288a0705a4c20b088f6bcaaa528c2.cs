using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Threading;
//using System.Windows.Media.Animation;
using System.IO;
//using System.Data.Linq;
using System.Data;
using System.Speech;
using System.Speech.Recognition;

namespace AHT_Buddy
{
   
 public struct MyAlarm
    {        
        private string cTime;
        private string aTime;
        private string sTime;

        public string shortTime
        {
            get { return sTime; }
            set { sTime = value; }
        }


        public string setAlarm
        {
            
            set { aTime = value; }
        }
        public string setTime
        {
            
            set { cTime = value; }
        }
        public string getAlarm
        {
            get { return aTime; }
        }
        public string getTime
        {
            get { return cTime; }
        }
        public bool IsTripped
        {
            get
            {
                if(cTime == aTime)
                {
                    return true;                    
                }
                else
                {
                    return false;
                }
            }
        }
        public bool IsArmed
        {
            get;set;     
        }
        public bool Done
        {
            get;set;
        }
    }
public struct SetRemedyForm
    {
        public string Email{get{return "Customer Email";} } 
        public string Ticket{get{return "Ticket Number";} }
        public string CustomerName { get { return "Customer Name"; } }
        public string AccountNumber{get{return "Account Number";} }
        public string AttemptNumber {get{return "Attempt Number";} }
        public string Contact{ get{return "Contact Number";} }
        public string Affected{get{return "Affected Device";} }
        public string ReportedIssue{get { return "Reported Issue";} }
        public string IdentifyAndResolve {get{return "Identify & Resolve";}}
        public string NextAction{get{return "Next Action";} }
        public string Notes{get{return "Notes";} }
    }


    public partial class MainWindow : Window
    {
        //SpeechRecognizer rec = new SpeechRecognizer();
        public string sEmail = "Customer Email";
        public string sTicket = "Ticket Number";
        public string sCx = "Customer Name";
        public string sAccount = "Account Number";
        public string sAttempt = "Attempt Number";
        public string sContact = "Contact Number";
        public string sAffected = "Affected Device";
        public string sIssue = "Reported Issue";
        public string sResolve = "Identify & Resolve";
        public string sNext = "Next Action";
        public string sCallNotes = "Notes";
        public string ContactNumberRaw, DateShort, curCombo;
        public string StrokeNormal = "#FF494949";
        public string StrokeHighlight = "#FF766C01";
        public string buttonColorGS1 = "#FF000000";
        public string buttonColorGS2 = "#FF000000";

        public string podRemove = "Clear POD Data?";
        public int todayPOD;

        

        public DataTable RemedyCodes = ConvertCSVtoDataTable(@"Remedy Codes Full.csv");
        public Dictionary<string, string> Abbrev = new Dictionary<string, string>();
        public bool bSpace;

        public MyAlarm Break1 = new MyAlarm();
        public MyAlarm Break2 = new MyAlarm();
        public MyAlarm Break3 = new MyAlarm();
        public MyAlarm Lunch = new MyAlarm();
        public SetRemedyForm AHTbuddy = new SetRemedyForm();

      
        public string sAlarmTime = "{0}" + ":" + "{1}" + ":00 " + "{2}";
        public string sTextBoxTime = "{0}" + ":" + "{1}" + " " + "{2}";
        public string sliderHH;
        public string sliderMM;
        public string sliderTT;
        public BitmapImage Coffee = new BitmapImage();
        public BitmapImage Hamburger = new BitmapImage();

        public byte curHour, curMinute;
        public DispatcherTimer myTimer = new DispatcherTimer();
        private int time = 900 ;


        public MainWindow()
        {
            InitializeComponent();

            //rec.SpeechRecognized += rec_SpeechRecognized;

            BreakIcon.Visibility = System.Windows.Visibility.Collapsed;
            
            Coffee.BeginInit();
            Coffee.UriSource = new Uri("Coffee.png", UriKind.Relative);
            Coffee.EndInit();
            Hamburger.BeginInit();
            Hamburger.UriSource = new Uri("Hamburger.png", UriKind.Relative);
            Hamburger.EndInit();
            
            //Start Clock - Set Current Time
            DispatcherTimer clock = new System.Windows.Threading.DispatcherTimer();
            clock.Tick += new EventHandler(DispatcherTimer_Tick);
            clock.Interval = new TimeSpan(0, 0, 1);
            clock.Start();

            
            
            
            //Set Current Date
            curDate.Content = DateTime.Now.ToString("dddd MMMM d, yyyy");

            tbCause.Content = "";
            tbProblem.Content = "";
            tbSolution.Content = "";
            
            ComboLoad(comboProblem, "tPC");
            Properties.Settings.Default.Upgrade();
            SaveData_Load(true);
            AutoReplace_Load();
            AlarmSet(); 

        }
        void rec_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            //Issue.Text = e.Result.Text;
        }
        private void MyTimerTick_Tick(object sender, EventArgs e)
        {

            if(time > 0)
            {


                if (time <= 10)
                {
                    time--;
                    breakTimer.Text = string.Format("00:0{0}:0{1}", time / 60, time % 60);
                }
                else
                {
                    if(time >= 660)
                    {
                        time--;
                        breakTimer.Text = string.Format("00:{0}:{1}", time / 60, time % 60);
                        if(time % 60 < 10)
                        {
                            breakTimer.Text = string.Format("00:{0}:0{1}", time / 60, time % 60);
                        }
                    }
                    else
                    {
                        time--;
                        breakTimer.Text = string.Format("00:0{0}:{1}", time / 60, time % 60);
                        if (time % 60 < 10)
                        {
                            breakTimer.Text = string.Format("00:0{0}:0{1}", time / 60, time % 60);
                        }
                    }


                    
                }
                
            }
            else
            {
                myTimer.Stop();
                MessageBox.Show("Timer Finished!");
            }




        }


        private void CloseExpanders()
        {
            if (NotesExpander.IsExpanded == true)
            {
                NotesExpander.IsExpanded = false;
            }           
           
            if (pw_Expander.IsExpanded == true)
            {
                pw_Expander.IsExpanded = false;
            }
            if (podExpander.IsExpanded == true)
            {
                podExpander.IsExpanded = false;
            }
            if(RemedyCodesExpander.IsExpanded == true)
            {
                RemedyCodesExpander.IsExpanded = false;
            }
        }
        public void YesNoBox(string mbMessage, string mbTitle, MessageBoxImage mbImage)
        {
            MessageBoxResult Result = MessageBox.Show(mbMessage, mbTitle, MessageBoxButton.OKCancel, mbImage);
            if (Result == MessageBoxResult.OK)
            {
                if (mbMessage == podRemove)
                {
                    podTechnicolor.Clear();
                    podArris.Clear();
                    podCisco.Clear();
                    podDory.Clear();
                    podSMC.Clear();
                }
                else if (mbMessage == "Reset All Alarms?")
                {
                    tbBreak1.Text = "12:00 AM";
                    tbBreak2.Text = "12:00 AM";
                    tbBreak3.Text = "12:00 AM";
                    tbLunch.Text = "12:00 AM";
                    Break1.IsArmed = false;
                    Break2.IsArmed = false;
                    Break3.IsArmed = false;
                    Lunch.IsArmed = false;
                  
                }
                else
                {
                    SaveData_Load(false);
                }

            }
        }
        public static DataTable ConvertCSVtoDataTable(string strFilePath)
        {
            StreamReader sr = new StreamReader(strFilePath);
            string[] headers = sr.ReadLine().Split(',');
            DataTable dt = new DataTable();
            foreach (string header in headers)
            {
                dt.Columns.Add(header);
            }
            while (!sr.EndOfStream)
            {
                string[] rows = sr.ReadLine().Split(',');
                DataRow dr = dt.NewRow();
                for (int i = 0; i < headers.Length; i++)
                {
                    dr[i] = rows[i];
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
        public void ChangeHighlight(Rectangle rect, bool Enter)
        {
            if (Enter == true)
            {
                rect.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(StrokeHighlight));
            }
            else
            {
                rect.Stroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString(StrokeNormal));
            }
        }
        public void GetPOD(String POD)
        {
            //Get Password of the Day
            if (POD == "")
            {
                return;
            }
            string DateString = DateTime.Now.ToString("MM/dd/yyyy");
            string DayNum = DateString.Substring(3, 2);
            if (cbPM.IsChecked == true)
            {
                DateTime NextDay = DateTime.Now;
                DayNum = NextDay.AddDays(1).ToString("MM/dd/yyyy");
                DayNum = DayNum.Substring(3, 2);
            }
            DayNum = DayNum + "-";
            string[] arrayPOD = POD.Split('\t', '\n');
            foreach (string p in arrayPOD)
            {
                var pos = Array.FindIndex(arrayPOD, row => row.Contains(DayNum));
                todayPOD = pos + 1;
            }
            
            Clipboard.SetText(arrayPOD.ElementAt(todayPOD));
            return;
        }

        private void AlarmSet()
        {
            if (sliderHour.Value >= 12)
            {
                if (sliderHour.Value != 12)
                {
                    lbHour.Content = sliderHour.Value - 12;
                }

                lbAMPM.Content = "PM";
            }
            else
            {
                lbHour.Content = sliderHour.Value;
                lbAMPM.Content = "AM";
            }
            if (sliderMinute.Value < 10)
            {
                lbMinute.Content = "0" + sliderMinute.Value;
            }
            else
            {
                lbMinute.Content = sliderMinute.Value;
            }
            byte ZeroAdd = byte.Parse(lbHour.Content.ToString());

            if (ZeroAdd < 10)
            {
                lbHour.Content = "0" + lbHour.Content.ToString();
            }
            
        }
        
        private void AutoReplace_Load()
        {
            //Dictionary >> Auto-replace 
            Abbrev.Add("wg", "Wireless gateway");
            Abbrev.Add("WG", "Wireless Gateway");
            Abbrev.Add("APT", "Re-Provision");
            Abbrev.Add("apt", "re-provision");
            Abbrev.Add("mdm", "modem");
            Abbrev.Add("rtr", "router");
            Abbrev.Add("FR", "Factory Restore");
            Abbrev.Add("PW", "Password");
            Abbrev.Add("pw", "password");
            Abbrev.Add("Cx", "Customer");
            Abbrev.Add("cx", "customer");
            Abbrev.Add("Wif", "Wi-Fi");
            Abbrev.Add("wif", "Wi-Fi");
            Abbrev.Add("want", "wants to");
            Abbrev.Add("ss", "SSID");
            Abbrev.Add("SS", "SSID");
            Abbrev.Add("hw", "hard wired");
            Abbrev.Add("HW", "Hard Wired");
            Abbrev.Add("interm", "intermittent");
            Abbrev.Add("ether", "Ethernet");
            Abbrev.Add("Ether", "Ethernet");
            Abbrev.Add("Chann", "Channel");
            Abbrev.Add("chann", "channel");
            Abbrev.Add("auto", "automatic");
            Abbrev.Add("Auto", "Automatic");
            Abbrev.Add("w/", "with");
            Abbrev.Add("Cann", "Cannot");
            Abbrev.Add("cann", "cannot");
            Abbrev.Add("Conn", "Connect");
            Abbrev.Add("conn", "connect");
            Abbrev.Add("cwp", "Change WiFi Password");
            Abbrev.Add("css", "Change SSID");
            Abbrev.Add("pwcwg", "Power Cycle Wireless Gateway");
            Abbrev.Add("cown", "customer owned");
            Abbrev.Add("ctwni", "Connected to WiFi, No Internet");
            Abbrev.Add("@gm", "@gmail.com");
            Abbrev.Add("@yah", "@yahoo.com");
            Abbrev.Add("@outl", "@outlook.com");
            Abbrev.Add("@hotm", "@hotmail.com");
            Abbrev.Add("xfer", "transfer");
            Abbrev.Add("Xfer", "Transfer");
        }
        private void SaveData(bool AllData)
        {
            Properties.Settings.Default.Email = Email.Text;
            Properties.Settings.Default.CxName = Cx.Text;
            Properties.Settings.Default.CxTicket = Ticket.Text;
            Properties.Settings.Default.CxAccount = Account.Text;
            Properties.Settings.Default.CxChronic = rbChronic.IsChecked.Value;
            Properties.Settings.Default.CxAttemptNum = Attempt.Text;
            Properties.Settings.Default.CxPhone = Contact.Text;
            Properties.Settings.Default.CxAffected = Affected.Text;
            Properties.Settings.Default.CxIssue = Issue.Text;
            Properties.Settings.Default.CxResolution = Resolve.Text;
            Properties.Settings.Default.CxNextAction = NextAction.Text;
            Properties.Settings.Default.Notes = CallNotes.Text;
            Properties.Settings.Default.TechPM = cbPM.IsChecked.Value;
            Properties.Settings.Default.ContactRaw = ContactNumberRaw;

            if (AllData == true)
            {
                Properties.Settings.Default.Technicolor = this.podTechnicolor.Text;
                Properties.Settings.Default.Arris = this.podArris.Text;
                Properties.Settings.Default.Cisco = this.podCisco.Text;
                Properties.Settings.Default.Dory = this.podDory.Text;
                Properties.Settings.Default.SMC = this.podSMC.Text;
                Properties.Settings.Default.Break1 = this.Break1.shortTime;
                Properties.Settings.Default.Break2 = this.Break2.shortTime;
                Properties.Settings.Default.Break3 = this.Break3.shortTime;
                Properties.Settings.Default.Lunch = this.Lunch.shortTime;
                Properties.Settings.Default.b1Armed = Break1.IsArmed;
                Properties.Settings.Default.b2Armed = Break2.IsArmed;
                Properties.Settings.Default.b3Armed = Break3.IsArmed;
                Properties.Settings.Default.blArmed = Lunch.IsArmed;
                Properties.Settings.Default.b1Tripped = Break1.IsTripped;
                Properties.Settings.Default.b2Tripped = Break2.IsTripped;
                Properties.Settings.Default.b3Tripped = Break3.IsTripped;
                Properties.Settings.Default.blTripped = Lunch.IsTripped;

            }
            Properties.Settings.Default.Save();
        }
        private void SaveData_Load(bool AllData)
        {
            Properties.Settings.Default.Upgrade();
            this.Email.Text = Properties.Settings.Default.Email;
            this.Cx.Text = Properties.Settings.Default.CxName;
            this.Ticket.Text = Properties.Settings.Default.CxTicket;
            this.Account.Text = Properties.Settings.Default.CxAccount;
            this.rbChronic.IsChecked = Properties.Settings.Default.CxChronic;
            this.Attempt.Text = Properties.Settings.Default.CxAttemptNum;
            this.Contact.Text = Properties.Settings.Default.CxPhone;
            this.Affected.Text = Properties.Settings.Default.CxAffected;
            this.Issue.Text = Properties.Settings.Default.CxIssue;
            this.Resolve.Text = Properties.Settings.Default.CxResolution;
            this.NextAction.Text = Properties.Settings.Default.CxNextAction;
            this.CallNotes.Text = Properties.Settings.Default.Notes;
            this.cbPM.IsChecked = Properties.Settings.Default.TechPM;
            this.ContactNumberRaw = Properties.Settings.Default.ContactRaw;
            if (AllData == true)
            {
                this.podTechnicolor.Text = Properties.Settings.Default.Technicolor;
                this.podArris.Text = Properties.Settings.Default.Arris;
                this.podCisco.Text = Properties.Settings.Default.Cisco;
                this.podDory.Text = Properties.Settings.Default.Dory;
                this.podSMC.Text = Properties.Settings.Default.SMC;
                this.tbBreak1.Text = Properties.Settings.Default.Break1;
                this.tbBreak2.Text = Properties.Settings.Default.Break2;
                this.tbBreak3.Text = Properties.Settings.Default.Break3;
                this.tbLunch.Text = Properties.Settings.Default.Lunch;
                this.Break1toggle.IsChecked = Properties.Settings.Default.b1Armed;
                this.Break2toggle.IsChecked = Properties.Settings.Default.b2Armed;
                this.Break3toggle.IsChecked = Properties.Settings.Default.b3Armed;
                this.Lunchtoggle.IsChecked = Properties.Settings.Default.blArmed;
                this.Break1.setAlarm = Properties.Settings.Default.Break1;
                this.Break2.setAlarm = Properties.Settings.Default.Break2;
                this.Break3.setAlarm = Properties.Settings.Default.Break3;
                this.Lunch.setAlarm = Properties.Settings.Default.Lunch;
                
            }
        }
        private void AutoReplace(TextBox txt)
        {
            string GotLastWord = "";
            string ReplacedWord = "";
            int lastWord = txt.Text.LastIndexOf(" ");
            lastWord = lastWord + 1;
            GotLastWord = txt.Text.Substring(lastWord);

            if (Abbrev.ContainsKey(GotLastWord))
            {
                ReplacedWord = GotLastWord.Replace(GotLastWord, Abbrev[GotLastWord]);
                txt.Text = txt.Text.Replace(GotLastWord, ReplacedWord);
                txt.SelectionStart = txt.Text.Length;
                txt.GetRectFromCharacterIndex(txt.CaretIndex);
            }
        }
     
        private void disableCombo(ComboBox cmb, bool disable)
        {
            if (disable == true)
            {
                cmb.IsHitTestVisible = false;
                cmb.Focusable = false;
            }
            else
            {
                cmb.IsHitTestVisible = true;
                cmb.Focusable = false;
            }
        }
        private void _ComboVisible(ComboBox cmb)
        {
            cmb.Visibility = System.Windows.Visibility.Visible;
        }
        private void _ComboHide(ComboBox cmb)
        {
            cmb.Visibility = System.Windows.Visibility.Collapsed;
        }
        private void ComboLoad(ComboBox cmb, string list)
        {
            cmb.Items.Clear();
            int i = 0;
            foreach (DataRow Row in RemedyCodes.Rows)
            {
                if (string.IsNullOrWhiteSpace(RemedyCodes.Rows[i][list].ToString()) == true)
                {
                    break;
                }
                cmb.Items.Add(RemedyCodes.Rows[i][list]).ToString();
                i++;
            }
        }

        private void getLabel(ComboBox cmb, string list, Label lb, TextBlock tb, string stb)
        {
            if (cmb.SelectedIndex != -1)
            {
                lb.Content = RemedyCodes.Rows[cmb.SelectedIndex][list].ToString();
                tb.Visibility = Visibility.Visible;
                tb.Text = stb + lb.Content.ToString();
            }
        }
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            bool peak = true;
            curTime.Content = DateTime.Now.ToString("T");
            GetAlarms();
         }
        private void GetAlarms()
        {
            string TimeNow = DateTime.Now.ToString("T");
            
            switch (Break1.IsArmed)
            {
                case true:
                    {
                        Break1.setTime = TimeNow;
                        if (Break1.IsTripped)
                        {
                            YesNoBox("Time to take your First Break!", "First Break Alarm", MessageBoxImage.Exclamation);
                            Break1.IsArmed = false;
                            Break1.Done = true;
                        }
                        else
                        {
                            CurrentBreak.Text = "First Break @ " + Break1.shortTime;
                            BreakIcon.Source = Coffee;
                        }
                        break;
                    }
                case false:
                    switch (Lunch.IsArmed)
                    {
                        case true:
                            {
                                Lunch.setTime = TimeNow;
                                if (Lunch.IsTripped)
                                {
                                    YesNoBox("Time to take your Lunch!", "Lunch Break Alarm", MessageBoxImage.Exclamation);
                                    Lunch.IsArmed = false;
                                    Lunch.Done = true;
                                }
                                else
                                {
                                    if (Break1.Done == true)
                                    {
                                        CurrentBreak.Text = "Lunch Break @ " + Lunch.shortTime;
                                        BreakIcon.Source = Hamburger;
                                    }
                                }
                                break;
                            }
                        case false:
                            {
                                switch (Break2.IsArmed)
                                {
                                    case true:
                                        {
                                            Break2.setTime = TimeNow;
                                            if (Break2.IsTripped)
                                            {
                                                if (cb3rdBreak.IsChecked == false)
                                                {
                                                    YesNoBox("Time to take your Last Break!", "Last Break Alarm", MessageBoxImage.Exclamation);
                                                }
                                                else
                                                {
                                                    YesNoBox("Time to take your Second Break!", "Second Break Alarm", MessageBoxImage.Exclamation);
                                                }
                                                Break2.IsArmed = false;
                                                Break2.Done = true;
                                            }
                                            else
                                            {
                                                if(cb3rdBreak.IsChecked == false)
                                                {
                                                    CurrentBreak.Text = "Last Break @ " +Break2.shortTime;
                                                    
                                                }
                                                else
                                                {
                                                    CurrentBreak.Text = "Second Break @ " + Break2.shortTime;
                                                    
                                                }
                                                BreakIcon.Source = Coffee;
                                            }
                                            break;                                         
                                        }
                                    case false:
                                        {
                                            switch (Break3.IsArmed)
                                            {
                                                case true:
                                                    {
                                                        Break3.setTime = TimeNow;
                                                        if (Break2.IsTripped)
                                                        {
                                                            YesNoBox("Time to take your Last Break!", "Last Break Alarm", MessageBoxImage.Exclamation);
                                                            Break3.IsArmed = false;
                                                            Break3.Done = true;
                                                        }
                                                        else
                                                        {
                                                            CurrentBreak.Text = Break3.shortTime;
                                                            BreakIcon.Source = Coffee;
                                                        }
                                                        break;
                                                    }
                                            }
                                            break;
                                        }


                                }
                                break;
                            }

                    }
                    break;
            }
            
           
        }
          
        private void Email_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Email.Text == sEmail)
            {
                Email.FontStyle = FontStyles.Normal;
                Email.Clear();
            }
            CloseExpanders();
        }
        private void Email_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Email.Text))
            {
                Email.FontStyle = FontStyles.Italic;
                Email.Text = sEmail;
            }
        }
        private void Ticket_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Ticket.Text == sTicket)
            {
                Ticket.FontStyle = FontStyles.Normal;
                Ticket.Clear();
            }
            CloseExpanders();
        }
        private void Ticket_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Ticket.Text))
            {
                Ticket.FontStyle = FontStyles.Italic;
                Ticket.Text = sTicket;
            }
        }
        private void Cx_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Cx.Text == sCx)
            {
                Cx.FontStyle = FontStyles.Normal;
                Cx.Clear();
            }
            CloseExpanders();
        }
        private void Cx_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Cx.Text))
            {
                Cx.FontStyle = FontStyles.Italic;
                Cx.Text = sCx;
            }
        }
        private void Account_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Account.Text == sAccount)
            {
                Account.FontStyle = FontStyles.Normal;
                Account.Clear();
            }
            CloseExpanders();
        }
        private void Account_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Account.Text))
            {
                Account.FontStyle = FontStyles.Italic;
                Account.Text = AHTbuddy.AccountNumber;
            }
        }
        private void Contact_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Contact.Text == sContact)
            {
                Contact.FontStyle = FontStyles.Normal;
                Contact.Clear();
            }
            CloseExpanders();
        }

        private void Contact_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Contact.Text))
            {
                Contact.FontStyle = FontStyles.Italic;
                Contact.Text = sContact;
            }
            var NumbersOnly = Regex.Replace(Contact.Text, @"\D", "");
            if (NumbersOnly != "")
            {
                ContactNumberRaw = NumbersOnly.ToString();
                Contact.Text = Convert.ToInt64(NumbersOnly).ToString("(###) ###-####");
                return;
            }
        }
        private void Affected_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Affected.Text == sAffected)
            {
                Affected.FontStyle = FontStyles.Normal;
                Affected.Clear();
            }
            CloseExpanders();
        }
        private void Affected_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Affected.Text))
            {
                Affected.FontStyle = FontStyles.Italic;
                Affected.Text = sAffected;
            }
            CloseExpanders();
        }
        private void Issue_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Issue.Text == sIssue)
            {
                Issue.FontStyle = FontStyles.Normal;
                Issue.Clear();
            }
            CloseExpanders();
        }
        private void Issue_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Issue.Text))
            {
                Issue.FontStyle = FontStyles.Italic;
                Issue.Text = sIssue;
            }
        }
        private void Resolve_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Resolve.Text == sResolve)
            {
                Resolve.FontStyle = FontStyles.Normal;
                Resolve.Clear();
            }
            CloseExpanders();
        }
        private void Resolve_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Resolve.Text))
            {
                Resolve.FontStyle = FontStyles.Italic;
                Resolve.Text = sResolve;
            }
        }
        private void Issue_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (bSpace == false)
            {
                AutoReplace(Issue);
            }
            
        }
        private void Resolve_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (bSpace == false)
            {
                AutoReplace(Resolve);
            }
        }
        private void GetLastWord(RichTextBox rtb)
        {
            TextPointer endText = rtb.CaretPosition;
            
        }
        private void comboProblem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            pcLabel.Visibility = System.Windows.Visibility.Collapsed;
            ccLabel.Visibility = System.Windows.Visibility.Visible;
            scLabel.Visibility = System.Windows.Visibility.Visible;

            PC.Visibility = Visibility.Visible;
         


            getLabel(comboProblem, "nPC", tbProblem, PC, "PC - ");

            int sVal = int.Parse(tbProblem.Content.ToString());
            curCombo = string.Empty;
            tbCause.Content = "";
            tbSolution.Content = "";

            if (sVal == 15003 | sVal == 15002 | sVal == 15001 | sVal == 15006 | sVal == 15005 | sVal == 15004 | sVal == 15567)
            {
                curCombo = "t1500x";
                ComboLoad(comboCause, curCombo);
            }
            else if (sVal == 1126)
            {
                curCombo = "t1126";
                ComboLoad(comboCause, curCombo);
            }
            else if (sVal == 14179)
            {
                curCombo = "t14179";
                ComboLoad(comboCause, curCombo);
            }
            else if (sVal == 15730)
            {
                curCombo = "t15730";
                ComboLoad(comboCause, curCombo);
            }
            else if (sVal == 15731)
            {
                curCombo = "t15731";
                ComboLoad(comboCause, curCombo);
            }
            else if (sVal == 1129)
            {
                curCombo = "t1129";
                ComboLoad(comboCause, curCombo);
            }
            else if (sVal == 14194 | sVal == 14966)
            {
                curCombo = "t14194";
                ComboLoad(comboCause, curCombo);
            }
            else if (sVal == 16176)
            {
                curCombo = "t16176";
                ComboLoad(comboCause, curCombo);
            }
        }
        private void pcLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            pcLabel.Visibility = System.Windows.Visibility.Collapsed;
            comboProblem.IsDropDownOpen = true;
        }
        private void pcLabel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (comboProblem.SelectedIndex == -1)
            {
                pcLabel.Visibility = System.Windows.Visibility.Visible;
            }
        }
        private void pcLabel_MouseLeave(object sender, MouseEventArgs e)
        {
            if (comboProblem.SelectedIndex == -1)
            {
                pcLabel.Visibility = System.Windows.Visibility.Visible;
            }
        }
        private void comboCause_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ccLabel.Visibility = System.Windows.Visibility.Collapsed;
            CC.Visibility = Visibility.Visible;
            curCombo = curCombo.Replace('t', 'n');
            getLabel(comboCause, curCombo, tbCause, CC, "CC - ");
            comboSolution.Items.Clear();
            scLabel.Visibility = System.Windows.Visibility.Collapsed;
            disableCombo(comboSolution, true);

            switch (tbCause.Content.ToString())
            {
                case "165":
                    comboSolution.Items.Add("Cx Education Hardware/Software");
                    comboSolution.SelectedIndex = 0;
                    tbSolution.Content = "2804";
                    break;
                case "1559":
                    comboSolution.Items.Add("Reconfigure WiFi Security Settings");
                    comboSolution.SelectedIndex = 0;
                    tbSolution.Content = "6045";
                    break;
                case "347":
                    disableCombo(comboSolution, false);
                    scLabel.Visibility = System.Windows.Visibility.Visible;
                    ComboLoad(comboSolution, "t347");
                    comboSolution.SelectedIndex = 0;
                    break;
                case "1083":
                    comboSolution.Items.Add("TR Cancelled by Phone");
                    tbSolution.Content = "2901";
                    comboSolution.SelectedIndex = 0;
                    break;
                case "825":
                    comboSolution.Items.Add("Premise Truck Roll");
                    tbSolution.Content = "9797";
                    comboSolution.SelectedIndex = 0;
                    break;
                case "123":

                    if (tbProblem.Content.ToString() == "15731" | tbProblem.Content.ToString() == "1129")
                    {
                        comboSolution.Items.Add("Reconfigure WiFi Security Settings");
                        tbSolution.Content = "6045";
                    }
                    else
                    {
                        comboSolution.Items.Add("Toggled Bridge Mode");
                        tbSolution.Content = "6092";
                    }

                    comboSolution.SelectedIndex = 0;
                    break;
            }
            if(tbCause.Content.ToString()!= "347")
            {
                SC.Visibility = Visibility.Visible;
                SC.Text = "SC - " + tbSolution.Content.ToString();
            }
            else
            {
               
            }

        }
        private void comboSolution_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            scLabel.Visibility = System.Windows.Visibility.Collapsed;
            if (tbCause.Content.ToString() == "347")
            {
                getLabel(comboSolution, "n347", tbSolution,SC, "SC - ");
            }
        }
        private void Resolve_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                bSpace = true;
            }
            else
            {
                bSpace = false;
            }
        }
        private void Issue_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                bSpace = true;
            }
            else
            {
                bSpace = false;
            }
        }
        private void Issue_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }
        private void Email_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (bSpace == false)
            {
                AutoReplace(Email);
            }
        }
        private void Email_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                bSpace = true;
            }
            else
            {
                bSpace = false;
            }
        }
        private void b_Technicolor_Click(object sender, RoutedEventArgs e)
        {
            GetPOD(podTechnicolor.Text);
        }
        private void b_Arris_Click(object sender, RoutedEventArgs e)
        {
            GetPOD(podArris.Text);
        }
        private void b_Cisco_Click(object sender, RoutedEventArgs e)
        {
            GetPOD(podCisco.Text);
        }
        private void b_Dory_Click(object sender, RoutedEventArgs e)
        {
            GetPOD(podDory.Text);
        }
        private void b_SMC_Click(object sender, RoutedEventArgs e)
        {
            GetPOD(podSMC.Text);
        }
        private void bRemedy_Click(object sender, RoutedEventArgs e)
        {
            //Copy Cx data and apply Format Template
            DateShort = DateTime.Now.ToString("MM/dd/yyyy");

            string Yes = "No";
            if (rbChronic.IsChecked == true) { Yes = "Yes"; };
            string formatNotes = "Date: {0}\n" +
                               "Ticket#: {1}\n" +
                               "Customer Name: {2}\n" +
                               "Account Number: {3}\n" +
                               "Chronic Account: {4}\n" +
                               "Attempt Number: {5}\n" +
                               "Contact Number: {6}\n" +
                               "Affected Device: {7}\n" +
                               "Reported Issue: {8}\n" +
                               "Steps Taken to Identify and Resolve: {9}\n" +
                               "Next Action: {10}\n";
            string AttemptNumber = Attempt.Text;
            if (AttemptNumber == sAttempt)
            {
                AttemptNumber = "1";
            }
            string Result = string.Format(formatNotes, DateShort, Ticket.Text, Cx.Text, Account.Text, Yes,
                                          AttemptNumber, Contact.Text, Affected.Text, Issue.Text,
                                          Resolve.Text, NextAction.Text);
            Clipboard.SetText(Result);
        }
        private void bWDT_Click(object sender, RoutedEventArgs e)
        {
            string WDTNotes = Cx.Text + "/" + Issue.Text + "/" + Resolve.Text + "/" + Ticket.Text;
            Clipboard.SetText(WDTNotes);
        }
        private void podTechnicolor_DragEnter(object sender, DragEventArgs e)
        {
            TextBox txt = (TextBox)sender;
            txt.Clear();
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
                e.Effects = DragDropEffects.None;
        }
        private void podTechnicolor_Drop(object sender, DragEventArgs e)
        {
            TextBox txt = (TextBox)sender;
            txt.Clear();
            txt.Text = txt.Text + e.Data.GetData(DataFormats.Text).ToString();
        }
        private void bNextCall_Click(object sender, RoutedEventArgs e)
        {

            SaveData(false);

            rbChronic.IsChecked = false;
            Attempt.Clear();
            Email.Clear();
            Ticket.Clear();
            Cx.Clear();
            Account.Clear();
            Contact.Clear();
            Affected.Clear();
            Issue.Clear();
            Resolve.Clear();
            NextAction.Clear();

            Attempt.Text = sAttempt;
            Email.Text = sEmail;
            Ticket.Text = sTicket;
            Cx.Text = sCx;
            Account.Text = sAccount;
            Contact.Text = sContact;
            Affected.Text = sAffected;
            Issue.Text = sIssue;
            Resolve.Text = sResolve;
            NextAction.Text = sNext;

            comboProblem.Items.Clear();
            comboCause.Items.Clear();
            comboSolution.Items.Clear();

            pcLabel.Visibility = System.Windows.Visibility.Visible;
            ccLabel.Visibility = System.Windows.Visibility.Visible;
            scLabel.Visibility = System.Windows.Visibility.Visible;
            ComboLoad(comboProblem, "tPC");

            tbProblem.Content = "";
            tbCause.Content = "";
            tbSolution.Content = "";
            PC.Text = "";
            CC.Text = "";
            SC.Text = "";

        }
        private void NextAction_GotFocus(object sender, RoutedEventArgs e)
        {
            if (NextAction.Text == sNext)
            {
                NextAction.FontStyle = FontStyles.Normal;
                NextAction.Clear();
            }
        }
        private void NextAction_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(NextAction.Text))
            {
                NextAction.FontStyle = FontStyles.Italic;
                NextAction.Text = sNext;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ResetAllAlarms_Click(object sender, RoutedEventArgs e)
        {
            YesNoBox("Reset All Alarms?", "Reset Alarms", MessageBoxImage.Question);

        }

        private void Break1toggle_Checked(object sender, RoutedEventArgs e)
        {
            CurrentBreak.Visibility = System.Windows.Visibility.Visible;
            BreakIcon.Visibility = System.Windows.Visibility.Visible;
            BreakIcon.Source = Coffee;
            Break1.IsArmed = true;
            Break1.setAlarm = string.Format(sAlarmTime, sliderHH, sliderMM, sliderTT);
            tbBreak1.Text = string.Format(sTextBoxTime, sliderHH, sliderMM, sliderTT);
            Break1.shortTime = tbBreak1.Text;          
            
            if (Break1.IsArmed) { lightBreak1.Fill = new SolidColorBrush(color: Colors.Green); }
        }

        private void Break1toggle_Unchecked(object sender, RoutedEventArgs e)
        {
            Break1.IsArmed = false;
            Break1.Done = false;
            if (Break1.IsArmed == false) { lightBreak1.Fill = new SolidColorBrush(color: Colors.Red); }
        }

        private void Break2toggle_Checked(object sender, RoutedEventArgs e)
        {
            CurrentBreak.Visibility = System.Windows.Visibility.Visible;
            
            Break2.IsArmed = true;
            tbBreak2.Text= string.Format(sTextBoxTime, sliderHH, sliderMM, sliderTT);
            Break2.setAlarm = string.Format(sAlarmTime, sliderHH, sliderMM, sliderTT);
            Break2.shortTime = tbBreak2.Text;           
            
            if (Break2.IsArmed) { lightBreak2.Fill = new SolidColorBrush(color: Colors.Green); }
        }

        private void Break2toggle_UnChecked (object sender, RoutedEventArgs e)
        {
            Break2.IsArmed = false;
            Break2.Done = false;
            if (Break2.IsArmed == false) { lightBreak2.Fill = new SolidColorBrush(color: Colors.Red); }
        }

        private void Lunchtoggle_Checked(object sender, RoutedEventArgs e)
        {
            CurrentBreak.Visibility = System.Windows.Visibility.Visible;
            Lunch.IsArmed = true;

            tbLunch.Text = string.Format(sTextBoxTime, sliderHH, sliderMM, sliderTT);
            Lunch.setAlarm = string.Format(sAlarmTime, sliderHH, sliderMM, sliderTT);
            Lunch.shortTime = tbLunch.Text;
            
            if (Lunch.IsArmed) { lightLunch.Fill = new SolidColorBrush(color: Colors.Green); }
        }

        private void Lunchtoggle_Unchecked(object sender, RoutedEventArgs e)
        {
            Lunch.IsArmed = false;
            Lunch.Done = false;
            if (Lunch.IsArmed == false) { lightLunch.Fill = new SolidColorBrush(color: Colors.Red); }
        }

        private void Break3toggle_Checked(object sender, RoutedEventArgs e)
        {
            CurrentBreak.Visibility = System.Windows.Visibility.Visible;
            Break3.IsArmed = true;
            tbBreak3.Text = string.Format(sTextBoxTime, sliderHH, sliderMM, sliderTT);
            Break3.setAlarm = string.Format(sAlarmTime, sliderHH, sliderMM, sliderTT);            
            
            Break3.shortTime = tbBreak3.Text;
            if (Break3.IsArmed) { lightBreak3.Fill = new SolidColorBrush(color: Colors.Green); }
        }

        private void Break3toggle_Unchecked(object sender, RoutedEventArgs e)
        {
            Break3.IsArmed = false;
            Break3.Done = false;
            if (Break3.IsArmed == false) { lightBreak3.Fill = new SolidColorBrush(color: Colors.Red); }
        }

        private void window_Closed(object sender, EventArgs e)
        {
            SaveData(true);
        }
        private void LastCall_Click(object sender, RoutedEventArgs e)
        {
            YesNoBox("Retrieve Last Call Data?", "ReKall! ReKall! ReKall!", MessageBoxImage.Question);
        }
        private void PODreset_Click(object sender, RoutedEventArgs e)
        {
            YesNoBox(podRemove, "Reset POD Data", MessageBoxImage.Warning);
        }
        private void r_ticket_MouseEnter(object sender, MouseEventArgs e)
        {
            Rectangle rect = (Rectangle)sender;
            ChangeHighlight(rect, true);
        }
        private void r_ticket_MouseLeave(object sender, MouseEventArgs e)
        {
            Rectangle rect = (Rectangle)sender;
            ChangeHighlight(rect, false);
        }

        private void Email_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TextBox t = (TextBox)sender;
            Clipboard.SetText(t.Text);

        }
        private void CallNotes_GotFocus(object sender, RoutedEventArgs e)
        {
            if (CallNotes.Text == sCallNotes)
            {
                CallNotes.FontStyle = FontStyles.Normal;
                CallNotes.Clear();
            }
        }
        private void CallNotes_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CallNotes.Text))
            {
                CallNotes.FontStyle = FontStyles.Italic;
                CallNotes.Text = sCallNotes;
            }
        }
        private void rbChronic_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            rbChronic.IsChecked = false;
        }


        private void Contact_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (cb91.IsChecked == true)
            {
                Clipboard.SetText("91" + ContactNumberRaw);
            }
            else
            {
                Clipboard.SetText(ContactNumberRaw);
            }
        }

        private void sliderHour_MouseMove(object sender, MouseEventArgs e)
        {
            if (sliderHour.Value >= 12)
            {
                lbHour.Content = sliderHour.Value;
                if (sliderHour.Value > 12)
                {
                    lbHour.Content = sliderHour.Value - 12;
                }

                lbAMPM.Content = "PM";
            }
            else
            {
                lbHour.Content = sliderHour.Value;
                lbAMPM.Content = "AM";
                if(sliderHour.Value == 0)
                {
                    lbHour.Content = "12";
                }
            }
            byte ZeroAdd = byte.Parse(lbHour.Content.ToString());
            sliderHH = lbHour.Content.ToString();
            sliderTT = lbAMPM.Content.ToString();
        }

        private void sliderMinute_MouseMove_1(object sender, MouseEventArgs e)
        {
            if (sliderMinute.Value < 10)
            {
                lbMinute.Content = "0" + sliderMinute.Value;
            }
            else
            {
                lbMinute.Content = sliderMinute.Value;
            }
            sliderMM = lbMinute.Content.ToString();
        }
        private void Break1ON_Checked(object sender, RoutedEventArgs e)
        {        
            lightBreak1.Fill = new SolidColorBrush(color: Colors.Green);
        }
        private void Break1OFF_Checked(object sender, RoutedEventArgs e)
        {
        
            lightBreak1.Fill = new SolidColorBrush(color: Colors.Red);
        }

        private void SetBreak1_Click(object sender, RoutedEventArgs e)
        {
            tbBreak1.Text = "";
            Break1toggle.IsChecked = false;
            Break1.Done = false;

        }
        private void SetBreak2_Click(object sender, RoutedEventArgs e)
        {
            tbBreak2.Text = "";
            Break2toggle.IsChecked = false;
            Break2.Done = false;

        }
        private void SetBreak3_Click(object sender, RoutedEventArgs e)
        {
            tbBreak3.Text = "";
            Break3toggle.IsChecked = false;
            Break3.Done = false;
        }        
        private void SetLunch_Click(object sender, RoutedEventArgs e)
        {
            tbLunch.Text = "";
            Lunchtoggle.IsChecked = false;
            Lunch.Done = false;
        }

        private void Digits_MouseWheel(object sender, MouseWheelEventArgs e)
        {
           
        }

        private void Rectangle_MouseWheel(object sender, MouseWheelEventArgs e)
        {


            System.Windows.Shapes.Path[] DigitPaths = new System.Windows.Shapes.Path[10];
           

            
        }

        void Countdown(int count, TimeSpan interval, Action<int> ts)
        {

        }


        private void popupAutoReplace_Opened(object sender, EventArgs e)
        {
            myTimer.Tick += new EventHandler(MyTimerTick_Tick);
            myTimer.Interval = new TimeSpan(0, 0, 1);
            myTimer.Start();



    }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //popupAutoReplace.IsOpen = true;
            windowTimer wt = new windowTimer();
            wt.Show();
            
        }

        private void cbPM_Checked(object sender, RoutedEventArgs e)
        {
            LinearGradientBrush PMgradientBrush = new LinearGradientBrush();
            PMgradientBrush.StartPoint = new Point(0, 0);
            PMgradientBrush.EndPoint = new Point(0, 1);
            PMgradientBrush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 117, 106, 189), 0.0));
            PMgradientBrush.GradientStops.Add(new GradientStop(Colors.Black, 1.0));
            b_Technicolor.Background = PMgradientBrush;
        }

        private void AutoReplaceDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void cb3rdBreak_Checked(object sender, RoutedEventArgs e)
        {
            lightBreak3.IsEnabled = true;
            tbBreak3.IsEnabled = true;
            Break3toggle.IsEnabled = true;
            Break3Coffee.IsEnabled = true;
        }

        private void cb3rdBreak_Unchecked(object sender, RoutedEventArgs e)
        {
            lightBreak3.IsEnabled = false;
            tbBreak3.IsEnabled = false;
            Break3toggle.IsEnabled = false;
            Break3Coffee.IsEnabled = false;
        }

    }
}


    








